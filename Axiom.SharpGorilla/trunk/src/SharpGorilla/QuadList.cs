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
using Axiom.Core;
#endregion
namespace SharpGorilla
{
    /// <summary>
    /// "ManualObject" like class to quickly draw rectangles, gradients, sprites and borders.
    /// </summary>
    public class QuadList
    {
        /// <summary>
        /// a list of quads
        /// </summary>
        protected DynamicBuffer<Quad> _quads;
        /// <summary>
        /// A list of vertices
        /// </summary>
        internal DynamicBuffer<Vertex> _vertices;
        internal bool _isDirty;
        protected Vector2 _whiteUV;
        protected Layer _layer;

        internal QuadList(Layer layer)
        {
        }
        /// <summary>
        /// Clear everything and start again
        /// </summary>
        public void Begin()
        {
        }

        /// <summary>
        ///  Draw a rectangle sized w,h at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="color"></param>
        public void Rectangle(Real x, Real y, Real w, Real h)
        {
            Rectangle(x, y, w, h, ColorEx.White);
        }
        /// <summary>
        ///  Draw a rectangle sized w,h at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="color"></param>
        public void Rectangle(Real x, Real y, Real w, Real h, ColorEx color)
        {
        }
        /// <summary>
        /// Draw a gradient rectangle sized w,h at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="color"></param>
        public void Gradient(Real x, Real y, Real w, Real h)
        {
            Gradient(x, y, w, h, ColorEx.White);
        }
        /// <summary>
        /// Draw a gradient rectangle sized w,h at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="color"></param>
        public void Gradient(Real x, Real y, Real w, Real h, Colors.Color color)
        {
            Gradient(x, y, w, h, Converter.ToWebColor(color));
        }
        /// <summary>
        /// Draw a gradient rectangle sized w,h at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="color"></param>
        public void Gradient(Real x, Real y, Real w, Real h, ColorEx color)
        {
        }

        /// <summary>
        /// Draw a sprite sized w,h at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="sprite"></param>
        public void Sprite(Real x, Real y, Real w, Real h, Sprite sprite)
        {
        }
        /// <summary>
        /// Draw a border sized w,h at x,y of a thickness
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="thickness"></param>
        public void Border(Real x, Real y, Real w, Real h, Real thickness)
        {
            Border(x, y, w, h, thickness, ColorEx.White);
        }
        /// <summary>
        /// Draw a border sized w,h at x,y of a thickness
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="thickness"></param>
        /// <param name="color"></param>
        public void Border(Real x, Real y, Real w, Real h, Real thickness, Colors.Color color)
        {
            Border(x, y, w, h, thickness, Converter.ToWebColor(color));
        }
        /// <summary>
        /// Draw a border sized w,h at x,y of a thickness
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="thickness"></param>
        /// <param name="color"></param>
        public void Border(Real x, Real y, Real w, Real h, Real thickness, ColorEx color)
        {
        }
        /// <summary>
        /// Draw a border sized w,h at x,y of a thickness
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="thickness"></param>
        /// <param name="northColor"></param>
        /// <param name="eastColor"></param>
        /// <param name="southColor"></param>
        /// <param name="westColor"></param>
        public void Border(Real x, Real y, Real w, Real h, Real thickness, ColorEx northColor, 
            ColorEx eastColor, ColorEx southColor, ColorEx westColor)
        {
        }
        /// <summary>
        /// Draw a border sized w,h at x,y of a thickness
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="thickness"></param>
        /// <param name="northColor"></param>
        /// <param name="eastColor"></param>
        /// <param name="southColor"></param>
        /// <param name="westColor"></param>
        public void Border(Real x, Real y, Real w, Real h, Real thickness, Colors.Color northColor,
            Colors.Color eastColor, Colors.Color southColor, Colors.Color westColor)
        {
            Border(x, y, w, h, thickness, Converter.ToWebColor(northColor), Converter.ToWebColor(eastColor), Converter.ToWebColor(southColor), Converter.ToWebColor(westColor));
        }
        /// <summary>
        /// Draw a glyph with a custom size.
        /// </summary>
        /// <param name="glyphDataIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="character"></param>
        /// <param name="color"></param>
        public void Glyph(int glyphDataIndex, Real x, Real y, Real w, Real h, char character, ColorEx color)
        {
        }
        /// <summary>
        /// Draw a glyph with a custom size.
        /// </summary>
        /// <param name="glyphDataIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="character"></param>
        /// <param name="color"></param>
        public void Glyph(int glyphDataIndex, Real x, Real y, Real w, Real h, char character, Colors.Color color)
        {
            Glyph(glyphDataIndex, x, y, w, h, character, Converter.ToWebColor(color));
        }
        /// <summary>
        /// Draw a glyph
        /// </summary>
        /// <param name="glyphDataIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="character"></param>
        /// <param name="color"></param>
        public void Glyph(int glyphDataIndex, Real x, Real y, char character, ColorEx color)
        {
        }
        /// <summary>
        /// Draw a glpyh
        /// </summary>
        /// <param name="glyphDataIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="character"></param>
        /// <param name="color"></param>
        public void Glyph(int glyphDataIndex, Real x, Real y, char character, Colors.Color color)
        {
            Glyph(glyphDataIndex, x, y, character, Converter.ToWebColor(color));
        }
        /// <summary>
        /// Stop drawing and calculate vertices.
        /// </summary>
        public void End()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public void Redraw()
        {
        }
    }
}
