/*
 * Author: Shon Verch
 * File Name: FoundationPile.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: The foundation card pile.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities;

namespace FreeCell
{
    /// <summary>
    /// The foundation <see cref="CardPile"/>.
    /// </summary>
    public class FoundationPile : CardPile
    {
        /// <summary>
        /// The <see cref="CardSuit"/> that this <see cref="FoundationPile"/> allows.
        /// </summary>
        public CardSuit Suit { get; }

        /// <summary>
        /// A collection of all the <see cref="FoundationPile"/> textures mapped by <see cref="CardSuit"/>s.
        /// </summary>
        private readonly Texture2D foundationPileTexture;

        /// <summary>
        /// Initializes a new <see cref="FoundationPile"/>.
        /// <remarks>
        /// A <see cref="FoundationPile"/> has a maximum size of 13: ace through king, one card each.
        /// </remarks>
        /// </summary>
        public FoundationPile(CardSuit suit, RectangleF rectangle) : base(13, rectangle)
        {
            Suit = suit;
            foundationPileTexture = GetTexture(suit);
        }

        /// <summary>
        /// Indicates whether the specified <see cref="Card"/> can be pushed on this <see cref="FoundationPile"/>.
        /// </summary>
        /// <param name="card">The <see cref="Card"/> to push onto this <see cref="FoundationPile"/>.</param>
        /// <returns>A boolean value indicating whether the <see cref="Card"/> can be pushed.</returns>
        protected override bool CanPush(Card card)
        {
            // The suit of the card must match the suit of the foundation piles.
            if (card.Suit != Suit) return false;

            // If our pile is empty, only an ace can be pushed onto the pile.
            if (Empty && card.Rank == CardRank.Ace) return true;

            // With a non-empty pile, only a card that is one bigger in rank than
            // the current top card can be pushed onto the pile.
            return (int) card.Rank == (int) Peek().Rank + 1;
        }

        /// <summary>
        /// Draw this <see cref="FoundationPile"/>.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(foundationPileTexture, Rectangle.Position, Color.White);
        }

        /// <summary>
        /// Retrieve the texture for the <see cref="FoundationPile"/> of the specified <see cref="CardSuit"/>.
        /// </summary>
        /// <param name="suit">The <see cref="CardSuit"/> of the <see cref="FoundationPile"/>.</param>
        public static Texture2D GetTexture(CardSuit suit)
        {
            // Resolve the name of the foundation pile texture for the current suit.
            string foundationPileTextureName = $"Cards/foundation{Card.CardSuitIdentifier[suit]}";
            return MainGame.Context.Content.Load<Texture2D>(foundationPileTextureName);
        }
    }
}
