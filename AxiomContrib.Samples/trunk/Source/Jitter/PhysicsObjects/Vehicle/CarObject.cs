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

using Axiom.Core;
using Jitter;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;

#endregion

namespace AxiomContrib.Samples.Jitter.PhysicsObjects.Vehicle
{
    public class CarObject
    {
        private JitterSample sample;
        private SceneNode chassisModelNode = null;
        private SceneNode[] tireModelNodes = new SceneNode[ 4 ];

        private DefaultCar car = null;

        public CarObject(JitterSample sample)
        {
            this.sample = sample;
            BuildCar();
        }

        private void BuildCar()
        {
            World world = sample.PhysicWorld;

            CompoundShape.TransformedShape lower = new CompoundShape.TransformedShape(
                new BoxShape(2.5f, 1f, 6.0f), JMatrix.Identity, JVector.Zero);

            CompoundShape.TransformedShape upper = new CompoundShape.TransformedShape(
                new BoxShape(2.0f, 0.5f, 3.0f), JMatrix.Identity, JVector.Up * 0.75f + JVector.Backward * 1.0f);

            CompoundShape.TransformedShape[] subShapes = { lower, upper };

            Shape chassis = new CompoundShape(subShapes);

            //chassis = new BoxShape(2.5f, 1f, 6.0f);

            car = new DefaultCar(world, chassis);

            // use the inertia of the lower box.
            car.UseUserMassProperties(lower.Shape.Inertia, lower.Shape.Mass, false);

            // adjust some driving values
            car.SteerAngle = 30; car.DriveTorque = 155;
            car.AccelerationRate = 10;
            car.SteerRate = 2f;
            car.AdjustWheelValues();

            car.Tag = true;
            car.AllowDeactivation = false;

            // place the car two units above the ground.
            car.Position = new JVector(0, 5, 0);

            world.AddBody(car);
        }

        public void Update()
        {
            //KeyboardState keyState = Keyboard.GetState();

            //float steer, accelerate;
            //if (keyState.IsKeyDown(Keys.Up)) accelerate = 1.0f;
            //else if (keyState.IsKeyDown(Keys.Down)) accelerate = -1.0f;
            //else accelerate = 0.0f;

            //if (keyState.IsKeyDown(Keys.Left)) steer = 1;
            //else if (keyState.IsKeyDown(Keys.Right)) steer = -1;
            //else steer = 0.0f;

            //car.SetInput(accelerate, steer);
        }

        #region Draw Wheels

        private void DrawWheels()
        {
            //for(int i = 0;i<car.Wheels.Length;i++)
            //{
            //    Wheel wheel = car.Wheels[i];

            //    Vector3 position = Conversion.ToAxiomVector(wheel.GetWorldPosition());

            //    foreach (ModelMesh mesh in tireModelNodes.Meshes)
            //    {
            //        foreach (BasicEffect effect in mesh.Effects)
            //        {
            //            Matrix addOrienation;

            //            if (i % 2 != 0) addOrienation = Matrix.CreateRotationX(System.Math.PI);
            //            else addOrienation = Matrix.Identity;

            //            effect.World =
            //                addOrienation *
            //                Matrix.CreateRotationZ(MathHelper.PiOver2) *
            //                Matrix.CreateRotationX(MathHelper.ToRadians(-wheel.WheelRotation)) *
            //                Matrix.CreateRotationY(MathHelper.ToRadians(wheel.SteerAngle)) *
            //                Conversion.ToMatrix4(car.Orientation) *
            //                Matrix.CreateTranslation(position);

            //            effect.EnableDefaultLighting();
            //            effect.View = demo.Camera.View;
            //            effect.Projection = demo.Camera.Projection;
            //        }
            //        mesh.Draw();
            //    }
            //}
        }
        #endregion

        private void DrawChassis()
        {
            //foreach (ModelMesh mesh in chassisModelNode.Meshes)
            //{
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        Matrix matrix = Conversion.ToMatrix4(car.Orientation);
            //        matrix.Translation = Conversion.ToAxiomVector(car.Position) -
            //            Vector3.Transform(new Vector3(0,0.5f,0),matrix);

            //        effect.EnableDefaultLighting();
            //        effect.World = matrix;
            //        //effect.View = demo.Camera.View;
            //        //effect.Projection = demo.Camera.Projection;
            //    }
            //    mesh.Draw();
            //}
        }

        protected void LoadContent()
        {
            //chassisModelNode = this.Game.Content.Load<Model>("car");
            //tireModelNodes = this.Game.Content.Load<Model>("wheel");
        }

        public void Draw()
        {
            DrawWheels();
            DrawChassis();
        }
    }
}
