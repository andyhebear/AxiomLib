using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Axiom.SoundSystems.Demos
{
    /// <summary>
    /// Denotes a sound demo class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=true)]
    public class SoundDemoAttribute : Attribute
    {
    }
}
