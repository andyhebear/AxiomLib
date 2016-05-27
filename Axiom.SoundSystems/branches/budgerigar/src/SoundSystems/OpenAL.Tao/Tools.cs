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
using Tao.OpenAl;

namespace Axiom.SoundSystems.OpenAL.Tao
{
    public static class Tools
    {
        /// <summary>
        /// Reads the last Alut error and builds a textual representation of it.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string BuildAlutErrorMessage(string fmtPrefix, params object[] args)
        {
            int err = Alut.alutGetError();
            string prefix = String.Format(fmtPrefix, args);
            string msg = String.Format(prefix + " : [{0}]{1}", err, Alut.alutGetErrorString(err));

            return msg;
        }

        ///<summary>
        /// Convert pitch value from Xna to OpenAL
        ///</summary>
        ///<remarks>
        /// Xna pitch is 0.0f for no pitch and ranges from -1.0f (down one octave) to 1.0f (up one octave)
        /// OpenAL pitch is 1.0f for no pitch, halving it means down one octave, doubling it means up one octave
        /// </para>i.e.:
        /// openALPitch = Pow(2.0, xnaPitch);
        /// xnaPitch = Log(openAlPitch, 2.0);</para>
        /// </remarks>
        /// <param name="pitch">Xna style pitch</param>
        /// <returns>OpenAL style pitch</returns>
        public static float XnaToOpenALPitch(float pitch)
        {
            return (float)System.Math.Pow(2.0, (double)pitch);
        }

        ///<summary>
        /// Convert pitch value from OpenAL to Xna
        ///</summary>
        ///<remarks>
        /// Xna pitch is 0.0f for no pitch and ranges from -1.0f (down one octave) to 1.0f (up one octave)
        /// OpenAL pitch is 1.0f for no pitch, halving it means down one octave, doubling it means up one octave
        /// </para>i.e.:
        /// openALPitch = Pow(2.0, xnaPitch);
        /// xnaPitch = Log(openAlPitch, 2.0);</para>
        /// </remarks>
        /// <param name="pitch">Xna style pitch</param>
        /// <returns>OpenAL style pitch</returns>
        public static float OpenALToXnaPitch(float pitch)
        {
            return (float)System.Math.Log((double)pitch, 2.0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public static float XnaToOpenALVolume(float volume)
        {
            return volume; // TODO: get sure that a conversion is not necessary
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public static float OpenALToXnaVolume(float volume)
        {
        	return volume; // TODO: get sure that a conversion is not necessary
        }
        
        /// <summary>
        /// Convert xna's 'distance scale' value to openal's 'rolloff factor'.
        /// </summary>
        /// <param name="distanceScale"></param>
        /// <returns></returns>
        public static float XnaDistanceScaleToOpenALRollOff(float distanceScale)
        {
        	return 1.0f / distanceScale;
        }
        
        /// <summary>
        /// Convert xna's 'doppler scale' to openal's 'doppler factor'.
        /// </summary>
        /// <remarks>
        /// For OpenAL the default doppler factor value is 1.0. 
        /// Growing it makes the effect stronger, setting it to zero effectively disables it.
        /// </remarks>
        /// <param name="dopplerScale"></param>
        /// <returns></returns>
        public static float XnaDopplerScaleToOpenALDopplerFactor(float dopplerScale)
        {
        	return dopplerScale;
        }
    }
}
