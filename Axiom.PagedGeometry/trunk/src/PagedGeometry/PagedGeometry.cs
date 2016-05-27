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
using System.Diagnostics;
using Axiom.Core;
using Axiom.Math;

namespace Axiom.Forests
{
    public class PagedGeometry
    {
        protected SceneManager mSceneMgr;
        protected SceneNode mRootNode;
        protected Camera mSceneCam;
        protected Vector3 mOldCamPos;
        protected Camera mLastSceneCam;
        protected Vector3 mLastOldCamPos;
        /// <summary>
        /// 
        /// </summary>
        protected List<GeometryPageManager> mMangerList = new List<GeometryPageManager>();
        protected PageLoader mPageLoader;
        protected TBounds mBounds;
        protected float mPageSize;
        protected ITimer mTimer;
        protected long mLastTime;

        /// <summary>
        /// 
        /// </summary>
        public Camera Camera
        {
            get { return mSceneCam; }
            set 
            {
                if (value == null)
                    mSceneCam = null;
                else
                {
                    if (mSceneMgr != value.SceneManager)
                    {
                        throw new Exception("The specified camera is from the wrong SceneManager.");
                    }
                    if (value == mLastSceneCam)
                    {
                        Axiom.Math.Utility.Swap(ref mOldCamPos, ref mLastOldCamPos);
                        Axiom.Math.Utility.Swap(ref mSceneCam, ref mLastSceneCam);
                    }
                    else
                    {
                        mLastSceneCam = mSceneCam;
                        mLastOldCamPos = mOldCamPos;
                        mSceneCam = value;
                    }

                    if (mSceneMgr == null)
                        mSceneMgr = mSceneCam.SceneManager;

                    if (mRootNode == null)
                    {
                        mRootNode = mSceneMgr.RootSceneNode;
                    }
                    
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public SceneManager SceneManager
        {
            get { return mSceneMgr; }
        }
        /// <summary>
        /// 
        /// </summary>
        public SceneNode SceneNode
        {
            get { return mRootNode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public TBounds Bounds
        {
            set 
            {
                if (mMangerList.Count > 0)
                    throw new Exception("PagedGeometry::setBounds() cannot be called after detail levels have been added. Call removeDetailLevels() first.");
                if (value.Width != value.Height)
                    throw new Exception("Bounds must be square");
                if (value.Width <= 0 || value.Height <= 0)
                    throw new Exception("Bounds must have positive width and height");

                mBounds = value;
            }
            get { return mBounds; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float PageSize
        {
            set
            {
                if (mMangerList.Count > 0)
                    throw new Exception("PagedGeometry::setPageSize() cannot be called after detail levels have been added. Call removeDetailLevels() first.");

                mPageSize = value;
            }
            get { return mPageSize; }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<GeometryPageManager> DetailLevels
        {
            get { return mMangerList; }
        }
        /// <summary>
        /// 
        /// </summary>
        public PageLoader PageLoader
        {
            set { mPageLoader = value; }
            get { return mPageLoader; }
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PagedGeometry() 
            : this(null,100) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        public PagedGeometry(Camera cam)
            : this(cam, 100) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="pageSize"></param>
        public PagedGeometry(Camera cam, float pageSize)
        {
            if (cam != null)
            {
                mSceneCam = cam;
                mSceneMgr = mSceneCam.SceneManager;
                mOldCamPos = mSceneCam.DerivedPosition;
                mRootNode = mSceneMgr.RootSceneNode;
            }
            else
            {
                mSceneCam = null;
                mSceneMgr = null;
                mOldCamPos = Vector3.Zero;
            }
            mTimer = Root.Instance.Timer;
            mTimer.Reset();
            mLastTime = 0;
            PageSize = pageSize;
            mBounds = new TBounds(0, 0, 0, 0);

            mPageLoader = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetInfinite()
        {
            if (mMangerList.Count > 0)
                throw new Exception("PagedGeometry::setPageSize() cannot be called after detail levels have been added. Call removeDetailLevels() first.");

            mBounds = new TBounds(0, 0, 0, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxRange"></param>
        /// <returns></returns>
        public GeometryPageManager AddDetailLevel<T>(float maxRange) where T : GeometryPage
        {
            return AddDetailLevel<T>(maxRange, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxRange"></param>
        /// <param name="transitionLength"></param>
        /// <returns></returns>
        public GeometryPageManager AddDetailLevel<T>(float maxRange, float transitionLength) where T : GeometryPage
        {
            //Create a new page manager
            GeometryPageManager mgr = new GeometryPageManager(this);

            //If vertex shaders aren't supported, don't use transitions
            Root root = Root.Instance;
            float transLength = transitionLength;
            if (!root.RenderSystem.Capabilities.HasCapability(Axiom.Graphics.Capabilities.VertexPrograms))
                transLength = 0;

            //Add it to the list (also initializing maximum viewing distance)
            AddDetailLevel(mgr, maxRange, transLength);

            //And initialize the paged (dependent on maximum viewing distance)
            mgr.InitPages<T>(Bounds);

            return mgr;
        }
        /// <summary>
        /// 
        /// </summary>
        public void RemoveDetailLevels()
        {
            mMangerList.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            //If no camera has been set, then return without doing anything
            if (mSceneCam == null)
                return;
            //Calculate time since last update
            long deltaTime = 0, tmp = 0;
            tmp = mTimer.Milliseconds;
            deltaTime = tmp - mLastTime;
            mLastTime = tmp;

            //Get camera position and speed
            Vector3 camPos = ConvertToLocal(mSceneCam.DerivedPosition);
            Vector3 camSpeed = Vector3.Zero;
            if (deltaTime == 0)
            {
                camSpeed = new Vector3(0, 0, 0);
            }
            else
            {
                camSpeed = (camPos - mOldCamPos) / deltaTime;
            }

            mOldCamPos = camPos;

            if (mPageLoader != null)
            {
                //Update the PageLoader
                mPageLoader.FrameUpdate();
                //Update all the page managers
                bool enableCache = true;
                GeometryPageManager prevMgr = null;
                foreach (GeometryPageManager it in mMangerList)
                {
                    GeometryPageManager mgr = it;
                    mgr.Update(deltaTime, camPos, camSpeed, enableCache, prevMgr);
                    prevMgr = mgr;
                }

            }

            //Update misc. subsystems
            StaticBillboardSet.UpdateAll(ConvertToLocal(Camera.DerivedDirection));
        }
        /// <summary>
        /// 
        /// </summary>
        public void ReloadGeometry()
        {
            Debug.Assert(mPageLoader != null, "Pageloader can not be null!");
            foreach (GeometryPageManager mgr in mMangerList)
                mgr.ReloadGeometry();

        }
        /// <summary>
        /// 
        /// </summary>
        public void ReloadGeometryPage(Vector3 point)
        {
            if (mPageLoader == null)
                return;

            foreach (GeometryPageManager mgr in mMangerList)
                mgr.ReloadGeometryPage(point);
        }
        /// <summary>
        /// 
        /// </summary>
        public void ReloadGeometryPages(Vector3 center, float radius)
        {
            if (mPageLoader == null)
                return;

            foreach (GeometryPageManager mgr in mMangerList)
                mgr.ReloadGeometryPages(center,radius);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        public void ReloadGeometryPages(TBounds area)
        {
            if (mPageLoader == null)
                return;

            foreach (GeometryPageManager mgr in mMangerList)
                mgr.ReloadGeometryPages(area);
        }

        internal Vector3 ConvertToLocal(Vector3 globalVec)
        {
            Debug.Assert(SceneNode != null);

            return (SceneNode.Orientation.Inverse() * globalVec);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="maxRange"></param>
        /// <param name="transitionLength"></param>
        protected void AddDetailLevel(GeometryPageManager mgr, float maxRange, float transitionLength)
        {
            //Calculate the near range
            float minRange = 0;
            if (mMangerList.Count > 0)
            {
                GeometryPageManager lastMgr = mMangerList[mMangerList.Count - 1];
                minRange = lastMgr.FarRange;
            }
            //Error check
            if (maxRange <= minRange)
                throw new Exception("Closer detail levels must be added before farther ones");

            //Setup the new manager
            mgr.NearRange = minRange;
            mgr.FarRange = maxRange;
            mgr.Transition = transitionLength;
            mMangerList.Add(mgr);
        }
    }
}
