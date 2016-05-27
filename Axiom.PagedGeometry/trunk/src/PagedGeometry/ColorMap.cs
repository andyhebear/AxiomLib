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
    /// A 2D greyscale image that is assigned to a certain region of your world to represent color levels.
    /// </summary>
    /// <remarks>
    /// This class is used by various PagedLoader's internally, so it's not necessary to learn anything about this class.
    /// However, you can achieve more advanced effects through the ColorMap class interface than you can with the standard
    /// GrassLayer color map functions, for example.
    /// 
    /// Probably the most useful function in this class is getPixelBox(), which you can use to directly manipulate the
    /// color map pixels in real-time.
    /// </remarks>
    public class ColorMap : IDisposable
    {
        private static Dictionary<string, ColorMap> mSelfList = new Dictionary<string, ColorMap>();
        private string mSelfKey = string.Empty;
        private uint mRefCount = 0;
        private MapFilter mFilter;
        private PixelBox mPixels;
        private TBounds mMapBounds;
        private IntPtr mPixelPtr = IntPtr.Zero;
        /// <summary>
        /// Get's or set's the filtering mode used for this color map.
        /// </summary>
        public MapFilter Filter
        {
            get { return mFilter; }
            set { mFilter = value; }
        }

        /// <summary>
        /// Get's or set's the boundaries that this color map affects.
        /// </summary>
        public TBounds MapBounds
        {
            set { mMapBounds = value; }
            get { return mMapBounds; }
        }

        /// <summary>
        /// 
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
        private ColorMap(Texture texture, MapChannel channel)
        {
            Debug.Assert(texture != null);
            mFilter = MapFilter.Bilinear;
            //Add self to selfList
            mSelfKey = texture.Name + (int)channel;
            mSelfList.Add(mSelfKey, this);
            mRefCount = 0;

            //Get the texture buffer
            HardwarePixelBuffer buff = texture.GetBuffer();

#warning Root::getSingleton().getRenderSystem()->getColourVertexElementType();

            //Prepare a PixelBox (24-bit RGB) to receive the color values
            VertexElementType format = VertexElementType.Color_ARGB;
            switch (format)
            {
                case VertexElementType.Color_ARGB:
                    //DirectX9
                    mPixels = new PixelBox(new BasicBox(0, 0, buff.Width, buff.Height), PixelFormat.A8B8G8R8);
                    break;
                case VertexElementType.Color_ABGR:
                    //OpenGL
                    mPixels = new PixelBox(new BasicBox(0, 0, buff.Width, buff.Height), PixelFormat.A8B8G8R8);
                    //Patch for Ogre's incorrect blitToMemory() when copying from PF_L8 in OpenGL
                    if (buff.Format == PixelFormat.L8)
                        channel = MapChannel.Red;
                    break;
                default:
                    throw new Exception("Unknown RenderSystem color format");
            }

            byte[] pixelData = new byte[mPixels.ConsecutiveSize];
            mPixelPtr = Memory.PinObject(pixelData);
            mPixels.Data = mPixelPtr;

            if (channel == MapChannel.Color)
            {
                //Copy to the color map directly if no channel extraction is necessary
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
                        *outputPtr++ = *inputPtr;
                        *outputPtr++ = *inputPtr;
                        *outputPtr++ = *inputPtr;
                        *outputPtr++ = 0xFF;	//Full alpha
                        inputPtr += 4;
                    }

                    //Finally, delete the temporary PF_R8G8B8A8 pixel buffer
                    Memory.UnpinObject(tmpPix);
                    tmpPixels = null;
                    tmpPix = null;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ColorMap Load(string fileName)
        {
            return Load(fileName, MapChannel.Color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static ColorMap Load(string fileName, MapChannel channel)
        {
            //Load image
            Texture map = TextureManager.Instance.Load(fileName, ResourceGroupManager.DefaultResourceGroupName);

            //Copy image to pixelbox
            return Load(map, channel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static ColorMap Load(Texture texture)
        {
            return Load(texture, MapChannel.Color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static ColorMap Load(Texture texture, MapChannel channel)
        {
            string key = texture.Name + (int)channel;
            ColorMap m = null;
            if (!mSelfList.TryGetValue(key, out m))
            {
                m = new ColorMap(texture, channel);
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
        /// Gets the color value at the specified position
        /// </summary>
        /// <param name="x">x postion</param>
        /// <param name="z">z position</param>
        /// <returns>color value as uint at the specified position</returns>
        public uint GetColorAt(float x, float z)
        {
            if (mFilter == MapFilter.None)
                return _GetColorAt(x, z);
            else
                return GetColorAtBilinear(x, z);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public ColorEx GetColorAtUnpacked(float x, float z)
        {
            uint c = 0;
            if (mFilter == MapFilter.None)
                c = _GetColorAt(x, z);
            else
                c = GetColorAtBilinear(x, z);

            float r = 0, g = 0, b = 0, a = 0;
#warning fix me!Ogre::Root::getSingleton().getRenderSystem()->getColourVertexElementType();

            b = ((c) & 0xFF) / 255.0f;
            g = ((c >> 8) & 0xFF) / 255.0f;
            r = ((c >> 16) & 0xFF) / 255.0f;
            a = ((c >> 24) & 0xFF) / 255.0f;

            return new ColorEx(a, r, g, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <param name="ratio"></param>
        /// <param name="rationInv"></param>
        /// <returns></returns>
        private uint InterpolateColor(uint color1, uint color2, float ratio, float ratioInv)
        {
            byte a1, b1, c1, d1;
            a1 = (byte)(color1 & 0xFF);
            b1 = (byte)(color1 >> 8 & 0xFF);
            c1 = (byte)(color1 >> 16 & 0xFF);
            d1 = (byte)(color1 >> 24 & 0xFF);

            byte a2, b2, c2, d2;
            a2 = (byte)(color2 & 0xFF);
            b2 = (byte)(color2 >> 8 & 0xFF);
            c2 = (byte)(color2 >> 16 & 0xFF);
            d2 = (byte)(color2 >> 24 & 0xFF);

            byte a, b, c, d;
            a = (byte)(ratioInv * a1 + ratio * a2);
            b = (byte)(ratioInv * b1 + ratio * b2);
            c = (byte)(ratioInv * c1 + ratio * c2);
            d = (byte)(ratioInv * d1 + ratio * d2);

            uint clr = (uint)(a | (b << 8) | (c << 16) | (d << 24));
            return clr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private uint _GetColorAt(float x, float z)
        {
            Debug.Assert(mPixels != null);

            uint mapWidth = (uint)mPixels.Width;
            uint mapHeight = (uint)mPixels.Height;
            float boundsWidth = mMapBounds.Width;
            float boundsHeight = mMapBounds.Height;

            uint xindex = (uint)(mapWidth * (x - mMapBounds.Left) / boundsWidth);
            uint zindex = (uint)(mapHeight * (z - mMapBounds.Top) / boundsHeight);
            if (xindex < 0 || zindex < 0 || xindex >= mapWidth || zindex >= mapHeight)
                return 0xFFFFFFFF;

            unsafe
            {
                uint* data = (uint*)mPixels.Data;
                return data[mapWidth * zindex + xindex];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private uint GetColorAtBilinear(float x, float z)
        {
            Debug.Assert(mPixels != null);

            uint mapWidth = (uint)mPixels.Width;
            uint mapHeight = (uint)mPixels.Height;
            float boundsWidth = mMapBounds.Width;
            float boundsHeight = mMapBounds.Height;

            float xIndexFloat = (mapWidth * (x - mMapBounds.Left) / boundsWidth) - 0.5f;
            float zIndexFloat = (mapHeight * (z - mMapBounds.Top) / boundsHeight) - 0.5f;

            uint xIndex = (uint)xIndexFloat;
            uint zIndex = (uint)zIndexFloat;
            if (xIndex < 0 || zIndex < 0 || xIndex >= mapWidth - 1 || zIndex >= mapHeight - 1)
                return 0;

            float xRatio = xIndexFloat - xIndex;
            float xRatioInv = 1 - xRatio;
            float zRatio = zIndexFloat - zIndex;
            float zRatioInv = 1 - zRatio;
            unsafe
            {
                uint* data = (uint*)mPixels.Data;

                uint val11 = data[mapWidth * zIndex + xIndex];
                uint val21 = data[mapWidth * zIndex + xIndex + 1];
                uint val12 = data[mapWidth * ++zIndex + xIndex];
                uint val22 = data[mapWidth * zIndex + xIndex + 1];

                uint val1 = InterpolateColor(val11, val21, xRatio, xRatioInv);
                uint val2 = InterpolateColor(val12, val22, xRatio, xRatioInv);

                uint val = InterpolateColor(val1, val2, zRatio, zRatioInv);

                return val;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Memory.UnpinObject(mPixelPtr);
            mSelfList.Remove(mSelfKey);
        }
    }
}
