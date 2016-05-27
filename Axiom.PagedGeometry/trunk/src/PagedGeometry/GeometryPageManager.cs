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
    public class GeometryPageManager
    {
        /// <summary>
        /// 
        /// </summary>
        private PagedGeometry mMainGeom;
        /// <summary>
        /// A dynamic 2D array of pointers (2D grid of GeometryPage's)
        /// </summary>
        private GeometryPage[] mGeomGrid;
        /// <summary>
        /// A dynamic 1D array of pointers (temporary GeometryPage's used in scrolling geomGrid)
        /// </summary>
        private GeometryPage[] mScrollBuffer;
        /// <summary>
        /// 
        /// </summary>
        private int mGeomGridX;
        /// <summary>
        /// 
        /// </summary>
        private int mGeomGridZ;
        /// <summary>
        /// 
        /// </summary>
        private TBounds mGridBounds;
        private float mFadeLength;
        private float mFadeLengthSq;
        private bool mFadeEnabled;
        private long mCacheTimer;
        private List<GeometryPage> mPendingList = new List<GeometryPage>();
        private List<GeometryPage> mLoadedList = new List<GeometryPage>();
        private long mMaxCacheInterval;
        private long mInactivePageLife;
        private float mNearDist;
        private float mNearDistSq;
        private float mFarDist;
        private float mFarDistSq;
        private float mFarTransDist;
        private float mFarTransDistSq;
        public bool IsFadeEnabled
        {
            get { return mFadeEnabled; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float NearRange
        {
            set
            {
                mNearDist = value;
                mNearDistSq = mNearDist * mNearDist;
            }
            get { return mNearDist; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float FarRange
        {
            set
            {
                mFarDist = value;
                mFarDistSq = mFarDist * mFarDist;

                mFarTransDist = mFarDist + mFadeLength;
                mFadeLengthSq = mFarTransDist * mFarTransDist;
            }
            get
            {
                return mFarDist;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public float Transition
        {
            set
            {
                if (value > 0)
                {
                    mFadeLength = value;
                    mFadeLengthSq = mFadeLength * mFadeLength;
                    mFadeEnabled = true;
                }
                else
                {
                    mFadeLength = 0;
                    mFadeLengthSq = 0;
                    mFadeEnabled = false;
                }

                mFarTransDist = mFarDist + mFadeLength;
                mFarTransDistSq = mFarTransDist * mFarTransDist;
            }
            get { return mFadeLength; }
        }
        /// <summary>
        /// 
        /// </summary>
        internal List<GeometryPage> LoadedPages
        {
            get { return mLoadedList; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainGeom"></param>
        public GeometryPageManager(PagedGeometry mainGeom)
        {
            mMainGeom = mainGeom;
            mCacheTimer = 0;
            mScrollBuffer = null;
            mGeomGrid = null;
            SetCacheSpeed(200, 2000);
            Transition = 0;
            mGridBounds = new TBounds(0, 0, 0, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxCacheInterval"></param>
        /// <param name="inactivePageLife"></param>
        public void SetCacheSpeed(long maxCacheInterval, long inactivePageLife)
        {
            mMaxCacheInterval = maxCacheInterval;
            mInactivePageLife = inactivePageLife;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        internal void InitPages<T>(TBounds bounds)
        {
            
            // Calculate grid size, if left is Real minimum, it means that bounds are infinite
            // scrollBuffer is used as a flag. If it is allocated than infinite bounds are used
            // !!! Two cases are required because of the way scrolling is implemented
            // if it is redesigned it would allow to use the same functionality.
            if (bounds.Width < 0.00001f)
            {
                // In case of infinite bounds bounding rect needs to be calculated in a different manner, since
                // it represents local bounds, which are shifted along with the player's movements around the world.
                mGeomGridX = (int)((2 * mFarTransDist / mMainGeom.PageSize) + 4);
                mGridBounds.Top = 0;
                mGridBounds.Left = 0;
                mGridBounds.Right = mGeomGridX * mMainGeom.PageSize;
                mGridBounds.Bottom = mGeomGridX * mMainGeom.PageSize;
                // Allocate scroll buffer (used in scrolling the grid)
                mScrollBuffer = new GeometryPage[mGeomGridX];
                //Note: All this padding and transition preparation is performed because even in infinite
                //mode, a local grid size must be chosen. Unfortunately, this also means that view ranges
                //and transition lengths cannot be exceeded dynamically with set functions.
            }
            else
            {
                //Bounded mode
                mGridBounds = bounds;
                // In case the devision does not give the round number use the next largest integer
                mGeomGridX = (int)System.Math.Ceiling(mGridBounds.Width / mMainGeom.PageSize);
            }
            mGeomGridZ = mGeomGridX;//Note: geomGridX == geomGridZ; Need to merge.

            //Allocate grid array
            mGeomGrid = new GeometryPage[mGeomGridX * mGeomGridZ];
            for (int x = 0; x < mGeomGridX; ++x)
            {
                for (int z = 0; z < mGeomGridZ; ++z)
                {
                    GeometryPage p = null;
                    Type t = typeof(T);
                    switch (t.Name)
                    {
                        case "GrassPage":
                            p = new GrassPage();
                            break;
                        case "BatchPage":
                            p = new BatchPage();
                            break;
                        case "ImpostorPage":
                            p = new ImpostorPage();
                            break;
                        default:
                            throw new Exception("This GeometryPage is Unkown!,GeometryPageManager.Update!");
                            break;
                    }
                    p.Init(mMainGeom);
                    float cx = 0, cy = 0, cz = 0;
                    // 0,0 page is located at (gridBounds.left,gridBounds.top) corner of the bounds
                    cx = ((x + 0.5f) * mMainGeom.PageSize) + mGridBounds.Left;
                    cz = ((z + 0.5f) * mMainGeom.PageSize) + mGridBounds.Top;
                    cy = 0.0f;
                    p.CenterPoint = new Vector3(cx, cy, cz);
                    p.mXIndex = x;
                    p.mZIndex = z;
                    p.mInactiveTime = 0;
                    p.mIsLoaded = false;
                    p.mNeedsUnload = false;
                    p.mIsPending = false;
                    p.mIsVisible = false;
                    p.mUserData = null;
                    p.ClearBoundingBox();
                    SetGridPage(x, z, p);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="campos"></param>
        /// <param name="camSpeed"></param>
        /// <param name="enableCache"></param>
        /// <param name="prevManager"></param>
        internal void Update(long deltaTime, Vector3 campos, Vector3 camSpeed, bool enableCache, GeometryPageManager prevManager)
        {
            //-- Cache new geometry pages --

            //Cache 1 page ahead of the view ranges
            float cacheDist = mFarTransDist + mMainGeom.PageSize;
            float cacheDistSq = cacheDist * cacheDist;

            //First calculate the general area where the pages will be processed
            // 0,0 is the left top corner of the bounding box
            int x1 = (int)System.Math.Floor(
                ((campos.x - cacheDist) - mGridBounds.Left) / mMainGeom.PageSize);

            int x2 = (int)System.Math.Floor(
                ((campos.x + cacheDist) - mGridBounds.Left) / mMainGeom.PageSize);

            int z1 = (int)System.Math.Floor(
                ((campos.z - cacheDist) - mGridBounds.Top) / mMainGeom.PageSize);

            int z2 = (int)System.Math.Floor(
                ((campos.z + cacheDist) - mGridBounds.Top) / mMainGeom.PageSize);

            if (mScrollBuffer != null)
            {
                //Check if the page grid needs to be scrolled
                int shiftX = 0, shiftZ = 0;
                if (x1 < 0) shiftX = x1; else if (x2 >= mGeomGridX - 1) shiftX = x2 - (mGeomGridX - 1);
                if (z1 < 0) shiftZ = z1; else if (z2 >= mGeomGridZ - 1) shiftZ = z2 - (mGeomGridZ - 1);
                if (shiftX != 0 || shiftZ != 0)
                {
                    //Scroll grid
                    ScrollGridPages(shiftX, shiftZ);
                    //Update grid bounds and processing area
                    mGridBounds.Left += shiftX * mMainGeom.PageSize;
                    mGridBounds.Right += shiftX * mMainGeom.PageSize;
                    mGridBounds.Top += shiftZ * mMainGeom.PageSize;
                    mGridBounds.Bottom += shiftZ * mMainGeom.PageSize;
                    x1 -= shiftX; x2 -= shiftX;
                    z1 -= shiftZ; z2 -= shiftZ;
                }
            }
            else
            {
                // make sure that values are inbounds
                if (x2 >= mGeomGridX)
                    x2 = mGeomGridX - 1;
                if (z2 >= mGeomGridZ)
                    z2 = mGeomGridZ - 1;

                if (x1 < 0)
                    x1 = 0;
                if (z1 < 0)
                    z1 = 0;
            }

            //Now, in that general area, find what pages are within the cacheDist radius
            //Pages within the cacheDist radius will be added to the pending block list
            //to be loaded later, and pages within farDist will be loaded immediately.
            for (int x = x1; x <= x2; ++x)
            {
                for (int z = z1; z <= z2; ++z)
                {
                    GeometryPage blk = GetGridPage(x, z);
                    float dx = campos.x - blk.CenterPoint.x;
                    float dz = campos.z - blk.CenterPoint.z;
                    float distSq = dx * dx + dz * dz;

                    //If the page is in the cache radius...
                    if (distSq <= cacheDistSq)
                    {
                        //If the block hasn't been loaded yet, it should be
                        if (!blk.IsLoaded)
                        {
                            //Test if the block's distance is between nearDist and farDist
                            if (distSq >= mNearDistSq && distSq < mFarTransDistSq)
                            {
                                //If so, load the geometry immediately
                                LoadPage(blk);
                                mLoadedList.Add(blk);
                                if (blk.IsPending)
                                {
                                    mPendingList.Remove(blk);
                                    blk.IsPending = false;
                                }
                            }
                            else
                            {
                                //Otherwise, add it to the pending geometry list (if not already)
                                //Pages in then pending list will be loaded later (see below)
                                if (!blk.IsPending)
                                {
                                    mPendingList.Add(blk);
                                    blk.IsPending = true;
                                }
                            }
                        }
                        else
                        {
                            //Set the inactive time to 0 (since the page is active). This
                            //must be done in order to keep it from expiring (and deleted).
                            //This way, blocks not in the cache radius won't have their
                            //inactivity clock reset, and they will expire in a few seconds.
                            blk.InactiveTime = 0;
                        }
                    }
                }
            }//end for

            //Calculate cache speeds based on camera speed. This is important to keep the cache
            //process running smooth, because if the cache can't keep up with the camera, the
            //immediately visible pages will be forced to load instantly, which can cause
            //noticeable and sudden stuttering. The cache system results in smoother performance
            //because it smooths the loading tasks out across multiple frames. For example,
            //instead of loading 10+ blocks every 2 seconds, the cache would load 1 block every
            //200 milliseconds.
            float speed = Axiom.Math.Utility.Sqrt(camSpeed.x * camSpeed.x + camSpeed.z * camSpeed.z);
            long cacheInterval = 0;
            if (speed == 0)
            {
                cacheInterval = mMaxCacheInterval;
            }
            else
            {
                cacheInterval = (long)((mMainGeom.PageSize * 0.8f) / (speed * mPendingList.Count));
                    if(cacheInterval > mMaxCacheInterval)
                        cacheInterval = mMaxCacheInterval;
            }

            int geomPageBegin = 0;
            int geomPageEnd = mPendingList.Count - 1;
            //GeometryPage i1 = null;
            GeometryPage i2 = null;
            //Now load a single geometry page periodically, based on the cacheInterval
            mCacheTimer += deltaTime;
            if(mCacheTimer >= cacheInterval && enableCache)
            {
                //Find a block to be loaded from the pending list
                //i1 = mPendingList[geomPageBegin];
               // i2 = mPendingList[geomPageEnd];
                //while (i1 != i2)
                GeometryPage[] pend = mPendingList.ToArray();
                foreach(GeometryPage i1 in pend)
                {
                    GeometryPage blk = i1;
                    //Remove it from the pending list
                    mPendingList.Remove(i1);
                    //if (geomPageBegin < mPendingList.Count)
                    //    i1 = mPendingList[geomPageBegin++];
                    blk.IsPending = false;
                    //If it's within the geometry cache radius, load it and break out of the loop
                    float dx = campos.x - blk.CenterPoint.x;
                    float dz = campos.z - blk.CenterPoint.z;
                    float distSq = dx * dx + dz * dz;
                    if (distSq <= cacheDistSq)
                    {
                        LoadPage(blk);
                        mLoadedList.Add(blk);
                        blk = mLoadedList[mLoadedList.Count - 1];
                        enableCache = false;
                        break;
                    }
                    //Otherwise this will keep looping until an unloaded page is found
                }

                mCacheTimer = 0;

            }

            //-- Update existing geometry and impostors --

            //Loop through each loaded geometry block
            int loadPageBegin = 0;
            int loadPageEnd = mLoadedList.Count - 1;

            //i1 = mLoadedList[loadPageBegin];
           // i2 = mLoadedList[loadPageEnd];

            float halfPageSize = mMainGeom.PageSize * 0.5f;
            //while (i1 != i2)
            GeometryPage[] load = mLoadedList.ToArray();
            foreach(GeometryPage i1 in load)
            {
                GeometryPage blk = i1;
                //If the geometry has expired...
                if (blk.InactiveTime >= mInactivePageLife)
                {
                    //Unload it
                    UnLoadPage(blk);
                    mLoadedList.Remove(i1);
                    //if (loadPageBegin < mLoadedList.Count)
                    //    i1 = mLoadedList[loadPageBegin];
                }
                else
                {//Update it's visibility/fade status based on it's distance from the camera
                    bool visible = false;
                    float dx = campos.x - blk.CenterPoint.x;
                    float dz = campos.z - blk.CenterPoint.z;
                    float distSq = dx * dx + dz * dz;

                    float overlap = 0, tmp = 0;
                    tmp = blk.BoundingBox.Maximum.x - halfPageSize;
                    if (tmp > overlap) overlap = tmp;
                    tmp = blk.BoundingBox.Maximum.z - halfPageSize;
                    if (tmp > overlap) overlap = tmp;
                    tmp = blk.BoundingBox.Minimum.x + halfPageSize;
                    if (tmp > overlap) overlap = tmp;
                    tmp = blk.BoundingBox.Minimum.z + halfPageSize;
                    if (tmp > overlap) overlap = tmp;

                    float pageLengthSq = Axiom.Math.Utility.Sqr((mMainGeom.PageSize + overlap) * 1.41421356f);
                    if (distSq + pageLengthSq >= mNearDistSq && distSq - pageLengthSq < mFarTransDistSq)
                    {
                        //Fade the page when transitioning
                        bool enable = false;
                        float fadeNear = 0;
                        float fadeFar = 0;

                        if (mFadeEnabled && distSq + pageLengthSq >= mFarDistSq)
                        {
                            //Fade in
                            visible = true;
                            enable = true;
                            fadeNear = mFarDist;
                            fadeFar = mFarTransDist;
                        }
                        else if ( prevManager != null && prevManager.mFadeEnabled && (distSq - pageLengthSq < prevManager.mFarTransDistSq))
                        {
                            //Fade out
                            visible = true;
                            enable = true;
                            fadeNear = prevManager.mFarDist + (prevManager.mFarTransDist - prevManager.mFarDist) * 0.5f;//This causes geometry to fade out faster than it fades in, avoiding a state where a transition appears semitransparent
                            fadeFar = prevManager.mFarDist;
                        }

                        //apply fade settings
                        if (enable != blk.IsFadeEnabled)
                        {
                            blk.SetFade(enable, fadeNear, fadeFar);
                            blk.IsFadeEnabled = enable;
                        }
                    }
                    //Non-fade visibility
                    if (distSq > mNearDistSq && distSq < mFarDistSq)
                        visible = true;
                    //Update visibility
                    if (visible)
                    {
                        //Show the page if it isn't visible
                        if (!blk.IsVisible)
                        {
                            blk.IsVisible = true;
                        }
                    }
                    else
                    {
                        //Hide the page if it's not already
                        if (blk.IsVisible)
                        {
                            blk.IsVisible = false;
                        }
                    }

                    //And update it
                    blk.Update();
                    //if (loadPageBegin < mLoadedList.Count)
                    //    i1 = mLoadedList[loadPageBegin++];

                }
                //Increment the inactivity timer for the geometry
                blk.InactiveTime += deltaTime;

            }

        }
        /// <summary>
        /// 
        /// </summary>
        internal void ReloadGeometry()
        {
            foreach (GeometryPage it in mLoadedList)
            {
                GeometryPage page = it;
                UnLoadPage(page);
            }
            mLoadedList.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        internal void ReloadGeometryPage(Vector3 point)
        {
            //Determine which grid block contains the given points
            int x = (int)System.Math.Floor(mGeomGridX * (point.x - mGridBounds.Left) / mGridBounds.Width);
            int z = (int)System.Math.Floor(mGeomGridZ * (point.z - mGridBounds.Top) / mGridBounds.Height);
            //Unload the grid block if it's in the grid area, and is loaded
            if (x >= 0 && z >= 0 && x < mGeomGridX && z < mGeomGridZ)
            {
                GeometryPage page = GetGridPage(x, z);
                if (page.IsLoaded)
                {
                    UnLoadPage(page);
                    mLoadedList.Remove(page);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        internal void ReloadGeometryPages(TBounds area)
        {
            //Determine which grid block contains the top-left corner
            int x1 = (int)System.Math.Floor(mGeomGridX * (area.Left - mGridBounds.Left) / mGridBounds.Width);
            int z1 = (int)System.Math.Floor(mGeomGridZ * (area.Top - mGridBounds.Top) / mGridBounds.Height);

            if (x1 < 0) x1 = 0; else if (x1 > mGeomGridX) x1 = mGeomGridX;
            if (z1 < 0) z1 = 0; else if (z1 > mGeomGridZ) z1 = mGeomGridZ;

            //...and the bottom right
            int x2 = (int)System.Math.Floor(mGeomGridX * (area.Right - mGridBounds.Left) / mGridBounds.Width);
            int z2 = (int)System.Math.Floor(mGeomGridZ * (area.Bottom - mGridBounds.Top) / mGridBounds.Height);
            if (x2 < 0) x2 = 0; else if (x2 > mGeomGridX) x2 = mGeomGridX;
            if (z2 < 0) z2 = 0; else if (z2 > mGeomGridZ) z2 = mGeomGridZ;

            for (int x = x1; x <= x2; ++x)
            {
                for (int z = z1; z <= z2; ++z)
                {
                    GeometryPage page = GetGridPage(x, z);
                    if (page.IsLoaded)
                    {
                        UnLoadPage(page);
                        mLoadedList.Remove(page);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        internal void ReloadGeometryPages(Vector3 center, float radius)
        {
            //First calculate a square boundary to eliminate the search space
            TBounds area = new TBounds(center.x - radius, center.z - radius, center.x + radius, center.z + radius);

            //Determine which grid block contains the top-left corner
            int x1 = (int)System.Math.Floor(mGeomGridX * (area.Left - mGridBounds.Left) / mGridBounds.Width);
            int z1 = (int)System.Math.Floor(mGeomGridZ * (area.Top - mGridBounds.Top) / mGridBounds.Height);

            if (x1 < 0) x1 = 0; else if (x1 > mGeomGridX) x1 = mGeomGridX;
            if (z1 < 0) z1 = 0; else if (z1 > mGeomGridZ) z1 = mGeomGridZ;

            //...and the bottom right
            int x2 = (int)System.Math.Floor(mGeomGridX * (area.Right - mGridBounds.Left) / mGridBounds.Width);
            int z2 = (int)System.Math.Floor(mGeomGridZ * (area.Bottom - mGridBounds.Top) / mGridBounds.Height);
            if (x2 < 0) x2 = 0; else if (x2 > mGeomGridX) x2 = mGeomGridX;
            if (z2 < 0) z2 = 0; else if (z2 > mGeomGridZ) z2 = mGeomGridZ;

            //Scan all the grid blocks in the region
            float radiusSq = radius * radius;
            for (int x = x1; x <= x2; ++x)
            {
                for (int z = z1; z <= z2; ++z)
                {
                    GeometryPage page = GetGridPage(x, z);
                    if (page.IsLoaded)
                    {
                        Vector3 pos = page.CenterPoint;
                        Real distX = (pos.x - center.x), distZ = (pos.z - center.z);
                        Real distSq = distX * distX + distZ * distZ;

                        if (distSq <= radius)
                        {
                            UnLoadPage(page);
                            mLoadedList.Remove(page);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private GeometryPage GetGridPage(int x, int z)
        {
            if (x >= mGeomGridX || z >= mGeomGridZ)
                throw new Exception("Grid dimension is out of bounds, GeometryPageManager.GetGridPage()");

            return mGeomGrid[z * mGeomGridX + x];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="page"></param>
        private void SetGridPage(int x, int z, GeometryPage page)
        {
            if (x >= mGeomGridX || z >= mGeomGridZ)
                throw new Exception("Grid dimension is out of bounds, GeometryPageManager.GetGridPage()");

            mGeomGrid[z * mGeomGridX + x] = page;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        private void LoadPage(GeometryPage page)
        {
            //Calculate page info
            PageInfo info = new PageInfo();
            float halfPageSize = mMainGeom.PageSize * 0.5f;

            info.Bounds = new TBounds(0, 0, 0, 0);
            info.Bounds.Left = page.CenterPoint.x - halfPageSize;
            info.Bounds.Right = page.CenterPoint.x + halfPageSize;
            info.Bounds.Top = page.CenterPoint.z - halfPageSize;
            info.Bounds.Bottom = page.CenterPoint.z + halfPageSize;
            info.CenterPoint = page.CenterPoint;
            info.XIndex = page.mXIndex;
            info.ZIndex = page.mZIndex;
            info.UserData = page.mUserData;

            //Check if page needs unloading (if a delayed unload has been issued)
            if (page.mNeedsUnload)
            {
                page.RemoveEntites();
                mMainGeom.PageLoader.UnloadPage(info);
                page.mUserData = null;
                page.mNeedsUnload = false;
                page.ClearBoundingBox();
            }
            //Load the page
            page.SetRegion(info.Bounds.Left, info.Bounds.Top, info.Bounds.Right, info.Bounds.Bottom);
            mMainGeom.PageLoader.mGeomPage = page;
            mMainGeom.PageLoader.LoadPage(info);
            page.mUserData = info.UserData;
            page.Build();
            page.IsVisible = page.mIsVisible;
            page.InactiveTime = 0;
            page.mIsLoaded = true;
            page.mIsFadeEnabled = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        private void UnLoadPage(GeometryPage page)
        {
            //Calculate boundaries to unload
            PageInfo info = new PageInfo();
            float halfPageSize = mMainGeom.PageSize * 0.5f;

            info.Bounds = new TBounds(0, 0, 0, 0);
            info.Bounds.Left = page.CenterPoint.x - halfPageSize;
            info.Bounds.Right = page.CenterPoint.x + halfPageSize;
            info.Bounds.Top = page.CenterPoint.z - halfPageSize;
            info.Bounds.Bottom = page.CenterPoint.z + halfPageSize;
            info.CenterPoint = page.CenterPoint;
            info.XIndex = page.mXIndex;
            info.ZIndex = page.mZIndex;
            info.UserData = page.mUserData;

            page.RemoveEntites();
            mMainGeom.PageLoader.UnloadPage(info);
            page.mUserData = null;
            page.mNeedsUnload = false;
            page.ClearBoundingBox();

            page.InactiveTime = 0;
            page.mIsLoaded = false;
            page.mIsFadeEnabled = false;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        private void UnLoadPageDelayed(GeometryPage page)
        {
            page.mNeedsUnload = true;
            page.mIsLoaded = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shiftX"></param>
        /// <param name="shiftZ"></param>
        private void ScrollGridPages(int shiftX, int shiftZ)
        {
            //Check if the camera moved completely out of the grid
            if (shiftX > mGeomGridX || shiftX < -mGeomGridX || shiftZ > mGeomGridZ || shiftZ < -mGeomGridZ)
            {
                //If so, just reload all the tiles (reloading is accomplished by unloading - loading is automatic)
                for (int x = 0; x < mGeomGridX; x++)
                {
                    for (int z = 0; z < mGeomGridZ; z++)
                    {
                        GeometryPage page = GetGridPage(x, z);
                        if (page.IsLoaded)
                        {
                            UnLoadPage(page);
                            mLoadedList.Remove(page);
                        }
                        float cx = shiftX * mMainGeom.PageSize;
                        float cz = shiftZ * mMainGeom.PageSize;
                        page.CenterPoint = new Vector3(page.CenterPoint.x + cx, page.CenterPoint.y, page.CenterPoint.z + cz);
                        page.mXIndex += shiftX;
                        page.mZIndex += shiftZ;
                    }
                }
            }
            else//If not, scroll the grid by the X and Y values
            {
                if (shiftX > 0)//Scroll horizontally (X)
                {
                    for (int z = 0; z < mGeomGridZ; z++)
                    {
                        //Temporarily store off-shifted pages first
                        for (int x = 0; x < shiftX; x++)
                        {
                            GeometryPage page = GetGridPage(x, z);
                            if (page.IsLoaded)
                            {
                                UnLoadPage(page);
                                mLoadedList.Remove(page);
                            }
                            mScrollBuffer[x] = page;
                        }

                        //shift left
                        int shiftedMidPoint = mGeomGridX - shiftX;
                        for (int x = 0; x < shiftedMidPoint; x++)
                        {
                            SetGridPage(x, z, GetGridPage(x + shiftX, z));
                        }
                        //Rotate temporary pages around to other side of grid
                        for (int x = 0; x < shiftX; x++)
                        {
                            float cx = mGeomGridX * mMainGeom.PageSize;
                            mScrollBuffer[x].CenterPoint = new Vector3(
                                mScrollBuffer[x].CenterPoint.x + cx,
                                mScrollBuffer[x].CenterPoint.y,
                                mScrollBuffer[x].CenterPoint.z);
                            mScrollBuffer[x].mXIndex += mGeomGridX;
                            SetGridPage(x + shiftedMidPoint, z, mScrollBuffer[x]);
                        }
                    }
                }
                else if (shiftX < 0)
                {
                    for (int z = 0; z < mGeomGridZ; ++z)
                    {
                        //Temporarily store off-shifted pages first
                        int initialMidpoint = mGeomGridX+ shiftX;
                        for (int x = initialMidpoint; x < mGeomGridX; ++x)
                        {
                            GeometryPage page = GetGridPage(x, z);
                            if (page.IsLoaded)
                            {
                                UnLoadPageDelayed(page);
                                mLoadedList.Remove(page);
                            }
                            mScrollBuffer[x - initialMidpoint] = page;
                        }
                        //Shift right
                        for (int x = mGeomGridX - 1; x >= -shiftX; x--)
                        {
                            SetGridPage(x, z, GetGridPage(x + shiftX, z));
                        }
                        //Rotate temporary pages around to other side of grid
                        for (int x = 0; x < -shiftX; ++x)
                        {
                            float cx = mGeomGridX * mMainGeom.PageSize;
                            mScrollBuffer[x].CenterPoint = new Vector3(
                                mScrollBuffer[x].CenterPoint.x - cx,
                                mScrollBuffer[x].CenterPoint.y,
                                mScrollBuffer[x].CenterPoint.z);
                            mScrollBuffer[x].mXIndex -= mGeomGridX;
                            SetGridPage(x, z, mScrollBuffer[x]);
                        }
                    }
                }
                //Scroll vertically (Z)
                if (shiftZ > 0)
                {
                    for (int x = 0; x < mGeomGridX; x++)
                    {
                        //Temporarily store off-shifted pages first
                        for (int z = 0; z < shiftZ; z++)
                        {
                            GeometryPage page = GetGridPage(x, z);
                            if (page.IsLoaded)
                            {
                                UnLoadPageDelayed(page);
                                mLoadedList.Remove(page);
                            }
                            mScrollBuffer[z] = page;
                        }
                        //Shift left
                        int shiftedMidPoint = mGeomGridZ - shiftZ;
                        for (int z = 0; z < shiftedMidPoint; z++)
                        {
                            SetGridPage(x, z, GetGridPage(x, z + shiftZ));
                        }
                        //Rotate temporary pages around to other side of grid
                        for (int z = 0; z < shiftZ; z++)
                        {
                            float cz = mGeomGridZ * mMainGeom.PageSize;
                            mScrollBuffer[z].CenterPoint = new Vector3(
                                mScrollBuffer[z].CenterPoint.x,
                                mScrollBuffer[z].CenterPoint.y,
                                mScrollBuffer[z].CenterPoint.z + cz);
                            mScrollBuffer[z].mZIndex += mGeomGridZ;
                            SetGridPage(x,z + shiftedMidPoint,mScrollBuffer[z]);
                        }
                    }
                }
                else if(shiftZ < 0)
                {
                    for(int x = 0; x <mGeomGridX;x++)
                    {
                        //Temporarily store off-shifted pages first
                        int initialMidpoint = mGeomGridZ + shiftZ;
                        for(int z = initialMidpoint; z < mGeomGridZ;z++)
                        {
                            GeometryPage page = GetGridPage(x,z);
                            if(page.IsLoaded)
                            {
                                UnLoadPageDelayed(page);
                                mLoadedList.Remove(page);
                            }
                            mScrollBuffer[z - initialMidpoint] = page;
                        }
                        //shift right
                        for(int z = mGeomGridZ -1; z >= -shiftZ;z--)
                        {
                            SetGridPage(x,z,GetGridPage(x,z+shiftZ));
                        }
                       //Rotate temporary pages around to other side of grid
                        for(int z = 0; z < -shiftZ;z++)
                        {
                            float cz = mGeomGridZ * mMainGeom.PageSize;
                            mScrollBuffer[z].CenterPoint = new Vector3(
                                mScrollBuffer[z].CenterPoint.x,
                                mScrollBuffer[z].CenterPoint.y,
                                mScrollBuffer[z].CenterPoint.z - cz);

                            mScrollBuffer[z].mZIndex -= mGeomGridZ;
                            SetGridPage(x,z,mScrollBuffer[z]);
                        }
                    }
                }
            }
        }
    }
}
