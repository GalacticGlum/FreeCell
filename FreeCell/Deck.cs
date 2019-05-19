/*
 * Author: Shon Verch
 * File Name: Deck.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: A collection of cards.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Random = MonoGameUtilities.Random;

namespace FreeCell
{
    /// <summary>
    /// A collection of cards.
    /// </summary>
    public class Deck : IEnumerable<Card>
    {
        /// <summary>
        /// The maximum game seed for the shuffling algorithm.
        /// </summary>
        public const int MaximumGameSeed = 32000;

        /// <summary>
        /// The minimum game seed for the shuffling algorithm.
        /// </summary>
        public const int MinimumGameSeed = 1;

        /// <summary>
        /// The cards of this <see cref="Deck"/>.
        /// </summary>
        private Card[] cards = new Card[52];

        /// <summary>
        /// The game seed used to shuffle this <see cref="Deck"/>.
        /// </summary>
        public int Seed { get; private set; }

        /// <summary>
        /// The number of <see cref="Card"/>s in this <see cref="Deck"/>.
        /// </summary>
        public int Count => cards.Length;

        /// <summary>
        /// Gets/sets the <see cref="Card"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index in this <see cref="Deck"/>.</param>
        public Card this[int index]
        {
            get => cards[index];
            set => cards[index] = value;
        }

        /// <summary>
        /// Initialize a new <see cref="Deck"/> in a pre-shuffled state.
        /// <remarks>
        /// The 'pre-shuffled' state refers to the deck in a sorted order of <see cref="CardSuit.Clubs"/>,
        /// <see cref="CardSuit.Diamonds"/>, <see cref="CardSuit.Hearts"/>, and <see cref="CardSuit.Spades"/>,
        /// for each card value from <see cref="CardRank.Ace"/> to <see cref="CardRank.King"/>:
        /// AC, AD, AH, AS, 2C, 2D, ..., KC, KD, KH, KD.
        /// </remarks>
        /// </summary>
        public Deck()
        {
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = new Card(i);
            }
        }

        /// <summary>
        /// Shuffle this <see cref="Deck"/> based on the specified <paramref name="seed"/>.
        /// <remarks>
        /// <par>The <paramref name="seed"/> must exist in the interval [<see cref="MinimumGameSeed"/>, <see cref="MaximumGameSeed"/>].</par>
        /// <par>
        /// If the <paramref name="seed"/> is outside these bounds, it will automatically be "resized" according to the formula
        /// min(<see cref="MaximumGameSeed"/>, max(<see cref="MinimumGameSeed"/>, <see cref="Seed"/>)).
        /// </par>
        /// </remarks>
        /// </summary>
        /// <param name="seed"></param>
        public void Shuffle(int seed)
        {
            // "Resize" the seed, if needed, to ensure that it is in the interval.
            Seed = Math.Min(MaximumGameSeed, Math.Max(MinimumGameSeed, seed));
            CardRandomEngine cardRandomEngine = new CardRandomEngine(Seed);

            // Create a temporary array to store the cards as they are shuffled.
            Card[] cardBuffer = new Card[cards.Length];

            for (int i = 0; i < cards.Length; i++)
            {
                int swapIndex = cards.Length - i;
                int index = cardRandomEngine.Next() % swapIndex;

                cardBuffer[i] = cards[index];
                cards[index] = cards[swapIndex - 1];
            }

            cards = cardBuffer;
        }

        /// <summary>
        /// Retrieve the <see cref="IEnumerator{T}"/> for this <see cref="Deck"/> which iterates over the stored <see cref="Card"/> collection.
        /// </summary>
        public IEnumerator<Card> GetEnumerator() => (IEnumerator<Card>) cards.GetEnumerator();

        /// <summary>
        /// Retrieve the <see cref="IEnumerator{T}"/> for this <see cref="Deck"/> which iterates over the stored <see cref="Card"/> collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
