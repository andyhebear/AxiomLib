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
    public interface ILayerContainer
    {
        /// <summary>
        /// 
        /// </summary>
        TextureAtlas Atlas
        {
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        Real TexelOffsetX
        {
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        Real TexelOffsetY
        {
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Layer CreateLayer(int index);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        void Destroy(ref Layer layer);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialSize"></param>
        void CreateVertexBuffer(int initialSize);
        /// <summary>
        /// 
        /// </summary>
        void DestroyVertexBuffer();
        /// <summary>
        /// 
        /// </summary>
        void ResizeVertexBuffer(int requestedSize);
        /// <summary>
        /// 
        /// </summary>
        void ReCalculateIndexes();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="force"></param>
        void RedrawIndex(int id, bool force);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        void RedrawAllIndexs(bool force);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        void RequestIndexRedraw(int index);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        void RenderVertices(bool force);
        /// <summary>
        /// 
        /// </summary>
        void RenderOnce();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        void Transform(ref DynamicBuffer<Vertex> vertices, int begin, int end);
    }
}
