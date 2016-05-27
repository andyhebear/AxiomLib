#region MIT/X11 License
// This file is part of the Axiom.PagedGeometry project
// Copyright (c) 2009 Bostich <bostich83@googlemail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Axiom.PagedGeometry is a reimplementation of the PagedGeometry project for .Net/Mono
// PagedGeometry is Copyright (C) 2006 John Judnich
#endregion MIT/X11 License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Axiom.Media;
using Axiom.Core;
using Axiom.Graphics;
namespace Axiom.Forests
{
    /// <summary>
    /// A 2D greyscale image that is assigned to a certain region of your world to represent density levels.
    /// </summary>
    /// <remarks>
    /// This class is used by various PagedLoader's internally, so it's not necessary to learn anything about this class.
    /// However, you can achieve more advanced effects through the DensityMap class interface than you can with the standard
    /// GrassLayer density map functions, for example.
    /// 
    /// Probably the most useful function in this class is getPixelBox(), which you can use to directly manipulate the
    /// density map pixels in real-time.
    /// </remarks>
    public class DensityMap : IDisposable
    {
        private static Dictionary<string, DensityMap> mSelfList = new Dictionary<string, DensityMap>();
        private string mSelfKey = string.Empty;
        private uint mRefCount = 0;
        private MapFilter mFilter;
        private PixelBox mPixels;
        private TBounds mMapBounds;
        private IntPtr mPixelPtr = IntPtr.Zero;
        /// <summary>
        /// Get's or set's the filtering mode used for this density map.
        /// </summary>
        public MapFilter Filter
        {
            set { mFilter = value; }
            get { return mFilter; }
        }

