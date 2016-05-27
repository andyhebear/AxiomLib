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
using Microsoft.Xna.Framework.Audio;

namespace Axiom.SoundSystems.Xna.Simple
{
	/// <summary>
	/// Xna sound template.
	/// </summary>
    public class Sound : Axiom.SoundSystems.Sound
    {
        #region Constructors

        protected internal Sound(SoundContext creator, string fileName, SoundKind kind)
            : base(creator, fileName, kind)
        {
        }

        #endregion

        #region Properties

        protected SoundEffect _soundEffect;
        /// <summary>
        /// The underlying Xna <see cref="SoundEffect"/>
        /// </summary>
        public SoundEffect SoundEffect
        {
            get { return _soundEffect; }
            set { _soundEffect = value; }
        }

        /// <summary>
        /// Get the duration of the underlying <see cref="SoundEffect"/>
        /// </summary>
        public TimeSpan Duration
        {
            get { return _soundEffect.Duration; }
        }

        #endregion

        #region Protected

        /// <summary>
        /// 
        /// </summary>
        public override void Load()
        {
            base.Load();

            string xnaFileName = FileName;
            if (xnaFileName.EndsWith(".xnb"))
                xnaFileName = xnaFileName.Remove(xnaFileName.Length - 4);

            _soundEffect = ((SoundContext)Creator).Content.Load<SoundEffect>(xnaFileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override Axiom.SoundSystems.SoundInstance CreateSoundInstance(string key)
        {
            SoundInstance sound = new SoundInstance(this, key, _kind, _soundEffect);

            NotifyCreated(sound);

            return sound;
        }

        #endregion

        #region ICloneable

        public override object Clone()
        {
            Sound s = new Sound((SoundContext)Creator, FileName, Kind);
            SoundContext.CopySettings(this, s);
            s.SoundEffect = SoundEffect;

            return s;
        }

        #endregion
    }
}
