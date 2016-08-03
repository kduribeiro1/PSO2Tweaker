using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// Using this interface, various settings required by the Tweaker can be read strongly.
    /// </summary>
    public interface ITweakerSettings
    {
        /// <summary>
        /// Sets or gets value to the folder path for pso2_bin.
        /// Expected result example: D:\PSO2\pso2_bin
        /// </summary>
        string GameDirectory { set; get; }
    }
}
