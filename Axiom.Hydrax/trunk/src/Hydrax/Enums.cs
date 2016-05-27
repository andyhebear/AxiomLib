using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Axiom.Hydrax
{
    [Flags]
    public enum HydraxComponent
    {
        Sun = 1 << 0,
        Foam = 1 << 1,
        Depth = 1 << 2,
        /// <summary>
        /// Smooth transitions and caustics components need depth component
        /// </summary>
        Smooth = 1 << 3,
        Caustics = 1 << 4,
        Underwater = 1 << 5,
        /// <summary>
        /// Underwater reflections and god rays need underwater component
        /// </summary>
        UnderwaterReflections = 1 << 6,
        UnderwaterGodRays = 1 << 7,

        None = 0x0000,
        All = 0x001F,
    }
}
