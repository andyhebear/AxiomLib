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
/*
using System;
using System.Collections.Generic;
using Engine;
using Engine.MathEx;

namespace GameEntities.Caelum
{
    /// <summary>
    /// A simple circular sun position model. 
	/// It just creates an inclinated circular orbit.
    /// </summary>
    public class SimpleSunPositionModel
    {
        /// <summary>
        /// Returns the updated sun's position.
        /// </summary>
        static public Vec3 update(float Time)
        {
            if (CaelumManager.Instance == null)
                return Vec3.Zero;

            Degree mInclination = new Degree(CaelumManager.Instance.SunOrbiteInclination);

            // Gets the inclinated axis
            Vec3 axis = CaelumUtils.ZAxis;
            axis *= CaelumUtils.GenerateQuat(CaelumUtils.XAxis, mInclination);

            // Gets the inclinated light direction, according to the day time
            Vec3 dir = CaelumUtils.YAxis;
            dir *= CaelumUtils.GenerateQuat(axis, new Radian(Time * 2 * MathFunctions.PI));

            Vec3 mSunPosition = dir.GetNormalize() * -1;

            return mSunPosition * -1;
        }
    }
}*/
