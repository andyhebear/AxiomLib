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

using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;

namespace Caelum
{
    /// <summary>
    /// Describes a base class for all elements of caelum wich are rendered with a mesh.
    /// </summary>
    public class CaelumBaseMesh : CaelumBase
    {
        // Attributes -----------------------------------------------------------------

        protected Entity mMesh;
        protected Vector3 mOffset;

        // Accessors --------------------------------------------------------------------

        /// <summary>
        /// Gets the main node's material.</summary>
        /// <remarks>Returns null if there isn't material</remarks>
        public Material MainMaterial
        {
            get
            {
                if (mMesh == null || mMesh.Mesh != null || mMesh.Mesh.SubMeshCount == 0)
                    return null;

                string material = mMesh.Mesh.GetSubMesh(0).MaterialName;
                return MaterialManager.Instance.GetByName(material);
            }
        }

        // Methods --------------------------------------------------------------------

        ~CaelumBaseMesh()
        {
            Dispose();
        }

        public override void Dispose()
        {            
            if (mDisposed)
                return;

            if (mMesh != null)
                mMesh.Dispose();

            mMesh = null;

            base.Dispose();
        }

        /// <summary>
        /// Creates the element in the world. It automatically
        /// sets up the mesh and the node.</summary>
        protected virtual void Initialise(RenderQueueGroupID renderGroup, string meshName, Vector3 scale, Vector3 rotation, Vector3 translation)
        {
            // Creates the mesh in the world
            mMesh = Root.Instance.SceneManager.CreateEntity(meshName,meshName);
            mMesh.CastShadows = false;
            mMesh.RenderQueueGroup = renderGroup;

            // Attaches the mesh on a node
            mNode = new SceneNode(Root.Instance.SceneManager);
            mNode.AddChild(mMesh);

            // Sets up the node (Position, Scale and Rotation)
            mNode.Position = translation;
            mNode.Scale(scale);
            mNode.Orientation *= CaelumUtils.GenerateQuat(CaelumUtils.XAxis, Axiom.Math.Utility.RadiansToDegrees(rotation.x));
            mNode.Orientation *= CaelumUtils.GenerateQuat(CaelumUtils.YAxis, Axiom.Math.Utility.RadiansToDegrees(rotation.y));
            mNode.Orientation *= CaelumUtils.GenerateQuat(CaelumUtils.ZAxis, Axiom.Math.Utility.RadiansToDegrees(rotation.z));

            mOffset = translation;
        }
    }
}
