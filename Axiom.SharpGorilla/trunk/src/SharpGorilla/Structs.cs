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
using Axiom.Core;
#endregion
namespace SharpGorilla
{
    /// <summary>
    /// Structure for a single vertex.
    /// </summary>
    public struct Vertex
    {
        /// <summary>
        ///     Position of the Vertex
        /// </summary>
        public Vector3 Position;
        /// <summary>
        ///     Color of the vertex
        /// </summary>
        public ColorEx Color;
        /// <summary>
        ///     UV coord of the vertex
        /// </summary>
        public Vector2 UV;
    }
    /// <summary>
    /// Distances between two characters next to each other.
    /// </summary>
    public struct Kerning
    {
        /// <summary>
        /// 
        /// </summary>
        public int Character;
        /// <summary>
        /// 
        /// </summary>
        public Real kerning;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="kerning"></param>
        public Kerning(int character, Real kerning)
        {
            Character = character;
            this.kerning = kerning;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Quad
    {
        public Vector2[] Postion = new Vector2[4];
        public Vector2[] UV = new Vector2[4];
        public ColorEx[] Color = new ColorEx[4];
    }
    /// <summary>
    /// 
    /// </summary>
    public class Character
    {
        public Vector2[] Position = new Vector2[4];
        public Vector2[] UV = new Vector2[4];
        public ColorEx Color;
        public int Index;
    }
    /// <summary>
    /// 
    /// </summary>
    public struct IndexData
    {
        public List<Layer> Layers;
        public DynamicBuffer<Vertex> Vertices;
        public bool RedrawNeeded;
    }
}
