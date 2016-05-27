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
using System.Runtime.InteropServices;

using Engine.MathEx;

namespace Caelum
{
    /// <summary>
    /// Static class with astronomy routines.
    /// This class contains various astronomical routines useful in Caelum.
    ///
    /// Most of the formulas are from http://stjarnhimlen.se/comp/ppcomp.html
    /// That site contains much more than was implemented here; it has code
    /// for determining the positions of all the planets. Only the sun and
    /// moon are actually useful for caelum.
    ///
    /// The formulas are isolated here in pure procedural code for easier
    /// testing.
    ///
    /// All angles are in degrees unless otherwise mentioned.</summary>
    public class Astronomy
    {
        // Protected --------------------------------------------------------------------

        /// <summary>
        /// Imports function to allow a better floating point precision mode.</summary>
        [DllImport("msvcr71.dll", CallingConvention = CallingConvention.Cdecl)]
        protected static extern int _controlfp(int n, int mask);

        /// <summary>
        /// Defines floating precision modes.</summary>
        protected const int _MCW_PC = 0x00030000;   /* Precision Control */
        protected const int _PC_64 = 0x00000000;    /*    64 bits */

        protected const double PI = 3.1415926535897932384626433832795029d;

        /// <summary>
        /// Normalizes an angle to the 0, 360 range.</summary>
        protected static double NormalizeDegrees(double value)
        {
            value = value % 360d;
            if (value < 0d)
                value += 360d;
            return value;
        }

        /// <summary>
        /// Converts radians to degrees.</summary>
        protected static double RadToDeg(double x)
        {
            return x * 180d / PI;
        }

        /// <summary>
        /// Converts degrees to radians.</summary>
        protected static double DegToRad(double x)
        {
            return x * PI / 180d;
        }

        /// <summary>
        /// Gets sinus from a angle in degrees.</summary>
        protected static double SinDeg(double x)
        {
            return Math.Sin(DegToRad(x));
        }

        /// <summary>
        /// Gets cosinus from a angle in degrees.</summary>
        protected static double CosDeg(double x)
        {
            return Math.Cos(DegToRad(x));
        }

        /// <summary>
        /// Gets aTan from two angles in degrees.</summary>
        protected static double Atan2Deg(double y, double x)
        {
            return RadToDeg(Math.Atan2(y,x));
        }

        // Public --------------------------------------------------------------------

        public const double J2000 = 2451545.0d;

        /// <summary>
        /// Converts from ecliptic to equatorial spherical coordinates, in radians.</summary>
        /// <param name="lon">Ecliptic longitude</param>
        /// <param name="lat">Ecliptic latitude</param>
        /// <param name="rasc">Right ascension</param>
        /// <param name="decl">Declination</param>
        public static void ConvertEclipticToEquatorialRad(
                double lon, double lat,
                out double rasc, out double decl)
        {
            double ecl = DegToRad(23.439281d);

            double x = Math.Cos(lon) * Math.Cos(lat);
            double y = Math.Cos(ecl) * Math.Sin(lon) * Math.Cos(lat) - Math.Sin(ecl) * Math.Sin(lat);
            double z = Math.Sin(ecl) * Math.Sin(lon) * Math.Cos(lat) + Math.Cos(ecl) * Math.Sin(lat);

            double r = Math.Sqrt(x * x + y * y);
            rasc = Math.Atan2(y, x);
            decl = Math.Atan2(z, r);
        }

        public static void ConvertRectangularToSpherical(
                double x, double y, double z,
                out double rasc, out double decl, out double dist)
        {
            dist = Math.Sqrt(x * x + y * y + z * z);
            rasc = Atan2Deg(y, x);
            decl = Atan2Deg(z, Math.Sqrt(x * x + y * y));
        }

        public static void ConvertSphericalToRectangular(
                double rasc, double decl, double dist,
                out double x, out double y, out double z)
        {
            x = dist * CosDeg(rasc) * CosDeg(decl);
            y = dist * SinDeg(rasc) * CosDeg(decl);
            z = dist * SinDeg(decl);
        }

