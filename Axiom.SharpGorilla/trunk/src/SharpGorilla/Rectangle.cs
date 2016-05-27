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

    /// <summary>
    /// Single rectangle with an optional border.
    /// </summary>
    public class Rectangle
    {
        protected Layer _layer;
        protected Real _left;
        protected Real _top;
        protected Real _right;
        protected Real _bottom;
        protected Real _borderWidth;
        protected ColorEx[] _backgroundColor = new ColorEx[4];
        protected ColorEx[] _borderColor = new ColorEx[4];
        protected Vector2[] _uv = new Vector2[4];
        internal bool _isDirty;
        internal DynamicBuffer<Vertex> _vertices;
		/// <summary>
		/// 
		/// </summary>
		public Real Right
		{
			get { return _right; }
			set { _right = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		public Real Bottom
		{
			get { return _bottom; }
			set { _bottom = value; }
		}
        /// <summary>
        /// Gets or sets the position
        /// </summary>
        public Vector2 Position
        {
            get { return new Vector2(_left, _top); }
            set
            {
				Top = value.y;
				Left = value.x;
            }
        }
        /// <summary>
        /// Gets or Sets the left position
        /// </summary>
        public Real Left
        {
            get { return _left; }
            set
            {
                Real w = Width;
                _left = value;
                _right = _left + w;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or Sets the top postion
        /// </summary>
        public Real Top
        {
            get { return _top; }
            set
            {
                Real h = Height;
                _top = value;
                _bottom = Top + h;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or Sets the width
        /// </summary>
        public Real Width
        {
            get { return _right - _left; }
            set
            {
                _right = _left + value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public Real Height
        {
            get { return _bottom - _top; }
            set
            {
                _bottom = _top + value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or sets the current border width.
        /// </summary>
        public Real BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                _borderWidth = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="parent"></param>
        internal Rectangle(Real left, Real top, Real width, Real height, Layer parent)
        {
            _vertices = new DynamicBuffer<Vertex>();
            _layer = parent;
            _isDirty = true;
            _left = left;
            _top = top;
            _right = left + width;
            _bottom = top + height;
            for (int i = 0; i < 4; i++)
            {
                _backgroundColor[i] = ColorEx.White;
                _uv[i] = _layer.SolidUV;
            }

        }
        /// <summary>
        /// Don't draw the background.
        /// <para></para>
        /// Note:<para></para> 
        /// This just sets the background colour alpha to zero. Which on the next
        /// <para></para>
        /// draw tells Rectangle to skip over drawing the background.
        /// </summary>
        public void NoBackground()
        {
            _backgroundColor[0].a = 0;
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Don't draw the border.
        /// Note:<para></para> 
        /// This just sets the border to zero. Which on the next
        /// <para></para>
        /// draw tells Rectangle to skip over drawing the border.
        /// </summary>
        /// </summary>
        public void NoBorder()
        {
            _borderWidth = 0;
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Gets a background color of a specific corner.
        /// </summary>
        /// <param name="index">index of the corner</param>
        /// <returns>color of selected corner</returns>
        public ColorEx GetBackgroundColor(QuadCorner index)
        {
            return _backgroundColor[(int)index];
        }
        /// <summary>
        /// Sets a background color to all corners.
        /// </summary>
        /// <param name="color">background color</param>
        public void SetBackgroundColor(ColorEx color)
        {
			ColorEx val = color;
			_backgroundColor[ 0 ] = val;
			_backgroundColor[ 1 ] = val;
			_backgroundColor[ 2 ] = val;
			_backgroundColor[ 3 ] = val;
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Sets a background color to all corners.
        /// </summary>
        /// <param name="color"></param>
        public void SetBackgroundColor(Color color)
        {
			ColorEx val = Converter.ToWebColor( color );
			_backgroundColor[ 0 ] = new ColorEx( val.r, val.g, val.b, val.a );
            _backgroundColor[1] = _backgroundColor[0];
            _backgroundColor[2] = _backgroundColor[0];
            _backgroundColor[3] = _backgroundColor[0];
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Sets a background color to a specific corner.
        /// </summary>
        /// <param name="index">specific corner</param>
        /// <param name="color">color of the specific corner</param>
        public void SetBackgroundColor(QuadCorner index, ColorEx color)
        {
            _borderColor[(int)index] = color;
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Sets the background to a gradient.
        /// </summary>
        /// <param name="gradient">gradient direction</param>
        /// <param name="colorA"></param>
        /// <param name="colorB"></param>
        public void SetBackgroundGradient(Gradient gradient, ColorEx colorA, ColorEx colorB)
        {
            switch (gradient)
            {
                case Gradient.NorthSouth:
                    _backgroundColor[0] = _backgroundColor[1] = colorA;
                    _backgroundColor[2] = _backgroundColor[3] = colorB;
                    break;
                case Gradient.WestEast:
                    _backgroundColor[0] = _backgroundColor[3] = colorA;
                    _backgroundColor[1] = _backgroundColor[2] = colorB;
                    break;
                case Gradient.Diagonal:
                    ColorEx avg = new ColorEx();
                    avg.r = (colorA.r + colorB.r) * 0.5f;
                    avg.g = (colorA.g + colorB.g) * 0.5f;
                    avg.b = (colorA.b + colorB.b) * 0.5f;
                    avg.a = (colorA.a + colorB.a) * 0.5f;
                    _backgroundColor[0] = colorA;
                    _backgroundColor[1] = avg = _backgroundColor[3] = avg;
                    _backgroundColor[2] = colorB;
                    break;
            }

            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Sets the background to a sprite from the texture atlas.
        /// Note:
        /// <para></para>
        /// To remove the image pass on a null pointer.
        /// </summary>
        /// <param name="sprite"></param>
        public void SetBackgroundImage(Sprite sprite)
        {
            if (sprite == null)
            {
                _uv[0] = _uv[1] = _uv[2] = _uv[3] = _layer.SolidUV;
            }
            else
            {
                Real texelOffsetX = _layer.TexelX, texelOffsetY = _layer.TexelY;
                texelOffsetX /= _layer.TextureSize.x;
                texelOffsetY /= _layer.TextureSize.y;
                _uv[0].x = _uv[3].x = sprite.UVLeft - texelOffsetX;
                _uv[0].y = _uv[1].y = sprite.UVTop - texelOffsetY;
                _uv[1].x = _uv[2].x = sprite.UVRight + texelOffsetX;
                _uv[2].y = _uv[3].y = sprite.UVBottom + texelOffsetY;
            }
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Set the background to a sprite from the texture atlas, with clipping.<para></para>
        /// Clipping is used for example with RPM meters on HUDs, where a portion<para></para>
        /// of the sprite needs to be shown to indicate the RPM on the car.<para></para>
        /// <para></para>
        /// widthClip  is a decimal percentage of the width of the sprite (0.0 none, 1.0 full)<para></para>
        /// heightClip is a decimal percentage of the height of the sprite (0.0 none, 1.0 full)<para></para>
        /// <para></para>
        /// You should use this with the width() and height() functions for a full effect.<para></para>
        /// Note:<para></para>
        /// To remove the image pass on a null pointer.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="widthClip"></param>
        /// <param name="heightClip"></param>
        public void SetBackgroundImage(Sprite sprite, Real widthClip, Real heightClip)
        {
            if (sprite == null)
            {
                _uv[0] = _uv[1] = _uv[2] = _uv[3] = _layer.SolidUV;
            }
            else
            {
                Real texelOffsetX = _layer.TexelX, texelOffsetY = _layer.TexelY;
                texelOffsetX /= _layer.TextureSize.x;
                texelOffsetY /= _layer.TextureSize.y;
                _uv[0].x = _uv[3].x = sprite.UVLeft - texelOffsetX;
                _uv[0].y = _uv[1].y = sprite.UVTop - texelOffsetY;
                _uv[1].x = _uv[2].x = sprite.UVLeft + ((sprite.UVRight - sprite.UVLeft) * widthClip) + texelOffsetX;
                _uv[2].y = _uv[3].y = sprite.UVTop + ((sprite.UVBottom - sprite.UVTop) * heightClip) + texelOffsetY;
            }
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Set the background to a sprite from the texture atlas.
        /// Note:<para></para>
        /// To remove the image pass on "none" or a empty string
        /// </summary>
        /// <param name="spriteNameOrNone"></param>
        public void SetBackgroundImage(string spriteNameOrNone)
        {
            if (string.IsNullOrEmpty(spriteNameOrNone) || spriteNameOrNone.ToLower().Equals("none"))
            {
                _uv[0] = _uv[1] = _uv[2] = _uv[3] = _layer.SolidUV;
            }
            else
            {
                Real texelOffsetX = _layer.TexelX, texelOffsetY = _layer.TexelY;
                texelOffsetX /= _layer.TextureSize.x;
                texelOffsetY /= _layer.TextureSize.y;
                Sprite sprite = _layer.GetSprite(spriteNameOrNone);
                _uv[0].x = _uv[3].x = sprite.UVLeft - texelOffsetX;
                _uv[0].y = _uv[1].y = sprite.UVTop - texelOffsetY;
                _uv[1].x = _uv[2].x = sprite.UVRight + texelOffsetX;
                _uv[2].y = _uv[3].y = sprite.UVBottom + texelOffsetY;
            }
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Gets the border color.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ColorEx GetBorderColor(Border index)
        {
            return _borderColor[(int)index];
        }
        /// <summary>
        /// Sets all borders to one color 
        /// </summary>
        /// <param name="bordercolor">Color of all borders</param>
        public void SetBorderColor(ColorEx bordercolor)
        {
            _borderColor[0] = bordercolor;
            _borderColor[1] = bordercolor;
            _borderColor[2] = bordercolor;
            _borderColor[3] = bordercolor;
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Sets a border part to one color. 
        /// </summary>
        /// <param name="index">border index</param>
        /// <param name="borderColor">border color</param>
        public void SetBorderColor(Border index, Color borderColor)
        {
            _borderColor[(int)index] = Converter.ToWebColor(borderColor);
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Set the border width and colour.
        /// </summary>
        /// <param name="width">width of the Border</param>
        /// <param name="color">Color of the border</param>
        public void SetBorder(Real width, ColorEx color)
        {
            _borderColor[0] = color;
            _borderColor[1] = _borderColor[0];
            _borderColor[2] = _borderColor[0];
            _borderColor[3] = _borderColor[0];
            BorderWidth = width;
        }
        /// <summary>
        /// Sets the border width and specific colors for each part.
        /// </summary>
        /// <param name="width">width of the border</param>
        /// <param name="north">northern border color</param>
        /// <param name="east">eastern border color</param>
        /// <param name="south">southern border color</param>
        /// <param name="west">western border color</param>
        public void SetBorder(Real width, ColorEx north, ColorEx east, ColorEx south, ColorEx west)
        {
            _borderColor[(int)Border.North] = north;
            _borderColor[(int)Border.South] = south;
            _borderColor[(int)Border.East] = east;
            _borderColor[(int)Border.West] = west;
            BorderWidth = width;
        }
        /// <summary>
        /// Sets the border width and color.
        /// </summary>
        /// <param name="width">width of the border</param>
        /// <param name="color">web color of the border</param>
        public void SetBorder(Real width, Color color)
        {
            _borderColor[0] = Converter.ToWebColor(color);
            _borderColor[1] = _borderColor[0];
            _borderColor[2] = _borderColor[0];
            _borderColor[3] = _borderColor[0];
            BorderWidth = width;
        }
        /// <summary>
        /// Sets the border width and specific colors for each part.
        /// </summary>
        /// <param name="width">width of the border</param>
        /// <param name="north">northern border color</param>
        /// <param name="east">eastern border color</param>
        /// <param name="south">southern border color</param>
        /// <param name="west">western border color</param>
        public void SetBorder(Real width, Color north, Color east, Color south, Color west)
        {
            _borderColor[(int)Border.North] = Converter.ToWebColor(north);
            _borderColor[(int)Border.South] = Converter.ToWebColor(south);
            _borderColor[(int)Border.East] = Converter.ToWebColor(east);
            _borderColor[(int)Border.West] = Converter.ToWebColor(west);
            BorderWidth = width;
        }
        /// <summary>
        /// Redraw the rectangle
        /// Note:<para></para>
        /// This should not be needed to be called by the user.
        /// </summary>
        public void Redraw()
        {
            if (!_isDirty)
                return;

            _vertices.Clear();

            Vector2 a = Vector2.Zero;
            Vector2 b = Vector2.Zero;
            Vector2 c = Vector2.Zero;
            Vector2 d = Vector2.Zero;
            Real texelOffsetX = _layer.TexelX, texelOffsetY = _layer.TexelY;

            a.x = _left - texelOffsetX; a.y = _top - texelOffsetY;
            b.x = _right + texelOffsetX; b.y = _top - texelOffsetY;
            c.x = _left - texelOffsetX; c.y = _bottom + texelOffsetY;
            d.x = _right + texelOffsetX; d.y = _bottom + texelOffsetY;

            // Border
            if (_borderWidth != 0)
            {
                Vector2 i = a, j = b, k = c, l = d;
                i.x -= _borderWidth; i.y -= _borderWidth;
                j.x += _borderWidth; j.y -= _borderWidth;
                k.x -= _borderWidth; k.y += _borderWidth;
                l.x += _borderWidth; l.y += _borderWidth;

                Vertex temp = new Vertex();
                Vector2 uv = _layer.SolidUV;

                //north
                Helper.AddTriangle(ref _vertices, temp, a, j, i, uv, _borderColor[(int)Border.North]);
                Helper.AddTriangle(ref _vertices, temp, a, b, j, uv, _borderColor[(int)Border.North]);

                //east
                Helper.AddTriangle(ref _vertices, temp, d, j, b, uv, _borderColor[(int)Border.East]);
                Helper.AddTriangle(ref _vertices, temp, d, l, j, uv, _borderColor[(int)Border.East]);

                //south
                Helper.AddTriangle(ref _vertices, temp, k, d, c, uv, _borderColor[(int)Border.South]);
                Helper.AddTriangle(ref _vertices, temp, k, l, d, uv, _borderColor[(int)Border.South]);

                //west
                Helper.AddTriangle(ref _vertices, temp, k, a, i, uv, _borderColor[(int)Border.West]);
                Helper.AddTriangle(ref _vertices, temp, k, c, a, uv, _borderColor[(int)Border.West]);
            }

            //fill
            if (_backgroundColor[0].a != 0)
            {
                Vertex temp = new Vertex();

                // Triangle A
				//Helper.AddVertex(ref _vertices, temp, c.x, c.y, _uv[3], _backgroundColor[3]);// Left/Bottom  3
				//Helper.AddVertex(ref _vertices, temp, b.x, b.y, _uv[1], _backgroundColor[1]);// Right/Top    1
				//Helper.AddVertex(ref _vertices, temp, a.x, a.y, _uv[0], _backgroundColor[0]);// Left/Top     0
				Helper.AddVertex( ref _vertices, temp, c.x, c.y, _uv[ 3 ], _backgroundColor[ 3 ] );//new ColorEx( _backgroundColor[ 3 ].r, _backgroundColor[ 3 ].g, _backgroundColor[ 3 ].b, _backgroundColor[ 3 ].a ) );// _backgroundColor[ 3 ] );// Left/Bottom  3
				Helper.AddVertex( ref _vertices, temp, b.x, b.y, _uv[ 1 ], _backgroundColor[ 1 ] );// Right/Top    1
				Helper.AddVertex( ref _vertices, temp, a.x, a.y, _uv[ 0 ], _backgroundColor[ 0 ] );// Left/Top     0
                // Triangle B
                Helper.AddVertex(ref _vertices, temp, c.x, c.y, _uv[3], _backgroundColor[3]);// Left/Bottom   3
                Helper.AddVertex(ref _vertices, temp, d.x, d.y, _uv[2], _backgroundColor[2]);// Right/Bottom  2
                Helper.AddVertex(ref _vertices, temp, b.x, b.y, _uv[1], _backgroundColor[1]);// Right/Top     1
            }

            _isDirty = false;
        }
        /// <summary>
        /// Does a set of coordinates lie within this rectangle?
        /// </summary>
        /// <param name="coordinates">coordinates to check for intersection</param>
        /// <returns>true if it intersects</returns>
        public bool Intersects(Vector2 coordinates)
        {
            return ((coordinates.x >= _left && coordinates.x <= _right) && (coordinates.y >= _top && coordinates.y <= _bottom));
        }
    }
}
