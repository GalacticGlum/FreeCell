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
        /// A collection of all the <see cref="FoundationPile"/> textures mapped by <see cref="CardSuit"/>s.
        /// </summary>
        private Dictionary<CardSuit, Texture2D> foundationPileTextures;

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

            tableTexture = MainGame.Context.Content.Load<Texture2D>("Table");

            // Load the card and foundation pile textures
            cardTextures = new Dictionary<Tuple<CardSuit, CardRank>, Texture2D>();
            foundationPileTextures = new Dictionary<CardSuit, Texture2D>();
            freeCellTexture = MainGame.Context.Content.Load<Texture2D>("Cards/freeCell");

            CardSuit[] cardSuits = Enum.GetValues(typeof(CardSuit)).Cast<CardSuit>().ToArray();

            // One foundation pile per suit
            foundationPiles = new FoundationPile[cardSuits.Length]; 

            foreach (CardSuit suit in cardSuits)
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)).Cast<CardRank>())
                {
                    // Load the texture corresponding the (suit, rank) pair.
                    string cardTextureName = Card.GetTextureName(suit, rank);
                    cardTextures[new Tuple<CardSuit, CardRank>(suit, rank)] = MainGame.Context.Content.Load<Texture2D>($"Cards/{cardTextureName}");
                }

                // Resolve the name of the foundation pile texture for the current suit.
                string foundationPileTextureName = $"Cards/foundation{Card.CardSuitIdentifier[suit]}";
                foundationPileTextures[suit] = MainGame.Context.Content.Load<Texture2D>(foundationPileTextureName);
            }

            // Initialize the foundation piles.
            for (int i = 0; i < cardSuits.Length; i++)
            {
                foundationPiles[i] = new FoundationPile(cardSuits[i]);
            }

            // Initialize the free cells
            freeCells = new FreeCell[FreeCellPileCount];
            for (int i = 0; i < freeCells.Length; i++)
            {
                freeCells[i] = new FreeCell();
            }

            // Initialize the deck and tableau piles
            deck = new Deck();
            deck.Shuffle(GameSeed);

            // Populate the tableau piles
            tableauPiles = new TableauPile[TableauPileCount];
            for (int i = 0; i < tableauPiles.Length; i++)
            {
                tableauPiles[i] = new TableauPile();
            }

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
            for (int i = 0; i < freeCells.Length; i++)
            {
                // Free cells are placed from the left of the screen
                float positionX = PileGroupHorizontalPadding + i * (freeCellTexture.Width + PileHorizontalSpacing);
                spriteBatch.Draw(freeCellTexture, new Vector2(positionX, PileGroupPositionY), Color.White);
            }
        }

        /// <summary>
        /// Draw the <see cref="FoundationPile"/>s.
        /// </summary>
        private void DrawFoundationPiles()
        {
            for (int i = 0; i < foundationPiles.Length; i++)
            {
                FoundationPile foundationPile = foundationPiles[i];
                Texture2D pileTexture = foundationPileTextures[foundationPile.Suit];

                // We use the reverse index since we are positioning
                // the foundation piles from the right of the screen.
                int reverseIndex = foundationPiles.Length - i - 1;

                // Foundation piles are placed from the right of the screen.
                // We find the total space from the current card to the final card and then apply the horizontal padding to the right side.
                float positionX = MainGame.GameScreenWidth - (pileTexture.Width * (reverseIndex + 1) + PileHorizontalSpacing * reverseIndex + PileGroupHorizontalPadding);
                spriteBatch.Draw(pileTexture, new Vector2(positionX, PileGroupPositionY), Color.White);
            }
        }

        /// <summary>
        /// Draw the <see cref="TableauPile"/>s.
        /// </summary>
        private void DrawTableauPiles()
        {
            float positionY = PileGroupPositionY + freeCellTexture.Height + TableauPileVerticalPadding;

            // Choose an arbitrary card texture and use that to get the width of the tableau pile (plus spacing).
            float cardTextureWidth = cardTextures[new Tuple<CardSuit, CardRank>(CardSuit.Clubs, CardRank.Ace)].Width;
            // The width of all the tableau piles (including spacing).
            float fullWidth = cardTextureWidth * tableauPiles.Length + TableauPileHorizontalSpacing * (tableauPiles.Length - 1);
            // The amount of pixels to offset each tableau pixel such that they are horizontally centered.
            float centreOffsetX = 0.5f * (MainGame.GameScreenWidth - fullWidth);

            for (int i = 0; i < tableauPiles.Length; i++)
            {
                TableauPile tableauPile = tableauPiles[i];
                float positionX = centreOffsetX + i * (cardTextureWidth + TableauPileHorizontalSpacing);

                spriteBatch.Draw(cardTextures[new Tuple<CardSuit, CardRank>(CardSuit.Clubs, CardRank.Five)], new Vector2(positionX, positionY), Color.White);
            }
        }
    }
}