        /// <summary>
        /// Converts from equatorial to horizontal coordinates.
        /// This function converts from angles relative to the earth's equator
        /// to angle relative to the horizon at a given point.</summary>
        /// <param name="jday">Astronomical time as julian day.</param>
        /// <param name="longitude">Observer's longitude in degrees east.</param>
        /// <param name="latitude">Observer's latitude in degrees north.</param>
        /// <param name="rasc">Object's right ascension.</param>
        /// <param name="decl">Object's declination.</param>
        /// <param name="azimuth">Object's azimuth (clockwise degrees from true north).</param>
        /// <param name="altitude">Object's altitude (degrees above the horizon).</param>
        public static void ConvertEquatorialToHorizontal(
                double jday,
                double longitude, double latitude,
                double rasc, double decl,
                out double azimuth, out double altitude)
        {
            double d = jday - 2451543.5d;
            double w = 282.9404d + 4.70935E-5d * d;
            double M = 356.0470d + 0.9856002585d * d;
            // Sun's mean longitude
            double L = w + M;
            // Universal time of day in degrees.
            double UT = (d % 1) * 360d;
            double hourAngle = longitude + L + 180d + UT - rasc;

            double x = CosDeg(hourAngle) * CosDeg(decl);
            double y = SinDeg(hourAngle) * CosDeg(decl);
            double z = SinDeg(decl);

            double xhor = x * SinDeg(latitude) - z * CosDeg(latitude);
            double yhor = y;
            double zhor = x * CosDeg(latitude) + z * SinDeg(latitude);

            azimuth = Atan2Deg(yhor, xhor) + 180d;
            altitude = Atan2Deg(zhor, Math.Sqrt(xhor * xhor + yhor * yhor));
        }

        /// <summary>
        /// Gets the sun's position in the sky in, relative to the horizon.</summary>
        /// <param name="jday"> Astronomical time as julian day.</param>
        /// <param name="longitude">Observer's longitude</param>
        /// <param name="latitude">Observer's latitude</param>
        /// <param name="azimuth">Astronomical azimuth, measured clockwise from North = 0.</param>
        /// <param name="altitude">Astronomical altitude, elevation above the horizon.</param>
        public static void GetHorizontalSunPosition(
                double jday,
                double longitude, double latitude,
                out double azimuth, out double altitude)
        {
            // Midnight at the start of 31 december 2000 
            // 2451543.5 == Astronomy::getJulianDayFromGregorianDateTime(1999, 12, 31, 0, 0, 0));
            double d = jday - 2451543.5d;

            // Sun's Orbital elements:
            // argument of perihelion
            double w = 282.9404d + 4.70935E-5d * d;
            // eccentricity (0=circle, 0-1=ellipse, 1=parabola)
            double e = 0.016709d - 1.151E-9d * d;
            // mean anomaly (0 at perihelion; increases uniformly with time)
            double M = 356.0470d + 0.9856002585d * d;
            // Obliquity of the ecliptic.
            //double oblecl = double (23.4393 - 3.563E-7 * d);

            // Eccentric anomaly
            double E = M + RadToDeg(e * SinDeg(M) * (1d + e * CosDeg(M)));

            // Sun's Distance(R) and true longitude(L)
            double xv = CosDeg(E) - e;
            double yv = SinDeg(E) * Math.Sqrt(1d - e * e);
            //double r = sqrt (xv * xv + yv * yv);
            double lon = Atan2Deg(yv, xv) + w;
            double lat = 0d;

            double lambda = DegToRad(lon);
            double beta = DegToRad(lat);
            double rasc, decl;
            ConvertEclipticToEquatorialRad(lambda, beta, out rasc, out decl);
            rasc = RadToDeg(rasc);
            decl = RadToDeg(decl);

            // Horizontal spherical.
            ConvertEquatorialToHorizontal(jday, longitude, latitude, rasc, decl, out azimuth, out altitude);
        }

