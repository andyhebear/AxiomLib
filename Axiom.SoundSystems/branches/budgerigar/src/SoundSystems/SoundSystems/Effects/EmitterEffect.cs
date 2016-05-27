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

namespace Axiom.SoundSystems.Effects
{
    public abstract class EmitterEffect : IEmitterEffect
    {
        #region Contructors

        public EmitterEffect(EffectKind kind, EffectEventHandler notifyEffectChange)
        {
            Kind = kind;
            NotifyEffectChange += notifyEffectChange;
        }

        #endregion

        #region Fields

        protected int _numStateChanges;
        protected bool _prepared;
		protected SoundContext _context;
		
        #endregion

        #region Properties
        
        protected ISoundEmitter Emitter { get; set; }
        
        protected EffectKind Kind { get; set; }
        
        #endregion
        
        #region ICloneable

        public abstract object Clone();

        #endregion

        #region IEmitterEffect

        /// <summary>
        /// 
        /// </summary>
        public event EffectEventHandler NotifyEffectChange;

        private bool _effectEnabled;
        /// <summary>
        /// 
        /// </summary>
        public virtual bool EffectEnabled
        {
            get
            {
                return _effectEnabled;
            }
            set
            {
                if (value != _effectEnabled)
                    NotifyChange(value ? EffectEventKind.Started : EffectEventKind.Paused);

                _effectEnabled = value;
            }
        }

        public virtual void InitializeEffect(ISoundEmitter emitter)
        {
            if (emitter == null)
                throw new ArgumentNullException("emitter");

            if (!(emitter is SoundInstance))
            	throw new ArgumentException("The specified emitter must actually be a SoundInstance", "emitter");

            _context = ((SoundInstance)emitter).ParentSound.Creator;
            Emitter = emitter;
            _effectEnabled = true;
            _prepared = true;
        }

        public virtual void UpdateEffect()
        {
            if (_prepared)
                _prepared = false;
        }

        public abstract void ResetEffect();

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        protected virtual void ChangeEffectState()
        {
            _numStateChanges++;

            switch (Kind)
            {
                case EffectKind.Once:
                    // stop effect
                    NotifyChange(EffectEventKind.Finished);
                    QueueForDisposal();
                    break;

                case EffectKind.Loop:
                    NotifyChange(EffectEventKind.Changed);
                    ResetEffect();
                    break;

                case EffectKind.PingPongOnce:
                    if (_numStateChanges == 1)
                    {
                        NotifyChange(EffectEventKind.Changed);
                    }
                    else
                    {
                        NotifyChange(EffectEventKind.Finished);
	                    QueueForDisposal();
                    }
                    break;

                case EffectKind.PingPong:
                    NotifyChange(EffectEventKind.Changed);
                    break;
            }
        }

        protected void NotifyChange(EffectEventKind eventKind)
        {
            if (NotifyEffectChange != null)
            {
                EffectEventArgs args = new EffectEventArgs(eventKind, _numStateChanges);
                NotifyEffectChange(this, args);
            }
        }

        protected void QueueForDisposal()
        {
        	_context.QueuedForDisposal.Add(this);
        }
        
        #endregion

        #region IDisposable

        ///// <summary>
        ///// 
        ///// </summary>
        //~EmitterEffect()
        //{
        //    _dispose(false);
        //}

        protected bool _isDisposed;
        /// <summary>
        /// 
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        protected virtual void _dispose(bool disposeManagedResources)
        {
            if (!_isDisposed)
            {
                if (disposeManagedResources)
                {
                    Emitter.Effects.Remove(this);
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            _dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion 
    }
}
