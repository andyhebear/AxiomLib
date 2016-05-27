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
using Axiom.Graphics;
using Axiom.Math;
using Axiom.Core;
using AG = Axiom.Graphics;
#endregion

#region - namespace -
namespace Axiom.Hydrax
{
    #region - MaterialManager -
    /// <summary>
    /// Material/Shader manager class
    /// </summary>
    public class MaterialManager
    {
        #region - Consts -
        public const string  _def_Water_Material_Name  = "_Hydrax_Water_Material";
        public const string  _def_Water_Shader_VP_Name = "_Hydrax_Water_VP";
        public const string  _def_Water_Shader_FP_Name = "_Hydrax_Water_FP";

        public const string  _def_Depth_Material_Name  = "_Hydrax_Depth_Material";
        public const string  _def_Depth_Shader_VP_Name = "_Hydrax_Depth_VP";
        public const string  _def_Depth_Shader_FP_Name = "_Hydrax_Depth_FP";

        public const string  _def_DepthTexture_Shader_VP_Name = "_Hydrax_DepthTexture_VP";
        public const string  _def_DepthTexture_Shader_FP_Name = "_Hydrax_DepthTexture_FP";

        public const string  _def_Underwater_Material_Name  = "_Hydrax_Underwater_Material";
        public const string  _def_Underwater_Shader_VP_Name = "_Hydrax_Underwater_Shader_VP";
        public const string  _def_Underwater_Shader_FP_Name = "_Hydrax_Underwater_Shader_FP";

        public const string  _def_Underwater_Compositor_Material_Name  = "_Hydrax_Underwater_Compositor_Material";
        public const string  _def_Underwater_Compositor_Shader_VP_Name = "_Hydrax_Underwater_Compositor_Shader_VP";
        public const string  _def_Underwater_Compositor_Shader_FP_Name = "_Hydrax_Underwater_Compositor_Shader_FP";

        public const string  _def_Underwater_Compositor_Name = "_Hydrax_Underwater_Compositor_Name";

        public const string  _def_Simple_Red_Material_Name = "_Hydrax_Simple_Red_Material";
        public const string  _def_Simple_Black_Material_Name = "_Hydrax_Simple_Black_Material";
        #endregion

        #region - Enums - 

        #region - MaterialType -
        /// <summary>
        ///Material type enum
        ///<remarks>Use in Materials[MaterialType]</remarks>
        /// </summary>
        public enum MaterialType
        {
            /// <summary>
            ///Water material 
            /// </summary>
            MAT_WATER = 0, 
            /// <summary>
            /// Depth material
            /// </summary>
            MAT_DEPTH = 1,
            /// <summary>
            /// Underwater material
            /// </summary>
            MAT_UNDERWATER = 2,
            /// <summary>
            /// Compositor material(material wich is used in underwater compositor) 
            /// </summary>
            MAT_UNDERWATER_COMPOSITOR = 3,
            /// <summary>
            /// Simple red material
            /// </summary>
            MAT_SIMPLE_RED = 4,
            /// <summary>
            /// Simple black material
            /// </summary>
            MAT_SIMPLE_BLACK = 5
        };
        #endregion

        #region - ShaderMode -
        /// <summary>
        /// Shader Mode.
        /// </summary>
        public enum ShaderMode
        {
            /// <summary>
            /// HLSL
            /// </summary>
            SM_HLSL = 0,
            /// <summary>
            /// CG
            /// </summary>
            SM_CG = 1,
            /// <summary>
            /// GLSL
            /// </summary>
            SM_GLSL = 2
        }
        #endregion

        #region - NormalMode -
        /// <summary>
        /// Normal generation mode
        /// </summary>
        public enum NormalMode
        {
            // Normal map from precomputed texture(CPU)
            NM_TEXTURE = 0,
            // Normal map from vertex(CPU)
            NM_VERTEX = 1,
            // Normal map from RTT(GPU)
            NM_RTT = 2
        };
        #endregion

        #region - CompositorType -
        ///<summary>
        ///Compositor type enum
        ///<remarks>Use in getCompositor(CompositorType)</remarks>
        /// </summary>
        public enum CompositorType
        {
            /// <summary>
            /// Underwater compositor
            /// </summary>
            COMP_UNDERWATER = 0
        };
        #endregion

        #region - GpuProgram
        ///<summary>
        ///Gpu program enum
        ///<remarks>Use in setGpuProgramParameter()</remarks>
        /// </summary>
        public enum GpuProgram
        {
            /// <summary>
            /// Vertex program 
            /// </summary>
            GPUP_VERTEX = 0,
            /// <summary>
            /// Fragment program
            /// </summary>
            GPUP_FRAGMENT = 1
        };
        #endregion

        #endregion

        #region - Options -
        /// <summary>
        /// Material options
        /// </summary>
        public struct Options
        {
            /// <summary>
            /// ShaderMode.
            /// </summary>
            public ShaderMode SM;
            /// <summary>
            /// NormalMode.
            /// </summary>
            public NormalMode NM;
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="SM">Shader mode</param>
            /// <param name="NM">Normal generation mode</param>
            public Options(ShaderMode SM, NormalMode NM)
            {
                this.SM = SM;
                this.NM = NM;
            }
        }
        #endregion

        #region - Fields -
        protected bool mIsCreated;
        protected Hydrax mHydrax;
        protected Options mOptions;
        protected Material[] mMaterials = new Material[6];
        protected Compositor[] mCompositors = new Compositor[1];
        protected bool[] mCompositorNeedToBeReloaded = new bool[1];
        protected bool[] mCompositorEnable = new bool[1];
        protected List<Technique> mDepthTechniques = new List<Technique>();
        protected HydraxComponent mComponents;
        internal UnderwaterCompositorListener mUnderwaterCompositorListener;
        #endregion

        #region - Properties -
        public HydraxComponent Components
        {
            set { mComponents = value; }
            get { return mComponents; }
        }
        public bool[] CompositorNeedToBeReloaded
        {
            set { mCompositorNeedToBeReloaded = value; }
            get { return mCompositorNeedToBeReloaded; }
        }
        public Hydrax Hydrax
        {
            set { mHydrax = value; }
            get { return mHydrax; }
        }
        /// <summary>
        /// Get external depth techniques
        /// </summary>
        public List<Technique> DepthTechniques
        {
            set { mDepthTechniques = value; }
            get { return mDepthTechniques; }
        }
        public Material[] Materials
        {
            set { mMaterials = value; }
            get { return mMaterials; }
        }
        public Options LastOptions
        {
            set { mOptions = value; }
            get { return mOptions; }
        }
        public bool IsCreated
        {
            set { mIsCreated = value; }
            get { return mIsCreated; }
        }
        #endregion

        #region - Constructor, Destructor -
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Hydrax"></param>
        public MaterialManager(Hydrax Hydrax)
        {
            mHydrax = Hydrax;
            //set default options.
            mOptions.NM = NormalMode.NM_TEXTURE;
            mOptions.SM = ShaderMode.SM_HLSL;

            mUnderwaterCompositorListener = new UnderwaterCompositorListener(this);
        }
        ~MaterialManager()
        {
            RemoveMaterials();
        }
        #endregion

        #region - Methods -

        #region - Public -

        #region - CreateMaterials -
        /// <summary>
        /// Create Materials.
        /// </summary>
        /// <param name="Components">Components of the Shader.</param>
        /// <param name="Options">Material Options</param>
        /// <returns>true if Succed.</returns>
        public bool CreateMaterials(HydraxComponent Components, Options Options)
        {
            RemoveMaterials();

            Hydrax.HydraxLog("Creating water material...");
            if (!CreateWaterMaterial(Components, Options))
            {
                return false;
            }

            mHydrax.DecalsManager.RegisterAll();
            Hydrax.HydraxLog("Water material created.");

            if (IsComponent(Components, HydraxComponent.Depth))
            {
                Hydrax.HydraxLog("Creating depth material...");
                if (!CreateDepthMaterial(Components, Options))
                {
                    return false;
                }
                Hydrax.HydraxLog("Depth material created.");
            }

            
            if (IsComponent(Components, HydraxComponent.Underwater))
            {
                Hydrax.HydraxLog("Creating underwater materials...");
                if (!CreateUnderwaterMaterial(Components, Options))
                {
                    return false;
                }
                if (!CreateUnderwaterCompositor(Components, Options))
                {
                    return false;
                }
                if (!CreateSimpleColorMaterial(ColorEx.Red, MaterialType.MAT_SIMPLE_RED, _def_Simple_Red_Material_Name, false, true))
                {
                    return false;
                }
                Hydrax.HydraxLog("Underwater material created.");
            }

            mComponents = Components;
            mOptions = Options;
            mIsCreated = true;
            
            foreach(Technique technique in mDepthTechniques)
			{
				bool isTextureDepthTechnique = technique.Name == "_Hydrax_Depth_Technique" ? false : true;
				
				if (isTextureDepthTechnique)
				{
					String DepthTextureName = technique.GetPass(0).GetTextureUnitState(0).Name == "_DetphTexture_Hydrax" ? technique.GetPass(0).GetTextureUnitState(0).TextureName : technique.GetPass(0).GetTextureUnitState(1).TextureName;
					AddDepthTextureTechnique(technique,DepthTextureName,technique.GetPass(0).Name,false);
				}
				else
				{
					AddDepthTechnique(technique, false);
				}
				
			}
			return true;
        }
        #endregion

        #region - RemoveMaterials -
        /// <summary>
        /// Remove Materials.
        /// <remarks>RemoveCompositor() is called too.</remarks>
        /// </summary>
        public void RemoveMaterials()
        {
            if (AG.MaterialManager.Instance.ResourceExists(_def_Water_Material_Name))
            {
                AG.MaterialManager.Instance.Remove(_def_Water_Material_Name);
                HighLevelGpuProgramManager.Instance.Unload(_def_Water_Shader_VP_Name);
                HighLevelGpuProgramManager.Instance.Unload(_def_Water_Shader_FP_Name);
                HighLevelGpuProgramManager.Instance.Remove(_def_Water_Shader_VP_Name);
                HighLevelGpuProgramManager.Instance.Remove(_def_Water_Shader_FP_Name);
            }

            if(AG.MaterialManager.Instance.ResourceExists(_def_Depth_Material_Name))
            {
                AG.MaterialManager.Instance.Remove(_def_Depth_Material_Name);
                HighLevelGpuProgramManager.Instance.Unload(_def_Depth_Shader_VP_Name);
                HighLevelGpuProgramManager.Instance.Unload(_def_Depth_Shader_FP_Name);
                HighLevelGpuProgramManager.Instance.Remove(_def_Depth_Shader_VP_Name);
                HighLevelGpuProgramManager.Instance.Remove(_def_Depth_Shader_FP_Name);
            }

            if (AG.MaterialManager.Instance.ResourceExists(_def_Simple_Red_Material_Name))
            {
                AG.MaterialManager.Instance.Remove(_def_Simple_Red_Material_Name);
            }
            if (AG.MaterialManager.Instance.ResourceExists(_def_Simple_Black_Material_Name))
            {
                AG.MaterialManager.Instance.Remove(_def_Simple_Black_Material_Name);
            }

            string[] AlphaChannels = {"x","y","z","w",
		                              "r","g","b","a"};

            if (HighLevelGpuProgramManager.Instance == null)
                return;
            for (int k = 0; k < 8; k++)
            {
                if (HighLevelGpuProgramManager.Instance.ResourceExists(_def_DepthTexture_Shader_VP_Name + AlphaChannels[k]))
                {
                    HighLevelGpuProgramManager.Instance.Unload(_def_DepthTexture_Shader_VP_Name + AlphaChannels[k]);
                    HighLevelGpuProgramManager.Instance.Unload(_def_DepthTexture_Shader_FP_Name + AlphaChannels[k]);
                    HighLevelGpuProgramManager.Instance.Remove(_def_DepthTexture_Shader_VP_Name + AlphaChannels[k]);
                    HighLevelGpuProgramManager.Instance.Remove(_def_DepthTexture_Shader_FP_Name + AlphaChannels[k]);
                }
            }

            RemoveCompositor();

            mIsCreated = false;
        }
        #endregion

        #region - RemoveCompositor -
        /// <summary>
        /// Remove Compositor.
        /// </summary>
        public void RemoveCompositor()
        {
            if (AG.MaterialManager.Instance.ResourceExists(_def_Underwater_Compositor_Material_Name))
            {
                SetCompositorEnabled(CompositorType.COMP_UNDERWATER, false);
                CompositorManager.Instance.Remove(_def_Underwater_Compositor_Name);

                AG.MaterialManager.Instance.Remove(_def_Underwater_Compositor_Material_Name);

                HighLevelGpuProgramManager.Instance.Unload(_def_Underwater_Compositor_Shader_VP_Name);
                HighLevelGpuProgramManager.Instance.Unload(_def_Underwater_Compositor_Shader_FP_Name);
                HighLevelGpuProgramManager.Instance.Remove(_def_Underwater_Compositor_Shader_VP_Name);
                HighLevelGpuProgramManager.Instance.Remove(_def_Underwater_Compositor_Shader_FP_Name);
            }
        }
        #endregion

