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
using Engine.MathEx;

namespace Caelum
{

    /// <summary>
    /// Class which returns various sky colours.</summary>
    public class SkyColorModel
    {
        // Static attributes -----------------------------------------------------------------

        static protected Bitmap mGradientImage;

        static protected Bitmap mSkyGradientImage;

        // Static Accesors --------------------------------------------------------------------

        /// <summary>
        /// Sets the name of the image used to calculate sun color</summary>
        static public string GradientImage
        {
            set { mGradientImage = GetBMPFromPath(value); }
        }

        /// <summary>
        /// Sets the sky color gradients image's name.</summary>
        static public string SkyGradientImage
        {
            set { mSkyGradientImage = GetBMPFromPath(value); }
        }

        // Static Methods --------------------------------------------------------------------

        static public void Dispose()
        {
            if (mGradientImage != null)
                mGradientImage.Dispose();

            if (mSkyGradientImage != null)
                mSkyGradientImage.Dispose();

            mGradientImage = null;
            mSkyGradientImage = null;
        }

        /// <summary>
        /// Gets the colour of the sun sphere.
        /// This colour is used to draw the sun sphere in the sky.</summary>
        static public ColorValue GetSunColor()
        {
            Vec3 sunDirection = SolarSystemModel.GetSunDirection();
            float elevation = Vec3.Dot(sunDirection, CaelumUtils.YAxis) * 2.0f + 0.4f;
            return ImageHelper.GetInterpolatedColour(elevation, 1.0f, mGradientImage, false);
        }

        /// <summary>
        /// Gets the colour of sun light.
        /// This color is used to illuminate the scene.</summary>
        static public ColorValue GetSunLight()
        {
            Vec3 sunDirection = SolarSystemModel.GetSunDirection();
            float elevation = Vec3.Dot(sunDirection, CaelumUtils.YAxis) * 0.5f + 0.5f;
            ColorValue col = ImageHelper.GetInterpolatedColour(elevation, elevation, mSkyGradientImage, false);
            float val = (col.Red + col.Green + col.Blue) / 3;

            // Don't use an alpha value for lights, this can can nasty problems
            return new ColorValue(val, val, val, 1.0f);
        }

        /// <summary>
        /// Gets the colour of moon's body.</summary>
        static public ColorValue GetMoonBodyColour()
        {
            return CaelumUtils.ColorWhite;
        }
        
        /// <summary>
        /// Gets the colour of moon's light.
        /// This color is used to illuminate the scene.</summary>
        static public ColorValue GetMoonLight()
        {
            //scaled version of getSunLightColor
            Vec3 moonDirection = SolarSystemModel.GetMoonDirection();
            float elevation = Vec3.Dot(moonDirection, CaelumUtils.YAxis) * 0.5f + 0.5f;
            ColorValue col = ImageHelper.GetInterpolatedColour(elevation, elevation, mSkyGradientImage, false);
            float val = (col.Red + col.Green + col.Blue) / 3;

            // Don't use an alpha value for lights, this can can nasty problems
            return new ColorValue(val, val, val, 1.0f);
        }

        /// <summary>
        /// Gets the fog colour for a certain daytime.</summary>
        static public ColorValue GetFogColor()
        {
            Vec3 sunDirection = SolarSystemModel.GetSunDirection();
            float elevation = Vec3.Dot(sunDirection, CaelumUtils.YAxis) * 0.5f + 0.5f;
            return ImageHelper.GetInterpolatedColour(elevation, 1.0f, mSkyGradientImage, false);
        }

        /// <summary>
        /// Gets the fog density for a certain daytime.</summary>
        static public float GetFogDensity()
        {
            Vec3 sunDirection = SolarSystemModel.GetSunDirection();
            float elevation = Vec3.Dot(sunDirection, CaelumUtils.YAxis) * 0.5f + 0.5f;
            ColorValue col = ImageHelper.GetInterpolatedColour(elevation, 1.0f, mSkyGradientImage, false);
            return col.Alpha;
        }

        /// <summary>
        /// Gets a bitmap from his virtual path</summary>
        static protected Bitmap GetBMPFromPath(string virtualImageName)
        {
            string imageName = VirtualFileSystem.ResourceDirectory + "/" + virtualImageName;
            
            if (File.Exists(imageName))
                return new Bitmap(imageName);

            return null;
        }
    }
}
