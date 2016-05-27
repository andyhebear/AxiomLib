using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using Axiom.Graphics;
namespace Axiom.Hydrax
{
    /// <summary>
    /// Class to manager GPU normal maps
    /// </summary>
    public class GPUNormalMapManager
    {
        #region - Fields -
        protected Material mNormalMapMaterial;
        protected Hydrax mHydrax;
        protected RttManager mRttManager;
        protected bool mIsCreated;
        protected List<Texture> mTextures = new List<Texture>();
        #endregion

        #region - Properties -
        /// <summary>
        /// Get's the Hydrax object.
        /// <remarks>
        /// Needed by noise module in order to acced to the 
        /// MaterialManager to create vertex/fragment programs
        /// and more if needed.
        /// </remarks>
        /// </summary>
        public Hydrax Hydrax
        {
            set { }
            get { return mHydrax; }
        }
        /// <summary>
        /// Has been created allready called?
        /// true if yes, false if not
        /// </summary>
        public bool IsCreated
        {
            set { mIsCreated = value; }
            get { return mIsCreated; }
        }
        /// <summary>
        /// Get's the Normal Map material.
        /// </summary>
        public Material NormalMapMaterial
        {
            set { mNormalMapMaterial = value; }
            get { return mNormalMapMaterial; }
        }
        #endregion

        #region - Constructor, Destructor -
        public GPUNormalMapManager(Hydrax Hydrax)
        {
            mHydrax = Hydrax;
            mRttManager = Hydrax.RttManager;
            mRttManager.SetBitsPerChannel(RttManager.RttType.RTT_GPU_NORMAL_MAP, RttManager.BitsPerChannel.BPC_16);
            mRttManager.SetNumberOfChannels(RttManager.RttType.RTT_GPU_NORMAL_MAP, RttManager.NumberOfChannels.NOC_3);
        }
        ~GPUNormalMapManager()
        {
            Remove();
        }
        #endregion

        #region - Methods -
        /// <summary>
        /// Create.
        /// <remarks>NormalMapMaterial must have been created by the noise module before calling Create()</remarks>
        /// </summary>
        public void Create()
        {
            mRttManager.Initialize(RttManager.RttType.RTT_GPU_NORMAL_MAP);
            mHydrax.MaterialManager.Reload(MaterialManager.MaterialType.MAT_WATER);

            if(mHydrax.IsComponent(HydraxComponent.Underwater))
            {
                mHydrax.MaterialManager.Reload(MaterialManager.MaterialType.MAT_UNDERWATER);
            }

            mIsCreated = true;
        }
        /// <summary>
        /// Remove.
        /// </summary>
        public void Remove()
        {
            if (!mIsCreated)
                return;


            for (int k = 0; k < mTextures.Count; k++)
            {
                Axiom.Core.TextureManager.Instance.Remove(mTextures[k].Name);
            }

            mTextures.Clear();
            mRttManager.Remove(RttManager.RttType.RTT_GPU_NORMAL_MAP);

            HighLevelGpuProgramManager.Instance.Unload(
                mNormalMapMaterial.GetTechnique(0).GetPass(0).VertexProgramName);
            HighLevelGpuProgramManager.Instance.Unload(
                mNormalMapMaterial.GetTechnique(0).GetPass(0).FragmentProgramName);
            HighLevelGpuProgramManager.Instance.Remove(
                mNormalMapMaterial.GetTechnique(0).GetPass(0).VertexProgramName);
            HighLevelGpuProgramManager.Instance.Remove(
                mNormalMapMaterial.GetTechnique(0).GetPass(0).FragmentProgramName);

            Axiom.Graphics.MaterialManager.Instance.Remove(mNormalMapMaterial.Name);
            mNormalMapMaterial = null;
            mIsCreated = false;

        }
        /// <summary>
        /// Set active
        /// </summary>
        /// <param name="Active">true for yes, false for not</param>
        public void SetActive(bool Active)
        {
            if (Active)
            {
                mRttManager.Initialize(RttManager.RttType.RTT_GPU_NORMAL_MAP);
            }
            else
            {
                mRttManager.Remove(RttManager.RttType.RTT_GPU_NORMAL_MAP);
            }
        }
        /// <summary>
        /// Get a Texture
        /// </summary>
        /// <param name="Index">Texture Index.</param>
        /// <returns>Texture</returns>
        public Texture GetTexture(int Index)
        {
            return mTextures[Index];
        }
        /// <summary>
        /// Create a Texture.
        /// </summary>
        /// <param name="Texture">a Texture</param>
        public void AddTexture(Texture Texture)
        {
            mTextures.Add(Texture);
        }
        /// <summary>
        /// Removes a Texture.
        /// </summary>
        /// <param name="Index">Texture index</param>
        public void RemoveTexture(int Index)
        {
            Axiom.Core.TextureManager.Instance.Remove(mTextures[Index].Name);
            mTextures.Remove(mTextures[Index]);
        }
        #endregion
    }
}
