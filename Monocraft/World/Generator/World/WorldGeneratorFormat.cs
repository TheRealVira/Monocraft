#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: WorldGeneratorFormat.cs
// Date - created: 2016.06.22 - 09:49
// Date - current: 2016.06.24 - 13:09

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
        public TerrainGeneratorTyp TGType { get; set; }

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