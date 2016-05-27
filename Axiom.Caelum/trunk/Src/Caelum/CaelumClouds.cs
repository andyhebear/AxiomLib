/*
This file is part of Caelum for NeoAxis Engine.
Caelum for NeoAxisEngine is a Caelum's modified version.
See http://www.ogre3d.org/wiki/index.php/Caelum for the original version.

Copyright (c) 2008-2009 Association Hat. See Contributors.txt for details.

Caelum for NeoAxis Engine is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Caelum for NeoAxis Engine is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Caelum for NeoAxis Engine. If not, see <http://www.gnu.org/licenses/>.
*/

using System.Drawing;
using System.IO;
using Engine.FileSystem;
using Engine.Renderer;
using Engine.MathEx;

namespace Caelum
{
    /// <summary>
    /// Class for layered clouds.</summary>
    /// <remarks>This is tighly integrated with CaelumShaders.cg and LayeredClouds.material.
    /// There are two "cloud mass" layers blended to create animating clouds and an extra
    /// detailLayer.
    /// Most of the parameters in this class are direct wrappers from GPU shader params.
    /// Cloud offsets and speeds are not in any meaningful world units. Maybe they should
    /// be in radians or something?</remarks>
    public class CaelumClouds : CaelumBaseMesh
    {
        // Attributes -----------------------------------------------------------------

        /// <summary>
        /// Current cloud blend factor.</summary>
        protected float mCloudMassBlend;

        /// <summary>
        /// Current cloud layer offset.</summary>
        protected Vec2 mCloudMassOffset;

        /// <summary>
        /// Current cloud detail layer offset.</summary>
        protected Vec2 mCloudDetailOffset;

        /// <summary>
        /// Lookup used for cloud coverage.</summary>
        protected Bitmap mCoverLookupImage;


        protected bool mShadersEnabled;


        private static CaelumClouds mInstance;

        // Accessors --------------------------------------------------------------------

        public static CaelumClouds Instance
        {
            get { return mInstance; }
        }

        // Methods --------------------------------------------------------------------

        /// <summary>
        /// Creates the element in the world. It automatically
        /// sets up the mesh and the node with the infos in <paramref name="Item"/></summary>.
        /// <param name="item">Describes the element and all necessary infos</param>
        public CaelumClouds(CaelumItem item)
        {
            Initialise(RenderQueueGroupID.Queue2, item.Mesh, item.Scale, item.Rotation, item.Translation); 
            
            Pass pass = MainMaterial.GetBestTechnique().Passes[0];
            mShadersEnabled = !(string.IsNullOrEmpty(pass.FragmentProgramName) ||
                                string.IsNullOrEmpty(pass.VertexProgramName));

            SetCloudMassOffset(Vec2.Zero);
            SetCloudDetailOffset(Vec2.Zero);
            SetCloudMassBlend(0.5f);

            mInstance = this;
        }

        ~CaelumClouds()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (mDisposed)
                return;

            if (mCoverLookupImage != null)
                mCoverLookupImage.Dispose();

            mCoverLookupImage = null;
            mInstance = null;

            base.Dispose();
        }

        /// <summary>
        /// Sets the bitmap which will be used to coverage's calculation.</summary>
        /// <param name="virtualImageName">The virtual path of the bitmap</param>
        /// <example>setCoverImage(Caelum\Clouds\CloudCoverLookup.png)</example>
        public void SetCoverImage(string virtualImageName)
        {
            if (CaelumManager.Instance == null)
                return;

            string imageName = VirtualFileSystem.ResourceDirectory + "/" + virtualImageName;
            mCoverLookupImage = null;

            if (File.Exists(imageName))
                mCoverLookupImage = new Bitmap(imageName);
        }

        /// <summary>
        /// Sets the current blending factor between the two cloud mass layers.</summary>
        /// <param name="massBlend"></param>
        public void SetCloudMassBlend(float massBlend)
        {
            mCloudMassBlend = massBlend;
            if(mShadersEnabled)
                GetFpParams().SetNamedConstant("cloudMassBlend", mCloudMassBlend);
        }

