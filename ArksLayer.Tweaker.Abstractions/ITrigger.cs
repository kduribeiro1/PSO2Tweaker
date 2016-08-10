using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// This interface is what enables the UpdateEngine to communicate with your UI.
    /// See ArksLayer.Tweaker.Terminal.ConsoleTrigger for example on how best to implement this.
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// Called when the library wants to log something. This method may run in a separated thread, implement with caution.
        /// </summary>
        /// <param name="s"></param>
        void AppendLog(string s);

        /// <summary>
        /// Output.WriteLine("Your game is up-to-date!");
        /// </summary>
        void IfUpdateNotNeeded();

        /// <summary>
        /// Output.AppendLog($"Moving backup from {file} to {target}");
        /// Output.WriteLine($"Found {backupFiles.Count()} files in backup directory. Restoring...");
        /// </summary>
        /// <param name="backupFiles"></param>
        void OnBackupRestore(IEnumerable<string> backupFiles);

        /// <summary>
        /// Output.WriteLine("Uncensoring game chat...");
        /// </summary>
        void OnCensorRemoval();

        /// <summary>
        /// Output.WriteLine("Unable to read the latest client file hashes!");
        /// </summary>
        void OnClientHashReadFailed();

        /// <summary>
        /// Output.WriteLine($"Game client hashes found!");
        /// </summary>
        void OnClientHashReadSuccess();

        /// <summary>
        /// Called when a file download failed completely after certain number of retries.
        /// </summary>
        /// <param name="url"></param>
        void OnDownloadAborted(string url);

        /// <summary>
        /// Called when a file download finished successfully!
        /// You might want to destroy or update the progress bar of this file here...
        /// </summary>
        /// <param name="url"></param>
        void OnDownloadFinish(string url);

        /// <summary>
        /// Called when a file download failed but will be retried.
        /// Retry is not immediately and will happen after a certain number of seconds delay.
        /// You might want to notify the user when this happens.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="delaySecond"></param>
        void OnDownloadRetry(string url, int delaySecond);

        /// <summary>
        /// Called when a file download is started.
        /// You might want to attach a progress delegate to that WebClient object here or summon a progress bar here...
        /// Also, you can save that WebClient object into something like a Dictionary of url and WebClient, so user can cancel the individual file download if stuck, if provided the UI control to do so.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="client"></param>
        void OnDownloadStart(string url, WebClient client);

        /// <summary>
        /// Called when full client hash process is finished.
        /// You might want to destroy the hash progress bar here...
        /// </summary>
        void OnHashComplete();

        /// <summary>
        /// Called every second, sends hash progress update.
        /// You might want to update the hash progress bar here...
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="total"></param>
        void OnHashProgress(int progress, int total);

        /// <summary>
        /// Called when full client hash process is started.
        /// You might want to summon a progress bar here...
        /// Foreach Output.AppendLog("Queued for hashing: " + file.FileName);
        /// Output.WriteLine($"Discovered { fileCount } files in game directory.");
        /// </summary>
        /// <param name="files"></param>
        void OnHashStart(IEnumerable<string> files);

        /// <summary>
        /// Output.WriteLine("All done!");
        /// </summary>
        void OnHousekeepingCompleted();

        /// <summary>
        /// Output.WriteLine("A little housekeeping...");
        /// </summary>
        void OnHousekeepingStart();

        /// <summary>
        /// Output.WriteLine($"Found { legacyFiles.Count } legacy files: {megabytes} MB");
        /// Output.AppendLog($"Legacy file {Q.FullName} deleted.");
        /// </summary>
        /// <param name="legacyFiles"></param>
        void OnLegacyFilesFound(IEnumerable<string> legacyFiles);

        /// <summary>
        /// Output.WriteLine("There are no legacy files to clean up!");
        /// </summary>
        void OnLegacyFilesNotFound();

        /// <summary>
        /// Output.WriteLine("Scanning for legacy files...");
        /// </summary>
        void OnLegacyFilesScanning();

        /// <summary>
        /// Output.WriteLine($"Discovering missing files...");
        /// Output.WriteLine($"Found {missingFiles.Count} missing files!");
        /// </summary>
        /// <param name="missingFiles"></param>
        void OnMissingFilesDiscovery(IEnumerable<string> missingFiles);

        /// <summary>
        /// Output.WriteLine($"Failed to download {failCount} files!");
        /// </summary>
        /// <param name="failCount"></param>
        void OnPatchingFailed(int failCount);

        /// <summary>
        /// Output.WriteLine($"Resuming patch download of {missingFiles.Count} files...");
        /// </summary>
        /// <param name="missingFiles"></param>
        void OnPatchingResume(IEnumerable<string> missingFiles);

        /// <summary>
        /// Called when the UpdateEngine is going to batch start patch download Tasks. Sends how many files are to be downloaded in total.
        /// </summary>
        /// <param name="fileCount"></param>
        void OnPatchingStart(int fileCount);

        /// <summary>
        /// Output.WriteLine("Update complete!");
        /// </summary>
        void OnPatchingSuccess();

        /// <summary>
        /// Output.WriteLine($"Patchlist contains {result.Count} file hashes.");
        /// </summary>
        /// <param name="count"></param>
        void OnPatchlistFetchCompleted(int count);

        /// <summary>
        /// Output.WriteLine("Downloading patchlists...");
        /// </summary>
        void OnPatchlistFetchStart();
    }
}