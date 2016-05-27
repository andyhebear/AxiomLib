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

using Engine.Renderer;
using Engine.MathEx;

namespace Caelum
{
    /// <summary>
    ///  Class for skydome.</summary>
    /// <remarks>This is tighly integrated with CG script and Skydome.material.</remarks>
    public class CaelumSkydome : CaelumBaseMesh
    {
        // Attributes -----------------------------------------------------------------

        protected bool mShadersEnabled;

        private static CaelumSkydome mInstance;

        // Accessors --------------------------------------------------------------------

        public static CaelumSkydome Instance
        {
            get { return mInstance; }
        }

        // Methods --------------------------------------------------------------------

        public CaelumSkydome(CaelumItem item)
        {
            Initialise(RenderQueueGroupID.SkiesEarly + 1, item.Mesh, item.Scale, item.Rotation, item.Translation);

            Pass pass = MainMaterial.GetBestTechnique().Passes[0];
            mShadersEnabled = !(string.IsNullOrEmpty(pass.FragmentProgramName) || 
                                string.IsNullOrEmpty(pass.VertexProgramName));

            mInstance = this;
        }

        ~CaelumSkydome()
        {
            Dispose();
        }

        public override void Dispose()
        {
            base.Dispose();
            mInstance = null;
        }

        public override void Update(float time, Camera cam)
        {
            base.Update(time, cam);

            if (CaelumManager.Instance == null || MainMaterial == null)
                return;

            // Gets the sun's direction
            Vec3 sunDir = SolarSystemModel.GetSunDirection();
            float elevation = Vec3.Dot(sunDir, CaelumUtils.YAxis) * 0.5f + 0.5f;

            // Sets Cg script parameters
            Pass pass = MainMaterial.Techniques[0].Passes[0];

            if (mShadersEnabled)
            {
                GpuProgramParameters vpParams = pass.VertexProgramParameters;
                GpuProgramParameters fpParams = pass.FragmentProgramParameters;
                vpParams.SetNamedConstant("sunDirection", sunDir);
                fpParams.SetNamedConstant("offset", elevation);
            }
            else
            {
                TextureUnitState gradientTus = pass.TextureUnitStates[0];
                gradientTus.TextureScroll = new Vec2(elevation, gradientTus.TextureScroll.Y);
            }

            mNode.Position = cam.Position + mOffset;
        }
    }
}
