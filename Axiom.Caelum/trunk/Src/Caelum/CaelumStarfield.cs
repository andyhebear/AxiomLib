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
    /// Class for starfield.</summary>
    public class CaelumStarfield : CaelumBaseMesh
    {
        // Attributes -----------------------------------------------------------------

        private static CaelumStarfield mInstance;

        // Accessors --------------------------------------------------------------------

        public static CaelumStarfield Instance
        {
            get { return mInstance; }
        }

        // Methods --------------------------------------------------------------------

        public CaelumStarfield(CaelumItem item)
        {
            Initialise(RenderQueueGroupID.SkiesEarly, item.Mesh, item.Scale, item.Rotation, item.Translation); 
            mInstance = this;
        }

        ~CaelumStarfield()
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

            if (CaelumManager.Instance == null || mNode == null)
                return;

            float inclinaison = CaelumManager.Instance.StarFieldInclination;

            // Calculates rotation quaternion
            Quat orientation = Quat.Identity;
            orientation *= CaelumUtils.GenerateQuat(CaelumUtils.XAxis, new Radian( new Degree(inclinaison + 90)));
            orientation *= CaelumUtils.GenerateQuat(CaelumUtils.YAxis, new Radian(-time * 2 * MathFunctions.PI));

            mNode.Rotation = orientation;
            mNode.Position = cam.Position + mOffset;
        }
    }
}
