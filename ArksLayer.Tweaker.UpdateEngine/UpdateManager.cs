using ArksLayer.Tweaker.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        /// Lock object for download success log file.
        /// </summary>
        internal static object DownloadSuccessLogLock = new object();

        /// <summary>
        /// File name for storing client hash information.
        /// </summary>
        private const string ClientHashesJson = "client.json";

        /// <summary>
        /// File name for logging files that were successfully downloaded.
        /// </summary>
        private const string DownloadSuccessLog = "done.log";

        /// <summary>
        /// File name for storing missing files information.
        /// </summary>
        private const string MissingFilesJson = "missing.json";

        /// <summary>
        /// File name for storing patch information.
        /// </summary>
        private const string PatchlistJson = "patchlist.json";

        /// <summary>
        /// Constructs an instance of UpdateManager requiring a tweaker settings class and a UI renderer class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="output"></param>
        public UpdateManager(ITweakerSettings settings, ITrigger output)
        {
            this.Settings = settings;
            this.Output = output;

            // At the moment, the downloader is tightly coupled against this class.
            // In the future, downloader can be decoupled away in the very case where we have PSO2 private server running a patch server.
            this.Downloader = new SegaDownloader(output);
        }

        /// <summary>
        /// Gets the known, usual backup directory for the legacy Tweaker functions.
        /// </summary>
        public string BackupDirectory
        {
            get
            {
                return Path.Combine(DataWin32Directory, "backup");
            }
        }

        /// <summary>
        /// Gets the game file directory where the game asset files usually located in.
        /// </summary>
        public string DataWin32Directory
        {
            get
            {
                return Path.Combine(Settings.GameDirectory, @"data\win32");
            }
        }

        /// <summary>
        /// Dependency to patch downloader class.
        /// </summary>
        private SegaDownloader Downloader { get; set; }

        /// <summary>
        /// Dependency to UI renderer class.
        /// </summary>
        private ITrigger Output { get; set; }

        /// <summary>
        /// Dependency to Tweaker Settings class.
        /// </summary>
        private ITweakerSettings Settings { get; set; }

        /// <summary>
        /// Throws an exception on-demand from within the engine.
        /// Might be useful if you're trying to develop a working exception handler for the engine that doesn't say Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs
        /// </summary>
        public async Task Crash(int type)
        {
            switch (type)
            {
                case 1:
                    {
                        throw new Exception("An exception happened synchronously.");
                    }
                case 2:
                    {
                        await Task.Run(() =>
                        {
                            throw new Exception("An exception happened asynchronously.");
                        });
                        break;
                    }
                case 3:
                    {
                        Parallel.For(1, 4, i =>
                        {
                            throw new Exception("An exception happened in a parallel loop.");
                        });
                        break;
                    }
                case 4:
                    {
                        IEnumerable<int> faux = new int[] { 1, 2, 3, 4 };
                        faux = faux.AsParallel().Select(Q =>
                        {
                            throw new Exception("An exception happened in a PLINQ");
                            return Q;
                        });
                        break;
                    }
            }
        }

        /// <summary>
        /// Deletes the file that must not exist for the next patching operation and then uncensor the game if not uncensored already.
        /// </summary>
        public Task Housekeeping()
        {
            Output.OnHousekeeping();
            return Task.Run(() =>
            {
                if (File.Exists(PatchlistJson)) File.Delete(PatchlistJson);
                if (File.Exists(MissingFilesJson)) File.Delete(MissingFilesJson);
                if (File.Exists(DownloadSuccessLog)) File.Delete(DownloadSuccessLog);

                //Remove Censor
                var censorFile = Path.Combine(DataWin32Directory, "ffbff2ac5b7a7948961212cefd4d402c");
                if (File.Exists(censorFile))
                {
                    Output.OnCensorRemoval();
                    File.Delete(censorFile);
                }
            });
        }

        /// <summary>
        /// Restore all backups in the backup directory to the assets folder, indiscriminately.
        /// </summary>
        public async Task RestoreBackupFiles()
        {
            if (!Directory.Exists(BackupDirectory)) return;

            var backupFiles = await Task.Run(() => Directory.GetFiles(BackupDirectory, "*.*", SearchOption.AllDirectories));
            if (!backupFiles.Any()) return;

            Output.OnBackupRestore(backupFiles);
            await Task.Run(() =>
            {
                foreach (var file in backupFiles)
                {
                    var fileName = Path.GetFileName(file);
                    var target = Path.Combine(DataWin32Directory, fileName);

                    if (File.Exists(target)) File.Delete(target);
                    File.Move(file, target);
                }
            });

            // Why the fuck are we using hard-coded string as enum values? This is stupid
            Settings.EnglishPatchVersion = "Not Installed";
            Settings.EnglishLargePatchVersion = "Not Installed";
            Settings.StoryPatchVersion = "Not Installed";
        }

        /// <summary>
        /// Execute a game client update operation.
        /// Supports download resume in case of interruptions or due to past failure of downloading certain files.
        /// Uncensor the client chat automatically, regardless of the Tweaker settings.
        /// </summary>
        /// <param name="rehash">Can be set true if desiring a full client rehash and hence rebuilding the game client hash file.
        /// Else will attempt to read from past client hashes, which in turn gets compiled from the latest game client update patchlist.
        /// (A much more elegant solution instead of maintaining your own hashes!)</param>
        /// <param name="cleanLegacy">Can be set true if desiring legacy files purging.</param>
        /// <returns>True if patching is successful. False otherwise.</returns>
        public async Task Update(bool rehash, bool cleanLegacy)
        {
            var remoteVersion = Downloader.GetRemoteVersion();
            Output.OnUpdateStart(rehash);

            IList<PatchInfo> patchlist;
            IList<PatchInfo> missingFiles;

            if (File.Exists(MissingFilesJson))
            {
                missingFiles = await ResumePatching();
                patchlist = await ReadPatchlistFromJson();
            }
            else
            {
                var patchlistDownload = Downloader.FetchUpdatePatchlist();

                await RestoreBackupFiles();
                var gameFiles = await Task.Run(() => EnumerateGameFiles());
                patchlist = await patchlistDownload;

                if (cleanLegacy)
                {
                    await CleanLegacyFiles(patchlist, gameFiles);
                }
                var gameHashes = await GetClientHash(rehash);

                missingFiles = await DiscoverMissingPatches(gameHashes, patchlist);
                if (File.Exists(DownloadSuccessLog))
                {
                    File.Delete(DownloadSuccessLog);
                }
            }

            if (missingFiles.Count > 0)
            {
                int failCount = await TryUpdateGame(missingFiles);
                if (failCount > 0)
                {
                    // Uh-oh, one or more download failed
                    Output.OnPatchingFailed(failCount);
                    return;
                }

                Output.AppendLog("Saving the latest client hashes...");
                await OverwriteLatestClientJson(patchlist.ToDictionary(Q => Q.File, Q => Q.Hash));
                Output.OnPatchingSuccess();
                await Housekeeping();
            }
            else
            {
                Output.IfUpdateNotNeeded();
                await Housekeeping();
            }

            Settings.GameVersion = await remoteVersion;
            Output.OnUpdateCompleted();
        }

        /// <summary>
        /// Checks if game client version equals to the remote update version.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsGameUpToDate()
        {
            return await Downloader.GetRemoteVersion() == Settings.GameVersion;
        }

        /// <summary>
        /// Apparently, there are many files that are not listed in the patchlist.
        /// I assume these are legacy files that are not needed anymore and can be safely deleted!
        /// My game client was updated ever since Episode 2: using this method weeds out 1300+ files (1GB).
        /// </summary>
        /// <returns></returns>
        private async Task CleanLegacyFiles(IList<PatchInfo> patchlist, IList<string> gameFiles)
        {
            Output.OnLegacyFilesScanning();

            var requiredFiles = patchlist.AsParallel().Select(Q =>
            {
                var shortpath = Q.File.Replace('/', '\\');
                return Path.Combine(Settings.GameDirectory, shortpath);
            });

            // Only delete files in /data/win32 for safety. Prevents deleting weird stuffs like GameGuard or Tweaker stuffs.
            // EnumerateGameFiles is only sending out files in /data/win32 and several whitelisted files in pso2_bin.

            var legacyFiles = gameFiles
                .AsParallel()
                .Except(requiredFiles)
                .ToList();

            if (!legacyFiles.Any())
            {
                Output.OnLegacyFilesNotFound();
                return;
            }

            Output.OnLegacyFilesFound(legacyFiles);
            await Task.Run(() =>
            {
                foreach (var file in legacyFiles)
                {
                    File.Delete(file);
                }
            });
        }
        /// <summary>
        /// Compares the patchlist to the game client hashes and discover whether there are missing files that require downloading.
        /// </summary>
        /// <param name="gameFiles"></param>
        /// <param name="patchlist"></param>
        /// <returns></returns>
        private async Task<IList<PatchInfo>> DiscoverMissingPatches(IDictionary<string, string> gameFiles, IList<PatchInfo> patchlist)
        {
            var missingFiles = patchlist
                .ToDictionary(Q => Q.File, Q => Q)
                .AsParallel()
                .Select(Q =>
                {
                    string hash;

                    var isFound = gameFiles.TryGetValue(Q.Key, out hash);
                    if (!isFound) return Q.Value;

                    var isEqual = hash.Equals(Q.Value.Hash, StringComparison.InvariantCultureIgnoreCase);
                    return isEqual ? null : Q.Value;

                    // For cleaner syntax, NULL here simply means that the file hash in patchlist is found and is equal.
                    // AKA, items that are NOT NULL means that the file in patchlist is NOT FOUND or is DIFFERENT and SHOULD BE DOWNLOADED.
                })
                .Where(Q => Q != null)
                .ToList();

            Output.OnMissingFilesDiscovery(missingFiles.Select(Q => Q.File));
            await WriteMissingFilesToJson(missingFiles);

            return missingFiles;
        }

        /// <summary>
        /// Returns a list of all game files.
        /// </summary>
        /// <returns></returns>
        private IList<string> EnumerateGameFiles()
        {
            var gameFiles = Directory.GetFiles(DataWin32Directory, "*.*", SearchOption.AllDirectories)
                .AsParallel()
                // Logically, since this method is being called AFTER backup restore, there shouldn't be any more files in the backup folder. But hey, better be safe than sorry!
                .Where(Q => !Q.StartsWith(BackupDirectory, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            var whitelist = new[] { "pso2.exe", "pso2launcher.exe", "pso2updater.exe", "pso2download.exe", "pso2predownload.exe", "gameversion.ver", "edition.txt" };
            foreach (var file in whitelist)
            {
                var path = Path.Combine(Settings.GameDirectory, file);
                if (File.Exists(path))
                {
                    gameFiles.Add(path);
                }
            }

            return gameFiles;
        }

        /// <summary>
        /// Calculate a hard disk buffer size. Buffer size block gets increased by multiply of 2, starting from 4KB and up to 4MB.
        /// </summary>
        /// <param name="length">File size in bytes.</param>
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
                Output.OnClientHashReadFailed();
                return await RehashWholeClient();
            }
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
        /// Return all hashable information of game files.
        /// File order is sorted for optimal hill-climbing disk buffer and to avoid Branch Predictor failures.
        /// </summary>
        /// <returns></returns>
        private IList<HashModel> PrepareGameFilesHashModels()
        {
            return EnumerateGameFiles()
                .AsParallel()
                .Select(Q => new FileInfo(Q))
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
        /// Returns a sequence of all files that were successfully downloaded in prior game client update attempt.
        /// </summary>
        /// <returns></returns>
        private async Task<IList<string>> ReadDownloadedFilesFromLog()
        {
            if (!File.Exists(DownloadSuccessLog)) return new List<string>();

            using (var file = File.OpenText(DownloadSuccessLog))
            {
                return (await file.ReadToEndAsync()).Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        /// <summary>
        /// Read latest game client hash dictionary.
        /// </summary>
        /// <returns></returns>
        private async Task<IDictionary<string, string>> ReadLatestClientHash()
        {
            using (var file = File.OpenText(ClientHashesJson))
            {
                var client = await file.ReadToEndAsync();
                var hashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(client);

                Output.OnClientHashReadSuccess();
                return hashes;
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
        /// Perform a (fast) whole client rehash.
        /// When done, save the hashes into the named JSON file and return the result.
        /// </summary>
        /// <returns></returns>
        private async Task<IDictionary<string, string>> RehashWholeClient()
        {
            var gameFiles = PrepareGameFilesHashModels();
            Output.OnHashStart(gameFiles.Select(Q => Q.FileName));

            // Perform the long-running operation in the background thread instead of the UI thread.
            var progress = 0;
            var hashJob = Task.Run(() =>
            {
                // Should probably not use multi-threading here.
                // Hashing is input-bound instead of CPU-bound. Logically should be able to use Async-Await here, but...
                // Unless you're using SSD, parallel read will cause hard drive head to move back and forth trying to read two files at the same time.
                // Hence, files are read and hashed in sequential. Too bad.
                // However, binary hashes can be converted to string later using PLINQ.
                for (progress = 0; progress < gameFiles.Count; progress++)
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
                await Task.Delay(1000);
                if (progress > lastProgress)
                {
                    lastProgress = progress;
                    Output.OnHashProgress(lastProgress, gameFiles.Count);
                }
            } while (lastProgress < gameFiles.Count);

            Output.OnHashComplete();

            var hashes = await hashJob;
            await OverwriteLatestClientJson(hashes);
            return hashes;
        }
        /// <summary>
        /// Read missing files again prior to update interruptions / failure and then read the files that were successfully downloaded previously.
        /// Update the missing files using this information and then return it.
        /// </summary>
        /// <returns></returns>
        private async Task<IList<PatchInfo>> ResumePatching()
        {
            var missingFiles = await ReadMissingFilesFromJson();
            var downloaded = new HashSet<string>(await ReadDownloadedFilesFromLog());

            if (downloaded.Any())
            {
                missingFiles = missingFiles.Where(Q => !downloaded.Contains(Q.File)).ToList();
            }

            Output.OnPatchingResume(missingFiles.Select(Q => Q.File));
            return missingFiles;
        }
        /// <summary>
        /// Attempts to recover the missing files from remote update server or local update repository.
        /// </summary>
        /// <param name="missingFiles">List of missing files</param>
        /// <returns>How many files failed to download</returns>
        private async Task<int> TryUpdateGame(IList<PatchInfo> missingFiles)
        {
            using (var doneLog = File.AppendText(DownloadSuccessLog))
            {
                // TODO: implement sideloading prepatch files here.
                Output.OnPatchingStart(missingFiles.Count);

                var downloads = missingFiles.Select(patch => Downloader.DownloadGamePatch(patch, Settings.GameDirectory, doneLog)).ToList();
                return (await Task.WhenAll(downloads)).Count(Q => Q == false);
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
    }
}
