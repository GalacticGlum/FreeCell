/*
 * Author: Shon Verch
 * File Name: FreeCell.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: The free cell card pile.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities;

namespace FreeCell
{
    /// <summary>
    /// The free cell <see cref="CardPile"/>.
    /// </summary>
    public class FreeCell : CardPile
    {
        /// <summary>
        /// The <see cref="Card"/> occupying this <see cref="FreeCell"/>.
        /// </summary>
        public Card Value => Peek();

        /// <summary>
        /// The texture of a <see cref="FreeCell"/> pile.
        /// </summary>
        private readonly Texture2D freeCellTexture;

        /// <summary>
        /// Initializes a new <see cref="FreeCell"/>.
        /// </summary>
        public FreeCell(RectangleF rectangle) : base(1, rectangle)
        {
            freeCellTexture = MainGame.Context.Content.Load<Texture2D>("Cards/freeCell");
        }

        /// <summary>
        /// A <see cref="Card"/> can only be pushed onto this <see cref="FreeCell"/> if
        /// another <see cref="Card"/> is not already occupying it (i.e. it is empty).
        /// </summary>
        /// <param name="card">The <see cref="Card"/> to be pushed.</param>
        /// <returns>A boolean value indicating whether the <see cref="Card"/> can be pushed onto this <see cref="FreeCell"/>.</returns>
        public override bool CanPush(Card card) => Empty;

        /// <summary>
        /// Gets the rectangle of a <see cref="Card"/> as it were in this <see cref="FreeCell"/>.
        /// </summary>
        public override RectangleF GetCardRectangle(Card card)
        {
            float offsetX = 0.5f * (freeCellTexture.Width - card.Texture.Width);
            float offsetY = 0.5f * (freeCellTexture.Height - card.Texture.Height);

            return new RectangleF(Rectangle.Position + new Vector2(offsetX, offsetY), Rectangle.Size);
        }

        /// <summary>
        /// Draw this <see cref="FreeCell"/>.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(freeCellTexture, Rectangle.Position, Color.White);

            // Make sure that our free cell actually has a card on it.
            if (!Empty && Value.Rectangle.HasValue)
            {
                GameplayScreen gameplayScreen = MainGame.Context.GameScreenManager.Get<GameplayScreen>();

                // Determine whether the card should glow.
                bool isSelected = false;
                if (gameplayScreen.CurrentSelection != null)
                {
                    isSelected = gameplayScreen.CurrentSelection.Card == Value;
                }

                Value.Draw(spriteBatch, isGlowing: isSelected);
            }
        }
    }
}
