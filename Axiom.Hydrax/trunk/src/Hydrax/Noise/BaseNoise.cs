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

#region - namespace -
namespace Axiom.Hydrax.Noise
{
    #region - BaseNoise -
    /// <summary>
    /// Base noise class,
    /// Override it for create different ways of create water noise.
    /// </summary>
    public class BaseNoise
    {
        #region - Fields -
        protected bool mIsCreated;
        protected string mName;
        protected bool mIsGPUNormalMapResourcesCreated;
        protected bool mIsGPUNormalMapSupported;
        #endregion

        #region - Properties -

        #region - IsGPUNormalMapSupported -
        /// <summary>
        /// Get's true if GPU normal map generation is supported otherwise false.
        /// </summary>
        public bool IsGPUNormalMapSupported
        {
            set { }
            get { return mIsGPUNormalMapSupported; }
        }
        #endregion

        #region - IsGPUNormalMapResourceCreated -
        /// <summary>
        /// Get's true if GPU normal map resources created, otherwise false
        /// </summary>
        public bool IsGPUNormalMapResourcesCreated
        {
            set { }
            get { return mIsGPUNormalMapResourcesCreated; }
        }
        #endregion

        #region - Name -
        /// <summary>
        /// Get's the name of this Noise Module.
        /// </summary>
        public string Name
        {
            set { }
            get { return mName; }
        }
        #endregion

        #region - IsCreated -
        /// <summary>
        /// Get's true if Noise is created, otherwise false.
        /// </summary>
        public bool IsCreated
        {
            set { }
            get { return mIsCreated; }
        }
        #endregion

        #endregion

        #region - Constructor, Destructor -
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Name">Noise Name</param>
        /// <param name="GPUNormalMapSupport">Is GPU normal map generation supported?</param>
        public BaseNoise(string Name, bool GPUNormalMapSupport)
        {
            mName = Name;
            mIsGPUNormalMapSupported = GPUNormalMapSupport;

        }
        ~BaseNoise()
        {
        }
        #endregion

        #region - Methods - 

        #region - Create -
        /// <summary>
        /// Create.
        /// </summary>
        public virtual void Create()
        {
            mIsCreated = true;
        }
        #endregion

        #region - Remove -
        /// <summary>
        /// Remove.
        /// </summary>
        public virtual void Remove()
        {
            mIsCreated = false;
        }
        #endregion

        #region - Update -
        /// <summary>
        /// Call it each frame.
        /// </summary>
        /// <param name="TimeSinceLastFrame">Time since last frame(delta)</param>
        public virtual void Update(float TimeSinceLastFrame)
        {
        }
        #endregion

        #region - CreateGPUNormalMapResources -
        /// <summary>
        /// Create GPUNormalMap resources
        /// </summary>
        /// <param name="G">GPUNormalMapManager object</param>
        /// <returns>true if it needs to be created, otherwise false</returns>
        public virtual bool CreateGPUNormalMapResources(GPUNormalMapManager G)
        {
            if (mIsGPUNormalMapSupported)
            {
                if (mIsGPUNormalMapResourcesCreated)
                {
                    RemoveGPUNormalMapRecources(G);
                }

                mIsGPUNormalMapResourcesCreated = true;

                G.Remove();

                return true;
            }
            return false;
        }
        #endregion

        #region - RemoveGPUNormalMapRecources -
        /// <summary>
        /// Remove GPUNormalMap resources
        /// </summary>
        /// <param name="G">GPUNormalMapManager object</param>
        public virtual void RemoveGPUNormalMapRecources(GPUNormalMapManager G)
        {
            if (mIsGPUNormalMapSupported && mIsGPUNormalMapResourcesCreated)
            {
                mIsGPUNormalMapResourcesCreated = false;

                G.Remove();
            }
        }
        #endregion

        #region - GetValue -
        /// <summary>
        /// Get the especified x/y noise value
        /// </summary>
        /// <param name="X">X Coord</param>
        /// <param name="Y">Y Coord</param>
        /// <returns>Noise value</returns>
        public virtual float GetValue(float X, float Y)
        {
            return -1;
        }
        #endregion

        #endregion
    }//end class
    #endregion
}//end namespace
#endregion