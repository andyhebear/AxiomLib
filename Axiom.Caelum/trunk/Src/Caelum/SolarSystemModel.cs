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
using Engine;
using Engine.MathEx;

namespace Caelum
{
    /// <summary>
    /// Class which calculates sun and moon positions on the sky. 
    /// Most of the calculations are done in the astronomy class.</summary>
    public class SolarSystemModel
    {
        /// <summary>
        /// Returns the updated sun's position.</summary>
        static public Vec3 GetSunDirection()
        {
            if (CaelumManager.Instance == null || UniversalClock.Instance == null)
                return CaelumUtils.YAxis;

            int fpmode = Astronomy.EnterHighPrecissionFloatingPointMode ();
            double jday = UniversalClock.Instance.JulianDay;

            Degree azimuth, altitude;
            Degree longitude = CaelumManager.Instance.ObserverLongitude;
            Degree latitude = CaelumManager.Instance.ObserverLatitude;

            Astronomy.GetHorizontalSunPosition(jday, longitude, latitude,
                out azimuth, out altitude);
            Vec3 res = MakeDirection(azimuth, altitude);

            Astronomy.RestoreFloatingPointMode(fpmode);
            return res;
        }

        /// <summary>
        /// Returns the updated moon's position.</summary>
        static public Vec3 GetMoonDirection()
        {
            if (CaelumManager.Instance == null || UniversalClock.Instance == null)
                return CaelumUtils.YAxis;

            int fpmode = Astronomy.EnterHighPrecissionFloatingPointMode ();
            double jday = UniversalClock.Instance.JulianDay;

            Degree azimuth, altitude;
            Degree longitude = CaelumManager.Instance.ObserverLongitude;
            Degree latitude = CaelumManager.Instance.ObserverLatitude;
            Astronomy.GetHorizontalMoonPosition(jday, longitude, latitude,
                out azimuth, out altitude);

            Vec3 res = MakeDirection(azimuth, altitude);

            Astronomy.RestoreFloatingPointMode(fpmode);
            return res;
        }

        /// <summary>
        /// Fake function to get the phase of the moon</summary>
        /// <remarks>The calculations performed by this function are completely fake.
        /// It's a triangle wave with a period of 28.5 days.</remarks>
        /// <returns>The phase of the moon; ranging from 0(full moon) to 2(new moon).</returns>
        static public float GetMoonPhase()
        {
            if (CaelumManager.Instance == null || UniversalClock.Instance == null)
                return 0.3f;

            double jday = UniversalClock.Instance.JulianDay;
            double T = (jday - 2454488.0665d) / 29.531026d;
            T = Math.Abs(T % 1);
            double phase = -Math.Abs(-4 * T + 2) + 2;
            return Convert.ToSingle(phase);
        }

        static protected Vec3 MakeDirection(Degree azimuth, Degree altitude)
        {
            Vec3 res = new Vec3();
            res.X = MathFunctions.Sin(azimuth.InRadians()) * MathFunctions.Cos(altitude.InRadians());  // East
            res.Z = -MathFunctions.Sin(altitude.InRadians()); // Zenith
            res.Y = -MathFunctions.Cos(azimuth.InRadians()) * MathFunctions.Cos(altitude.InRadians());  // North 
            return res;
        }
    }
}
