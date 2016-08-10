using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.UpdateEngine
{
    /// <summary>
    /// A class containing values required for mass hashing operation.
    /// </summary>
    internal class HashModel
    {
        /// <summary>
        /// The hard disk buffer size for hashing the file.
        /// </summary>
        public int BufferSize { set; get; }

        /// <summary>
        /// The full path to the file to be hashed.
        /// </summary>
        public string FileName { set; get; }

        /// <summary>
        /// The size of the file to be hashed.
        /// </summary>
        public long FileSize { set; get; }

        /// <summary>
        /// The resulting hash, in binary. You might want to convert this into human-readable string.
        /// </summary>
        public byte[] HashBinary { set; get; }
    }
}
