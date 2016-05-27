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
    /// 
    /// </summary>
    public class TreeLoader3D : PageLoader
    {
        public struct TreeDef
        {
            public float YPos;
            public short XPos;
            public short ZPos;
            public byte Scale;
            public byte Rotation;
            public object UserData;
        }

        private int mPageGridX;
        private int mPageGridZ;
        private float mPageSize;
        private TBounds mGridBounds;
        private TBounds mActualBounds;
        private float mMaximumScale;
        private float mMinimumScale;
        private ColorMap mColorMap;
        private MapFilter mColorMapFilter;
        private PagedGeometry mGeom;

        public TreeIterator3D Trees
        {
            get { throw new NotSupportedException(); }
        }
        /// <summary>
        /// 
        /// </summary>
        public ColorMap ColorMap
        {
            get { return mColorMap; }
        }

        public MapFilter ColorMapFilter
        {
            set
            {
                mColorMapFilter = value;
                if (mColorMap != null)
                    mColorMap.Filter = mColorMapFilter;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        public float MaximumScale
        {
            set { mMaximumScale = value; }
            get { return mMaximumScale; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float MinimumScale
        {
            set { mMinimumScale = value; }
            get { return mMinimumScale; }
        }
        /// <summary>
        /// 
        /// </summary>
        public TBounds Bounds
        {
            get { return mActualBounds; }
        }
        private Dictionary<Entity, List<List<TreeDef>>> mPageGridList = new Dictionary<Entity, List<List<TreeDef>>>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        /// <param name="bounds"></param>
        public TreeLoader3D(PagedGeometry geom, TBounds bounds)
        {
            //Calculate grid size
            mGeom = geom;
            mPageSize = mGeom.PageSize;

            //Make sure the bounds are aligned with PagedGeometry's grid, so the TreeLoader's grid tiles will have a 1:1 relationship
            mActualBounds = bounds;
            mGridBounds = bounds;

            mGridBounds.Left = (float)(mPageSize * System.Math.Floor((mGridBounds.Left - mGeom.Bounds.Left) / mPageSize) + mGeom.Bounds.Left);
            mGridBounds.Top = (float)(mPageSize * System.Math.Floor((mGridBounds.Top - mGeom.Bounds.Top) / mPageSize) + mGeom.Bounds.Top);
            mGridBounds.Right = (float)(mPageSize * System.Math.Ceiling((mGridBounds.Right - mGeom.Bounds.Left) / mPageSize) + mGeom.Bounds.Left);
            mGridBounds.Bottom = (float)(mPageSize * System.Math.Ceiling((mGridBounds.Bottom - mGeom.Bounds.Top) / mPageSize) + mGeom.Bounds.Top);

            //Calculate page grid size
            mPageGridX = (int)(System.Math.Ceiling(mGridBounds.Width / mPageSize) + 1);
            mPageGridZ = (int)(System.Math.Ceiling(mGridBounds.Height / mPageSize) + 1);

            //Reset color map
            mColorMap = null;
            mColorMapFilter = MapFilter.None;

            //Default scale range
            mMaximumScale = 2.0f;
            mMinimumScale = 0.0f;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position"></param>
        public void AddTree(Entity entity, Vector3 position)
        {
            AddTree(entity, position, new Degree((Real)0), 1.0f, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position"></param>
        /// <param name="yaw"></param>
        public void AddTree(Entity entity, Vector3 position, Degree yaw)
        {
            AddTree(entity, position, yaw, 1.0f, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position"></param>
        /// <param name="yaw"></param>
        /// <param name="scale"></param>
        public void AddTree(Entity entity, Vector3 position, Degree yaw,float scale)
        {
            AddTree(entity, position, yaw, scale, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position"></param>
        /// <param name="yaw"></param>
        /// <param name="scale"></param>
        /// <param name="userData"></param>
        public void AddTree(Entity entity, Vector3 position, Degree yaw, float scale, object userData)
        {
            //First convert the coordinate to PagedGeometry's local system
            Vector3 pos = mGeom.ConvertToLocal(position);

            //If the tree is slightly out of bounds (due to imprecise coordinate conversion), fix it
            if (pos.x < mActualBounds.Left)
                pos.x = mActualBounds.Left;
            else if (pos.x > mActualBounds.Right)
                pos.x = mActualBounds.Right;

            if (pos.z < mActualBounds.Top)
                pos.z = mActualBounds.Top;
            else if (pos.z > mActualBounds.Bottom)
                pos.z = mActualBounds.Bottom;

            //Check that the tree is within bounds (DEBUG)
            float smallVal = 0.01f;
            if (pos.x < mActualBounds.Left - smallVal || pos.x > mActualBounds.Right + smallVal || pos.z < mActualBounds.Top - smallVal || pos.z > mActualBounds.Bottom + smallVal)
                throw new Exception("Tree position is out of bounds");
            if (scale < mMinimumScale || scale > mMaximumScale)
                throw new Exception("Tree scale out of range");

            //Find the appropriate page grid for the entity
            List<List<TreeDef>> pageGrid = null;
            if (!mPageGridList.TryGetValue(entity, out pageGrid))
            {
                //If it does not exist, create a new page grid
                pageGrid = new List<List<TreeDef>>();
                for (int i = 0; i < (mPageGridX * mPageGridZ); i++)
                    pageGrid.Add(new List<TreeDef>());

                //Register the new page grid in the pageGridList for later retrieval
                mPageGridList.Add(entity, pageGrid);
            }

            //Calculate the gridbounds-relative position of the tree
            float xrel = pos.x - mGridBounds.Left;
            float zrel = pos.z - mGridBounds.Top;

            //Get the appropriate grid element based on the new tree's position
            int pageX = (int)System.Math.Floor(xrel / mPageSize);
            int pageZ = (int)System.Math.Floor(zrel / mPageSize);
            List<TreeDef> treeList = GetGridPage(pageGrid, pageX, pageZ);

            //Create the new tree
            TreeDef tree = new TreeDef();
            tree.YPos = pos.y;
            tree.XPos = (short)(65535 * (xrel - (pageX * mPageSize)) / mPageSize);
            tree.ZPos = (short)(65535 * (zrel - (pageZ * mPageSize)) / mPageSize);
            tree.Rotation = (byte)(255 * ((Real)yaw / 360.0f));
            tree.Scale = (byte)(255 * ((scale-mMinimumScale) / mMaximumScale));
            tree.UserData = userData;

            //Add it to the tree list
            treeList.Add(tree);

            //Rebuild geometry if necessary
            mGeom.ReloadGeometryPage(pos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapFile"></param>
        public void SetMapColor(string mapFile)
        {
            SetMapColor(mapFile, MapChannel.Color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapFile"></param>
        /// <param name="channel"></param>
        public void SetMapColor(string mapFile, MapChannel channel)
        {
            if (mColorMap != null)
            {
                mColorMap.Unload();
                mColorMap = null;
            }
            if (mapFile != "")
            {
                mColorMap = ColorMap.Load(mapFile, channel);
                mColorMap.MapBounds = mActualBounds;
                mColorMap.Filter = mColorMapFilter;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        public void SetMapColor(Texture map)
        {
            SetMapColor(map, MapChannel.Color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="channel"></param>
        public void SetMapColor(Texture map, MapChannel channel)
        {
            if (mColorMap != null)
            {
                mColorMap.Unload();
                mColorMap = null;
            }
            if (map != null)
            {
                mColorMap = ColorMap.Load(map, channel);
                mColorMap.MapBounds = mActualBounds;
                mColorMap.Filter = mColorMapFilter;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public List<object> DeleteTrees(Vector3 position, float radius)
        {
            return DeleteTrees(position, radius, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<object> DeleteTrees(Vector3 position, float radius, Entity type)
        {
            //First convert the coordinate to PagedGeometry's local system
            Vector3 pos = mGeom.ConvertToLocal(position);

            //Keep a list of user-defined data associated with deleted trees
            List<object> deletedUserData = new List<object>();

            //If the tree is slightly out of bounds (due to imprecise coordinate conversion), fix it
            if (pos.x < mActualBounds.Left)
                pos.x = mActualBounds.Left;
            else if (pos.x > mActualBounds.Right)
                pos.x = mActualBounds.Right;

            if (pos.z < mActualBounds.Top)
                pos.z = mActualBounds.Top;
            else if (pos.z > mActualBounds.Bottom)
                pos.z = mActualBounds.Bottom;

            //Determine the grid blocks which might contain the requested trees
            int minPageX = (int)System.Math.Floor(((pos.x - radius) - mGridBounds.Left) / mPageSize);
            int minPageZ = (int)System.Math.Floor(((pos.z - radius) - mGridBounds.Top) / mPageSize);
            int maxPageX = (int)System.Math.Floor(((pos.x + radius) - mGridBounds.Left) / mPageSize);
            int maxPageZ = (int)System.Math.Floor(((pos.z + radius) - mGridBounds.Top) / mPageSize);
            float radiusSq = radius * radius;

            if (minPageX < 0) minPageX = 0; else if (minPageX >= mPageGridX) minPageX = mPageGridX - 1;
            if (minPageZ < 0) minPageZ = 0; else if (minPageZ >= mPageGridZ) minPageZ = mPageGridZ - 1;
            if (maxPageX < 0) maxPageX = 0; else if (maxPageX >= mPageGridX) maxPageX = mPageGridX - 1;
            if (maxPageZ < 0) maxPageZ = 0; else if (maxPageZ >= mPageGridZ) maxPageZ = mPageGridZ - 1;

            //Scan all the grid blocks
            foreach (KeyValuePair<Entity, List<List<TreeDef>>> it in mPageGridList)
            {
                List<List<TreeDef>> pageGrid = it.Value;
                for (int tileZ = minPageZ; tileZ <= maxPageZ; tileZ++)
                {
                    for (int tileX = maxPageX; tileX <= maxPageX; tileX++)
                    {
                        bool modified = false;
                        //Scan all trees in grid block
                        List<TreeDef> treeList = GetGridPage(pageGrid, tileX, tileZ);
                        int i = 0;
                        while (i < treeList.Count)
                        {
                            //Get tree distance
                            float distX = (mGridBounds.Left + (tileX * mPageSize) + ((float)treeList[i].XPos / 65535) * mPageSize) - pos.x;
                            float distZ = (mGridBounds.Top + (tileZ * mPageSize) + ((float)treeList[i].ZPos / 65535) * mPageSize) - pos.z;
                            float distSq = distX * distX + distZ * distZ;

                            if (distSq <= radiusSq)
                            {
                                deletedUserData.Add(treeList[i].UserData);
                                //If it's within the radius, delete it
                                treeList[i] = treeList[treeList.Count - 1];
                                treeList.Remove(treeList[treeList.Count - 1]);
                                modified = true;
                            }
                            else
                                i++;
                        }

                        //Rebuild geometry if necessary
                        if (modified)
                        {
                            Vector3 poss = new Vector3(mGridBounds.Left + ((0.5f + tileX) * mPageSize), 0, mGridBounds.Top + ((0.5f + tileZ) * mPageSize));
                            mGeom.ReloadGeometryPage(poss);
                        }
                    }
                }
            }
            return deletedUserData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public override void LoadPage(PageInfo page)
        {
            //Calculate x/z indexes for the grid array
            page.XIndex -= (int)System.Math.Floor(mGridBounds.Left / mPageSize);
            page.ZIndex -= (int)System.Math.Floor(mGridBounds.Top / mPageSize);

            //Check if the requested page is in bounds
            if (page.XIndex < 0 || page.ZIndex < 0 || page.XIndex >= mPageGridX || page.ZIndex >= mPageGridZ)
                return;

            //For each tree type...
            foreach (KeyValuePair<Entity, List<List<TreeDef>>> it in mPageGridList)
            {
                //Get the appropriate tree list for the specified page
                List<List<TreeDef>> pageGrid = it.Value;
                List<TreeDef> treeList = GetGridPage(pageGrid, page.XIndex, page.ZIndex);
                Entity entity = it.Key;

                //Load the listed trees into the page
                foreach (TreeDef o in treeList)
                {
                    //Get position
                    Vector3 pos = Vector3.Zero;
                    pos.x = page.Bounds.Left + ((float)o.XPos / 65535) * mPageSize;
                    pos.z = page.Bounds.Top + ((float)o.ZPos / 65535) * mPageSize;
                    pos.y = o.YPos;

                    //Get rotation
                    Degree angle = new Degree((Real)o.Rotation * (360.0f / 255));
                    Quaternion rot = Quaternion.FromAngleAxis((float)angle.InRadians, Vector3.UnitY);

                    //Get scale
                    Vector3 scale = Vector3.Zero;
                    scale.y = (float)(o.Scale * (mMaximumScale / 255) + mMinimumScale);
                    scale.x = scale.y;
                    scale.z = scale.y;

                    //Get color
                    ColorEx col = ColorEx.White;
                    if (mColorMap != null)
                        col = mColorMap.GetColorAtUnpacked(pos.x, pos.z);

                    AddEntity(entity, pos, rot, scale, col);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private List<TreeDef> GetGridPage(List<List<TreeDef>> grid, int x, int z)
        {
            if (x >= mPageGridX || z >= mPageGridZ)
                throw new Exception("Grid dimension is out of bounds");

           return grid[z * mPageGridX + x];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="page"></param>
        private void SetGridPage(List<List<TreeDef>> grid, int x, int z, List<TreeDef> page)
        {
            if (x >= mPageGridX || z >= mPageGridZ)
                throw new Exception("Grid dimension is out of bounds");

            grid[z * mPageGridX + x] = page;
        }
    }
}
