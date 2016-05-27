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

using Axiom.Core;
using Axiom.Math;
#endregion
namespace SharpGorilla
{
    /// <summary>
    /// A multi-line collection of text formatted by a light markup language, that can
    /// switch colours, change to monospace and insert sprites directly into the text.
    /// </summary>
    public class MarkupText
    {
        protected Layer _layer;
        protected GlyphData _defaultGlyphData;
        protected Real _left;
        protected Real _top;
        protected Real _width;
        protected Real _height;
        protected string _text;
        protected ColorEx _backgroundColor;
        internal bool _isDirty;
        internal bool _isTextDirty;
        protected DynamicBuffer<Character> _characters = new DynamicBuffer<Character>();
        internal DynamicBuffer<Vertex> _vertices = new DynamicBuffer<Vertex>();
        protected int _clippedIndex;

        /// <summary>
        /// Gets or sets where the text should drawn vertically
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
        /// Gets or sets where the text should drawn vertically
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
        /// Gets or sets the maximum width of the text can draw into.
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
        /// Gets or sets the text to show.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _isTextDirty = true;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or set the background color
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
        /// Gets or set the background color as web color
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
        /// <param name="defaultGlyphIndex"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="text"></param>
        /// <param name="parent"></param>
        internal MarkupText(int defaultGlyphIndex, Real left, Real top, string text, Layer parent)
        {
            _layer = parent;
            _isTextDirty = true;
            _isDirty = true;
            _layer.MarkDirty();
            _defaultGlyphData = _layer.GetGlyphData(defaultGlyphIndex);
            _left = left;
            _top = top;
            _width = 0;
            _height = 0;
            _text = text;
            _backgroundColor = ColorEx.White;
            _backgroundColor.a = 0;
        }
        /// <summary>
        /// Sets the maximum width and height of the text can draw into.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSize(Real width, Real height)
        {
            _width = width;
            _height = height;
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Redraw the text
        /// Note:<para></para>
        /// This shouldn't be need to be called by the user.
        /// </summary>
        public void Redraw()
        {
            if (!_isDirty)
                return;

            _vertices.Clear();

            Vertex temp = new Vertex();

            for (int i = 0; i < _characters.Size; i++)
            {
                Helper.AddQuad2(ref _vertices, temp, _characters[i].Position, _characters[i].Color, _characters[i].UV);
            }
            _isDirty = false;
        }
        /// <summary>
        /// 
        /// </summary>
        public void CalculateCharacters()
        {
            if (!_isTextDirty)
                return;

            Real cursorX = _left, cursorY = _top, kerning = 0, texelOffsetX = _layer.TexelX, texelOffsetY = _layer.TexelY, right = 0, bottom = 0, left = 0, top = 0;
            char thisChar = (char)0;
            char lastChar = (char)0;

            Glyph glyph = null;

            _characters.Clear();

            bool markupMode = false;
            ColorEx color = _layer.GetMarkupColor(0);
            bool fixedWidth = false;

            GlyphData glyphData = _defaultGlyphData;
            Real lineHeight = glyphData.LineHeight;

            for (int i = 0; i < Text.Length; i++)
            {
                thisChar = Text[i];

                if (thisChar == ' ')
                {
                    lastChar = thisChar;
                    cursorX += glyphData.SpaceLength;
                    continue;
                }

                if (thisChar == '\n')
                {
                    lastChar = thisChar;
                    cursorX = _left;
                    cursorY += lineHeight;
                    lineHeight = glyphData.LineHeight;
                    continue;
                }
                if (thisChar < glyphData.RangeBegin || thisChar > glyphData.RangeEnd)
                {
                    lastChar = (char)0;
                    continue;
                }

                if (thisChar == '%' && !markupMode)
                {
                    markupMode = true;
                    continue;
                }

                if (markupMode)
                {
                    if (thisChar == '%')
                    {
                        //escape character
                    }
                    else
                    {
                        markupMode = false;
                        if (thisChar >= (int)'0' && thisChar <= (int)'9')
                        {
                            color = _layer.GetMarkupColor(thisChar - 48);
                        }
                        else if (thisChar == 'R' || thisChar == 'r')
                        {
                            color = _layer.GetMarkupColor(0);
                        }
                        else if (thisChar == 'M' || thisChar == 'm')
                        {
                            fixedWidth = !fixedWidth;
                        }
                        else if (thisChar == '@')
                        {
                            markupMode = false;
                            bool foundIt = false;
                            int begin = i;
                            while (i < Text.Length)
                            {
                                if (Text[i] == '%')
                                {
                                    foundIt = true;
                                    break;
                                }
                                i++;
                            }

                            if (!foundIt)
                                return;

                            int index = int.Parse(Text.Substring(begin + 1, i - begin - 1));
                            glyphData = _layer.GetGlyphData(index);
                            if (glyphData == null)
                                return;
                            // TODO: Check against line height?
                            //lineHeight = System.Math.Max(lineHeight, glyphData.LineHeight);
							lineHeight = System.Math.Max( cursorX == _left ? 0 : lineHeight, glyphData.LineHeight );

                            continue;
                        }
                        else if (thisChar == ':')
                        {
                            markupMode = false;
                            bool foundIt = false;
                            int begin = i;
                            while (i < Text.Length)
                            {
                                if (Text[i] == '%')
                                {
                                    foundIt = true;
                                    break;
                                }
                                i++;
                            }

                            if (!foundIt)
                                return;

                            string spriteName = Text.Substring(begin + 1, i - begin - 1);

                            Sprite sprite = _layer.GetSprite(spriteName);
                            if (sprite == null)
                                continue;

                            left = cursorX - texelOffsetX;
                            top = cursorY - texelOffsetY;
                            right = left + sprite.SpriteWidth + texelOffsetX;
                            bottom = top + sprite.SpriteHeight + texelOffsetY;

                            Character c = new Character();
                            c.Index = i;
                            c.Position[(int)QuadCorner.TopLeft].x = left;
                            c.Position[(int)QuadCorner.TopLeft].y = top;
                            c.Position[(int)QuadCorner.TopRight].x = right;
                            c.Position[(int)QuadCorner.TopRight].y = top;
                            c.Position[(int)QuadCorner.BottomLeft].x = left;
                            c.Position[(int)QuadCorner.BottomLeft].y = bottom;
                            c.Position[(int)QuadCorner.BottomRight].x = right;
                            c.Position[(int)QuadCorner.BottomRight].y = bottom;
                            c.UV[0] = sprite.TexCoords[0];
                            c.UV[1] = sprite.TexCoords[1];
                            c.UV[2] = sprite.TexCoords[2];
                            c.UV[3] = sprite.TexCoords[3];
                            c.Color = color;

                            _characters.Add(c);
                            cursorX += sprite.SpriteWidth;
                            lineHeight = System.Math.Max(lineHeight, sprite.SpriteHeight);

                            continue;
                        }
                        continue;
                    }
                    markupMode = false;
                }//end if markup mode

                glyph = glyphData.GetGlyph(thisChar);

                if (!fixedWidth)
                {
                    kerning = glyph.GetKerning(lastChar);
                    if (kerning == 0)
                        kerning = glyphData.LetterSpacing;
                }

                right = cursorX + glyph.GlyphWidth + texelOffsetX;
                bottom = cursorY + glyph.GlyphHeight + texelOffsetY;

                Character ch = new Character();
                ch.Index = i;
                ch.Position[(int)QuadCorner.TopLeft].x = cursorX;
                ch.Position[(int)QuadCorner.TopLeft].y = cursorY;
                ch.Position[(int)QuadCorner.TopRight].x = right;
                ch.Position[(int)QuadCorner.TopRight].y = cursorY;
                ch.Position[(int)QuadCorner.BottomLeft].x = cursorX;
                ch.Position[(int)QuadCorner.BottomLeft].y = bottom;
                ch.Position[(int)QuadCorner.BottomRight].x = right;
                ch.Position[(int)QuadCorner.BottomRight].y = bottom;
                ch.UV[0] = glyph.TexCoords[0];
                ch.UV[1] = glyph.TexCoords[1];
                ch.UV[2] = glyph.TexCoords[2];
                ch.UV[3] = glyph.TexCoords[3];
                ch.Color = color;

                _characters.Add(ch);

                if (fixedWidth)
                    cursorX += glyphData.MonoWidth;
                else
                    cursorX += glyph.GlyphAdvance + kerning;

                lastChar = thisChar;
            }//end for

            _isTextDirty = false;
        }
    }
}
