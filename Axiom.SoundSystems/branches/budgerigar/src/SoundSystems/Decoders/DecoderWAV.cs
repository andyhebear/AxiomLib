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
    /// The Wave decoder
    /// </summary>
    public class DecoderWAV : IDecoder
    {
        /// <summary>
        /// Decodes the Wave file
        /// </summary>
        /// <remarks>
        /// This decoder only passes the input stream to the <see cref="WaveStream"/>
        /// </remarks>
        /// <param name="input">The incoming stream</param>
        /// <returns>The outgoing stream</returns>
        public WaveStream Decode(Stream input)
        {
            WaveStream wf = new WaveStream();
            wf.Format = WaveFormat.Wav;
            wf.Data = input;

            return wf;
        }
    }
}
