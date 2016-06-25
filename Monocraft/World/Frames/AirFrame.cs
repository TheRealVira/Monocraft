#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: AirFrame.cs
// Date - created: 2016.06.23 - 12:21
// Date - current: 2016.06.25 - 18:38

#endregion

#region Usings

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Monocraft.World.Frames
{
    class AirFrame : Frame
    {
        public AirFrame() : base(null, null, true)
        {
        }

        public override void Draw(GraphicsDevice device, Matrix projection, Matrix view, Vector3 position)
        {
        }
    }
}