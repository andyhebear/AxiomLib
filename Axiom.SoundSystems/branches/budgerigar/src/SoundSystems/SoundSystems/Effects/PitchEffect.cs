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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Controllers;
using Axiom.SoundSystems.Controllers;

namespace Axiom.SoundSystems.Effects
{

    public class PitchEffect : EmitterEffect
    {
        #region Fields

        FrameTimeControllerValue _frameTimeValue;
        Controller<float> _controller;
        AttenuationFunction _attenuationFunc;
        float _startPitch;
        float _endPitch;
        EmitterPitch _emitterPitch;

        #endregion

        #region Constructors

        public PitchEffect(float startPitch, float endPitch, AttenuationFunction attenuationFunc, EffectKind kind)
            : this(startPitch, endPitch, attenuationFunc, kind, null)
        {
        }

        public PitchEffect(float startPitch, float endPitch, AttenuationFunction attenuationFunc, EffectKind kind, EffectEventHandler notifyEffectChange)
            : base(kind, notifyEffectChange)
        {
            _frameTimeValue = new FrameTimeControllerValue();
            _startPitch = startPitch;
            _endPitch = endPitch;
            _attenuationFunc = attenuationFunc;
        }

        #endregion

        #region ICloneable

        public override object Clone()
        {
            PitchEffect fx = new PitchEffect(_startPitch, _endPitch, (AttenuationFunction)_attenuationFunc.Clone(), Kind);

            return fx;
        }

        #endregion

        #region EmitterEffect

        public override bool EffectEnabled
        {
            set
            {
                base.EffectEnabled = value;
                _controller.IsEnabled = value;
            }
        }

        public override void InitializeEffect(ISoundEmitter emitter)
        {
            base.InitializeEffect(emitter);

            Emitter.Pitch = _startPitch;

            // create controller value, function and the controller
            _emitterPitch = new EmitterPitch(Emitter, _startPitch, _startPitch, _endPitch, OnExtentReached);

            _controller = ControllerManager.Instance.CreateController(_frameTimeValue, _emitterPitch, _attenuationFunc);
            _controller.IsEnabled = false;
        }

        public override void UpdateEffect()
        {
            if (_prepared)
            {
                _controller.IsEnabled = true;
            }

            base.UpdateEffect();
            // the controller is updated automatically by a scene manager
        }

        public override void ResetEffect()
        {
        	_attenuationFunc.CurrentValue = _startPitch;
        }

        #endregion

        #region Protected

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnExtentReached(object sender, EmitterValueEventArgs e)
        {
            switch (e.Extent)
            {
                case ValueExtent.Base:
                    // do nothing
                    break;

                case ValueExtent.Maximum:
                case ValueExtent.Mininum:
                    ChangeEffectState();
                    break;
            }
        }

        protected override void ChangeEffectState()
        {
            base.ChangeEffectState();

            switch (Kind)
            {
                case EffectKind.PingPongOnce:
                    if (_numStateChanges == 1)
                    {
                        _attenuationFunc.Attenuation = -_attenuationFunc.Attenuation;
                    }
                    break;

                case EffectKind.PingPong:
                    _attenuationFunc.Attenuation = -_attenuationFunc.Attenuation;
                    break;
            }
        }

        #endregion

        #region IDisposable

        protected override void _dispose(bool disposeManagedResources)
        {
            if (!_isDisposed)
            {
                if (disposeManagedResources)
                {
                    ControllerManager.Instance.DestroyController(_controller);
                }

                base._dispose(disposeManagedResources);
            }
        }

        #endregion
    }
}
