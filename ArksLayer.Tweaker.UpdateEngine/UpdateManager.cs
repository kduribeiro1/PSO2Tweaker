using ArksLayer.Tweaker.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.UpdateEngine
{
    /// <summary>
    /// A class capable of performing update, hash and cleanup tasks against the game client files.
    /// </summary>
    public class UpdateManager
    {
        /// <summary>
        /// Constructs an instance of UpdateManager requiring a tweaker settings class and a UI renderer class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="output"></param>
        public UpdateManager(ITweakerSettings settings, IRenderer output)
        {
            this.Settings = settings;
            this.Output = output;

            // At the moment, the downloader is tightly coupled against this class.
            // In the future, downloader can be decoupled away in the very case where we have PSO2 private server running a patch server.
            this.Downloader = new SegaDownloader(output);
        }

        /// <summary>
        /// Apparently, there are many files that are not listed in the patchlist.
        /// I assume these are legacy files that are not needed anymore and can be safely deleted!
        /// My game client was updated ever since Episode 2: using this method weeds out 1300+ files (1GB).
        /// This method will delete all backup files as well, so backup restore should be performed BEFORE invoking this method.
        /// </summary>
        /// <returns></returns>
        private void CleanLegacyFiles(IList<PatchInfo> patchlist)
        {
            Output.WriteLine("Scanning for legacy files...");

            var gamefiles = EnumerateGameFiles().Select(Q => Q.FileName);

            var requiredFiles = patchlist.Select(Q =>
            {
                var shortpath = Q.File.Replace('/', '\\');
                return Path.Combine(Settings.GameDirectory, shortpath);
            });

            // Only delete files in /data/win32 for safety. 
            // Prevents deleting weird stuffs like GameGuard or Tweaker stuffs

            var legacyFiles = gamefiles.Except(requiredFiles)
                .Where(Q => Q.Contains(@"\data\win32\"))
                .Select(Q => new FileInfo(Q))
                .ToList();

            if (!legacyFiles.Any())
            {
                Output.WriteLine("There are no legacy files to clean up!");
                return;
            }

            var megabytes = legacyFiles.Sum(Q => Q.Length) / 1024 / 1024;
            Output.WriteLine($"Found { legacyFiles.Count } legacy files: {megabytes} MB");

            Parallel.ForEach(legacyFiles, Q =>
            {
                File.Delete(Q.FullName);
                Output.WriteLine($"{Q.FullName} deleted.");
            });
        }

        /// <summary>
        /// Dependency to UI renderer class.
        /// </summary>
        private IRenderer Output { get; set; }

        /// <summary>
        /// Dependency to Tweaker Settings class.
        /// </summary>
        private ITweakerSettings Settings { get; set; }

        /// <summary>
        /// Dependency to patch downloader class.
        /// </summary>
        private SegaDownloader Downloader { get; set; }

        /// <summary>
        /// File name for storing missing files information.
        /// </summary>
        private const string MissingFilesJson = "missing.json";

        /// <summary>
        /// File name for storing patch information.
        /// </summary>
        private const string PatchlistJson = "patchlist.json";

        /// <summary>
        /// File name for storing client hash information.
        /// </summary>
        private const string ClientHashesJson = "client.json";

        /// <summary>
        /// File name for logging files that were successfully downloaded.
        /// </summary>
        private const string DownloadedFilesLog = "done.log";

        /// <summary>
        /// Returns all hashable information of game files that does not contain the word "backup".
        /// File order is sorted for optimal hill-climbing disk buffer and to avoid Branch Predictor failures.
        /// https://en.wikipedia.org/wiki/Branch_predictor
        /// AIDA tends to have folders named "backup" or files with ".backup" extension.
        /// Dammit make up your mind. Be consistent. Why not put all files into backup folder anyway?
        /// </summary>
        /// <returns></returns>
        private IList<HashModel> EnumerateGameFiles()
        {
            return Directory.GetFiles(Settings.GameDirectory, "*.*", SearchOption.AllDirectories)
                .AsParallel()
                .Select(Q => new FileInfo(Q))
                .Where(Q => !Q.DirectoryName.Contains("backup"))
                .Select(Q => new HashModel
                {
                    FileName = Q.FullName,
                    FileSize = Q.Length,
                    BufferSize = GetBufferSize(Q.Length),
                    HashBinary = null
                })
                .OrderBy(Q => Q.FileSize)
                .ToList();
        }

        /// <summary>
        /// Perform a (fast) whole client rehash.
        /// When done, save the hashes into the named JSON file.
        /// </summary>
        /// <returns></returns>
        private async Task<IDictionary<string, string>> RehashWholeClient()
        {
            Output.OnHashStart();

            var gameFiles = EnumerateGameFiles();
            var fileCount = gameFiles.Count();

            // Perform the long-running operation in the background thread instead of the UI thread.
            var progress = 0;
            var hashJob = Task.Run(() =>
            {
                Output.WriteLine($"Discovered { fileCount } files in game directory.");

                // Should probably not use multi-threading here.
                // Hashing is input-bound instead of CPU-bound. Logically should be able to use Async-Await here, but...
                // Unless you're using SSD, parallel read will cause hard drive head to move back and forth trying to read two files at the same time.
                // Hence, files are read and hashed in sequential. Too bad.
                // However, binary hashes can be converted to string later using PLINQ.
                for (progress = 0; progress < fileCount; progress++)
                {
                    var file = gameFiles[progress];
                    try
                    {
                        using (var stream = new FileStream(file.FileName, FileMode.Open, FileAccess.Read, FileShare.Read, file.BufferSize))
                        using (var md5 = MD5.Create())
                        {
                            file.HashBinary = md5.ComputeHash(stream);
                        }
                    }
                    catch (Exception Ex)
                    {
                        Output.AppendLog($"Hashing failed for file {file.FileName} : {Ex.Message}");
                    }
                }

                Output.WriteLine("Converting hash results into a file-hash dictionary...");

                return gameFiles.AsParallel().Where(Q => Q.HashBinary != null).Select(Q =>
                {
                    var key = Q.FileName.Remove(0, Settings.GameDirectory.Length + 1).Replace('\\', '/');
                    var hash = BitConverter.ToString(Q.HashBinary).Replace("-", "").ToLower();

                    return new KeyValuePair<string, string>(key, hash);
                }).ToDictionary(Q => Q.Key, Q => Q.Value);
            });

            var lastProgress = 0;
            do
            {
                await Task.Delay(2 * 1000);
                if (progress > lastProgress)
                {
                    lastProgress = progress;
                    Output.OnHashProgress(lastProgress, fileCount);
                }
            } while (lastProgress < fileCount);

            Output.OnHashComplete();

            var hashes = await hashJob;
            await OverwriteLatestClientJson(hashes);
            return hashes;
        }

        /// <summary>
        /// Calculate a hard disk buffer size for a given file. 
        /// Buffer size block gets increased by multiply of 2, starting from 4KB and up to 4MB.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private int GetBufferSize(long length)
        {
            // Default stream buffer size is 4KB
            if (length < 4096) return 4096;

            if (length < 4096 * 1024)
            {
                // 3000 KB file = 3072000 bytes
                // pow = 22
                var pow = Math.Ceiling(Math.Log(length, 2));

                // 2^16 = 4194304 bytes = 4096 KB
                return (int)Math.Pow(2, pow);
            }

            // Most hard drives have 16MB or 32MB buffer size.
            // We'll take 4MB off that for hashing larger files!
            return 4096 * 1024;
        }

        /// <summary>
        /// Execute a game client update operation.
        /// First parameter can be set true if desiring a full client rehash and hence rebuilding the game client hash file.
        /// Else, will attempt to read from past client hashes, which in turn gets compiled from the latest game client update patchlist.
        /// (A much more elegant solution instead of maintaining your own hashes!)
        /// Second parameter can be set true if desiring a legacy (unneeded) file cleanup, which should be used with caution.
        /// In addition, support download resume in case of interruptions or due to past failure of downloading certain files.
        /// Uncensor the client chat automatically, regardless of the Tweaker settings.
        /// </summary>
        /// <param name="rehash"></param>
        /// <param name="cleanLegacy"></param>
        /// <returns></returns>
        public async Task<bool> Update(bool rehash = false, bool cleanLegacy = true)
        {
            IList<PatchInfo> patchlist;
            IList<PatchInfo> missingFiles;

            if (File.Exists(MissingFilesJson))
            {
                missingFiles = await ResumePatching();
                patchlist = await ReadPatchlistFromJson();
            }
            else
            {
                RestoreBackupFiles();
                patchlist = await Downloader.FetchUpdatePatchlist();

                if (cleanLegacy) CleanLegacyFiles(patchlist);
                var gameFiles = await GetClientHash(rehash);

                missingFiles = await DiscoverMissingPatches(gameFiles, patchlist);
            }

            if (missingFiles.Count > 0)
            {
                var downloads = missingFiles.Select(patch => Downloader.DownloadGamePatch(patch, Settings.GameDirectory, DownloadedFilesLog));
                var failCount = (await Task.WhenAll(downloads)).Count(Q => !Q);

                if (failCount > 0)
                {
                    // Uh-oh, one or more download failed
                    Output.WriteLine($"Failed to download {failCount} files!");

                    return false;
                }
                else
                {
                    Output.WriteLine("Saving the latest client hashes...");
                    await OverwriteLatestClientJson(patchlist.ToDictionary(Q => Q.File, Q => Q.Hash));
                    Output.WriteLine("Update complete!");
                    Housekeeping();

                    return true;
                }
            }
            else
            {
                Output.WriteLine("Your game is up-to-date!");
                Housekeeping();

                return true;
            }
        }

        /// <summary>
        /// Attempts to obtain client hashes, depending on required operation and whether the past hashes are available for read.
        /// </summary>
        /// <param name="rehash"></param>
        /// <returns></returns>
        private async Task<IDictionary<string, string>> GetClientHash(bool rehash)
        {
            if (rehash) return await RehashWholeClient();

            try
            {
                return await ReadLatestClientHash();
            }
            catch
            {
                Output.WriteLine("Unable to read the latest client file hashes!");
                return await RehashWholeClient();
            }
        }

        /// <summary>
        /// Deletes the file that must not exist for the next patching operation and then uncensor the game if not uncensored already.
        /// </summary>
        private void Housekeeping()
        {
            Output.WriteLine("A little housekeeping...");
            if (File.Exists(PatchlistJson)) File.Delete(PatchlistJson);
            if (File.Exists(MissingFilesJson)) File.Delete(MissingFilesJson);
            if (File.Exists(DownloadedFilesLog)) File.Delete(DownloadedFilesLog);

            //Remove Censor
            var censorFile = Path.Combine(DataWin32Directory, "ffbff2ac5b7a7948961212cefd4d402c");
            if (File.Exists(censorFile))
            {
                Output.WriteLine("Uncensoring game chat...");
                File.Delete(censorFile);
            }

            Output.WriteLine("All done!");
        }

        /// <summary>
        /// Read missing files again prior to update interruptions / failure and then read the files that were successfully downloaded previously.
        /// Update the missing files using this information and then return it.
        /// </summary>
        /// <returns></returns>
        private async Task<IList<PatchInfo>> ResumePatching()
        {
            var missingFiles = await ReadMissingFilesFromJson();
            var downloaded = await ReadDownloadedFilesFromLog();

            missingFiles = missingFiles.Where(Q => !downloaded.Contains(Q.File)).ToList();
            await WriteMissingFilesToJson(missingFiles);

            Output.WriteLine($"Resuming patch download of {missingFiles.Count} files...");

            return missingFiles;
        }

        /// <summary>
        /// Returns a sequence of all files that were successfully downloaded in prior game client update attempt.
        /// </summary>
        /// <returns></returns>
        private async Task<IList<string>> ReadDownloadedFilesFromLog()
        {
            if (!File.Exists(DownloadedFilesLog)) return new List<string>();

            using (var file = File.OpenText(DownloadedFilesLog))
            {
                return (await file.ReadToEndAsync()).Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        /// <summary>
        /// Write the sequence of missing files to the named JSON file.
        /// </summary>
        /// <param name="missingFiles"></param>
        /// <returns></returns>
        private async Task WriteMissingFilesToJson(IList<PatchInfo> missingFiles)
        {
            if (missingFiles.Any())
            {
                using (var file = File.CreateText(MissingFilesJson))
                {
                    await file.WriteAsync(JsonConvert.SerializeObject(missingFiles));
                }
            }
        }

        /// <summary>
        /// Read the sequence of missing files from the named JSON file.
        /// </summary>
        /// <returns></returns>
        private async Task<IList<PatchInfo>> ReadMissingFilesFromJson()
        {
            using (var file = File.OpenText(MissingFilesJson))
            {
                var json = await file.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<PatchInfo>>(json);
            }
        }

        /// <summary>
        /// Read the sequence of patch information from the named JSON file.
        /// </summary>
        /// <returns></returns>
        private async Task<IList<PatchInfo>> ReadPatchlistFromJson()
        {
            //Output.WriteLine("Reading last downloaded patchlist...");
            using (var file = File.OpenText(PatchlistJson))
            {
                var json = await file.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<PatchInfo>>(json);
            }
        }

        /// <summary>
        /// Gets the known, usual backup directory for the legacy Tweaker functions.
        /// </summary>
        public string BackupDirectory
        {
            get
            {
                return DataWin32Directory + @"\backup";
            }
        }

        /// <summary>
        /// Gets the game file directory where the game asset files usually located in.
        /// </summary>
        public string DataWin32Directory
        {
            get
            {
                return Settings.GameDirectory + @"\data\win32";
            }
        }

        /// <summary>
        /// Restore all backups in the backup directory to the assets folder, indiscriminately.
        /// </summary>
        public void RestoreBackupFiles()
        {
            if (!Directory.Exists(BackupDirectory)) return;

            var gameFiles = Directory.GetFiles(BackupDirectory, "*.*", SearchOption.AllDirectories).ToList();
            if (!gameFiles.Any()) return;

            Output.WriteLine($"Found {gameFiles.Count()} files in backup directory. Restoring...");

            foreach (var file in gameFiles)
            {
                var fileName = Path.GetFileName(file);
                var target = Path.Combine(DataWin32Directory, fileName);

                if (File.Exists(target)) File.Delete(target);
                Output.AppendLog($"Moving backup from {file} to {target}");
                File.Move(file, target);
            }

            Output.WriteLine("Backup restore complete!");
        }

        /// <summary>
        /// Create a named JSON file containing file-MD5 hashes dictionary.
        /// </summary>
        /// <param name="dictionary"></param>
        private async Task OverwriteLatestClientJson(IDictionary<string, string> dictionary)
        {
            using (var file = File.CreateText(ClientHashesJson))
            {
                await file.WriteAsync(JsonConvert.SerializeObject(dictionary));
            }
        }

        /// <summary>
        /// Compares the patchlist to the game client hashes and discover whether there are missing files that require downloading.
        /// </summary>
        /// <param name="gameFiles"></param>
        /// <param name="patchlist"></param>
        /// <returns></returns>
        private async Task<IList<PatchInfo>> DiscoverMissingPatches(IDictionary<string, string> gameFiles, IList<PatchInfo> patchlist)
        {
            var patchDictionary = patchlist.ToDictionary(Q => Q.File, Q => Q);

            Output.WriteLine($"Discovering missing files...");
            var missingFiles = patchDictionary.AsParallel()
                .Select(Q =>
                {
                    string hash;

                    var isFound = gameFiles.TryGetValue(Q.Key, out hash);
                    if (!isFound) return Q.Value;

                    var isEqual = hash.Equals(Q.Value.Hash, StringComparison.InvariantCultureIgnoreCase);
                    return isEqual ? null : Q.Value;
                })
                .Where(Q => Q != null)
                .ToList();

            Output.WriteLine($"Found {missingFiles.Count} missing files!");
            await WriteMissingFilesToJson(missingFiles);

            return missingFiles;
        }

        /// <summary>
        /// Read latest game client hash dictionary.
        /// </summary>
        /// <returns></returns>
        private async Task<IDictionary<string, string>> ReadLatestClientHash()
        {
            Output.WriteLine($"Searching for latest game client hashes...");

            using (var file = File.OpenText(ClientHashesJson))
            {
                var client = await file.ReadToEndAsync();
                var hashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(client);

                Output.WriteLine($"Game client hashes found!");
                return hashes;
            }
        }
    }
}
