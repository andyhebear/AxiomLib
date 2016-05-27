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

using System;
using Axiom.Controllers;
using System.Collections;
using System.Collections.Generic;

namespace Axiom.SoundSystems.Controllers
{
    /// <summary>
    /// Base class for controlling <see cref="ISoundEmitter"/> properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EmitterValue<T> : IControllerValue<T> where T : IComparable<T>
    {
        #region Constructors

        public EmitterValue(ISoundEmitter emitter, T baseValue, T minValue, T maxValue)
            : this(emitter, baseValue, minValue, maxValue, null)
        {        
        }

        public EmitterValue(ISoundEmitter emitter, T baseValue, T minValue, T maxValue, EmitterValueEventHandler extentReached)

        {
            if (emitter == null)
                throw new ArgumentNullException("emitter");

            _emitter = emitter;
            _baseValue = baseValue;
            _minValue = minValue;
            _maxValue = maxValue;
            ExtentReached += new EventHandler<EmitterValueEventArgs>(extentReached);
        }

        #endregion

        #region Events

        /// <summary>
        /// This event is fired whenever the value reaches an extent.
        /// </summary>
        public event EventHandler<EmitterValueEventArgs> ExtentReached;

        #endregion

        #region IControllerValue<T>

        /// <summary>
        /// The value to be set for a <see cref="ISoundEmitter"/> property, when setting it, the value shall be combined with <see cref="BaseValue"/>
        /// </summary>
        /// <remarks>
        /// Must be overriden in concrete implementation to propagate the value to a property of the <see cref="ISoundEmitter"/>
        /// </remarks>
        public abstract T Value
        {
            get;
            set;
        }

        #endregion

        #region Properties

        private ISoundEmitter _emitter;
        /// <summary>
        /// The controlled <see cref="ISoundEmitter"/>
        /// </summary>
        public ISoundEmitter Emitter
        {
            get { return _emitter; }
        }

        private T _baseValue;
        /// <summary>
        /// 
        /// </summary>
        public T BaseValue
        {
            get { return _baseValue; }
            set { _baseValue = value; }
        }

        private T _minValue;
        /// <summary>
        /// 
        /// </summary>
        public T MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        private T _maxValue;
        /// <summary>
        /// 
        /// </summary>
        public T MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        #endregion

        #region Protected

        protected void NotifyExtent(ValueExtent extent)
        {
            if (ExtentReached != null)
            {
                EmitterValueEventArgs args = new EmitterValueEventArgs(extent);
                ExtentReached(this, args);
            }
        }

        #endregion    
    }

    /// <summary>
    /// Base class for controlling <see cref="ISoundEmitter"/> properties of type float
    /// </summary>
    public abstract class EmitterValueFloat : EmitterValue<float>
    {
        #region Constructors

        public EmitterValueFloat(ISoundEmitter emitter, float baseValue, float minValue, float maxValue)
            : base(emitter, baseValue, minValue, maxValue, null)
        {        
        }

        public EmitterValueFloat(ISoundEmitter emitter, float baseValue, float minValue, float maxValue, EmitterValueEventHandler extentReached)
            : base(emitter, baseValue, minValue, maxValue, extentReached)
        {
        }

        #endregion

        #region EmitterValue<float>

        protected float _value;
        /// <summary>
        /// The value to be set for a <see cref="ISoundEmitter"/> property, when setting it, the value is combined with <see cref="BaseValue"/>
        /// </summary>
        /// <remarks>
        /// Must be overriden in concrete implementation to propagate the value to a property of the <see cref="ISoundEmitter"/>
        /// </remarks>
        public override float Value
        {
            get
            {
                return _value;
            }
            set
            {
                bool c1 = _value > BaseValue;

                _value = value + BaseValue; // update value

                bool c2 = _value <= BaseValue;

                // check extents reached
                if (c1 != c2)
                {
                    NotifyExtent(ValueExtent.Base);
                }
                if (_value > MaxValue)
                {
                    _value = MaxValue;
                    NotifyExtent(ValueExtent.Maximum);
                }
                if (_value < MinValue)
                {
                    _value = MinValue;
                    NotifyExtent(ValueExtent.Mininum);
                }
            }
        }

        #endregion
    }

}
