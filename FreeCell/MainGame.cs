/*
 * Author: Shon Verch
 * File Name: MainGame.cs
 * Project Name: FreeCell
 * Creation Date: 05/16/2019
 * Modified Date: 05/16/2019
 * Description: The core engine instance of the game that spawns all other entities
 *              and simulates logic and rendering.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
