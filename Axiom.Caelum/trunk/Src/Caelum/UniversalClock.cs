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
namespace Caelum
{
    /// <summary>
    /// The system's time model.
    /// This class is responsible of keeping track of the current time, transform it to the system's time model, 
    /// and return the values associated (time of day, day of year...)</summary>
    public class UniversalClock
    {
        // Attributes -----------------------------------------------------------------

        /// <summary>
        /// Astronomical julian day at mCurrentTime = 0;</summary>
        protected double mJulianDayBase = 0d;

        /// <summary>
        /// Seconds since mJulianDayBase.</summary>
        protected double mCurrentTime = 0d;

        /// <summary>
        /// Seconds since mJulianDayBase at last update.</summary>
        protected double mLastTime = 0d;


        protected DateTime lastUpdateTime;


        private static UniversalClock mInstance = null;

        // Methods --------------------------------------------------------------------

        public UniversalClock()
        {
            lastUpdateTime = DateTime.Now;
            mInstance = this;
        }

        ~UniversalClock()
        {
            Dispose();
        }

        public void Dispose()
        {
            mInstance = null;
        }

        public static UniversalClock Instance
        {
            get { return mInstance; }
        }
        
        /// <summary>
        /// The current time as a julian day.</summary>
        public double JulianDay
        {
            get 
            { 
                int fpmode = Astronomy.EnterHighPrecissionFloatingPointMode ();
                double res =  mJulianDayBase + mCurrentTime / 86400d;
                Astronomy.RestoreFloatingPointMode(fpmode);
                return res;
            }
            set
            {
                mJulianDayBase = value;
                mCurrentTime = 0d;
            }
        }

        /// <summary>
        /// Gets the difference in seconds between this and the last update.
        /// This is what you want for per-frame updates.</summary>
        public float getJulianSecondDifference()
        {
            return Convert.ToSingle(mCurrentTime - mLastTime);
        }

        public void SetGregorianDateTime(
                int year, int month, int day)
        {
            SetGregorianDateTime(year, month, day, 0, 0, 0);
        }

        /// <summary>
        /// Sets the current time as a gregorian date.
        /// This is here as an easy to use function.</summary>
        public void SetGregorianDateTime(
                int year, int month, int day,
                int hour, int minute, double second)
        {
            int fpmode = Astronomy.EnterHighPrecissionFloatingPointMode ();
            JulianDay = Astronomy.GetJulianDayFromGregorianDateTime( year, month, day,
                                                                     hour, minute, second);
            Astronomy.RestoreFloatingPointMode(fpmode);
        }

        /// <summary>
        /// Updates the clock.</summary>
        /// <remarks> The time to be added to the clock is automatically
        /// calculated from the system clock. This will be affected 
        /// by the time scale.</remarks>
        public void Update(bool onGame, float delta)
        {
            if (CaelumManager.Instance == null)
                return;

            double timeScale = CaelumManager.Instance.TimeScale;

            // Calculate the time to be added
            if (!onGame)
            {
                TimeSpan inter = DateTime.Now - lastUpdateTime;
                delta = Convert.ToSingle(inter.TotalSeconds);
                lastUpdateTime = DateTime.Now;
            }

            mLastTime = mCurrentTime;
            mCurrentTime += delta * timeScale;
        }
    }
}
