#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: PerlinNoise.cs
// Date - created: 2016.06.19 - 13:20
// Date - current: 2016.06.25 - 18:38

#endregion

#region Usings

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Monocraft.World;

#endregion

namespace Monocraft.Basics
{
    static class PerlinNoise
    {
        public const int RESOLUTION = 10;
        private static RenderTarget2D cloudsRenderTarget;
        //private static RenderTarget2D temp1;
        private static Texture2D permTexture;
        private static Effect PerlinEffect;
        private static Effect Blur;
        private static Random rand;
        private static VertexPositionTexture[] fullScreenVertices;
        private static readonly short[] IndexData = { 0, 1, 2, 2, 3, 0 };

        public static void Initialise(int seed, GraphicsDevice device, ContentManager content)
        {
            rand=new Random(seed);

            PerlinEffect = content.Load<Effect>("Effects/PerlinNoise");
            PerlinEffect.CurrentTechnique = PerlinEffect.Techniques["PerlinNoise"];

            Blur = content.Load<Effect>("Effects/GaussianBlur");
            Blur.CurrentTechnique = Blur.Techniques["GaussianBlur"];

            fullScreenVertices=SetUpFullscreenVertices();

            permTexture = GeneratePermTexture(32, device);

            cloudsRenderTarget = new RenderTarget2D(device, WorldTile.WORLD_TILE_WIDTH* RESOLUTION,
                WorldTile.WORLD_TILE_WIDTH * RESOLUTION);
            //temp1 = new RenderTarget2D(device, WorldTile.WORLD_TILE_WIDTH*RESOLUTION,
            //    WorldTile.WORLD_TILE_WIDTH*RESOLUTION);
            //cloudStaticMap = content.Load<Texture2D>("Textures/Frames/grass");
        }

        public static Texture2D GeneratePermTexture(byte resolution, GraphicsDevice device)
        {
            var noise = new Color[resolution * resolution];
            for (var x = 0; x < resolution; x++)
            {
                for (var y = 0; y < resolution; y++)
                {
                    noise[x + y * resolution] = new Color(new Vector3(rand.Next(1000) / 1000f, 0, 0));
                }
            }

            var toRet = new Texture2D(device, resolution, resolution);
            toRet.SetData(noise);
            return toRet;
        }

        public static Texture2D GeneratePerlinNoiseGPU(Vector3 position, GraphicsDevice device, SpriteBatch sp)
        {
            device.SetRenderTarget(cloudsRenderTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            sp.Begin(SpriteSortMode.Deferred, null, null, null, null, PerlinEffect);

            PerlinEffect.Parameters["permTexture"].SetValue(permTexture);
            PerlinEffect.Parameters["Overcast"].SetValue(1.1f);
            PerlinEffect.Parameters["xCoord"].SetValue(new Vector2(position.X, position.Z));
            foreach (var pass in PerlinEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, fullScreenVertices, 0, 4, IndexData, 0, 2);
            }
            sp.End();

            device.SetRenderTarget(null);

            return cloudsRenderTarget.CloneRenderTarget(device);
        }

        private static VertexPositionTexture[] SetUpFullscreenVertices()
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[4]
            {
                new VertexPositionTexture(new Vector3(1, -1, 0), Vector2.One),
                new VertexPositionTexture(new Vector3(-1, -1, 0), Vector2.UnitY),
                new VertexPositionTexture(new Vector3(-1, 1, 0), Vector2.Zero),
                new VertexPositionTexture(new Vector3(1, 1, 0), Vector2.UnitX)
            };

            return vertices;
        }
    }
}