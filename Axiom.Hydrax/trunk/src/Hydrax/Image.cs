using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace Axiom.Hydrax
{
    public class Image
    {
        /** Image type enum
		 */
        public enum ImageType
        {
            TYPE_ONE_CHANNEL = 1,
            TYPE_TWO_CHANNELS = 2,
            TYPE_RGB = 3,
            /// Default
            TYPE_RGBA = 4
        };

        /** Channel enum
         */
        public enum Channel
        {
            CHANNEL_R = 0, // Red
            CHANNEL_G = 1, // Green
            CHANNEL_B = 2, // Blue
            CHANNEL_A = 3  // Alpha
        };

        #region - Fields -
        protected ImageType mType;
        protected Size mSize;
        protected int mChannels;
        protected float[] mData;
        #endregion

        #region - Properties -
        /// <summary>
        /// Get's the Type of this Image.
        /// </summary>
        public ImageType Type
        {
            get { return mType; }
        }
        /// <summary>
        /// Get's Size of this Image.
        /// </summary>
        public Size Size
        {
            get { return mSize; }
        }
        #endregion

        public float GetValue(int X, int Y, int C)
        {
            if (C < 0 || C > mChannels ||
                X < 0 || X > mSize.Width ||
                Y < 0 || Y > mSize.Height)
            {
                Hydrax.HydraxLog("Error in Image::getValue, x = " + X.ToString() +
                                " y = " + Y.ToString() 
							   + " Channel = " + C.ToString());
                return 0;
            }
            return mData[(Y * mSize.Width + X) * mChannels + C];
        }
    }
}
