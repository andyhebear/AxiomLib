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
    public class ImpostorPage : GeometryPage
    {
        private SceneManager mSceneMgr;
        private PagedGeometry mGeom;
        private ImpostorBlendMode mBlendMode;
        private static int mImpostorResolution = 128;
        private static ColorEx mImpostorBackgroundColor = new ColorEx(0, 0, 0.3f, 0);
        private static BillboardOrigin mImpostorPivot = BillboardOrigin.Center;
        private static uint mSelfInstances = 0;
        private static uint mUpdateInstanceID = 0;
        private uint mInstanceID;
        private ITimer mUpdateTimeer;
        private Vector3 mCenter;
        private int mAveCounter;
        internal Dictionary<string, ImpostorBatch> mImpostorBatches = new Dictionary<string, ImpostorBatch>();

        public static ColorEx ImpostorBackgroundColor
        {
            get { return mImpostorBackgroundColor; }
        }
        public ImpostorBlendMode BlendMode
        {
            get { return mBlendMode; }
        }
        public PagedGeometry PagedGeometry
        {
            get { return mGeom; }
        }
        public SceneManager SceneManager
        {
            get { return mSceneMgr; }
        }
        /// <summary>
        /// 
        /// </summary>
        public static int ImpostorResolution
        {
            set { mImpostorResolution = value; }
            get { return mImpostorResolution; }
        }
        /// <summary>
        /// 
        /// </summary>
        public static ColorEx ImpostorColor
        {
            set
            { 
                mImpostorBackgroundColor = value;
                mImpostorBackgroundColor.a = 0;
            }
            get
            {
                return mImpostorBackgroundColor;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static BillboardOrigin ImpostorPivot
        {
            set
            {
                if (value != BillboardOrigin.Center && value != BillboardOrigin.BottomCenter)
                    throw new Exception("Invalid origin - only BBO_CENTER and BBO_BOTTOM_CENTER is supported");
                mImpostorPivot = value;
            }
            get
            {
                return mImpostorPivot;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ImpostorBlendMode ImpostorBlendMode
        {
            set { mBlendMode = value; }
            get { return mBlendMode; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        public override void Init(PagedGeometry geom)
        {
            //Save pointers to PagedGeometry object
            mSceneMgr = geom.SceneManager;
            mGeom = geom;

            //Init. variables
            ImpostorBlendMode = ImpostorBlendMode.AlphaReject;

            if (++mSelfInstances == 1)
            {
                //Set up a single instance of a scene node which will be used when rendering impostor textures
                mGeom.SceneNode.CreateChildSceneNode("ImpostorPage.RenderNode");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public override void SetRegion(float left, float top, float right, float bottom)
        {
            //Calculate center of region
            mCenter = new Vector3();
            mCenter.x = (left + right) * 0.5f;
            mCenter.z = (top + bottom) * 0.5f;

            mCenter.y = 0.0f;
            mAveCounter = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public override void AddEntity(Axiom.Core.Entity ent, Axiom.Math.Vector3 position, Axiom.Math.Quaternion rotation, Axiom.Math.Vector3 scale, Axiom.Core.ColorEx color)
        {
            //Get the impostor batch that this impostor will be added to
            ImpostorBatch iBatch = ImpostorBatch.GetBatch(this, ent);

            //Then add the impostor to the batch
            iBatch.AddBillboard(position, rotation, scale, color);

            //Add the Y position to the center.y value (to be averaged later)
            mCenter.y += position.y + ent.BoundingBox.Center.y * scale.y;

            ++mAveCounter;
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Build()
        {
            //Calculate the average Y value of all the added entities
            if (mAveCounter != 0)
                mCenter.y /= mAveCounter;
            else
                mCenter.y = 0.0f;

            //Build all batches
            foreach (ImpostorBatch batch in mImpostorBatches.Values)
                batch.Build();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void RemoveEntites()
        {
            //Clear all impostor batches
            foreach (ImpostorBatch batch in mImpostorBatches.Values)
                batch.Clear();

            //Reset y center
            mCenter.y = 0;
            mAveCounter = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        public override void SetVisible(bool visible)
        {
            //Update visibility status of all batches
            foreach (ImpostorBatch batch in mImpostorBatches.Values)
                batch.SetVisible(visible);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="visibleDist"></param>
        /// <param name="invisibleDist"></param>
        public override void SetFade(bool enabled, float visibleDist, float invisibleDist)
        {
            //Update fade status of all batches
            foreach (ImpostorBatch batch in mImpostorBatches.Values)
                batch.SetFade(enabled, visibleDist, invisibleDist);
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Update()
        {
            //Calculate the direction the impostor batches should be facing
            Vector3 camPos = mGeom.ConvertToLocal(mGeom.Camera.DerivedPosition);

            //Update all batches
            float distX = camPos.x - mCenter.x;
            float distZ = camPos.z - mCenter.z;
            float distY = camPos.y - mCenter.y;
            float distRelZ = Axiom.Math.Utility.Sqrt(distX * distX + distZ * distZ);
            Radian pitch = Axiom.Math.Utility.ATan2(distY, distZ);

            Radian yaw;
            if (distRelZ > mGeom.PageSize * 3)
            {
                yaw = Axiom.Math.Utility.ATan2(distY, distZ);
            }
            else
            {
                Vector3 dir = mGeom.ConvertToLocal(mGeom.Camera.DerivedDirection);
                yaw = Axiom.Math.Utility.ATan2(-distX, -distZ);
            }

            foreach (ImpostorBatch batch in mImpostorBatches.Values)
                batch.SetAngle((float)pitch.InDegrees, (float)yaw.InDegrees);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public static void Regenerate(Entity entity)
        {
            ImpostorTexture tex = ImpostorTexture.GetTexture(null, entity);
            if (tex != null)
                tex.Regenerate();
        }
        /// <summary>
        /// 
        /// </summary>
        public static void RegenerateAll()
        {
            ImpostorTexture.RegenerateAll();
        }
    }
}
