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
using System.Collections.Generic;
using Axiom.Math;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Media;
namespace Axiom.SkyX.Clouds
{
    public class DataManager : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public class Cell
        {
            /// <summary>
            /// 
            /// </summary>
            public bool Hum;
            /// <summary>
            /// 
            /// </summary>
            public bool Act;
            /// <summary>
            /// 
            /// </summary>
            public bool Cld;
            /// <summary>
            /// 
            /// </summary>
            public float Phum;
            /// <summary>
            /// 
            /// </summary>
            public float Pext;
            /// <summary>
            /// 
            /// </summary>
            public float Pact;
            /// <summary>
            /// 
            /// </summary>
            public float Dens;
            /// <summary>
            /// 
            /// </summary>
            public float Light;
        }

        /// <summary>
        /// Volumetric textures enumeration
        /// </summary>
        [Flags]
        public enum VolTextureId
        {
            Tex0 = 0,
            Tex1 = 1,
        }
        private Cell[,,] _cellsCurrent;
        private Cell[,,] _cellsTmp;
        private float _currentTransition;
        private float _updateTime;
        private int _nx;
        private int _ny;
        private int _nz;
        private Texture[] _volTextures = new Texture[2];
        private bool _volTexToUpdate;
        private bool _isCreated;
        private FastFakeRandom _ffRandom;
        private int _maxNumberOfClouds;
        private List<Ellipsoid> _ellipsoids = new List<Ellipsoid>();
        private VClouds _vClouds;
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
        public float UpdateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float Interpolation
        {
            get { return _currentTransition / _updateTime; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vc"></param>
        public DataManager(VClouds vc)
        {
            _vClouds = vc;
            _updateTime = 10.0f;
            _maxNumberOfClouds = 250;
            _volTexToUpdate = true;
            _isCreated = false;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Remove();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Remove()
        {
            if (!this.IsCreated)
            {
                return;
            }

            for (int k = 0; k < 2; k++)
            {
                TextureManager.Instance.Remove(_volTextures[k].Name);
                _volTextures[k] = null;
            }

            Delete3DCellArray(_cellsCurrent, _nx, _ny);
            Delete3DCellArray(_cellsTmp, _nx, _ny);

            _ffRandom = null;
            this.IsCreated = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSinceLastFrame"></param>
        public void Update(float timeSinceLastFrame)
        {
            if (_volTexToUpdate)
            {
                _currentTransition += timeSinceLastFrame;

                if (_currentTransition > _updateTime)
                {
                    PerformCalculations(_nx, _ny, _nz);
                    UpdateVolTextureData(_cellsCurrent, VolTextureId.Tex0, _nx, _ny, _nz);

                    _currentTransition = _updateTime;
                    _volTexToUpdate = !_volTexToUpdate;
                }
            }
            else
            {
                _currentTransition -= timeSinceLastFrame;

                if (_currentTransition < 0)
                {
                    PerformCalculations(_nx, _ny, _nz);
                    UpdateVolTextureData(_cellsCurrent, VolTextureId.Tex1, _nx, _ny, _nz);
                    _currentTransition = 0;
                    _volTexToUpdate = !_volTexToUpdate;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        public void Create(int nx, int ny, int nz)
        {
            Remove();
            _nx = nx;
            _ny = ny;
            _nz = nz;

            _ffRandom = new FastFakeRandom(1024, 0, 1);

            InitData(nx, ny, nz);

            for (int k = 0; k < 2; k++)
            {
                CreateVolTexture((VolTextureId)k, nx, ny, nz);
            }

            this.IsCreated = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ForceToUpdateData()
        {
            if (_volTexToUpdate)
            {
                PerformCalculations(_nx, _ny, _nz);
                UpdateVolTextureData(_cellsCurrent, VolTextureId.Tex0, _nx, _ny, _nz);
                _currentTransition = _updateTime;
                _volTexToUpdate = !_volTexToUpdate;
            }
            else
            {
                PerformCalculations(_nx, _ny, _nz);
                UpdateVolTextureData(_cellsCurrent, VolTextureId.Tex1, _nx, _ny, _nz);
                _currentTransition = 0;
                _volTexToUpdate = !_volTexToUpdate;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        private void InitData(int nx, int ny, int nz)
        {
            _cellsCurrent = Create3DCellArray(nx, ny, nz);
            _cellsTmp = Create3DCellArray(nx, ny, nz);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <returns></returns>
        private Cell[,,] Create3DCellArray(int nx, int ny, int nz)
        {
            return Create3DCellArray(nx, ny, nz, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="init"></param>
        /// <returns></returns>
        private Cell[,,] Create3DCellArray(int nx, int ny, int nz, bool init)
        {
            Cell[,,] c = new Cell[nx,ny,nz];

            int u = 0, v = 0, w = 0;
            for (u = 0; u < nx; u++)
            {
                for (v = 0; v < ny; v++)
                {
                    for (w = 0; w < nz; w++)
                    {
                        c[u, v, w] = new Cell();
                    }
                }
            }
            if (!init)
            {
                return c;
            }

            for (u = 0; u < nx; u++)
            {
                for (v = 0; v < ny; v++)
                {
                    for (w = 0; w < nz; w++)
                    {
                        c[u,v,w].Act = false;
                        c[u,v,w].Cld = false;
                        c[u,v,w].Hum = false;

                        c[u,v,w].Pact = 0;
                        c[u,v,w].Pext = 1;
                        c[u,v,w].Phum = 0;

                        c[u,v,w].Dens = 0.0f;
                        c[u,v,w].Light = 0.0f;
                    }
                }
            }

            return c;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        private void Delete3DCellArray(Cell[,,] c, int nx, int ny)
        {
            c = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        private void Copy3DCellArraysData(Cell[,,] or, Cell[,,] dest, int nx, int ny, int nz)
        {
            int u, v, w;

            for (u = 0; u < nx; u++)
            {
                for (v = 0; v < ny; v++)
                {
                    for (w = 0; w < nz; w++)
                    {
                        dest[u,v,w].Act = or[u,v,w].Act;
                        dest[u,v,w].Cld = or[u,v,w].Cld;
                        dest[u,v,w].Hum = or[u,v,w].Hum;

                        dest[u,v,w].Pact = or[u,v,w].Pact;
                        dest[u,v,w].Pext = or[u,v,w].Pext;
                        dest[u,v,w].Phum = or[u,v,w].Phum;

                        dest[u,v,w].Dens = or[u,v,w].Dens;
                        dest[u,v,w].Light = or[u,v,w].Light;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="humidity"></param>
        /// <param name="averageCloudSize"></param>
        public void SetWeather(float humidity, float averageCloudSize)
        {
            SetWeather(humidity, averageCloudSize, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="humidity"></param>
        /// <param name="averageCloudSize"></param>
        /// <param name="numberOfForcedUpdates"></param>
        public void SetWeather(float humidity, float averageCloudSize, int numberOfForcedUpdates)
        {
            int numberOfClouds = (int)(humidity * _maxNumberOfClouds);
            Vector3 maxcloudsize = averageCloudSize * new Vector3(_nx / 14, _ny / 14, (float)((float)(_nz) / 2.75));

            // update old clouds with new parameters
            Vector3 currentdimensions = new Vector3();

            foreach (Ellipsoid iter in _ellipsoids)
            {
                currentdimensions = iter.Dimensions;

                if (currentdimensions.x / maxcloudsize.x < 0.5 || currentdimensions.x / maxcloudsize.x > 2)
                {
                    currentdimensions.x = maxcloudsize.x + Utility.RangeRandom(-0.2, 0.2) * maxcloudsize.x;
                }
                if (currentdimensions.y / maxcloudsize.y < 0.5 || currentdimensions.y / maxcloudsize.y > 2)
                {
                    currentdimensions.y = maxcloudsize.y + Utility.RangeRandom(-0.2, 0.2) * maxcloudsize.y;
                }
                if (currentdimensions.z / maxcloudsize.z < 0.5 || currentdimensions.x / maxcloudsize.z > 2)
                {
                    currentdimensions.z = maxcloudsize.z + Utility.RangeRandom(-0.2, 0.2) * maxcloudsize.z;
                }
            }

            // Remove some clouds if needed
            while ((int)numberOfClouds < _ellipsoids.Count)
            {
                _ellipsoids.Remove(_ellipsoids[_ellipsoids.Count - 1]);
            }

            // Add new clouds if needed
            Vector3 newclouddimensions = Vector3.Zero;

            while ((int)numberOfClouds > _ellipsoids.Count)
            {
                newclouddimensions = maxcloudsize * 
                    new Vector3(
                        Utility.RangeRandom(0.5, 2), 
                        Utility.RangeRandom(0.5, 2), 
                        Utility.RangeRandom(0.8, 1.2));
                AddEllipsoid(
                    new Ellipsoid(
                        (int)newclouddimensions.x, 
                        (int)newclouddimensions.y, 
                        (int)newclouddimensions.z,
                        _nx, _ny, _nz,
                    (int)Utility.RangeRandom(0, _nx),
                    (int)Utility.RangeRandom(0, _ny),
                    (int)Utility.RangeRandom(newclouddimensions.z + 2, _nz - newclouddimensions.z - 2),
                    Utility.RangeRandom(1, 5.0f)), false);
            }

            UpdateProbabilities(_cellsCurrent,_nx,_ny,_nz);

            for (int k = 0; k < numberOfForcedUpdates; k++)
            {
                ForceToUpdateData();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void AddEllipsoid(Ellipsoid e)
        {
            AddEllipsoid(e, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="updateProbalities"></param>
        public void AddEllipsoid(Ellipsoid e, bool updateProbalities)
        {
            _ellipsoids.Add(e);
            if (updateProbalities)
            {
                e.UpdatePropabilities(_cellsCurrent, _nx, _ny, _nz);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        private void ClearProbabilities(Cell[,,] c, int nx, int ny, int nz)
        {
            int u, v, w;

            for (u = 0; u < nx; u++)
            {
                for (v = 0; v < ny; v++)
                {
                    for (w = 0; w < nz; w++)
                    {
                        c[u,v,w].Pact = 0;
                        c[u,v,w].Pext = 1;
                        c[u,v,w].Phum = 0;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        private void UpdateProbabilities(Cell[,,] c, int nx, int ny, int nz)
        {
            UpdateProbabilities(c, nx, ny, nz, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="clearProbabilites"></param>
        private void UpdateProbabilities(Cell[,,] c, int nx, int ny, int nz, bool clearProbabilites)
        {
            if (clearProbabilites)
            {
                ClearProbabilities(c, nx, ny, nz);
            }

            foreach (Ellipsoid iter in _ellipsoids)
            {
                iter.UpdatePropabilities(c, nx, ny, nz);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="d"></param>
        /// <param name="att"></param>
        /// <returns></returns>
        private float GetLightAbsorcionAt(Cell[,,] c, int nx, int ny, int nz, int x, int y, int z, Vector3 d, float att)
        {
            float step = 1, factor = 1;
            Vector3 pos = new Vector3(x, y, z);
            bool outOfBounds = false;
            int u = 0, v = 0, uu = 0, vv = 0;
            int current_iteration = 0, max_iterations = 8;

            while (!outOfBounds)
            {
                if ((int)pos.z >= nz || (int)pos.z < 0 || factor <= 0 || current_iteration >= max_iterations)
                {
                    outOfBounds = true;
                }
                else
                {
                    u = (int)pos.x; v = (int)pos.y;

                    uu = (u < 0) ? (u + nx) : u; if (u >= nx) { uu -= nx; }
                    vv = (v < 0) ? (v + ny) : v; if (v >= ny) { vv -= ny; }

                    factor -= c[uu,vv,(int)pos.z].Dens * att * (1 - (float)(current_iteration) / max_iterations);
                    pos += step * (-d);

                    current_iteration++;
                }
            }

            return Utility.Clamp<float>(factor, 1, 0);
        }
        void LogCells(Cell cell)
        {
            LogManager.Instance.Write(
                "Act " + cell.Act +
                " Cld " + cell.Cld +
                " Dens " + cell.Dens +
                " Hum " + cell.Hum +
                " Light " + cell.Light +
                " Pact " + cell.Pact +
                " Pext " + cell.Pext +
                " Phum " + cell.Phum, null);

        }
        bool first = true;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        private void PerformCalculations(int nx, int ny, int nz)
        {
            // First step

            int u, v, w;

            for (u = 0; u < nx; u++)
            {
                for (v = 0; v < ny; v++)
                {
                    for (w = 0; w < nz; w++)
                    {
                        // ti+1                       ti
                        _cellsCurrent[u,v,w].Hum = _cellsTmp[u,v,w].Hum && !_cellsTmp[u,v,w].Act;
                        _cellsCurrent[u,v,w].Cld = _cellsTmp[u,v,w].Cld || _cellsTmp[u,v,w].Act;
                        _cellsCurrent[u,v,w].Act = !_cellsTmp[u,v,w].Act && _cellsTmp[u,v,w].Hum && Fact(_cellsTmp, nx, ny, nz, u, v, w);

                        
                    }
                }
            }

            // Second step

            Copy3DCellArraysData(_cellsCurrent, _cellsTmp, nx, ny, nz);

            for (u = 0; u < nx; u++)
            {
                for (v = 0; v < ny; v++)
                {
                    for (w = 0; w < nz; w++)
                    {

                        // ti+1                       ti
                        _cellsCurrent[u,v,w].Hum = _cellsTmp[u,v,w].Hum || (_ffRandom.Get() < _cellsTmp[u,v,w].Phum);
                        _cellsCurrent[u,v,w].Cld = _cellsTmp[u,v,w].Cld && (_ffRandom.Get() > _cellsTmp[u,v,w].Pext);
                        _cellsCurrent[u,v,w].Act = _cellsTmp[u,v,w].Act || (_ffRandom.Get() < _cellsTmp[u,v,w].Pact);
                    }
                }
            }

            // Final steps

            // Continous density
            for (u = 0; u < nx; u++)
            {
                for (v = 0; v < ny; v++)
                {
                    for (w = 0; w < nz; w++)
                    {
                        _cellsCurrent[u,v,w].Dens = GetDensityAt(_cellsCurrent, nx, ny, nz, u, v, w, 1);
                    }
                }
            }

            // Light scattering
            Vector3 SunDir = new Vector3(_vClouds.SunDirection.x, _vClouds.SunDirection.z, _vClouds.SunDirection.y);

            for (u = 0; u < nx; u++)
            {
                for (v = 0; v < ny; v++)
                {
                    for (w = 0; w < nz; w++)
                    {
                        _cellsCurrent[u,v,w].Light = GetLightAbsorcionAt(_cellsCurrent, _nx, _ny, _nz, u, v, w, SunDir, 0.15f);
                    }
                }
            }

            Copy3DCellArraysData(_cellsCurrent, _cellsTmp, nx, ny, nz);
            first = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private bool Fact(Cell[,,] c, int nx, int ny, int nz, int x, int y, int z)
        {
            bool i1m, j1m, k1m,
             i1r, j1r, k1r,
             i2r, i2m, j2r, j2m, k2r;

            i1m = ((x + 1) >= nx) ? c[0,y,z].Act : c[x + 1,y,z].Act;
            j1m = ((y + 1) >= ny) ? c[x,0,z].Act : c[x,y + 1,z].Act;
            k1m = ((z + 1) >= nz) ? false : c[x,y,z + 1].Act;

            i1r = ((x - 1) < 0) ? c[nx - 1,y,z].Act : c[x - 1,y,z].Act;
            j1r = ((y - 1) < 0) ? c[x,ny - 1,z].Act : c[x,y - 1,z].Act;
            k1r = ((z - 1) < 0) ? false : c[x,y,z - 1].Act;

            i2r = ((x - 2) < 0) ? c[nx - 2,y,z].Act : c[x - 2,y,z].Act;
            j2r = ((y - 2) < 0) ? c[x,ny - 2,z].Act : c[x,y - 2,z].Act;
            k2r = ((z - 2) < 0) ? false : c[x,y,z - 2].Act;

            i2m = ((x + 2) >= nx) ? c[1,y,z].Act : c[x + 2,y,z].Act;
            j2m = ((y + 2) >= ny) ? c[x,1,z].Act : c[x,y + 2,z].Act;

            return i1m || j1m || k1m || i1r || j1r || k1r || i2r || i2m || j2r || j2m || k2r;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private float GetDensityAt(Cell[,,] c, int nx, int ny, int nz, int x, int y, int z, int r)
        {
            int zr = ((z - r) < 0) ? 0 : z - r,
             zm = ((z + r) >= nz) ? nz : z + r,
             u, uu, v, vv, w,
             clouds = 0, div = 0;

            for (u = x - r; u <= x + r; u++)
            {
                for (v = y - r; v <= y + r; v++)
                {
                    for (w = zr; w < zm; w++)
                    {
                        // x/y Seamless!
                        uu = (u < 0) ? (u + nx) : u; if (u >= nx) { uu -= nx; }
                        vv = (v < 0) ? (v + ny) : v; if (v >= ny) { vv -= ny; }

                        clouds += c[uu,vv,w].Cld ? 1 : 0;
                        div++;
                    }
                }
            }

            return ((float)clouds) / div;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private float GetDensityAt(Cell[,,] c, int x, int y, int z)
        {
            return c[x,y,z].Cld ? 1.0f : 0.0f;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texId"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        private void CreateVolTexture(VolTextureId texId, int nx, int ny, int nz)
        {
            _volTextures[(int)texId] =
                (Texture)TextureManager.Instance.CreateManual(
                "_SkyX_VolCloudsData" + (int)texId,
                SkyX.SkyXResourceGroup,
                 TextureType.ThreeD,
                 nx, ny,nz , 0, Axiom.Media.PixelFormat.BYTE_RGB,TextureUsage.Default,null);

            _volTextures[(int)texId].Load();

            Material mat = (Material)MaterialManager.Instance.GetByName("SkyX_VolClouds");
            mat.GetTechnique(0).GetPass(0).GetTextureUnitState((int)texId).SetTextureName("_SkyX_VolCloudsData" + (int)texId, TextureType.ThreeD);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="texId"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        private void UpdateVolTextureData(Cell[,,] c, VolTextureId texId, int nx, int ny, int nz)
        {
            HardwarePixelBuffer buffer = _volTextures[(int)texId].GetBuffer(0, 0);

            buffer.Lock(BufferLocking.Discard);
            PixelBox pb = buffer.CurrentLock;
            int x = 0; int y = 0; int z = 0;
            unsafe
            {

                byte* pbptr = (byte*)pb.Data;


                for (z = pb.Front; z < pb.Back; z++)
                {
                    for (y = pb.Top; y < pb.Bottom; y++)
                    {
                        for (x = pb.Left; x < pb.Right; x++)
                        {
                            PixelConverter.PackColor(0, 0, (uint)c[x,y,z].Dens, (uint)c[x,y,z].Light, PixelFormat.BYTE_RGB, &pbptr[x]);
                        }
                        pbptr += pb.RowPitch;
                    }
                    pbptr += pb.SliceSkip;
                }
            }
            buffer.Unlock();

        }
    }
}
