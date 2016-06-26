#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: Frame.cs
// Date - created: 2016.06.18 - 19:50
// Date - current: 2016.06.26 - 11:08

#endregion

#region Usings

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocraft.Basics;

#endregion

namespace Monocraft.World
{
    public class Frame
    {
        public readonly float Depth;
        public readonly float Height;
        public readonly bool Translucent;
        //private readonly BoundingBox Dimensions;
        public readonly float Width;
        protected Model MyModel;

        protected Texture2D Texture; // Not readonly because I want to be able to animate the texture

        public Frame(Texture2D texture, Model me, bool translucent)
        {
            Texture = texture;
            MyModel = me;
            Translucent = translucent;

            if (me == null) return;

            var transforms = new Matrix[MyModel.Bones.Count];
            MyModel.CopyAbsoluteBoneTransformsTo(transforms);
            var meshTransform = transforms[MyModel.Meshes[0].ParentBone.Index];
            var dimensions = MyModel.Meshes[0].BuildBoundingBox(meshTransform);
            Width = dimensions.Max.X - dimensions.Min.X;
            Height = dimensions.Max.Y - dimensions.Min.Y;
            Depth = dimensions.Max.Z - dimensions.Min.Z;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="Frame" /> class.
        /// </summary>
        ~Frame()
        {
            Dispose();
        }

        /// <summary>
        ///     Draws the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="projection">The projection.</param>
        /// <param name="view">The view.</param>
        /// <param name="position">The position.</param>
        public virtual void Draw(GraphicsDevice device, Matrix projection, Matrix view, Vector3 position)
        {
            //if (!_transparent)
            //{
            foreach (var mesh in MyModel.Meshes)
            {
                foreach (BasicEffect basicEffect in mesh.Effects)
                {
                    basicEffect.Projection = projection;
                    basicEffect.View = view;
                    basicEffect.World = Matrix.CreateTranslation(position);
                    basicEffect.FogEnabled = true;
                    basicEffect.FogEnd = 1000;
                    basicEffect.FogStart = 100;
                    basicEffect.FogColor = new Vector3(255, 255, 255);
                    basicEffect.EnableDefaultLighting();
                    basicEffect.LightingEnabled = true; // Turn on the lighting subsystem.

                    basicEffect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1, 1); // a reddish light
                    basicEffect.DirectionalLight0.Direction = new Vector3(0, 0, 0); // coming along the x-axis
                    basicEffect.DirectionalLight0.SpecularColor = new Vector3(.1f, 0, 0); // with green highlights

                    basicEffect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                    // Add some overall ambient light.

                    basicEffect.TextureEnabled = true;
                    basicEffect.Texture = Texture;
                }

                mesh.Draw();
            }
        }

        public void Dispose()
        {
            Texture?.Dispose();
            Texture = null;
            MyModel = null;
        }
    }
}