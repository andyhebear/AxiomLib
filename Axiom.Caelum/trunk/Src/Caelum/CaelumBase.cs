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
using Axiom.Core;

namespace Caelum
{
    /// <summary>
    /// Describes a base class for all elements of caelum.</summary>
    public class CaelumBase : IDisposable
    {
        // Attributes -----------------------------------------------------------------

        protected SceneNode mNode;
        protected bool mDisposed = false;

        // Methods --------------------------------------------------------------------

        public CaelumBase()
        {
            mDisposed = false;
        }

        ~CaelumBase()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes all resources used by this element.</summary>
        public virtual void Dispose() 
        {
            if (mDisposed)
                return;

            //if (mNode != null)
            //    mNode.Dispose();

            mNode = null;
            mDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Updates Caelum's calculations. Is necessary for a correct render.</summary>
        /// <param name="time">The relative day time.</param>
        /// <param name="cam">The main camera used for the render.</param>
        public virtual void Update(float time, Camera cam) { }
    }
}
