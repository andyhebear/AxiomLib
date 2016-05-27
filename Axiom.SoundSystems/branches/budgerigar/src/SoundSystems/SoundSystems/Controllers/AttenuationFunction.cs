#region MIT License
/*
The MIT License

Copyright (c) 2010 Axiom Contrib Developers

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using Axiom.Controllers;
using System;

namespace Axiom.SoundSystems.Controllers
{
    /// <summary>
    /// Base class for functions attenuating a float value
    /// </summary>
    public abstract class AttenuationFunction : IControllerFunction<float>, ICloneable
    {
        #region Properties

        /// <summary>
        /// The amount of attenuation of the source value (for example the amount of a linear attenuation)
        /// </summary>
        public virtual float Attenuation { get; set; }
                
        /// <summary>
        /// The current value of the function
        /// </summary>
        public virtual float CurrentValue { get; set; }

        #endregion

        #region IControllerFunction<float>

        public abstract float Execute(float sourceValue);

        #endregion

        #region ICloneable

        public abstract object Clone();

        #endregion
    }
}
