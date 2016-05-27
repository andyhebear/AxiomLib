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
using System.Collections.Generic;
using Axiom;
using Axiom.Core;
using Axiom.Math;
using Axiom.Graphics;
using Axiom.Hydrax.Modules;
#endregion

#region - namespace -
namespace Axiom.Hydrax
{
    #region - class -
    /// <summary>
    /// Main Hydrax Class.
    /// Hydrax is a plugin for the Axiom 3D engine whose aim is rendering realistic water scenes.
    /// Do not use two instances of hydrax class.
    /// </summary>
    public class Hydrax
    {
        #region - statics -
        public static string HYDRAX_VERSION = "0.5.0";
        #endregion
        #region - Constructor, Destructor -
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="SceneManager">Axiom Scenemanger</param>
        /// <param name="Camera">Axiom Camera</param>
        /// <param name="Viewport">Axiom Mainwindow Viewport</param>
        public Hydrax(SceneManager SceneManager, Camera Camera, Viewport Viewport)
        {
            mSceneManager = SceneManager;
            mCamera = Camera;
            mViewport = Viewport;
            mIsVisible = true;
            mPolygonMode = PolygonMode.Solid;
            mShaderMode = MaterialManager.ShaderMode.SM_HLSL;
            mPosition = new Vector3(0, 0, 0);
            mPlanesError = 0;
            mFullReflectionDistance = 99999997952.0f;
            mGlobalTransparency = 0.05f;
            mWaterColor = new Vector3(0, 0.1f, 0.172f);
            mNormalDistortion = 0.09f;
            mSunPosition = new Vector3(5000f, 3000f, 1f);
            mSunStrenght = 1.75f;
            mSunArea = 150;
            mSunColor = new Vector3(1f, 0.75f, 0.25f);
            mFoamMaxDistance = 75000000.0f;
            mFoamScale = 0.0075f;
            mFoamStart = 0;
            mFoamTransparency = 1;
            mDepthLimit = 0;
            mSmoothPower = 30;
            mCausticsScale = 20;
            mCausticsPower = 15;
            mCausticsEnd = 0.55f;
            mGodRaysExposure = new Vector3(0.76f, 2.46f, 2.29f);
            mGodRaysIntensity = 0.015f;
            mUnderWaterCameraSwitchDelta = 1.25f;
            mIsCurrentFrameUnderwater = false;
            mMesh = new Mesh(this);
            mMaterialManager = new MaterialManager(this);
            mRttManager = new RttManager(this);
            mTextureManager = new TextureManager(this);
            mGodRaysManager = new GodRaysManager(this);
            mDecalsManager = new DecalsManager(this);
            mGPUNormalMapManager = new GPUNormalMapManager(this);
            mCfgFileManager = new CfgFileManager(this);
            mComponents = HydraxComponent.None;/*HydraxComponent.Sun |
                HydraxComponent.Foam |
                HydraxComponent.Caustics |
                HydraxComponent.Smooth |
          //     HydraxComponent.Underwater |
                HydraxComponent.UnderwaterGodRays |
                HydraxComponent.UnderwaterReflections |
                HydraxComponent.Depth;*/
            HydraxLog("Hydrax object created.");
        }
        /// <summary>
        /// Destrucor.
        /// </summary>
        ~Hydrax()
        {
            Remove();

            if (mModule != null)
            {
                mModule = null;
            }

            mTextureManager = null;
            mMaterialManager = null;
            mGPUNormalMapManager = null;
            mDecalsManager = null;
            mGodRaysManager = null;
            mRttManager = null;
            mCfgFileManager = null;
            mMesh = null;

            HydraxLog("Hydrax object removed");
        }
        #endregion

        #region - Fields -
        protected bool mIsCreated;
        protected bool mIsVisible;
        protected HydraxComponent mComponents;
        protected MaterialManager.ShaderMode mShaderMode;
        protected PolygonMode mPolygonMode;
        protected Vector3 mPosition;
        protected float mPlanesError;
        protected float mFullReflectionDistance;
        protected float mGlobalTransparency;
        protected Vector3 mWaterColor;
        protected float mNormalDistortion;
        protected Vector3 mSunPosition;
        protected float mSunStrenght;
        protected float mSunArea;
        protected Vector3 mSunColor;
        protected float mFoamMaxDistance;
        protected float mFoamScale;
        protected float mFoamStart;
        protected float mFoamTransparency;
        protected float mDepthLimit;
        protected float mSmoothPower;
        protected float mCausticsScale;
        protected float mCausticsPower;
        protected float mCausticsEnd;
        protected Vector3 mGodRaysExposure;
        protected float mGodRaysIntensity;
        protected float mUnderWaterCameraSwitchDelta;
        protected bool mIsCurrentFrameUnderwater;
        protected MaterialManager mMaterialManager;
        protected RttManager mRttManager;
        protected TextureManager mTextureManager;
        protected GodRaysManager mGodRaysManager;
        protected DecalsManager mDecalsManager;
        protected GPUNormalMapManager mGPUNormalMapManager;
        protected CfgFileManager mCfgFileManager;
        protected Module mModule;
        protected SceneManager mSceneManager;
        protected Camera mCamera;
        protected Viewport mViewport;
        protected Mesh mMesh;
        #endregion

        #region - Properties -

