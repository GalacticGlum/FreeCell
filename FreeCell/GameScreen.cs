/*
 * Author: Shon Verch
 * File Name: GameScreen.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: The base game screen.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreeCell
{
    /// <summary>
    /// The base game screen.
    /// </summary>
    public abstract class GameScreen
    {
        /// <summary>
        /// The <see cref="SpriteBatch"/> instance.
        /// </summary>
        protected SpriteBatch spriteBatch;

        /// <summary>
        /// Called once per game and is the place to load all content for this <see cref="GameScreen"/>.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance.</param>
        public virtual void LoadContent(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        /// <summary>
        /// Called whenever this <see cref="GameScreen"/> is shown.
        /// <remarks>
        /// This is NOT an initialization method. It will be called every time
        /// the game screen switches to this <see cref="GameScreenType"/>.
        /// </remarks>
        /// </summary>
        public virtual void OnScreenStarted() { }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Draw(GameTime gameTime) { }
    }
}
