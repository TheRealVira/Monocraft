#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: VisFrame.cs
// Date - created: 2016.06.26 - 19:34
// Date - current: 2016.06.26 - 20:15

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