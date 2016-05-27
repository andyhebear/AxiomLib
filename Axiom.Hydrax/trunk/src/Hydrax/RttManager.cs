#region - License -

#region LGPL License
/*
 This file is part of the Axiom 3D Hydrax port.

 Originial project developer:
 Xavier Verguín González <xavierverguin@hotmail.com>
                         <xavyiy@gmail.com>
 (see http://www.ogre3d.org/addonforums/viewforum.php?f=20&sid=e58afbbf10919de484bf7c4f590f7d17 )
 
 Axiom Hydrax developer:
 Bostich

The overall design, and a majority of the core engine and rendering code 
contained within this library is a derivative of the open source Object Oriented 
Graphics Engine OGRE, which can be found at http://ogre.sourceforge.net.  
Many thanks to the OGRE team for maintaining such a high quality project.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/
#endregion

#endregion

#region - using -
using System;
using System.Drawing;
using System.Collections.Generic;
using Axiom.Core;
using Axiom.Core.Collections; //For MovableCollection
using Axiom.Media;
using Axiom.Math;
using Axiom.Graphics;
using Axiom.Collections;
using Axiom.Controllers;
#endregion

#region - namespace -
namespace Axiom.Hydrax
{
    /// <summary>
    /// Rtt's manager class
    /// </summary>
    public class RttManager
    {
        #region - Constants -
        public const string  _def_Hydrax_Reflection_Rtt_Name        = "HydraxReflectionMap";
        public const string  _def_Hydrax_Refraction_Rtt_Name        = "HydraxRefractionMap";
        public const string  _def_Hydrax_Depth_Rtt_Name             = "HydraxDepthMap";
        public const string  _def_Hydrax_Depth_Reflection_Rtt_Name  = "HydraxDepthReflectionMap";
        public const string  _def_Hydrax_API_Rtt_Name               = "HydraxAPIMap";
        public const string  _def_Hydrax_GPU_Normal_Map_Rtt_Name    = "HydraxNormalMap";
        #endregion

        #region - Enums -
        /// <summary>
        /// Rtt enumeration
        /// </summary>
        [Flags]
        public enum RttType
        {
            RTT_REFLECTION = 0,
            RTT_REFRACTION = 1,
            RTT_DEPTH = 2,
            RTT_DEPTH_REFLECTION = 3,
            RTT_DEPTH_AIP = 4,
            RTT_GPU_NORMAL_MAP = 5
        };

        /// <summary>
        /// Bits per channel
        /// </summary>
        public enum BitsPerChannel
        {
            BPC_8 = 8,
            BPC_16 = 16,
            BPC_32 = 32
        };

        /// <summary>
        /// Number of channels
        /// </summary>
        public enum NumberOfChannels
        {
            NOC_1 = 1,
            NOC_2 = 2,
            NOC_3 = 3,
            NOC_4 = 4
        };
        #endregion

        #region - RttOptions -
        /// <summary>
        /// Rtt options struct
        /// </summary>
        public struct RttOptions
        {
            /// <summary>
            /// Texture names
            /// </summary>
            public string Name;
            /// <summary>
            /// Size; Size(0,0) to get main viewport size
            /// </summary>
            public Size Size_;
            /// <summary>
            /// Number of channels
            /// </summary>
            public NumberOfChannels NumberOfChannels_;
            /// <summary>
            /// Bits per channel
            /// </summary>
            public BitsPerChannel BitsPerChannel_;
        };
        #endregion

        #region - Delegate,Events -
        public delegate void WaterRenderTargetHandle(RttType type);
		
		public event WaterRenderTargetHandle PreRenderTargetUpdate;
		public event WaterRenderTargetHandle PostRenderTargetUpdate;
		
        #endregion
        
        #region - Fields -
        protected float mReflectionDisplacementError;
        protected Hydrax mHydrax;
        protected SceneNode mPlaneSceneNode;
        protected MovablePlane[] mPlanes = new MovablePlane[6];
        protected Texture[] mTextures = new Texture[6];
        protected RttOptions[] mRttOptions = new RttOptions[6];
        protected List<RenderQueueGroupID> mDisableReflectionCustomNearCliplPlaneRenderQueues = new List<RenderQueueGroupID>();
#if false
        //not used anymore!
        internal CDepthListener mDepthListener;
        internal CReflectionListener mReflectionListener;
        internal CRefractionListener mRefractionListener;
        internal CGPUNormalMapListener mGPUNormalMapListener;
        internal CDepthReflectionListener mDepthReflectionListener;
#endif
        #endregion

        #region - Properties -
        /// <summary>
        /// 
        /// </summary>
        public SceneNode PlaneSceneNode
        {
            set { mPlaneSceneNode = value; }
            get { return mPlaneSceneNode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<RenderQueueGroupID> DisableReflectionCustomNearCliplPlaneRenderQueues
        {
            set { mDisableReflectionCustomNearCliplPlaneRenderQueues = value; }
            get { return mDisableReflectionCustomNearCliplPlaneRenderQueues; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Texture[] Textures
        {
            set { mTextures = value; }
            get { return mTextures; }
        }
        /// <summary>
        /// Set reflection displacement error
        /// ReflectionDisplacementError Range [0.05, ~2], increase if you experiment
        /// reflection issues when the camera is near to the water.
        /// </summary>
        public float ReflectionDisplacementError
        {
            set { mReflectionDisplacementError = value; }
            get { return mReflectionDisplacementError; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Hydrax Hydrax
        {
            set { mHydrax = value; }
            get { return mHydrax; }
        }
        /// <summary>
        /// 
        /// </summary>
        public MovablePlane[] Planes
        {
            set { mPlanes = value; }
            get { return mPlanes; }
        }
        #endregion

        #region - Constructor, Destructor -
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Hydrax">Hydrax parent object</param>
        public RttManager(Hydrax Hydrax)
        {
            mHydrax = Hydrax;
            string[] RttNames = new string[6];
            RttNames[0] = _def_Hydrax_Reflection_Rtt_Name;
            RttNames[1] = _def_Hydrax_Refraction_Rtt_Name;
            RttNames[2] = _def_Hydrax_Depth_Rtt_Name;
            RttNames[3] = _def_Hydrax_Depth_Reflection_Rtt_Name;
            RttNames[4] = _def_Hydrax_API_Rtt_Name;
            RttNames[5] = _def_Hydrax_GPU_Normal_Map_Rtt_Name;

            for (int k = 0; k < 6; k++)
            {
                mPlanes[k] = new MovablePlane(RttNames[k]);
                mRttOptions[k] = new RttOptions();
                mRttOptions[k].Name = RttNames[k];
                mRttOptions[k].Size_ = new Size();
                mRttOptions[k].NumberOfChannels_ = NumberOfChannels.NOC_3;
                mRttOptions[k].BitsPerChannel_ = BitsPerChannel.BPC_8;
            }
            mReflectionDisplacementError = 0.5f;
            mDisableReflectionCustomNearCliplPlaneRenderQueues.Add(RenderQueueGroupID.SkiesEarly | RenderQueueGroupID.SkiesLate);
            mDisableReflectionCustomNearCliplPlaneRenderQueues.Add(RenderQueueGroupID.SkiesEarly);
            mDisableReflectionCustomNearCliplPlaneRenderQueues.Add(RenderQueueGroupID.SkiesEarly);
            SetTextureSize(RttType.RTT_DEPTH, new Size());
            SetTextureSize(RttType.RTT_REFLECTION, new Size());
            SetTextureSize(RttType.RTT_REFRACTION, new Size());
            SetTextureSize(RttType.RTT_GPU_NORMAL_MAP, new Size());
            SetTextureSize(RttType.RTT_DEPTH_REFLECTION, new Size());
        }
        ~RttManager()
        {
            RemoveAll();
        }
        #endregion

        #region - Methods -
        #region - Initialize -
        /// <summary>
        /// Initialize a RTT
        /// </summary>
        /// <param name="Rtt">Rtt to initialize</param>
        /// <remarks>If the RTT is already created, it will be recreated.</remarks>
        public void Initialize(RttType Rtt)
        {
            if (mPlaneSceneNode == null)
            {
                mPlaneSceneNode = mHydrax.SceneManager.RootSceneNode.CreateChildSceneNode();
                mPlaneSceneNode.Position = new Vector3(0, mHydrax.Position.y, 0);
            }
            Vector3 wc = new Vector3();
            bool RenderSkyBox = false;
            switch (Rtt)
            {
                case RttType.RTT_REFLECTION:
                    wc = mHydrax.WaterColor;
                    RenderSkyBox = !mHydrax.IsCurrentFrameUnderwater;
                    InititializeRtt(Rtt, Vector3.UnitY, new ColorEx(wc.x, wc.y, wc.z), RenderSkyBox, null);
                    break;
                case RttType.RTT_REFRACTION:
                    wc = mHydrax.WaterColor;
                    RenderSkyBox = !mHydrax.IsCurrentFrameUnderwater;
                    InititializeRtt(Rtt, Vector3.NegativeUnitY, new ColorEx(wc.x, wc.y, wc.z), RenderSkyBox, null);
                    break;
                case RttType.RTT_DEPTH:
                    InititializeRtt(Rtt, Vector3.NegativeUnitY, ColorEx.Black, false, "HydraxDepth", null);
                    break;
                case RttType.RTT_DEPTH_REFLECTION:
                    InititializeRtt(Rtt, Vector3.NegativeUnitY, ColorEx.Black, false, "HydraxDepth", null);
                    break;
                case RttType.RTT_GPU_NORMAL_MAP:
                    InititializeRtt(Rtt, Vector3.NegativeUnitY, new ColorEx(0.5f, 1, 0.5f), false, "", false, null);
                    break;
            }
        }
        #endregion

        #region - Remove -
        /// <summary>
        /// Removes a RTT
        /// </summary>
        /// <param name="Rtt">Rtt to remove</param>
        public void Remove(RttType Rtt)
        {
            try
            {
                if (Axiom.Core.TextureManager.Instance == null)
                    return;
                Texture Tex = mTextures[(int)Rtt];
                if (Tex != null)
                {
                    Axiom.Core.TextureManager.Instance.Remove(mRttOptions[(int)Rtt].Name);
                    MeshManager.Instance.Remove(mRttOptions[(int)Rtt].Name + "ClipPlane");
                    RenderTarget rt = Tex.GetBuffer().GetRenderTarget();
                    rt.Dispose();
                    Tex.Dispose();
                    mPlaneSceneNode.DetachObject(mPlanes[(int)Rtt]);
                    mPlanes[(int)Rtt] = new MovablePlane(mPlanes[(int)Rtt].Name);
                }

                // Check it to avoid any possible problem(texture initializated by createTextureUnit(Name..))
                if (Axiom.Core.TextureManager.Instance.ResourceExists(mRttOptions[(int)Rtt].Name))
                {
                    Axiom.Core.TextureManager.Instance.Remove(mRttOptions[(int)Rtt].Name);
                }
            }
            catch { };
        }
        #endregion

        #region - RemoveAll -
        /// <summary>
        ///  Remove all RttManager resources
        /// </summary>
        /// <remarks>After calling RemoveAll(), calling Initialize(...) is allowed.</remarks>
        public void RemoveAll()
        {
            for (int k = 0; k < 6; k++)
            {
                Remove((RttType)k);
            }

            if (mPlaneSceneNode != null)
            {/*
                mPlaneSceneNode.DetachAllObjects();
                mPlaneSceneNode.Parent.RemoveChild(mPlaneSceneNode.Name);
                mPlaneSceneNode = null;*/
