/*
 * Author: Shon Verch
 * File Name: CardPile.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: The base card pile.
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonoGameUtilities.Logging;

namespace FreeCell
{
    /// <summary>
    /// The base card pile.
    /// </summary>
    public abstract class CardPile : IEnumerable<Card>
    {
        /// <summary>
        /// The maximum size of this <see cref="CardPile"/>.
        /// </summary>
        public int MaximumSize { get; }

        /// <summary>
        /// The number of <see cref="Card"/>s in this <see cref="CardPile"/>.
        /// </summary>
        public int Count => topIndex + 1;

        /// <summary>
        /// A boolean value indicating whether this <see cref="CardPile"/> is empty.
        /// </summary>
        public bool Empty => topIndex < 0;

        /// <summary>
        /// Get the <see cref="Card"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the <see cref="Card"/> in this <see cref="CardPile"/>.</param>
        /// <returns>The <see cref="Card"/> at the specified <paramref name="index"/>.</returns>
        public Card this[int index] => data[index];

        /// <summary>
        /// The data of this <see cref="CardPile"/>.
        /// </summary>
        private readonly Card[] data;

        /// <summary>
        /// The index of the top of this <see cref="CardPile"/>.
        /// </summary>
        private int topIndex;

        /// <summary>
        /// Initializes a new <see cref="CardPile"/> with the specified <see cref="MaximumSize"/>.
        /// </summary>
        /// <param name="maximumSize">The maximum size of this <see cref="CardPile"/>.</param>
        protected CardPile(uint maximumSize)
        {
            MaximumSize = (int) maximumSize;
            data = new Card[maximumSize];

            topIndex = -1;
        }

        /// <summary>
        /// Pushes the specified <paramref name="card"/> onto this <see cref="CardPile"/>.
        /// </summary>
        /// <param name="card">The <see cref="Card"/> to push.</param>
        /// <param name="force">
        /// Ignores <see cref="CanPush"/> and always pushes the <see cref="Card"/>. Defaults to <value>false</value>.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the <paramref name="card"/> was successfully
        /// pushed onto this <see cref="CardPile"/>.
        /// </returns>
        public bool Push(Card card, bool force = false)
        {
            if (topIndex < MaximumSize)
            {
                if (!CanPush(card) && !force) return false;
                data[++topIndex] = card;

                OnPushed(card);
            }

            Logger.LogFunctionEntry(string.Empty, "Attempted to push card onto full card pile.", LoggerVerbosity.Warning);
            return false;

        }

        /// <summary>
        /// Indicates whether the specified <see cref="Card"/> can be pushed onto this <see cref="CardPile"/>.
        /// </summary>
        /// <param name="card">The <see cref="Card"/> to push.</param>
        /// <returns>A boolean value indicating whether the <see cref="Card"/> can be pushed.</returns>
        protected virtual bool CanPush(Card card) => true;

        /// <summary>
        /// Called when a <see cref="Card"/> is pushed onto this <see cref="CardPile"/>.
        /// </summary>
        /// <param name="newCard"></param>
        protected virtual void OnPushed(Card newCard) { }

        /// <summary>
        /// Called when a <see cref="Card"/> is popped from this <see cref="CardPile"/>.
        /// </summary>
        /// <param name="removedCard"></param>
        protected virtual void OnPopped(Card removedCard) { }

        /// <summary>
        /// Pops a <see cref="Card"/> from this <see cref="CardPile"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="Card"/> on top of the <see cref="CardPile"/>.
        /// If the <see cref="CardPile"/> is empty, a value of <value>null</value> is returned.
        /// </returns>
        public Card Pop()
        {
            if (topIndex >= 0)
            {
                Card card = data[topIndex--];
                OnPopped(card);

                return card;
            }

            Logger.LogFunctionEntry(string.Empty, "Attempted to pop card from empty card pile.", LoggerVerbosity.Warning);
            return null;
        }

        /// <summary>
        /// Retrieves the top <see cref="Card"/> from this <see cref="CardPile"/> without popping it.
        /// </summary>
        /// <returns>
        /// The <see cref="Card"/> on top of the <see cref="CardPile"/>.
        /// If the <see cref="CardPile"/> is empty, a value of <value>null</value> is returned.
        /// </returns>
        public Card Peek() => topIndex < 0 ? null : data[topIndex];

        /// <summary>
        /// Retrieve the <see cref="IEnumerator{T}"/> for this <see cref="CardPile"/> which iterates over the stored <see cref="Card"/> collection.
        /// </summary>
        public IEnumerator<Card> GetEnumerator() => data.Take(topIndex).GetEnumerator();

        /// <summary>
        /// Retrieve the <see cref="IEnumerator{T}"/> for this <see cref="Deck"/> which iterates over the stored <see cref="Card"/> collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