        #region - Reload -
        /// <summary>
        /// Reload Material
        /// </summary>
        /// <param name="MatType">Material to reload.</param>
        public void Reload(MaterialType MatType)
        {
            Material Mat = mMaterials[(int)MatType];
            if (Mat == null)
                return;

            Mat.Reload();

            bool cDepth = IsComponent(mComponents, HydraxComponent.Depth);
            bool cSmooth = IsComponent(mComponents, HydraxComponent.Smooth);
            bool cSun = IsComponent(mComponents, HydraxComponent.Sun);
            bool cFoam = IsComponent(mComponents, HydraxComponent.Foam);
            bool cCaustics = IsComponent(mComponents, HydraxComponent.Caustics);
            bool cUReflections = IsComponent(mComponents, HydraxComponent.UnderwaterReflections);

            switch (MatType)
            {
                #region - MatWater -
                case MaterialType.MAT_WATER:
                    {
                        Pass M_Technique0_Pass0 = Mat.GetTechnique(0).GetPass(0);

                        #region - SwitchNormalMode -
                        switch (mOptions.NM)
                        {
                            case NormalMode.NM_TEXTURE:
                                //fall trough
                            case NormalMode.NM_RTT:
                                {
                                    M_Technique0_Pass0.GetTextureUnitState(0).SetTextureName("HydraxNormalMap");
                                    M_Technique0_Pass0.GetTextureUnitState(1).SetTextureName("HydraxReflectionMap");
                                    M_Technique0_Pass0.GetTextureUnitState(2).SetTextureName("HydraxRefractionMap");
                                    if (cDepth)
                                    {
                                        M_Technique0_Pass0.GetTextureUnitState(3).SetTextureName("HydraxDepthMap");
                                    }
                                }
                                break;
                            case NormalMode.NM_VERTEX:
                                {
                                    M_Technique0_Pass0.GetTextureUnitState(0).SetTextureName("HydraxReflectionMap");
                                    M_Technique0_Pass0.GetTextureUnitState(1).SetTextureName("HydraxRefractionMap");
                                    if (cDepth)
                                    {
                                        M_Technique0_Pass0.GetTextureUnitState(2).SetTextureName("HydraxDepthMap");
                                    }
                                }
                                break;
                        }//end switch normal mode
                        #endregion

                        GpuProgramParameters VP_Parameters = M_Technique0_Pass0.VertexProgramParameters;
                        GpuProgramParameters FP_Parameters = M_Technique0_Pass0.FragmentProgramParameters;
                        VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
                        if (cFoam)
                        {
                            VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);
                        }
                        FP_Parameters.SetNamedAutoConstant("uEyePosition", GpuProgramParameters.AutoConstantType.CameraPositionObjectSpace, 0);
                        FP_Parameters.SetNamedConstant("uFullReflectionDistance", mHydrax.FullReflectionDistance);
                        FP_Parameters.SetNamedConstant("uGlobalTransparency", mHydrax.GlobalTransparency);
                        FP_Parameters.SetNamedConstant("uNormalDistortion", mHydrax.NormalDistortion);

                        if (cDepth)
                        {
                            FP_Parameters.SetNamedConstant("uWaterColor", mHydrax.WaterColor);
                        }
                        if (cSmooth)
                        {
                            FP_Parameters.SetNamedConstant("uSmoothPower", mHydrax.SmoothPower);
                        }
                        if (cSun)
                        {
                            FP_Parameters.SetNamedConstant("uSunPosition", mHydrax.Mesh.GetObjectSpacePosition(mHydrax.SunPosition));
                            FP_Parameters.SetNamedConstant("uSunStrength", mHydrax.SunStrenght);
                            FP_Parameters.SetNamedConstant("uSunArea", mHydrax.SunArea);
                            FP_Parameters.SetNamedConstant("uSunColor", mHydrax.SunColor);
                        }
                        if (cFoam)
                        {
                            FP_Parameters.SetNamedConstant("uFoamRange", mHydrax.Mesh.MeshOptions.MeshStrength);
                            FP_Parameters.SetNamedConstant("uFoamMaxDistance", mHydrax.FoamMaxDistance);
                            FP_Parameters.SetNamedConstant("uFoamScale", mHydrax.FoamScale);
                            FP_Parameters.SetNamedConstant("uFoamStart", mHydrax.FoamStart);
                            FP_Parameters.SetNamedConstant("uFoamTransparency", mHydrax.FoamTransparency);
                        }
                        if (cCaustics)
                        {
                            FP_Parameters.SetNamedConstant("uCausticsPower", mHydrax.CausticsPower);
                        }
                    }
                    break;
                #endregion

                #region - MatDepth -
                case MaterialType.MAT_DEPTH:
                    {
                        Pass M_Technique0_Pass0 = Mat.GetTechnique(0).GetPass(0);

                        GpuProgramParameters VP_Parameters = M_Technique0_Pass0.VertexProgramParameters;
                        GpuProgramParameters FP_Parameters = M_Technique0_Pass0.FragmentProgramParameters;

                        VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
                        VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);
                        VP_Parameters.SetNamedConstant("uPlaneYPos", mHydrax.Position.y);

                        FP_Parameters.SetNamedConstant("uDepthLimit", 1 / mHydrax.DepthLimit);

                        if (cCaustics)
                        {
                            FP_Parameters.SetNamedConstant("uCausticsScale", mHydrax.CausticsScale);
                            FP_Parameters.SetNamedConstant("uCausticsEnd", mHydrax.CausticsEnd);
                        }
                    }
                    break;
                #endregion

                #region - MatUnderwater -
                case MaterialType.MAT_UNDERWATER:
                    {
                        Pass M_Technique0_Pass0 = Mat.GetTechnique(0).GetPass(0);

                        #region - NormalMode -
                        switch (mOptions.NM)
                        {
                            case NormalMode.NM_TEXTURE:
                                //fall trough
                            case NormalMode.NM_RTT:
                                {
                                    M_Technique0_Pass0.GetTextureUnitState(0).SetTextureName("HydraxNormalMap");
                                    int Index = 1;
                                    if (cUReflections)
                                    {
                                        M_Technique0_Pass0.GetTextureUnitState(Index).SetTextureName("HydraxReflectionMap");
                                        Index++;
                                    }
                                    M_Technique0_Pass0.GetTextureUnitState(Index).SetTextureName("HydraxRefractionMap");
                                    if (cDepth && cUReflections)
                                    {
                                        M_Technique0_Pass0.GetTextureUnitState(Index).SetTextureName("HydraxDepthReflectionMap");
                                    }
                                }
                                break;
                            case NormalMode.NM_VERTEX:
                                {
                                    int Index = 0;
                                    if (cUReflections)
                                    {
                                        M_Technique0_Pass0.GetTextureUnitState(Index).SetTextureName("HydraxReflectionMap");
                                        Index++;
                                    }
                                    M_Technique0_Pass0.GetTextureUnitState(Index).SetTextureName("HydraxRefractionMap");
                                    Index++;
                                    if (cDepth && cUReflections)
                                    {
                                        M_Technique0_Pass0.GetTextureUnitState(Index).SetTextureName("HydraxDepthReflectionMap");
                                    }
                                }
                                break;
                        }
                        #endregion

                        GpuProgramParameters VP_Parameters = M_Technique0_Pass0.VertexProgramParameters;
                        GpuProgramParameters FP_Parameters = M_Technique0_Pass0.FragmentProgramParameters;

                        VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix,0);
                        if (cFoam)
                        {
                            VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);
                        }
                        FP_Parameters.SetNamedAutoConstant("uEyePosition", GpuProgramParameters.AutoConstantType.CameraPositionObjectSpace, 0);
                        FP_Parameters.SetNamedConstant("uFullReflectionDistance", mHydrax.FullReflectionDistance);
                        FP_Parameters.SetNamedConstant("uGlobalTransparency", mHydrax.GlobalTransparency);
                        FP_Parameters.SetNamedConstant("uNormalDistortion", mHydrax.NormalDistortion);

