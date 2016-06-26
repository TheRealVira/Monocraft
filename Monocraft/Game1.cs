#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: Game1.cs
// Date - created: 2016.06.18 - 18:57
// Date - current: 2016.06.25 - 18:38

#endregion

#region Usings

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocraft.Basics;
using Monocraft.World.Frames;
using Monocraft.World.Generator.World;

#endregion

namespace Monocraft
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private World.World TestWorld;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public static KeyboardState NewKeyboardState { get; set; }
        public static KeyboardState OldKeyboardState { get; set; }
        public static MouseState NewMouseState { get; set; }
        public static MouseState OldMouseState { get; set; }
        public static Texture2D SolidWhite { get; set; }
        public static Model TestingModel { get; set; }
        public static bool STOPP_WORKING { get; private set; }

        public static BasicEffect NormalEffect;

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
#if (DEBUG)
            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
#else
            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
#endif

            NormalEffect =new BasicEffect(graphics.GraphicsDevice);
            PerlinNoise.Initialise(200,graphics.GraphicsDevice, Content);
            SolidWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            SolidWhite.SetData(new[] {Color.White});
            TestingModel = Content.Load<Model>("Models/Frames/Frame");

            base.Initialize();
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            FrameManager.Initialise(Content, graphics.GraphicsDevice, spriteBatch);
            WorldGeneratorManager.Initialise(Content, graphics.GraphicsDevice, spriteBatch);
            TestWorld = new World.World(WorldGeneratorManager.WorldGenerators["Default"], graphics.GraphicsDevice,
                Content,
                DateTime.Now.Millisecond, spriteBatch);
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload
        ///     game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            FrameManager.Dispose();
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            NewMouseState = Mouse.GetState();
            NewKeyboardState = Keyboard.GetState();

            if (WasKeyKlicked(Keys.Escape))
            {
                Exit();
                STOPP_WORKING = true;
            }

            TestWorld.Update(gameTime, graphics, spriteBatch);

            OldMouseState = NewMouseState;
            OldKeyboardState = NewKeyboardState;

            time++;

            base.Update(gameTime);
        }

        public static float time;

        public static bool WasKeyKlicked(Keys key)
        {
            return NewKeyboardState.IsKeyUp(key) && OldKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);

            TestWorld.Draw(spriteBatch, graphics.GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}