/*
 * Author: Shon Verch
 * File Name: MainGame.cs
 * Project Name: FreeCell
 * Creation Date: 05/16/2019
 * Modified Date: 05/16/2019
 * Description: The core engine instance of the game that spawns all other entities
 *              and simulates logic and rendering.
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities;
using MonoGameUtilities.Logging;

namespace FreeCell
{
    /// <summary>
    /// The core engine instance of the game that spawns
    /// all other entities and simulates logic and rendering.
    /// </summary>
    public class MainGame : Game
    {
        /// <summary>
        /// The width of the game window in pixels.
        /// </summary>
        public const int GameScreenWidth = 1193;

        /// <summary>
        /// The height of the game window in pixels.
        /// </summary>
        public const int GameScreenHeight = 671;

        /// <summary>
        /// The current instance of this <see cref="MainGame"/>.
        /// </summary>
        public static MainGame Context { get; private set; }

        /// <summary>
        /// The <see cref="FreeCell.GameScreenManager"/>.
        /// </summary>
        public GameScreenManager GameScreenManager { get; }

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Initialize the <see cref="MainGame"/>.
        /// </summary>
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Context = this;

            GameScreenManager = new GameScreenManager();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Update the size of the game screen
            graphics.PreferredBackBufferWidth = GameScreenWidth;
            graphics.PreferredBackBufferHeight = GameScreenHeight;
            graphics.ApplyChanges();

            Logger.Destination = LoggerDestination.File;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameScreenManager.LoadContent(spriteBatch);

            GameScreenManager.SwitchScreen<GameplayScreen>();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // We NEED to update input before we execute game logic
            // so that the gameplay does not lag by a frame (due to not synchronized input).
            Input.Update();
            GameScreenManager.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            GameScreenManager.Draw(gameTime);
            spriteBatch.End();
        }

        /// <summary>
        /// Called when the game is closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(object sender, EventArgs args)
        {
            // Write the logger buffer to the file system
            Logger.FlushMessageBuffer();
        }
    }
}
