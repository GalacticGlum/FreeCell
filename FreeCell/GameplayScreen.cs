/*
 * Author: Shon Verch
 * File Name: GameplayScreen.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: The game screen containing all the gameplay.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities;
using Random = MonoGameUtilities.Random;

namespace FreeCell
{
    /// <inheritdoc />
    /// <summary>
    /// The <see cref="GameScreen" /> containing all the gameplay.
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        /// <summary>
        /// The vertical padding, in pixels, from the top of the screen which
        /// is allocated for UI elements.
        /// </summary>
        public const float TopVerticalBoundary = MainGame.GameScreenHeight * 0.08f;

        /// <summary>
        /// The horizontal padding, in pixels, before the free cell/foundation piles.
        /// </summary>
        public const float PileGroupHorizontalPadding = MainGame.GameScreenWidth * 0.07f;

        /// <summary>
        /// The vertical padding, in pixels, between the top UI elements and the
        /// free cell/foundation piles.
        /// </summary>
        public const int PileGroupVerticalPadding = 10;

        /// <summary>
        /// The horizontal padding, in pixels, between each free cell/foundation pile.
        /// </summary>
        public const int PileHorizontalSpacing = 25;

        /// <summary>
        /// The vertical position of the pile group.
        /// </summary>
        public const float PileGroupPositionY = TopVerticalBoundary + PileGroupVerticalPadding;

        /// <summary>
        /// The padding, in pixels, between adjacent tableau piles.
        /// </summary>
        public const int TableauPileHorizontalSpacing = 20;

        /// <summary>
        /// The vertical padding, in pixels, between the end of the pile group and the tableau piles.
        /// </summary>
        public const int TableauPileVerticalPadding = 20;

        /// <summary>
        /// The vertical padding, in pixels, between the tableau piles and the bottom of the screen.
        /// </summary>
        public const int TableauPileBottomPadding = 20;

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
        /// The number of <see cref="FreeCell"/> piles.
        /// </summary>
        public const int FreeCellPileCount = 4;

        /// <summary>
        /// The number of <see cref="TableauPile"/>s.
        /// </summary>
        public const int TableauPileCount = 8;

        /// <summary>
        /// The current game seed.
        /// </summary>
        public int GameSeed { get; }

        /// <summary>
        /// The texture of the table (background).
        /// </summary>
        private Texture2D tableTexture;

        /// <summary>
        /// A collection of all the <see cref="Card"/> textures mapped by (<see cref="CardSuit"/>, <see cref="CardRank"/>) pairs.
        /// </summary>
        private Dictionary<Tuple<CardSuit, CardRank>, Texture2D> cardTextures;

        /// <summary>
        /// The texture of a <see cref="FreeCell"/> pile.
        /// </summary>
        private Texture2D freeCellTexture;

        private Deck deck;
        private FreeCell[] freeCells;
        private FoundationPile[] foundationPiles;
        private TableauPile[] tableauPiles;

        /// <summary>
        /// Initializes a new <see cref="GameplayScreen"/>.
        /// </summary>
        public GameplayScreen() : this(Random.Range(Deck.MinimumGameSeed, Deck.MaximumGameSeed + 1)) { }

        /// <summary>
        /// Initializes a new <see cref="GameplayScreen"/> with the specified <paramref name="gameSeed"/>.
        /// </summary>
        /// <param name="gameSeed">
        /// The game seed.
        /// Defaults to an invalid seed (less than the minimum); in this case,
        /// a randomly generated seed is used.
        /// </param>
        public GameplayScreen(int gameSeed)
        {
            GameSeed = gameSeed;
        }

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            base.LoadContent(spriteBatch);

            // Load texture
            tableTexture = MainGame.Context.Content.Load<Texture2D>("Table");
            cardTextures = new Dictionary<Tuple<CardSuit, CardRank>, Texture2D>();
            freeCellTexture = MainGame.Context.Content.Load<Texture2D>("Cards/freeCell");

            CardSuit[] cardSuits = Enum.GetValues(typeof(CardSuit)).Cast<CardSuit>().ToArray();
            foundationPiles = new FoundationPile[cardSuits.Length]; 
            foreach (CardSuit suit in cardSuits)
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)).Cast<CardRank>())
                {
                    // Load the texture corresponding the (suit, rank) pair.
                    string cardTextureName = Card.GetTextureName(suit, rank);
                    cardTextures[new Tuple<CardSuit, CardRank>(suit, rank)] = MainGame.Context.Content.Load<Texture2D>($"Cards/{cardTextureName}");
                }
            }

            // Initialize the foundation piles.
            for (int i = 0; i < cardSuits.Length; i++)
            {
                Texture2D pileTexture = FoundationPile.GetTexture(cardSuits[i]);

                // We use the reverse index since we are positioning
                // the foundation piles from the right of the screen.
                int reverseIndex = foundationPiles.Length - i - 1;

                // Foundation piles are placed from the right of the screen.
                // We find the total space from the current card to the final card and then apply the horizontal padding to the right side.
                float totalSapceFromCurrent = pileTexture.Width * (reverseIndex + 1) + PileHorizontalSpacing * reverseIndex + PileGroupHorizontalPadding;
                float positionX = MainGame.GameScreenWidth - totalSapceFromCurrent;

                foundationPiles[i] = new FoundationPile(cardSuits[i], new RectangleF(positionX, PileGroupPositionY, pileTexture.Width, pileTexture.Height));
            }

            // Initialize the free cells
            freeCells = new FreeCell[FreeCellPileCount];
            for (int i = 0; i < freeCells.Length; i++)
            {
                // Free cells are placed from the left of the screen
                float positionX = PileGroupHorizontalPadding + i * (freeCellTexture.Width + PileHorizontalSpacing);
                freeCells[i] = new FreeCell(new RectangleF(positionX, PileGroupPositionY, freeCellTexture.Width, freeCellTexture.Height));
            }

            // Initialize the deck
            deck = new Deck();
            deck.Shuffle(GameSeed);

            // Initialize the tableau piles
            tableauPiles = new TableauPile[TableauPileCount];
            for (int i = 0; i < tableauPiles.Length; i++)
            {
                tableauPiles[i] = new TableauPile();
            }

            // Populate the tableau piles
            for (int i = 0; i < deck.Count; i++)
            {
                tableauPiles[i % tableauPiles.Length].Push(deck[i], true);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw the background
            spriteBatch.Draw(tableTexture, Vector2.Zero, Color.White);

            DrawFreeCells();
            DrawFoundationPiles();
            DrawTableauPiles();
        }

        /// <summary>
        /// Draw the <see cref="FreeCell"/> piles.
        /// </summary>
        private void DrawFreeCells()
        {
            foreach (FreeCell freeCell in freeCells)
            {
                freeCell.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Draw the <see cref="FoundationPile"/>s.
        /// </summary>
        private void DrawFoundationPiles()
        {
            foreach (FoundationPile foundationPile in foundationPiles)
            {
                foundationPile.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// The starting vertical position, in pixels, of the <see cref="TableauPile"/>s.
        /// </summary>
        private float GetTableauPileStartPositionY() => PileGroupPositionY + freeCellTexture.Height + TableauPileVerticalPadding;

        /// <summary>
        /// The size of a <see cref="Card"/>.
        /// </summary>
        /// <returns>
        /// A two-dimensional vector with the x component represents the width of the <see cref="Card"/>
        /// and the y component represents the height of the <see cref="Card"/>.
        /// </returns>
        private Vector2 GetCardSize()
        {
            // Choose an arbitrary card texture and use that to get the size
            Texture2D cardTexture = cardTextures[new Tuple<CardSuit, CardRank>(CardSuit.Clubs, CardRank.Ace)];
            return new Vector2(cardTexture.Width, cardTexture.Height);
        }

        /// <summary>
        /// Draw the <see cref="TableauPile"/>s.
        /// </summary>
        private void DrawTableauPiles()
        {
            float positionY = GetTableauPileStartPositionY();
            float cardTextureWidth = GetCardSize().X;

            // The width of all the tableau piles (including spacing).
            float fullWidth = cardTextureWidth * tableauPiles.Length + TableauPileHorizontalSpacing * (tableauPiles.Length - 1);
            // The amount of pixels to offset each tableau pixel such that they are horizontally centered.
            float centreOffsetX = 0.5f * (MainGame.GameScreenWidth - fullWidth);

            for (int i = 0; i < tableauPiles.Length; i++)
            {
                TableauPile tableauPile = tableauPiles[i];
                float positionX = centreOffsetX + i * (cardTextureWidth + TableauPileHorizontalSpacing);

                float[] cardShifts = GetTableauPileCardLayout(tableauPile);

                float offsetY = 0;
                for (int j = 0; j < tableauPile.Count; j++)
                {
                    // Use the reverse index for the card shift since
                    // the 0-th element represents the top-most card in the
                    // shift array whereas the 0-th element in the card pile
                    // is the bottom-most card.
                    offsetY += cardShifts[tableauPile.Count - j - 1];

                    Card card = tableauPile[j];
                    spriteBatch.Draw(cardTextures[card.SuitRankTuple], new Vector2(positionX, positionY + offsetY), 
                        null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, j / (float) tableauPile.Count);
                }
            }
        }

        /// <summary>
        /// Get the card layout for a <see cref="TableauPile"/>.
        /// </summary>
        /// <param name="tableauPile">The <see cref="TableauPile"/>.</param>
        /// <param name="focusCardIndex">The index of the <see cref="Card"/> that is focused. Zero is the top of the <see cref="TableauPile"/>.</param>
        /// <returns>
        /// An array of integers where the i-th integer denotes the vertical shift, in pixels, of the i-th card; the 0-th elements
        /// represents the shift for the TOP-MOST card.
        /// </returns>
        private float[] GetTableauPileCardLayout(TableauPile tableauPile, int focusCardIndex = 0)
        {
            // The height of the tableau pile, in pixels. "Window" height is so called since it
            // refers to the area available for the cards.
            float windowHeight = MainGame.GameScreenHeight - GetTableauPileStartPositionY() - TableauPileBottomPadding;
            float cardHeight = GetCardSize().Y;
            float pixelVisibility = cardHeight * PercentPixelVisibility;

            // The number of cards (including the first one) that can be fitted into the area without shrinking
            // the pixel visibility for a group of cards.
            int minimumCards = (int) Math.Ceiling((windowHeight - cardHeight) / pixelVisibility);
            // The minimum pixel visibility when trying to fit all the cards in the tableau pile.
            float minimumPixelVisibility = (windowHeight - cardHeight) / (tableauPile.MaximumSize - 1);

            int excess = tableauPile.Count - minimumCards;
            float compressionFactor = cardHeight * PercentCardCompressionFactor;
            float compressionVisibility = Math.Max(pixelVisibility - excess * compressionFactor, minimumPixelVisibility);

            // Distribute the leftover space among the non-compressed cards.
            float leftoverVisibility = (windowHeight - cardHeight - compressionVisibility * CardCompressionGroupSize) / (tableauPile.Count - CardCompressionGroupSize - 1);

            float[] allocations = new float[tableauPile.Count];
            for (int i = 0; i < tableauPile.Count; i++)
            {
                // The bottom card has no shift since there is no card after it.
                if (i == tableauPile.Count - 1) continue;
                if (tableauPile.Count <= minimumCards)
                {
                    allocations[i] = pixelVisibility;
                }
                else
                {
                    allocations[i] = tableauPile.Count - 1 <= CardCompressionGroupSize ? compressionVisibility : leftoverVisibility;
                }
            }

            return allocations;
        }
    }
}
