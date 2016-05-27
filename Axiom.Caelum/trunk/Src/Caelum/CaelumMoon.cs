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
    /// Class representing the moon.</summary>
    public class CaelumMoon : CaelumBaseSkyLight
    {
        // Attributes -----------------------------------------------------------------

        private static CaelumMoon mInstance;

        // Accessors --------------------------------------------------------------------

        public static CaelumMoon Instance
        {
            get { return mInstance; }
        }

        // Methods --------------------------------------------------------------------

        public CaelumMoon(CaelumItem item)
        {
            Initialise(RenderQueueGroupID.SkiesEarly + 2, item.Material, item.Scale);
            mMainLight.CastShadows = false;

            mInstance = this;
        }

        ~CaelumMoon()
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

            //Sets the sun position
            Vec3 moonDir = SolarSystemModel.GetMoonDirection();
            mNode.Position = (cam.Position - moonDir * mFarDistance);

            float phase = SolarSystemModel.GetMoonPhase();
            Pass pass = MainMaterial.GetBestTechnique().Passes[0];
            pass.FragmentProgramParameters.SetNamedConstant("phase", phase);

            //Sets the sun colors
            ColorValue lightColor = SkyColorModel.GetMoonLight();
            setBodyColor(SkyColorModel.GetMoonBodyColour());
            setLighting(lightColor * (1 - phase/2), moonDir);

            mMainLight.CastShadows = false;
        }
    }
}