        public static void GetHorizontalSunPosition(
                double jday,
                Degree longitude, Degree latitude,
                out Degree azimuth, out Degree altitude)
        {
            double az, al;
            GetHorizontalSunPosition(jday, longitude, latitude, out az, out al);
            azimuth = new Degree(Convert.ToSingle(az));
            altitude = new Degree(Convert.ToSingle(al));
        }

        /// <summary> 
        /// Gets the moon position at a specific time in ecliptic coordinates.</summary>
        /// <param name="lon">Ecliptic longitude, in radians.</param>
        /// <param name="lat">Ecliptic latitude, in radians.</param>
        /// <param name="jday">Astronomical time as julian day.</param>
        public static void GetEclipticMoonPositionRad(
                double jday,
                out double lon,
                out double lat)
        {
            // Julian centuries since January 1, 2000
            double T = (jday - 2451545d) / 36525d;
            double lprim = 3.8104d + 8399.7091d * T;
            double mprim = 2.3554d + 8328.6911d * T;
            double m = 6.2300d + 648.3019d * T;
            double d = 5.1985d + 7771.3772d * T;
            double f = 1.6280d + 8433.4663d * T;
            lon = lprim
                    + 0.1098d * Math.Sin(mprim)
                    + 0.0222d * Math.Sin(2.0d * d - mprim)
                    + 0.0115d * Math.Sin(2.0d * d)
                    + 0.0037d * Math.Sin(2.0d * mprim)
                    - 0.0032d * Math.Sin(m)
                    - 0.0020d * Math.Sin(2.0d * f)
                    + 0.0010d * Math.Sin(2.0d * d - 2.0d * mprim)
                    + 0.0010d * Math.Sin(2.0d * d - m - mprim)
                    + 0.0009d * Math.Sin(2.0d * d + mprim)
                    + 0.0008d * Math.Sin(2.0d * d - m)
                    + 0.0007d * Math.Sin(mprim - m)
                    - 0.0006d * Math.Sin(d)
                    - 0.0005d * Math.Sin(m + mprim);
            lat =
                    +0.0895d * Math.Sin(f)
                    + 0.0049d * Math.Sin(mprim + f)
                    + 0.0048d * Math.Sin(mprim - f)
                    + 0.0030d * Math.Sin(2.0d * d - f)
                    + 0.0010d * Math.Sin(2.0d * d + f - mprim)
                    + 0.0008d * Math.Sin(2.0d * d - f - mprim)
                    + 0.0006d * Math.Sin(2.0d * d + f);
        }

        public static void GetHorizontalMoonPosition(
                double jday,
                double longitude, double latitude,
                out double azimuth, out double altitude)
        {
            // Ecliptic spherical
            double lonecl, latecl;
            GetEclipticMoonPositionRad(jday, out lonecl, out latecl);

            // Equatorial spherical
            double rasc, decl;
            ConvertEclipticToEquatorialRad(lonecl, latecl, out rasc, out decl);

            // Radians to degrees (all angles are in radians up to this point)
            rasc = RadToDeg(rasc);
            decl = RadToDeg(decl);

            // Equatorial to horizontal
            ConvertEquatorialToHorizontal(jday, longitude, latitude, rasc, decl, out azimuth, out altitude);
        }

        public static void GetHorizontalMoonPosition(
                double jday,
                Degree longitude, Degree latitude,
                out Degree azimuth, out Degree altitude)
        {
            double az, al;
            GetHorizontalMoonPosition(jday, longitude, latitude, out az, out al);
            azimuth = new Degree(Convert.ToSingle(az));
            altitude = new Degree(Convert.ToSingle(al));
        }

        /// <summary>Gets astronomical julian day from normal gregorian calendar.
        /// From wikipedia: the integer number of days that have elapsed
        /// since the initial epoch defined as
        /// noon Universal Time (UT) Monday, January 1, 4713 BC.</summary>
        /// <remarks>This is the time at noon, not midnight.</remarks>
        public static int GetJulianDayFromGregorianDate(
                int year, int month, int day)
        {
            // Formulas from http://en.wikipedia.org/wiki/Julian_day
            // These are all integer divisions, but I'm not sure it works
            // correctly for negative values.
            int a = (14 - month) / 12;
            int y = year + 4800 - a;
            int m = month + 12 * a - 3;
            return day + (153 * m + 2) / 5 + 365 * y + y / 4 - y / 100 + y / 400 - 32045;
        }

