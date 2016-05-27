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
using Axiom.Graphics;
namespace Axiom.Forests
{
    public class ImpostorTexture
    {
        public const int ImpostorYawAngles = 8;
        public const int ImpostorPitchAngles = 4;

        protected static Dictionary<string, ImpostorTexture> mSelfList = new Dictionary<string, ImpostorTexture>();
        protected SceneManager mSceneMgr;
        protected Entity mEntity;
        protected string mEntityKey;
        internal Material[,] mMaterial = new Material[ImpostorPitchAngles, ImpostorYawAngles];
        protected Texture mTexture;
        protected short mSourceMesh;
        protected AxisAlignedBox mBoundingBox;
        internal float mEntityDiameter;
        internal float mEntityRadius;
        internal Vector3 mEntityCenter;
        protected ImpostorTextureResourceLoader mLoader;
        protected static long GUID;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="entity"></param>
        protected ImpostorTexture(ImpostorPage group, Entity entity)
        {
            //Store scene manager and entity
            mSceneMgr = group.SceneManager;
            mEntity = entity;

            //Add self to list of ImpostorTexture's
            mEntityKey = ImpostorBatch.GenerateEntityKey(entity);
            mSelfList.Add(mEntityKey, this);

            //Calculate the entity's bounding box and it's diameter
            mBoundingBox = entity.BoundingBox;

            //Note - this radius calculation assumes the object is somewhat rounded (like trees/rocks/etc.)
            float tmp = 0;
            mEntityRadius = mBoundingBox.Maximum.x - mBoundingBox.Center.x;
            tmp = mBoundingBox.Maximum.y - mBoundingBox.Center.y;
            if (tmp > mEntityRadius) mEntityRadius = tmp;
            tmp = mBoundingBox.Maximum.z - mBoundingBox.Center.z;
            if (tmp > mEntityRadius) mEntityRadius = tmp;

            mEntityDiameter = 2.0f * mEntityRadius;
            mEntityCenter = mBoundingBox.Center;

            //Render impostor textures
            RenderTextures(false);
            //Set up materials
            for (int o = 0; o < ImpostorYawAngles; o++)
            {
                for (int i = 0; i < ImpostorPitchAngles; i++)
                {
                    mMaterial[i, o] = (Material)MaterialManager.Instance.Create(GetUniqueID("ImpostorMaterial"), "Impostors");

                    Material m = mMaterial[i, o];
                    Pass p = m.GetTechnique(0).GetPass(0);

                    TextureUnitState t = p.CreateTextureUnitState(mTexture.Name);
                    t.TextureScrollU = (float)(o / ImpostorYawAngles);
                    t.TextureScrollV = (float)(i / ImpostorPitchAngles);

                    p.LightingEnabled = false;
                    m.ReceiveShadows = false;

                    if (group.BlendMode == ImpostorBlendMode.AlphaReject)
                    {

						p.AlphaRejectFunction = CompareFunction.GreaterEqual;
						p.AlphaRejectValue = 128;
                    }
                    else if (group.BlendMode == ImpostorBlendMode.AlphaBlend)
                    {
                        p.SetSceneBlending(SceneBlendFactor.SourceAlpha, SceneBlendFactor.OneMinusSourceAlpha);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static ImpostorTexture GetTexture(ImpostorPage group, Entity entity)
        {
            //Search for an existing impostor texture for the given entity
            string entityKey = ImpostorBatch.GenerateEntityKey(entity);
            ImpostorTexture texture = null;
            if (!mSelfList.TryGetValue(entityKey, out texture))
            {
                if (group != null)
                {
                    texture = new ImpostorTexture(group, entity);
                }
            }
            return texture;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        public static void RemoveTexture(ImpostorTexture texture)
        {
            string strToRemove = string.Empty;
            //Search for an existing impostor texture, in case it was already deleted
            if (mSelfList.ContainsValue(texture))
            {
                foreach (KeyValuePair<string, ImpostorTexture> kp in mSelfList)
                {
                    if (kp.Value == texture)
                    {
                        strToRemove = kp.Key;
                        break;
                    }
                }
            }
            if (strToRemove != string.Empty)
                mSelfList.Remove(strToRemove);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void RegenerateAll()
        {
            foreach (ImpostorTexture it in mSelfList.Values)
                it.Regenerate();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Regenerate()
        {
            Debug.Assert(mTexture != null);
            string texName = mTexture.Name;
            mTexture = null;
            if (TextureManager.Instance != null)
                TextureManager.Instance.Remove(texName);

            RenderTextures(true);
            UpdateMaterials();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        protected void RenderTextures(bool force)
        {
            Texture renderTexture = null;
            RenderTexture renderTarget = null;
            Camera renderCamera = null;
            Viewport renderViewport = null;
            //Set up RTT texture
            int textureSize = ImpostorPage.ImpostorResolution;
            if (renderTexture == null)
            {
                renderTexture = (Texture)TextureManager.Instance.CreateManual(GetUniqueID("ImpostorTexture"), "Impostors",
                     TextureType.TwoD, textureSize * ImpostorYawAngles, textureSize * ImpostorPitchAngles, 0,
                      Axiom.Media.PixelFormat.A8B8G8R8, TextureUsage.RenderTarget, mLoader);
            }

            renderTexture.MipmapCount = 0x7FFFFFFF;

            //Set up render target
            renderTarget = renderTexture.GetBuffer().GetRenderTarget();
            renderTarget.IsAutoUpdated = false;

            //Set up camera
            renderCamera = mSceneMgr.CreateCamera(GetUniqueID("ImpostorCam"));
            renderCamera.LodBias = 1000.0f;
            renderViewport = renderTarget.AddViewport(renderCamera);
            renderViewport.ShowOverlays = false;
#warning why is set == protected?
           // renderViewport.ClearEveryFrame = true;
            renderViewport.ShowShadows = false;
            renderViewport.BackgroundColor = ImpostorPage.ImpostorBackgroundColor;

            //Set up scene node
            SceneNode node = mSceneMgr.GetSceneNode("ImpostorPage.RenderNode");

            SceneNode oldSceneNode = mEntity.ParentSceneNode;
            if (oldSceneNode != null)
                oldSceneNode.DetachObject(mEntity);

            node.AttachObject(mEntity);
            node.Position = -mEntityCenter;

            //Set up camera FOV
            float objectDist = mEntityRadius * 100;
            float nearDist = objectDist - (mEntityRadius + 1);
            float farDist = objectDist + (mEntityRadius + 1);

            renderCamera.AspectRatio = 1.0f;
            renderCamera.FieldOfView = (float)Axiom.Math.Utility.ATan(mEntityDiameter / objectDist);
            renderCamera.Near = nearDist;
            renderCamera.Far = farDist;

            //Disable mipmapping (without this, masked textures look bad)
            MaterialManager mm = MaterialManager.Instance;
            FilterOptions oldMinFilter = mm.GetDefaultTextureFiltering(FilterType.Min);
            FilterOptions oldMagFilter = mm.GetDefaultTextureFiltering(FilterType.Mag);
            FilterOptions oldMipFilter = mm.GetDefaultTextureFiltering(FilterType.Mip);
            mm.SetDefaultTextureFiltering(FilterOptions.Point, FilterOptions.Linear, FilterOptions.None);

            //Disable fog
            FogMode oldFogMode = mSceneMgr.FogMode;
            ColorEx oldFogColor = mSceneMgr.FogColor;
            float oldFogDensity = mSceneMgr.FogDensity;
            float oldFogStart = mSceneMgr.FogStart;
            float oldFogEnd = mSceneMgr.FogEnd;
            mSceneMgr.FogMode = FogMode.None;

            // Get current status of the queue mode
            SpecialCaseRenderQueueMode oldSpecialCaseRenderQueueMode = mSceneMgr.SpecialCaseRenderQueueList.RenderQueueMode;

            //Only render the entity
            mSceneMgr.SpecialCaseRenderQueueList.RenderQueueMode = SpecialCaseRenderQueueMode.Include;
            mSceneMgr.SpecialCaseRenderQueueList.AddRenderQueue(RenderQueueGroupID.Six + 1);

            RenderQueueGroupID oldRenderGroup = mEntity.RenderQueueGroup;
            mEntity.RenderQueueGroup = RenderQueueGroupID.Six + 1;
            bool oldVisible = mEntity.IsVisible;
            mEntity.IsVisible = true;
#warning implement float oldMaxDistance = entity->getRenderingDistance();
#warning implement entity->setRenderingDistance(0);

            bool needsRegen = true;
            //Calculate the filename used to uniquely identity this render
            string strKey = mEntityKey;
            char[] key = new char[32];
            int i = 0;
            foreach (char c in mEntityKey)
            {
                key[i] ^= c;
                i = (i + 1) % key.Length;
            }
            for (i = 0; i < key.Length; i++)
                key[i] = (char)((key[i] % 26) + 'A');

            ResourceGroupManager.Instance.AddResourceLocation(".", "Folder", "BinFolder");
            string keyStr = string.Empty;
            foreach(char c in key)
                keyStr += c.ToString();
            string fileName = "Impostor." + keyStr + "." + textureSize + ".png";
            //Attempt to load the pre-render file if allowed
            needsRegen = force;
            if (!needsRegen)
            {
                try
                {
                    mTexture = (Texture)TextureManager.Instance.Load(fileName, "BinFolder", TextureType.TwoD, 0x7FFFFFFF);
                }
                catch
                {
                    needsRegen = true;
                }
            }

            if (needsRegen)
            {
                //If this has not been pre-rendered, do so now
                float xDivFactor = 1.0f / ImpostorYawAngles;
                float yDivFactor = 1.0f / ImpostorPitchAngles;
                for (int o = 0; o < ImpostorPitchAngles; o++)//4 pitch angle renders
                {
                    Radian pitch = (Radian)new Degree((Real)((90.0f * o) * yDivFactor));//0, 22.5, 45, 67.5
                    for (i = 0; i < ImpostorYawAngles; i++) //8 yaw angle renders
                    {
                        Radian yaw = (Radian)new Degree((Real)((360.0f * i) * xDivFactor));//0, 45, 90, 135, 180, 225, 270, 315

                        //Position camera
                        renderCamera.Position = new Vector3(0, 0, 0);
                        renderCamera.Orientation = Quaternion.Identity;
                        renderCamera.Pitch((float)-pitch);
                        renderCamera.Yaw((float)yaw);
                        renderCamera.MoveRelative(new Vector3(0, 0, objectDist));

                        //Render the impostor
                        renderViewport.SetDimensions((float)(i * xDivFactor), (float)(o * yDivFactor), xDivFactor, yDivFactor);
                        renderTarget.Update();
                    }
                }

                //Save RTT to file
                renderTarget.WriteContentsToFile(fileName);

                //Load the render into the appropriate texture view
                mTexture = (Texture)TextureManager.Instance.Load(fileName, "BinFolder", TextureType.TwoD, 0x7FFFFFFF);
            }

            mEntity.IsVisible = oldVisible;
            mEntity.RenderQueueGroup = oldRenderGroup;
#warning entity->setRenderingDistance(oldMaxDistance);
            mSceneMgr.SpecialCaseRenderQueueList.RemoveRenderQueue(RenderQueueGroupID.Six + 1);
            // Restore original state
            mSceneMgr.SpecialCaseRenderQueueList.RenderQueueMode = oldSpecialCaseRenderQueueMode;

            //Re-enable mipmapping
            mm.SetDefaultTextureFiltering(oldMinFilter, oldMagFilter, oldMipFilter);

            //Re-enable fog
            mSceneMgr.SetFog(oldFogMode, oldFogColor, oldFogDensity, oldFogStart, oldFogEnd);

            //Delete camera
            renderTarget.RemoveViewport(0);
            renderCamera.SceneManager.DestroyCamera(renderCamera);

            //Delete scene node
            node.DetachAllObjects();
            if (oldSceneNode != null)
                oldSceneNode.AttachObject(mEntity);


            //Delete RTT texture
            Debug.Assert(renderTexture != null);
            string texName2 = renderTexture.Name;

            renderTexture = null;
            if (TextureManager.Instance != null)
                TextureManager.Instance.Remove(texName2);
        }
        /// <summary>
        /// 
        /// </summary>
        protected void UpdateMaterials()
        {
            for (int o = 0; o < ImpostorYawAngles; o++)
            {
                for (int i = 0; i < ImpostorPitchAngles; i++)
                {
                    Material m = mMaterial[i, o];
                    Pass p = m.GetTechnique(0).GetPass(0);
                    TextureUnitState t = p.GetTextureUnitState(0);

                    t.SetTextureName(mTexture.Name);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected string RemoveInvalidCharacters(string s)
        {
            string s2 = string.Empty;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '/' || c == '\\' || c == ':' || c == '*' || c == '?' || c == '\"' || c == '<' || c == '>' || c == '|')
                {
                    s2 += "-";
                }
                else
                {
                    s2 += c.ToString();
                }
            }

            return s2;
        }
        protected static string GetUniqueID(string prefix)
        {
            return prefix + ++GUID;
        }
    }
}
