using System;
using System.Collections.Generic;
using System.Linq;
using Axiom.Core;
using Axiom.Configuration;
namespace Axiom.Hydrax
{
    public class CfgFileManager
    {
        protected Hydrax mHydrax;
        public CfgFileManager(Hydrax Hydrax)
        {
            mHydrax = Hydrax;
        }
        public bool Load(string File)
        {
            MathHelper.Pair<bool, ConfigFile> CfgFileResult = new MathHelper.Pair<bool, ConfigFile>();
          
            LoadCfgFile(File, CfgFileResult);
            if (!CfgFileResult.First)
            {
                return false;
            }
            ConfigFile mCfgFile = CfgFileResult.Second;
            if (!CheckVersion(mCfgFile))
            {
                return false;
            }
           
            //load main options
            mHydrax.Position = mCfgFile.GetVector("Position");
            mHydrax.PlanesError = mCfgFile.GetFloat("PlanesError");
            mHydrax.ShaderMode = mCfgFile.GetShaderMode("ShaderMode");
            mHydrax.FullReflectionDistance = mCfgFile.GetFloat("FullReflectionDistance");
            mHydrax.GlobalTransparency = mCfgFile.GetFloat("GlobalTransparency");
            mHydrax.NormalDistortion = mCfgFile.GetFloat("NormalDistortion");
            mHydrax.WaterColor = mCfgFile.GetVector("WaterColor");

            //Load Components
            LoadComponentsSettings(mCfgFile);

            //Load rtt settings
            LoadRTTSettings(mCfgFile);
            return true;
        }
        private void LoadComponentsSettings(ConfigFile CfgFile)
        {
            HydraxComponent mComponents = CfgFile.GetComponents();
            mHydrax.SetComponents(mComponents);
            if (mHydrax.IsComponent(HydraxComponent.Sun))
            {
                mHydrax.SunPosition = CfgFile.GetVector("SunPosition");
                mHydrax.SunStrenght = CfgFile.GetFloat("SunStrength");
                mHydrax.SunArea = CfgFile.GetFloat("SunArea");
                mHydrax.SunColor = CfgFile.GetVector("SunColor");
            }
            if (mHydrax.IsComponent(HydraxComponent.Foam))
            {
                mHydrax.FoamMaxDistance = CfgFile.GetFloat("FoamMaxDistance");
                mHydrax.FoamScale = CfgFile.GetFloat("FoamScale");
                mHydrax.FoamStart = CfgFile.GetFloat("FoamStart");
                mHydrax.FoamTransparency = CfgFile.GetFloat("FoamTransparency");
            }
            if (mHydrax.IsComponent(HydraxComponent.Depth))
            {
                mHydrax.DepthLimit = CfgFile.GetFloat("DepthLimit");
            }
            if (mHydrax.IsComponent(HydraxComponent.Smooth))
            {
                mHydrax.SmoothPower = CfgFile.GetFloat("SmoothPower");
            }
            if (mHydrax.IsComponent(HydraxComponent.Caustics))
            {
                mHydrax.CausticsScale = CfgFile.GetFloat("CausticsScale");
                mHydrax.CausticsPower = CfgFile.GetFloat("CausticsPower");
                mHydrax.CausticsEnd = CfgFile.GetFloat("CausticsEnd");
            }
            if (mHydrax.IsComponent(HydraxComponent.UnderwaterGodRays))
            {
                mHydrax.GodRaysExposure = CfgFile.GetVector("GodRaysExposure");
                mHydrax.GodRaysIntensity = CfgFile.GetFloat("GodRaysIntensity");
                mHydrax.GodRaysManager.SimulationSpeed = CfgFile.GetFloat("GodRaysSpeed");
                mHydrax.GodRaysManager.SetNumbersOfRays(CfgFile.GetInt("GodRaysNumberOfRays"));
                mHydrax.GodRaysManager.RaySize = CfgFile.GetFloat("GodRaysRaysSize");
                mHydrax.GodRaysManager.SetObjectIntersectionsEnabled(CfgFile.GetBool("GodRaysIntersections"));
            }
        }
        private void LoadRTTSettings(ConfigFile CfgFile)
        {
            mHydrax.RttManager.SetTextureSize(RttManager.RttType.RTT_REFLECTION, CfgFile.GetSize("Rtt_Quality_Reflection"));
            mHydrax.RttManager.SetTextureSize(RttManager.RttType.RTT_REFRACTION, CfgFile.GetSize("Rtt_Quality_Refraction"));
            mHydrax.RttManager.SetTextureSize(RttManager.RttType.RTT_DEPTH, CfgFile.GetSize("Rtt_Quality_Depth"));
            mHydrax.RttManager.SetTextureSize(RttManager.RttType.RTT_DEPTH_REFLECTION, CfgFile.GetSize("Rtt_Quality_URDepth"));
            mHydrax.RttManager.SetTextureSize(RttManager.RttType.RTT_GPU_NORMAL_MAP, CfgFile.GetSize("Rtt_Quality_GPUNormalMap"));

        }
        private bool CheckVersion(ConfigFile CfgFile)
        {
            if (CfgFile.HydraxVersion != Hydrax.HYDRAX_VERSION)
                return false;

            return true;
        }
        private void LoadCfgFile(string File, MathHelper.Pair<bool, ConfigFile> Result)
        {
            if (!ResourceGroupManager.Instance.ResourceExists("General", File))
            {
                Result.First = false;
                Hydrax.HydraxLog("CfgFileManager.LoadCfgFile(...): " + File + " doesn't found in HYDRAX_RESOURCE_GROUP resource group.");
                return;
            }

            Result.First = true;
            ConfigFile mCfgFile = new ConfigFile();
            Result.Second = mCfgFile;
            Result.Second.Load(ResourceGroupManager.Instance.OpenResource(File, "General"));
            Hydrax.HydraxLog(File + " loaded.");
        }
    }
}
