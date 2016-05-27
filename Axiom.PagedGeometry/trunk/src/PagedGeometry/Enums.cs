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
using System.Linq;
using System.Text;

namespace Axiom.Forests
{
    /// <summary>
    ///  A technique used to render grass. Passed to GrassLayer.RenderTechnique.
    /// </summary>
    public enum GrassTechnique
    {
        /// <summary>
        /// Grass constructed of randomly placed and rotated quads
        /// </summary>
        Quad,
        /// <summary>
        /// Grass constructed of two quads forming a "X" cross shape
        /// </summary>
        CrossQuads,
        /// <summary>
        /// Grass constructed of camera-facing billboard quads
        /// </summary>
        Sprite,
    }
    /// <summary>
    ///  A technique used to fade grass into the distance. Passed to GrassLayer.FadeTechnique.
    /// </summary>
    public enum FadeTechnique
    {
        /// <summary>
        /// Grass that fades into the distance with transparency. Fairly effective in most cases.
        /// </summary>
        Alpha,
        /// <summary>
        /// Grass that fades in by "growing" up out of the ground. 
        /// Very effective when grass fades in against the sky, or with alpha-rejected grass.
        /// </summary>
        Grow,
        /// <summary>
        /// Grass that fades in by slowly becoming opaque while it "grows" up out of the ground. 
        /// Effective with alpha grass fading in against the sky.
        /// </summary>
        AlphaGrow,
    }

    /// <summary>
    /// Specifies which color channel(s) to extract from an image
    /// </summary>
    public enum MapChannel
    {
        /// <summary>
        /// Use only the image's red color channel
        /// </summary>
        Red,
        /// <summary>
        /// Use only the image's green color channel
        /// </summary>
        Green,
        /// <summary>
        /// Use only the image's blue color channel
        /// </summary>
        Blue,
        /// <summary>
        /// Use only the image's alpha channel
        /// </summary>
        Alpha,
        /// <summary>
        /// Use the image's full RGB color information
        /// </summary>
        Color,
    }

    /// <summary>
    /// Specifies the filtering method used to interpret property maps
    /// </summary>
    public enum MapFilter
    {
        /// <summary>
        /// Use no filtering - fast, but may appear blocky
        /// </summary>
        None,
        /// <summary>
        /// Use bilinear filtering - slower, but will appear less blocky
        /// </summary>
        Bilinear,
    }
    /// <summary>
    /// 
    /// </summary>
    public enum ImpostorBlendMode
    {
        /// <summary>
        /// 
        /// </summary>
        AlphaReject,
        /// <summary>
        /// 
        /// </summary>
        AlphaBlend,
    }

    /// <summary>
    /// Different methods used to render billboards. This can be supplied as a parameter
    /// to the StaticBillboardSet constructor to manually select how you want billboards
    /// rendered (although in almost all cases Accelerated is the best choice).
    /// </summary>
    public enum BillboardMethod
    {
        /// <summary>
        /// This mode accelerates the performance of billboards by using vertex shaders
	    /// to keep billboards facing the camera. Note: If the computer's hardware is not
	    /// capable	of vertex shaders, it will automatically fall back to Compatible
        /// mode.
        /// </summary>
        Accelerated = 1,
        /// <summary>
        /// Unlike Accelerated, this does not use vertex shaders to align
	    ///  billboards to the camera. This is more compatible with old video cards,
        /// although it can result in poor performance with high amounts of billboards.
        /// </summary>
        Compatible = 2,
    }
}
