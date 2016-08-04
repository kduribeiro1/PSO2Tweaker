using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.UpdateEngine
{
    /// <summary>
    /// An class containing information of how to download a specific patch file from SEGA server.
    /// </summary>
    public class PatchInfo
    {
        /// <summary>
        /// The file name. Expected value example: "data/win32/abc123"
        /// </summary>
        [JsonProperty("F")]
        public string File { set; get; }

        /// <summary>
        /// The hash of the file, in MD5.
        /// </summary>
        [JsonProperty("H")]
        public string Hash { set; get; }

        /// <summary>
        /// Determines whether the file should be downloaded from the old patch server or the normal server.
        /// </summary>
        [JsonProperty("O")]
        public bool Old { set; get; }
    }
}