#warning check RTTManager RemoveAll()
            }
        }
        #endregion

        #region - SetTextureSize -
        /// <summary>
        /// Set Rtt textures size
        /// </summary>
        /// <param name="Rtt">Rtt to change</param>
        /// <param name="Size">New texture size (0,0 -> get main viewport size)</param>
        public void SetTextureSize(RttType Rtt, Size Size)
        {
            mRttOptions[(int)Rtt].Size_ = Size;
            if (mTextures[(int)Rtt] != null)
            {
                Initialize(Rtt);

                mHydrax.MaterialManager.Reload(MaterialManager.MaterialType.MAT_WATER);
                if (mHydrax.IsComponent(HydraxComponent.Underwater))
                {
                    mHydrax.MaterialManager.Reload(MaterialManager.MaterialType.MAT_UNDERWATER);
                    if (mHydrax.IsCurrentFrameUnderwater)
                    {
                        mHydrax.MaterialManager.Reload(MaterialManager.MaterialType.MAT_UNDERWATER_COMPOSITOR);
                    }
                }
            }
        }
        /// <summary>
        /// Set Rtt textures size
        /// </summary>
        /// <param name="Size">New texture size (0,0 -> get main viewport size)</param>
        public void SetTextureSize(Size Size)
        {
            bool ReloadMaterialsNeeded = false;

            for (int k = 0; k < 5; k++)
            {
                RttType mType = (RttType)k;
                mRttOptions[(int)mType].Size_ = Size;

                if (mTextures[(int)mType] != null)
                {
                    Initialize(mType);
                    ReloadMaterialsNeeded = true;
                }
            }

            if (ReloadMaterialsNeeded)
            {
                mHydrax.MaterialManager.Reload(MaterialManager.MaterialType.MAT_WATER);
                if (mHydrax.IsComponent(HydraxComponent.Underwater))
                {
                    mHydrax.MaterialManager.Reload(MaterialManager.MaterialType.MAT_UNDERWATER);

                    if (mHydrax.IsCurrentFrameUnderwater)
                    {
                        mHydrax.MaterialManager.Reload(MaterialManager.MaterialType.MAT_UNDERWATER_COMPOSITOR);
                    }
                }
            }
        }
        #endregion

        #region - GetPixelFormat -
        /// <summary>
        /// Get pixel format
        /// </summary>
        /// <param name="Rtt">Rtt type</param>
        /// <returns>pixel format</returns>
        public PixelFormat GetPixelFormat(RttType Rtt)
        {
            switch (mRttOptions[(int)Rtt].NumberOfChannels_)
            {
                case NumberOfChannels.NOC_1:
                    {
                        switch (mRttOptions[(int)Rtt].BitsPerChannel_)
                        {
                            case BitsPerChannel.BPC_8:
                                return PixelFormat.L8;
                            case BitsPerChannel.BPC_16:
                                return PixelFormat.FLOAT16_R;
                            case BitsPerChannel.BPC_32:
                                return PixelFormat.FLOAT32_R;
                        }
                    }
                    break;
                case NumberOfChannels.NOC_2:
                    {
                        switch (mRttOptions[(int)Rtt].BitsPerChannel_)
                        {
                            case BitsPerChannel.BPC_8:
                                break;
                            case BitsPerChannel.BPC_16:
                                return PixelFormat.FLOAT16_GR;
                            case BitsPerChannel.BPC_32:
                                return PixelFormat.FLOAT32_GR;
                        }
                    }
                    break;
                case NumberOfChannels.NOC_3:
                    {
                        switch (mRttOptions[(int)Rtt].BitsPerChannel_)
                        {
                            case BitsPerChannel.BPC_8:
                                return PixelFormat.B8G8R8;
                            case BitsPerChannel.BPC_16:
                                return PixelFormat.FLOAT16_RGB;
                            case BitsPerChannel.BPC_32:
                                return PixelFormat.FLOAT32_RGB;
                        }
                    }
                    break;
                case NumberOfChannels.NOC_4:
                    {
                        switch (mRttOptions[(int)Rtt].BitsPerChannel_)
                        {
                            case BitsPerChannel.BPC_8:
                                return PixelFormat.B8G8R8A8;
                            case BitsPerChannel.BPC_16:
                                return PixelFormat.FLOAT16_RGBA;
                            case BitsPerChannel.BPC_32:
                                return PixelFormat.FLOAT32_RGBA;
                        }
                    }
                    break;
            }
            return PixelFormat.FLOAT32_RGBA;
        }
        #endregion

        #region - SetNumberOfChannels -
        /// <summary>
        /// Set number of channels
        /// </summary>
        /// <param name="Rtt">Rtt type</param>
        /// <param name="NOC">Number of channels</param>
        public void SetNumberOfChannels(RttType Rtt, NumberOfChannels NOC)
        {
            mRttOptions[(int)Rtt].NumberOfChannels_ = NOC;

            if (mTextures[(int)Rtt] == null)
            {
                Initialize(Rtt);
            }
        }
        #endregion

        #region - GetNumberOfChannels -
        /// <summary>
        ///  Get number of channels
        /// </summary>
        /// <param name="Rtt">Rtt type</param>
        /// <returns>Number of channels</returns>
        public NumberOfChannels GetNumberOfChannels(RttType Rtt)
        {
            return mRttOptions[(int)Rtt].NumberOfChannels_;
        }
        #endregion

        #region - SetBitsPerChannel -
        /// <summary>
        /// Set bits per channel
        /// </summary>
        /// <param name="Rtt">Rtt type</param>
        /// <param name="BPC"> Bits per channel</param>
        public void SetBitsPerChannel(RttType Rtt, BitsPerChannel BPC)
        {
            mRttOptions[(int)Rtt].BitsPerChannel_ = BPC;

            if (mTextures[(int)Rtt] == null)
            {
                Initialize(Rtt);
            }
        }
        #endregion

        #region - GetBitsPerChannel -
        /// <summary>
        /// Get bits per channels
        /// </summary>
        /// <param name="Rtt">Rtt type</param>
        /// <returns>Bits per channel</returns>
        public BitsPerChannel GetBitsPerChannel(RttType Rtt)
        {
            return mRttOptions[(int)Rtt].BitsPerChannel_;
        }
        #endregion

        #region - GetRttOptions -
        /// <summary>
        /// Get Rtt options
        /// </summary>
        /// <param name="Rtt">Rtt type</param>
        /// <returns>RttOptions</returns>
        public RttOptions GetRttOptions(RttType Rtt)
        {
            return mRttOptions[(int)Rtt];
        }
        #endregion
        bool first = true;
        #region - InititializeRtt -
        private void InititializeRtt(RttType Rtt, Vector3 PlaneNormal,
            ColorEx BackGroundColor, bool RenderSky, string MaterialScheme,
            bool ShadowsEnabled, RenderTarget ReflectionListener)
        {
            if (mRttOptions[(int)Rtt].Name == "HydraxReflectionMap")
            {
                if (first)
                    first = false;
                else
                    return;
            }
            Remove(Rtt);
            
            mPlanes[(int)Rtt] = new MovablePlane(mRttOptions[(int)Rtt].Name + "Plane");            
            mPlanes[(int)Rtt].D = 0;
            mPlanes[(int)Rtt].Normal = PlaneNormal;

            MeshManager.Instance.CreatePlane(
                mRttOptions[(int)Rtt].Name + "ClipPlane",
                "HYDRAX_RESOURCE_GROUP",
                mPlanes[(int)Rtt].Plane,
                mHydrax.Mesh.MeshOptions.MeshSize.Width,
                mHydrax.Mesh.MeshOptions.MeshSize.Height,
                10,
                10,
                true,
                1,
                5,
                5,
                Vector3.UnitZ);

            mPlanes[(int)Rtt].CastShadows = false;
            mPlaneSceneNode.AttachObject(mPlanes[(int)Rtt]);

            Size TSize = mRttOptions[(int)Rtt].Size_;
            if (TSize.Width == 0 && TSize.Height == 0)
            {
                TSize.Width = mHydrax.Viewport.ActualWidth;
                TSize.Height = mHydrax.Viewport.ActualHeight;
            }

            mTextures[(int)Rtt] = Axiom.Core.TextureManager.Instance.CreateManual(
                mRttOptions[(int)Rtt].Name,
                "HYDRAX_RESOURCE_GROUP",
                TextureType.TwoD,
                TSize.Width,
                TSize.Height,
                0,
                GetPixelFormat(Rtt),
                TextureUsage.RenderTarget);

            RenderTexture RT_Texture = mTextures[(int)Rtt].GetBuffer().GetRenderTarget();
            Viewport RT_Texture_Viewport = RT_Texture.AddViewport(mHydrax.Camera);

            RT_Texture_Viewport.ClearEveryFrame = true;
            RT_Texture_Viewport.BackgroundColor = BackGroundColor;
            RT_Texture_Viewport.ShowOverlays = false;
            RT_Texture_Viewport.ShowShadows = ShadowsEnabled;
            if (MaterialScheme != "")
            {
                RT_Texture_Viewport.MaterialScheme = MaterialScheme;
            }
            if (MaterialScheme == null)
            {
            }
            RT_Texture_Viewport.ShowSkies = RenderSky;
            switch (Rtt)
            {
                case RttType.RTT_REFLECTION:
					RT_Texture.BeforeUpdate += new RenderTargetEventHandler( Reflection_BeforeUpdate );
					RT_Texture.AfterUpdate += new RenderTargetEventHandler( Reflection_AfterUpdate );
					mHydrax.SceneManager.QueueEnded += new EventHandler<SceneManager.EndRenderQueueEventArgs>( SceneManager_QueueEnded );
					mHydrax.SceneManager.QueueStarted += new EventHandler<SceneManager.BeginRenderQueueEventArgs>( SceneManager_QueueStarted );
                    break;
                case RttType.RTT_REFRACTION:
					RT_Texture.BeforeUpdate += new RenderTargetEventHandler( Refraction_BeforeUpdate );
					RT_Texture.AfterUpdate += new RenderTargetEventHandler( Refraction_AfterUpdate );
                    break;
                case RttType.RTT_GPU_NORMAL_MAP:
                    RT_Texture.AfterUpdate += new RenderTargetEventHandler(GpuNormalMap_AfterUpdate);
                    RT_Texture.BeforeUpdate += new RenderTargetEventHandler(GpuNormalMap_BeforeUpdate);
                    break;
                case RttType.RTT_DEPTH_REFLECTION:
					RT_Texture.AfterUpdate += new RenderTargetEventHandler( DepthReflection_AfterUpdate );
					RT_Texture.BeforeUpdate += new RenderTargetEventHandler( DepthReflection_BeforeUpdate );
                    break;
                case RttType.RTT_DEPTH:
					RT_Texture.AfterUpdate += new RenderTargetEventHandler( Depth_AfterUpdate );
					RT_Texture.BeforeUpdate += new RenderTargetEventHandler( Depth_BeforeUpdate );
                    break;
            }

        }
        public void InititializeRtt(RttType Rtt, Vector3 PlaneNormal,
    ColorEx BackGroundColor, bool RenderSky, RenderTarget ReflectionListener)
        {
            InititializeRtt(Rtt, PlaneNormal, BackGroundColor, RenderSky, "", true, ReflectionListener);
        }
        public void InititializeRtt(RttType Rtt, Vector3 PlaneNormal,
    ColorEx BackGroundColor, bool RenderSky, string MaterialScheme, RenderTarget ReflectionListener)
        {
            InititializeRtt(Rtt, PlaneNormal, BackGroundColor, RenderSky, MaterialScheme, true, ReflectionListener);
        }
        #endregion

        #region - IsRenderQueueInList -
        public bool IsRenderQueueInList(List<RenderQueueGroupID> RQG, RenderQueueGroupID RQ)
        {
            foreach (RenderQueueGroupID grp in RQG)
            {
                if (grp == RQ)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region - InvokeRttListeners -
        
        private void InvokeRttListeners(RttType Rtt, bool pre)
		{
			if (pre)
			{
				if(PreRenderTargetUpdate != null)
				{
					PreRenderTargetUpdate(Rtt);
				}
			}
			else
			{
				if(PostRenderTargetUpdate != null)
				{
					PostRenderTargetUpdate(Rtt);
				}
			}
		}
        
        #endregion
        
        #endregion

        #region - Events -
        bool mQueueIsActive = false;
		void SceneManager_QueueStarted( object sender, SceneManager.BeginRenderQueueEventArgs e )
        {
            if (IsRenderQueueInList(DisableReflectionCustomNearCliplPlaneRenderQueues,
                             e.RenderQueueId) && mQueueIsActive)
            {
                Hydrax.Camera.DisableCustomNearClipPlane();

                Root.Instance.RenderSystem.ProjectionMatrix =
                    Hydrax.Camera.ProjectionMatrixRS;
            }
            e.SkipInvocation = false;
        }
		void SceneManager_QueueEnded( object sender, SceneManager.EndRenderQueueEventArgs e )
        {
            if (IsRenderQueueInList(DisableReflectionCustomNearCliplPlaneRenderQueues,
                e.RenderQueueId) && mQueueIsActive)
            {
                Hydrax.Camera.EnableCustomNearClipPlane(
                    Planes[(int)RttManager.RttType.RTT_REFLECTION]);

                Root.Instance.RenderSystem.ProjectionMatrix =
                    Hydrax.Camera.ProjectionMatrixRS;
            }
            e.RepeatInvocation = false;
        }

        protected List<string> mMaterials = new List<string>();
        void Depth_BeforeUpdate(RenderTargetEventArgs e)
        {
            mMaterials.Clear();
            MovableObjectCollection Entitys;
            mHydrax.SceneManager.MovableObjectCollectionMap.TryGetValue("Entity", out Entitys);
            Material SubEntMat;
            bool DepthTechniquePresent = false;

            foreach (Entity CurrentEntity in Entitys)
            {
                for (int k = 0; k < CurrentEntity.SubEntityCount; k++)
                {
#warning check me
                    SubEntMat = (Material)Axiom.Graphics.MaterialManager.Instance.GetByName(
                        CurrentEntity.GetSubEntity(k).MaterialName);
                    if (SubEntMat != null)
                    {
                        foreach (Technique TechIt in SubEntMat.SupportedTechniques)
                        {
                            if (TechIt.SchemeName == "HydraxDepth")
                            {
                                DepthTechniquePresent = true;
                            }
                        }
                    }


                    if (DepthTechniquePresent)
                    {
                        mMaterials.Add("_HydraxDepth_Technique_Present_");
                        DepthTechniquePresent = false;
                        continue;
                    }

                    mMaterials.Add(CurrentEntity.GetSubEntity(k).MaterialName);
                    CurrentEntity.GetSubEntity(k).MaterialName = mHydrax.MaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_DEPTH].Name;
                    
                }//end for

            }//end foreach

            if (System.Math.Abs(mHydrax.Position.y - mHydrax.Camera.DerivedPosition.y) > mHydrax.PlanesError)
            {
                if (mHydrax.IsCurrentFrameUnderwater)
                {
                    Planes[(int)RttManager.RttType.RTT_DEPTH].Normal =
                        -Planes[(int)RttManager.RttType.RTT_DEPTH].Normal;
                    Planes[(int)RttManager.RttType.RTT_DEPTH].ParentNode.Translate(
                        new Vector3(0, -mHydrax.PlanesError, 0));
                }
                else
                {
                    Planes[(int)RttManager.RttType.RTT_DEPTH].ParentNode.Translate(
                        new Vector3(0, mHydrax.PlanesError, 0));

                    mHydrax.Camera.EnableCustomNearClipPlane(Planes[(int)RttManager.RttType.RTT_DEPTH]);
                }
            }

            if (mHydrax.IsCurrentFrameUnderwater)
            {
            	
                mHydrax.Mesh.Entity.IsVisible = true;
                mHydrax.Mesh.Entity.MaterialName =
                    mHydrax.MaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_SIMPLE_RED].Name;

                mHydrax.Mesh.Entity.RenderQueueGroup = RenderQueueGroupID.SkiesEarly;
                mHydrax.GodRaysManager.IsVisible = true;
            }
            else
            {
                mHydrax.Mesh.Entity.IsVisible = false;
            }
        }

        void Depth_AfterUpdate(RenderTargetEventArgs e)
        {
        	
            MovableObjectCollection Entitys;
            mHydrax.SceneManager.MovableObjectCollectionMap.TryGetValue("Entity", out Entitys);

            string Mat;
            foreach (Entity CurrentEntity in Entitys)
            {
                for (int k = 0; k < CurrentEntity.SubEntityCount; k++)
                {
                	Mat = mMaterials[0];
                    if (Mat == "_HydraxDepth_Technique_Present_")
                    {
                    	mMaterials.RemoveAt(0);
                        continue;
                    }

                    CurrentEntity.GetSubEntity(k).MaterialName = Mat;
                    
                    mMaterials.RemoveAt(0);
                }//end for
            }//end 

            mHydrax.Mesh.Entity.IsVisible = true;
            mHydrax.GodRaysManager.IsVisible = false;
            mHydrax.Mesh.Entity.RenderQueueGroup = RenderQueueGroupID.One;

            if (System.Math.Abs(mHydrax.Position.y - mHydrax.Camera.DerivedPosition.y) > mHydrax.PlanesError)
            {
                if (mHydrax.IsCurrentFrameUnderwater)
                {
                    Planes[(int)RttManager.RttType.RTT_DEPTH].Normal =
                        -Planes[(int)RttManager.RttType.RTT_DEPTH].Normal;
                    Planes[(int)RttManager.RttType.RTT_DEPTH].ParentNode.Translate(
                        new Vector3(0, +mHydrax.PlanesError, 0));
                }
                else
                {

                    if (Planes[(int)RttManager.RttType.RTT_DEPTH].ParentNode != null)
                        Planes[(int)RttManager.RttType.RTT_DEPTH].ParentNode.Translate(
                            new Vector3(0, -mHydrax.PlanesError, 0));

                }

                mHydrax.Camera.DisableCustomNearClipPlane();
            }
        }
        
        void DepthReflection_BeforeUpdate(RenderTargetEventArgs e)
        {
        	/*
            mHydrax.Mesh.Entity.IsVisible = false;

            MovableObjectCollection Entitys = null;
            mHydrax.SceneManager.MovableObjectCollectionMap.TryGetValue("Entity", out Entitys);
            Material SubEntMat;
            bool DepthTechniquePresent = false;
            mMaterials.Clear();
            foreach (Entity CurrentEntity in Entitys)
            {
                for (int K = 0; K < CurrentEntity.SubEntityCount; K++)
                {
                    SubEntMat = (Material)Axiom.Graphics.MaterialManager.Instance.GetByName(
                        CurrentEntity.GetSubEntity(K).MaterialName);
                    if (SubEntMat != null)
                    {

                        foreach (Technique TechIt in SubEntMat.SupportedTechniques)
                        {
                            if (TechIt.SchemeName == "HydraxDepth")
                            {
                                DepthTechniquePresent = true;
                            }
                        }//end foreach

                        if (DepthTechniquePresent)
                        {
                            mMaterials.Enqueue("_HydraxDepth_Technique_Present_");
                            DepthTechniquePresent = false;
                            continue;
                        }

                        mMaterials.Enqueue(CurrentEntity.GetSubEntity(K).MaterialName);
                        CurrentEntity.GetSubEntity(K).MaterialName =
                            mHydrax.MaterialManager.Materials[
                            (int)MaterialManager.MaterialType.MAT_DEPTH].Name;

                        CurrentEntity.GetSubEntity(K).MaterialName =
                             mHydrax.MaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_DEPTH].Name;
                    }//end for
                }
            }//end foreach
            if (Planes[(int)RttManager.RttType.RTT_DEPTH_REFLECTION].ParentNode != null)
                Planes[(int)RttManager.RttType.RTT_DEPTH_REFLECTION].ParentNode.Translate(
                    new Vector3(0, -mHydrax.PlanesError, 0));

            bool IsInUnderwaterError = false;

            if (Planes[(int)RttManager.RttType.RTT_DEPTH_REFLECTION].ParentNode != null)
            {
                if (mHydrax.Camera.DerivedPosition.y >
                    Planes[(int)RttManager.RttType.RTT_DEPTH_REFLECTION].ParentNode.Position.y)
                {
                    mCameraPlaneDiff = 0;
                    IsInUnderwaterError = true;
                }
                else
                {
                    mCameraPlaneDiff = 0;
                }
            }
            mHydrax.Camera.EnableReflection(Planes[(int)RttManager.RttType.RTT_DEPTH_REFLECTION]);

            if (!IsInUnderwaterError)
            {
                mHydrax.Camera.EnableCustomNearClipPlane(Planes[(int)RttManager.RttType.RTT_DEPTH_REFLECTION]);
            }*/
        }

        void DepthReflection_AfterUpdate(RenderTargetEventArgs e)
        {
        	/*
            String Mat;
            MovableObjectCollection Entitys = null;
            mHydrax.SceneManager.MovableObjectCollectionMap.TryGetValue("Entity", out Entitys);
            if (Entitys != null)
            {
                foreach (Entity CurrentEntity in Entitys)
                {
                    for (int k = 0; k < CurrentEntity.SubEntityCount; k++)
                    {
                        Mat = mMaterials.Dequeue();

                        if (Mat == "_HydraxDepth_Technique_Present_")
                        {

                            continue;
                        }

                        CurrentEntity.GetSubEntity(k).MaterialName = Mat;
                    }//end for
                }//end foreach
            }//end if

            mHydrax.Mesh.Entity.IsVisible = true;

            if (mCameraPlaneDiff != 0)
            {
                if (Planes[(int)RttManager.RttType.RTT_DEPTH_REFLECTION].ParentNode != null)
                    Planes[(int)RttManager.RttType.RTT_DEPTH_REFLECTION].ParentNode.Translate(
                        new Vector3(0, mCameraPlaneDiff, 0));
            }
            if (Planes[(int)RttManager.RttType.RTT_DEPTH_REFLECTION].ParentNode != null)
                Planes[(int)RttManager.RttType.RTT_DEPTH_REFLECTION].ParentNode.Translate(
                    new Vector3(0, mHydrax.PlanesError, 0));

            mHydrax.Camera.DisableReflection();
            mHydrax.Camera.DisableCustomNearClipPlane();*/
        }
        string mOldMaterialName;
        void GpuNormalMap_BeforeUpdate(RenderTargetEventArgs e)
        {
            mOldMaterialName = mHydrax.Mesh.MaterialName;
            if (mHydrax.GPUNormalMapManager.IsCreated)
                mHydrax.Mesh.MaterialName = mHydrax.GPUNormalMapManager.NormalMapMaterial.Name;
            RenderQueue que = mHydrax.SceneManager.GetRenderQueue();
            que.DefaultRenderGroup = RenderQueueGroupID.One;
            
            InvokeRttListeners(RttType.RTT_GPU_NORMAL_MAP, true);
        }

        void GpuNormalMap_AfterUpdate(RenderTargetEventArgs e)
        {
            if (mHydrax.GPUNormalMapManager.IsCreated)
                mHydrax.Mesh.MaterialName = mOldMaterialName;

            RenderQueue que = mHydrax.SceneManager.GetRenderQueue();
            que.DefaultRenderGroup = RenderQueueGroupID.Main;
            
            InvokeRttListeners(RttType.RTT_GPU_NORMAL_MAP, false);
        }

        void Refraction_AfterUpdate(RenderTargetEventArgs e)
        {
            mHydrax.Mesh.Entity.IsVisible = true;
            if (System.Math.Abs(mHydrax.Position.y - mHydrax.Camera.DerivedPosition.y) > mHydrax.PlanesError)
            {
                if (mHydrax.IsCurrentFrameUnderwater)
                {

                    Planes[(int)RttManager.RttType.RTT_REFRACTION].Normal =
                        -Planes[(int)RttManager.RttType.RTT_REFRACTION].Normal;

                    Planes[(int)RttManager.RttType.RTT_REFRACTION].ParentNode.Translate(new Vector3(
                        0, +mHydrax.PlanesError, 0));
                }
                else
                {
                    Planes[(int)RttManager.RttType.RTT_REFRACTION].ParentNode.Translate(new Vector3(
                        0, -mHydrax.PlanesError, 0));
                }
            }

            mHydrax.Camera.DisableCustomNearClipPlane();
            
            InvokeRttListeners(RttType.RTT_REFRACTION, false);
        }

        void Refraction_BeforeUpdate(RenderTargetEventArgs e)
        {
            mHydrax.Mesh.Entity.IsVisible = false;
            if (System.Math.Abs(mHydrax.Position.y - mHydrax.Camera.DerivedPosition.y) < mHydrax.PlanesError)
            {
                Planes[(int)RttManager.RttType.RTT_REFRACTION].Normal =
                    -Planes[(int)RttManager.RttType.RTT_REFRACTION].Normal;

                Planes[(int)RttManager.RttType.RTT_REFRACTION].ParentNode.Translate(new Vector3(
                    0, mHydrax.PlanesError, 0));
            }
            else
            {
                Planes[(int)RttManager.RttType.RTT_REFRACTION].ParentNode.Translate(new Vector3(
                    0, mHydrax.PlanesError, 0));
            }

            mHydrax.Camera.EnableCustomNearClipPlane(Planes[(int)RttManager.RttType.RTT_REFRACTION]);
            
            InvokeRttListeners(RttType.RTT_REFRACTION, true);
        }

        float mCameraPlaneDiff = 0;
        void Reflection_AfterUpdate(RenderTargetEventArgs e)
        {
            mHydrax.Mesh.Entity.IsVisible = true;

            if (mCameraPlaneDiff != 0)
            {
                if (Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode != null)
                    Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode.Translate(
                        new Vector3(0, mCameraPlaneDiff, 0));
            }

            if (mHydrax.IsCurrentFrameUnderwater)
            {
                if (Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode != null)
                    Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode.Translate(
                        new Vector3(0, mHydrax.PlanesError, 0));
            }
            else
            {
                if (Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode != null)
                    Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode.Translate(
                        new Vector3(0, -mHydrax.PlanesError, 0));
            }

            mHydrax.Camera.DisableReflection();
            mHydrax.Camera.DisableCustomNearClipPlane();

            if (mHydrax.IsComponent(HydraxComponent.Underwater))
            {
                if (mHydrax.IsCurrentFrameUnderwater)
                {
                    Planes[(int)RttManager.RttType.RTT_REFLECTION].Normal =
                        -Planes[(int)RttManager.RttType.RTT_REFLECTION].Normal;
                }
            }
            mQueueIsActive = false;
            //mCReflectionQueueListener.IsActive = false;
            
            InvokeRttListeners(RttType.RTT_REFLECTION, false);
        }

        void Reflection_BeforeUpdate(RenderTargetEventArgs e)
        {
            mQueueIsActive = true;
            //mCReflectionQueueListener.IsActive = true;
            mHydrax.Mesh.Entity.IsVisible = false;
            if (mHydrax.IsCurrentFrameUnderwater)
            {
                if (Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode != null)
                    Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode.Translate(
                        new Vector3(0, -mHydrax.PlanesError, 0));
            }
            else
            {
                if (Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode != null)
                    Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode.Translate(
                       new Vector3(0, +mHydrax.PlanesError, 0));
            }

            bool IsInUnderwaterError = false;

            if (Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode != null)
            {
                //underwater
                if (mHydrax.IsCurrentFrameUnderwater &&
                   (mHydrax.Camera.DerivedPosition.y >
                    Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode.Position.y))
                {
                    mCameraPlaneDiff = 0;
                    IsInUnderwaterError = true;
                }

            //over water
                else if (!mHydrax.IsCurrentFrameUnderwater &&
                    (mHydrax.Camera.DerivedPosition.y <
                Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode.Position.y))
                {
                    mCameraPlaneDiff = Planes[(int)RttManager.RttType.RTT_REFLECTION].
                        ParentNode.Position.y -
                        mHydrax.Camera.DerivedPosition.y +
                        ReflectionDisplacementError;

                    Planes[(int)RttManager.RttType.RTT_REFLECTION].ParentNode.Translate(
                        new Vector3(0, -mCameraPlaneDiff, 0));
                }
            }
            else
            {
                mCameraPlaneDiff = 0;
            }

            if (mHydrax.IsComponent(HydraxComponent.Underwater))
            {
                if (mHydrax.IsCurrentFrameUnderwater)
                {
                    Planes[(int)RttManager.RttType.RTT_REFLECTION].Normal =
                        -Planes[(int)RttManager.RttType.RTT_REFLECTION].Normal;
                }
            }

            mHydrax.Camera.EnableReflection(Planes[(int)RttManager.RttType.RTT_REFLECTION]);

            if (IsInUnderwaterError)
            {
                mQueueIsActive = false;
                //mCReflectionQueueListener.IsActive = false;
            }
            else
            {
                mHydrax.Camera.EnableCustomNearClipPlane(Planes[(int)RttManager.RttType.RTT_REFLECTION]);
            }
            
            InvokeRttListeners(RttType.RTT_REFLECTION, true);
        }
        #endregion
    }//end RTT manager class

}//end namespace
#endregion