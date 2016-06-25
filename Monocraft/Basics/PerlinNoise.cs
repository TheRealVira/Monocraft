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
        private static RenderTarget2D temp1;
        private static Texture2D permGradTexture;
        private static Texture2D permTexture2d;
        private static Effect PerlinEffect;
        private static Effect Blur;
        private static Random rand;
        // permutation table
        private static readonly int[] permutation = new int[256];

        // gradients for 3d noise
        private static readonly float[,] gradients =
        {
            {1, 1, 0},
            {-1, 1, 0},
            {1, -1, 0},
            {-1, -1, 0},
            {1, 0, 1},
            {-1, 0, 1},
            {1, 0, -1},
            {-1, 0, -1},
            {0, 1, 1},
            {0, -1, 1},
            {0, 1, -1},
            {0, -1, -1},
            {1, 1, 0},
            {0, -1, 1},
            {-1, 1, 0},
            {0, -1, -1}
        };

        public static void Initialise(int seed, GraphicsDevice device, ContentManager content)
        {
            rand=new Random(seed);

            PerlinEffect = content.Load<Effect>("Effects/PerlinNoise");
            PerlinEffect.CurrentTechnique = PerlinEffect.Techniques["PerlinNoise"];

            cloudsRenderTarget = new RenderTarget2D(device, WorldTile.WORLD_TILE_WIDTH*RESOLUTION,
                WorldTile.WORLD_TILE_WIDTH*RESOLUTION);
            temp1 = new RenderTarget2D(device, WorldTile.WORLD_TILE_WIDTH*RESOLUTION,
                WorldTile.WORLD_TILE_WIDTH*RESOLUTION);
            //cloudStaticMap = content.Load<Texture2D>("Textures/Frames/grass");

            Blur = content.Load<Effect>("Effects/GaussianBlur");
            Blur.CurrentTechnique = Blur.Techniques["GaussianBlur"];

            // Reset
            for (int i = 0; i < permutation.Length; i++)
            {
                permutation[i] = -1;
            }

            // Generate random numbers
            for (int i = 0; i < permutation.Length; i++)
            {
                while (true)
                {
                    int iP = rand.Next() % permutation.Length;
                    if (permutation[iP] == -1)
                    {
                        permutation[iP] = i;
                        break;
                    }
                }
            }

            permGradTexture = GeneratePermGradTexture(device);
            permTexture2d = GeneratePermTexture2d(device);
        }

        public static Texture2D GeneratePermGradTexture(GraphicsDevice device)
        {
            Texture2D permGradTexture = new Texture2D(device, 256, 1, true,
                                                                             SurfaceFormat.NormalizedByte4);
            NormalizedByte4[] data = new NormalizedByte4[256 * 1];
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 1; y++)
                {
                    data[x + (y * 256)] = new NormalizedByte4(gradients[permutation[x] % 16, 0],
                                                                                                 gradients[permutation[x] % 16, 1],
                                                                                                 gradients[permutation[x] % 16, 2], 1);
                }
            }
            permGradTexture.SetData<NormalizedByte4>(data);
            return permGradTexture;
        }

        public static Texture2D GeneratePermTexture2d(GraphicsDevice device)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var noise = new Color[256 * 256];
            for (var x = 0; x < 256; x++)
            {
                for (var y = 0; y < 256; y++)
                {
                    noise[x + y * 256] = new Color(new Vector3(rand.Next(1000) / 1000f, 0, 0));
                }
            }

            var toRet = new Texture2D(device, 256, 256);
            toRet.SetData(noise);
            return toRet;
            //Texture2D permTexture2d = new Texture2D(device, 256, 256, true, SurfaceFormat.Color);
            //Color[] data = new Color[256 * 256];
            //for (int x = 0; x < 256; x++)
            //{
            //    for (int y = 0; y < 256; y++)
            //    {
            //        int A = perm2d(x) + y;
            //        int AA = perm2d(A);
            //        int AB = perm2d(A + 1);
            //        int B = perm2d(x + 1) + y;
            //        int BA = perm2d(B);
            //        int BB = perm2d(B + 1);
            //        data[x + (y * 256)] = new Color((byte)(AA), (byte)(AB),
            //                                                                (byte)(BA), (byte)(BB));
            //    }
            //}
            //permTexture2d.SetData<Color>(data);
            //return permTexture2d;
        }

        public static Texture2D GeneratePerlinNoiseGPU(Vector3 position, Matrix view, Matrix projection, GraphicsDevice device, SpriteBatch sp)
        {
            device.SetRenderTarget(cloudsRenderTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            sp.Begin(SpriteSortMode.Deferred, null, null, null, null, PerlinEffect);
            
            PerlinEffect.Parameters["permTexture2d"].SetValue(permTexture2d);
            PerlinEffect.Parameters["permGradTexture"].SetValue(permGradTexture);
            PerlinEffect.Parameters["World"].SetValue(Matrix.CreateWorld(position,Vector3.Forward, Vector3.Up));
            PerlinEffect.Parameters["View"].SetValue(view);
            PerlinEffect.Parameters["Projection"].SetValue(projection);
            foreach (var pass in Blur.CurrentTechnique.Passes)
            {
                pass.Apply();

                //device.DrawUserPrimitives(PrimitiveType.TriangleStrip, fullScreenVertices, 0, 2);
                sp.Draw(Game1.SolidWhite, null, new Rectangle(0, 0, device.Adapter.CurrentDisplayMode.Width, device.Adapter.CurrentDisplayMode.Height), null,
                    null, 0f, null, Color.White);
            }
            sp.End();

            device.SetRenderTarget(null);
            return cloudsRenderTarget;

            //sp.Begin();
            //sp.Draw(cloudStaticMap, new Vector2(), Color.White);
            //sp.End();

            //device.SetRenderTarget(null);
            //Texture2D notToRet = cloudsRenderTarget;
            //device.SetRenderTarget(temp1);
            //device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            //// Bluring the image a bit
            //sp.Begin(SpriteSortMode.Deferred, null, null, null, null, Blur);
            ////sp.Begin();

            //Blur.Parameters["xTexture"].SetValue(notToRet);
            //Blur.Parameters["xPixelHeight"].SetValue(1f / notToRet.Height);
            //Blur.Parameters["xPixelWidth"].SetValue(1f / notToRet.Width);
            //foreach (EffectPass pass in Blur.CurrentTechnique.Passes)
            //{
            //    pass.Apply();

            //    sp.Draw(notToRet, new Vector2(), Color.White);
            //}
            //sp.End();

            //device.SetRenderTarget(null);
        }

        private static VertexPositionTexture[] SetUpFullscreenVertices()
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[4];

            vertices[0] = new VertexPositionTexture(new Vector3(-1, 1, 0f), new Vector2(0, 1));
            vertices[1] = new VertexPositionTexture(new Vector3(1, 1, 0f), new Vector2(1, 1));
            vertices[2] = new VertexPositionTexture(new Vector3(-1, -1, 0f), new Vector2(0, 0));
            vertices[3] = new VertexPositionTexture(new Vector3(1, -1, 0f), new Vector2(1, 0));

            return vertices;
        }
    }
}