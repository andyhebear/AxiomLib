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
    public class ImpostorBatch
    {
        protected ImpostorTexture mTex;
        protected StaticBillboardSet mBBSet;
        protected Vector3 mEntityBBCenter;
        protected ImpostorPage mIGroup;
        protected short mPitchIndex;
        protected short mYawIndex;
        protected static long GUID;
        /// <summary>
        /// 
        /// </summary>
        public BillboardOrigin BillboardOrigin
        {
            set 
            {
                mBBSet.BillboardOrigin = value;
                if (mBBSet.BillboardOrigin == BillboardOrigin.Center)
                    mEntityBBCenter = mTex.mEntityCenter;
                else if (mBBSet.BillboardOrigin == BillboardOrigin.BottomCenter)
                    mEntityBBCenter = new Vector3(mTex.mEntityCenter.x, mTex.mEntityCenter.y - mTex.mEntityRadius, mTex.mEntityCenter.z);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="entity"></param>
        protected ImpostorBatch(ImpostorPage group, Entity entity)
        {
            //Render impostor texture for this entity
            mTex = ImpostorTexture.GetTexture(group, entity);

            //Create billboard set
            mBBSet = new StaticBillboardSet(group.SceneManager, group.PagedGeometry.SceneNode);
            mBBSet.SetTextureStacksAndSlices(ImpostorTexture.ImpostorPitchAngles, ImpostorTexture.ImpostorYawAngles);

            BillboardOrigin = ImpostorPage.ImpostorPivot;
            //Default the angle to 0 degrees
            mPitchIndex = -1;
            mYawIndex = -1;
            SetAngle(0.0f, 0.0f);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static ImpostorBatch GetBatch(ImpostorPage group, Entity entity)
        {
            //Search for an existing impostor batch for this entity
            string entityKey = GenerateEntityKey(entity);
            ImpostorBatch batch = null;
            if (!group.mImpostorBatches.TryGetValue(entityKey, out batch))
            {
                //Otherwise, create a new batch
                batch = new ImpostorBatch(group, entity);
                //Add it to the impostorBatches list
                group.mImpostorBatches.Add(entityKey, batch);
            }
            return batch;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GenerateEntityKey(Entity entity)
        {
            string entityKey = string.Empty;
            entityKey = entity.Mesh.Name;
            for (int i = 0; i < entity.SubEntityCount; i++)
                entityKey += "-" + entity.GetSubEntity(i).Name;

            return entityKey;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Build()
        {
            mBBSet.Build();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            mBBSet.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            mBBSet.SetVisible(visible);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="visibleDist"></param>
        /// <param name="invisibleDist"></param>
        public void SetFade(bool enabled, float visibleDist, float invisibleDist)
        {
            mBBSet.SetFade(enabled, visibleDist, invisibleDist);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public void AddBillboard(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            AddBillboard(position, rotation, scale, ColorEx.White);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public void AddBillboard(Vector3 position, Quaternion rotation, Vector3 scale, ColorEx color)
        {
            Vector3 zVector = rotation * Vector3.UnitZ;
            float degrees = (float)new Degree((Real)Axiom.Math.Utility.ATan2(zVector.x, zVector.z));
            if (degrees < 0) degrees += 360;

            int n = (int)(ImpostorTexture.ImpostorYawAngles * (degrees / 360.0f) + 0.5f);
            short texCoordIndx = (short)((ImpostorTexture.ImpostorYawAngles - n) % ImpostorTexture.ImpostorYawAngles);

            mBBSet.CreateBillboard(position + (rotation * mEntityBBCenter) * scale,
                mTex.mEntityDiameter * 0.5f * (scale.x + scale.z),
                mTex.mEntityDiameter * scale.y, color,
                texCoordIndx);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pitchDeg"></param>
        /// <param name="yawDeg"></param>
        public void SetAngle(float pitchDeg, float yawDeg)
        {
            //Calculate pitch material index
            short newPitchIndex = 0;
            if (pitchDeg > 0)
            {
                newPitchIndex = (short)(ImpostorTexture.ImpostorPitchAngles * (pitchDeg / 67.5f));
                if (newPitchIndex > ImpostorTexture.ImpostorPitchAngles - 1)
                    newPitchIndex = (short)(ImpostorTexture.ImpostorPitchAngles - 1);
            }
            else
            {
                newPitchIndex = 0;
            }

            //Calculate yaw material index
            short newYawIndex = 0;
            if (yawDeg > 0)
            {
                newYawIndex = (short)(ImpostorTexture.ImpostorYawAngles * (yawDeg / 360.0f) % ImpostorTexture.ImpostorYawAngles);
            }
            else
            {
                newYawIndex = (short)((ImpostorTexture.ImpostorYawAngles + ImpostorTexture.ImpostorYawAngles * (yawDeg / 360.0f) + 0.5f) % ImpostorTexture.ImpostorYawAngles);
            }

            //Change materials if necessary
            if (newPitchIndex != mPitchIndex || newYawIndex != mYawIndex)
            {
                mPitchIndex = newPitchIndex;
                mYawIndex = newYawIndex;
                mBBSet.MaterialName = mTex.mMaterial[mPitchIndex, mYawIndex].Name;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        protected static string GetUniqueID(string prefix)
        {
            return prefix + ++GUID;
        }


    }
}
