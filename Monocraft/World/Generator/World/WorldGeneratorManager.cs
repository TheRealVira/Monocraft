#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: WorldGeneratorManager.cs
// Date - created: 2016.06.22 - 10:05
// Date - current: 2016.06.24 - 13:09

#endregion

#region Usings

using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monocraft.Basics;

#endregion

namespace Monocraft.World.Generator.World
{
    static class WorldGeneratorManager
    {
        public static Dictionary<string, WorldGenerator> WorldGenerators;

        public static void Initialise(ContentManager content, GraphicsDevice device, SpriteBatch sp)
        {
            WorldGenerators = new Dictionary<string, WorldGenerator>();

            //var test = new Dictionary<string, WorldGeneratorFormat>();
            //test.Add("Wood", new WorldGeneratorFormat() {ColumnData = new List<FrameHeight>() { new FrameHeight() { Frame = "Sun", Height = 1 }, new FrameHeight() {Frame = "Dirt", Height=0},new FrameHeight() {Frame = "Gras",Height=1} }, MaxHeight = 5, MinHeight = 5, TGType = WorldGeneratorTyp.None});
            //JsonParser<WorldGeneratorFormat>.SaveObjects(test, Directory.GetCurrentDirectory() + "\\Content\\WorldGenerators");
            foreach (
                var frameFormat in
                    JsonParser<WorldGeneratorFormat>.LoadObjects(Directory.GetCurrentDirectory() +
                                                                 "\\Content\\WorldGenerators"))
            {
                WorldGenerators.Add(frameFormat.Key,
                    new WorldGenerator(frameFormat.Value.MaxHeight, frameFormat.Value.MinHeight,
                        frameFormat.Value.TGType, frameFormat.Value.ColumnData));
            }

            //Frames.Add("test",new Frame(PerlinNoise.GeneratePerlinNoiseGPU(1,device,sp), content.Load<Model>($"Models/Frames/Frame"), false));
        }
    }
}