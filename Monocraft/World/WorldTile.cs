#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: WorldTile.cs
// Date - created: 2016.06.19 - 11:23
// Date - current: 2016.06.25 - 18:38

#endregion

#region Usings

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocraft.World.Frames;
using Monocraft.World.Generator.World;

#endregion

namespace Monocraft.World
{
    public class WorldTile
    {
        public const int WORLD_TILE_WIDTH = 16;
        public const int WORLD_TILE_HEIGHT = 256;

        // Initialize an array of indices for the box. 12 lines require 24 indices
        private static readonly short[] BBoxIndices =
        {
            0, 1, 1, 2, 2, 3, 3, 0, // Front edges
            4, 5, 5, 6, 6, 7, 7, 4, // Back edges
            0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
        };

        public readonly VisFrame[,,] Frames;
        public readonly WorldTile[] Neighbours;
        public readonly Vector3 Position;
        private BoundingBox Trigger;

        /// <param name="neighbours">
        ///     Arrayinformation:
        ///     [0]=front
        ///     [1]=right
        ///     [2]=back
        ///     [3]=left
        /// </param>
        public WorldTile(Vector3 position, WorldTile[] neighbours, WorldGenerator generator, Random rand,
            GraphicsDevice device, SpriteBatch sp)
        {
            Frames = new VisFrame[WORLD_TILE_WIDTH, WORLD_TILE_HEIGHT, WORLD_TILE_WIDTH];
            Position = position;
            Trigger = new BoundingBox(new Vector3(Position.X, 0, Position.Z),
                new Vector3(Position.X + WORLD_TILE_WIDTH - 1, WORLD_TILE_HEIGHT, Position.Z + WORLD_TILE_WIDTH - 1));
            generator.Generate(Position, Frames, neighbours, device, sp);
            Neighbours = neighbours;
        }

        /// <summary>
        ///     Draws the worldtile.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="projection">The projection.</param>
        /// <param name="view">The view.</param>
        /// <param name="cameraFrustum"></param>
        /// <param name="deepness"></param>
        public void Draw(GraphicsDevice device, Matrix projection, Matrix view, BoundingFrustum cameraFrustum,
            byte deepness = 2)
        {
            device.RasterizerState = new RasterizerState {FillMode = FillMode.Solid};
            for (var x = 0; x < Frames.GetLength(0); x++)
            {
                for (var y = 0; y < Frames.GetLength(1); y++)
                {
                    for (var z = 0; z < Frames.GetLength(2); z++)
                    {
                        if (Frames[x, y, z].Visible &&
                            cameraFrustum.Contains(new Vector3(x + Position.X, y + Position.Y, z + Position.Z)) !=
                            ContainmentType.Disjoint
                            /* ||cameraFrustum.Contains(new Vector3(x + Frames[x, y, z].Frame.Width+Position.X, y + Frames[x, y, z].Frame.Height+Position.Y, z+Frames[x,y,z].Frame.Depth + Position.Z)) != ContainmentType.Disjoint*/)
                        {
                            Frames[x, y, z].Frame.Draw(device, projection, view,
                                new Vector3(Position.X + x, Position.Y + y, Position.Z + z));
                        }
                    }
                }
            }

#if (DEBUG)
            var vase = new BasicEffect(device);
            var corners = Trigger.GetCorners();
            var primitiveList = new VertexPositionColor[corners.Length];

            // Assign the 8 box vertices
            for (var i = 0; i < corners.Length; i++)
            {
                primitiveList[i] = new VertexPositionColor(corners[i], Color.Red);
            }

            /* Set your own effect parameters here */

            vase.World = Matrix.Identity;
            vase.View = view;
            vase.Projection = projection;
            vase.VertexColorEnabled = true;
            vase.TextureEnabled = false;

            // Draw the box with a LineList
            foreach (var pass in vase.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList, primitiveList, 0, 8,
                    BBoxIndices, 0, 12);
            }
#endif
            if (deepness == 0) return;

            for (var i = 0; i < Neighbours.Length; i++)
            {
                if (Neighbours[i] != null)
                {
                    Neighbours[i].Draw(device, projection, view, cameraFrustum, (byte) (deepness - 1));
                }
            }
        }

