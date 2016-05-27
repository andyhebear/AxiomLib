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
using Axiom.SoundSystems.Decoders;
using Tao.OpenAl;

namespace Axiom.SoundSystems.OpenAL.Tao
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

            // setup OpenAL buffers
            byte[] data;

            if (stream.Format == WaveFormat.Wav)
            {
                // we have a full wave file with headers
                // read the stream into a byte array
                // and create a OpenAL buffer from that file image

                int fileLength = (int)stream.Data.Length;

                data = new byte[fileLength];
                stream.Data.Read(data, 0, fileLength);

                unsafe
                {
                    fixed (byte* pdata = data)
                    {
                        _soundId = Alut.alutCreateBufferFromFileImage((IntPtr)pdata, fileLength);
                    }
                }
                if (_soundId == Al.AL_NONE)
                {
                    throw new SoundSystemsException(Tools.BuildAlutErrorMessage("Loading Sound '{0}' failed", FileName));
                }
            }
            else if (stream.Format == WaveFormat.PCM)
            {
                // we only have raw PCM encoded data

                int format, size, frequency;

                // read the stream into a byte array
                data = new byte[stream.Data.Length];
                stream.Data.Read(data, 0, (int)stream.Data.Length);
                size = (int)stream.Data.Length;
                frequency = stream.Frequency;
                format = 0;

                // get the data format from the Channels and Bits properties
                switch (stream.Bits)
                {
                    case 8:
                        switch (stream.Channels)
                        {
                            case 1:
                                format = Al.AL_FORMAT_MONO8;
                                break;
                            case 2:
                                format = Al.AL_FORMAT_STEREO8;
                                break;
                        }
                        break;
                    case 16:
                        switch (stream.Channels)
                        {
                            case 1:
                                format = Al.AL_FORMAT_MONO16;
                                break;
                            case 2:
                                format = Al.AL_FORMAT_STEREO16;
                                break;
                        }
                        break;
                }

                // create and fill the buffer
                Al.alGenBuffers(1, out _soundId);
                Al.alBufferData(_soundId, format, data, size, frequency);
            }
            else
            {
                throw new SoundSystemsException(String.Format("Unknown wave format obtained from decoder: '{0}'", stream.Format));
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

                Al.alDeleteBuffers(1, ref _soundId);

                base._dispose(disposeManagedResources);
            }
        }

        #endregion
    }
}
