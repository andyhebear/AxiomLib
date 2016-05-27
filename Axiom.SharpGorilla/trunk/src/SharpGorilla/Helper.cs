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
    public static class Helper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="vertex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="uv"></param>
        /// <param name="color"></param>
        public static void AddVertex(ref DynamicBuffer<Vertex> vertices, Vertex vertex, Real x, Real y, Vector2 uv, ColorEx color)
        {
            vertex.Position.x = x;
            vertex.Position.y = y;
            vertex.Position.z = 0;
            vertex.UV.x = uv.x;
            vertex.UV.y = uv.y;
			vertex.Color = color;
            vertices.Add(vertex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="vertex"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="uv"></param>
        /// <param name="color"></param>
        public static void AddTriangle(ref DynamicBuffer<Vertex> vertices, Vertex vertex, Vector2 a, Vector2 b, Vector2 c, Vector2 uv, ColorEx color)
        {
            AddVertex(ref vertices, vertex, a.x, a.y, uv, color);
            AddVertex(ref vertices, vertex, b.x, b.y, uv, color);
            AddVertex(ref vertices, vertex, c.x, c.y, uv, color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="vertex"></param>
        /// <param name="positions"></param>
        /// <param name="colors"></param>
        /// <param name="uv"></param>
        public static void AddQuad(ref DynamicBuffer<Vertex> vertices, Vertex vertex, Vector2[] positions, ColorEx[] colors, Vector2[] uv)
        {
            AddVertex(ref vertices, vertex, positions[3].x, positions[3].y, uv[3], colors[3]);
            AddVertex(ref vertices, vertex, positions[1].x, positions[1].y, uv[1], colors[1]);
            AddVertex(ref vertices, vertex, positions[0].x, positions[0].y, uv[0], colors[0]);

            AddVertex(ref vertices, vertex, positions[3].x, positions[3].y, uv[3], colors[3]);
            AddVertex(ref vertices, vertex, positions[2].x, positions[2].y, uv[2], colors[2]);
            AddVertex(ref vertices, vertex, positions[1].x, positions[1].y, uv[1], colors[1]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="vertex"></param>
        /// <param name="positions"></param>
        /// <param name="colors"></param>
        /// <param name="uv"></param>
        public static void AddQuad2(ref DynamicBuffer<Vertex> vertices, Vertex vertex, Vector2[] positions, ColorEx color, Vector2[] uv)
        {
            AddVertex(ref vertices, vertex, positions[3].x, positions[3].y, uv[3], color);
            AddVertex(ref vertices, vertex, positions[1].x, positions[1].y, uv[1], color);
            AddVertex(ref vertices, vertex, positions[0].x, positions[0].y, uv[0], color);

            AddVertex(ref vertices, vertex, positions[3].x, positions[3].y, uv[3], color);
            AddVertex(ref vertices, vertex, positions[2].x, positions[2].y, uv[2], color);
            AddVertex(ref vertices, vertex, positions[1].x, positions[1].y, uv[1], color);
        }
    }
}
