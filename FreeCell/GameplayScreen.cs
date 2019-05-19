/*
 * Author: Shon Verch
 * File Name: GameplayScreen.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/19/2019
 * Description: The game screen containing all the gameplay.
 */

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities;
using MonoGameUtilities.Logging;
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
        /// Information about the current selection.
        /// <remarks>
        /// A <value>null</value> value indicates no selection.
        /// </remarks>
        /// </summary>
        public CardSelectionInformation CurrentSelection { get; private set; }

        /// <summary>
        /// The texture of the table (background).
        /// </summary>
        private Texture2D tableTexture;

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
            freeCellTexture = MainGame.Context.Content.Load<Texture2D>("Cards/freeCell");

            CardSuit[] cardSuits = Enum.GetValues(typeof(CardSuit)).Cast<CardSuit>().ToArray();
            foundationPiles = new FoundationPile[cardSuits.Length];

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
            InitializeTableauPiles();

            // Populate the tableau piles
            for (int i = 0; i < deck.Count; i++)
            {
                tableauPiles[i % tableauPiles.Length].Push(deck[i], true);
            }
        }

        /// <summary>
        /// Initialize the <see cref="TableauPile"/>s.
        /// </summary>
        private void InitializeTableauPiles()
        {
            tableauPiles = new TableauPile[TableauPileCount];

            float tableauPilePositionY = GetTableauPileStartPositionY();
            float cardTextureWidth = Card.GetTexture(CardSuit.Clubs, CardRank.Ace).Width;

            // The width of all the tableau piles (including spacing).
            float fullWidth = cardTextureWidth * tableauPiles.Length + TableauPileHorizontalSpacing * (tableauPiles.Length - 1);
            // The amount of pixels to offset each tableau pixel such that they are horizontally centered.
            float centreOffsetX = 0.5f * (MainGame.GameScreenWidth - fullWidth);

            for (int i = 0; i < tableauPiles.Length; i++)
            {
                float positionX = centreOffsetX + i * (cardTextureWidth + TableauPileHorizontalSpacing);

                // The height of the tableau pile, in pixels. "Window" height is so called since it
                // refers to the area available for the cards.
                float height = MainGame.GameScreenHeight - tableauPilePositionY - TableauPileBottomPadding;
                tableauPiles[i] = new TableauPile(new RectangleF(positionX, tableauPilePositionY, cardTextureWidth, height));
            }
        }

        /// <summary>
        /// Update the gameplay.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (HandleCardMovement()) return;
            HandleCardSelection();
        }

        /// <summary>
        /// Handle the card selection input.
        /// </summary>
        private void HandleCardSelection()
        {
            if (!Input.GetMouseButtonDown(MouseButton.Left)) return;

            // Check if we clicked on a card in a tableau pile
            foreach (TableauPile tableauPile in tableauPiles)
            {
                Card card = tableauPile.GetCardFromPoint(Input.MousePosition);

                // If the card is null, we didn't click on this tableau pile.
                if (card == null) continue;

                CurrentSelection = new CardSelectionInformation(card, tableauPile);
                return;
            }

            // If we have selected a non-top card in a tableau pile, clicking on an empty free cell
            // or foundation pile shouldn't change the selection.
            bool isMiddleTableauCard = CurrentSelection?.CardPile is TableauPile && CurrentSelection.Card != CurrentSelection.CardPile.Peek();

            // Check if we clicked on a free cell
            foreach (FreeCell freeCell in freeCells)
            {
                if (!freeCell.Rectangle.Contains(Input.MousePosition)) continue;
                if (isMiddleTableauCard && freeCell.Empty) return;
                if (!freeCell.Empty)
                {
                    CurrentSelection = new CardSelectionInformation(freeCell.Value, freeCell);
                }

                return;
            }
            
            // At this point, we are just clicking on an empty space.
            CurrentSelection = null;
        }

        /// <summary>
        /// Handle card movement.
        /// </summary>
        /// <returns>A boolean indicating whether a card was successfully moved.</returns>
        private bool HandleCardMovement()
        {
            // If we aren't selecting anything, there is no card to move.
            if (!Input.GetMouseButtonDown(MouseButton.Left) || CurrentSelection == null) return false;

            // Determine whether we clicked on a free cell
            foreach (FreeCell freeCell in freeCells)
            {
                if (!freeCell.Rectangle.Contains(Input.MousePosition)) continue;

                // If we are trying to move a card from the tableau pile to a free cell,
                // we need to make sure that the card is on the top of the tableau pile.
                if (CurrentSelection.CardPile is TableauPile &&
                    CurrentSelection.Card != CurrentSelection.CardPile.Peek()) continue;

                // If the free cell is empty, we can move the card onto it;
                // otherwise, select the free cell as this was an invalid move.
                if (freeCell.Empty)
                {
                    Card card = CurrentSelection.CardPile.Pop();
                    freeCell.Push(card);

                    CurrentSelection = null;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Draw the gameplay.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Draw the background
            spriteBatch.Draw(tableTexture, Vector2.Zero, Color.White);

            // Draw the free cell, foundation piles, and tableau piles.
            Array.ForEach(freeCells, freeCell => freeCell.Draw(spriteBatch));
            Array.ForEach(foundationPiles, foundationPile => foundationPile.Draw(spriteBatch));
            Array.ForEach(tableauPiles, tableauPile => tableauPile.Draw(spriteBatch));
        }

        /// <summary>
        /// The starting vertical position, in pixels, of the <see cref="TableauPile"/>s.
        /// </summary>
        private float GetTableauPileStartPositionY() => PileGroupPositionY + freeCellTexture.Height + TableauPileVerticalPadding;
    }
}
