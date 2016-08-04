using ArksLayer.Tweaker.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.UpdateEngine
{
    /// <summary>
    /// A class capable of interacting with SEGA patch server.
    /// </summary>
    public class SegaDownloader
    {
        /// <summary>
        /// Construct an instance of SegaDownloader using the provided UI renderer.
        /// </summary>
        /// <param name="output"></param>
        public SegaDownloader(IRenderer output)
        {
            this.Output = output;
        }

        /// <summary>
        /// Base URL for the standard patch server.
        /// </summary>
        private const string PatchBaseUrl = "http://download.pso2.jp/patch_prod/patches/";

        /// <summary>
        /// Base URL for the older patch server.
        /// </summary>
        private const string OldPatchBaseUrl = "http://download.pso2.jp/patch_prod/patches_old/";

        /// <summary>
        /// Patchlist URL for the game launcher executables.
        /// </summary>
        private const string LauncherListUrl = PatchBaseUrl + "launcherlist.txt";

        /// <summary>
        /// Patchlist URL from the standard patch server.
        /// </summary>
        private const string PatchlistUrl = PatchBaseUrl + "patchlist.txt";

        /// <summary>
        /// Patchlist URL from the older patch server.
        /// </summary>
        private const string OldPatchlistUrl = OldPatchBaseUrl + "patchlist.txt";

        /// <summary>
        /// Dependency to UI renderer class.
        /// </summary>
        private IRenderer Output { get; set; }

        /// <summary>
        /// Attempts do download an update patchlist from SEGA server.
        /// The value returned will be a list of patch information.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<PatchInfo>> FetchUpdatePatchlist()
        {
            Output.WriteLine("Downloading patchlists...");

            var launcherList = DownloadAndParsePatchlist(LauncherListUrl);
            var patchlist = DownloadAndParsePatchlist(PatchlistUrl);
            var oldPatchlist = DownloadAndParsePatchlist(OldPatchlistUrl);

            await Task.WhenAll(launcherList, patchlist, oldPatchlist);

            // Apparently, there are duplicates and the (new) patchlist takes priority before the old patchlist!
            Output.WriteLine("Merging patchlists...");

            var merge = new ConcurrentDictionary<string, PatchInfo>();
            MergePatchlist(merge, await launcherList, false);
            MergePatchlist(merge, await patchlist, false);
            MergePatchlist(merge, await oldPatchlist, true);

            var result = merge.Values.ToList();
            using (var file = File.CreateText("patchlist.json"))
            {
                await file.WriteAsync(JsonConvert.SerializeObject(result));
            }
            Output.WriteLine($"Patchlist contains {result.Count} file hashes.");
            return result;
        }

        /// <summary>
        /// Attempts to merge multiple patches into a dictionary of filename to patch information.
        /// If a filename already exists in the dictionary, then the patch information will be disregarded. (First Come First Serve)
        /// Old parameter indicates whether the patch should be downloaded from the older patch server.
        /// </summary>
        /// <param name="merge"></param>
        /// <param name="patchlist"></param>
        /// <param name="old"></param>
        private void MergePatchlist(ConcurrentDictionary<string, PatchInfo> merge, IList<PatchInfo> patchlist, bool old)
        {
            Parallel.ForEach(patchlist, p =>
            {
                p.Old = old;
                // TryAdd returns false if the key already exists, unlike TryUpdate or AddOrUpdate.
                merge.TryAdd(p.File, p);
            });
        }

        /// <summary>
        /// Attempts to download a patchlist from a given url then parse into a sequence of patch information.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<IList<PatchInfo>> DownloadAndParsePatchlist(string url)
        {
            var response = await DownloadPatchlist(url);

            Output.WriteLine($"PARSING {url}");
            var result = ParsePatchlist(response);

            Output.WriteLine($"READY {url}");
            return result;
        }

        /// <summary>
        /// Attempts to download a patchlist from a given url, then read as string and returned to caller.
        /// If attempt number is provided, will cause the download to be retried as much as the number allows, with exponential backoff algorithm.
        /// If download failed, will throw an exception.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        private async Task<string> DownloadPatchlist(string url, int attempts = 4)
        {
            if (attempts < 1) attempts = 1;

            for (int i = 0; i < attempts; i++)
            {
                await ExponentialBackoff(i, url);

                var client = new AquaClient();
                var uiDelay = Stopwatch.StartNew();
                long lastProgress = 0;
                client.DownloadProgressChanged += (sender, e) =>
                {
                    if (uiDelay.ElapsedMilliseconds < (2 * 1000) || lastProgress == e.BytesReceived) return;
                    Output.OnDownloadProgress(url, e.BytesReceived, e.TotalBytesToReceive);
                    lastProgress = e.BytesReceived;
                    uiDelay.Restart();
                };

                Output.OnDownloadStart(url, client);
                var download = client.DownloadStringTaskAsync(url);

                try
                {
                    var response = await download;
                    if (string.IsNullOrEmpty(response)) throw new Exception("Empty response!");

                    Output.OnDownloadFinish(url);
                    return response;
                }
                catch (Exception Ex)
                {
                    Output.AppendLog($"Download failed {url} because: {Ex.Message}");
                }
            }

            Output.OnDownloadAborted(url);
            throw new Exception("Unable to download " + url);
        }

        private static object PatchDownloadSuccessLogLock = new object();

        /// <summary>
        /// Attempts to download a patch into a target directory.
        /// If log name is provided, will write a line then flush immediately into the log when a download is successful.
        /// If attempt number is provided, will cause the download to be retried as much as the number allows, with exponential backoff algorithm.
        /// Returns true if download is successful, else false.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="directory"></param>
        /// <param name="logName"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        public async Task<bool> DownloadGamePatch(PatchInfo target, string directory, string logName = null, int attempts = 4)
        {
            if (attempts < 1) attempts = 1;

            var file = Path.Combine(directory, target.File);
            var folder = new FileInfo(file).DirectoryName;
            //Directory.CreateDirectory(folder);

            var baseUrl = target.Old ? OldPatchBaseUrl : PatchBaseUrl;
            var url = baseUrl + target.File + ".pat";

            for (int i = 0; i < attempts; i++)
            {
                await ExponentialBackoff(i, url);

                var client = new AquaClient();
                var uiDelay = Stopwatch.StartNew();
                long lastProgress = 0;
                client.DownloadProgressChanged += (sender, e) =>
                {
                    if (uiDelay.ElapsedMilliseconds < (2 * 1000) || lastProgress == e.BytesReceived) return;
                    Output.OnDownloadProgress(url, e.BytesReceived, e.TotalBytesToReceive);
                    lastProgress = e.BytesReceived;
                    uiDelay.Restart();
                };

                Output.OnDownloadStart(url, client);
                var download = client.DownloadFileTaskAsync(url, file);

                try
                {
                    await download;
                    if (new FileInfo(file).Length == 0) throw new Exception("Empty response!");

                    if (!string.IsNullOrEmpty(logName))
                    {
                        lock (PatchDownloadSuccessLogLock)
                        {
                            File.AppendAllText(logName, target.File + "\n", Encoding.UTF8);
                        }
                    }

                    Output.OnDownloadFinish(url);
                    return true;
                }
                catch (Exception Ex)
                {
                    Output.AppendLog($"Download failed {url} because: {Ex.Message}");
                }
            }

            Output.OnDownloadAborted(url);
            return false;
            //throw new Exception("Unable to download " + url);
        }

        /// <summary>
        /// Induce an artificial delay against file download as not to flood the SEGA server with download requests.
        /// Then notify the user that the download is being delayed.
        /// </summary>
        /// <param name="iteration"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private Task ExponentialBackoff(int iteration, string url)
        {
            // 4^1 = 4 seconds
            // 4^2 = 16 seconds
            // 4^3 = 64 seconds (1 minute and 4 seconds)
            // 4^4 = 256 seconds (4 minutes and 16 seconds)
            if (iteration > 0)
            {
                var backoff = Convert.ToInt32(Math.Pow(4, iteration));
                Output.OnDownloadRetry(url, backoff);
                return Task.Delay(backoff * 1000);
            }

            return Task.Delay(1);
        }

        /// <summary>
        /// Parse a raw downloaded patchlist content.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private IList<PatchInfo> ParsePatchlist(string raw)
        {
            // patchlist structure: [File]\t[Size]\t[Hash]\r\n

            return raw.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .AsParallel()
                .Select(Q => ParsePatchlistRow(Q))
                .Where(Q => Q != null) // ParsePatchlistRow may be NULL if exception occured!
                .ToList();
        }

        /// <summary>
        /// Parse a line extracted from the raw patchlist content.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private PatchInfo ParsePatchlistRow(string row)
        {
            try
            {
                var columns = row.Split('\t');
                var filename = PatTrim(columns[0].Trim());
                var hash = columns[2].Trim();

                return new PatchInfo
                {
                    File = filename,
                    Hash = hash
                };
            }
            catch (Exception Ex)
            {
                Output.AppendLog($"Error parsing patchlist: {JsonConvert.SerializeObject(row)}");
                Debug.WriteLine(Ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Removes .pat extension from the file downloaded.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string PatTrim(string s)
        {
            if (s.EndsWith(".pat", StringComparison.InvariantCultureIgnoreCase))
            {
                return s.Remove(s.Length - ".pat".Length);
            }

            return s;
        }

    }
}
