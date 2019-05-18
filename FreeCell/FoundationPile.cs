/*
 * Author: Shon Verch
 * File Name: FoundationPile.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: The foundation card pile.
 */

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
        /// Initializes a new <see cref="FoundationPile"/>.
        /// <remarks>
        /// A <see cref="FoundationPile"/> has a maximum size of 13: ace through king, one card each.
        /// </remarks>
        /// </summary>
        public FoundationPile(CardSuit suit) : base(13)
        {
            Suit = suit;
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
    }
}
