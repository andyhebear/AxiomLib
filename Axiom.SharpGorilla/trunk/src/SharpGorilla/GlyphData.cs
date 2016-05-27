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
using System.Collections.Generic;

using Axiom.Math;
#endregion
namespace SharpGorilla
{
    /// <summary>
    /// Collection of glyphs of the same size.
    /// </summary>
    public class GlyphData
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Glyph> Glyphs
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real RangeBegin
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real RangeEnd
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real SpaceLength
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real LineHeight
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real Baseline
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real LetterSpacing
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real MonoWidth
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public GlyphData()
        {
            Glyphs = new List<Glyph>();
        }
        /// <summary>
        /// Get a glyph (character information) from a specific character.
        /// </summary>
        /// <param name="character"></param>
        /// <returns>
        /// If the character doesn't exist then a null pointer is returned.
        /// Do not delete the Glyph pointer.
        /// </returns>
        public Glyph GetGlyph(int character)
        {
            character = character - (int)RangeBegin;
            if (character > RangeEnd)
                return null;
            return Glyphs[character];
        }
    }
}
