#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: PerlinNoise.cs
// Date - created: 2016.06.19 - 13:20
// Date - current: 2016.06.24 - 13:09

#endregion

#region Usings

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monocraft.World;

#endregion

namespace Monocraft.Basics
{
    static class PerlinNoise
    {
        public const int RESOLUTION = 10;
        private static RenderTarget2D cloudsRenderTarget;
        private static RenderTarget2D temp1;
        private static Texture2D cloudStaticMap;
        private static Effect PerlinEffect;
        private static Effect Blur;

        public static void Initialise(GraphicsDevice device, ContentManager content)
        {
            PerlinEffect = content.Load<Effect>("Effects/PerlinNoise");
            PerlinEffect.CurrentTechnique = PerlinEffect.Techniques["PerlinNoise"];

            cloudsRenderTarget = new RenderTarget2D(device, WorldTile.WORLD_TILE_WIDTH*RESOLUTION,
                WorldTile.WORLD_TILE_WIDTH*RESOLUTION);
            temp1 = new RenderTarget2D(device, WorldTile.WORLD_TILE_WIDTH*RESOLUTION,
                WorldTile.WORLD_TILE_WIDTH*RESOLUTION);

            cloudStaticMap = CreateStaticMap(WorldTile.WORLD_TILE_WIDTH*WorldTile.WORLD_TILE_WIDTH*RESOLUTION, device);
            //cloudStaticMap = content.Load<Texture2D>("Textures/Frames/grass");

            Blur = content.Load<Effect>("Effects/GaussianBlur");
            Blur.CurrentTechnique = Blur.Techniques["GaussianBlur"];
        }

        public static Texture2D CreateStaticMap(int resolution, GraphicsDevice device)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var noise = new Color[resolution*resolution];
            for (var x = 0; x < resolution; x++)
            {
                for (var y = 0; y < resolution; y++)
                {
                    noise[x + y*resolution] = new Color(new Vector3(rand.Next(1000)/1000f, 0, 0));
                }
            }

            var toRet = new Texture2D(device, resolution, resolution);
            toRet.SetData(noise);
            return toRet;
        }

        public static Texture2D GeneratePerlinNoiseGPU(float time, GraphicsDevice device, SpriteBatch sp)
        {
            device.SetRenderTarget(cloudsRenderTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            //sp.Begin(SpriteSortMode.Deferred, null, null, null, null, PerlinEffect);
            //foreach (EffectPass pass in PerlinEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();

            //    PerlinEffect.Parameters["xTexture"].SetValue(cloudStaticMap);
            //    PerlinEffect.Parameters["xOvercast"].SetValue(1.1f);
            //    PerlinEffect.Parameters["xTime"].SetValue(time);
            //    sp.Draw(cloudStaticMap, new Vector2(), Color.White);
            //}
            //sp.End();

            sp.Begin(SpriteSortMode.Deferred, null, null, null, null, Blur);
            //sp.Begin();

            Blur.Parameters["xTexture"].SetValue(cloudsRenderTarget);
            Blur.Parameters["xPixelHeight"].SetValue(1f / cloudsRenderTarget.Height);
            Blur.Parameters["xPixelWidth"].SetValue(1f / cloudsRenderTarget.Width);
            foreach (EffectPass pass in Blur.CurrentTechnique.Passes)
            {
                pass.Apply();

                sp.Draw(Game1.SolidWhite,null,new Rectangle(0,0,cloudStaticMap.Width,cloudStaticMap.Height),null,null,0f,null,Color.White);
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
    }
}