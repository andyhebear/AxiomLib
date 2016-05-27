#region MIT License
/*
-----------------------------------------------------------------------------
This source file is part of the Jitter Sample
Jitter Physics Engine Copyright (c) 2010 Thorben Linneweber (admin@jitter-physics.com)

This a port for Axiom of samples using Jitter Physics Engine,
developed by Thorben Linneweber and ported by Francesco Guastella (aka romeoxbm).
 
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
-----------------------------------------------------------------------------
*/
#endregion

#region Using Statements

using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

#endregion

namespace AxiomContrib.Samples.Jitter.PhysicsObjects.Vehicle
{
    /// <summary>
    /// Enumeration for the four car wheels.
    /// </summary>
    public enum WheelPosition
    {
        /// <summary>Front left wheel.</summary>
        FrontLeft,
        /// <summary>Front right wheel.</summary>
        FrontRight,
        /// <summary>Back left wheel.</summary>
        BackLeft,
        /// <summary>Back right wheel.</summary>
        BackRight
    }

    /// <summary>
    /// Creates the Jitter default car with 4 wheels. To create a custom car
    /// use the Wheel class and add it to a body.
    /// </summary>
    public class DefaultCar : RigidBody
    {
        // the default car has 4 wheels
        private Wheel[] wheels = new Wheel[4];
        private World world;

        private float destSteering = 0.0f;   
        private float destAccelerate = 0.0f;
        private float steering = 0.0f;
        private float accelerate = 0.0f;

        /// <summary>
        /// The maximum steering angle in degrees
        /// for both front wheels
        /// </summary>
        public float SteerAngle { get; set; }

        /// <summary>
        /// The maximum torque which is applied to the
        /// car when accelerating.
        /// </summary>
        public float DriveTorque { get; set; }

        /// <summary>
        /// Lower/Higher the acceleration of the car.
        /// </summary>
        public float AccelerationRate { get; set; }

        /// <summary>
        /// Lower/Higher the steering rate of the car.
        /// </summary>
        public float SteerRate { get; set; }

        // don't damp perfect, allow some bounciness.
        private const float dampingFrac = 0.5f;
        private const float springFrac = 0.1f;

        /// <summary>
        /// Initializes a new instance of the DefaultCar class.
        /// </summary>
        /// <param name="world">The world the car should be in.</param>
        /// <param name="shape">The shape of the car. Recommend is a box shape.</param>
        public DefaultCar(World world,Shape shape) : base(shape)
        {
            this.world = world;
            world.PostStep += new WorldStep(world_PostStep);

            // set some default values
            this.AccelerationRate = 5.0f;
            this.SteerAngle = 20.0f;
            this.DriveTorque = 50.0f;
            this.SteerRate = 5.0f;

            // create default wheels
            wheels[(int)WheelPosition.FrontLeft] = new Wheel(world, this, JVector.Left + 1.8f * JVector.Forward + 0.4f * JVector.Down,0.6f);
            wheels[(int)WheelPosition.FrontRight] = new Wheel(world, this, JVector.Right + 1.8f * JVector.Forward + 0.4f * JVector.Down, 0.6f);
            wheels[(int)WheelPosition.BackLeft] = new Wheel(world, this, JVector.Left + 1.8f * JVector.Backward + 0.4f * JVector.Down, 0.6f);
            wheels[(int)WheelPosition.BackRight] = new Wheel(world, this, JVector.Right + 1.8f * JVector.Backward + 0.4f * JVector.Down, 0.6f);

            AdjustWheelValues();
        }

        /// <summary>
        /// This recalculates the inertia, damping and spring of all wheels based
        /// on the car mass, the wheel radius and the gravity. Should be called
        /// after manipulating wheel data.
        /// </summary>
        public void AdjustWheelValues()
        {
            float mass = this.Mass / 4;

            foreach (Wheel w in wheels)
            {
                w.Inertia = 0.5f * (w.Radius * w.Radius) * mass;
                w.Spring = mass * world.Gravity.Length() / (w.WheelTravel * springFrac);
                w.Damping = 2.0f * (float)System.Math.Sqrt(w.Spring * this.Mass) * 0.25f * dampingFrac;
            }
        }

        /// <summary>
        /// Access the wheels. Wheel index follows <see cref="WheelPosition"/>
        /// </summary>
        public Wheel[] Wheels { get { return wheels; } }

        /// <summary>
        /// Set input values for the car.
        /// </summary>
        /// <param name="accelerate">A value between -1 and 1 (other values get clamped). Adjust
        /// the maximum speed of the car by setting <see cref="DriveTorque"/>. The maximum acceleration is adjusted
        /// by setting <see cref="AccelerationRate"/>.</param>
        /// <param name="steer">A value between -1 and 1 (other values get clamped). Adjust
        /// the maximum steer angle by setting <see cref="SteerAngle"/>. The speed of steering
        /// change is adjusted by <see cref="SteerRate"/>.</param>
        public void SetInput(float accelerate, float steer)
        {
            destAccelerate = accelerate;
            destSteering = steer;
        }

        private void world_PostStep(float timeStep)
        {
            float deltaAccelerate = timeStep * AccelerationRate;
            float deltaSteering = timeStep * SteerRate;

            float dAccelerate = destAccelerate - accelerate;
            dAccelerate = JMath.Clamp(dAccelerate, -deltaAccelerate, deltaAccelerate);

            accelerate += dAccelerate;

            float dSteering = destSteering - steering;
            dSteering = JMath.Clamp(dSteering, -deltaSteering, deltaSteering);

            steering += dSteering;

            float maxTorque = DriveTorque * 0.5f;

            foreach (Wheel w in wheels)
            {
                w.AddTorque(maxTorque * accelerate);
            }

            float alpha = SteerAngle * steering;

            wheels[(int)WheelPosition.FrontLeft].SteerAngle = alpha;
            wheels[(int)WheelPosition.FrontRight].SteerAngle = alpha;
        }
    }
}
