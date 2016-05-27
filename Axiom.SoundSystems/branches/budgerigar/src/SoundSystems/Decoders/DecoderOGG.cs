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
using Axiom.Core;
using csvorbis;

namespace Axiom.SoundSystems.Decoders
{
    /// <summary>
    /// The OGG-Vorbis decoder
    /// </summary>
    public class DecoderOGG : IDecoder
    {
        /// <summary>
        /// Decodes the ogg-vorbis file
        /// </summary>
        /// <param name="input">Stream of the ogg-vorbis file</param>
        /// <returns>PCM-Wave version of the input</returns>
        public WaveStream Decode(Stream input)
        {
            MemoryStream output = new MemoryStream();
            WaveStream wf = new WaveStream();

            VorbisFile vf = new VorbisFile((FileStream)input, null, 0);
            Info inf = vf.getInfo(-1);

            wf.Channels = (short)inf.channels;
            wf.Frequency = inf.rate;
            wf.Bits = 16;

            LogManager.Instance.Write("SoundSystems: File is Ogg Vorbis " + inf.version.ToString() + " " + inf.rate.ToString() + "Hz, " + inf.channels.ToString() + " channels");

            int bufferlen = 4096;
            int result = 1;
            byte[] buffer = new byte[bufferlen];
            int[] section = new int[1];

            while (result != 0)
            {
                result = vf.read(buffer, bufferlen, 0, 2, 1, section);
                output.Write(buffer, 0, result);
            }

            output.Seek(0, SeekOrigin.Begin);
            wf.Format = WaveFormat.PCM;
            wf.Data = output;

            return wf;
        }
    }
}
