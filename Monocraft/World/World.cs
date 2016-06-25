#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: World.cs
// Date - created: 2016.06.18 - 23:28
// Date - current: 2016.06.24 - 13:09

#endregion

#region Usings

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monocraft.World.Generator.World;

#endregion

namespace Monocraft.World
{
    class World
    {
        // Self explaining stuff:
        private readonly Player.Player _currentPlayer;

        // Quick-info: I thought it would be great to put the TileWorlds into a dictionary, so I'd find single tiles easier, but after rethinking it about twenty times, I got the idea
        //              of building some kind of 2D - grid (a 2d linked list) with them, which works pretty great btw :)

        //private Dictionary<Vector3,Frame> FrameWorld;
        //private readonly Dictionary<Vector3, WorldTile> TileWorld;

        /// <summary>
        ///     The _world generator will be used to generate the terrain of the single world tiles.
        /// </summary>
        private readonly WorldGenerator _worldGenerator;

        private readonly Random rand;

        /// <summary>
        ///     "The Grid" is the main point of the hole grid and will be used for searching tiles, loading worlds, saving worlds
        /// </summary>
        private readonly WorldTile TheGrid; // Btw. some kind of Tron reference here xD

        /// <summary>
        ///     The current (active) world tile, which is, where the player currently stands in
        /// </summary>
        private WorldTile CurrentWorldTile; // The current tile, which the player should be currently on.

        /// <summary>
        ///     The last loaded tile coordinates (which is used, to gain some performance)
        /// </summary>
        private Vector3 LastLoadedTile;

        /// <summary>
        ///     Initializes a new instance of the <see cref="World" /> class.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="device">The device.</param>
        /// <param name="content">The content.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="sp">The sp.</param>
        public World(WorldGenerator generator, GraphicsDevice device, ContentManager content, int seed, SpriteBatch sp)
        {
            // Initialise necessary stuff
            _worldGenerator = generator;
            rand = new Random(seed);
            _currentPlayer = new Player.Player(device, content);

            var left = new WorldTile(new Vector3(-WorldTile.WORLD_TILE_WIDTH, 0, 0), new WorldTile[4], _worldGenerator,
                rand, device, sp);
            var right = new WorldTile(new Vector3(WorldTile.WORLD_TILE_WIDTH, 0, 0), new WorldTile[4], _worldGenerator,
                rand, device, sp);
            var front = new WorldTile(new Vector3(0, 0, WorldTile.WORLD_TILE_WIDTH), new WorldTile[4], _worldGenerator,
                rand, device, sp);
            var back = new WorldTile(new Vector3(0, 0, -WorldTile.WORLD_TILE_WIDTH), new WorldTile[4], _worldGenerator,
                rand, device, sp);
            CurrentWorldTile = new WorldTile(new Vector3(0, 0, 0), new WorldTile[4] {front, right, back, left},
                _worldGenerator, rand, device, sp);

            // This will save us a reference to the main world tile:
            TheGrid = CurrentWorldTile;

            // Join the neighbours to the main world tile
            left.Neighbours[1] = CurrentWorldTile;
            right.Neighbours[3] = CurrentWorldTile;
            front.Neighbours[2] = CurrentWorldTile;
            back.Neighbours[0] = CurrentWorldTile;

            // Update the visability of all frames:
            CurrentWorldTile.UpdateVis();
            left.UpdateVis();
            right.UpdateVis();
            front.UpdateVis();
            back.UpdateVis();

            //Spawn the player in the middle of the current frame (on top of the highest frame (which isn't air))
            Spawn(new Vector3(WorldTile.WORLD_TILE_WIDTH/2f,
                CurrentWorldTile.GetHeightOf(new Point(WorldTile.WORLD_TILE_WIDTH/2, WorldTile.WORLD_TILE_WIDTH/2)) +
                1.8f,
                WorldTile.WORLD_TILE_WIDTH/2f));
        }

