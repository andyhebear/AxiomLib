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
using Axiom.Math;
using Axiom.Core;
namespace Axiom.Forests
{
    public class TreeRef
    {
        private Vector3 mPosition;
        private Degree mYaw;
        private float mScale;
        private Entity mEntity;
        private object mUserData;

        /// <summary>
        /// Get's the tree's postion.
        /// </summary>
        public Vector3 Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }
        /// <summary>
        /// Get's the tree's yaw as degree value
        /// </summary>
        public Degree Yaw
        {
            get { return mYaw; }
            set { mYaw = value; }
        }
        /// <summary>
        /// Get's the tree's uniform scale value.
        /// </summary>
        public float Scale
        {
            get { return mScale; }
            set { mScale = value; }
        }
        /// <summary>
        /// Get's the tree's orientation as quaternation.
        /// </summary>
        public Quaternion Orientation
        {
            get { return new Quaternion((float)mYaw, 0, 1, 0); }
        }
        /// <summary>
        /// 
        /// </summary>
        public Entity Entity
        {
            get { return mEntity; }
            set { mEntity = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public object UserData
        {
            set { mUserData = value; }
            get { return mUserData; }
        }
    }
}
