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
    /// Class representing the sun.</summary>
    public class CaelumSun : CaelumBaseSkyLight
    {
        // Attributes -----------------------------------------------------------------

        private static CaelumSun mInstance;

        // Accessors --------------------------------------------------------------------

        public static CaelumSun Instance
        {
            get { return mInstance; }
        }

        // Methods --------------------------------------------------------------------

        public CaelumSun(CaelumItem item)
        {
            Initialise(RenderQueueGroupID.Queue1, item.Material, item.Scale);
            mInstance = this;
        }

        ~CaelumSun()
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

            if (CaelumManager.Instance == null)
                return;

            //Sets the sun position
            Vec3 sunDir = SolarSystemModel.GetSunDirection();
            mNode.Position = (cam.Position - sunDir * mFarDistance);

            //Sets the sun colors
            ColorValue bodyColor = SkyColorModel.GetSunColor();
            setBodyColor(bodyColor);

            ColorValue lightColor = SkyColorModel.GetSunLight();
            SetAmbientLight(lightColor);
            setLighting(lightColor, sunDir);
        }

        /// <summary>
        /// Sets the sunlight colour used as ambient light.</summary>
        protected void SetAmbientLight(ColorValue light)
        {
            // Retrieves parameters from CaelumManager
            ColorValue ambient = light * CaelumManager.Instance.AmbientMultiplier;
            ColorValue minAmbient = CaelumManager.Instance.MinAmbientLight;

            if (ambient.Red < minAmbient.Red)
                ambient = minAmbient;

            if (CaelumManager.Instance.ManageAmbientLight)
                SceneManager.Instance.AmbientLight = ambient;
        }
    }
}
