using System;

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
        /// Called when a file download is started.
        /// You might want to summon a progress bar here...
        /// </summary>
        /// <param name="url"></param>
        void OnDownloadStart(string url);

        /// <summary>
        /// Called every 5 seconds, sends a progress update for a file download.
        /// You might want to update the progress bar of this file here...
        /// </summary>
        /// <param name="url"></param>
        /// <param name="progressByte"></param>
        /// <param name="totalByte"></param>
        void OnDownloadProgress(string url, long progressByte, long totalByte);

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
        /// Called every 5 seconds, sends hash progress update.
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