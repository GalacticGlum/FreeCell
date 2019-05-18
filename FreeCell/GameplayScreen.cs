/*
 * Author: Shon Verch
 * File Name: GameplayScreen.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: The game screen containing all the gameplay.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreeCell
{
    /// <summary>
    /// The <see cref="GameScreen"/> containing all the gameplay.
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        private Texture2D tableTexture;

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            base.LoadContent(spriteBatch);
            tableTexture = MainGame.Context.Content.Load<Texture2D>("Table");
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(tableTexture, Vector2.Zero, Color.White);
        }
    }
}
