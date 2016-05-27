#region License
/*
    Gorilla
    -------
    
    Copyright (c) 2010 Robin Southern
 
    This is a c# (Axiom) port of Gorrilla, developed by Robin Southern, ported by me (bostich)

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

#region SVN Version Information
// <file>
//     <id value="$Id: 2118 2010-09-26 23:56:56Z bostich $"/>
// </file>
#endregion SVN Version Information

#region Namespace Declarations

using System;
using SharpGorilla.Colors;
using Axiom.Core;
using Axiom.Math;

#endregion
namespace SharpGorilla
{
    public class Converter
    {
        public static Real Inv255 = new Real(0.00392156863);
        public static Double InvHight = 0.00392156863;

        /// <summary>
        /// Convert three/four RGBA values into an ColorEx
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ColorEx ToRGB(byte r, byte g, byte b)
        {
            return ToRGB(r,g,b,255);
        }
        /// <summary>
        /// Convert three/four RGBA values into an ColorEx
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static ColorEx ToRGB(byte r, byte g, byte b, byte a)
        {
            double av = a *InvHight;
            double gv = g * InvHight;
            double rv = r * InvHight;
            double bv = b * InvHight;
			ColorEx ret = new ColorEx( new Real( r ) * Inv255, new Real( g ) * Inv255, new Real( b ) * Inv255, new Real( a ) * Inv255 );
			//ColorEx ret = new ColorEx( new Real( a ) * Inv255, new Real( r ) * Inv255, new Real( r ) * Inv255, new Real( b ) * Inv255 );
            return ret;
        }

        /// <summary>
        /// Turn a webcolor from the SharpGorilla.Colors.Color enum into an ColorEx
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static ColorEx ToWebColor( Color color)
        {
            return ToWebColor(color,1.0f);
        }

        /// <summary>
        /// Turn a webcolor from the SharpGorilla.Colors.Color enum into an ColorEx
        /// </summary>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
		public static ColorEx ToWebColor( Color color, float alpha )
		{
			int intCol = (int)color;
			ColorEx ret = new ColorEx();
			ret.b = ( intCol & 0xFF ) * Inv255;
			ret.g = ( ( intCol >> 8 ) & 0xFF ) * Inv255;
			ret.r = ( ( intCol >> 16 ) & 0xFF ) * Inv255;
			ret.a = alpha;
			return ret;
		}
    }
}
