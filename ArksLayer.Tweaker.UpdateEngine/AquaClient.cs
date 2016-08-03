using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.UpdateEngine
{
    /// <summary>
    /// A special web client class that is capable of downloading from SEGA PSO2 patch server.
    /// </summary>
    public class AquaClient : WebClient
    {
        /// <summary>
        /// Creates an instance of AquaClient.
        /// </summary>
        public AquaClient()
        {
            this.Headers["user-agent"] = "AQUA_HTTP";
        }
    }
}
