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

namespace Axiom.Forests
{
    /// <summary>
    /// A custom page type designed specifically for use with GrassLoader.
    /// </summary>
    /// <remarks>
    /// You can use this in your own project if you want, but remember that no optimizations
    /// are performed. The given entity is simply cloned and attached to a new scene node as
    /// quickly and simply as possible (this means there's no batching overhead as in BatchPage,
    /// but it also means potentially poor performance if you don't know what you're doing).
    /// </remarks>
    public class GrassPage : GeometryPage
    {
        private SceneManager mSceneMgr;
        private SceneNode mRootNode;
        private List<SceneNode> mNodeList = new List<SceneNode>();
        private static long GUID = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        public override void Init(PagedGeometry geom)
        {
            mSceneMgr = geom.SceneManager;
            mRootNode = geom.SceneNode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public override void AddEntity(Entity entity, Axiom.Math.Vector3 position, Axiom.Math.Quaternion rotation, Axiom.Math.Vector3 scale, ColorEx color)
        {
            SceneNode node = mRootNode.CreateChildSceneNode();
            node.Position = position;
            mNodeList.Add(node);

            Entity ent = entity.Clone(GetUniqueID());
            ent.CastShadows = false;
            ent.RenderQueueGroup = entity.RenderQueueGroup;
            node.AttachObject(ent);
        }
        /// <summary>
        /// 
        /// </summary>
        public override void RemoveEntites()
        {
            for (int i = 0; i < mNodeList.Count; i++)
            {
                SceneNode node = mNodeList[i];
                mSceneMgr.DestroySceneNode(node);
            }
            mNodeList.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="visibleDist"></param>
        /// <param name="invisibleDist"></param>
        public override void SetFade(bool enabled, float visibleDist, float invisibleDist)
        {
            //base.SetFade(enabled, visibleDist, invisibleDist);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        public override void SetVisible(bool visible)
        {
            foreach (SceneNode node in mNodeList)
                node.IsVisible = visible;
        }
        private static string GetUniqueID()
        {
            return "GrassPage" + ++GUID;
        }
    }
}
