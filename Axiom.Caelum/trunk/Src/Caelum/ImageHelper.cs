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

using System;
using System.Drawing;
using Engine.FileSystem;
using Engine.MathEx;

namespace Caelum
{
    /// <summary>
    /// Defines some bitmap tools</summary>
    public class ImageHelper
    {

        /// <summary>
        /// Gets the interpolated colour between two pixels from an image.
        /// Interpolate a texture pixel by hand. (fx, fy) are in texture coordinates,
        /// ranging [0-1] across the entire texture.
        /// Smooth blending is only done on the x coordinate.
        /// Wrapping is only supported on X as well.</summary>
        static public ColorValue GetInterpolatedColour(float fx, float fy, Bitmap img, bool wrapX)
        {
            if (img == null)
                return CaelumUtils.ColorWhite;

            // Get the image width
            int imgWidth = img.Width;
            int imgHeight = img.Height;

            // Calculate pixel y coord.
            int py = Convert.ToInt32( MathFunctions.Floor(Math.Abs(fy)*(imgHeight - 1) ));
            // Snap to py image bounds.
            py = Math.Max(0, Math.Min(py, imgHeight - 1));

            // Get the two closest pixels on x.
            // px1 and px2 are the closest integer pixels to px.
            float px = fx * (img.Width - 1);
	        int px1 = Convert.ToInt32( MathFunctions.Floor(px));
            int px2 = Convert.ToInt32( Math.Ceiling(px));

            if (wrapX) 
            {
                // Wrap x coords. The funny addition ensures that it does
                // "the right thing" for negative values.
                px1 = (px1 % imgWidth + imgWidth) % imgWidth;
                px2 = (px2 % imgWidth + imgWidth) % imgWidth;
            } 
            else 
            {
                px1 = Math.Max(0, Math.Min(px1, imgWidth - 1));
                px2 = Math.Max(0, Math.Min(px2, imgWidth - 1));
            }

            // Gets (px1, py) pixel
            Color col = img.GetPixel(px1, py);
            float A = col.A, R = col.R, G = col.G, B = col.B;
            ColorValue c1 = new ColorValue(R / 255, G / 255, B / 255, A / 255);

            // Gets (px2, py) pixel
            col = img.GetPixel(px2, py);
            A = col.A; R = col.R; G = col.G; B = col.B;
            ColorValue c2 = new ColorValue(R / 255, G / 255, B / 255, A / 255);

            // Blend the two pixels together.
            // diff is the weight between pixel 1 and pixel 2.
            float diff = px - px1;
            ColorValue cf = c1 * (1 - diff) + c2 * diff;

            return cf;
        }
    }
}
