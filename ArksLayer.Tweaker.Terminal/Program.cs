using ArksLayer.Tweaker.Abstractions;
using ArksLayer.Tweaker.UpdateEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.Terminal
{
    public class Program
    {
        public static async Task MainAsync()
        {
            // Use IOC Container in the main Tweaker project to deal with dependencies.
            var output = new ConsoleTrigger();
            var settings = new RegistryTweakerSettings(@"Software\AIDA");
            var updater = new UpdateManager(settings, output);

            Console.WriteLine(settings.GameDirectory);
            Console.WriteLine(updater.DataWin32Directory);
            Console.WriteLine(updater.BackupDirectory);
            
            await updater.Update(false, true);
        }

        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
    }
}
