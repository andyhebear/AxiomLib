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
using Axiom.Math;
using Microsoft.Xna.Framework.Audio;

namespace Axiom.SoundSystems.Xna.Simple
{
	/// <summary>
	/// Xna sound instance.
	/// </summary>
	public class SoundInstance : Axiom.SoundSystems.SoundInstance
	{
		#region Constructor

		internal SoundInstance(Sound creator, string name, SoundKind kind, SoundEffect soundEffect)
			: base(creator, name, kind)
		{
			_listener = new AudioListener();
			_emitter = new AudioEmitter();
			_soundEffect = soundEffect;

			// copy sound emitter properties
			SoundContext.CopySettings(creator, this);
		}

		#endregion

		#region Fields

		protected AudioEmitter _emitter;

		protected AudioListener _listener;

		protected SoundEffect _soundEffect;
		
		#endregion

		#region Properties

		/// <summary>
		/// The underlying Xna sound instance
		/// </summary>
		public SoundEffectInstance EffectInstance { get; protected set; }

		#endregion

		#region ISoundEmitter

		/// <summary>
		/// 
		/// </summary>
		public override float Volume
		{
			set
			{
				base.Volume = value;
				if (EffectInstance != null)
					EffectInstance.Volume = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override float Pan
		{
			set
			{
				base.Pan = value;
				if (EffectInstance != null)
					EffectInstance.Pan = value; //TODO: causes some troubles
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override float Pitch
		{
			set
			{
				base.Pitch = value;
				if (EffectInstance != null)
					EffectInstance.Pitch = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public override void Play()
		{
			base.Play();

			if (EffectInstance == null || EffectInstance.IsDisposed)
			{
				// init the effect instance
				switch (Kind)
				{
					case SoundKind.Simple:
						EffectInstance = _soundEffect.CreateInstance();
						EffectInstance.IsLooped = Loop;
						EffectInstance.Volume = Volume;
						EffectInstance.Pitch = Pitch;
						EffectInstance.Pan = Pan;
						EffectInstance.Play();
						break;
					case SoundKind.Spatial:
						// init spatial info
						ConvertListener(ParentSound.Creator.Listener, _listener);
						ConvertEmitter(this, _emitter);
						SoundEffect.DopplerScale = DopplerScale;
						SoundEffect.DistanceScale = DistanceScale;
						SoundEffect.SpeedOfSound = SpeedOfSound;
						// play
						EffectInstance = _soundEffect.CreateInstance();
						EffectInstance.IsLooped = Loop;
						EffectInstance.Volume = Volume;
						EffectInstance.Pitch = Pitch;
						EffectInstance.Pan = Pan; // (pan while applying 3d too?)
						EffectInstance.Apply3D(_listener, _emitter);
						EffectInstance.Play();
						break;
					default:
						throw new SoundSystemsException("Unknown sound kind: " + Kind);
				}
			}
			else
			{
				EffectInstance.Play();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Pause()
		{
			base.Pause();
			if (EffectInstance != null)
				EffectInstance.Pause();
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Stop()
		{
			base.Stop();
			if (EffectInstance != null)
				EffectInstance.Stop();
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Update()
		{
			if (EffectInstance != null)
			{
				// whether played to the end
				if (EffectInstance.State == Microsoft.Xna.Framework.Audio.SoundState.Stopped && !_explicitStop || EffectInstance.IsDisposed)
				{
					ParentSound.Creator.QueuedForDisposal.Add(this);
					return;
				}

				if (Kind == SoundKind.Spatial && State == SoundState.Playing)
				{
					// update spatial info
					ConvertListener(ParentSound.Creator.Listener, _listener);
					ConvertEmitter(this, _emitter);

					EffectInstance.Apply3D(_listener, _emitter);
				}
			}
			
			base.Update();
		}

		protected void ConvertListener(ISoundListener source, AudioListener target)
		{
			target.Position = Tools.Convert.Vector3(source.Position);
			target.Velocity = Tools.Convert.Vector3(source.Velocity);
			target.Forward = Tools.Convert.Vector3(source.Forward);
			target.Up = Tools.Convert.Vector3(source.Up);
		}

		protected void ConvertEmitter(ISoundEmitter source, AudioEmitter target)
		{
			target.Position = Tools.Convert.Vector3(source.Position);
			target.Velocity = Tools.Convert.Vector3(source.Velocity);
			target.DopplerScale = source.DopplerScale;
			target.Up = Tools.Convert.Vector3(source.Up);
			target.Forward = Tools.Convert.Vector3(source.Forward);
		}
		
		#endregion

		#region IDisposable

		/// <summary>
		/// Finalizer
		/// </summary>
		~SoundInstance()
		{
			_dispose(false);
		}

		protected override void _dispose(bool disposeManagedResources)
		{
			if (!IsDisposed)
			{
				if (disposeManagedResources)
				{
					if (EffectInstance != null && ! EffectInstance.IsDisposed)
						EffectInstance.Dispose();
				}

				base._dispose(disposeManagedResources);
			}
		}

		#endregion
	}
}
