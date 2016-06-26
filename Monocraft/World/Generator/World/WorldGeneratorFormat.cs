#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: WorldGeneratorFormat.cs
// Date - created: 2016.06.26 - 19:34
// Date - current: 2016.06.26 - 20:15

#endregion

#region Usings

using System.Collections.Generic;

#endregion

namespace Monocraft.World.Generator.World
{
    struct WorldGeneratorFormat
    {
        public int MaxHeight { get; set; }
        public int MinHeight { get; set; }
        public WorldGeneratorTyp TGType { get; set; }

        /// <summary>
        ///     Key = int or * (Represents how many blocks should be together
        ///     Value = Frame(name)
        /// </summary>
        public List<FrameHeight> ColumnData { get; set; }
    }

    public struct FrameHeight
    {
        public byte Height; // 0 = fill space between
        public string Frame; // Frame(name)
    }
}