using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// Extension method for common values obtainable from ITweakerSettings.
    /// </summary>
    public static class TweakerSettingsExtensions
    {
        /// <summary>
        /// Gets the known, usual Tweaker backup directory.
        /// </summary>
        public static string BackupDirectory(this ITweakerSettings settings)
        {
            return Path.Combine(settings.DataWin32Directory(), "backup");
        }

        /// <summary>
        /// Gets the game file directory where the game asset files usually located in.
        /// </summary>
        public static string DataWin32Directory(this ITweakerSettings settings)
        {
            return Path.Combine(settings.GameDirectory, @"data\win32");
        }
    }
}
