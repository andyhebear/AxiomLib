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
    /// A regular n-sided polygon.
    /// </summary>
    public class Polygon
    {
        protected Layer _layer;
        protected Real _left;
        protected Real _top;
        protected Real _radius;
        protected Real _borderWidth;
        protected Radian _angle;
        protected int _sides;
        protected ColorEx _backgroundColor;
        protected ColorEx _borderColor;
        protected Sprite _sprite;
        internal bool _isDirty;
		internal DynamicBuffer<Vertex> _vertices = new DynamicBuffer<Vertex>();

        /// <summary>
        /// Gets or sets left position
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
        /// Gets or Sets top position
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
        /// Gets or sets the current border width
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
        /// Gets or sets the radius
        /// </summary>
        public Real Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or Sets the number of sides the polygon has.
        /// Note:
        /// <para></para>
        ///  Number of sides must be at least 3.
        /// </summary>
        public int Sides
        {
            get { return _sides; }
            set
            {
                _sides = value;
                if (_sides < 3)
                    _sides = 3;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or sets the angle of the polygon
        /// </summary>
        public Radian Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        /// Gets or sets the sprite used as a background image
        /// Note:<para></para>
        /// Pass null to clear the image, in case there is no image set, this will return a null pointer
        /// </summary>
        public Sprite BackgroundImage
        {
            get { return _sprite; }
            set
            {
                _sprite = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }
        /// <summary>
        ///  Set the background color.
        ///  Note:<para></para>
        ///  If there is a background sprite then it will be tinted by this color.
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
        /// Gets or sets the border color
        /// </summary>
        public ColorEx BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                _isDirty = true;
                _layer.MarkDirty();
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="radius"></param>
		/// <param name="sides"></param>
		/// <param name="layer"></param>
		internal Polygon( Real left, Real top, Real radius, int sides, Layer layer )
		{
			_layer = layer;
			_isDirty = true;
			_layer.MarkDirty();
			_left = left;
			_top = top;
			_radius = radius;
			_sides = sides;
			_backgroundColor = ColorEx.White;
			_borderColor.a = 0;
			_borderWidth = 0;
		}
        /// <summary>
        /// Set the sprite used as a background image from a string.
        /// </summary>
        /// <param name="nameOrNone">Use a empty string or "none" to clear.</param>
        public void SetBackgroundImage(string nameOrNone)
        {
            if (string.IsNullOrEmpty(nameOrNone) || nameOrNone.ToLower().Equals("none"))
            {
                _sprite = null;
            }
            else
            {
                _sprite = _layer.GetSprite(nameOrNone);
            }

            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Set the border width and color
        /// </summary>
        /// <param name="width">width of the border</param>
        /// <param name="color">color of the border</param>
        public void SetBorder(Real width, ColorEx color)
        {
            _borderColor = color;
            BorderWidth = width;
        }
        /// <summary>
        /// Set the border width and color
        /// </summary>
        /// <param name="width">width of the border</param>
        /// <param name="color">color of the border</param>
        public void SetBorder(Real width, Colors.Color color)
        {
            switch (color)
            {
                case Colors.Color.None:
                    _borderColor.a = 0;
                    BorderWidth = 0;
                    break;
                default:
                    _borderColor = Converter.ToWebColor(color);
                    BorderWidth = width;
                    break;
            }
        }
        /// <summary>
        /// Sets the border color
        /// </summary>
        /// <param name="color">color of the border</param>
        public void SetBorderColor(Colors.Color color)
        {
            switch (color)
            {
                case Colors.Color.None:
                    _borderColor.a = 0;
                    break;
                default:
                    _borderColor = Converter.ToWebColor(color);
                    break;
            }
            _isDirty = true;
            _layer.MarkDirty();
        }
        /// <summary>
        /// Redraw the polygon
        /// </summary>
        internal void Redraw()
        {
			if ( !_isDirty )
				return;

			_vertices.Clear();

			Vertex temp = new Vertex();

			Real theta = _angle;
			Real inc = ( 2 * Axiom.Math.Utility.PI ) / _sides;
			Vector2 lastVertex = Vector2.Zero;
			lastVertex.x = _left + ( _radius * Axiom.Math.Utility.Cos( theta ) );
			lastVertex.y = _top + ( _radius * Axiom.Math.Utility.Sin( theta ) );

			Vector2 thisVertex = Vector2.Zero;

			if ( _borderWidth != 0 )
			{
				Vector2 lastOuterVertex = Vector2.Zero, outerVertex = Vector2.Zero, uv = Vector2.Zero;
				uv = _layer.SolidUV;

				lastOuterVertex.x = _left + ( ( _radius + _borderWidth ) * Axiom.Math.Utility.Cos( theta ) );
				lastOuterVertex.y = _top + ( ( _radius + _borderWidth ) * Axiom.Math.Utility.Sin( theta ) );

				for ( int i = 0; i < _sides; i++ )
				{
					
					theta += inc;
					thisVertex.x = _left + ( _radius * Axiom.Math.Utility.Cos( theta ) );
					thisVertex.y = _top + ( _radius * Axiom.Math.Utility.Sin( theta ) );
					outerVertex.x = _left + ( ( _radius + _borderWidth ) * Axiom.Math.Utility.Cos( theta ) );
					outerVertex.y = _top + ( ( _radius + _borderWidth ) * Axiom.Math.Utility.Sin( theta ) );

					Helper.AddTriangle( ref _vertices, temp, lastVertex, outerVertex, lastOuterVertex, uv, _borderColor );
					Helper.AddTriangle( ref _vertices, temp, thisVertex, outerVertex, lastOuterVertex, uv, _borderColor );

					lastVertex = thisVertex;
					lastOuterVertex = outerVertex;
				}
			}

			if ( _backgroundColor.a != 0 )
			{
				if ( _sprite != null )
				{
					Real xRadius = _sprite.SpriteWidth * 0.5f;
					Real yRadius = _sprite.SpriteHeight * 0.5f;

					Vector2 centerUV = Vector2.Zero, lastUV = Vector2.Zero, thisUV = Vector2.Zero, baseUV = Vector2.Zero, texSize = _layer.TextureSize;

					baseUV.x = _sprite.UVLeft * texSize.x;
					baseUV.y = _sprite.UVTop * texSize.y;
					baseUV.x += xRadius;
					baseUV.y += yRadius;

					centerUV = new Vector2( baseUV.x / texSize.x, baseUV.y / texSize.y );
					lastUV = baseUV;
					lastUV.x = baseUV.x + ( xRadius * Axiom.Math.Utility.Cos( theta ) );
					lastUV.y = baseUV.y + ( yRadius * Axiom.Math.Utility.Sin( theta ) );

					lastUV = new Vector2( lastUV.x / texSize.x, lastUV.y / texSize.y );
					for ( int i = 0; i < _sides; i++ )
					{
						Helper.AddVertex( ref _vertices, temp, _left, _top, centerUV, _backgroundColor );
						theta += inc;
						thisVertex.x = _left + ( _radius * Axiom.Math.Utility.Cos( theta ) );
						thisVertex.y = _top + ( _radius * Axiom.Math.Utility.Sin( theta ) );

						thisUV.x = baseUV.x + ( xRadius * Axiom.Math.Utility.Cos( theta ) );
						thisUV.y = baseUV.y + ( yRadius * Axiom.Math.Utility.Sin( theta ) );
						thisUV.x /= texSize.x;
						thisUV.y /= texSize.y;

						Helper.AddVertex( ref _vertices, temp, thisVertex.x, thisVertex.y, thisUV, _backgroundColor );
						Helper.AddVertex( ref _vertices, temp, lastVertex.x, lastVertex.y, lastUV, _backgroundColor );

						lastVertex = thisVertex;
						lastUV = thisUV;
					}
				}
				else
				{
					Vector2 uv = _layer.SolidUV;
					for ( int i = 0; i < _sides; i++ )
					{
						Helper.AddVertex( ref _vertices, temp, _left, _top, uv, _backgroundColor );
						theta += inc;
						thisVertex.x = _left + ( _radius * Axiom.Math.Utility.Cos( theta ) );
						thisVertex.y = _top + ( _radius * Axiom.Math.Utility.Sin( theta ) );
						Helper.AddVertex( ref _vertices, temp, thisVertex.x, thisVertex.y, uv, _backgroundColor );
						Helper.AddVertex( ref _vertices, temp, lastVertex.x, lastVertex.y, uv, _backgroundColor );
						lastVertex = thisVertex;
					}
				}
			}

			_isDirty = false;
        }
    }
}
