#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: Player.cs
// Date - created: 2016.06.19 - 10:02
// Date - current: 2016.06.25 - 18:38

#endregion

#region Usings

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Monocraft.Player
{
    class Player
    {
        private readonly Texture2D _modelTexture;
        private readonly Model _playerModel;
        public readonly Camera Cam;

        public Player(GraphicsDevice device, ContentManager content)
        {
            Cam = new Camera(device, new Vector3(0, 4.8f, 0), Vector3.Forward, 3f);
            _playerModel = content.Load<Model>("Models/Frames/Ball");
            _modelTexture = content.Load<Texture2D>("Textures/Player/steve");
        }

        public void TeleportTo(Vector3 position)
        {
            Cam.Position = position;
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            Cam.Update(gameTime);
            Mouse.SetPosition(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2);
        }

        public void Draw(SpriteBatch sp, GraphicsDevice device)
        {
            //device.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };
            foreach (var mesh in _playerModel.Meshes)
            {
                foreach (BasicEffect basicEffect in mesh.Effects)
                {
                    basicEffect.Projection = Cam.Projektion;
                    basicEffect.View = Cam.View;
                    basicEffect.World = Matrix.CreateRotationX(Cam.Rotation.X)*Matrix.CreateRotationY(Cam.Rotation.Y)*
                                        Matrix.CreateRotationZ(1 - Cam.Rotation.Z)*
                                        Matrix.CreateTranslation(Cam.Position);
                    basicEffect.FogEnabled = true;
                    basicEffect.FogEnd = 1000;
                    basicEffect.FogStart = 100;
                    basicEffect.FogColor = new Vector3(255, 255, 255);
                    basicEffect.EnableDefaultLighting();
                    basicEffect.LightingEnabled = true; // Turn on the lighting subsystem.

                    basicEffect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1, 1); // a reddish light
                    basicEffect.DirectionalLight0.Direction = new Vector3(0, 0, 0); // coming along the x-axis
                    basicEffect.DirectionalLight0.SpecularColor = new Vector3(.1f, 0, 0); // with green highlights

                    basicEffect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f); // Add some overall ambient light.

                    basicEffect.TextureEnabled = true;
                    basicEffect.Texture = _modelTexture;
                }

                mesh.Draw();
            }
        }
    }
}