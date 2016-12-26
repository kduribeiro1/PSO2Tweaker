using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// A class for managing Tweaker settings using JSON file.
    /// </summary>
    public class JsonTweakerSettings : ITweakerSettings
    {
        /// <summary>
        /// Shared lock object for the configuration file.
        /// </summary>
        public static object ConfigurationLock = new object();

        private TweakerJson Configuration;
        private FileSystemWatcher Watch;
        private string FilePath;

        /// <summary>
        /// Returns a default Tweaker JSON configuration file path, which is located in %appdata%\PSO2 Tweaker\settings.json 
        /// </summary>
        public static string DefaultTweakerJsonFilePath
        {
            get
            {
                var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appdata, "PSO2 Tweaker", "settings.json");
            }
        }

        /// <summary>
        /// Sets or gets value of the directory path to the English Large Patch version.
        /// </summary>
        public string EnglishLargePatchVersion
        {
            get
            {
                return Configuration.LargeFilesVersion;
            }

            set
            {
                Configuration.LargeFilesVersion = value;
                WriteConfiguration();
            }
        }

        /// <summary>
        /// Sets or gets value of the directory path to the English Patch version.
        /// </summary>
        public string EnglishPatchVersion
        {
            get
            {
                return Configuration.ENPatchVersion;
            }

            set
            {
                Configuration.ENPatchVersion = value;
                WriteConfiguration();
            }
        }

        /// <summary>
        /// Sets or gets value of the directory path to pso2_bin.
        /// Expected result example: D:\PSO2\pso2_bin
        /// </summary>
        public string GameDirectory
        {
            get
            {
                return Configuration.PSO2Dir;
            }

            set
            {
                Configuration.PSO2Dir = value;
                WriteConfiguration();
            }
        }

        /// <summary>
        /// Sets or gets the latest game client version.
        /// </summary>
        public string GameVersion
        {
            get
            {
                return this.GetGameVersion();
            }

            set
            {
                this.SetGameVersion(value);
            }
        }

        /// <summary>
        /// Sets or gets value of the directory path to the Story Patch version.
        /// </summary>
        public string StoryPatchVersion
        {
            get
            {
                return Configuration.StoryPatchVersion;
            }

            set
            {
                Configuration.StoryPatchVersion = value;
                WriteConfiguration();
            }
        }

        /// <summary>
        /// Constructs a new setting against the specified file.
        /// </summary>
        /// <param name="file"></param>
        public JsonTweakerSettings(string file)
        {
            this.FilePath = file;

            if (File.Exists(file) == false)
            {
                this.Configuration = new TweakerJson();
                WriteConfiguration();
            }

            ReadConfiguration();
            InitializeWatcher();
        }

        private void InitializeWatcher()
        {
            this.Watch = new FileSystemWatcher();
            Watch.Path = Path.GetDirectoryName(FilePath);
            Watch.Filter = Path.GetFileName(FilePath);
            Watch.Changed += Watch_Changed;

            Watch.EnableRaisingEvents = true;
        }

        private void Watch_Changed(object sender, FileSystemEventArgs e)
        {
            ReadConfiguration();
            Debug.WriteLine(JsonConvert.SerializeObject(Configuration));
        }

        private void ReadConfiguration()
        {
            lock (ConfigurationLock)
            {
                using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();
                    this.Configuration = JsonConvert.DeserializeObject<TweakerJson>(json);
                }
            }
        }

        private void WriteConfiguration()
        {
            lock (ConfigurationLock)
            {
                var dir = Path.GetDirectoryName(FilePath);
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }

                var json = JsonConvert.SerializeObject(Configuration, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
        }
    }
}
