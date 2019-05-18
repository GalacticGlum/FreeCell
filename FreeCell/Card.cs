/*
 * Author: Shon Verch
 * File Name: Card.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: A singular playing card.
 */

namespace FreeCell
{
    /// <summary>
    /// A singular playing card.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// The <see cref="CardRank"/> of this <see cref="Card"/>.
        /// </summary>
        public CardRank Rank { get; }

        /// <summary>
        /// The <see cref="CardSuit"/> of this <see cref="Card"/>.
        /// </summary>
        public CardSuit Suit { get; }

        /// <summary>
        /// A boolean value indicating whether this <see cref="Card"/> is red: <value>true</value> if it is; <value>false</value> otherwise.
        /// </summary>
        public bool IsRed => Suit == CardSuit.Diamonds || Suit == CardSuit.Hearts;

        /// <summary>
        /// Initializes a new <see cref="Card"/> with the specified <see cref="Rank"/> and <see cref="Suit"/>.
        /// </summary>
        /// <param name="rank">The <see cref="CardRank"/> of this <see cref="Card"/>.</param>
        /// <param name="suit">The <see cref="CardSuit"/> of this <see cref="Card"/>.</param>
        public Card(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        /// <summary>
        /// Initializes a new <see cref="Card"/> based on a zero-based <paramref name="index"/> of the card.
        /// <remarks>
        /// The <paramref name="index"/> refers to a sorted deck in the order of <see cref="CardSuit.Clubs"/>,
        /// <see cref="CardSuit.Diamonds"/>, <see cref="CardSuit.Hearts"/>, and <see cref="CardSuit.Spades"/>,
        /// for each card value from <see cref="CardRank.Ace"/> to <see cref="CardRank.King"/>:
        /// AC, AD, AH, AS, 2C, 2D, ..., KC, KD, KH, KD.
        /// </remarks>
        /// </summary>
        /// <param name="index">The index of the card in the sorted deck.</param>
        public Card(int index) : this((CardRank)(index / 4 + 1), (CardSuit)(index % 4)) { }
    }
}
