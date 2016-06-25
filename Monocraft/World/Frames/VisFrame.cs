#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: VisFrame.cs
// Date - created: 2016.06.23 - 13:11
// Date - current: 2016.06.24 - 13:09

#endregion

namespace Monocraft.World.Frames
{
    public class VisFrame
    {
        public readonly Frame Frame;

        public bool Visible;

        public VisFrame(Frame frame)
        {
            Frame = frame;
            Visible = true;
        }
    }
}