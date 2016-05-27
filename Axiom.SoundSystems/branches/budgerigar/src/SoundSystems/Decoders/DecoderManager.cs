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
using System.IO;
using Axiom.Core;

namespace Axiom.SoundSystems.Decoders
{
    /// <summary>
    /// Loading and decoding of audio files
    /// </summary>
    public sealed class DecoderManager : Singleton<DecoderManager>, IDisposable
    {

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private DecoderManager()
            : base()
        {
            // register the built-in decoders
            decoderMap.Add("wav", typeof(DecoderWAV));
            decoderMap.Add("ogg", typeof(DecoderOGG));

            LogSupportedFormats();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Map file extensions to decoder types.
        /// </summary>
        private Dictionary<string, Type> decoderMap = new Dictionary<string, Type>();

        /// <summary>
        /// Map of the available file extensions to relevant decoder types. The extension (key) is without a leading dot.
        /// </summary>
        public Dictionary<string, Type> DecoderMap
        {
            get
            {
                return decoderMap;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads a file with a given name from the engine resources and decodes it
        /// </summary>
        /// <param name="filename">The name of the file to load</param>
        /// <returns>A stream with the decoded file contents</returns>
        public WaveStream Decode(string filename)
        {
            // load the stream with the resource manager
            
            Stream originalFile = ResourceGroupManager.Instance.OpenResource(filename);

            // extract the file extension
            string extension = Path.GetExtension(filename).ToLower().Replace(".", String.Empty);

            // instantiate appropriate decoder
            IDecoder decoder;
            try
            {
                decoder = (IDecoder)Activator.CreateInstance(DecoderManager.Instance.DecoderMap[extension]);
            }
            catch (Exception e)
            {
                throw new DecoderManagerException(String.Format("Failed to load decoder for file: '{0}'", filename), e);
            }

            // return the decoded file
            return decoder.Decode(originalFile);
        }

        /// <summary>
        /// Add a sound decoder to be used with a particular file extension.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="decoderType"></param>
        public void AddDecoder(string fileExtension, Type decoderType)
        {
            // parameter check
            if (fileExtension == null || fileExtension == String.Empty)
                throw new ArgumentException("File extension not specified");

            if (decoderType == null || decoderType.GetInterface(typeof(IDecoder).FullName) != typeof(IDecoder))
                throw new ArgumentException("Decoder type not specified");

            // already registered?
            fileExtension = fileExtension.ToLower();
            if (DecoderMap.ContainsKey(fileExtension))
                throw new DecoderManagerException(String.Format("Decoder for file extension '{0}' already registered: '{1}'", fileExtension, DecoderMap[fileExtension].ToString()));

            // add decoder
            DecoderMap.Add(fileExtension, decoderType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileExtension"></param>
        public void RemoveDecoder(string fileExtension)
        {
            if (!DecoderMap.ContainsKey(fileExtension))
                throw new DecoderManagerException(String.Format("File extension '{0}' not registered, can't remove decoder.", fileExtension));

            DecoderMap.Remove(fileExtension);
        }

        /// <summary>
        /// Writes a line to the log with all loaded decoder types
        /// </summary>
        public void LogSupportedFormats()
        {
            string extensions = String.Empty;

            foreach (string ext in decoderMap.Keys)
            {
                extensions += ext + ", ";
            }

            extensions = extensions.TrimEnd(new char[] { ',', ' ' });

            LogManager.Instance.Write("SoundSystems: Support for file types '{0}' is loaded", extensions);
        }

        #endregion

    }
}