        /// <summary>
        ///     Updates the visibility.
        /// </summary>
        public void UpdateVis(int deepness = 1)
        {
            var temp = new bool[Frames.GetLength(0), Frames.GetLength(1), Frames.GetLength(2)];
            // Temporarly save the visibility state in an three dimensional array

            for (var x = 0; x < Frames.GetLength(0); x++)
            {
                for (var y = 0; y < Frames.GetLength(1); y++)
                {
                    for (var z = 0; z < Frames.GetLength(2); z++)
                    {
                        // Every translucent frame will be drawn
                        if (Frames[x, y, z].Frame.Translucent)
                        {
                            temp[x, y, z] = true;
                            continue;
                        }

                        // A list of all neighbours
                        var neig = new List<VisFrame>();

                        // left
                        if (x == 0)
                        {
                            if (Neighbours[3] != null)
                            {
                                neig.Add(Neighbours[3].Frames[WORLD_TILE_WIDTH - 1, y, z]);
                            }
                            else
                            {
                                temp[x, y, z] = false;
                                //continue;
                            }
                        }
                        else
                        {
                            neig.Add(Frames[x - 1, y, z]);
                        }

                        // right
                        if (x == WORLD_TILE_WIDTH - 1)
                        {
                            if (Neighbours[1] != null)
                            {
                                neig.Add(Neighbours[1].Frames[0, y, z]);
                            }
                            else
                            {
                                temp[x, y, z] = false;
                                //continue;
                            }
                        }
                        else
                        {
                            neig.Add(Frames[x + 1, y, z]);
                        }

                        // front
                        if (z == WORLD_TILE_WIDTH - 1)
                        {
                            if (Neighbours[0] != null)
                            {
                                neig.Add(Neighbours[0].Frames[x, y, 0]);
                            }
                            else
                            {
                                temp[x, y, z] = false;
                                //continue;
                            }
                        }
                        else
                        {
                            neig.Add(Frames[x, y, z + 1]);
                        }

                        // back
                        if (z == 0)
                        {
                            if (Neighbours[2] != null)
                            {
                                neig.Add(Neighbours[2].Frames[x, y, WORLD_TILE_WIDTH - 1]);
                            }
                            else
                            {
                                temp[x, y, z] = false;
                                //continue;
                            }
                        }
                        else
                        {
                            neig.Add(Frames[x, y, z - 1]);
                        }

                        if (y != 0)
                        {
                            neig.Add(Frames[x, y - 1, z]); // bottom
                        }

                        if (y != WORLD_TILE_HEIGHT - 1)
                        {
                            neig.Add(Frames[x, y + 1, z]); // top
                        }

                        // If any neighbour is invisible or translucent, we'll have to draw the current frame (setting the visibility state to true)
                        var done = false;
                        for (var i = 0; i < neig.Count; i++)
                        {
                            if (neig[i].Frame.Translucent /* || !neig[i].Visible*/)
                            {
                                temp[x, y, z] = true;
                                done = true;
                                break;
                            }
                        }

                        if (done) continue;

                        temp[x, y, z] = false;
                    }
                }
            }

            // Set the visibility of all frames
            for (var x = 0; x < Frames.GetLength(0); x++)
            {
                for (var y = 0; y < Frames.GetLength(1); y++)
                {
                    for (var z = 0; z < Frames.GetLength(2); z++)
                    {
                        Frames[x, y, z].Visible = temp[x, y, z];
                    }
                }
            }

            if (deepness != 0)
            {
                for (var i = 0; i < Neighbours.Length; i++)
                {
                    if (Neighbours[i] != null)
                    {
                        Neighbours[i].UpdateVis(deepness - 1);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the height of the position in a worldtile.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public float GetHeightOf(Point position)
        {
            var y = 0;
            while (!(Frames[position.X, y, position.Y].Frame is AirFrame))
            {
                y++;
            }

            return y;
        }

        /// <summary>
        ///     Determines whether the worldtile contains the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public bool ContainsPosition(Vector3 position)
        {
            return Trigger.Contains(position) == ContainmentType.Contains;
        }

        /// <summary>
        ///     Searches a world tile (by a given position) (revursive).
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public WorldTile SearchWorldTile(Vector3 position)
        {
            return SearchWorldTile(position, new List<Vector3> {Position});
        }

        private WorldTile SearchWorldTile(Vector3 position, List<Vector3> searchedThroughPos)
        {
            // Try searching the tile in the attached neighbours:
            for (var i = 0; i < Neighbours.Length; i++)
            {
                if (Neighbours[i] != null && Neighbours[i].Position == position)
                {
                    return Neighbours[i];
                }
            }

            // Try searching the tile from the neighbours sight (recursive):
            for (var i = 0; i < Neighbours.Length; i++)
            {
                if (Neighbours[i] != null && !searchedThroughPos.Contains(Neighbours[i].Position))
                {
                    searchedThroughPos.Add(Neighbours[i].Position);
                    var test = Neighbours[i].SearchWorldTile(position, searchedThroughPos);
                    if (test != null)
                    {
                        return test;
                    }
                }
            }

            return null;
        }
    }
}