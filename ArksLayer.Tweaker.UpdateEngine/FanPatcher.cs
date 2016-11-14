using ArksLayer.Tweaker.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.UpdateEngine
{
    /// <summary>
    /// Contains methods for downloading and applying English fan patches.
    /// </summary>
    public class FanPatcher
    {
        private readonly ITweakerSettings Settings;
        private readonly ITrigger Triggers;

        /// <summary>
        /// Constructs an instance of FanPatcher using given Tweaker settings and output triggers.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="triggers"></param>
        public FanPatcher(ITweakerSettings settings, ITrigger triggers)
        {
            this.Settings = settings;
            this.Triggers = triggers;
        }

        /// <summary>
        /// Attempts to apply a patch of given name (for backup folder naming) from a given URL or File Path.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task ApplyPatch(string name, string path)
        {
            Stream buffer = await TryObtainFile(path);
            if (buffer == null) return;

            Triggers.OnFanPatching(name);

            await Task.Run(() =>
            {
                var backupFolder = Path.Combine(Settings.BackupDirectory(), name);
                if (Directory.Exists(backupFolder) == false)
                {
                    Directory.CreateDirectory(backupFolder);
                }

                using (var zip = new ZipArchive(buffer))
                {
                    foreach (var entry in zip.Entries)
                    {
                        var target = Path.Combine(Settings.DataWin32Directory(), entry.Name);
                        if (File.Exists(target))
                        {
                            var backup = Path.Combine(backupFolder, entry.Name);
                            File.Move(target, backup);
                        }
                        entry.ExtractToFile(target);
                    }
                }
            });

            switch (name)
            {
                case "English Patch":
                    {
                        Settings.EnglishPatchVersion = Path.GetFileNameWithoutExtension(path);
                        break;
                    }
                case "Large Files":
                    {
                        Settings.EnglishLargePatchVersion = Path.GetFileNameWithoutExtension(path);
                        break;
                    }
            }

            Triggers.OnFanPatchSuccessful(name);
            buffer.Dispose();
        }

        /// <summary>
        /// Either obtain a fan patch file from a URL or a File System.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private async Task<Stream> TryObtainFile(string path)
        {
            Uri uriResult;
            bool isUrl = Uri.TryCreate(path, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (isUrl)
            {
                var binary = await DownloadPatch(uriResult.AbsoluteUri);
                return new MemoryStream(binary);
            }
            if (File.Exists(path))
            {
                return new FileStream(path, FileMode.Open);
            }

            return null;
        }

        /// <summary>
        /// Attempts to download a binary from URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<byte[]> DownloadPatch(string url)
        {
            var client = new WebClient();
            Triggers.OnDownloadStart(url, client);
            var binary = await client.DownloadDataTaskAsync(url);
            Triggers.OnDownloadFinish(url);

            return binary;
        }
    }
}
