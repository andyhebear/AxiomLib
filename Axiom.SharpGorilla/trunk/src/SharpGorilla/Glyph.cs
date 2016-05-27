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
using Axiom.Math;
#endregion
namespace SharpGorilla
{
    /// <summary>
    /// Texture and size information about a single character loaded from a TextureAtlas.
    /// </summary>
    public class Glyph
    {
        private Vector2[] _texCoords = new Vector2[4];
        /// <summary>
        /// 
        /// </summary>
        public DynamicBuffer<Kerning> Kerning
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector2[] TexCoords
        {
            get { return _texCoords; }
            set { _texCoords = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Real UVTop
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real UVWidth
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real UVBottom
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real UVHeight
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real UVLeft
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real UVRight
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real GlyphWidth
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real GlyphHeight
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Real GlyphAdvance
        {
            get;
            set;
        }
        public Glyph()
        {
            Kerning = new DynamicBuffer<Kerning>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftof"></param>
        /// <returns></returns>
        public Real GetKerning(char leftof)
        {
            if (Kerning.Size == 0)
                return 0;

            for (int i = 0; i < Kerning.Size; i++)
            {
                if (Kerning[i].Character == leftof)
                    return Kerning[i].kerning;
            }

            return 0;
        }
    }
}
