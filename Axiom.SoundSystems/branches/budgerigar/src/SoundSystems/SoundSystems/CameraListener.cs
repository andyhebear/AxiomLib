#region MIT License
/*
The MIT License

Copyright (c) 2010 Axiom Contrib Developers

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using System;
using Axiom.Core;
using Axiom.Math;

namespace Axiom.SoundSystems
{
    /// <summary>
    /// A listening camera
    /// </summary>
    public class CameraListener : ISoundListener
    {
        #region Constructor

        public CameraListener(Camera camera)
        {
            _cameraObject = camera;
        }

        #endregion

        #region ISoundListener

        protected Vector3 _velocity;
        /// <summary>
        /// Velocity vector of the listener
        /// </summary>
        public virtual Vector3 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        /// <summary>
        /// Orientation in world coordinates
        /// </summary>
        /// <remarks>
        /// Is equivalent to the derived orientation of the underlying camera
        /// </remarks>
        public virtual Quaternion Orientation
        {
            get
            {
                return _cameraObject.DerivedOrientation;
            }
            set
            {
                // TODO: there's no setter for Camera.DerivedOrientation, needs some math
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Forward vector.
        /// </summary>
        /// <remarks>
        /// Is equivalent to the derived direction vector of the underlying camera.        
        /// </remarks>
        public virtual Vector3 Forward
        {
            get
            {
                return _cameraObject.DerivedDirection;
            }
            set
            {
                // TODO: there's no setter for Camera.DerivedDirection, needs some math
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Position of the listener
        /// </summary>
        /// <remarks>
        /// Is equivalent to the derived position vector of the underlying camera.
        /// </remarks>
        public virtual Vector3 Position
        {
            get
            {
                return _cameraObject.DerivedPosition;
            }
            set
            {
                // TODO: there's no setter for Camera.DerivedPosition, needs some math
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Up vector
        /// </summary>
        /// <remarks>
        /// Is equivalent to the derived up vector of the underlying camera.
        /// </remarks>
        public virtual Vector3 Up
        {
            get
            {
                return _cameraObject.DerivedUp;
            }
            set
            {
                // TODO: there's no setter for Camera.DerivedUp, needs some math
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Properties

        protected Camera _cameraObject;
        /// <summary>
        /// The camera that represents the listener.
        /// </summary>
        public virtual Camera CameraObject
        {
            get { return _cameraObject; }
            set { _cameraObject = value; }
        }

        #endregion
    }
}
