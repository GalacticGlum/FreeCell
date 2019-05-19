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
        /// The <see cref="RectangleF"/> for this <see cref="FreeCell"/>.
        /// </summary>
        public RectangleF Rectangle { get; }

        /// <summary>
        /// The texture of a <see cref="FreeCell"/> pile.
        /// </summary>
        private readonly Texture2D freeCellTexture;

        /// <summary>
        /// Initializes a new <see cref="FreeCell"/>.
        /// </summary>
        public FreeCell(RectangleF rectangle) : base(1)
        {
            Rectangle = rectangle;
            freeCellTexture = MainGame.Context.Content.Load<Texture2D>("Cards/freeCell");
        }

        /// <summary>
        /// A <see cref="Card"/> can only be pushed onto this <see cref="FreeCell"/> if
        /// another <see cref="Card"/> is not already occupying it (i.e. it is empty).
        /// </summary>
        /// <param name="card">The <see cref="Card"/> to be pushed.</param>
        /// <returns>A boolean value indicating whether the <see cref="Card"/> can be pushed onto this <see cref="FreeCell"/>.</returns>
        protected override bool CanPush(Card card) => Empty;

        /// <summary>
        /// Draw this <see cref="FreeCell"/>.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(freeCellTexture, Rectangle.Position, Color.White);
        }
    }
}
