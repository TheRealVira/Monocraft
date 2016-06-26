#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: FrameManager.cs
// Date - created: 2016.06.18 - 19:58
// Date - current: 2016.06.26 - 11:08

#endregion

#region Usings

using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monocraft.Basics;

#endregion

namespace Monocraft.World.Frames
{
    static class FrameManager
    {
        /// <summary>
        ///     The frames are saved in a dictionary with the following format:
        ///     [Name, Frame] (quite sounds like Mainframe btw)
        /// </summary>
        public static Dictionary<string, Frame> Frames;

        /// <summary>
        ///     Initialises all frames.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="device">The device.</param>
        /// <param name="sp">The sp.</param>
        public static void Initialise(ContentManager content, GraphicsDevice device, SpriteBatch sp)
        {
            Frames = new Dictionary<string, Frame>();

            //var test = new Dictionary<string, FrameFormat>();
            //test.Add("Wood", new FrameFormat("Frame", "Wood_floor_texture_sketchup_warehouse_type008",false));
            //FrameParser.SaveFrames(test, Directory.GetCurrentDirectory() + "\\Content\\Frames");
            foreach (
                var frameFormat in
                    JsonParser<FrameFormat>.LoadObjects(Directory.GetCurrentDirectory() + "\\Content\\Frames"))
            {
                Frames.Add(frameFormat.Key,
                    new Frame(content.Load<Texture2D>($"Textures/Frames/{frameFormat.Value.Texture}"),
                        content.Load<Model>($"Models/Frames/{frameFormat.Value.Model}"), frameFormat.Value.Translucent));
            }

            Frames.Add("Air", new AirFrame());
            //Frames.Add("test",new Frame(PerlinNoise.GeneratePerlinNoiseGPU(1,device,sp), content.Load<Model>($"Models/Frames/Frame"), false));
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources. For a greener planet.
        /// </summary>
        public static void Dispose()
        {
            foreach (var frame in Frames)
            {
                frame.Value.Dispose();
            }

            Frames.Clear();
            Frames = null;
        }
    }
}