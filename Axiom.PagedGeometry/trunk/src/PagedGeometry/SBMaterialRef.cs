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
using Axiom.Graphics;
using Axiom.Core;
namespace Axiom.Forests
{
    /// <summary>
    /// 
    /// </summary>
    public class SBMaterialRef
    {
        private static Dictionary<Material, SBMaterialRef> mSelfList = new Dictionary<Material, SBMaterialRef>();
        private uint mRefCount = 0;
        private Material mMaterial;
        private BillboardOrigin mOrigin;

        /// <summary>
        /// 
        /// </summary>
        public Material Material
        {
            get { return mMaterial; }
        }
        /// <summary>
        /// 
        /// </summary>
        public BillboardOrigin Origin
        {
            get { return mOrigin; }
        }
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<Material, SBMaterialRef> SelfList
        {
            get { return mSelfList; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="origin"></param>
        private SBMaterialRef(Material mat, BillboardOrigin origin)
        {
            mMaterial = mat;
            mOrigin = origin;
            mRefCount = 1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matP"></param>
        /// <param name="origin"></param>
        public static void AddMaterialRef(Material matP, BillboardOrigin origin)
        {
            Material mat = matP;
            SBMaterialRef matRef = null;
            if (!mSelfList.TryGetValue(mat, out matRef))
            {
                matRef = new SBMaterialRef(mat, origin);
            }
            else
            {
                //its allready there, increase the refcount
                matRef.mRefCount++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matP"></param>
        public static void RemoveMaterialRef(Material matP)
        {
            SBMaterialRef matRef = null;
            if (mSelfList.TryGetValue(matP, out matRef))
            {
                if (--matRef.mRefCount == 0)
                {
                    mSelfList.Remove(matRef.Material);
                }
            }
        }
    }
}
