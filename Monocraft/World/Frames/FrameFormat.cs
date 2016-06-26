#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: FrameFormat.cs
// Date - created: 2016.06.18 - 20:15
// Date - current: 2016.06.26 - 11:08

#endregion

namespace Monocraft.World.Frames
{
    struct FrameFormat
    {
        // The location of the model/texture
        public string Model { get; set; }
        public string Texture { get; set; }
        public bool Translucent { get; set; }

        public FrameFormat(string model, string texture, bool translucent)
        {
            Model = model;
            Texture = texture;
            Translucent = translucent;
        }
    }
}