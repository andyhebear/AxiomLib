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
using System.IO;
using Axiom.SoundSystems.Decoders;
using Axiom.Core;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace Axiom.SoundSystems.OpenAL.OpenTK
{
	/// <summary>
	/// OpenAL sound.
	/// </summary>
	/// <remarks>If the sound shall be spatial then a mono waveform must be used
	/// to have the effect. Stereo samples are not spatially processed at all in OpenAL.
	/// </remarks>
    public class Sound : Axiom.SoundSystems.Sound
    {
        #region Constructor

        protected internal Sound(SoundContext creator, string fileName, SoundKind kind)
            : base(creator, fileName, kind)
        {
        }

        #endregion

        #region ICloneable

        public override object Clone()
        {
            Sound s = new Sound((SoundContext)Creator, FileName, Kind);
            SoundContext.CopySettings(this, s);
            s.SoundId = SoundId;

            return s;
        }

        #endregion

        #region Properties

        private int _soundId;
        /// <summary>
        /// OpenAL sound Id
        /// </summary>
        public int SoundId
        {
            get { return _soundId; }
            set { _soundId = value; }
        }

        #endregion

        #region Axiom.SoundSystems.Sound

        protected override Axiom.SoundSystems.SoundInstance CreateSoundInstance(string key)
        {
            SoundInstance sound = new SoundInstance(this, key, _soundId, _kind);
            SoundContext.CopySettings(this, sound);

            NotifyCreated(sound);

            return sound;
        }

        public override void Load()
        {
            base.Load();

            // decode the file
            WaveStream stream = DecoderManager.Instance.Decode(FileName);

            if (stream.Format == WaveFormat.PCM) // .ogg
            {
                // read the stream into a byte array
                byte[]data = new byte[stream.Data.Length];
                stream.Data.Read(data, 0, (int)stream.Data.Length);
                int size = (int)stream.Data.Length;
                int frequency = stream.Frequency;

                ALFormat wavFormat;
                if (stream.Bits == 8)
                    if (stream.Channels == 1) wavFormat = ALFormat.Mono8; else wavFormat = ALFormat.Stereo8;
                else
                    if (stream.Channels == 1) wavFormat = ALFormat.Mono16; else wavFormat = ALFormat.Stereo16;

                AL.GenBuffers(1, out _soundId);
                AL.BufferData(_soundId, wavFormat, data, size, frequency);
            }
            else if (stream.Format == WaveFormat.Wav) // .wav
            {
                using (BinaryReader reader = new BinaryReader(stream.Data))
                {
                    int channels, rate;
                    string signature = new string(reader.ReadChars(4));
                    reader.ReadInt32();
                    string format = new string(reader.ReadChars(4));
                    string formatSignature = new string(reader.ReadChars(4));

                    if (signature != "RIFF" || format != "WAVE" || formatSignature != "fmt ")
                        throw new NotSupportedException("Specified wave file is not supported.");

                    reader.ReadInt32();
                    reader.ReadInt16();
                    channels = reader.ReadInt16();
                    rate = reader.ReadInt32();
                    int byte_rate = reader.ReadInt32();
                    reader.ReadInt16();

                    ALFormat wavFormat;
                    if (reader.ReadInt16() == 8)
                        if (channels == 1) wavFormat = ALFormat.Mono8; else wavFormat = ALFormat.Stereo8;
                    else
                        if (channels == 1) wavFormat = ALFormat.Mono16; else wavFormat = ALFormat.Stereo16;

                    if (new string(reader.ReadChars(4)) != "data")
                        throw new NotSupportedException("Specified wave file is not supported.");

                    reader.ReadInt32();
                    byte[] dta = reader.ReadBytes((int)reader.BaseStream.Length);

                    AL.GenBuffers(1, out _soundId);
                    AL.BufferData(_soundId, wavFormat, dta, dta.Length, rate);
                }
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
                }

                AL.DeleteBuffers(1, ref _soundId);

                base._dispose(disposeManagedResources);
            }
        }

        #endregion
    }
}
