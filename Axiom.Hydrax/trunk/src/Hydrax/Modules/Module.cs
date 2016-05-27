#region - License -

#region - Original -
/*
--------------------------------------------------------------------------------
This source file is part of Hydrax.
Visit ---

Copyright (C) 2008 Xavier Verguín González <xavierverguin@hotmail.com>
                                           <xavyiy@gmail.com>

This program is free software; you can redistribute it and/or modify it under
the terms of the GNU Lesser General Public License as published by the Free Software
Foundation; either version 2 of the License, or (at your option) any later
version.

This program is distributed in the hope that it will be useful, but WITHOUT
ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with
this program; if not, write to the Free Software Foundation, Inc., 59 Temple
Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/lesser.txt.
--------------------------------------------------------------------------------
*/
#endregion

#region - Port -
/*
 * This file is part of the Axiom 3D Hydrax port.
 * Port author: (Bostich)
 */
#endregion

#endregion

#region - Using -
using Axiom.Math;
using Axiom.Hydrax.Noise;
#endregion

namespace Axiom.Hydrax.Modules
{
    #region - Module -
    /// <summary>
    /// Base module Class.
    /// Override it for create different ways of create water noise.
    /// </summary>
    public class Module
    {
        #region - Fields -
        protected MaterialManager.NormalMode mNormalMode;
        protected string mName;
        protected BaseNoise mNoise;
        protected Mesh.Options mMeshOptions;
        protected bool mIsCreated;
        #endregion

        #region - Properties -

        #region - NormalMode -
        /// <summary>
        /// Get's the normal generation mode.
        /// </summary>
        public MaterialManager.NormalMode NormalMode
        {
            set { }
            get { return mNormalMode; }
        }
        #endregion

        #region - MeshOptions -
        /// <summary>
        /// Get's the mesh options for this module.
        /// </summary>
        public Mesh.Options MeshOptions
        {
            set { }
            get { return mMeshOptions; }
        }
        #endregion

        #region - Noise -
        /// <summary>
        /// Get's the current noise.
        /// </summary>
        public BaseNoise Noise
        {
            set { }
            get { return mNoise; }
        }
        #endregion

        #region - IsCreated -
        /// <summary>
        /// Get's if created or not.
        /// </summary>
        public bool IsCreated
        {
            set { }
            get { return mIsCreated; }
        }
        #endregion

        #region - Name -
        /// <summary>
        /// Get's the name of this Module.
        /// </summary>
        public string Name
        {
            set { }
            get { return mName; }
        }
        #endregion

        #endregion

        #region - Constructor, Destructor -
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Name">Module name</param>
        /// <param name="Noise">Noise Generator</param>
        /// <param name="MeshOptions">Mesh options</param>
        /// <param name="NormalMode">Generation Mode</param>
        public Module(string Name, BaseNoise Noise, Mesh.Options MeshOptions,
            MaterialManager.NormalMode NormalMode)
        {
            mName = Name;
            mNoise = Noise;
            mMeshOptions = MeshOptions;
            mNormalMode = NormalMode;
        }
        ~Module()
        {
            mNoise = null;
        }
        #endregion

        #region - Methods -

        #region - Create -
        /// <summary>
        /// Create.
        /// <remarks>Do not forget call ovveride class</remarks>
        /// </summary>
        public virtual void Create()
        {
            mNoise.Create();
            mIsCreated = true;
        }
        #endregion

        #region - Remove -
        /// <summary>
        /// Remove.
        /// <remarks>Do not forget call ovveride class</remarks>
        /// </summary>
        public virtual void Remove()
        {
            //mNoise.Remove();
            mIsCreated = false;
        }
        #endregion

        #region - SetNoise -
        /// <summary>
        /// Set noise
        /// </summary>
        /// <param name="Noise">New noise module</param>
        /// <param name="GNMN">GPUNormalMapManager pointer, default: NULL, use it if GPU Normal map generation is needed</param>
        /// <param name="DeleteOldNoise">Delete the old noise module</param>
        public void SetNoise(BaseNoise Noise, GPUNormalMapManager GNM, bool DeleteOldNoise)
        {
            if (DeleteOldNoise)
            {
                mNoise = null;
            }

            mNoise = Noise;
            if (mIsCreated)
            {
                if (!mNoise.IsCreated)
                {
                    mNoise.Create();
                }

                if (mNormalMode == MaterialManager.NormalMode.NM_RTT)
                {
                    if (!mNoise.CreateGPUNormalMapResources(GNM))
                    {
                        Hydrax.HydraxLog(mNoise.Name + " doesn't support GPU Normal map generation");
                    }
                }
                else
                {
                    mNoise.RemoveGPUNormalMapRecources(GNM);
                }
            }
            else
            {
                mNoise.RemoveGPUNormalMapRecources(GNM);
            }
        }
        #endregion

        #region - CreateGeometry -
        /// <summary>
        /// Create geometry in module (if special module is needed)
        /// </summary>
        /// <param name="Mesh">mesh</param>
        /// <returns>false if it must becreate by default Mesh.CreateGeometry</returns>
        /// <remarks>Override it if any especial geometry mesh creation is needed.</remarks>
        public virtual bool CreateGeometry(Mesh Mesh)
        {
            return false;
        }
        #endregion

        #region - GetHeight -
        /// <summary>
        /// Get the current heigth at a especified world-space point.
        /// </summary>
        /// <param name="Position">Position X/Z World position</param>
        /// <returns>Heigth at the given position in y-World coordinates, if it's outside of the water return -1</returns>
        public virtual float GetHeight(Vector2 Position)
        {
            return -1;
        }
        #endregion

        #region - Update -
        /// <summary>
        /// Call it each frame
        /// </summary>
        /// <param name="TimeSinceLastFrame">Time since last frame (delta)</param>
        public virtual void Update(float TimeSinceLastFrame)
        {
            mNoise.Update(TimeSinceLastFrame);
        }
        #endregion

        #endregion
    }//end Class
    #endregion
}//end Namespcae
