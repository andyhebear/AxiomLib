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
using Axiom.Core;
using Axiom.Math;

namespace Axiom.Forests
{
    /// <summary>
    /// A class which you extend to provide a callback function for loading entities.
    /// </summary>
    public abstract class PageLoader : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        internal GeometryPage mGeomPage;
        /// <summary>
        ///  This should be overridden to load a specified region of entities.
        /// </summary>
        /// <param name="page"></param>
        public abstract void LoadPage(PageInfo page);

        /// <summary>
        /// This may be overridden (optional) to unload custom data associated with a page.
        /// </summary>
        /// <param name="page"></param>
        public virtual void UnloadPage(PageInfo page) { }

        /// <summary>
        ///  Provides a method for you to perform per-frame tasks for your PageLoader if overridden (optional)
        /// </summary>
        public virtual void FrameUpdate() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        protected void AddEntity(Entity ent, Vector3 position, Quaternion rotation)
        {
            AddEntity(ent, position, rotation, Vector3.UnitScale, ColorEx.White);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        protected void AddEntity(Entity ent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            AddEntity(ent, position, rotation, scale, ColorEx.White);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        protected void AddEntity(Entity ent, Vector3 position, Quaternion rotation, Vector3 scale, ColorEx color)
        {
            mGeomPage.AddEntity(ent, position, rotation, scale, color);
            mGeomPage.AddEntityToBoundingBox(ent, position, rotation, scale);
        }
        /// <summary>
        /// Called on dispose of this class.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
