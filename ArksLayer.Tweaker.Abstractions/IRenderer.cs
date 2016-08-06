using System;
using System.Net;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// This interface is what enables the UpdateEngine to communicate with your UI.
    /// See ArksLayer.Tweaker.Terminal.ConsoleRenderer for example on how best to implement this.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Called when the library wants to displays a simple message to user.
        /// </summary>
        /// <param name="s"></param>
        void WriteLine(string s);

        /// <summary>
        /// Called when the library wants to log something.
        /// </summary>
        /// <param name="s"></param>
        void AppendLog(string s);

        /// <summary>
        /// Called when the UpdateEngine is going to batch start patch download Tasks. Sends how many files are to be downloaded in total.
        /// </summary>
        /// <param name="fileCount"></param>
        void OnPatchingStart(int fileCount);

        /// <summary>
        /// Called when a file download is started.
        /// You might want to attach a progress delegate to that WebClient object here or summon a progress bar here...
        /// Also, you can save that WebClient object into something like a Dictionary of url and WebClient, so user can cancel the individual file download if stuck, if provided the UI control to do so.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="client"></param>
        void OnDownloadStart(string url, WebClient client);

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
        /// Called when a file download failed completely after certain number of retries.
        /// </summary>
        /// <param name="url"></param>
        void OnDownloadAborted(string url);

        /// <summary>
        /// Called when full client hash process is started.
        /// You might want to summon a progress bar here...
        /// </summary>
        void OnHashStart();

        /// <summary>
        /// Called every 2 seconds, sends hash progress update.
        /// You might want to update the hash progress bar here...
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="total"></param>
        void OnHashProgress(int progress, int total);

        /// <summary>
        /// Called when full client hash process is finished.
        /// You might want to destroy the hash progress bar here...
        /// </summary>
        void OnHashComplete();
    }
}