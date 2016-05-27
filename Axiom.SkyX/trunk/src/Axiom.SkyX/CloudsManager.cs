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
using Axiom.Graphics;
namespace Axiom.SkyX
{
    public class CloudsManager : IDisposable
    {
        /// <summary>
        /// Cloud layers
        /// </summary>
        private List<CloudLayer> _cloudLayers = new List<CloudLayer>();
        /// <summary>
        /// SkyX reference.
        /// </summary>
        private SkyX _skyX;
        public SkyX SkyX
        {
            get; 
            private set; 
        }

        #region Construction and Destruction
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skyX"></param>
        public CloudsManager( SkyX skyX )
        {
            this.SkyX = skyX;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            RemoveAll();
        }
        #endregion Construction and Destruction
        /// <summary>
        /// Add a cloud layer
        /// </summary>
        /// <param name="options">Cloud layer options</param>
        public CloudLayer Add(CloudLayer.Options options)
        {
            CloudLayer newCloudLayer = new CloudLayer(this.SkyX, options);

            // TODO
            Material mat = (Material)MaterialManager.Instance.GetByName(this.SkyX.GpuManager.SkydomeMaterialName);
            newCloudLayer.RegisterCloudLayer(mat.GetTechnique(0).CreatePass());
            mat.Reload();
            _cloudLayers.Add(newCloudLayer);

            bool changeOrder = false;
            // Short layers by height
            for (int k = 0; k < _cloudLayers.Count; k++)
            {
                if (k + 1 < _cloudLayers.Count)
                {
                    if (_cloudLayers[k].LayerOptions.Height < _cloudLayers[k + 1].LayerOptions.Height)
                    {
                        // Swap
                        CloudLayer cl = _cloudLayers[k];
                        _cloudLayers[k] = _cloudLayers[k + 1];
                        _cloudLayers[k + 1] = cl;

                        changeOrder = true;
                        k = 0;
                    }
                }
            }

            if (changeOrder)
            {
                UnregisterAll();
                RegisterAll();
            }

            return newCloudLayer;
        }
        /// <summary>
        /// Remove the specified cloud layer
        /// </summary>
        /// <param name="cl"></param>
        public void Remove(CloudLayer cl)
        {
            if (_cloudLayers.Contains(cl))
                _cloudLayers.Remove(cl);
        }
        /// <summary>
        /// Remove all cloud layers
        /// </summary>
        public void RemoveAll()
        {
            _cloudLayers.Clear();
        }
        /// <summary>
        /// Register all
        /// </summary>
        public void RegisterAll()
        {
            for (int k = 0; k < _cloudLayers.Count; k++)
            {
                Material mat = (Material)MaterialManager.Instance.GetByName(this.SkyX.GpuManager.SkydomeMaterialName);
                _cloudLayers[k].RegisterCloudLayer(mat.GetTechnique(0).CreatePass());
                mat.Reload();
            }
        }

        /// <summary>
        /// Unregister cloud layer
        /// </summary>
        /// <param name="cl">Cloud layer to unregister</param>
        public void Unregister(CloudLayer cl)
        {
            for (int k = 0; k < _cloudLayers.Count; k++)
            {
                if (_cloudLayers[k] == cl)
                {
                    _cloudLayers[k].Unregister();
                    break;
                }
            }
        }
        /// <summary>
        /// Unregister all cloud layers
        /// </summary>
        public void UnregisterAll()
        {
            foreach (CloudLayer cl in _cloudLayers)
                cl.Unregister();
        }
        /// <summary>
        /// Update internal cloud layer parameters
        /// </summary>
        internal void UpdateInternal()
        {
            foreach (CloudLayer cl in _cloudLayers)
                cl.UpdateInternalPassParameters();
        }

    }
}