                        if ((cDepth && cUReflections) || (!cUReflections))
                        {
                            FP_Parameters.SetNamedConstant("uWaterColor", mHydrax.WaterColor);
                        }
                        if (cSun)
                        {
                            FP_Parameters.SetNamedConstant("uSunPosition", mHydrax.Mesh.GetObjectSpacePosition(mHydrax.SunPosition));
                            FP_Parameters.SetNamedConstant("uSunStrength", mHydrax.SunStrenght);
                            FP_Parameters.SetNamedConstant("uSunArea", mHydrax.SunArea);
                            FP_Parameters.SetNamedConstant("uSunColor", mHydrax.SunColor);
                        }
                        if (cFoam)
                        {
                            FP_Parameters.SetNamedConstant("uFoamRange", mHydrax.Mesh.MeshOptions.MeshStrength);
                            FP_Parameters.SetNamedConstant("uFoamMaxDistance", mHydrax.FoamMaxDistance);
                            FP_Parameters.SetNamedConstant("uFoamScale", mHydrax.FoamScale);
                            FP_Parameters.SetNamedConstant("uFoamStart", mHydrax.FoamStart);
                            FP_Parameters.SetNamedConstant("uFoamTransparency", mHydrax.FoamTransparency);
                        }
                        if (cCaustics && cDepth && cUReflections)
                        {
                            FP_Parameters.SetNamedConstant("uCausticsPower", mHydrax.CausticsPower);
                        }
                    }
                    break;
                #endregion

                #region - MatUnderwaterCompositor -
                case MaterialType.MAT_UNDERWATER_COMPOSITOR:
                    {
                        mCompositorNeedToBeReloaded[(int)CompositorType.COMP_UNDERWATER] = true;
                    }
                    break;
                #endregion
            }//end switch mattype
        }//end method
        #endregion

        #region - FillGpuProgramsToPass -
        /// <summary>
        /// Fill GPU vertex and fragment program to a pass
        /// </summary>
        /// <param name="Pass">Pass to fill Gpu programs</param>
        /// <param name="GpuProgramNames">[0]: Vertex program name, [1]: Fragment program name</param>
        /// <param name="SM">Shader mode, note: Provided data strings will correspond with selected shader mode</param>
        /// <param name="EntryPoints">[0]: Vertex program entry point, [1]: Fragment program entry point</param>
        /// <param name="Data">[0] Vertex program data, [1]: Fragment program data</param>
        /// <returns>true if succed</returns>
        public bool FillGpuProgramsToPass(Pass Pass, string[] GpuProgramNames,
            ShaderMode SM, string[] EntryPoints, string[] Data)
        {
            GpuProgram[] GpuPrograms = { GpuProgram.GPUP_VERTEX, GpuProgram.GPUP_FRAGMENT };

            for (int k = 0; k < 2; k++)
            {
                if (!CreateGpuProgram(GpuProgramNames[k], SM, GpuPrograms[k], EntryPoints[k], Data[k]))
                {
                    return false;
                }
            }

            Pass.VertexProgramName = GpuProgramNames[0];
            Pass.FragmentProgramName = GpuProgramNames[1];
            
            return true;
        }
        #endregion

        #region - CreateGpuProgram -
        /// <summary>
        /// Create GPU program
        /// </summary>
        /// <param name="Name">HighLevelGpuProgram name</param>
        /// <param name="SM">Shader mode</param>
        /// <param name="GPUP">GpuProgram type</param>
        /// <param name="EntryPoint">Entry point</param>
        /// <param name="Data">Data</param>
        /// <returns>true if succed.</returns>
        public bool CreateGpuProgram(string Name, ShaderMode SM,
            GpuProgram GPUP, string EntryPoint, string Data)
        {
            if (HighLevelGpuProgramManager.Instance.ResourceExists(Name))
            {
                Hydrax.HydraxLog("Error in bool MaterialManger.CreateGpuProgram(): " +
                    Name + " allready exists!");

                return false;
            }

            string[] ShaderModesStr = { "hlsl", "cg", "glsl" };
            string[] Profiles = new string[2];

            switch (SM)
            {
                case ShaderMode.SM_HLSL:
                    {
                        Profiles[0] = "target";

                        if (GPUP == GpuProgram.GPUP_VERTEX)
                        {
                            Profiles[1] = "vs_1_1";
                        }
                        else
                        {
                            Profiles[1] = "ps_2_0";
                        }
                    }
                    break;
                case ShaderMode.SM_CG:
                    {
                        Profiles[0] = "profiles";

                        if (GPUP == GpuProgram.GPUP_VERTEX)
                        {
                            Profiles[1] = "vs_1_1 arbvp1";
                        }
                        else
                        {
                            Profiles[1] = "ps_2_0 arbfp1 fp20";
                        }
                    }
                    break;
                case ShaderMode.SM_GLSL:
                    ///not yet supported?
                    break;
            }//end switch shadermode

            GpuProgramType GpuPtype;
            if (GPUP == GpuProgram.GPUP_VERTEX)
            {
                GpuPtype = GpuProgramType.Vertex;
            }
            else
            {
                GpuPtype = GpuProgramType.Fragment;
            }

            HighLevelGpuProgram HLGpuProgram =
                HighLevelGpuProgramManager.Instance.CreateProgram(
                Name,
                "HYDRAX_RESOURCE_GROUP",
                ShaderModesStr[(int)SM],
                GpuPtype);

            HLGpuProgram.Source = Data;
            HLGpuProgram.SetParam("entry_point", EntryPoint);
            HLGpuProgram.SetParam(Profiles[0], Profiles[1]);
            HLGpuProgram.Load();

            return true;

        }
        #endregion

        #region - GetMaterial -
        /// <summary>
        /// Get's Material.
        /// </summary>
        /// <param name="Material">Material you want to get.</param>
        /// <returns>Material to get.</returns>
        public Material GetMaterial(MaterialType Material)
        {
            return mMaterials[(int)Material];
        }
        #endregion

        #region - GetCompositor -
        /// <summary>
        /// Get's Compositor.
        /// </summary>
        /// <param name="Compositor">Compositor you want to get.</param>
        /// <returns>Compositor to get.</returns>
        public Compositor GetCompositor(CompositorType Compositor)
        {
            return mCompositors[(int)Compositor];
        }
        #endregion

        #region - IsCompositorEnabled -
        /// <summary>
        /// Is Compositor enabled?
        /// </summary>
        /// <param name="Compositor">Compositor to check</param>
        /// <returns>true if it's enabled.</returns>
        public bool IsCompositorEnable(CompositorType Compositor)
        {
            return mCompositorEnable[(int)Compositor];
        }
        #endregion

        #region -SetCompositorEnabled -
        /// <summary>
        /// Set's a compositor enabled/disabled
        /// </summary>
        /// <param name="Compositor">Compositor to change</param>
        /// <param name="Enable">true to enable, false to disable</param>
        public void SetCompositorEnabled(CompositorType Compositor, bool Enable)
        {
            AG.Compositor Comp = mCompositors[(int)Compositor];
            if (Comp == null)
                return;

            CompositorManager.Instance.SetCompositorEnabled(mHydrax.Viewport, Comp.Name, Enable);
            mCompositorEnable[(int)Compositor] = Enable;
        }
        #endregion

        #region - AddDepthTechnique -
        /// <summary>
        /// Add depth technique to an especified material
        /// </summary>
        /// <param name="Technique">Technique where depth technique will be added</param>
        /// <param name="AutoUpdate">The technique will be automatically updated when water parameters change</param>
        /// <remarks>Call it after Hydrax::create()/Hydrax::setComponents(...)
        /// The technique will be automatically updated when water parameters change if parameter AutoUpdate == true
        /// Add depth technique when a material is not an Ogre::Entity, such terrains, PLSM2 materials, etc.
        /// This depth technique will be added with "HydraxDepth" scheme in ordeto can use it in the Depth RTT. 
        /// </remarks>
        public void AddDepthTechnique(Technique Technique, bool AutoUpdate)
        {
            if (!AG.MaterialManager.Instance.ResourceExists(_def_Depth_Material_Name))
            {
                CreateDepthMaterial(mComponents, this.LastOptions);
            }
            Technique.RemoveAllPasses();
            Technique.CreatePass();
            Technique.Name = "_Hydrax_Depth_Technique";
            Technique.SchemeName = "HydraxDepth";

            Pass DM_Technique_Pass0 = Technique.GetPass(0);

            DM_Technique_Pass0.VertexProgramName = _def_Depth_Shader_VP_Name;
            DM_Technique_Pass0.FragmentProgramName = _def_Depth_Shader_FP_Name;

            GpuProgramParameters VP_Parameters = DM_Technique_Pass0.VertexProgramParameters;
            GpuProgramParameters FP_Parameters = DM_Technique_Pass0.FragmentProgramParameters;

            VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
            VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);
            VP_Parameters.SetNamedConstant("uPlaneYPos", mHydrax.Position.y);

            FP_Parameters.SetNamedConstant("uDepthLimit", 1 / mHydrax.DepthLimit);

            if (IsComponent(mComponents, HydraxComponent.Caustics))
            {
                FP_Parameters.SetNamedConstant("uCausticsScale", mHydrax.CausticsScale);
                FP_Parameters.SetNamedConstant("uCausticsEnd", mHydrax.CausticsEnd);

                TextureUnitState TUS_Caustics = DM_Technique_Pass0.CreateTextureUnitState("Caustics_1.bmp");
				TUS_Caustics.SetTextureAddressingMode( TextureAddressing.Wrap );
                //TUS_Caustics.SetAnimatedTextureName("Caustics.bmp", 32, 1.5f);
            }

            if (AutoUpdate)
            {
                mDepthTechniques.Add(Technique);
            }
        }

        /// <summary>
        /// Add depth technique to an especified material
        /// </summary>
        /// <param name="Technique">Technique where depth technique will be added</param>
        public void AddDepthTechnique(Technique Technique)
        {
            AddDepthTechnique(Technique, true);
        }
        #endregion

        #region - AddDepthTextureTechnique -
        /// <summary>
        /// Add depth texture technique to an especified material
        /// </summary>
        /// <param name="Technique">Technique where depth technique will be added</param>
        /// <param name="TextureName">Texture name</param>
        /// <param name="AlphaChannel">"x","y","z","w" or "r","g","b","a" (Channel where alpha information is stored)</param>
        /// <param name="AutoUpdate">The technique will be automatically updated when water parameters change</param>
        /// <remarks>Call it after Hydrax::create()/Hydrax::setComponents(...)
        /// The technique will be automatically updated when water parameters change if parameter AutoUpdate == true
        /// Add depth technique when a material is not an Ogre::Entity, such terrains, PLSM2 materials, etc.
        /// This depth technique will be added with "HydraxDepth" scheme in ordeto can use it in the Depth RTT. 
        /// </remarks>
        public void AddDepthTextureTechnique(Technique Technique, string TextureName,
            string AlphaChannel, bool AutoUpdate)
        {
            if (!HighLevelGpuProgramManager.Instance.ResourceExists(_def_DepthTexture_Shader_VP_Name + AlphaChannel))
            {
                CreateDepthTextureGPUPrograms(mComponents, mOptions, AlphaChannel);
            }
            Technique.RemoveAllPasses();
            Technique.CreatePass();
            Technique.Name = "_Hydrax_DepthTexture_Technique";
            Technique.SchemeName = "HydraxDepth";

            Pass DM_Technique_Pass0 = Technique.GetPass(0);

            //alpha channel will be stored in pass 0 name
            DM_Technique_Pass0.Name = AlphaChannel;

            DM_Technique_Pass0.VertexProgramName = _def_DepthTexture_Shader_VP_Name + AlphaChannel;
            DM_Technique_Pass0.FragmentProgramName = _def_DepthTexture_Shader_FP_Name + AlphaChannel;

            DM_Technique_Pass0.SetSceneBlending(SceneBlendType.TransparentAlpha);
            DM_Technique_Pass0.DepthCheck = true;
            DM_Technique_Pass0.DepthWrite = false;

            GpuProgramParameters VP_Parameters = DM_Technique_Pass0.VertexProgramParameters;
            GpuProgramParameters FP_Parameters = DM_Technique_Pass0.FragmentProgramParameters;

            VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
            VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);
            VP_Parameters.SetNamedConstant("uPlaneYPos", mHydrax.Position.y);

            FP_Parameters.SetNamedConstant("uDepthLimit", 1 / mHydrax.DepthLimit);

            if(IsComponent(mComponents, HydraxComponent.Caustics))
            {
                FP_Parameters.SetNamedConstant("uCausticsScale", mHydrax.CausticsScale);
                FP_Parameters.SetNamedConstant("uCausticsEnd", mHydrax.CausticsEnd);

                TextureUnitState TUS_Caustics = DM_Technique_Pass0.CreateTextureUnitState("Caustics.bmp");
                TUS_Caustics.Name = "Caustics";
                TUS_Caustics.SetTextureAddressingMode( TextureAddressing.Wrap );
                TUS_Caustics.SetAnimatedTextureName("Caustics.bmp", 32, 1.5f);
            }

            TextureUnitState TUS_AlphaTex = DM_Technique_Pass0.CreateTextureUnitState(TextureName);
            TUS_AlphaTex.Name = "_DetphTexture_Hydrax";
			TUS_AlphaTex.SetTextureAddressingMode( TextureAddressing.Clamp );

            if (AutoUpdate)
            {
                mDepthTechniques.Add(Technique);
            }
        }
        /// <summary>
        /// Add depth texture technique to an especified material
        /// </summary>
        /// <param name="Technique">Technique where depth technique will be added</param>
        /// <param name="TextureName">Texture name</param>
        /// <param name="AlphaChannel">"x","y","z","w" or "r","g","b","a" (Channel where alpha information is stored)</param>
        public void AddDepthTextureTechnique(Technique Technique, string TextureName,
            string AlphaChannel)
        {
            AddDepthTextureTechnique(Technique, TextureName, AlphaChannel, true);
        }
        /// <summary>
        /// Add depth texture technique to an especified material
        /// </summary>
        /// <param name="Technique">Technique where depth technique will be added</param>
        /// <param name="TextureName">Texture name</param>
        public void AddDepthTextureTechnique(Technique Technique, string TextureName)
        {
            AddDepthTextureTechnique(Technique, TextureName, "w", true);
        }
        #endregion

        #region - SetGpuProgramParameter (float) -
        /// <summary>
        /// Set gpu program float parameter
        /// </summary>
        /// <param name="GpUP">Gpu program type (Vertex/Fragment)</param>
        /// <param name="MType">Water/Depth material</param>
        /// <param name="Name">param name</param>
        /// <param name="Value">value</param>
        public void SetGpuProgramParameter(GpuProgram GpUP, MaterialType MType, string Name,
            float Value)
        {
            if (!mIsCreated)
            {
                return;
            }

            GpuProgramParameters Parameters = null;

            switch (GpUP)
            {
                case GpuProgram.GPUP_VERTEX:
                    {
                        Parameters = mMaterials[(int)MType].GetTechnique(0).GetPass(0).VertexProgramParameters;
                    }
                    break;
                case GpuProgram.GPUP_FRAGMENT:
                    {
                        Parameters = mMaterials[(int)MType].GetTechnique(0).GetPass(0).FragmentProgramParameters;
                    }
                    break;
            }

            Parameters.SetNamedConstant(Name, Value);

            if (MType == MaterialType.MAT_DEPTH)
            {
                foreach (Technique TechIt in mDepthTechniques)
                {
                    if (TechIt == null)
                    {
                        mDepthTechniques.Remove(TechIt);
                        continue;
                    }

                    switch (GpUP)
                    {
                        case GpuProgram.GPUP_VERTEX:
                            {
                                Parameters = TechIt.GetPass(0).VertexProgramParameters;
                            }
                            break;
                        case GpuProgram.GPUP_FRAGMENT:
                            {
                                Parameters = TechIt.GetPass(0).FragmentProgramParameters;
                            }
                            break;
                    }

                    Parameters.SetNamedConstant(Name, Value);
                }//end foreach
            }
        }
        #endregion

        #region - SetGpuProgramParameter (Vector2) -
        /// <summary>
        /// Set gpu program Vector2 parameter
        /// </summary>
        /// <param name="GpUP">Gpu program type (Vertex/Fragment)</param>
        /// <param name="MType">Water/Depth material</param>
        /// <param name="Name">param name</param>
        /// <param name="Value">value</param>
        public void SetGpuProgramParameter(GpuProgram GpUP, MaterialType MType, string Name,
            Vector2 Value)
        {
            if (!mIsCreated)
            {
                return;
            }

            GpuProgramParameters Parameters = null;

            switch (GpUP)
            {
                case GpuProgram.GPUP_VERTEX:
                    {
                        Parameters = mMaterials[(int)MType].GetTechnique(0).GetPass(0).VertexProgramParameters;
                    }
                    break;
                case GpuProgram.GPUP_FRAGMENT:
                    {
                        Parameters = mMaterials[(int)MType].GetTechnique(0).GetPass(0).FragmentProgramParameters;
                    }
                    break;
            }

            float[] Value_ = { Value.x, Value.y };

            if (MType == MaterialType.MAT_DEPTH)
            {
                foreach (Technique TechIt in mDepthTechniques)
                {
                    if (TechIt == null)
                    {
                        mDepthTechniques.Remove(TechIt);
                        continue;
                    }

                    switch (GpUP)
                    {
                        case GpuProgram.GPUP_VERTEX:
                            {
                                Parameters = TechIt.GetPass(0).VertexProgramParameters;
                            }
                            break;
                        case GpuProgram.GPUP_FRAGMENT:
                            {
                                Parameters = TechIt.GetPass(0).FragmentProgramParameters;
                            }
                            break;
                    }

					Parameters.SetNamedConstant( Name, Value_, Value_.Length );
                }//end foreach
            }
        }
        #endregion

        #region - SetGpuProgramParameter (Vector3) -
        /// <summary>
        /// Set gpu program Vector3 parameter
        /// </summary>
        /// <param name="GpUP">Gpu program type (Vertex/Fragment)</param>
        /// <param name="MType">Water/Depth material</param>
        /// <param name="Name">param name</param>
        /// <param name="Value">value</param>
        public void SetGpuProgramParameter(GpuProgram GpUP, MaterialType MType, string Name,
            Vector3 Value)
        {
            if (!mIsCreated)
            {
                return;
            }

            GpuProgramParameters Parameters = null;

            switch (GpUP)
            {
                case GpuProgram.GPUP_VERTEX:
                    {
                        Parameters = mMaterials[(int)MType].GetTechnique(0).GetPass(0).VertexProgramParameters;
                    }
                    break;
                case GpuProgram.GPUP_FRAGMENT:
                    {
                        Parameters = mMaterials[(int)MType].GetTechnique(0).GetPass(0).FragmentProgramParameters;
                    }
                    break;
            }

            Parameters.SetNamedConstant(Name, Value);

            if (MType == MaterialType.MAT_DEPTH)
            {
                foreach (Technique TechIt in mDepthTechniques)
                {
                    if (TechIt == null)
                    {
                        mDepthTechniques.Remove(TechIt);
                        continue;
                    }

                    switch (GpUP)
                    {
                        case GpuProgram.GPUP_VERTEX:
                            {
                                Parameters = TechIt.GetPass(0).VertexProgramParameters;
                            }
                            break;
                        case GpuProgram.GPUP_FRAGMENT:
                            {
                                Parameters = TechIt.GetPass(0).FragmentProgramParameters;
                            }
                            break;
                    }

                    Parameters.SetNamedConstant(Name, Value);
                }//end foreach
            }
        }
        #endregion

        #region - IsComponent -
        /// <summary>
        /// Is component in the given list?
        /// </summary>
        /// <param name="List">Components list</param>
        /// <param name="ToCheck">Component to check</param>
        /// <returns>true if the component is in the given list.</returns>
        public bool IsComponent(HydraxComponent List, HydraxComponent ToCheck)
        {
            if ((List & ToCheck) == ToCheck)
            {
                return true;
            }
            if ((List & HydraxComponent.All) == ToCheck)
            {
                return true;
            }
            if ((List & HydraxComponent.None) == ToCheck)
            {
                return true;
            }
            return false;
        }
        #endregion

        #endregion

        #region - Private -

        #region - CreateWaterMaterial -
        /// <summary>
        /// Create water material
        /// </summary>
        /// <param name="Components">Components of the shader</param>
        /// <param name="Options"> Material options</param>
        /// <returns>true if succed.</returns>
        private bool CreateWaterMaterial(HydraxComponent Components, Options Options)
        {
            bool cDepth = IsComponent(Components, HydraxComponent.Depth);
            bool cSmooth = IsComponent(Components, HydraxComponent.Smooth);
            bool cSun = IsComponent(Components, HydraxComponent.Sun);
            bool cFoam = IsComponent(Components, HydraxComponent.Foam);
            bool cCaustics = IsComponent(Components, HydraxComponent.Caustics);

            string VertexProgramData = "", FragmentProgramData = "";

            #region - VertexProgram -
            switch (Options.NM)
            {
                case NormalMode.NM_TEXTURE:
                    {
                        #region - ShaderMode -
                        switch (Options.SM)
                        {
                            case ShaderMode.SM_HLSL:
                                //fall through
                            case ShaderMode.SM_CG:
                                {
                                    VertexProgramData +=
                                       "void main_vp(\n" +
						               // IN
                          	           "float4 iPosition         : POSITION,\n" +
                           	           "float2 iUv               : TEXCOORD0,\n" +
                           	           // OUT
                           	           "out float4 oPosition      : POSITION,\n" +
							           "out float4 oPosition_     : TEXCOORD0,\n" +
                                       "out float2 oUvNoise       : TEXCOORD1,\n" +
                                       "out float4 oUvProjection  : TEXCOORD2,\n";
						               if (cFoam)
							           {
							               VertexProgramData += "out float4 oWorldPosition : TEXCOORD3,\n uniform float4x4         uWorld,\n";
							           }
                                    VertexProgramData += 
                                       // UNIFORM
                                       "uniform float4x4         uWorldViewProj)\n" +
               	                       "{\n" +
                  	                   "oPosition_  = iPosition;\n";
							           if (cFoam)
							           {
							               VertexProgramData += "oWorldPosition = mul(uWorld, iPosition);\n";
							           }
						            VertexProgramData += 
	              	                   "oPosition = mul(uWorldViewProj, iPosition);\n" +
	               	                   // Projective texture coordinates, adjust for mapping
	                	               "float4x4 scalemat = float4x4(0.5,   0,   0, 0.5,"+
	                                              	                 "0,-0.5,   0, 0.5,"+
	                							  	                 "0,   0, 0.5, 0.5,"+
	                							  	                 "0,   0,   0,   1);\n" +
	               	                   "oUvProjection = mul(scalemat, oPosition);\n" +
	               	                   "oUvNoise = iUv;\n" +
               	                       "}\n";
                                }
                                break;
                            case ShaderMode.SM_GLSL:
                                //do nothing
                                break;
                        }
                        #endregion
                    }//end NM_Texture
                    break;
                case NormalMode.NM_VERTEX:
                    {
                        #region - ShaderMode -
                        switch (Options.SM)
                        {
                            case ShaderMode.SM_HLSL:
                            //fall through
                            case ShaderMode.SM_CG:
                                {
                                    VertexProgramData +=
                                        "void main_vp(\n" +
                                                                        // IN
                                       "float4 iPosition         : POSITION,\n" +
                                       "float3 iNormal           : NORMAL,\n" +
                                                                        // OUT
                                       "out float4 oPosition     : POSITION,\n" +
                                       "out float4 oPosition_    : TEXCOORD0,\n" +
                                       "out float4 oUvProjection : TEXCOORD1,\n" +
                                       "out float3 oNormal       : TEXCOORD2,\n";
                                    if (cFoam)
                                    {
                                        VertexProgramData += "out float4 oWorldPosition : TEXCOORD3,\n uniform float4x4         uWorld,\n";
                                    }
                                    VertexProgramData +=
                                        // UNIFORM
                                           "uniform float4x4         uWorldViewProj)\n" +
                                    "{\n" +
                                        "oPosition_  = iPosition;\n";
                                    if (cFoam)
                                    {
                                        VertexProgramData += "oWorldPosition = mul(uWorld, iPosition);\n";
                                    }
                                    VertexProgramData +=
                                        "oPosition = mul(uWorldViewProj, iPosition);\n" +
                                        // Projective texture coordinates, adjust for mapping
                                        "float4x4 scalemat = float4x4(0.5,   0,   0, 0.5," +
                                                                     "0,-0.5,   0, 0.5," +
                                                                     "0,   0, 0.5, 0.5," +
                                                                     "0,   0,   0,   1);\n" +
                                        "oUvProjection = mul(scalemat, oPosition);\n" +
                                        "oNormal = normalize(iNormal);\n" +
                                     "}\n";
                                }
                                break;
                            case ShaderMode.SM_GLSL:
                                //do nothing
                                break;
                        }
                        #endregion
                    }//end case NM_VERTEX
                    break;
                case NormalMode.NM_RTT:
                    {
                        #region - ShaderMode -
                        switch (Options.SM)
                        {
                            case ShaderMode.SM_HLSL:
                            //fall through
                            case ShaderMode.SM_CG:
                                {
                                    VertexProgramData +=
                                        "void main_vp(\n" +
                                        // IN
                                        "float4 iPosition         : POSITION,\n" +
                                        // OUT
                                       "out float4 oPosition     : POSITION,\n" +
                                       "out float4 oPosition_    : TEXCOORD0,\n" +
                                       "out float4 oUvProjection : TEXCOORD1,\n";
                                    if (cFoam)
                                    {
                                        VertexProgramData += "out float4 oWorldPosition : TEXCOORD2,\n uniform float4x4         uWorld,\n";
                                    }
                                    VertexProgramData +=
                                        // UNIFORM
                                           "uniform float4x4         uWorldViewProj)\n" +
                                    "{\n" +
                                        "oPosition_  = iPosition;\n";
                                    if (cFoam)
                                    {
                                        VertexProgramData += "oWorldPosition = mul(uWorld, iPosition);\n";
                                    }
                                    VertexProgramData +=
                                        "oPosition = mul(uWorldViewProj, iPosition);\n" +
                                        // Projective texture coordinates, adjust for mapping
                                        "float4x4 scalemat = float4x4(0.5,   0,   0, 0.5," +
                                                                     "0,-0.5,   0, 0.5," +
                                                                     "0,   0, 0.5, 0.5," +
                                                                     "0,   0,   0,   1);\n" +
                                        "oUvProjection = mul(scalemat, oPosition);\n" +
                                     "}\n";
                                }
                                break;
                            case ShaderMode.SM_GLSL:
                                //do nothing
                                break;
                        }
                        #endregion
                    }//end case NM_RTT
                    break;

            }//end switch normal mode
            #endregion

            #region - FragmentProgram -
            switch (Options.NM)
            {
                case NormalMode.NM_TEXTURE:
                    //fall through
                case NormalMode.NM_VERTEX:
                    //fall through
                case NormalMode.NM_RTT:
                    {
                        #region - ShaderMode -
                        switch (Options.SM)
                        {
                            case ShaderMode.SM_HLSL:
                                //fall trhough
                            case ShaderMode.SM_CG:
                                {
                                    FragmentProgramData += 
							 "float3 expand(float3 v)\n" +
						    "{\n" +
	                            "return (v - 0.5) * 2;\n" + 
							"}\n\n" +

							"void main_fp(" +
							    // IN
                                "float4 iPosition      : TEXCOORD0,\n";
						int TEXCOORDNUM = 1;
						if (Options.NM == NormalMode.NM_TEXTURE)
						{
							FragmentProgramData +=
								"float2 iUvNoise      : TEXCOORD" + TEXCOORDNUM.ToString() + ",\n";
							TEXCOORDNUM++;

						}
						FragmentProgramData +=
                                "float4 iUvProjection : TEXCOORD" + TEXCOORDNUM.ToString() + ",\n";
						TEXCOORDNUM++;
						if (Options.NM == NormalMode.NM_VERTEX)
						{
							FragmentProgramData += 
							    "float4 iNormal       : TEXCOORD" + TEXCOORDNUM.ToString() + ",\n";
							TEXCOORDNUM++;
						}
						if (cFoam)
						{
							FragmentProgramData += 
							    "float4 iWorldPosition  : TEXCOORD" + TEXCOORDNUM.ToString() + ",\n";
						}
						FragmentProgramData +=
	
	                            // OUT
	                            "out float4 oColor    : COLOR,\n" +
	                            // UNIFORM
	                            "uniform float3       uEyePosition,\n" +
	                            "uniform float        uFullReflectionDistance,\n" +
	                            "uniform float        uGlobalTransparency,\n" +
	                            "uniform float        uNormalDistortion,\n";

						if (cDepth)
						{
							FragmentProgramData += 
								"uniform float3       uWaterColor,\n";
						}
						if (cSmooth)
						{
							FragmentProgramData += 
								"uniform float        uSmoothPower,\n";
						}
						if (cSun)
						{
							FragmentProgramData += 
							    "uniform float3       uSunPosition,\n" +
	                            "uniform float        uSunStrength,\n" +
	                            "uniform float        uSunArea,\n" +
	                            "uniform float3       uSunColor,\n";
						}
						if (cFoam)
						{
							FragmentProgramData += 
							    "uniform float        uFoamRange,\n" +
							    "uniform float        uFoamMaxDistance,\n" +
	                            "uniform float        uFoamScale,\n" +
	                            "uniform float        uFoamStart,\n" +
	                            "uniform float        uFoamTransparency,\n";
						}
						if (cCaustics)
						{
							FragmentProgramData +=
								"uniform float        uCausticsPower,\n";
						}

						int TexNum = 0;

						if (Options.NM == NormalMode.NM_TEXTURE || Options.NM == NormalMode.NM_RTT)
						{
						    FragmentProgramData += 
							   "uniform sampler2D    uNormalMap       : register(s" + TexNum.ToString() + "),\n";
							TexNum++;
						}

						FragmentProgramData += 
						       "uniform sampler2D    uReflectionMap   : register(s" + TexNum.ToString() + "),\n" +
	                           "uniform sampler2D    uRefractionMap   : register(s" + (TexNum+1).ToString() + "),\n";

						TexNum += 2;

						if (cDepth)
						{
							FragmentProgramData += 
								"uniform sampler2D    uDepthMap        : register(s" + TexNum.ToString() + "),\n";
							TexNum++;
						}

						FragmentProgramData += 
								"uniform sampler1D    uFresnelMap      : register(s" + TexNum.ToString() + ")\n";
						TexNum++;

						if (cFoam)
						{
							FragmentProgramData +=
							 ",uniform sampler2D    uFoamMap         : register(s" + TexNum.ToString() + ")\n";
						}

						FragmentProgramData += 
							                                           ")\n" +
							"{\n"     +
							    "float2 ProjectionCoord = iUvProjection.xy / iUvProjection.w;\n" +
                                "float3 camToSurface = iPosition.xyz - uEyePosition;\n" +
                                "float additionalReflection=camToSurface.x*camToSurface.x+camToSurface.z*camToSurface.z;\n";

						if (cFoam)
						{
							// Calculate the foam visibility as a function fo distance specified by user
							FragmentProgramData +=
								"float foamVisibility=1.0f-saturate(additionalReflection/uFoamMaxDistance);\n";
						}

						FragmentProgramData += 
							
							    "additionalReflection/=uFullReflectionDistance;\n" +
								"camToSurface=normalize(-camToSurface);\n";
						if (Options.NM == NormalMode.NM_TEXTURE)
						{
							FragmentProgramData += 
								"float3 pixelNormal = tex2D(uNormalMap,iUvNoise);\n" +
								// Inverte y with z, because at creation our local normal to the plane was z
								"pixelNormal.yz=pixelNormal.zy;\n" +
								// Remap from [0,1] to [-1,1]
								"pixelNormal.xyz=expand(pixelNormal.xyz);\n";
						}
						else if (Options.NM == NormalMode.NM_VERTEX)
						{
							FragmentProgramData += 
								"float3 pixelNormal = iNormal;\n";
						}
						else // NM_RTT
						{
							FragmentProgramData +=
								"float3 pixelNormal = 2.0*tex2D(uNormalMap, ProjectionCoord.xy) - 1.0;\n";
						}
						FragmentProgramData += 
								"float2 pixelNormalModified = uNormalDistortion*pixelNormal.zx;\n";
						if (Options.NM == NormalMode.NM_TEXTURE || Options.NM == NormalMode.NM_RTT)
						{
							FragmentProgramData +=
								"float dotProduct=dot(camToSurface,pixelNormal);\n";
						}
						else
						{
							FragmentProgramData +=
								"float dotProduct=dot(-camToSurface,pixelNormal);\n";
						}
						FragmentProgramData += 
							
								"dotProduct=saturate(dotProduct);\n" +
								"float fresnel = tex1D(uFresnelMap,dotProduct);\n" +
								// Add additional reflection and saturate
								"fresnel+=additionalReflection;\n" +
								"fresnel=saturate(fresnel);\n" +
								// Decrease the transparency and saturate
								"fresnel-=uGlobalTransparency;\n" +
                                "fresnel=saturate(fresnel);\n" +
								// Get the reflection/refraction pixels. Make sure to disturb the texcoords by pixelnormal
								"float3 reflection=tex2D(uReflectionMap,ProjectionCoord.xy+pixelNormalModified);\n" +
								"float3 refraction=tex2D(uRefractionMap,ProjectionCoord.xy-pixelNormalModified);\n";

						if (cDepth)
						{
							if (cCaustics)
						    {
								FragmentProgramData += 
								"float2 depth = tex2D(uDepthMap,ProjectionCoord.xy-pixelNormalModified).rg;\n" +
								"refraction *= 1+depth.y*uCausticsPower;\n" +
								"refraction = lerp(uWaterColor,refraction,depth.x);\n";
						    }
							else
							{
								FragmentProgramData += 
								"float depth = tex2D(uDepthMap,ProjectionCoord.xy-pixelNormalModified).r;\n" +
								"refraction = lerp(uWaterColor,refraction,depth);\n";
							}
						}

						FragmentProgramData += 
								"oColor = float4(lerp(refraction,reflection,fresnel),1);\n";

						if (cSun)
						{
							FragmentProgramData += 
							    "float3 relfectedVector = normalize(reflect(-camToSurface,pixelNormal.xyz));\n" +
								"float3 surfaceToSun=normalize(uSunPosition-iPosition.xyz);\n" +
								"float3 sunlight = uSunStrength*pow(saturate(dot(relfectedVector,surfaceToSun)),uSunArea)*uSunColor;\n" +
								"oColor.xyz+=sunlight;\n";
						}

						if (cFoam)
						{
							FragmentProgramData += 
							    "float hmap = iPosition.y/uFoamRange*foamVisibility;\n" +
								"float2 foamTex=iWorldPosition.xz*uFoamScale+pixelNormalModified;\n" +
								"float foam=tex2D(uFoamMap,foamTex).r;\n" +
								"float foamTransparency=saturate(hmap-uFoamStart)*uFoamTransparency;\n" +
								"oColor.xyz=lerp(oColor.xyz,1,foamTransparency*foam);\n";
						}

						if (cSmooth)
						{
							FragmentProgramData += 
								"oColor.xyz = lerp(tex2D(uRefractionMap,ProjectionCoord.xy).xyz,oColor.xyz,saturate((1-tex2D(uDepthMap,ProjectionCoord.xy).r)*uSmoothPower));\n";
						}

						FragmentProgramData +=
							"}\n";
                                }
                                break;
                            case ShaderMode.SM_GLSL:
                                //do nothing
                                break;
                        }
                        #endregion
                    }
                    break;
            }
            #endregion

            #region - BuildMaterial -
            //build our material
            Material WaterMaterial = GetMaterial(MaterialType.MAT_WATER);
            WaterMaterial = (Material)AG.MaterialManager.Instance.Create(
                _def_Water_Material_Name,
                "HYDRAX_RESOURCE_GROUP");
            mMaterials[(int)MaterialType.MAT_WATER] = WaterMaterial;

            Pass WM_Technique0_Pass0 = WaterMaterial.GetTechnique(0).GetPass(0);
            WM_Technique0_Pass0.CullingMode = CullingMode.None;
            WM_Technique0_Pass0.DepthWrite = true;

            string[] GpuProgramsData = {VertexProgramData, FragmentProgramData};
            string[] GpuProgramNames   = {_def_Water_Shader_VP_Name, _def_Water_Shader_FP_Name};
		    string[] EntryPoints       = {"main_vp", "main_fp"};

            FillGpuProgramsToPass(WM_Technique0_Pass0, GpuProgramNames, Options.SM, EntryPoints, GpuProgramsData);

            GpuProgramParameters VP_Parameters = WM_Technique0_Pass0.VertexProgramParameters;
            GpuProgramParameters FP_Parameters = WM_Technique0_Pass0.FragmentProgramParameters;
            VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
            if (cFoam)
            {
                VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);
            }
            FP_Parameters.SetNamedAutoConstant("uEyePosition", GpuProgramParameters.AutoConstantType.CameraPositionObjectSpace, 0);

            FP_Parameters.SetNamedConstant("uFullReflectionDistance", mHydrax.FullReflectionDistance);
            FP_Parameters.SetNamedConstant("uGlobalTransparency", mHydrax.GlobalTransparency);
            FP_Parameters.SetNamedConstant("uNormalDistortion", mHydrax.NormalDistortion);

            if (cDepth)
            {
                FP_Parameters.SetNamedConstant("uWaterColor", mHydrax.WaterColor);
            }
            if (cSmooth)
            {
                FP_Parameters.SetNamedConstant("uSmoothPower", mHydrax.SmoothPower);
            }
            if (cSun)
            {
                FP_Parameters.SetNamedConstant("uSunPosition", mHydrax.Mesh.GetObjectSpacePosition(mHydrax.SunPosition));
                FP_Parameters.SetNamedConstant("uSunStrength", mHydrax.SunStrenght);
                FP_Parameters.SetNamedConstant("uSunArea", mHydrax.SunArea);
                FP_Parameters.SetNamedConstant("uSunColor", mHydrax.SunColor);
            }
            if (cFoam)
            {
                FP_Parameters.SetNamedConstant("uFoamRange", mHydrax.Mesh.MeshOptions.MeshStrength);
                FP_Parameters.SetNamedConstant("uFoamMaxDistance", mHydrax.FoamMaxDistance);
                FP_Parameters.SetNamedConstant("uFoamScale",mHydrax.FoamScale);     
                FP_Parameters.SetNamedConstant("uFoamStart",mHydrax.FoamStart);
                FP_Parameters.SetNamedConstant("uFoamTransparency", mHydrax.FoamTransparency);
            }
            if (cCaustics)
            {
                FP_Parameters.SetNamedConstant("uCausticsPower", mHydrax.CausticsPower);
            }

            if (Options.NM == NormalMode.NM_TEXTURE || Options.NM == NormalMode.NM_RTT)
            {
                WM_Technique0_Pass0.CreateTextureUnitState("HydraxNormalMap").SetTextureAddressingMode( TextureAddressing.Wrap );
            }

            WM_Technique0_Pass0.CreateTextureUnitState("HydraxReflectionMap").SetTextureAddressingMode( TextureAddressing.Clamp );
            WM_Technique0_Pass0.CreateTextureUnitState("HydraxRefractionMap").SetTextureAddressingMode( TextureAddressing.Clamp );

            if (cDepth)
            {
                WM_Technique0_Pass0.CreateTextureUnitState("HydraxDepthMap").SetTextureAddressingMode( TextureAddressing.Clamp );
            }

            WM_Technique0_Pass0.CreateTextureUnitState("Fresnel.png").SetTextureAddressingMode( TextureAddressing.Clamp );

            if (cFoam)
            {
                WM_Technique0_Pass0.CreateTextureUnitState("Foam.png").SetTextureAddressingMode( TextureAddressing.Wrap );
            }
            #endregion

            WaterMaterial.ReceiveShadows = false;
            WaterMaterial.Load();

            return true;
        }
        #endregion

        #region - CreateDepthMaterial -
        /// <summary>
        /// Create depth material
        /// </summary>
        /// <param name="Components">Components of the shader</param>
        /// <param name="Options"> Material options</param>
        /// <returns>true if succed.</returns>
        private bool CreateDepthMaterial(HydraxComponent Components, Options Options)
        {
            bool cCaustics = IsComponent(Components, HydraxComponent.Caustics);

            string VertexProgramData = "", FragmentProgramData = "";

            #region - VertexProgram -
            switch (Options.SM)
            {
                case ShaderMode.SM_HLSL:
                    //fall trough
                case ShaderMode.SM_CG:
                    {
                        //no caustics
                        if (!cCaustics)
                        {
                            VertexProgramData +=
                        "void main_vp(\n" +
                                // IN
                            "float4 iPosition         : POSITION,\n" +
                                // OUT 
                            "out float4 oPosition     : POSITION,\n" +
                            "out float  oPosition_    : TEXCOORD0,\n" +
                                // UNIFORM
                            "uniform float            uPlaneYPos,\n" +
                            "uniform float4x4         uWorld,\n" +
                            "uniform float4x4         uWorldViewProj)\n" +
                        "{\n" +
                           "oPosition = mul(uWorldViewProj, iPosition);\n" +
                           "oPosition_ = mul(uWorld, iPosition).y;\n" +
                           "oPosition_-=uPlaneYPos;\n" +
                        "}\n";
                        }
                        //Caustics
                        else
                        {
                            VertexProgramData +=
                        "void main_vp(\n" +
                                // IN
                            "float4 iPosition         : POSITION,\n" +
                                // OUT 
                            "out float4 oPosition     : POSITION,\n" +
                            "out float  oPosition_    : TEXCOORD0,\n" +
                            "out float2 oUvWorld      : TEXCOORD1,\n" +
                                // UNIFORM
                            "uniform float            uPlaneYPos,\n" +
                            "uniform float4x4         uWorld,\n" +
                            "uniform float4x4         uWorldViewProj)\n" +
                        "{\n" +
                           "oPosition = mul(uWorldViewProj, iPosition);\n" +
                           "float3 wPos = mul(uWorld, iPosition);\n" +
                           "oPosition_ = wPos.y;\n" +
                           "oPosition_-=uPlaneYPos;\n" +
                           "oUvWorld = wPos.xz;\n" +
                        "}\n";
                        }
                    }
                    break;
                case ShaderMode.SM_GLSL:
                    {
                        //not supported yet
                    }
                    break;
            }
            #endregion

            #region - FragmentProgram -
            switch (Options.SM)
            {
                case ShaderMode.SM_HLSL:
                    //fall trough
                case ShaderMode.SM_CG:
                    {
                       // No caustics
					if (!cCaustics)
					{
						FragmentProgramData += 
								"void main_fp(\n" +
							    // IN
							    "float  iPosition     : TEXCOORD0,\n" +
								// OUT
								"out float4 oColor    : COLOR,\n" +
								// UNIFORM
								"uniform float        uDepthLimit)\n" +
								"{\n" +
							    "float pixelYDepth = (iPosition*uDepthLimit+1);\n" +
								"pixelYDepth = saturate(pixelYDepth);\n" +
								"oColor = float4(pixelYDepth,0,0,0);\n" +
								"}\n";
					}
					else // Caustics
					{
						FragmentProgramData += 
								"void main_fp(\n" +
							    // IN
							    "float  iPosition     : TEXCOORD0,\n" +
	                            "float2 iUvWorld      : TEXCOORD1,\n" +
								// OUT
								"out float4 oColor    : COLOR,\n" +
								// UNIFORM
								"uniform float        uDepthLimit,\n" +
								"uniform float        uCausticsScale,\n" +
		                        "uniform float        uCausticsEnd,\n" +
								"uniform sampler2D    uCaustics : register(s0))\n" +
								"{\n" +
							    "float pixelYDepth = (iPosition*uDepthLimit+1);\n" +
								"pixelYDepth = saturate(pixelYDepth);\n" +
								"oColor = float4(pixelYDepth,0,0,0);\n" +
								"oColor.g = saturate(uCausticsEnd-pixelYDepth)*tex2D(uCaustics, iUvWorld/uCausticsScale).r;\n" +
								"}\n";
					}
                    }
                    break;
                case ShaderMode.SM_GLSL:
                    {
                        //not supported yet.
                    }
                    break;
            }
            #endregion

            #region - BuildMaterial -
            Material DepthMaterial = GetMaterial(MaterialType.MAT_DEPTH);
            DepthMaterial = (Material)AG.MaterialManager.Instance.Create(
                _def_Depth_Material_Name,
                "HYDRAX_RESOURCE_GROUP");
            mMaterials[(int)MaterialType.MAT_DEPTH] = DepthMaterial;
            DepthMaterial.GetTechnique(0).SchemeName = "HydraxDepth";

            Pass DM_Technique0_Pass0 = DepthMaterial.GetTechnique(0).GetPass(0);

            string[] GpuProgramsData = {VertexProgramData, FragmentProgramData};
		    string[] GpuProgramNames = {_def_Depth_Shader_VP_Name, _def_Depth_Shader_FP_Name};
		    string[] EntryPoints = {"main_vp", "main_fp"};

            FillGpuProgramsToPass(DM_Technique0_Pass0, GpuProgramNames, Options.SM, EntryPoints, GpuProgramsData);

            GpuProgramParameters VP_Parameters = DM_Technique0_Pass0.VertexProgramParameters;
            GpuProgramParameters FP_Parameters = DM_Technique0_Pass0.FragmentProgramParameters;

            VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix,0);
            VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);
            VP_Parameters.SetNamedConstant("uPlaneYPos", mHydrax.Position.y);

            FP_Parameters.SetNamedConstant("uDepthLimit", 1 / mHydrax.DepthLimit);

            if (cCaustics)
            {
                FP_Parameters.SetNamedConstant("uCausticsScale", mHydrax.CausticsScale);
                FP_Parameters.SetNamedConstant("uCausticsEnd", mHydrax.CausticsEnd);

                TextureUnitState TUS_Caustics = DM_Technique0_Pass0.CreateTextureUnitState("Caustics.bmp");
                TUS_Caustics.SetTextureAddressingMode( TextureAddressing.Wrap );
                TUS_Caustics.SetAnimatedTextureName("Caustics.bmp", 32, 1.5f);
            }

            DepthMaterial.ReceiveShadows = false;
            DepthMaterial.Load();
            
            mMaterials[1] = DepthMaterial;
            #endregion

            return true;
        }
        #endregion

        #region - CreateUnderwaterMaterial -
        /// <summary>
        /// Create underwater material
        /// </summary>
        /// <param name="Components">Components of the shader</param>
        /// <param name="Options"> Material options</param>
        /// <returns>true if succed.</returns>
        private bool CreateUnderwaterMaterial(HydraxComponent Components, Options Options)
        {
            bool cDepth = IsComponent(Components, HydraxComponent.Depth);
            bool cSmooth = IsComponent(Components, HydraxComponent.Smooth);
            bool cSun = IsComponent(Components, HydraxComponent.Sun);
            bool cFoam = IsComponent(Components, HydraxComponent.Foam);
            bool cCaustics = IsComponent(Components, HydraxComponent.Caustics);
            bool cUReflections = IsComponent(Components, HydraxComponent.UnderwaterReflections);

            string VertexProgramData = "", FragmentProgramData = "";

            #region - VertexProgram -
            switch (Options.NM)
            {
                case NormalMode.NM_TEXTURE:
                    {
                        #region - ShaderMode -
                        switch (Options.SM)
                        {
                            case ShaderMode.SM_HLSL:
                            //fall through
                            case ShaderMode.SM_CG:
                                {
                                    VertexProgramData +=
                                "void main_vp(\n" +
                                        // IN
                               "float4 iPosition         : POSITION,\n" +
                               "float2 iUv               : TEXCOORD0,\n" +
                                        // OUT
                               "out float4 oPosition      : POSITION,\n" +
                               "out float4 oPosition_     : TEXCOORD0,\n" +
                               "out float2 oUvNoise       : TEXCOORD1,\n" +
                               "out float4 oUvProjection  : TEXCOORD2,\n";
                                    if (cFoam)
                                    {
                                        VertexProgramData += "out float4 oWorldPosition : TEXCOORD3,\n uniform float4x4         uWorld,\n";
                                    }
                                    VertexProgramData +=
                                        // UNIFORM
                                           "uniform float4x4         uWorldViewProj)\n" +
                                    "{\n" +
                                        "oPosition_  = iPosition;\n";
                                    if (cFoam)
                                    {
                                        VertexProgramData += "oWorldPosition = mul(uWorld, iPosition);\n";
                                    }
                                    VertexProgramData +=
                                        "oPosition = mul(uWorldViewProj, iPosition);\n" +
                                        // Projective texture coordinates, adjust for mapping
                                        "float4x4 scalemat = float4x4(0.5,   0,   0, 0.5," +
                                                                     "0,-0.5,   0, 0.5," +
                                                                     "0,   0, 0.5, 0.5," +
                                                                     "0,   0,   0,   1);\n" +
                                        "oUvProjection = mul(scalemat, oPosition);\n" +
                                        "oUvNoise = iUv;\n" +
                                     "}\n";
                                }
                                break;
                            case ShaderMode.SM_GLSL:
                                {
                                    //not supported yet!
                                }
                                break;
                        }//end shader mode
                        #endregion
                    }//end NM_TEXTURE
                    break;
                case NormalMode.NM_VERTEX:
                    {
                        #region - ShaderMode -
                        switch (Options.SM)
                        {
                            case ShaderMode.SM_HLSL:
                            //fall through
                            case ShaderMode.SM_CG:
                                {
                                    VertexProgramData +=
                        "void main_vp(\n" +
                                        // IN
                               "float4 iPosition         : POSITION,\n" +
                               "float3 iNormal           : NORMAL,\n" +
                                        // OUT
                               "out float4 oPosition     : POSITION,\n" +
                               "out float4 oPosition_    : TEXCOORD0,\n" +
                               "out float4 oUvProjection : TEXCOORD1,\n" +
                               "out float3 oNormal       : TEXCOORD2,\n";
                                    if (cFoam)
                                    {
                                        VertexProgramData += "out float4 oWorldPosition : TEXCOORD3,\n uniform float4x4         uWorld,\n";
                                    }
                                    VertexProgramData +=
                                        // UNIFORM
                                           "uniform float4x4         uWorldViewProj)\n" +
                                    "{\n" +
                                        "oPosition_  = iPosition;\n";
                                    if (cFoam)
                                    {
                                        VertexProgramData += "oWorldPosition = mul(uWorld, iPosition);\n";
                                    }
                                    VertexProgramData +=
                                        "oPosition = mul(uWorldViewProj, iPosition);\n" +
                                        // Projective texture coordinates, adjust for mapping
                                        "float4x4 scalemat = float4x4(0.5,   0,   0, 0.5," +
                                                                     "0,-0.5,   0, 0.5," +
                                                                     "0,   0, 0.5, 0.5," +
                                                                     "0,   0,   0,   1);\n" +
                                        "oUvProjection = mul(scalemat, oPosition);\n" +
                                        "oNormal = normalize(iNormal);\n" +
                                     "}\n";
                                }
                                break;
                            case ShaderMode.SM_GLSL:
                                {
                                    //not supported yet!
                                }
                                break;
                        }
                        #endregion
                    }//end NM_VERTEX
                    break;
                case NormalMode.NM_RTT:
                    {
                        #region - ShaderModer -
                        switch (Options.SM)
                        {
                            case ShaderMode.SM_HLSL:
                            //fall through
                            case ShaderMode.SM_CG:
                                {
                                    VertexProgramData +=
                                "void main_vp(\n" +
                                        // IN
                               "float4 iPosition         : POSITION,\n" +
                                        // OUT
                               "out float4 oPosition     : POSITION,\n" +
                               "out float4 oPosition_    : TEXCOORD0,\n" +
                               "out float4 oUvProjection : TEXCOORD1,\n";
                                    if (cFoam)
                                    {
                                        VertexProgramData += "out float4 oWorldPosition : TEXCOORD2,\n uniform float4x4         uWorld,\n";
                                    }
                                    VertexProgramData +=
                                        // UNIFORM
                                           "uniform float4x4         uWorldViewProj)\n" +
                                    "{\n" +
                                        "oPosition_  = iPosition;\n";
                                    if (cFoam)
                                    {
                                        VertexProgramData += "oWorldPosition = mul(uWorld, iPosition);\n";
                                    }
                                    VertexProgramData +=
                                        "oPosition = mul(uWorldViewProj, iPosition);\n" +
                                        // Projective texture coordinates, adjust for mapping
                                        "float4x4 scalemat = float4x4(0.5,   0,   0, 0.5," +
                                                                     "0,-0.5,   0, 0.5," +
                                                                     "0,   0, 0.5, 0.5," +
                                                                     "0,   0,   0,   1);\n" +
                                        "oUvProjection = mul(scalemat, oPosition);\n" +
                                     "}\n";
                                }
                                break;
                            case ShaderMode.SM_GLSL:
                                {
                                    //not supported yet!
                                }
                                break;
                        }
                        #endregion
                    }//end NM_RTT
                    break;
            }//end normal mode
            #endregion

            #region - FragmentProgram -
            switch (Options.NM)
            {
                case NormalMode.NM_TEXTURE:
                    //fall through
                case NormalMode.NM_VERTEX:
                    //fall through
                case NormalMode.NM_RTT:
                    {
                        #region - ShaderMode -
                        switch (Options.SM)
                        {
                            case ShaderMode.SM_HLSL:
                            case ShaderMode.SM_CG:
                                {
                                    FragmentProgramData +=
                                     "float3 expand(float3 v)\n" +
                                    "{\n" +
                                        "return (v - 0.5) * 2;\n" +
                                    "}\n\n" +

                                    "void main_fp(" +
                                                                        // IN
                                        "float4 iPosition     : TEXCOORD0,\n";
                                    int TEXCOORDNUM = 1;
                                    if (Options.NM == NormalMode.NM_TEXTURE)
                                    {
                                        FragmentProgramData +=
                                            "float2 iUvNoise      : TEXCOORD" + TEXCOORDNUM.ToString() + ",\n";
                                        TEXCOORDNUM++;

                                    }
                                    FragmentProgramData +=
                                            "float4 iUvProjection : TEXCOORD" + TEXCOORDNUM.ToString() + ",\n";
                                    TEXCOORDNUM++;
                                    if (Options.NM == NormalMode.NM_VERTEX)
                                    {
                                        FragmentProgramData +=
                                            "float4 iNormal       : TEXCOORD" + TEXCOORDNUM.ToString() + ",\n";
                                        TEXCOORDNUM++;
                                    }
                                    if (cFoam)
                                    {
                                        FragmentProgramData +=
                                            "float4 iWorldPosition  : TEXCOORD" + TEXCOORDNUM.ToString() + ",\n";
                                    }
                                    FragmentProgramData +=
                                        // OUT
                                            "out float4 oColor    : COLOR,\n" +
                                        // UNIFORM
                                            "uniform float3       uEyePosition,\n" +
                                            "uniform float        uFullReflectionDistance,\n" +
                                            "uniform float        uGlobalTransparency,\n" +
                                            "uniform float        uNormalDistortion,\n";

                                    if ((cDepth && cUReflections) || (!cUReflections))
                                    {
                                        FragmentProgramData +=
                                            "uniform float3       uWaterColor,\n";
                                    }
                                    if (cSun)
                                    {
                                        FragmentProgramData +=
                                            "uniform float3       uSunPosition,\n" +
                                            "uniform float        uSunStrength,\n" +
                                            "uniform float        uSunArea,\n" +
                                            "uniform float3       uSunColor,\n";
                                    }
                                    if (cFoam)
                                    {
                                        FragmentProgramData +=
                                            "uniform float        uFoamRange,\n" +
                                            "uniform float        uFoamMaxDistance,\n" +
                                            "uniform float        uFoamScale,\n" +
                                            "uniform float        uFoamStart,\n" +
                                            "uniform float        uFoamTransparency,\n";
                                    }
                                    if (cCaustics && cUReflections)
                                    {
                                        FragmentProgramData +=
                                           "uniform float        uCausticsPower,\n";
                                    }

                                    int TexNum = 0;

                                    if (Options.NM == NormalMode.NM_TEXTURE || Options.NM == NormalMode.NM_RTT)
                                    {
                                        FragmentProgramData +=
                                           "uniform sampler2D    uNormalMap       : register(s" + TexNum.ToString() + "),\n";
                                        TexNum++;
                                    }

                                    if (cUReflections)
                                    {
                                        FragmentProgramData +=
                                            "uniform sampler2D    uReflectionMap   : register(s" + TexNum.ToString() + "),\n";
                                        TexNum++;
                                    }

                                    FragmentProgramData +=
                                        "uniform sampler2D    uRefractionMap   : register(s" + TexNum.ToString() + "),\n";
                                    TexNum++;

                                    if (cDepth && cUReflections)
                                    {
                                        FragmentProgramData +=
                                            "uniform sampler2D    uDepthReflectionMap : register(s" + TexNum.ToString() + "),\n";
                                        TexNum++;
                                    }

                                    FragmentProgramData +=
                                            "uniform sampler1D    uFresnelMap      : register(s" + TexNum.ToString() + ")";
                                    TexNum++;

                                    if (cFoam)
                                    {
                                        FragmentProgramData +=
                                         ",\nuniform sampler2D    uFoamMap         : register(s" + TexNum.ToString() + ")\n";
                                    }

                                    FragmentProgramData +=
                                                                                    ")\n" +
                                        "{\n" +
                                            "float2 ProjectionCoord = iUvProjection.xy / iUvProjection.w;\n" +
                                            "float3 camToSurface = iPosition.xyz - uEyePosition;\n" +
                                            "float additionalReflection=camToSurface.x*camToSurface.x+camToSurface.z*camToSurface.z;\n";

                                    if (cFoam)
                                    {
                                        // Calculate the foam visibility as a function fo distance specified by user
                                        FragmentProgramData +=
                                            "float foamVisibility=1.0f-saturate(additionalReflection/uFoamMaxDistance);\n";
                                    }

                                    FragmentProgramData +=
                                            "additionalReflection/=uFullReflectionDistance;\n" +
                                            "camToSurface=normalize(-camToSurface);\n";
                                    if (Options.NM == NormalMode.NM_TEXTURE)
                                    {
                                        FragmentProgramData +=
                                            "float3 pixelNormal = tex2D(uNormalMap,iUvNoise);\n" +
                                            // Inverte y with z, because at creation our local normal to the plane was z
                                            "pixelNormal.yz=pixelNormal.zy;\n" +
                                            // Remap from [0,1] to [-1,1]
                                            "pixelNormal.xyz=-expand(pixelNormal.xyz);\n";
                                    }
                                    else if (Options.NM == NormalMode.NM_VERTEX)
                                    {
                                        FragmentProgramData +=
                                            "float3 pixelNormal = -iNormal;\n";
                                    }
                                    else // NM_RTT
                                    {
                                        FragmentProgramData +=
                                            "float3 pixelNormal = -(2.0*tex2D(uNormalMap, ProjectionCoord.xy) - 1.0);\n";
                                    }
                                    FragmentProgramData +=
                                            "float2 pixelNormalModified = uNormalDistortion*pixelNormal.zx;\n";
                                    if (Options.NM == NormalMode.NM_TEXTURE || Options.NM == NormalMode.NM_RTT)
                                    {
                                        FragmentProgramData +=
                                            "float dotProduct=dot(camToSurface,pixelNormal);\n";
                                    }
                                    else
                                    {
                                        FragmentProgramData +=
                                            "float dotProduct=dot(-camToSurface,pixelNormal);\n";
                                    }
                                    FragmentProgramData +=
                                            "dotProduct=saturate(dotProduct);\n" +
                                            "float fresnel = tex1D(uFresnelMap,dotProduct);\n" +
                                        // Add additional reflection and saturate
                                            "fresnel+=additionalReflection;\n" +
                                            "fresnel=saturate(fresnel);\n" +
                                        // Decrease the transparency and saturate
                                            "fresnel-=uGlobalTransparency;\n" +
                                            "fresnel=saturate(fresnel);\n" +
                                        // Get the reflection/refraction pixels. Make sure to disturb the texcoords by pixelnormal
                                            "float3 refraction=tex2D(uRefractionMap,ProjectionCoord.xy-pixelNormalModified);\n";
                                    if (cUReflections)
                                    {
                                        FragmentProgramData +=
                                            "float3 reflection=tex2D(uReflectionMap,ProjectionCoord.xy+pixelNormalModified);\n";
                                    }
                                    else
                                    {
                                        FragmentProgramData +=
                                            "float3 reflection=uWaterColor;\n";
                                    }

                                    if (cDepth && cUReflections)
                                    {
                                        if (cCaustics)
                                        {
                                            FragmentProgramData +=
                                            "float2 depth = tex2D(uDepthReflectionMap,ProjectionCoord.xy+pixelNormalModified).rg;\n" +
                                            "reflection *= 1+depth.y*uCausticsPower;\n" +
                                            "reflection = lerp(uWaterColor,reflection,depth.x);\n";
                                        }
                                        else
                                        {
                                            FragmentProgramData +=
                                            "float depth = tex2D(uDepthReflectionMap,ProjectionCoord.xy-pixelNormalModified).r;\n" +
                                            "reflection = lerp(uWaterColor,reflection,depth);\n";
                                        }
                                    }

                                    FragmentProgramData +=
                                            "oColor = float4(lerp(refraction,reflection,fresnel),1);\n";

                                    if (cSun)
                                    {
                                        FragmentProgramData +=
                                            "float3 refractedVector = normalize(reflect(camToSurface, pixelNormal.xyz));\n" +
                                            "float3 surfaceToSun=normalize(uSunPosition-iPosition.xyz);\n" +
                                            // Temporally solution, fix this
                                            "surfaceToSun.xz = -surfaceToSun.xz;" +
                                            "float3 sunlight = uSunStrength*pow(saturate(dot(refractedVector,surfaceToSun)),uSunArea)*uSunColor;\n" +
                                            "oColor.xyz+=sunlight*saturate(1-additionalReflection);\n";
                                    }

                                    if (cFoam)
                                    {
                                        FragmentProgramData +=
                                            "float hmap = iPosition.y/uFoamRange*foamVisibility;\n" +
                                            "float2 foamTex=iWorldPosition.xz*uFoamScale+pixelNormalModified;\n" +
                                            "float foam=tex2D(uFoamMap,foamTex).r;\n" +
                                            "float foamTransparency=saturate(hmap-uFoamStart)*uFoamTransparency;\n" +
                                            "oColor.xyz=lerp(oColor.xyz,1,foamTransparency*foam);\n";
                                    }

                                    FragmentProgramData +=
                                        "}\n";
                                }
                                break;
                            case ShaderMode.SM_GLSL:
                                {
                                    //no supported yet.
                                }
                                break;
                        }//end shader mode
                        #endregion
                    }//end NM_RTT, end NM_VERTEX, end NM_TEXTURE
                    break;
            }//end normal mode
            #endregion

            #region - BuildMaterial -
            Material UnderwaterMaterial = mMaterials[(int)MaterialType.MAT_UNDERWATER];
            UnderwaterMaterial = (Material)AG.MaterialManager.Instance.Create(
                _def_Underwater_Material_Name,
                "HYDRAX_RESOURCE_GROUP");
            mMaterials[(int)MaterialType.MAT_UNDERWATER] = UnderwaterMaterial;
            Pass UM_Technique0_Pass0 = UnderwaterMaterial.GetTechnique(0).GetPass(0);
            UM_Technique0_Pass0.DepthWrite = true;
            UM_Technique0_Pass0.CullingMode = CullingMode.None;

            string[] GpuProgramsData = {VertexProgramData, FragmentProgramData};
		    string[] GpuProgramNames = {_def_Underwater_Shader_VP_Name, _def_Underwater_Shader_FP_Name};
		    string[] EntryPoints     = {"main_vp", "main_fp"};

            FillGpuProgramsToPass(UM_Technique0_Pass0, GpuProgramNames, Options.SM, EntryPoints, GpuProgramsData);

            GpuProgramParameters VP_Parameters = UM_Technique0_Pass0.VertexProgramParameters;
            GpuProgramParameters FP_Parameters = UM_Technique0_Pass0.FragmentProgramParameters;

            VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
            if (cFoam)
            {
                VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);
            }
            FP_Parameters.SetNamedAutoConstant("uEyePosition", GpuProgramParameters.AutoConstantType.CameraPositionObjectSpace, 0);

            FP_Parameters.SetNamedConstant("uFullReflectionDistance", mHydrax.FullReflectionDistance);
            FP_Parameters.SetNamedConstant("uGlobalTransparency", mHydrax.GlobalTransparency);
            FP_Parameters.SetNamedConstant("uNormalDistortion", mHydrax.NormalDistortion);

            if ((cDepth && cUReflections) || (!cUReflections))
            {
                FP_Parameters.SetNamedConstant("uWaterColor", mHydrax.WaterColor);
            }

            if (cSun)
            {
                FP_Parameters.SetNamedConstant("uSunPosition", mHydrax.Mesh.GetObjectSpacePosition(mHydrax.SunPosition));
                FP_Parameters.SetNamedConstant("uSunStrength", mHydrax.SunStrenght);
                FP_Parameters.SetNamedConstant("uSunArea", mHydrax.SunArea);
                FP_Parameters.SetNamedConstant("uSunColor", mHydrax.SunColor);
            }
            if (cFoam)
            {
                FP_Parameters.SetNamedConstant("uFoamRange", mHydrax.Mesh.MeshOptions.MeshStrength);
                FP_Parameters.SetNamedConstant("uFoamMaxDistance", mHydrax.FoamMaxDistance);
                FP_Parameters.SetNamedConstant("uFoamScale", mHydrax.FoamScale);
                FP_Parameters.SetNamedConstant("uFoamStart", mHydrax.FoamStart);
                FP_Parameters.SetNamedConstant("uFoamTransparency", mHydrax.FoamTransparency);
            }
            if (cCaustics && cDepth && cUReflections)
            {
                FP_Parameters.SetNamedConstant("uCausticsPower", mHydrax.CausticsPower);
            }

            if (Options.NM == NormalMode.NM_TEXTURE || Options.NM == NormalMode.NM_RTT)
            {
                UM_Technique0_Pass0.CreateTextureUnitState("HydraxNormalMap").SetTextureAddressingMode( TextureAddressing.Clamp );
            }

            if (cUReflections)
            {
                UM_Technique0_Pass0.CreateTextureUnitState("HydraxReflectionMap").SetTextureAddressingMode( TextureAddressing.Clamp );
            }
            UM_Technique0_Pass0.CreateTextureUnitState("HydraxRefractionMap").SetTextureAddressingMode( TextureAddressing.Clamp );

            if (cDepth && cUReflections)
            {
                UM_Technique0_Pass0.CreateTextureUnitState().SetTextureAddressingMode( TextureAddressing.Clamp );
            }

            UM_Technique0_Pass0.CreateTextureUnitState("Fresnel.png").SetTextureAddressingMode( TextureAddressing.Clamp );

            if (cFoam)
            {
                UM_Technique0_Pass0.CreateTextureUnitState("Foam.png").SetTextureAddressingMode( TextureAddressing.Wrap );
            }

            UnderwaterMaterial.ReceiveShadows = false;
            UnderwaterMaterial.Load();
            #endregion

            return true;
        }
        #endregion

        #region - CreateUnderwaterCompositor -
        /// <summary>
        /// Create underwater compositor
        /// </summary>
        /// <param name="Components">Components of the shader</param>
        /// <param name="Options"> Material options</param>
        /// <returns>true if succed.</returns>
        private bool CreateUnderwaterCompositor(HydraxComponent Components, Options Options)
        {
            bool cCaustics = IsComponent(Components, HydraxComponent.Caustics);
            bool cDepth = IsComponent(Components, HydraxComponent.Depth);
            bool cGodRays = IsComponent(Components, HydraxComponent.UnderwaterGodRays);

            string VertexProgramData = "", FragmentProgramData = "";

            #region - VertexProgram -
            switch (Options.SM)
            {
                case ShaderMode.SM_HLSL:
                //fall through
                case ShaderMode.SM_CG:
                    {
                        VertexProgramData +=
                    "void main_vp(\n" +
                            // IN
                        "float4 iPosition      : POSITION,\n" +
                            // OUT
                        "out float4 oPosition  : POSITION,\n" +
                        "out float3 oPosition_ : TEXCOORD0,\n" +
                        "out float2 oUV        : TEXCOORD1,\n";
                        // UNIFORM
                        if (cGodRays)
                        {
                            VertexProgramData +=
                        "uniform float3   uCorner0,\n" +
                        "uniform float3   uCorner01,\n" +
                        "uniform float3   uCorner02,\n";
                        }
                        VertexProgramData +=
                        "uniform float4x4 uWorldViewProj)\n" +
                    "{\n" +
                        "oPosition = mul(uWorldViewProj, iPosition);\n" +
                        "iPosition.xy = sign(iPosition.xy);\n" +
                        "oUV = (float2(iPosition.x, -iPosition.y) + 1.0f) * 0.5f;";
                        if (cGodRays)
                        {
                            VertexProgramData +=
                        "uCorner01 *= oUV.x;\n" +
                        "uCorner02 *= oUV.y;\n" +
                        "oPosition_ = uCorner0+uCorner01+uCorner02;";
                        }
                        VertexProgramData +=
                    "}\n";
                    }
                    break;
                case ShaderMode.SM_GLSL:
                    {
                    }
                    break;
            }//end shader mode
            #endregion

            #region - FragmentProgram -
            switch (Options.SM)
            {
                case ShaderMode.SM_HLSL:
                case ShaderMode.SM_CG:
                    {
                        FragmentProgramData +=
                    "void main_fp(\n" +
                            // IN
                        "float3  iPosition    : TEXCOORD0,\n" +
                        "float2  iUV          : TEXCOORD1,\n" +
                            // OUT
                        "out float4 oColor    : COLOR,\n";
                        // UNIFORM
                        if (cDepth)
                        {
                            FragmentProgramData +=
                                "uniform float3       uWaterColor,\n";
                        }
                        if (cCaustics)
                        {
                            FragmentProgramData +=
                                "uniform float        uCausticsPower,\n";
                        }
                        if (cGodRays)
                        {
                            FragmentProgramData +=
                                "uniform float3  uSunColor,\n" +
                                "uniform float3  uLightDirection,\n" +
                                "uniform float   uIntensity,\n" +
                                "uniform float3  uHGg,\n" +
                                "uniform float3  uCameraPos,\n";
                        }
                        FragmentProgramData +=
                            "uniform float        uTime,\n" +
                            "uniform sampler2D    uOriginalMap   : register(s0),\n" +
                            "uniform sampler2D    uDistortMap    : register(s1)\n";
                        if (cDepth)
                        {
                            FragmentProgramData +=
                                ",\nuniform sampler2D    uDepthMap      : register(s2)";
                        }
                        FragmentProgramData +=
                        ")\n{\n" +
                            "float2 distortUV = (tex2D(uDistortMap, float2(iUV.x + uTime, iUV.y + uTime)).xy - 0.5)/50;\n";
                        if (cCaustics) // Depth, caustics
                        {
                            FragmentProgramData +=
                                "float2 depth = tex2D(uDepthMap, iUV+distortUV).xy;\n" +
                                "oColor = float4(lerp(uWaterColor,tex2D(uOriginalMap, iUV+distortUV)*(1+depth.y*uCausticsPower), depth.x),1);\n";
                            if (cGodRays)
                            {
                                FragmentProgramData +=
                                    "float3 view_vector = normalize(iPosition-uCameraPos);" +
                                    "float dot_product = dot(view_vector, -uLightDirection);" +
                                    "float num = uHGg.x;" +
                                    "float den = (uHGg.y - uHGg.z*dot_product); " +
                                    "den = rsqrt(den); " +
                                    "float phase = num * (den*den*den);" +
                                    "oColor.xyz += (0.15 + uIntensity*tex2D(uDepthMap, iUV).z)*phase*uSunColor;";
                            }
                        }
                        else if (cDepth) // Depth, no caustics
                        {
                            FragmentProgramData +=
                                "oColor = float4(lerp(uWaterColor,tex2D(uOriginalMap, iUV+distortUV).xyz,tex2D(uDepthMap, iUV+distortUV).r),1);\n";
                            if (cGodRays)
                            {
                                FragmentProgramData +=
                                    "float3 view_vector = normalize(iPosition-uCameraPos);" +
                                    "float dot_product = dot(view_vector, -uLightDirection);" +
                                    "float num = uHGg.x;" +
                                    "float den = (uHGg.y - uHGg.z*dot_product); " +
                                    "den = rsqrt(den); " +
                                    "float phase = num * (den*den*den);" +
                                    "oColor.xyz += (0.15 + uIntensity*tex2D(uDepthMap, iUV).y)*phase*uSunColor;";
                            }
                        }
                        else // No depth, no caustics
                        {
                            FragmentProgramData +=
                                "oColor = tex2D(uOriginalMap, iUV+distortUV);";
                        }
                        FragmentProgramData +=
                        "}\n";
                    }
                    break;
                case ShaderMode.SM_GLSL:
                    {
                        //not yet supported
                    }
                    break;
            }//end Shader Mode
            #endregion

            #region - BuildMaterial -
            Material UnderwaterCompositorMaterial = mMaterials[(int)MaterialType.MAT_UNDERWATER_COMPOSITOR];
            UnderwaterCompositorMaterial = (Material)AG.MaterialManager.Instance.Create(
                _def_Underwater_Compositor_Material_Name,
                "HYDRAX_RESOURCE_GROUP");
            mMaterials[(int)MaterialType.MAT_UNDERWATER_COMPOSITOR] = UnderwaterCompositorMaterial;
            Pass DM_Technique0_Pass0 = UnderwaterCompositorMaterial.GetTechnique(0).GetPass(0);

            DM_Technique0_Pass0.CullingMode = CullingMode.None;
            DM_Technique0_Pass0.DepthFunction = CompareFunction.AlwaysPass;

            string[] GpuProgramsData = { VertexProgramData, FragmentProgramData };
            string[] GpuProgramNames = { _def_Underwater_Compositor_Shader_VP_Name, _def_Underwater_Compositor_Shader_FP_Name };
            string[] EntryPoints = { "main_vp", "main_fp" };

            FillGpuProgramsToPass(DM_Technique0_Pass0, GpuProgramNames, Options.SM, EntryPoints, GpuProgramsData);

            GpuProgramParameters VP_Parameters = DM_Technique0_Pass0.VertexProgramParameters;
            GpuProgramParameters FP_Parameters = DM_Technique0_Pass0.FragmentProgramParameters;

            VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);

            if (cDepth)
            {
                FP_Parameters.SetNamedConstant("uWaterColor", mHydrax.WaterColor);
            }
            FP_Parameters.SetNamedConstantFromTime("uTime", 0.1f);

            if (cCaustics)
            {
                FP_Parameters.SetNamedConstant("uCausticsPower", mHydrax.CausticsPower);
            }

            if (cGodRays)
            {
                FP_Parameters.SetNamedConstant("uSunColor", mHydrax.SunColor);
                FP_Parameters.SetNamedConstant("uLightDirection",
                    (mHydrax.Mesh.GetObjectSpacePosition(mHydrax.Camera.Position) -
                     mHydrax.Mesh.GetObjectSpacePosition(mHydrax.SunPosition)).ToNormalized());
                FP_Parameters.SetNamedConstant("uIntensity", mHydrax.GodRaysIntensity);
                FP_Parameters.SetNamedConstant("uHGg", mHydrax.GodRaysExposure);

                FP_Parameters.SetNamedAutoConstant("uCameraPos", GpuProgramParameters.AutoConstantType.CameraPosition, 0);
            }

            //from compositor, original scene
            DM_Technique0_Pass0.CreateTextureUnitState().SetTextureAddressingMode( TextureAddressing.Clamp );
            DM_Technique0_Pass0.CreateTextureUnitState("UnderwaterDistortion.jpg").SetTextureAddressingMode( TextureAddressing.Wrap );

            if (cDepth)
            {
                DM_Technique0_Pass0.CreateTextureUnitState("HydraxDepthMap").SetTextureAddressingMode( TextureAddressing.Clamp );
            }

            UnderwaterCompositorMaterial.ReceiveShadows = false;
            UnderwaterCompositorMaterial.Load();
            #endregion

            #region - CreateCompositor -
            Compositor UnderwaterCompositor = mCompositors[(int)CompositorType.COMP_UNDERWATER];
            UnderwaterCompositor = (Compositor)CompositorManager.Instance.Create(
                _def_Underwater_Compositor_Name,
                "HYDRAX_RESOURCE_GROUP");
            {
                mCompositors[(int)CompositorType.COMP_UNDERWATER] = UnderwaterCompositor;
                CompositionTechnique UnderWaterComp_Technique = UnderwaterCompositor.CreateTechnique();
                CompositionTechnique.TextureDefinition TDef = UnderWaterComp_Technique.CreateTextureDefinition("OriginalScene");
                TDef.Width = 0;
                TDef.Height = 0;
				TDef.PixelFormats = new List<Media.PixelFormat>();
				TDef.PixelFormats.Add( Axiom.Media.PixelFormat.A8B8G8R8 );
                //render the original scene
                CompositionTargetPass CTPass = UnderWaterComp_Technique.CreateTargetPass();
                CTPass.InputMode = CompositorInputMode.None;
                CTPass.OutputName = "OriginalScene";

                CompositionPass CPassClear = CTPass.CreatePass();
                CPassClear.Type = CompositorPassType.Clear;

                Vector3 WC = mHydrax.WaterColor;
                CPassClear.ClearColor = new ColorEx(WC.x, WC.y, WC.z);

                CompositionPass CPass = CTPass.CreatePass();
                CPass.Type = CompositorPassType.RenderScene;
                CPass.FirstRenderQueue = RenderQueueGroupID.SkiesEarly + 1;

                //build the output target pass
                CompositionTargetPass CTOutputPass = UnderWaterComp_Technique.OutputTarget;
                CTOutputPass.InputMode = CompositorInputMode.None;

                //final composition pass
                CompositionPass COutputPass = CTOutputPass.CreatePass();
                COutputPass.Type = CompositorPassType.RenderQuad;
                COutputPass.Material = mMaterials[(int)CompositorType.COMP_UNDERWATER];
                COutputPass.SetInput(0, "OriginalScene");
                COutputPass.LastRenderQueue = 0;

            }
