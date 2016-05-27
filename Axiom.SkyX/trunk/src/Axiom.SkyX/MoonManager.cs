#region MIT/X11 License
// This file is part of the Axiom.SkyX project
//
// Copyright (c) 2009 Michael Cummings <cummings.michael@gmail.com>
// Copyright (c) 2009 Bostich <bostich83@googlemail.com>

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
// Axiom.SkyX is a reimplementation of the SkyX project for .Net/Mono
// SkyX is Copyright (C) 2009 Xavier Verguín González <xavierverguin@hotmail.com> <xavyiy@gmail.com>
#endregion MIT/X11 License

using System;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
namespace Axiom.SkyX
{
    public class MoonManager
    {
        /// <summary>
        /// 
        /// </summary>
        private BillboardSet _moonBillboard;
        /// <summary>
        /// 
        /// </summary>
        private SceneNode _moonSceneNode;
        /// <summary>
        /// 
        /// </summary>
        private bool _isCreated;
        /// <summary>
        /// 
        /// </summary>
        private float _moonSize;
        private SkyX _skyX;
        /// <summary>
        /// 
        /// </summary>
        public bool IsCreated
        {
            get { return _isCreated; }
            private set { _isCreated = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public BillboardSet MoonBillboard
        {
            get { return _moonBillboard; }
            private set { _moonBillboard = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public SceneNode MoonSceneNode
        {
            get { return _moonSceneNode; }
            set { _moonSceneNode = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float MoonSize
        {
            get { return _moonSize; }
            set 
            { 
                _moonSize = value;
                if (!this.IsCreated)
                {
                    return;
                }

                UpdateBounds();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public SkyX SkyX
        {
            get { return _skyX; }
            private set { _skyX = value; }
        }

        #region Construction and Destruction

        public MoonManager( SkyX skyX )
        {
            this.SkyX = skyX;
            this.MoonSize = 0.225f;
        }

        #endregion Construction and Destruction
        /// <summary>
        /// 
        /// </summary>
        public void Create()
        {
            if (this.IsCreated)
            {
                return;
            }

            this.MoonSceneNode = this.SkyX.SceneManager.RootSceneNode.CreateChildSceneNode();

            this.MoonBillboard = this.SkyX.SceneManager.CreateBillboardSet("SkyXMoonBillboardSet", 1);
            this.MoonBillboard.MaterialName = this.SkyX.GpuManager.MoonMaterialName;
            this.MoonBillboard.BillboardType = BillboardType.OrientedCommon;
            this.MoonBillboard.RenderQueueGroup = RenderQueueGroupID.SkiesEarly + 1;
            this.MoonBillboard.CastShadows = false;

            this.MoonBillboard.CreateBillboard(new Vector3(0, 0, 0));

            this.MoonSceneNode.AttachObject(this.MoonBillboard);
            this.MoonSceneNode.Position = this.SkyX.Camera.DerivedPosition;

            this.IsCreated = true;

            UpdateBounds();
            Update();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (!this.IsCreated)
            {
                return;
            }

            float radius = this.SkyX.Camera.Far * 0.95f;
            float size = radius * this.MoonSize;

            this.MoonBillboard.CommonDirection = (this.SkyX.AtmosphereManager.SunDirection).NormalizedCopy().Perpendicular();

            Vector3 moonRelativePos = this.SkyX.AtmosphereManager.SunDirection *
                Utility.Cos(Utility.ASin((size / 2.0f) / radius)) * radius;

            this.MoonSceneNode.Position = this.SkyX.Camera.DerivedPosition + moonRelativePos;

            if (moonRelativePos.y < -size / 2)
            {
                this.MoonSceneNode.IsVisible = false;
            }
            else
            {
                this.MoonSceneNode.IsVisible = true;

                Material mat = (Material)MaterialManager.Instance.GetByName("SkyX_Moon");
                mat.GetTechnique(0).GetPass(0).VertexProgramParameters.SetNamedConstant("uSkydomeCenter", this.SkyX.Camera.DerivedPosition);
            }
        }
        public void Remove()
        {
        }
        internal void UpdateBounds()
        {
            float radius = this.SkyX.Camera.Far * 0.95f;
            float size = radius * this.MoonSize;

            this.MoonBillboard.SetDefaultDimensions(size, size);
            //this.MoonBillboard.BoundingBox = new AxisAlignedBox(
            //    new Vector3(-size / 1, -size / 1, -size / 1),
            //    new Vector3(size / 1, size / 1, size / 1));
            
            this.MoonSceneNode.NeedUpdate();
        }
    }
}
