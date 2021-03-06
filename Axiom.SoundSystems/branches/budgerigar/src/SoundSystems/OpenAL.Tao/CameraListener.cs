﻿#region MIT License
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

using Axiom.Core;
using Axiom.Math;
using Tao.OpenAl;

namespace Axiom.SoundSystems.OpenAL.Tao
{
    public class CameraListener : Axiom.SoundSystems.CameraListener
    {
        public CameraListener(Camera camera)
            : base(camera)
        {
        }

        private float[] _vector3 = new float[3];

        public override Vector3 Velocity
        {
            set
            {
                base.Velocity = value;

                _vector3[0] = value.x;
                _vector3[1] = value.y;
                _vector3[2] = value.z;

                Al.alListenerfv(Al.AL_VELOCITY, _vector3);
            }
        }
    }
}