        /// <summary>
        ///     Updates this world.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="sp">The sp.</param>
        public void Update(GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch sp)
        {
            if (Game1.STOPP_WORKING) return; // Just, so we dont' run into some disposed objects

            _currentPlayer.Update(gameTime, graphics);

            var current = new Vector3((int) _currentPlayer.Cam.Position.X, 0, (int) _currentPlayer.Cam.Position.Z);
            var x = (float) Math.Floor(current.X/WorldTile.WORLD_TILE_WIDTH);
            var z = (float) Math.Floor(current.Z/WorldTile.WORLD_TILE_WIDTH);
            current = new Vector3(x*WorldTile.WORLD_TILE_WIDTH, 0, z*WorldTile.WORLD_TILE_WIDTH);

            if (LastLoadedTile == current) return; // Some performance improvements
            LastLoadedTile = current;

            // If the player doesn't stand int the current worldtile
            if (CurrentWorldTile.Position != current)
            {
                WorldTile willBeCurrent;

                // than search through all neighbours, if on of them contains the player
                for (var i = 0; i < CurrentWorldTile.Neighbours.Length; i++)
                {
                    if (CurrentWorldTile.Neighbours[i] == null) continue;

                    var tempV = CurrentWorldTile.Neighbours[i].Position;
                    if (tempV.X == current.X && tempV.Y == current.Y && tempV.Z == current.Z)
                        // If there is a match, than set the tempurarly willBeCurrent variable to the matched one
                    {
                        willBeCurrent = CurrentWorldTile.Neighbours[i];
                        goto FILL_NEIGHBOURS;
                        // <- Basically - I didn't want to initialise a new falg, so I just created a label :D
                    }
                }

                return;

                FILL_NEIGHBOURS: // Connect or create neighbours and join them together
                AppendNewWorldTile(new Vector3(current.X - WorldTile.WORLD_TILE_WIDTH, current.Y, current.Z),
                    graphics.GraphicsDevice, sp); // left
                AppendNewWorldTile(new Vector3(current.X + WorldTile.WORLD_TILE_WIDTH, current.Y, current.Z),
                    graphics.GraphicsDevice, sp); // right
                AppendNewWorldTile(new Vector3(current.X, current.Y, current.Z + WorldTile.WORLD_TILE_WIDTH),
                    graphics.GraphicsDevice, sp); // front
                AppendNewWorldTile(new Vector3(current.X, current.Y, current.Z - WorldTile.WORLD_TILE_WIDTH),
                    graphics.GraphicsDevice, sp); // back

                CurrentWorldTile.UpdateVis(); // Ouptading the visablity of the frames in the current (/last) world tile
                CurrentWorldTile = willBeCurrent; // Switch to the new world tile
                CurrentWorldTile.UpdateVis(); // Same as above
            }
        }

        /// <summary>
        ///     Appends a new world tile to the specified position (and links them together).
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="device">The device.</param>
        /// <param name="batch">The graphics.</param>
        private void AppendNewWorldTile(Vector3 position, GraphicsDevice device, SpriteBatch batch)
        {
            var mayCurrent = TheGrid.SearchWorldTile(position);
            if (mayCurrent != null) return; // If a world tile exists on that position, than be lazy and return

            // Gathering the neighbours
            var left =
                TheGrid.SearchWorldTile(new Vector3(position.X - WorldTile.WORLD_TILE_WIDTH, position.Y, position.Z));
            var right =
                TheGrid.SearchWorldTile(new Vector3(position.X + WorldTile.WORLD_TILE_WIDTH, position.Y, position.Z));
            var front =
                TheGrid.SearchWorldTile(new Vector3(position.X, position.Y, position.Z + WorldTile.WORLD_TILE_WIDTH));
            var back =
                TheGrid.SearchWorldTile(new Vector3(position.X, position.Y, position.Z - WorldTile.WORLD_TILE_WIDTH));

            if (left == null && right == null && front == null && back == null)
                return; // Not joined with any existing tile

            // Get the tile that should be appended
            var newOne = new WorldTile(position, new WorldTile[4] {front, right, back, left}, _worldGenerator,
                rand, device, batch);

            // If a neighbour isn't null, than join them together:
            if (left != null)
            {
                newOne.Neighbours[3] = left;
                left.Neighbours[1] = newOne;
                left.UpdateVis();
            }

            if (right != null)
            {
                newOne.Neighbours[1] = right;
                right.Neighbours[3] = newOne;
                right.UpdateVis();
            }

            if (front != null)
            {
                newOne.Neighbours[0] = front;
                front.Neighbours[2] = newOne;
                front.UpdateVis();
            }

            if (back != null)
            {
                newOne.Neighbours[2] = back;
                back.Neighbours[0] = newOne;
                back.UpdateVis();
            }
        }

        /// <summary>
        ///     Draws the current world tile.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="device">The device.</param>
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice device)
        {
            if (Game1.STOPP_WORKING) return; // Just, so we dont' run into some disposed objects

            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.RasterizerState = RasterizerState.CullCounterClockwise;
            device.SamplerStates[0] = SamplerState.LinearWrap;
            CurrentWorldTile.Draw(device, _currentPlayer.Cam.Projektion, _currentPlayer.Cam.View, new BoundingFrustum(_currentPlayer.Cam.View* (_currentPlayer.Cam.Projektion* Matrix.CreateScale(1))));

            //device.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame, CullMode = CullMode.None};
            _currentPlayer.Draw(spriteBatch, device);
        }

        /// <summary>
        ///     Spawns the player at the specified coords.
        /// </summary>
        /// <param name="playerCoords">The new player coords.</param>
        public void Spawn(Vector3 playerCoords)
        {
            _currentPlayer.TeleportTo(playerCoords);
        }
    }
}