#warning ADD ME
			CompositorManager.Instance.AddCompositor(
				mHydrax.Viewport, _def_Underwater_Compositor_Name ).MaterialRender +=
				new CompositorInstanceMaterialEventHandler( mUnderwaterCompositorListener.NotifyMaterialRender );

            #endregion

            return true;
        }
        #endregion

        #region - CreateDepthTextureGPUPrograms -
        /// <summary>
        /// Create depth texture gpu programs
        /// </summary>
        /// <param name="Components">Components of the sahder</param>
        /// <param name="Options">Material options</param>
        /// <param name="AlphaChannel">"x","y","z","w" or "r","g","b","a" (Channel where alpha information is stored)</param>
        /// <returns>true if no problems had happend, false if yes</returns>
        public bool CreateDepthTextureGPUPrograms(HydraxComponent Components, Options Options,
            string AlphaChannel)
        {
            bool cCaustics = IsComponent(Components, HydraxComponent.Caustics);
            string VertexProgramData = "", FragmentProgramData = "";

            #region - VertexProgram -
            switch (Options.SM)
            {
                case ShaderMode.SM_HLSL:
                    //fall through
                case ShaderMode.SM_CG:
                    {
                        // No caustics
                        if (!cCaustics)
                        {
                            VertexProgramData +=
                                "void main_vp(\n" +
                                // IN
                                    "float4 iPosition         : POSITION,\n" +
                                    "float2 iUV               : TEXCOORD0,\n" +
                                // OUT 
                                    "out float4 oPosition     : POSITION,\n" +
                                    "out float3 oPosition_UV  : TEXCOORD0,\n" +
                                // UNIFORM
                                    "uniform float            uPlaneYPos,\n" +
                                    "uniform float4x4         uWorld,\n" +
                                    "uniform float4x4         uWorldViewProj)\n" +
                                "{\n" +
                                   "oPosition = mul(uWorldViewProj, iPosition);\n" +
                                   "oPosition_UV.x = mul(uWorld, iPosition).y;\n" +
                                   "oPosition_UV.x-=uPlaneYPos;\n" +
                                   "oPosition_UV.yz = iUV;\n" +
                                "}\n";
                        }
                        else // Caustics
                        {
                            VertexProgramData +=
                                "void main_vp(\n" +
                                // IN
                                    "float4 iPosition         : POSITION,\n" +
                                    "float2 iUV               : TEXCOORD0,\n" +
                                // OUT 
                                    "out float4 oPosition     : POSITION,\n" +
                                    "out float3 oPosition_UV  : TEXCOORD0,\n" +
                                    "out float2 oUvWorld      : TEXCOORD1,\n" +
                                // UNIFORM
                                    "uniform float            uPlaneYPos,\n" +
                                    "uniform float4x4         uWorld,\n" +
                                    "uniform float4x4         uWorldViewProj)\n" +
                                "{\n" +
                                   "oPosition = mul(uWorldViewProj, iPosition);\n" +
                                   "float3 wPos = mul(uWorld, iPosition);\n" +
                                   "oPosition_UV.x = wPos.y;\n" +
                                   "oPosition_UV.x-=uPlaneYPos;\n" +
                                   "oPosition_UV.yz = iUV;\n" +
                                   "oUvWorld = wPos.xz;\n" +
                                "}\n";
                        }
                    }
                    break;
                case ShaderMode.SM_GLSL:
                    {
                        //not supported yet!
                    }
                    break;
            }
            #endregion

            #region - FragmentProgram -
            switch (Options.SM)
            {
                case ShaderMode.SM_HLSL:
                    //fall trough
                case ShaderMode.SM_CG:
                    {
                        // No caustics
                        if (!cCaustics)
                        {
                            FragmentProgramData +=
                                "void main_fp(\n" +
                                // IN
                                    "float3 iPosition_UV  : TEXCOORD0,\n" +
                                // OUT
                                    "out float4 oColor    : COLOR,\n" +
                                // UNIFORM
                                    "uniform float        uDepthLimit,\n" +
                                    "uniform sampler2D    uAlphaTex : register(s0))\n" +
                                "{\n" +
                                    "float pixelYDepth = (iPosition_UV.x*uDepthLimit+1);\n" +
                                    "pixelYDepth = saturate(pixelYDepth);\n" +
                                    "oColor = float4(pixelYDepth,0,0,0);\n" +
                                    "oColor.a = tex2D(uAlphaTex, iPosition_UV.yz)." + AlphaChannel + ";" +
                                "}\n";
                        }
                        else // Caustics
                        {
                            FragmentProgramData +=
                                "void main_fp(\n" +
                                // IN
                                    "float3 iPosition_UV  : TEXCOORD0,\n" +
                                    "float2 iUvWorld      : TEXCOORD1,\n" +
                                // OUT
                                    "out float4 oColor    : COLOR,\n" +
                                // UNIFORM
                                    "uniform float        uDepthLimit,\n" +
                                    "uniform float        uCausticsScale,\n" +
                                    "uniform float        uCausticsEnd,\n" +
                                    "uniform sampler2D    uCaustics : register(s0),\n" +
                                    "uniform sampler2D    uAlphaTex : register(s1))\n" +
                                "{\n" +
                                    "float pixelYDepth = (iPosition_UV.x*uDepthLimit+1);\n" +
                                    "pixelYDepth = saturate(pixelYDepth);\n" +
                                    "oColor = float4(pixelYDepth,0,0,0);\n" +
                                    "oColor.g = saturate(uCausticsEnd-pixelYDepth)*tex2D(uCaustics, iUvWorld/uCausticsScale).r;\n" +
                                    "oColor.a = tex2D(uAlphaTex, iPosition_UV.yz)." + AlphaChannel + ";" +
                                "}\n";
                        }
                    }
                    break;
                case ShaderMode.SM_GLSL:
                    {
                        //not supported yet!
                    }
                    break;
            }
            #endregion

            string[] GpuProgramsData = {VertexProgramData, FragmentProgramData};
		    string[] GpuProgramNames = {_def_DepthTexture_Shader_VP_Name+AlphaChannel, _def_DepthTexture_Shader_FP_Name+AlphaChannel};
		    string[] EntryPoints     = {"main_vp", "main_fp"};

            GpuProgram[] GpuPrograms = { GpuProgram.GPUP_VERTEX, GpuProgram.GPUP_FRAGMENT };

            for (int k = 0; k < 2; k++)
            {
                if (!CreateGpuProgram(GpuProgramNames[k], Options.SM, GpuPrograms[k], EntryPoints[k], GpuProgramsData[k]))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region - CreateSimpleColorMaterial -
        /// <summary>
        /// Create a simple color material
        /// </summary>
        /// <param name="Color">Material color</param>
        /// <param name="MType">Material type</param>
        /// <param name="Name">Material name</param>
        /// <param name="DepthCheck">Depth check enabled</param>
        /// <param name="DepthWrite">Depth write enabled</param>
        /// <returns>true if succed.</returns>
        public bool CreateSimpleColorMaterial(ColorEx Color, MaterialType MType, string Name,
            bool DepthCheck, bool DepthWrite)
        {
            Material SimpleColorMaterial = GetMaterial(MType);
            SimpleColorMaterial = (Material)AG.MaterialManager.Instance.Create(
                Name,
                "HYDRAX_RESOURCE_GROUP");
            
            mMaterials[(int)MType] = SimpleColorMaterial;
            Pass SCM_T0_Pass0 = SimpleColorMaterial.GetTechnique(0).GetPass(0);
            SCM_T0_Pass0.LightingEnabled = false;
            SCM_T0_Pass0.DepthCheck = DepthCheck;
            SCM_T0_Pass0.DepthWrite = DepthWrite;
            SCM_T0_Pass0.CullingMode = CullingMode.None;
            SCM_T0_Pass0.CreateTextureUnitState().SetColorOperationEx(LayerBlendOperationEx.Modulate,
                 LayerBlendSource.Manual, LayerBlendSource.Current, Color);
            SimpleColorMaterial.ReceiveShadows = false;
            SimpleColorMaterial.Load();

            return true;
        }
        /// <summary>
        /// Create a simple color material
        /// </summary>
        /// <param name="Color">Material color</param>
        /// <param name="MType">Material type</param>
        /// <param name="Name">Material name</param>
        /// <returns>true if succed.</returns>
        public bool CreateSimpleColorMaterial(ColorEx Color, MaterialType MType, string Name)
        {
            return CreateSimpleColorMaterial(Color, MType, Name, true, true);
        }
        #endregion

        #endregion

        #endregion

    }//end material manager
    #endregion

    #region - UnderwaterCompositorListener -
    /// <summary>
    /// 
    /// </summary>
    internal class UnderwaterCompositorListener //: CompositorInstanceListener
    {
        protected MaterialManager mMaterialManager;

        public UnderwaterCompositorListener(MaterialManager MatManager)
        {
            mMaterialManager = MatManager;
        }

		//public override void NotifyMaterialSetup( uint pass_id, Material mat )
		//{
		//    // base.NotifyMaterialSetup(pass_id, mat);
		//}
		public void NotifyMaterialRender( CompositorInstance source, CompositorInstanceMaterialEventArgs e )
		{
			Vector3 WC = mMaterialManager.Hydrax.WaterColor;

			Compositor UnderwaterCompositor = mMaterialManager.GetCompositor( MaterialManager.CompositorType.COMP_UNDERWATER );
			UnderwaterCompositor.Techniques[0].GetTargetPass( 0 ).Passes[0].ClearColor =
				new ColorEx( WC.x, WC.y, WC.z );
#warning not correct!
			GpuProgramParameters FP_Parameters = mMaterialManager.Materials[ (int)MaterialManager.MaterialType.MAT_UNDERWATER_COMPOSITOR ].GetTechnique( 0 ).GetPass( 0 ).FragmentProgramParameters; //mat.GetTechnique(0).GetPass(0).FragmentProgramParameters;//

			if ( mMaterialManager.Hydrax.IsComponent( HydraxComponent.Depth ) )
			{
				FP_Parameters.SetNamedConstant( "uWaterColor", mMaterialManager.Hydrax.WaterColor );
			}

			if ( mMaterialManager.Hydrax.IsComponent( HydraxComponent.Caustics ) )
			{
				FP_Parameters.SetNamedConstant( "uCausticsPower", mMaterialManager.Hydrax.CausticsPower );
			}

			if ( mMaterialManager.Hydrax.IsComponent( HydraxComponent.UnderwaterGodRays ) )
			{
				FP_Parameters.SetNamedConstant( "uSunColor", mMaterialManager.Hydrax.SunColor );
				FP_Parameters.SetNamedConstant( "uLightDirection",
					( mMaterialManager.Hydrax.Mesh.GetObjectSpacePosition( mMaterialManager.Hydrax.Camera.Position -
					 mMaterialManager.Hydrax.Mesh.GetObjectSpacePosition( mMaterialManager.Hydrax.SunPosition ) ) )
					 .ToNormalized() );

				FP_Parameters.SetNamedConstant( "uIntensity", mMaterialManager.Hydrax.GodRaysIntensity );
				FP_Parameters.SetNamedConstant( "uHGg", mMaterialManager.Hydrax.GodRaysExposure );
#warning not correct!
				GpuProgramParameters VP_Parameters = mMaterialManager.Materials[ (int)MaterialManager.MaterialType.MAT_UNDERWATER_COMPOSITOR ].GetTechnique( 0 ).GetPass( 0 ).VertexProgramParameters; //mat.GetTechnique(0).GetPass(0).VertexProgramParameters;//

				//FAR_LEFT_TOP
				VP_Parameters.SetNamedConstant(
					"uCorner0", mMaterialManager.Hydrax.Camera.WorldSpaceCorners[ 5 ] );
				//FAR_RIGHT_TOP - FAR_LEFT_TOP
				VP_Parameters.SetNamedConstant(
					"uCorner01",
					mMaterialManager.Hydrax.Camera.WorldSpaceCorners[ 4 ] -
					mMaterialManager.Hydrax.Camera.WorldSpaceCorners[ 5 ] );
				//FAR_LEFT_BOTTOM - FAR_LEFT_TOP
				VP_Parameters.SetNamedConstant(
					"uCorner02",
					mMaterialManager.Hydrax.Camera.WorldSpaceCorners[ 6 ] -
					mMaterialManager.Hydrax.Camera.WorldSpaceCorners[ 5 ] );
			}

			if ( mMaterialManager.CompositorNeedToBeReloaded[ (int)MaterialManager.CompositorType.COMP_UNDERWATER ] )
			{
				Pass DM_Technique0_Pass0 = e.Material.GetTechnique( 0 ).GetPass( 0 );

				if ( mMaterialManager.IsComponent( mMaterialManager.Components, HydraxComponent.Depth ) )
				{
					DM_Technique0_Pass0.GetTextureUnitState( 2 ).SetTextureName( "HydraxDepthMap" );
				}

				mMaterialManager.CompositorNeedToBeReloaded[ (int)MaterialManager.CompositorType.COMP_UNDERWATER ] = false;
			}
			//  base.NotifyMaterialRender(pass_id, mat);
		}
    }//UnderwaterCompositorListener
    #endregion

}//end namespace
#endregion