        #region - Strength -
        /// <summary>
        /// Set's water strength GPU param
        /// </summary>
        public float Strength
        {
            set
            {
                if (!mIsCreated)
                    return;

                if (IsComponent(HydraxComponent.Foam))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                         MaterialManager.MaterialType.MAT_WATER,
                         "uFoamRange", value);
                }
                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                         MaterialManager.MaterialType.MAT_UNDERWATER,
                         "uFoamRange", value);
                }

                mDecalsManager.SetWaterStrength(value);
            }

        }
        #endregion

        #region - PolygonMode -
        /// <summary>
        /// Set Polygon Mode (Solid, Wireframe, Points)
        /// </summary>
        public PolygonMode PolygonMode
        {
            set 
            {
                mPolygonMode = value;
                if (!mIsCreated)
                {
                    return;
                }
                
                mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_WATER].GetTechnique(0)
                    .GetPass(0).PolygonMode = mPolygonMode;

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_UNDERWATER].GetTechnique(0)
                        .GetPass(0).PolygonMode = mPolygonMode;
                }
            }
            get { return mPolygonMode; }
        }
        #endregion

        #region - IsVisible -
        /// <summary>
        /// True if water is visible, otherwise false
        /// </summary>
        public bool IsVisible
        {
            set 
            { 
                mIsVisible = value;
                CheckIsVisible();
            }
            get { return mIsVisible; }
        }
        #endregion

        #region  - ShaderMode -
        /// <summary>
        /// Set's or Get's Current shader mode.
        /// </summary>
        public MaterialManager.ShaderMode ShaderMode
        {
            set
            {
                mShaderMode = value;

                if (mIsCreated && mModule != null)
                {
                    mMaterialManager.CreateMaterials(mComponents,
                        new MaterialManager.Options(mShaderMode, mModule.NormalMode));

                    mMesh.MaterialName = mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_WATER].Name;
                }
            }
            get { return mShaderMode; }
        }
        #endregion

        #region - GodRaysExposure -
        /// <summary>
        /// Get's or Set's GodRaysExposure.
        /// </summary>
        public Vector3 GodRaysExposure
        {
            set { mGodRaysExposure = value; }
            get { return mGodRaysExposure; }
        }
        #endregion

        #region  - GodRaysIntensity -
        /// <summary>
        /// Get's or Set's God rays intensity.
        /// </summary>
        public float GodRaysIntensity
        {
            set { mGodRaysIntensity = value; }
            get { return mGodRaysIntensity; }
        }
        #endregion

        #region - DepthLimit -
        /// <summary>
        /// Get's or Set's the Depthlimit.
        /// </summary>
        public float DepthLimit
        {
            set 
            {
                
                if (!IsComponent(HydraxComponent.Depth))
                    return;
                mDepthLimit = value;

                if (mDepthLimit <= 0)
                    mDepthLimit = 1;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                 MaterialManager.MaterialType.MAT_DEPTH,
                 "uDepthLimit",
                 1 / mDepthLimit);
            }
            get { return mDepthLimit; }
        }
        #endregion

        #region - CausticsEnd -
        /// <summary>
        /// Get's or Set's Caustics end.
        /// </summary>
        public float CausticsEnd
        {
            set 
            {
                if (!IsComponent(HydraxComponent.Caustics))
                    return;

                mCausticsEnd = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                   MaterialManager.MaterialType.MAT_DEPTH,
                   "uCausticsEnd",
                   mCausticsEnd);
            }
            get { return mCausticsEnd; }
        }
        #endregion

        #region - CausticsScale -
        /// <summary>
        /// Get's or Set's the Caustics scale.
        /// </summary>
        public float CausticsScale
        {
            set 
            {
                if (!IsComponent(HydraxComponent.Caustics))
                    return;

                mCausticsScale = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                 MaterialManager.MaterialType.MAT_DEPTH,
                 "uCausticsScale",
                 mCausticsScale);
            }
            get { return mCausticsScale; }
        }
        #endregion

        #region - CausticsPower -
        /// <summary>
        /// Get's or Set's the Caustics power.
        /// </summary>
        public float CausticsPower
        {
            set 
            {
                if (!IsComponent(HydraxComponent.Caustics))
                    return;

                mCausticsPower = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                 MaterialManager.MaterialType.MAT_WATER,
                 "uCausticsPower",
                 mCausticsPower);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    if (IsComponent(HydraxComponent.UnderwaterReflections))
                    {
                        mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                          MaterialManager.MaterialType.MAT_UNDERWATER,
                          "uCausticsPower",
                          mCausticsPower);
                    }

                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                      MaterialManager.MaterialType.MAT_UNDERWATER_COMPOSITOR,
                      "uCausticsPower",
                      mCausticsPower);

                }
            }
            get { return mCausticsPower; }
        }
        #endregion

        #region - FoamTransparency -
        /// <summary>
        /// Get's or Set's the Foam transparency.
        /// </summary>
        public float FoamTransparency
        {
            set 
            {
                if (!IsComponent(HydraxComponent.Foam))
                    return;

                mFoamTransparency = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                 MaterialManager.MaterialType.MAT_WATER,
                 "uFoamTransparency",
                 mFoamTransparency);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                      MaterialManager.MaterialType.MAT_UNDERWATER,
                      "uFoamTransparency",
                      mFoamTransparency);
                }
            }
            get { return mFoamTransparency; }
        }
        #endregion

        #region - FoamStart -
        /// <summary>
        /// Get's or Set's the Foam start.
        /// </summary>
        public float FoamStart
        {
            set 
            {
                if (!IsComponent(HydraxComponent.Foam))
                    return;

                mFoamStart = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                 MaterialManager.MaterialType.MAT_WATER,
                 "uFoamStart",
                 mFoamStart);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                      MaterialManager.MaterialType.MAT_UNDERWATER,
                      "uFoamStart",
                      mFoamStart);
                }
            }
            get { return mFoamStart; }
        }
        #endregion

        #region - FoamScale -
        /// <summary>
        /// Get's or Sets the Foam scale.
        /// </summary>
        public float FoamScale
        {
            set 
            {
                if (!IsComponent(HydraxComponent.Foam))
                    return;

                mFoamScale = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                 MaterialManager.MaterialType.MAT_WATER,
                 "uFoamScale",
                 mFoamScale);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                      MaterialManager.MaterialType.MAT_UNDERWATER,
                      "uFoamScale",
                      mFoamScale);
                }
            }
            get { return mFoamScale; }
        }
        #endregion

        #region - FoamMaxDistance -
        /// <summary>
        /// Get's or Set's the Foam max distance.
        /// </summary>
        public float FoamMaxDistance
        {
            set 
            {
                if (!IsComponent(HydraxComponent.Foam))
                    return;

                mFoamMaxDistance = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                 MaterialManager.MaterialType.MAT_WATER,
                 "uFoamMaxDistance",
                 mFoamMaxDistance);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                      MaterialManager.MaterialType.MAT_UNDERWATER,
                      "uFoamMaxDistance",
                      mFoamMaxDistance);
                }
            }
            get { return mFoamMaxDistance; }
        }
        #endregion

        #region - SunColor -
        /// <summary>
        /// Get's or Set's the Sun color.
        /// </summary>
        public Vector3 SunColor
        {
            set 
            {
                if (!IsComponent(HydraxComponent.Sun))
                    return;

                mSunColor = value;


                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                 MaterialManager.MaterialType.MAT_WATER,
                 "uSunColor",
                 mSunColor);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                      MaterialManager.MaterialType.MAT_UNDERWATER,
                      "uSunColor",
                      mSunColor);
                }

            }
            get { return mSunColor; }
        }
        #endregion

        #region - SunArea -
        /// <summary>
        /// Get's or Set's the Sun area.
        /// </summary>
        public float SunArea
        {
            set 
            {
                if (!IsComponent(HydraxComponent.Sun))
                    return;

                mSunArea = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                 MaterialManager.MaterialType.MAT_WATER,
                 "uSunArea",
                 mSunArea);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                      MaterialManager.MaterialType.MAT_UNDERWATER,
                      "uSunArea",
                      mSunArea);
                }
            }
            get { return mSunArea; }
        }
        #endregion

        #region - SunStrength -
        /// <summary>
        /// Get's or Set's the Sun strength.
        /// </summary>
        public float SunStrenght
        {
            set 
            {
                if (!IsComponent(HydraxComponent.Sun))
                    return;

                mSunStrenght = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                 MaterialManager.MaterialType.MAT_WATER,
                 "uSunStrength",
                 mSunStrenght);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                      MaterialManager.MaterialType.MAT_UNDERWATER,
                      "uSunStrength",
                      mSunStrenght);
                }
            }
            get { return mSunStrenght; }
        }
        #endregion

        #region - SunPosition -
        /// <summary>
        /// Get's or sets the Sun position.
        /// </summary>
        public Vector3 SunPosition
        {
            set 
            {
                mSunPosition = value;
                if (!IsComponent(HydraxComponent.Sun))
                    return;

                

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                     MaterialManager.MaterialType.MAT_WATER,
                     "uSunPosition",
                     mMesh.GetObjectSpacePosition(mSunPosition));

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                      MaterialManager.MaterialType.MAT_UNDERWATER,
                      "uSunPosition",
                      mMesh.GetObjectSpacePosition(mSunPosition));
                }

            }
            get { return mSunPosition; }
        }
        #endregion

        #region - SmoothPower -
        /// <summary>
        /// Get's or Set's the Smooth power.
        /// </summary>
        public float SmoothPower
        {
            set 
            {
                if (!IsComponent(HydraxComponent.Smooth))
                    return;

                mSmoothPower = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                 MaterialManager.MaterialType.MAT_WATER,
                 "uSmoothPower",
                 mSmoothPower);
            }
            get { return mSmoothPower; }
        }
        #endregion

        #region - NormalDistortion -
        /// <summary>
        /// Set's or Get's  the NormalDistortion.
        /// </summary>
        public float NormalDistortion
        {
            set 
            { 
                mNormalDistortion = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                     MaterialManager.MaterialType.MAT_WATER,
                     "uNormalDistortion",
                     value);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                     MaterialManager.MaterialType.MAT_UNDERWATER,
                     "uNormalDistortion",
                     value);
                }
            }
            get { return mNormalDistortion; }
        }
        #endregion

        #region - GlobalTransparency -
        /// <summary>
        /// Set's global transparency.
        /// </summary>
        public float GlobalTransparency
        {
            set 
            {

                mGlobalTransparency = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                     MaterialManager.MaterialType.MAT_WATER,
                     "uGlobalTransparency", value);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                         MaterialManager.MaterialType.MAT_UNDERWATER,
                         "uGlobalTransparency", value);
                }

            }
            get { return mGlobalTransparency; }
        }
        #endregion

        #region - FullReflectionDistance -
        /// <summary>
        /// Set's full reflection distance.
        /// </summary>
        public float FullReflectionDistance
        {
            set 
            {

                mFullReflectionDistance = value;

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                     MaterialManager.MaterialType.MAT_WATER,
                     "uFullReflectionDistance", value);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                         MaterialManager.MaterialType.MAT_UNDERWATER,
                         "uFullReflectionDistance", value);
                }

            }
            get { return mFullReflectionDistance; }
        }
        #endregion

        #region - DecalsManager -
        /// <summary>
        /// Get's or Set's the DecalManager.
        /// </summary>
        public DecalsManager DecalsManager
        {
            set { mDecalsManager = value; }
            get { return mDecalsManager; }
        }
        #endregion

        #region - RttManager -
        /// <summary>
        /// Get's the RttManager.
        /// </summary>
        public RttManager RttManager
        {
            set { mRttManager = value; }
            get { return mRttManager; }
        }
        #endregion

        #region - Viewport -
        /// <summary>
        /// Get's or Set's the Viewport.
        /// </summary>
        public Viewport Viewport
        {
            set { mViewport = value; }
            get { return mViewport; }
        }
        #endregion

        #region - WaterColor -
        /// <summary>
        /// Set's or Get's the water color.
        /// </summary>
        public Vector3 WaterColor
        {
            set 
            {
                mWaterColor = value;

                if (!mIsCreated)
                    return;
                ColorEx WC = new ColorEx(mWaterColor.x, mWaterColor.y, mWaterColor.z);

                mRttManager.Textures[(int)RttManager.RttType.RTT_REFLECTION]
                    .GetBuffer().GetRenderTarget().GetViewport(0)
                    .BackgroundColor = WC;

                if (!IsComponent(HydraxComponent.Depth))
                {
                    return;
                }

                mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                     MaterialManager.MaterialType.MAT_WATER,
                     "uWaterColor", mWaterColor);

                if (IsComponent(HydraxComponent.Underwater))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_FRAGMENT,
                         MaterialManager.MaterialType.MAT_UNDERWATER,
                         "uWaterColor", mWaterColor);

                }
            }
            get { return mWaterColor; }
        }
        #endregion

        #region - IsCurrentFrameUnderwater -
        /// <summary>
        /// If true, Camera is currently under water, otherwise false.
        /// </summary>
        public bool IsCurrentFrameUnderwater
        {
            set { mIsCurrentFrameUnderwater = value; }
            get { return mIsCurrentFrameUnderwater; }
        }
        #endregion

        #region - MaterialManager -
        /// <summary>
        /// Get's or Set's the Material manager.
        /// </summary>
        public MaterialManager MaterialManager
        {
            set { mMaterialManager = value; }
            get { return mMaterialManager; }
        }
        #endregion

        #region - PlanesError -
        /// <summary>
        /// Set's or Get's clip planes error
        /// </summary>
        public float PlanesError
        {
            set { mPlanesError = value; }
            get { return mPlanesError; }
        }
        #endregion

        #region - Mesh -
        /// <summary>
        /// Get's or Sets the Mesh.
        /// </summary>
        public Mesh Mesh
        {
            set { mMesh = value; }
            get { return mMesh; }
        }
        #endregion

        #region - Camera -
        /// <summary>
        /// Get's or Set's the Camera.
        /// </summary>
        public Camera Camera
        {
            set { mCamera = value; }
            get { return mCamera; }
        }
        #endregion

        #region - Position -
        /// <summary>
        /// Set's or Get's the water position.
        /// </summary>
        public Vector3 Position
        {
            set 
            { 
                mPosition = value;
                if (!mIsCreated)
                    return;

                if (IsComponent(HydraxComponent.Depth))
                {
                    mMaterialManager.SetGpuProgramParameter(MaterialManager.GpuProgram.GPUP_VERTEX,
                        MaterialManager.MaterialType.MAT_DEPTH,
                        "uPlaneYPos",
                        mPosition.y);
                }

                mMesh.SceneNode.Position = new Vector3(
                    mPosition.x -
                    mMesh.MeshOptions.MeshSize.Width / 2,
                    mPosition.y,
                    mPosition.z - mMesh.MeshOptions.MeshSize.Height / 2);
                mRttManager.PlaneSceneNode.Position = mPosition;

                //for world-space -> objectspace conversion
                SunPosition = mSunPosition;
                    
            }
            get { return mPosition; }
        }
        #endregion

        #region - Module -
        /// <summary>
        /// Get's the currently active Module.
        /// </summary>
        public Module Module
        {
            set { mModule = value; }
            get { return mModule; }
        }
        #endregion

        #region - SceneManeger -
        /// <summary>
        /// Get's the SceneManager.
        /// </summary>
        public SceneManager SceneManager
        {
            set { mSceneManager = value; }
            get { return mSceneManager; }
        }
        #endregion

        #region - GodRaysManager -
        /// <summary>
        /// Get's or Set's the God rays Manager.
        /// </summary>
        public GodRaysManager GodRaysManager
        {
            set { mGodRaysManager = value; }
            get { return mGodRaysManager; }
        }
        #endregion

        #region - GPUNormalMapManger -
        /// <summary>
        /// Get's or Set's the GPUNormalMapManager.
        /// </summary>
        public GPUNormalMapManager GPUNormalMapManager
        {
            set { mGPUNormalMapManager = value; }
            get { return mGPUNormalMapManager; }
        }
        #endregion

        #region - Components -
        /// <summary>
        /// Get's the Current setup components.
        /// </summary>
        public HydraxComponent Components
        {
            get { return mComponents; }
        }
        #endregion
        #endregion

        #region - Methods -

        #region - Create -
        /// <summary>
        /// Create all resources according with current Hydrax components and 
        /// add Hydrax to the Scene.
        /// <remarks>Call when all params set.</remarks>
        /// </summary>
        public void Create()
        {
            if (mModule == null)
            {
                HydraxLog("Module isn't set, skipping...");
                return;
            }

            if (mIsCreated)
            {
                HydraxLog("Hydrax is allready created, skipping...");
                return;
            }

            HydraxLog("Creating module...");
            mModule.Create();

            if (mModule.NormalMode == MaterialManager.NormalMode.NM_RTT)
            {
                if (!mModule.Noise.CreateGPUNormalMapResources(mGPUNormalMapManager))
                {
                    HydraxLog(mModule.Noise.Name + " doesn't support GPU Normal map generation.");
                }
            }
            else
            {
                if (mModule.Noise.IsGPUNormalMapResourcesCreated)
                {
                    mModule.Noise.RemoveGPUNormalMapRecources(mGPUNormalMapManager);
                }
            }
            
            HydraxLog("ModuleCreated.");

            HydraxLog("Initialize RTT Manager...");
            mRttManager.Initialize(RttManager.RttType.RTT_REFLECTION);
            mRttManager.Initialize(RttManager.RttType.RTT_REFRACTION);

            if(IsComponent(HydraxComponent.Depth))
            {
                mRttManager.Initialize(RttManager.RttType.RTT_DEPTH);
            }
            HydraxLog("RTT Manager initialized");

            HydraxLog("Creating materials...");
            mMaterialManager.CreateMaterials(mComponents, new MaterialManager.Options(mShaderMode, mModule.NormalMode));
            mMesh.MaterialName = mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_WATER].Name;
            HydraxLog("Materials created.");

            if(IsComponent(HydraxComponent.UnderwaterGodRays))
            {
                HydraxLog("Creating god rays...");
                mGodRaysManager.Create(mComponents);
                HydraxLog("God rays created.");
            }

            HydraxLog("Creating water mesh...");
            mMesh.MeshOptions = mModule.MeshOptions;
            mMesh.Create();
            HydraxLog("Water mesh created.");

            CheckIsVisible();
            mIsCreated = true;
        }
        #endregion

        #region - Remove -
        /// <summary>
        /// Remove Hydrax, you can call this method to remove Hydrax from the Scene.
        /// or release (secondary) Hydrax memory, call create() to retun Hydrax to the scene.
        /// </summary>
        public void Remove()
        {
            if (!mIsCreated)
            {
                return;
            }

            mMesh.Remove();
            mDecalsManager.Remove();
            mMaterialManager.RemoveMaterials();
            mRttManager.RemoveAll();
            mGodRaysManager.Remove();
            mModule.Remove();

            mIsCreated = false;
        }
        #endregion

        #region - Update -
        /// <summary>
        /// Call every frame.
        /// </summary>
        /// <param name="TimeSinceLastFrame"></param>
        public void Update(float TimeSinceLastFrame)
        {
            if (mIsCreated && mModule != null && mIsVisible)
            {
                mModule.Update(TimeSinceLastFrame);
                mDecalsManager.Update();
                CheckUnderwater(TimeSinceLastFrame);
            }
        }
        #endregion

        #region - IsComponent -
        /// <summary>
        /// Returns if the especified component is active
        /// </summary>
        /// <param name="Components">Component that we want to check</param>
        public bool IsComponent(HydraxComponent Component)
        {
            if ((mComponents & HydraxComponent.Caustics) == Component)
            {
                return true;
            }
            if ((mComponents & HydraxComponent.Depth) == Component)
            {
                return true;
            }
            if ((mComponents & HydraxComponent.Foam) == Component)
            {
                return true;
            }
            if ((mComponents & HydraxComponent.Smooth) == Component)
            {
                return true;
            }
            if ((mComponents & HydraxComponent.Sun) == Component)
            {
                return true;
            }
            if ((mComponents & HydraxComponent.Underwater) == Component)
            {
                return true;
            }
            if ((mComponents & HydraxComponent.UnderwaterGodRays) == Component)
            {
                return true;
            }
            if ((mComponents & HydraxComponent.UnderwaterReflections) == Component)
            {
                return true;
            }
            if ((mComponents & HydraxComponent.All) == Component)
            {
                return true;
            }
            if ((mComponents & HydraxComponent.None) == Component)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region - SetComponents -
        /// <summary>
        /// Set hydrax components
        /// </summary>
        /// <param name="Components">HydraxComponents</param>
        /// <remarks>It can be called after Create(), Components will be updated</remarks>
        public void SetComponents(HydraxComponent Components)
        {
            mComponents = Components;

            #region - Smooth || Caustics || UnderwaterGodrays -
            if (IsComponent(HydraxComponent.Smooth) ||
                IsComponent(HydraxComponent.Caustics) ||
                IsComponent(HydraxComponent.UnderwaterGodRays))
            {
                #region - NoDepthComponent -
                //delete smooth and/or caustics components which needs depth component
                if (!IsComponent(HydraxComponent.Depth))
                {
                    HydraxComponent s  = HydraxComponent.None;
                    HydraxComponent f  = HydraxComponent.None;
                    HydraxComponent u  = HydraxComponent.None;
                    HydraxComponent ur = HydraxComponent.None;

                    if (IsComponent(HydraxComponent.Sun))
                    {
                        s = HydraxComponent.Sun;
                    }
                    if(IsComponent(HydraxComponent.Foam))
                    {
                        f = HydraxComponent.Foam;
                    }
                    if (IsComponent(HydraxComponent.Underwater))
                    {
                        u = HydraxComponent.Underwater;
                    }
                    if (IsComponent(HydraxComponent.UnderwaterReflections))
                    {
                        ur = HydraxComponent.UnderwaterReflections;
                    }

                    if (IsComponent(HydraxComponent.Smooth))
                    {
                        HydraxLog("Smooth component needs depth component... \nSmooth component desactivated.");
                    }
                    if (IsComponent(HydraxComponent.Caustics))
                    {
                        HydraxLog("Caustics component needs depth component... \nCaustics component desactivated.");
                    }
                    if (IsComponent(HydraxComponent.UnderwaterGodRays))
                    {
                        HydraxLog("God rays component needs depth component... \nGod rays component desactivated.");
                    }

                    mComponents = s | f | u | ur;
                }
                #endregion
            }
            #endregion

            #region - UnderwaterReflections || UnderwaterGodRays -
            if (IsComponent(HydraxComponent.UnderwaterReflections) ||
                    IsComponent(HydraxComponent.UnderwaterGodRays))
            {
                #region - NoUnderwaterComponent -
                //Delete underwater reflections components wich needs underwater component
                if (!IsComponent(HydraxComponent.Underwater))
                {
                    HydraxComponent s = HydraxComponent.None;
                    HydraxComponent f = HydraxComponent.None;
                    HydraxComponent d = HydraxComponent.None;
                    HydraxComponent sm = HydraxComponent.None;
                    HydraxComponent c = HydraxComponent.None;

                    if (IsComponent(HydraxComponent.Sun))
                    {
                        s = HydraxComponent.Sun;
                    }
                    if (IsComponent(HydraxComponent.Foam))
                    {
                        f = HydraxComponent.Foam;
                    }
                    if (IsComponent(HydraxComponent.Depth))
                    {
                        d = HydraxComponent.Depth;
                    }
                    if (IsComponent(HydraxComponent.Smooth))
                    {
                        sm = HydraxComponent.Smooth;
                    }
                    if (IsComponent(HydraxComponent.Caustics))
                    {
                        c = HydraxComponent.Caustics;
                    }

                    if (IsComponent(HydraxComponent.UnderwaterReflections))
                    {
                        HydraxLog("Underwater reflections component needs underwater component... \nUnderwater reflections component desactivated.");
                    }
                    if (IsComponent(HydraxComponent.UnderwaterGodRays))
                    {
                        HydraxLog("God rays component needs underwater component... \nGod rays component desactivated.");
                    }

                    mComponents = s | f | d | sm | c;
                }
                #endregion

                #region - UnderwaterGodRays && !SunComponent -
                if (IsComponent(HydraxComponent.UnderwaterGodRays) &&
                    !IsComponent(HydraxComponent.Sun))
                {
                    HydraxLog("God rays component needs sun component... \nGod rays component desactivated.");
                    HydraxComponent f = HydraxComponent.None;
                    HydraxComponent d = HydraxComponent.None;
                    HydraxComponent c = HydraxComponent.None;
                    HydraxComponent sm = HydraxComponent.None;
                    HydraxComponent u = HydraxComponent.None;
                    HydraxComponent ur = HydraxComponent.None;

                    if (IsComponent(HydraxComponent.Foam))
                    {
                        f = HydraxComponent.Foam;
                    }
                    if (IsComponent(HydraxComponent.Depth))
                    {
                        d = HydraxComponent.Depth;
                    }
                    if (IsComponent(HydraxComponent.Caustics))
                    {
                        c = HydraxComponent.Caustics;
                    }
                    if (IsComponent(HydraxComponent.Underwater))
                    {
                        u = HydraxComponent.Underwater;
                    }
                    if (IsComponent(HydraxComponent.UnderwaterReflections))
                    {
                        ur = HydraxComponent.UnderwaterReflections;
                    }

                    mComponents = f | d | sm | c | u | ur;
                }
                #endregion
            }
            #endregion

            int NumberOfDepthChannels = 0;

            if (IsComponent(HydraxComponent.Depth))
            {
                NumberOfDepthChannels++;

                if (IsComponent(HydraxComponent.Caustics))
                {
                    NumberOfDepthChannels++;
                }

                if (IsComponent(HydraxComponent.UnderwaterGodRays))
                {
                    NumberOfDepthChannels++;
                }
            }

            if (NumberOfDepthChannels > 0)
            {
                RttManager.NumberOfChannels mNOC = (RttManager.NumberOfChannels)NumberOfDepthChannels;
                mRttManager.SetNumberOfChannels(RttManager.RttType.RTT_DEPTH, mNOC);
            }

            if (!mIsCreated || mModule == null)
            {
                return;
            }

            if (IsComponent(HydraxComponent.UnderwaterGodRays))
            {
                mGodRaysManager.Create(mComponents);
            }
            else
            {
                mGodRaysManager.Remove();
            }

            //Check Rtt's
            if (!IsComponent(HydraxComponent.Depth))
            {
                mRttManager.Remove(RttManager.RttType.RTT_DEPTH);
                mRttManager.Remove(RttManager.RttType.RTT_DEPTH_REFLECTION);
            }
            else
            {
                mRttManager.Initialize(RttManager.RttType.RTT_DEPTH);

                if (IsComponent(HydraxComponent.UnderwaterReflections) &&
                    mIsCurrentFrameUnderwater)
                {
                    mRttManager.Initialize(RttManager.RttType.RTT_DEPTH_REFLECTION);
                }
            }

            if (!IsComponent(HydraxComponent.Underwater) && mIsCurrentFrameUnderwater)
            {
                mRttManager.Textures[(int)RttManager.RttType.RTT_REFRACTION].GetBuffer()
                    .GetRenderTarget().GetViewport(0).ShowSkies = false;

                mRttManager.Textures[(int)RttManager.RttType.RTT_REFLECTION].GetBuffer()
                    .GetRenderTarget().GetViewport(0).ShowSkies = true;
            }

            mMaterialManager.CreateMaterials(mComponents, new MaterialManager.Options(
                mShaderMode, mModule.NormalMode));

            if (!IsComponent(HydraxComponent.Underwater))
            {
                mIsCurrentFrameUnderwater = false;
            }

            mMesh.MaterialName =
                (mIsCurrentFrameUnderwater ?
                mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_UNDERWATER].Name :
                mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_WATER].Name);

            if (mIsCurrentFrameUnderwater)
            {
                mMaterialManager.SetCompositorEnabled(MaterialManager.CompositorType.COMP_UNDERWATER, true);
            }

        }
        #endregion

        #region - SetModule -
        /// <summary>
        /// Set Hydrax module.
        /// </summary>
        /// <param name="Module">Hydrax Module</param>
        /// <param name="DeleteOldModule">Delete, if exists, the old Module</param>
        public void SetModule(Module Module, bool DeleteOldModule)
        {
            if (mModule != null)
            {
                if (mModule.NormalMode != mModule.NormalMode)
                {
                    mMaterialManager.CreateMaterials(mComponents,
                        new MaterialManager.Options(mShaderMode, mModule.NormalMode));
                    mMesh.MaterialName =
                        mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_WATER].Name;
                }

                if (mModule.NormalMode == MaterialManager.NormalMode.NM_RTT &&
                    mModule.IsCreated &&
                    mModule.Noise.IsGPUNormalMapResourcesCreated)
                {
                    mModule.Noise.RemoveGPUNormalMapRecources(mGPUNormalMapManager);
                }
                if (DeleteOldModule)
                {
                    mModule = null;
                }
                else
                {
                    mModule.Remove();
                }
                // Due to modules can change -internally- scene nodes position,
                // just reset them to the original position.
                Position = mPosition;
            }//end if module != null

            mModule = Module;

            if (mIsCreated)
            {
                if (!mModule.IsCreated)
                {
                    mModule.Create();

                    if (mModule.NormalMode == MaterialManager.NormalMode.NM_RTT)
                    {
                        if (!mModule.Noise.CreateGPUNormalMapResources(mGPUNormalMapManager))
                        {
                            HydraxLog(mModule.Name + " doesn't support GPU Normal map generation.");
                        }
                    }//end if NormalMode
                }//end if module.IsCreated
                else
                {
                    if (mModule.NormalMode == MaterialManager.NormalMode.NM_RTT)
                    {
                        if (!mModule.Noise.IsGPUNormalMapResourcesCreated)
                        {
                            if (!mModule.Noise.CreateGPUNormalMapResources(mGPUNormalMapManager))
                            {
                                HydraxLog(mModule.Name + " doesn't support GPU Normal map generation.");
                            }
                        }
                    }//end if normal mode
                    else
                    {
                        if (mModule.Noise.IsGPUNormalMapResourcesCreated)
                        {
                            mModule.Noise.RemoveGPUNormalMapRecources(mGPUNormalMapManager);
                        }
                    }//end else normal mode
                }//end else IsCreated
            }//end if IsCreated

            HydraxLog("Updating water mesh...");
            string MaterialNameTmp = mMesh.MaterialName;

            HydraxLog("Deleting water mesh...");
            mMesh.Remove();
            HydraxLog("Water mesh deleted.");

            HydraxLog("Creating water mesh...");
            mMesh.MeshOptions = mModule.MeshOptions;
            mMesh.MaterialName = MaterialNameTmp;
            mMesh.Create();
            Position = mPosition;
            HydraxLog("Water mesh created.");
            HydraxLog("Module set.");
        }
        public void SetModule(Module Module)
        {
            SetModule(Module, true);
        }
        #endregion

        #region - CheckIsVisible -
        /// <summary>
        /// IsVisible helper function
        /// </summary>
        private void CheckIsVisible()
        {
            if (!mIsCreated)
            {
                return;
            }
            if (!mIsVisible)
            {
                //stop RTTs
                mRttManager.RemoveAll();

                //Hide hydrax mehs
                mMesh.SceneNode.IsVisible = false;

                //stop compositor
                mMaterialManager.SetCompositorEnabled(MaterialManager.CompositorType.COMP_UNDERWATER, false);
            }
            else
            {
                //start refletion and refraction RTTs
                mRttManager.Initialize(RttManager.RttType.RTT_REFLECTION);
                mRttManager.Initialize(RttManager.RttType.RTT_REFRACTION);

                //start depth rtt if needed
                if(IsComponent(HydraxComponent.Depth))
                {
                    mRttManager.Initialize(RttManager.RttType.RTT_DEPTH);
                }

                //start GPU Normals rtt if needed
                if(mModule.NormalMode == MaterialManager.NormalMode.NM_RTT)
                {
                    mGPUNormalMapManager.Create();
                }

                //set over-water material check for underwater
                mMesh.MaterialName = mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_WATER].Name;
                mMaterialManager.Reload(MaterialManager.MaterialType.MAT_WATER);

                CheckUnderwater(0);

                //set hydrax mesh node visible
                mMesh.SceneNode.IsVisible = true;

            }
        }
        #endregion

        #region - Rotate -
        /// <summary>
        /// Rotate water and planes
        /// </summary>
        /// <param name="Q">Quaternion</param>
        public void Rotate(Quaternion Q)
        {
            if (!mIsCreated)
            {
                HydraxLog("Hydrax.Rotate(...) must be called after Hydrax.Create(), skipping...");
                return;
            }

            mMesh.SceneNode.Rotate(Q);
            mRttManager.PlaneSceneNode.Rotate(Q);

            //For world-space -> object-space conversion
            SunPosition = mSunPosition;
        }
        #endregion

        #region - CheckUnderwater -
        /// <summary>
        /// Check for underwater effects.
        /// </summary>
        /// <param name="TimeSinceLastFrame">Time since last frame</param>
        private void CheckUnderwater(float TimeSinceLastFrame)
        {
            if (!IsComponent(HydraxComponent.Underwater))
            {
                mIsCurrentFrameUnderwater = false;
                return;
            }

            //if the Camera is under the current water x/z position
            if (GetHeight(mCamera.DerivedPosition) > mCamera.DerivedPosition.y - mUnderWaterCameraSwitchDelta)
            {
                mIsCurrentFrameUnderwater = true;
                if (mMesh.MaterialName != mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_UNDERWATER].Name)
                {
                    mRttManager.Textures[(int)RttManager.RttType.RTT_REFLECTION].GetBuffer()
                        .GetRenderTarget().GetViewport(0).ShowSkies = true;

                    if (IsComponent(HydraxComponent.UnderwaterReflections))
                    {
                        mRttManager.Textures[(int)RttManager.RttType.RTT_REFLECTION].GetBuffer()
                            .GetRenderTarget().GetViewport(0).ShowSkies = false;
                        if (IsComponent(HydraxComponent.Depth))
                        {
                            mRttManager.Initialize(RttManager.RttType.RTT_DEPTH_REFLECTION);
                        }
                    }
                    else
                    {
                        mRttManager.Remove(RttManager.RttType.RTT_REFLECTION);
                    }

                    if (IsComponent(HydraxComponent.Depth) && IsComponent(HydraxComponent.UnderwaterReflections))
                    {
                        mRttManager.Initialize(RttManager.RttType.RTT_DEPTH_REFLECTION);
                    }

                    mMaterialManager.Reload(MaterialManager.MaterialType.MAT_UNDERWATER);

                    mMaterialManager.SetCompositorEnabled(MaterialManager.CompositorType.COMP_UNDERWATER, true);

                    mMesh.MaterialName = mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_UNDERWATER].Name;
                }//end second if

                //update god rays
                if (IsComponent(HydraxComponent.UnderwaterGodRays))
                {
                    mGodRaysManager.Update(TimeSinceLastFrame);
                }
            }//end first if
            else
            {
                mIsCurrentFrameUnderwater = false;
                if (mMesh.MaterialName != mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_WATER].Name)
                {
                    //we asume that RefractionRTT / ReflectionRTT are initialized
                    mRttManager.Textures[(int)RttManager.RttType.RTT_REFRACTION].GetBuffer()
                        .GetRenderTarget().GetViewport(0).ShowSkies = false;

                    if (!IsComponent(HydraxComponent.UnderwaterReflections))
                    {
                        mRttManager.Initialize(RttManager.RttType.RTT_REFLECTION);
                        mMaterialManager.Reload(MaterialManager.MaterialType.MAT_WATER);
                    }

                    mRttManager.Textures[(int)RttManager.RttType.RTT_REFLECTION].GetBuffer()
                        .GetRenderTarget().GetViewport(0).ShowSkies = true;

                    if (IsComponent(HydraxComponent.Depth) && IsComponent(HydraxComponent.UnderwaterReflections))
                    {
                        mRttManager.Initialize(RttManager.RttType.RTT_REFLECTION);
                    }

                    mMaterialManager.SetCompositorEnabled(MaterialManager.CompositorType.COMP_UNDERWATER, false);

                    mMesh.MaterialName = mMaterialManager.Materials[(int)MaterialManager.MaterialType.MAT_WATER].Name;

                }


            }

        }
        #endregion

        #region - GetHeight -
        /// <summary>
        /// Get the current heigth at a especified world-space point.
        /// </summary>
        /// <param name="Position">X/Z World position.</param>
        /// <returns>Heigth at the given position in y-World coordinates, if it's outside of the water return -1.</returns>
        public float GetHeight(Vector2 Position)
        {
            if (mModule != null)
            {
                return mModule.GetHeight(Position);
            }
            return -1;
        }
        /// <summary>
        /// Get the current heigth at a especified world-space point
        /// </summary>
        /// <param name="Position">X/(Y)/Z World position</param>
        /// <returns>Heigth at the given position in y-World coordinates, if it's outside of the water return -1</returns>
        public float GetHeight(Vector3 Position)
        {
            return GetHeight(new Vector2(Position.x, Position.z));
        }
        #endregion

        #region - LoadCfg -
        public void LoadCfg(string FileName)
        {
            mCfgFileManager.Load(FileName);
        }
        #endregion

        #region - HydraxLog -
        /// <summary>
        /// Logs a message to the Logfile.
        /// </summary>
        /// <param name="Message">Message to Log</param>
        public static void HydraxLog(String Message)
        {
            if (LogManager.Instance == null)
                return;
            LogManager.Instance.Write("[Hydrax]: " + Message, null);
        }
        #endregion
        #endregion
    }//end class
    #endregion
}//end namespace
#endregion