        /// <summary>
        /// Sets the current offset(translation) of the cloud mass on the sky.</summary>
        protected void SetCloudMassOffset(Vec2 massOffset)
        {
            mCloudMassOffset = massOffset;
            if(mShadersEnabled)
                GetFpParams().SetNamedConstant("cloudMassOffset", new Vec3(massOffset.X, massOffset.Y, 0));
            else
            {
                GetTUS(0).TextureScroll = mCloudMassOffset;
                GetTUS(1).TextureScroll = mCloudMassOffset;
            }
        }

        /// <summary>
        /// Sets current offset(translation) of cloud details on the sky.</summary>
        protected void SetCloudDetailOffset(Vec2 detailOffset)
        {
            mCloudDetailOffset = detailOffset;
            if(mShadersEnabled)
                GetFpParams().SetNamedConstant("cloudDetailOffser", new Vec3(detailOffset.X, detailOffset.Y, 0));
        }

        public override void Update(float time, Camera cam)
        {
            base.Update(time, cam);

            // Avoid some bug on destruction
            if (CaelumManager.Instance == null || MainMaterial == null || 
                UniversalClock.Instance == null)
                return;

            // Retrieves parameters from CaelumManager
            Vec2 cloudSpeed = CaelumManager.Instance.CloudsSpeed / 1000000;
            float cloudBlendTime = CaelumManager.Instance.CloudsBlendTime;
            float cloudCover = CaelumManager.Instance.CloudCover;
            time = UniversalClock.Instance.getJulianSecondDifference();

            // Sets sun parameters.
            UpdateSunDirection();
            UpdateSunColor();
            UpdateFogColor();

            // Sets coverage value. Needed to a real-time coverage's edition.
            if (mShadersEnabled)
            {
                float coverage = ImageHelper.GetInterpolatedColour(cloudCover, 1, mCoverLookupImage, false).Red;
                GetFpParams().SetNamedConstant("cloudCoverageThreshold", coverage);
            }

            // Clouds' calculation and animation
            if (CaelumManager.Instance.AnimatedClouds)
            {
                // Move clouds.
                SetCloudMassOffset(mCloudMassOffset + time * cloudSpeed);
                SetCloudDetailOffset(mCloudDetailOffset - time * cloudSpeed);

                // Animate cloud blending.
                float blend = mCloudMassBlend;
                blend += time / cloudBlendTime;
                blend = blend % 1;
                if (blend < 0)
                    blend = 1 - blend;

                SetCloudMassBlend(blend);
            }

            mNode.Position = cam.Position + mOffset;
        }

        /// <summary>
        /// Calculs and sends the current direction of the sun to Cg script.</summary>
        protected void UpdateSunDirection()
        {
            Vec3 mSunDirection = SolarSystemModel.GetSunDirection();
            if(mShadersEnabled)
                GetVpParams().SetNamedConstant("sunDirection", mSunDirection);
        }

        /// <summary>
        /// Calculs and sends the current color of the sun's light to Cg script.</summary>
        protected void UpdateSunColor()
        {
            ColorValue sunColour = SkyColorModel.GetSunLight();
            if (mShadersEnabled)
                GetFpParams().SetNamedConstant("sunColour", sunColour.ToVec4());
        }

        /// <summary>
        /// Calculs and sends the current fog's color to Cg script.</summary>
        protected void UpdateFogColor()
        {
            ColorValue fogColour = SkyColorModel.GetFogColor();
            if (mShadersEnabled)
                GetFpParams().SetNamedConstant("fogColour", fogColour.ToVec4());
        }

        /// <summary>
        /// Shortcut function for fragment program parameters.</summary>
        /// <remarks>Doesn't check if there is a material, can throws an exception.</remarks>
        protected GpuProgramParameters GetFpParams()
        {
            return MainMaterial.GetBestTechnique().Passes[0].FragmentProgramParameters;
        }

        /// <summary>
        /// Shortcut function for vertex program parameters.</summary>
        /// <remarks>Doesn't check if there is a material, can throws an exception.</remarks>
        protected GpuProgramParameters GetVpParams()
        {
            return MainMaterial.GetBestTechnique().Passes[0].VertexProgramParameters;
        }

        /// <summary>
        /// Shortcut function for texture unit state parameters</summary>
        /// <remarks>Doesn't check if there is a material, can throws an exception.</remarks>
        protected TextureUnitState GetTUS(ushort num)
        {
            return MainMaterial.GetBestTechnique().Passes[0].TextureUnitStates[num];
        }
    }
}
