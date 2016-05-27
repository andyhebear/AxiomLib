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
    public abstract class GeometryPage
    {
        private Vector3 mCenterPoint;
        internal int mXIndex;
        internal int mZIndex;
        internal long mInactiveTime;
        internal bool mIsVisible;
        internal bool mIsFadeEnabled;
        internal bool mIsPending;
        internal bool mIsLoaded;
        internal bool mNeedsUnload;
        private AxisAlignedBox mTrueBounds;
        internal object mUserData;
        private bool mTrueBoundsUndefined;
        public bool IsFadeEnabled
        {
            get { return mIsFadeEnabled; }
            set { mIsFadeEnabled = value; }
        }

        public long InactiveTime
        {
            get { return mInactiveTime; }
            set { mInactiveTime = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 CenterPoint
        {
            get { return mCenterPoint; }
            set { mCenterPoint = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsPending
        {
            get { return mIsPending; }
            set { mIsPending = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsLoaded
        {
            get { return mIsLoaded; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsVisible
        {
            set 
            {
                mIsVisible = value;
                SetVisible(mIsVisible); 
            }
            get{ return mIsVisible && mIsLoaded;}
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual AxisAlignedBox BoundingBox
        {
            get { return mTrueBounds; }
        }
        /// <summary>
        /// Prepare a geometry page for use.
        /// </summary>
        /// <param name="geom"></param>
        public abstract void Init(PagedGeometry geom);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public virtual void SetRegion(float left, float top, float right, float bottom) { }
        public virtual void SetVisible(bool visible) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void AddEntity(Entity ent, Vector3 position, Quaternion rotation)
        {
            AddEntity(ent, position, rotation, Vector3.UnitScale, ColorEx.White);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void AddEntity(Entity ent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            AddEntity(ent, position, rotation, scale, ColorEx.White);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public virtual void AddEntity(Entity ent, Vector3 position, Quaternion rotation, Vector3 scale, ColorEx color)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        protected virtual void SetFade(bool enabled)
        {
            SetFade(enabled, 0, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="visibleDist"></param>
        /// <param name="invisibleDist"></param>
        public abstract void SetFade(bool enabled, float visibleDist, float invisibleDist);
        /// <summary>
        /// 
        /// </summary>
        public virtual void Build() { }
        /// <summary>
        /// 
        /// </summary>
        public abstract void RemoveEntites();
        /// <summary>
        /// 
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public virtual void AddEntityToBoundingBox(Entity ent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
#warning Matrix4 accepts no Quaternation in ctor
            Matrix4 mat = Matrix4.FromMatrix3(rotation.ToRotationMatrix());
            mat.Scale = scale;
            AxisAlignedBox entBounds = ent.BoundingBox;
            Vector3 relPosition = position - mCenterPoint;
            if (mTrueBoundsUndefined)
            {
                mTrueBounds.Minimum = entBounds.Minimum + relPosition;
                mTrueBounds.Maximum = entBounds.Maximum + relPosition;
                mTrueBoundsUndefined = false;
            }
            else
            {
                Vector3 min = mTrueBounds.Minimum;
                Vector3 max = mTrueBounds.Maximum;
                min.Floor(entBounds.Minimum + relPosition);
                max.Floor(entBounds.Maximum + relPosition);
                mTrueBounds.Maximum = max;
                mTrueBounds.Minimum = min;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void ClearBoundingBox()
        {
            mTrueBounds = new AxisAlignedBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            mTrueBoundsUndefined = true;
        }
    }
}
