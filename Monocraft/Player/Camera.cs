#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: Camera.cs
// Date - created: 2016.06.18 - 19:36
// Date - current: 2016.06.24 - 13:09

#endregion

#region Usings

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Monocraft.Player
{
    public class Camera
    {
        //Attributes
        private Vector3 cameraPosition;
        public float cameraSpeed;

        //Collision Boxes for this instance of camera
        internal List<BoundingBox> collisionBoxes = new List<BoundingBox>();
        public Vector3 mouseRotationBuffer;
        private float movespeedmultiplikator;
        public bool NegateOrbit;
        public bool Orbit;
        private Matrix view;

        //Constructor
        public Camera(GraphicsDevice device, Vector3 position, Vector3 rotation, float speed)
        {
            cameraSpeed = speed;

            //Setup projektion Matrix
            Projektion = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), device.Viewport.AspectRatio,
                0.1f, 1000.0f);

            mouseRotationBuffer = new Vector3(0, 0, 0);

            //Set camara Position and rotation
            MoveTo(position, rotation);
            mouseRotationBuffer = new Vector3(-Rotation.Y, -Rotation.X, Rotation.Z);
            UpdateLookAt();
        }

        public Vector3 CameraRotation { get; private set; }

        //Properties
        public Vector3 Position
        {
            get { return cameraPosition; }
            set
            {
                cameraPosition = value;
                UpdateLookAt();
            }
        }

        public Vector3 Target { get; set; }

        public Vector3 Rotation
        {
            get { return CameraRotation; }
            set
            {
                CameraRotation = value;
                UpdateLookAt();
            }
        }

        public Matrix Projektion { get; protected set; }

        public Matrix View
        {
            get
            {
                if (Orbit)
                {
                    return view;
                }

                return Matrix.CreateLookAt(cameraPosition, Target, Vector3.Up);
            }

            set { view = value; }
        }

        //Set camaras position rotation
        private void MoveTo(Vector3 pos, Vector3 rot)
        {
            Position = pos;
            Rotation = new Vector3(rot.X, rot.Y, rot.Z);
        }

        //update the look at vector
        private void UpdateLookAt()
        {
            // Build a rotation Matrix
            var rotationMatrix = Matrix.CreateRotationX(CameraRotation.X)*Matrix.CreateRotationY(CameraRotation.Y);

            //Build look at offset vector
            var lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);

            //Update cameras look at vector
            Target = cameraPosition + lookAtOffset;
        }

        //Methode that simulates movement
        private Vector3 PreviewMove(Vector3 amount)
        {
            //Create a rotate Matrix
            var rotate = Matrix.CreateRotationY(CameraRotation.Y);
            //Create a movement vector
            var movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotate);

            var toReturn = cameraPosition;

            if (!collides(new Vector3(cameraPosition.X + movement.X, cameraPosition.Y, cameraPosition.Z)))
                toReturn.X += movement.X;
            if (!collides(new Vector3(cameraPosition.X, cameraPosition.Y + movement.Y, cameraPosition.Z)))
                toReturn.Y += movement.Y;
            if (!collides(new Vector3(cameraPosition.X, cameraPosition.Y, cameraPosition.Z + movement.Z)))
                toReturn.Z += movement.Z;

            //Return the value of camera position + movement vector
            return toReturn;
        }

        //Method that actually moves the camera
        public void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }

        public void Update(GameTime gameTime)
        {
            //Sprint-Key
            movespeedmultiplikator = Game1.NewKeyboardState.IsKeyDown(Keys.LeftShift) ? 2.5F : 1.6F;

            var dt = (float) (gameTime.ElapsedGameTime.TotalSeconds);

            var moveVector = Vector3.Zero;

            if (Game1.NewKeyboardState.IsKeyDown(Keys.W))
                moveVector.Z = 1;

            if (Game1.NewKeyboardState.IsKeyDown(Keys.S))
                moveVector.Z = -1;

            if (Game1.NewKeyboardState.IsKeyDown(Keys.A))
                moveVector.X = 1;

            if (Game1.NewKeyboardState.IsKeyDown(Keys.D))
                moveVector.X = -1;

            if (Game1.NewKeyboardState.IsKeyDown(Keys.Space))
                moveVector.Y = 1;

            if (Game1.NewKeyboardState.IsKeyDown(Keys.LeftControl))
                moveVector.Y = -1;

            //if we are moving
            if (moveVector != Vector3.Zero)
            {
                //normalize that vector, so we dont move faster diagonolly
                moveVector.Normalize();
                //Now we add in smooth and speed vector
                moveVector *= dt*cameraSpeed*movespeedmultiplikator;

                //Move camera
                Move(moveVector);
            }

            //Handle mouse movement

            float deltaX, deltaY;

            if (Game1.NewMouseState != Game1.OldMouseState)
            {
                //Cache mouse location
                deltaX = Game1.NewMouseState.X - (Game1.graphics.GraphicsDevice.Viewport.Width/2F);
                deltaY = Game1.NewMouseState.Y - (Game1.graphics.GraphicsDevice.Viewport.Height/2F);

                mouseRotationBuffer.X -= deltaX*dt*0.14f;
                mouseRotationBuffer.Y -= deltaY*dt*0.14f;

                //Don't do a barrel roll
                if (mouseRotationBuffer.Y < MathHelper.ToRadians(-89.0f))
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y -
                                            (mouseRotationBuffer.Y - MathHelper.ToRadians(-89.0f));

                if (mouseRotationBuffer.Y > MathHelper.ToRadians(89.0f))
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y -
                                            (mouseRotationBuffer.Y - MathHelper.ToRadians(89.0f));

                //Set the rotation

                Rotation =
                    new Vector3(
                        -MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-89.0f),
                            MathHelper.ToRadians(89.0f)),
                        MathHelper.WrapAngle(mouseRotationBuffer.X), 0);
            }
        }

        private bool collides(Vector3 v)
        {
            //Kolision
            foreach (var box in collisionBoxes)
            {
                if (box.Contains(v) != ContainmentType.Disjoint)
                    return true;
            }

            //Return
            return false;
        }

        public void DegubDrawer(SpriteBatch sp, SpriteFont font)
        {
            sp.Begin();

            sp.DrawString(font, "Position:  " + Position + "\n\nRotation:  " + Rotation, new Vector2(100, 100),
                Color.White);

            sp.End();
        }
    }
}