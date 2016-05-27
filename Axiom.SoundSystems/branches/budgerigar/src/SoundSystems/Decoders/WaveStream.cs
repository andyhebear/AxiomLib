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

using System.IO;

namespace Axiom.SoundSystems.Decoders
{

    /// <summary>
    /// Defines a wave stream and it's format
    /// </summary>
    public struct WaveStream
    {
        #region Fields

        private int frequency;
        private short bits;
        private short channels;
        private WaveFormat format;
        private Stream data;

        #endregion

        #region Properties

        /// <summary>
        /// The sampling frequency of the data (in Hz = samples / second)
        /// </summary>
        /// <remarks>
        /// Valid only if <see cref="Format"/> is <see cref="WaveFormat.PCM"/>
        /// </remarks>
        public int Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value;
            }
        }

        /// <summary>
        /// Sets the data 8 or 16 bits
        /// </summary>
        /// <remarks>
        /// Valid only if <see cref="Format"/> is <see cref="WaveFormat.PCM"/>
        /// </remarks>
        public short Bits
        {
            get
            {
                return bits;
            }
            set
            {
                bits = value;
            }
        }

        /// <summary>
        /// Set if it is a stereo or mono file
        /// </summary>
        /// <remarks>
        /// Valid only if <see cref="Format"/> is <see cref="WaveFormat.PCM"/>
        /// </remarks>
        public short Channels
        {
            get
            {
                return channels;
            }
            set
            {
                channels = value;
            }
        }

        /// <summary>
        /// The format of data in the <see cref="Data"/> stream.
        /// </summary>
        public WaveFormat Format
        {
            get
            {
                return format;
            }
            set
            {
                format = value;
            }
        }

        /// <summary>
        /// Stream containing wave data from decoders. 
        /// The format of the data is specified by the <see cref="Format"/> property.
        /// </summary>
        public Stream Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        #endregion
    }
}
