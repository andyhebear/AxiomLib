#region MIT/X11 License
// This file is part of the Axiom.PagedGeometry project
// Copyright (c) 2009 Bostich <bostich83@googlemail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Axiom.PagedGeometry is a reimplementation of the PagedGeometry project for .Net/Mono
// PagedGeometry is Copyright (C) 2006 John Judnich
#endregion MIT/X11 License
using System;
using System.Collections.Generic;
using System.Linq;
using Axiom.Math;

namespace Axiom.Forests
{
    public class TBounds : TRect<float>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        public TBounds(float left, float top,
            float right, float bottom)
        {
            mLeft = left;
            mRight = right;
            mTop = top;
            mBottom = bottom;
        }
        /// <summary>
        /// 
        /// </summary>
        public float Width
        {
            get { return mRight - mLeft; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float Height
        {
            get { return mBottom - mTop; }
        }
    }

    /// <summary>
    /// Useful page information supplied to a pageloader.
    /// </summary>
    public struct PageInfo
    {
        /// <summary>
        /// The page boundaries in which all entities should be placed.
        /// </summary>
        public TBounds Bounds;
        /// <summary>
        /// The center of the page.
        /// </summary>
        public Vector3 CenterPoint;
        /// <summary>
        /// The X index of the page tile.
        /// </summary>
        public int XIndex;
        /// <summary>
        /// The Z index of the page tile.
        /// </summary>
        public int ZIndex;
        /// <summary>
        /// Misc. custom data to associate with this page tile.
        /// </summary>
        public object UserData;
    }
}
