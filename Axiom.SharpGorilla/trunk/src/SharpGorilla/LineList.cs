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
    /// 
    /// </summary>
    public class LineList
    {
        /// <summary>
        /// 
        /// </summary>
        protected Layer _layer;
        protected Real _thickness;
        protected ColorEx _color;
        protected bool _isClosed;
        protected DynamicBuffer<Vector2> _positions;
        internal bool _isDirty;
		internal DynamicBuffer<Vertex> _vertices = new DynamicBuffer<Vertex>();


        internal LineList(Layer layer)
        {
			_layer = layer;
			_isDirty = false;
			_positions = new DynamicBuffer<Vector2>();
        }

        /// <summary>
        /// Clear lines and start again
        /// </summary>
        public void Begin()
        {
            Begin(1.0f);
        }
        /// <summary>
        /// Clear lines and start again
        /// </summary>
        /// <param name="lineThickness"></param>
        public void Begin(float lineThickness)
        {
            Begin(lineThickness, ColorEx.White);
        }
        /// <summary>
        /// Clear lines and start again
        /// </summary>
        /// <param name="lineThicknes"></param>
        /// <param name="color"></param>
        public void Begin(Real lineThicknes, Colors.Color color)
        {
            Begin(lineThicknes,Converter.ToWebColor(color));
        }
        /// <summary>
        /// Clear lines and start again
        /// </summary>
        /// <param name="lineThickness"></param>
        /// <param name="color"></param>
        public void Begin(float lineThickness, ColorEx color)
        {
			_isDirty = false;
			_positions.Clear();
			_thickness = lineThickness;
			_color = color;
        }
        /// <summary>
        /// Extent the list to x and y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Position(Real x, Real y)
        {
			_positions.Add( new Vector2( x, y ) );
        }
        /// <summary>
        /// Extent the list to given coordinates.
        /// </summary>
        /// <param name="position"></param>
        public void Position(Vector2 position)
        {
			_positions.Add( position );
        }

        /// <summary>
        /// Stop line drawing and calculate vertices.
        /// </summary>
        public void End()
        {
            End( false );
        }

        /// <summary>
        /// Stop line drawing and calculate vertices.
        /// </summary>
        /// <param name="isClosed">If "isClosed" is set to true, then the line list joins back to the first position.</param>
        public void End(bool isClosed)
        {
			_isClosed = isClosed;
			_isDirty = true;
        }
        /// <summary>
        /// Redraw the line list
        /// Note:<para></para>
        /// This should not be needed to be called by the user.
        /// </summary>
        public void Redraw()
        {
			if ( !_isDirty )
				return;

			_vertices.Clear();

			if ( _positions.Size < 2 )
				return;

			Vertex temp = new Vertex();

			Real halfThickness = _thickness * 0.5f;

			Vector2 perp = Vector2.Zero, lastLeft = Vector2.Zero, lastRight = Vector2.Zero, thisLeft = Vector2.Zero, thisRight = Vector2.Zero, uv = _layer.SolidUV;

			int i = 1;

			for ( ; i < _positions.Size; i++ )
			{
				perp = _positions[ i ] - _positions[ i - 1 ].Perpendicular.ToNormalized();
				lastLeft	= _positions[ i - 1 ] - perp * halfThickness;
				lastRight	= _positions[ i - 1 ] + perp * halfThickness;
				thisLeft	= _positions[ i ] - perp * halfThickness;
				thisRight	= _positions[ i ] + perp * halfThickness;

				// Triangle A
				Helper.AddVertex( ref _vertices, temp, lastRight.x, lastRight.y, uv, _color );       // Left/Bottom
				Helper.AddVertex( ref _vertices, temp, thisLeft.x, thisLeft.y, uv, _color );        // Right/Top
				Helper.AddVertex( ref _vertices, temp, lastLeft.x, lastLeft.y, uv, _color );         // Left/Top
				// Triangle B
				Helper.AddVertex( ref _vertices, temp, lastRight.x, lastRight.y, uv, _color );      // Left/Bottom
				Helper.AddVertex( ref _vertices, temp, thisRight.x, thisRight.y, uv, _color );    // Right/Bottom
				Helper.AddVertex( ref _vertices, temp, thisLeft.x, thisLeft.y, uv, _color );         // Right/Top
			}

			if ( _isClosed )
			{
				i = _positions.Size - 1;
				perp = _positions[ 0 ] - _positions[ i - 1 ].Perpendicular.ToNormalized();
				lastLeft	= _positions[ i ] - perp * halfThickness;
				lastRight	= _positions[ i ] + perp * halfThickness;
				thisLeft	= _positions[ 0 ] - perp * halfThickness;
				thisRight	= _positions[ 0 ] + perp * halfThickness;

				// Triangle A
				Helper.AddVertex( ref _vertices, temp, lastRight.x, lastRight.y, uv, _color );       // Left/Bottom
				Helper.AddVertex( ref _vertices, temp, thisLeft.x, thisLeft.y, uv, _color );         // Right/Top
				Helper.AddVertex( ref _vertices, temp, lastLeft.x, lastLeft.y, uv, _color );          // Left/Top
				// Triangle B
				Helper.AddVertex( ref _vertices, temp, lastRight.x, lastRight.y, uv, _color );       // Left/Bottom
				Helper.AddVertex( ref _vertices, temp, thisRight.x, thisRight.y, uv, _color );      // Right/Bottom
				Helper.AddVertex( ref _vertices, temp, thisLeft.x, thisLeft.y, uv, _color );         // Right/Top
			}
        }
    }
}