        /// <summary>
        /// Gets astronomical julian day from normal gregorian calendar.
        /// Calculates julian day from a day in the normal gregorian calendar.
        /// Time should be given as UTC.
        /// http://en.wikipedia.org/wiki/Julian_day</summary>
        public static double GetJulianDayFromGregorianDateTime(
                int year, int month, int day,
                int hour, int minute, double second)
        {
            int jdn = GetJulianDayFromGregorianDate(year, month, day);
            // These are NOT integer divisions.
            double jd = jdn + (hour - 12d) / 24.0d + minute / 1440.0d + second / 86400.0d;

            return jd;
        }

        /// <summary>
        /// Gets astronomical julian day from normal gregorian calendar.</summary>
        public static double GetJulianDayFromGregorianDateTime(
                int year, int month, int day,
                double secondsFromMidnight)
        {
            int jdn = GetJulianDayFromGregorianDate(year, month, day);
            double jd = jdn + secondsFromMidnight / 86400.0d - 0.5d;
            return jd;
        }

        /// <summary>
        /// Gets gregorian date from integer julian day.</summary>
        public static void GetGregorianDateFromJulianDay(
                int julianDay, out int year, out int month, out int day)
        {
            // From http://en.wikipedia.org/wiki/Julian_day
            int J = julianDay;
            int j = J + 32044;
            int g = j / 146097;
            int dg = j % 146097;
            int c = (dg / 36524 + 1) * 3 / 4;
            int dc = dg - c * 36524;
            int b = dc / 1461;
            int db = dc % 1461;
            int a = (db / 365 + 1) * 3 / 4;
            int da = db - a * 365;
            int y = g * 400 + c * 100 + b * 4 + a;
            int m = (da * 5 + 308) / 153 - 2;
            int d = da - (m + 4) * 153 / 5 + 122;
            year = y - 4800 + (m + 2) / 12;
            month = (m + 2) % 12 + 1;
            day = d + 1;
        }

        /// <summary>
        /// Gets gregorian date time from doubleing point julian day.</summary>
        public static void GetGregorianDateTimeFromJulianDay(
                double julianDay, out int year, out int month, out int day,
                out int hour, out int minute, out double second)
        {
            // Integer julian days are at noon.
            int ijd = System.Convert.ToInt32(Math.Floor(julianDay + 0.5d));
            GetGregorianDateFromJulianDay(ijd, out year, out month, out day);

            double s = (julianDay + 0.5d - ijd) * 86400.0d;
            hour = System.Convert.ToInt32((Math.Floor(s / 3600d)));
            s -= hour * 3600d;
            minute = System.Convert.ToInt32((Math.Floor(s / 60d)));
            s -= minute * 60d;
            second = s;
        }

        /// <summary>
        /// Gets gregorian date from doubleing point julian day.</summary>
        public static void GetGregorianDateFromJulianDay(
                double julianDay, out int year, out int month, out int day)
        {
            int hour;
            int minute;
            double second;
            GetGregorianDateTimeFromJulianDay(julianDay, out year, out month, out day, out hour, out minute, out second);
        }

        /// <summary>
        /// Enter high-precission floating-point mode.</summary>
        /// <remarks>Must be paired with restoreFloatingPointMode.</remarks>
        /// <returns>Value to pass to restoreFloatingModeMode.</returns>
        public static int EnterHighPrecissionFloatingPointMode()
        {
            int oldMode = _controlfp(0, 0);
            _controlfp(_PC_64, _MCW_PC);
            return oldMode;
        }

        /// <summary>
        /// Restore old floating point precission.</summary>
        public static void RestoreFloatingPointMode(int oldMode)
        {
            _controlfp(oldMode, _MCW_PC);
        }
    }
}