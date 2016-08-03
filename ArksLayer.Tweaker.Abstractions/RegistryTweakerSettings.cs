using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// Implementation of Tweaker Settings interface against Windows Registry.
    /// </summary>
    public class RegistryTweakerSettings : ITweakerSettings
    {
        /// <summary>
        /// Constructs an instance of the class, pointing to parameter registry path for HKEY_CURRENT_USER.
        /// </summary>
        /// <param name="path"></param>
        public RegistryTweakerSettings(string path = @"Software\AIDA")
        {
            this.Root = Registry.CurrentUser.CreateSubKey(path);
        }

        /// <summary>
        /// pso2_bin folder registry key.
        /// </summary>
        private const string GameDirectoryKey = "PSO2Dir";

        /// <summary>
        /// Sets or gets value to the folder path for pso2_bin.
        /// Expected result example: D:\PSO2\pso2_bin
        /// </summary>
        public string GameDirectory
        {
            get
            {
                var value = (string)Root.GetValue(GameDirectoryKey);
                return value.TrimEnd('\\');
            }
            set
            {
                Root.SetValue(GameDirectoryKey, value.TrimEnd('\\'));
            }
        }

        /// <summary>
        /// Using this property, you can manipulate Windows Registry for Tweaker directly.
        /// </summary>
        public RegistryKey Root { get; private set; }
    }
}
