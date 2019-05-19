﻿/*
 * Author: Shon Verch
 * File Name: TableauPile.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/19/2019
 * Description: The tableau card pile.
 */

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities;

namespace FreeCell
{
    /// <summary>
    /// The tableau <see cref="CardPile"/>.
    /// </summary>
    public class TableauPile : CardPile
    {
        /// <summary>
        /// The percent of the <see cref="Card"/> texture height before
        /// another <see cref="Card"/> overlaps in the <see cref="TableauPile"/>.
        /// </summary>
        public const float PercentPixelVisibility = 1 / 3f;

        /// <summary>
        /// The number of cards to maximally compress in a <see cref="TableauPile"/>.
        /// </summary>
        public const int CardCompressionGroupSize = 4;

        /// <summary>
        /// The percent factor by which the <see cref="Card"/> compress in on one another in the <see cref="TableauPile"/>.
        /// </summary>
        public const float PercentCardCompressionFactor = 0.02f;

        /// <summary>
        /// Initializes a new <see cref="TableauPile"/>.
        /// <remarks>
        /// A <see cref="TableauPile"/> has a maximum size of 19; however, it is very rare
        /// for the full tableau pile to become full.
        /// </remarks>
        /// </summary>
        public TableauPile(RectangleF rectangle) : base(19, rectangle) { }

        /// <summary>
        /// Indicates whether the specified <see cref="Card"/> can be pushed on this <see cref="TableauPile"/>.
        /// </summary>
        /// <param name="card">The <see cref="Card"/> to push onto this <see cref="TableauPile"/>.</param>
        /// <returns>A boolean value indicating whether the <see cref="Card"/> can be pushed.</returns>
        protected override bool CanPush(Card card)
        {
            // Any card can be pushed on the tableau pile if it is empty.
            if (Empty) return true;

            Card top = Peek();

            // A card can be moved onto the tableau pile if it is the opposite colour as 
            // the top card AND it is one less in rank than the top card.
            return !card.IsRed == top.IsRed && (int) top.Rank == (int) card.Rank - 1;
        }

        /// <summary>
        /// Called when a <see cref="Card"/> is a pushed onto this <see cref="TableauPile"/>.
        /// </summary>
        protected override void OnPushed(Card newCard) => CalculateCardRectangles();

        /// <summary>
        /// Called when a <see cref="Card"/> is popped onto this <see cref="TableauPile"/>.
        /// </summary>
        protected override void OnPopped(Card removedCard) => CalculateCardRectangles();

        /// <summary>
        /// Calculate the rectangle information for this <see cref="TableauPile"/>.
        /// </summary>
        private void CalculateCardRectangles()
        {
            float[] cardShifts = GetTableauPileCardLayout();
            float offsetY = 0;

            for (int j = 0; j < Count; j++)
            {
                // Use the reverse index for the card shift since
                // the 0-th element represents the top-most card in the
                // shift array whereas the 0-th element in the card pile
                // is the bottom-most card.
                offsetY += cardShifts[Count - j - 1];

                Card card = this[j];
                Rectangle bounds = card.Texture.Bounds;

                card.Rectangle = new RectangleF(Rectangle.X, Rectangle.Y + offsetY, bounds.Width, bounds.Height);
            }
        }

        /// <summary>
        /// Draw this <see cref="TableauPile"/>.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            GameplayScreen gameplayScreen = MainGame.Context.GameScreenManager.Get<GameplayScreen>();

            for (int j = 0; j < Count; j++)
            {
                Card card = this[j];

                float layerDepth = j / (float) (Count + 100);

                // Determine whether the card should glow.
                bool isSelected = false;
                if (gameplayScreen.CurrentSelection != null)
                {
                    // If this card is currently selected AND it is the top card, make it glow!
                    isSelected = gameplayScreen.CurrentSelection.Card == card && j == Count - 1;
                }
                
                card.Draw(spriteBatch, layerDepth, isSelected, layerDepth + 0.01f);
            }
        }

        /// <summary>
        /// Get the card layout for a <see cref="TableauPile"/>.
        /// </summary>
        /// <param name="focusCardIndex">The index of the <see cref="Card"/> that is focused. Zero is the top of the <see cref="TableauPile"/>.</param>
        /// <returns>
        /// An array of integers where the i-th integer denotes the vertical shift, in pixels, of the i-th card; the 0-th elements
        /// represents the shift for the TOP-MOST card.
        /// </returns>
        private float[] GetTableauPileCardLayout(int focusCardIndex = 0)
        {
            // Choose an arbitrary card and g et its texture.
            float cardHeight = Card.GetTexture(CardSuit.Clubs, CardRank.Ace).Height;
            float pixelVisibility = cardHeight * PercentPixelVisibility;

            // The number of cards (including the first one) that can be fitted into the area without shrinking
            // the pixel visibility for a group of cards.
            int minimumCards = (int)Math.Ceiling((Rectangle.Height - cardHeight) / pixelVisibility);
            // The minimum pixel visibility when trying to fit all the cards in the tableau pile.
            float minimumPixelVisibility = (Rectangle.Height - cardHeight) / (MaximumSize - 1);

            int excess = Count - minimumCards;
            float compressionFactor = cardHeight * PercentCardCompressionFactor;
            float compressionVisibility = Math.Max(pixelVisibility - excess * compressionFactor, minimumPixelVisibility);

            // Distribute the leftover space among the non-compressed cards.
            float leftoverVisibility = (Rectangle.Height - cardHeight - compressionVisibility * CardCompressionGroupSize) / (Count - CardCompressionGroupSize - 1);

            float[] allocations = new float[Count];
            for (int i = 0; i < Count; i++)
            {
                // The bottom card has no shift since there is no card after it.
                if (i == Count - 1) continue;
                if (Count <= minimumCards)
                {
                    allocations[i] = pixelVisibility;
                }
                else
                {
                    allocations[i] = Count - 1 <= CardCompressionGroupSize ? compressionVisibility : leftoverVisibility;
                }
            }

            return allocations;
        }

        /// <summary>
        /// Gets the <see cref="Card"/> under the specified <paramref name="point"/>.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>
        /// The <see cref="Card"/> under the <paramref name="point"/> or <value>null</value> if the
        /// none of the cards in this <see cref="TableauPile"/> contain the <paramref name="point"/>.
        /// </returns>
        public Card GetCardFromPoint(Vector2 point)
        {
            if (!Rectangle.Contains(point)) return null;

            // Iterate through the cards from top to bottom
            // so that we don't select a card that is behind another.
            for (int i = Count - 1; i >= 0; i--)
            {
                Card card = this[i];
                if (!card.Rectangle.HasValue || !card.Rectangle.Value.Contains(point)) continue;
                return card;
            }

            return null;
        }
    }
}
