using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Axiom.SoundSystems.Effects
{
    public abstract class SoundEffect : ISoundEffect
    {
        #region Contructor

        public SoundEffect(EffectKind kind, EffectEventHandler notifyEffectChange)
        {
            _kind = kind;
            NotifyEffectChange += notifyEffectChange;
        }

        #endregion

        #region Fields

        protected ISoundEmitter _emitter;
        protected EffectKind _kind;
        protected int _numStateChanges;
        protected bool _prepared;

        #endregion

        #region ICloneable

        public abstract object Clone();

        #endregion

        #region ISoundEffect

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

            _emitter = emitter;
            _effectEnabled = true;
            _prepared = true;
        }

        public virtual void UpdateEffect()
        {
            if (_prepared)
                _prepared = false;
        }

        public abstract void ResetEffect();

        /// <summary>
        /// 
        /// </summary>
        public event EffectEventHandler NotifyEffectChange;

        #endregion

        #region Protected

        /// <summary>
        /// 
        /// </summary>
        protected virtual void ChangeEffectState()
        {
            _numStateChanges++;

            switch (_kind)
            {
                case EffectKind.Once:
                    // stop effect
                    NotifyChange(EffectEventKind.Finished);
                    Dispose();
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
                        Dispose();
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

        #endregion

        #region IDisposable

        ///// <summary>
        ///// 
        ///// </summary>
        //~SoundEffect()
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
                    _emitter.Effects.Remove(this);
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
