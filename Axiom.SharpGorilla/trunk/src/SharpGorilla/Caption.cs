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
    /// A single line piece of text
    /// </summary>
    public class Caption
    {
        protected Layer _layer;
        protected GlyphData _glyphData;
        protected Real _left;
        protected Real _top;
        protected Real _width;
        protected Real _height;
        protected TextAlignment _algignment;
        protected VerticalAlignment _verticalAlignment;
        protected string _text;
        protected ColorEx _color;
        protected ColorEx _backgroundColor;
        internal bool _isDirty;
        internal DynamicBuffer<Vertex> _vertices = new DynamicBuffer<Vertex>();
        protected int _clippedLeftIndex;
        protected int _clippedRightIndex;

        /// <summary>
        /// Gets or sets where the text should be drawn vertically.
        /// Note:<para></para>
        /// If the TextAlignment is Right Aligned, then this will be the right-side of the last character drawn (with in width limits).<para></para>
        /// If the TextAlignment is Centre Aligned, then this will be the center of the drawn text drawn (with in width limits).
        /// </summary>
        public Real Left
        {
            get { return _left; }
            set
            {
                _left = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or sets where the text should be drawn vertically.
        /// </summary>
        public Real Top
        {
            get { return _top; }
            set
            {
                _top = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        ///  Gets or sets the maximum width of the text can draw into.
        /// </summary>
        public Real Width
        {
            get { return _width; }
            set
            {
                _width = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or sets the maximum height of the text can draw into.
        /// </summary>
        public Real Height
        {
            get { return _height; }
            set
            {
                _height = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or sets the text indented to show.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or sets the alignment of text.
        /// </summary>
        public TextAlignment HorizontalAlign
        {
            get { return _algignment; }
            set
            {
                _algignment = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or sets the vertical alignment of text.
        /// </summary>
        public VerticalAlignment VerticalAlign
        {
            get { return _verticalAlignment; }
            set
            {
                _verticalAlignment = value;
                _isDirty = true;
                _layer.MarkDirty();

            }
        }
        /// <summary>
        /// Gets the index (character position), of the first character that could not be drawn due to the limits<para></para>
        /// of the width of the text, to the left.<para></para>
        /// Note:<para></para>
        /// The index if the first clipped character, or -1 if there was no clipping and<para></para>
        /// the text was drawn fully within the given space.<para></para>
        /// </summary>
        public int ClippedLeftIndex
        {
            get { return _clippedLeftIndex; }
        }
        /// <summary>
        /// Gets the index (character position), of the first character that could not be drawn due to the limits<para></para>
        /// of the width of the text, to the right.<para></para>
        /// Note:<para></para>
        /// The index if the first clipped character, or -1 if there was no clipping and<para></para>
        /// the text was drawn fully within the given space.<para></para>
        /// </summary>
        public int ClippedRightIndex
        {
            get { return _clippedRightIndex; }
        }
        /// <summary>
        /// Gets or sets the text color
        /// </summary>
        public ColorEx Color
        {
            get { return _color; }
            set
            {
				_color = new ColorEx( value.r, value.g, value.b, value.a );
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Sets the color as web color
        /// </summary>
        public Colors.Color WebColor
        {
            set
            {
				ColorEx val = Converter.ToWebColor( value );
				_color = new ColorEx( val.r, val.g, val.b, val.a );
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or sets the background color
        /// </summary>
        public ColorEx BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Sets the Background color as web color
        /// </summary>
        public Colors.Color BackgroundWebColor
        {
            set
            {
                if (value == Colors.Color.None)
                    _backgroundColor.a = 0;
                else
                    _backgroundColor = Converter.ToWebColor(value);

                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="glyphDataIndex"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="caption"></param>
        /// <param name="parent"></param>
        internal Caption(int glyphDataIndex, Real left, Real top, string caption, Layer parent)
        {
            _isDirty = true;
            _layer = parent;
            _layer.MarkDirty();
            _glyphData = _layer.GetGlyphData(glyphDataIndex);
            _left = left;
            _top = top;
            _width = 0;
            _height = 0;
            _text = caption;
			_color = ColorEx.White;
            _backgroundColor.a = 0;
            _algignment = TextAlignment.Left;
            _verticalAlignment = VerticalAlignment.Top;

        }
        /// <summary>
        /// Does a set of coordinates lie within this caption?
        /// </summary>
        /// <param name="coordinates">coordinates to check for intersection</param>
        /// <returns>true if intersects</returns>
        public bool Intersects(Vector2 coordinates)
        {
            return ((coordinates.x >= _left && coordinates.x <= _left + _width) && (coordinates.y >= _top && coordinates.y <= _top + _height));
        }
        /// <summary>
        /// Set the maximum width and height of the text can draw into.
        /// </summary>
        /// <param name="width">width of the text</param>
        /// <param name="height">height of the text</param>
        public void SetSize(Real width, Real height)
        {
            _width = width;
            _height = height;
            _isDirty = true;
            _layer.MarkDirty();
        }
        internal void CalculateDrawSize(out Vector2 retSize)
        {
            Real cursor = 0;
            Real kerning = 0;
            char thisChar = (char)0;
            char lastChar = (char)0;
            Glyph glyph = null;
            retSize = new Vector2();
            retSize.x = 0;
            retSize.y = _glyphData.LineHeight;

            for (int i = 0; i < Text.Length; i++)
            {
                thisChar = Text[i];
                if (thisChar == ' ')
                {
                    lastChar = thisChar;
                    cursor += _glyphData.SpaceLength;
                    continue;
                }

                if (thisChar < _glyphData.RangeBegin || thisChar > _glyphData.RangeEnd)
                {
                    lastChar = (char)0;
                    continue;
                }

                glyph = _glyphData.GetGlyph((int)thisChar);
                kerning = glyph.GetKerning(lastChar);
                if (kerning == 0)
                    kerning = _glyphData.LetterSpacing;

                cursor += glyph.GlyphAdvance + kerning;
                lastChar = thisChar;
            }//end for i

            retSize.x = cursor - kerning;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Redraw()
        {
            if (!_isDirty)
                return;

            _vertices.Clear();

            Vector2 uv = _layer.SolidUV;
            Vertex temp = new Vertex();
            if (BackgroundColor.a > 0)
            {
                Vector2 a, b, c, d = Vector2.Zero;
                a.x = _left; a.y = _top;
                b.x = _left + _width; b.y = _top;
                c.x = _left; c.y = _top + _height;
                d.x = _left + _width; d.y = c.y = _top + _height;
                
                Helper.AddTriangle(ref _vertices, temp, c, b, a, uv, _backgroundColor);
                Helper.AddTriangle(ref _vertices, temp, c, d, b, uv, _backgroundColor);
            }

            Real left = 0, top = 0, right = 0, bottom = 0, cursorX = 0, cursorY = 0, kerning = 0, texelOffsetX = _layer.TexelX, texelOffsetY = _layer.TexelY;
            Vector2 knownSize = Vector2.Zero;
            Glyph glyph = null;
            bool clipleft = false, clipRight = false;
            Real clipLeftPos = 0, clipRightPos = 0;

            if (_algignment == TextAlignment.Left)
            {
                cursorX = _left;
                if (_width != 0)
                {
                    clipRight = true;
                    clipRightPos = _left + _width;
                }
            }
            else if (_algignment == TextAlignment.Center)
            {
                CalculateDrawSize(out knownSize);
                cursorX = _left + (_width * 0.5f) - (knownSize.x * 0.5f);

                if (_width != 0)
                {
                    clipleft = true;
                    clipLeftPos = _left;
                    clipRight = true;
                    clipRightPos = _left + _width;
                }
            }
            else if (_algignment == TextAlignment.Right)
            {
                CalculateDrawSize(out knownSize);
                cursorX = _left + _width - knownSize.x;
                if (_width != 0)
                {
                    clipleft = true;
                    clipLeftPos = _left;
                }
            }

            if (_verticalAlignment == VerticalAlignment.Top)
            {
                cursorY = _top;
            }
            else if (_verticalAlignment == VerticalAlignment.Middle)
            {
                cursorY = _top + (_height * 0.5f) - (_glyphData.LineHeight * 0.5f);
            }
            else if (_verticalAlignment == VerticalAlignment.Bottom)
            {
                cursorY = _top + _height - _glyphData.LineHeight;
            }

            char thisChar = (char)0;
            char lastChar = (char)0;
            _clippedLeftIndex = -1;
            _clippedRightIndex = -1;

            for (int i = 0; i < _text.Length; i++)
            {
                thisChar = _text[i];

                if (thisChar == ' ')
                {
                    lastChar = thisChar;
                    cursorX += _glyphData.SpaceLength;
                    continue;
                }
                if (thisChar < _glyphData.RangeBegin || thisChar > _glyphData.RangeEnd)
                {
                    lastChar = (char)0;
                    continue;
                }

                glyph = _glyphData.GetGlyph((int)thisChar);
                kerning = glyph.GetKerning(lastChar);
                if (kerning == 0)
                    kerning = _glyphData.LetterSpacing;

                left = cursorX - texelOffsetX;
                top = cursorY - texelOffsetY;
                right = left + glyph.GlyphWidth + texelOffsetX;
                bottom = top + glyph.GlyphHeight + texelOffsetY;

                if (clipleft)
                {
                    if (left < clipLeftPos)
                    {
                        if (_clippedLeftIndex == -1)
                            _clippedLeftIndex = i;

                        cursorX += glyph.GlyphAdvance + kerning;
                        lastChar = thisChar;
                        continue;
                    }
                }

                if (clipRight)
                {
                    if (right > clipRightPos)
                    {
                        if (_clippedRightIndex == -1)
                            _clippedRightIndex = i;

                        cursorX += glyph.GlyphAdvance + kerning;
                        lastChar = thisChar;
                        continue;
                    }
                }

                // Triangle A
                Helper.AddVertex(ref _vertices, temp, left, bottom, glyph.TexCoords[(int)QuadCorner.BottomLeft], _color);  // Left/Bottom  3
                Helper.AddVertex(ref _vertices, temp, right, top, glyph.TexCoords[(int)QuadCorner.TopRight], _color);    // Right/Top    1
                Helper.AddVertex(ref _vertices, temp, left, top, glyph.TexCoords[(int)QuadCorner.TopLeft], _color);     // Left/Top     0

                // Triangle B
                Helper.AddVertex(ref _vertices, temp, left, bottom, glyph.TexCoords[(int)QuadCorner.BottomLeft], _color);  // Left/Bottom  3
                Helper.AddVertex(ref _vertices, temp, right, bottom, glyph.TexCoords[(int)QuadCorner.BottomRight], _color); // Right/Bottom 2
                Helper.AddVertex(ref _vertices, temp, right, top, glyph.TexCoords[(int)QuadCorner.TopRight], _color);    // Right/Top    1


                cursorX += glyph.GlyphAdvance + kerning;
                lastChar = thisChar;
            }//end for

            _isDirty = false;
        }
    }
}