        /// <summary>
        /// Get's or set's the boundaries that this density map affects.
        /// </summary>
        public TBounds MapBounds
        {
            set { mMapBounds = value; }
            get { return mMapBounds; }
        }
        /// <summary>
        /// Get's the pixel data of the density map
        /// </summary>
        public PixelBox PixelBox
        {
            get
            {
                Debug.Assert(mPixels != null);
                return mPixels;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="channel"></param>
        private DensityMap(Texture texture, MapChannel channel)
        {
            Debug.Assert(texture != null);
            mFilter = MapFilter.Bilinear;

            //Add self to selfList
            mSelfKey = texture.Name + (int)channel;
            mSelfList.Add(mSelfKey, this);
            mRefCount = 0;

            //Get the texture buffer
            HardwarePixelBuffer buff = texture.GetBuffer();

            //Prepare a PixelBox (8-bit greyscale) to receive the density values
            mPixels = new PixelBox(new BasicBox(0, 0, buff.Width, buff.Height), PixelFormat.BYTE_L);
            byte[] pixelData = new byte[mPixels.ConsecutiveSize];
            mPixelPtr = Memory.PinObject(pixelData);
            mPixels.Data = mPixelPtr;

            if (channel == MapChannel.Color)
            {
                //Copy to the greyscale density map directly if no channel extraction is necessary
                buff.BlitToMemory(mPixels);
            }
            else
            {
                unsafe
                {
                    //If channel extraction is necessary, first convert to a PF_R8G8B8A8 format PixelBox
                    //This is necessary for the code below to properly extract the desired channel
                    PixelBox tmpPixels = new PixelBox(new BasicBox(0, 0, buff.Width, buff.Height), PixelFormat.R8G8B8A8);
                    byte[] tmpPix = new byte[tmpPixels.ConsecutiveSize];
                    byte* pixPtr = (byte*)Memory.PinObject(tmpPix);
                    tmpPixels.Data = (IntPtr)pixPtr;
                    buff.BlitToMemory(tmpPixels);

                    //Pick out a channel from the pixel buffer
                    int channelOffset = 0;
                    switch (channel)
                    {
                        case MapChannel.Red:
                            channelOffset = 3;
                            break;
                        case MapChannel.Green:
                            channelOffset = 2;
                            break;
                        case MapChannel.Blue:
                            channelOffset = 1;
                            break;
                        case MapChannel.Alpha:
                            channelOffset = 0;
                            break;
                        default:
                            //should never happen
                            throw new Exception("Invalid channel");
                    }

                    //And copy that channel into the density map
                    byte* inputPtr = (byte*)pixPtr + channelOffset;
                    byte* outputPtr = (byte*)pixPtr + channelOffset;
                    byte* outputEndPtr = outputPtr + mPixels.ConsecutiveSize;
                    while (outputPtr != outputEndPtr)
                    {
                        *outputPtr++ = *inputPtr++;
                        inputPtr += 4;
                    }

                    //Finally, delete the temporary PF_R8G8B8A8 pixel buffer
                    Memory.UnpinObject(tmpPix);
                    tmpPixels = null;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DensityMap Load(string fileName)
        {
            return Load(fileName, MapChannel.Color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static DensityMap Load(string fileName, MapChannel channel)
        {
            //load image
            Texture map = TextureManager.Instance.Load(fileName, ResourceGroupManager.DefaultResourceGroupName);

            //Copy image to pixelbox
            return Load(map, channel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static DensityMap Load(Texture texture)
        {
            return Load(texture, MapChannel.Color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static DensityMap Load(Texture texture, MapChannel channel)
        {
            string key = texture.Name + (int)channel;
            DensityMap m = null;
            if (!mSelfList.TryGetValue(key, out m))
            {
                m = new DensityMap(texture, channel);
            }
            ++(m.mRefCount);
            return m;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Unload()
        {
            --mRefCount;
            if (mRefCount == 0)
                Dispose();
        }

        /// <summary>
        /// Get's the density level at the specified position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float GetDensityAt(float x, float z)
        {
            if (mFilter == MapFilter.None)
                return GetDensityAtUnfiltered(x, z);
            else
                return GetDensityAtBilenear(x, z);
        }

        /// <summary>
        /// Returns the density map value at the given location
        /// Make sure a density map exists before calling this.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float GetDensityAtUnfiltered(float x, float z)
        {
            Debug.Assert(mPixels != null);
            float val = 0.0f;
            uint mapWidth = (uint)mPixels.Width;
            uint mapHeight = (uint)mPixels.Height;
            float boundsWitdh = mMapBounds.Width;
            float boundsHeight = mMapBounds.Height;

            uint xindex = (uint)(mapWidth * (x - mMapBounds.Left) / boundsWitdh);
            uint zindex = (uint)(mapHeight * (z - mMapBounds.Top) / boundsHeight);

            //check if outside
            if (xindex < 0 || zindex < 0 || zindex >= mapWidth || zindex >= mapHeight)
                return val;

            unsafe
            {
                byte* data = (byte*)mPixels.Data;
                val = data[mapWidth * zindex + xindex] / 255.0f;
            }
            return val;
        }
        /// <summary>
        ///Returns the density map value at the given location with bilinear filtering
        /// Make sure a density map exists before calling this.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float GetDensityAtBilenear(float x, float z)
        {
            Debug.Assert(mPixels != null);

            float val = 0.0f;
            uint mapWidth = (uint)mPixels.Width;
            uint mapHeight = (uint)mPixels.Height;
            float boundsWitdh = mMapBounds.Width;
            float boundsHeight = mMapBounds.Height;

            float xIndexFloat = (float)((mapWidth * (x - mMapBounds.Left) / boundsWitdh) - 0.5f);
            float zIndexFloat = (float)((mapWidth * (z - mMapBounds.Top) / boundsHeight) - 0.5f);

            uint xIndex = (uint)xIndexFloat;
            uint zIndex = (uint)zIndexFloat;

            //check if outside
            if (xIndex < 0 || zIndex < 0 || zIndex >= mapWidth-1 || zIndex >= mapHeight -1)
                return val;

            float xRatio = xIndexFloat - xIndex;
            float xRatioInv = 1 - xRatio;
            float zRatio = zIndexFloat - zIndex;
            float zRatioInv = 1 - zRatio;

            unsafe
            {
                byte* data = (byte*)mPixels.Data;

                float val11 = data[mapWidth * zIndex + xIndex] / 255.0f;
                float val21 = data[mapWidth * zIndex + xIndex + 1] / 255.0f;
                float val12 = data[mapWidth * ++zIndex + xIndex] / 255.0f;
                float val22 = data[mapWidth * zIndex + xIndex + 1] / 255.0f;

                float val1 = xRatioInv * val11 + xRatio * val21;
                float val2 = xRatioInv * val12 + xRatio * val22;

                val = zRatioInv * val1 + zRatio * val2;
            }

            return val;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Memory.UnpinObject(mPixelPtr);
        }
    }
}
