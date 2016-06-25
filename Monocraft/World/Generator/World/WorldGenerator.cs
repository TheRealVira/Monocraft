#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: WorldGenerator.cs
// Date - created: 2016.06.22 - 09:55
// Date - current: 2016.06.24 - 13:09

#endregion

#region Usings

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocraft.Basics;
using Monocraft.World.Frames;

#endregion

namespace Monocraft.World.Generator.World
{
    public class WorldGenerator
    {
        private readonly List<FrameHeight> _columnData;
        private readonly int _maxHeight;
        private readonly int _minHeight;
        private readonly WorldGeneratorTyp _tgType; // May be removed
        private readonly int BottomFramesHeightTogether; // May be removed

        public readonly GenerateD Generate;
        private readonly int UpperFramesHeightTogether;

        public WorldGenerator(int maxHeight, int minHeight, WorldGeneratorTyp tgType,
            List<FrameHeight> columnData)
        {
            _tgType = tgType;
            _columnData = columnData;

            var star = false;
            var heightInsg = 0;

            for (var i = _columnData.Count - 1; i > -1; i--)
            {
                if (_columnData[i].Height == 0)
                {
                    if (star)
                    {
                        _columnData.Remove(_columnData[i]);
                        continue;
                    }

                    star = true;
                }
            }

            star = false;
            for (var i = 0; i < _columnData.Count; i++)
            {
                heightInsg += _columnData[i].Height;

                if (star)
                {
                    UpperFramesHeightTogether += _columnData[i].Height;
                }
                else
                {
                    BottomFramesHeightTogether += _columnData[i].Height;
                }

                if (_columnData[i].Height == 0)
                {
                    star = true;
                }
            }

            _maxHeight = maxHeight > heightInsg ? maxHeight : heightInsg;
            _minHeight = maxHeight > minHeight ? minHeight : maxHeight;

            switch (tgType)
            {
                case WorldGeneratorTyp.Noisy:
                    Generate = PerlinNoiseGenration;
                    break;

                default:
                    Generate = NormalGeneration;
                    break;
            }
        }

        private void PerlinNoiseGenration(Vector3 position, VisFrame[,,] frames, WorldTile[] neighbours,
            GraphicsDevice device, SpriteBatch sp)
        {
            var grid = new Color[WorldTile.WORLD_TILE_WIDTH*WorldTile.WORLD_TILE_WIDTH*PerlinNoise.RESOLUTION];
            //PerlinNoise.CreateStaticMap(WorldTile.WORLD_TILE_WIDTH, device);
            PerlinNoise.GeneratePerlinNoiseGPU(position.X*position.Y, device, sp).GetData(grid);

            for (var x = 0; x < frames.GetLength(0); x++)
            {
                for (var z = 0; z < frames.GetLength(2); z++)
                {
                    var highest = grid[x*z*PerlinNoise.RESOLUTION].R < this._minHeight ? this._minHeight : grid[x*z*PerlinNoise.RESOLUTION].R;
                    var currentIndex = 0;
                    var counter = 0;

                    for (var y = 0; y < highest; y++)
                    {
                        frames[x, y, z] = new VisFrame(FrameManager.Frames[_columnData[currentIndex].Frame])
                        {
                            Visible = true
                        };
                        counter++;

                        if (_columnData[currentIndex].Height != 0)
                        {
                            if (counter >= _columnData[currentIndex].Height)
                            {
                                counter = 0;
                                currentIndex++;
                            }
                        }
                        else
                        {
                            if (highest - UpperFramesHeightTogether - 1 <= y)
                            {
                                counter = 0;
                                currentIndex++;
                            }
                        }
                    }
                }
            }

            FillWithAir(frames);
            //ApplyVis(frames,neighbours);
        }

        private void NormalGeneration(Vector3 position, VisFrame[,,] frames, WorldTile[] neighbours,
            GraphicsDevice device, SpriteBatch sp)
        {
            for (var x = 0; x < frames.GetLength(0); x++)
            {
                for (var z = 0; z < frames.GetLength(2); z++)
                {
                    var currentIndex = 0;
                    var counter = 0;

                    for (var y = 0; y < _maxHeight; y++)
                    {
                        frames[x, y, z] = new VisFrame(FrameManager.Frames[_columnData[currentIndex].Frame])
                        {
                            Visible = true
                        };
                        counter++;

                        if (_columnData[currentIndex].Height != 0)
                        {
                            if (counter >= _columnData[currentIndex].Height)
                            {
                                counter = 0;
                                currentIndex++;
                            }
                        }
                        else
                        {
                            if (_maxHeight - UpperFramesHeightTogether - 1 <= y)
                            {
                                counter = 0;
                                currentIndex++;
                            }
                        }
                    }
                }
            }

            FillWithAir(frames);
            //ApplyVis(frames, neighbours);
        }

        private void FillWithAir(VisFrame[,,] frames)
        {
            for (var x = 0; x < frames.GetLength(0); x++)
            {
                for (var z = 0; z < frames.GetLength(2); z++)
                {
                    for (var y = 0; y < WorldTile.WORLD_TILE_HEIGHT; y++)
                    {
                        if (frames[x, y, z] == null)
                        {
                            frames[x, y, z] = new VisFrame(FrameManager.Frames["Air"]) {Visible = false};
                        }
                    }
                }
            }
        }

        //private void ApplyVis(VisFrame[,,] Frames, List<WorldTile> neighbours)
        //{

        //}
    }
}