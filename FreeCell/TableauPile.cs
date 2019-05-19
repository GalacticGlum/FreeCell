/*
 * Author: Shon Verch
 * File Name: TableauPile.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/19/2019
 * Description: The tableau card pile.
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities;
using MonoGameUtilities.Logging;

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
        public const float PercentCardCompressionFactor = 0.05f;

        /// <summary>
        /// The selection portion of cards.
        /// </summary>
        public HashSet<Card> SelectedPortion { get; private set; }

        /// <summary>
        /// The height, in pixels, of the tableau area available to cards.
        /// </summary>
        private readonly float fullAreaHeight;

        /// <summary>
        /// Initializes a new <see cref="TableauPile"/>.
        /// <remarks>
        /// A <see cref="TableauPile"/> has a maximum size of 19; however, it is very rare
        /// for the full tableau pile to become full.
        /// </remarks>
        /// </summary>
        public TableauPile(RectangleF rectangle, float areaHeight) : base(19, rectangle)
        {
            fullAreaHeight = areaHeight;
        }

        /// <summary>
        /// Indicates whether the specified <see cref="Card"/> can be pushed on this <see cref="TableauPile"/>.
        /// </summary>
        /// <param name="card">The <see cref="Card"/> to push onto this <see cref="TableauPile"/>.</param>
        /// <returns>A boolean value indicating whether the <see cref="Card"/> can be pushed.</returns>
        public override bool CanPush(Card card)
        {
            // Any card can be pushed on the tableau pile if it is empty.
            if (Empty) return true;

            Card top = Peek();

            // A card can be moved onto the tableau pile if it is the opposite colour as 
            // the top card AND it is one less in rank than the top card.
            return card.IsRed != top.IsRed && (int) top.Rank - 1 == (int) card.Rank;
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
        /// Update this <see cref="TableauPile"/> after the selection has changed.
        /// </summary>
        /// <param name="isSelected">A boolean indicating whether this <see cref="TableauPile"/> is currently selected.</param>
        public void UpdateSelection(bool isSelected)
        {
            CalculateCardRectangles();

            if (isSelected)
            {
                GameplayScreen gameplayScreen = MainGame.Context.GameScreenManager.Get<GameplayScreen>();
                SelectedPortion = FindPortion(gameplayScreen.CurrentSelection.Card);
            }
            else
            {
                SelectedPortion = null;
            }
        }

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

            float height = fullAreaHeight;
            if (Count > 0)
            {
                RectangleF? lastCardRectangle = this[Count - 1].Rectangle;
                if (lastCardRectangle.HasValue)
                {
                    // Recalculate the tableau height
                    height = lastCardRectangle.Value.Bottom - Rectangle.Y;
                }
                else
                {
                    // If the rectangle of the final card is null, something went terribly wrong!
                    // (especially since we set the rectangles of the cards in the loop above).
                    // Use the full area height for the calculation instead and log this occurence.

                    Logger.LogFunctionEntry(string.Empty, "Rectangle of last card is null!", LoggerVerbosity.Warning);
                }
            }
            else
            {
                // Choose an arbitrary card and get its height.
                height = Card.GetTexture(CardSuit.Clubs, CardRank.Ace).Height;
            }

            Rectangle = new RectangleF(Rectangle.Position, new Vector2(Rectangle.Width, height));
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
                    isSelected = gameplayScreen.CurrentSelection.Card == card;
                }

                bool isPileSelected = gameplayScreen.CurrentSelection?.CardPile == this;
                bool isPortionCard = SelectedPortion != null && SelectedPortion.Contains(card) && isPileSelected;

                // If this card is currently selected AND it is the top card, make it glow!
                // bool isGlowing = isSelected && j == Count - 1;

                // If this card is part of a portion that extends to the top of the pile, make it glow!
                // (i.e. if the portion contains the top card, it extends to the top. This is guaranteed to work
                // since a portion is a continuous subset of the pile.)
                bool isGlowing = isPortionCard && SelectedPortion.Contains(Peek());

                // The card is grayed out if it is not selected BUT this tableau pile IS selected AND it isn't part of a portion.
                bool isGrayedOut = isPileSelected && !isPortionCard;

                card.Draw(spriteBatch, layerDepth, isGlowing, layerDepth + 0.01f, isGrayedOut);
            }
        }

        /// <summary>
        /// Find a portion in this <see cref="TableauPile"/> containing the specified <paramref name="card"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="HashSet{T}"/> of <see cref="Card"/>s that make up the portion. If no such portion
        /// of cards exists, the set simply contains the specified card.
        /// </returns>
        public HashSet<Card> FindPortion(Card card)
        {
            int index = GetIndexOf(card);

            // Could not find the card in the tableau pile
            if (index == -1) return null;

            HashSet<Card> portion = new HashSet<Card>
            {
                card
            };

            // Search down the pile (towards the bottom) for any cards
            for (int i = index - 1; i >= 0; i--)
            {
                Card current = this[i];
                Card previous = this[i + 1];

                if (current.IsRed == previous.IsRed || (int) current.Rank - 1 != (int) previous.Rank) break;
                portion.Add(current);
            }        

            // Search up the pile (towards the top) for any cards
            for (int i = index + 1; i < Count; i++)
            {
                Card current = this[i];
                Card previous = this[i - 1];

                if (current.IsRed == previous.IsRed || (int)current.Rank + 1 != (int)previous.Rank) break;
                portion.Add(current);
            }
            

            return portion;
        }
        /// <summary>
        /// Get the card layout for a <see cref="TableauPile"/>.
        /// </summary>
        /// <returns>
        /// An array of integers where the i-th integer denotes the vertical shift, in pixels, of the i-th card; the 0-th elements
        /// represents the shift for the TOP-MOST card.
        /// </returns>
        private float[] GetTableauPileCardLayout()
        {
            // Choose an arbitrary card and get its texture.
            float cardHeight = Card.GetTexture(CardSuit.Clubs, CardRank.Ace).Height;
            float pixelVisibility = cardHeight * PercentPixelVisibility;

            // The number of cards (including the first one) that can be fitted into the area without shrinking
            // the pixel visibility for a group of cards.
            int minimumCards = (int)Math.Ceiling((fullAreaHeight - cardHeight) / pixelVisibility);
            // The minimum pixel visibility when trying to fit all the cards in the tableau pile.
            float minimumPixelVisibility = (fullAreaHeight - cardHeight) / (MaximumSize - 1);

            int excess = Count - minimumCards;
            float compressionFactor = cardHeight * PercentCardCompressionFactor;

            // Check if one of the cards in this tableau pile IN the compressed group is selected
            GameplayScreen gameplayScreen = MainGame.Context.GameScreenManager.Get<GameplayScreen>();
            bool isCompressedSelected = false;
            if (gameplayScreen.CurrentSelection != null)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (gameplayScreen.CurrentSelection.Card != this[i]) continue;

                    isCompressedSelected = i < CardCompressionGroupSize - 1;
                    break;
                }
            }
                
            float compressionVisibility = isCompressedSelected ? pixelVisibility : Math.Max(pixelVisibility - excess * compressionFactor, minimumPixelVisibility);

            // Distribute the leftover space among the non-compressed cards.
            float leftoverVisibility = (fullAreaHeight - cardHeight - compressionVisibility * CardCompressionGroupSize) / (Count - CardCompressionGroupSize - 1);

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
                    allocations[i] = Count - i <= CardCompressionGroupSize ? compressionVisibility : leftoverVisibility;
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
