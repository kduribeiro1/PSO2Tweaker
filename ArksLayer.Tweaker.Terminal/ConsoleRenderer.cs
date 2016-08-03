using ArksLayer.Tweaker.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.Terminal
{
    public class ConsoleRenderer : IRenderer
    {
        public void AppendLog(string s)
        {
            Debug.WriteLine(s);
        }

        public void OnHashProgress(int progress, int total)
        {
            WriteLine($"{progress} out of {total} files hashed.");
        }

        public void OnDownloadProgress(string url, long progressByte, long totalByte)
        {
            var percentage = string.Format("{0:N2}%", Math.Truncate(progressByte / (double)totalByte * 100 * 100) / 100);
            var s = $"DOWNLOADING {url} | {progressByte / 1024} KB out of {totalByte / 1024} KB | {percentage}";
            WriteLine(s);
        }

        public void WriteLine(string s)
        {
            Console.WriteLine(s);
        }

        public void OnDownloadStart(string url)
        {
            WriteLine($"GET {url}");
        }

        public void OnDownloadFinish(string url)
        {
            WriteLine($"DOWNLOADED {url}");
        }

        public void OnDownloadRetry(string url, int delaySecond)
        {
            WriteLine($"Retrying {url} in {delaySecond}s");
        }

        public void OnDownloadAborted(string url)
        {
            WriteLine($"Failed to download {url}...");
        }

        public void OnHashStart()
        {
            WriteLine($"Commence full hash against game client files.");
        }

        public void OnHashComplete()
        {
            WriteLine("Game files hash complete!");
        }
    }
}
