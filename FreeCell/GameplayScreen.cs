/*
 * Author: Shon Verch
 * File Name: GameplayScreen.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/19/2019
 * Description: The game screen containing all the gameplay.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUtilities;
using MonoGameUtilities.Logging;
using Microsoft.Xna.Framework.Audio;
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
        public const float TopUIBarHeight = MainGame.GameScreenHeight * 0.08f;

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
        public const float PileGroupPositionY = TopUIBarHeight + PileGroupVerticalPadding;

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
        /// The horizontal padding, in pixels, between text and the edge of the screen.
        /// </summary>
        public const int TextScreenPadding = 20;

        /// <summary>
        /// The vertical padding, in pixels, for a UI element.
        /// </summary>
        private const int UIElementPadding = 10;

        /// <summary>
        /// The colour of the top ui bar.
        /// </summary>
        private static readonly Color UIBarColour = new Color(0, 0, 0, 0.4f);

        /// <summary>
        /// The colour when something is hovered over.
        /// </summary>
        private static readonly Color ButtonHoverColour = new Color(255, 251, 153);

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

        /// <summary>
        /// The header font (large font).
        /// </summary>
        private SpriteFont headerFont;

        /// <summary>
        /// The button font (smaller font).
        /// </summary>
        private SpriteFont buttonFont;

        /// <summary>
        /// The deck of cards.
        /// </summary>
        private Deck deck;

        /// <summary>
        /// The free cell piles.
        /// </summary>
        private FreeCell[] freeCells;

        /// <summary>
        /// The foundation piles.
        /// </summary>
        private FoundationPile[] foundationPiles;

        /// <summary>
        /// The tableau piles.
        /// </summary>
        private TableauPile[] tableauPiles;

        /// <summary>
        /// The new game button in the top bar.
        /// </summary>
        private TextButton newGameButton;

        /// <summary>
        /// The "play" game button in the modal window.
        /// </summary>
        private TextButton newGameConfirmButton;

        /// <summary>
        /// The randomize seed button in the modal window.
        /// </summary>
        private TextButton randomizeSeedButton;

        /// <summary>
        /// The seed textbox in the modal window.
        /// </summary>
        private Textbox gameSeedTextbox;
        
        /// <summary>
        /// A boolean indicating whether the modal window is currently active.
        /// </summary>
        private bool isNewGameModalActive;

        /// <summary>
        /// The sound effect that is played when a card is moved.
        /// </summary>
        private SoundEffect cardMoveSoundEffect;

        /// <summary>
        /// The primary selection sound effect.
        /// </summary>
        private SoundEffect cardSelectPrimarySoundEffect;

        /// <summary>
        /// The secondary selection sound effect.
        /// </summary>
        private SoundEffect cardSelectSecondarySoundEffect;

        /// <summary>
        /// The foundation pile card added sound effect.
        /// </summary>
        private SoundEffect foundationPileAddSoundEffect;

        /// <summary>
        /// The elapsed time, in seconds, since the start of the game.
        /// </summary>
        private float gameElapsedTime;

        /// <summary>
        /// A boolean indicating whether the game is complete.
        /// (i.e. all foundation piles are full).
        /// </summary>
        private bool isGameComplete;

        /// <summary>
        /// Information about the current card movement animation.
        /// <remarks>
        /// A <value>null</value> value means that no card is being animated right now.
        /// </remarks>
        /// </summary>
        private readonly List<CardMovementAnimation> cardMovementAnimations = new List<CardMovementAnimation>();

        /// <summary>
        /// Initializes a new <see cref="GameplayScreen"/>.
        /// </summary>
        public GameplayScreen() : this(GenerateSeed()) { }


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

            // Load content
            tableTexture = MainGame.Context.Content.Load<Texture2D>("Table");
            freeCellTexture = MainGame.Context.Content.Load<Texture2D>("Cards/freeCell");
            headerFont = MainGame.Context.Content.Load<SpriteFont>("Fonts/Arial_24");
            buttonFont = MainGame.Context.Content.Load<SpriteFont>("Fonts/Arial_18");

            // Load sound effects
            cardMoveSoundEffect = MainGame.Context.Content.Load<SoundEffect>("Audio/card_move");
            cardSelectPrimarySoundEffect = MainGame.Context.Content.Load<SoundEffect>("Audio/card_primary_select");
            cardSelectSecondarySoundEffect = MainGame.Context.Content.Load<SoundEffect>("Audio/card_secondary_select");
            foundationPileAddSoundEffect = MainGame.Context.Content.Load<SoundEffect>("Audio/foundation_add");

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

            // Update each tableau pile rectangle.
            foreach (TableauPile tableauPile in tableauPiles)
            {
                tableauPile.UpdateRectangles();
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
            // The amount of pixels to offset each tableau pile such that they are horizontally centered.
            float centreOffsetX = 0.5f * (MainGame.GameScreenWidth - fullWidth);

            for (int i = 0; i < tableauPiles.Length; i++)
            {
                float positionX = centreOffsetX + i * (cardTextureWidth + TableauPileHorizontalSpacing);

                // The height of the tableau card area, in pixels.
                float height = MainGame.GameScreenHeight - tableauPilePositionY - TableauPileBottomPadding;
                tableauPiles[i] = new TableauPile(new RectangleF(positionX, tableauPilePositionY, cardTextureWidth, height), height);
            }
        }

        /// <summary>
        /// Update the gameplay.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            UpdateUI(deltaTime);
            UpdateAnimations(deltaTime);

            isGameComplete = foundationPiles.All(foundationPile => foundationPile.Count == foundationPile.MaximumSize);

            // Halt gameplay logic while the modal is active
            if (isNewGameModalActive) return;

            if (!isGameComplete)
            {
                gameElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // If a card movement animation is currently active, disable card selection
            if (cardMovementAnimations.Count > 0) return;

            if (HandleCardMovement())
            {
                // Since we successfully moved a card, let's play the card movement sound effect
                cardMoveSoundEffect.Play();
                return;
            }

            HandleCardSelection();

            // We call this after handling card movement and selection since this relies on double clicking 
            // and it is possible for the user to register two clicks when they quickly switch selections;
            // in this case, we don't want to move the card to the foundation pile so we need our selection
            // to be updated beforehand.
            HandleMoveToFoundationPile();
        }

        /// <summary>
        /// Updates the animations.
        /// </summary>
        private void UpdateAnimations(float deltaTime)
        {
            // Iterate from the back so that we can remove the animations
            // that have finished.
            for (int i = cardMovementAnimations.Count - 1; i >= 0; i--)
            {
                cardMovementAnimations[i].Update(deltaTime);
                if (cardMovementAnimations[i].Finished)
                {
                    cardMovementAnimations.RemoveAt(i);
                }
            }

        }

        /// <summary>
        /// Handle the card selection input.
        /// </summary>
        private void HandleCardSelection()
        {
            if (!Input.GetMouseButtonDown(MouseButton.Left)) return;

            // A card is a "middle tableau" if it is not the top card.
            bool isMiddleTableauCardSelected = false;
            bool isMiddleTableauPortionSelected = false;

            TableauPile selectedTableauPile = null;
            if (CurrentSelection?.CardPile is TableauPile)
            {
                selectedTableauPile = (TableauPile) CurrentSelection.CardPile;

                isMiddleTableauCardSelected = CurrentSelection.Card != CurrentSelection.CardPile.Peek();
                isMiddleTableauPortionSelected = selectedTableauPile.SelectedPortion.Contains(CurrentSelection.Card) && 
                                                 !selectedTableauPile.SelectedPortion.Contains(selectedTableauPile.Peek());
            }

            // Check if we clicked on a card in a tableau pile
            foreach (TableauPile tableauPile in tableauPiles)
            {
                Card card = tableauPile.GetCardFromPoint(Input.MousePosition);

                // If the card is null, we didn't click on this tableau pile.
                if (card == null) continue;

                bool selectedPortionCard = false;
                if (selectedTableauPile != null)
                {
                    selectedPortionCard = selectedTableauPile.SelectedPortion.Contains(card);
                }

                // If we have a "middle tableau" card selected and we clicked on it again, let's deselect it.
                // OR if our "middle" tableau card is in a portion and we select another card in that portion
                // (i.e. we treat portions in the middle of a tableau pile as one card in terms of selection).
                if (isMiddleTableauCardSelected && card == CurrentSelection.Card || isMiddleTableauPortionSelected && selectedPortionCard)
                {
                    CurrentSelection = null;
                    return;
                }

   
                // If we were previously selecting a tableau pile, cache it so we can update it.
                TableauPile oldTableauPile = null;
                if (CurrentSelection?.CardPile is TableauPile pile)
                {
                    oldTableauPile = pile;
                }

                // Store our previous selection state so we can play the correct sound effect.
                CardSelectionInformation previousSelection = CurrentSelection;
                CurrentSelection = new CardSelectionInformation(card, tableauPile);
                
                oldTableauPile?.UpdateSelection(tableauPile == oldTableauPile);
                tableauPile.UpdateSelection(true);

                // Play the primary sound effect if we selected the top card AND if this is our first card selection
                if (tableauPile.Peek() == card && previousSelection == null)
                {
                    cardSelectPrimarySoundEffect.Play();
                }
                else
                {
                    cardSelectSecondarySoundEffect.Play();
                }

                return;
            }

            // Check if we clicked on a free cell
            foreach (FreeCell freeCell in freeCells)
            {
                if (!freeCell.Contains(Input.MousePosition)) continue;

                // Store our previous selection state so we can play the correct sound effect.
                CardSelectionInformation previousSelection = CurrentSelection;

                // If we have selected a non-top card in a tableau pile, clicking on an empty free cell shouldn't change the selection.
                if (isMiddleTableauCardSelected && freeCell.Empty) return;
                if (freeCell.Empty) return;

                CurrentSelection = new CardSelectionInformation(freeCell.Value, freeCell);

                // Play if this is our first card selection
                if (previousSelection == null)
                {
                    cardSelectPrimarySoundEffect.Play();
                }
                else
                {
                    cardSelectSecondarySoundEffect.Play();
                }

                return;
            }

            // If we have selected a non-top card in a tableau pile, clicking on a foundation pile shouldn't change the selection.
            bool isSelectingFoundationPile = foundationPiles.Any(foundationPile => foundationPile.Contains(Input.MousePosition));
            if (isSelectingFoundationPile && isMiddleTableauCardSelected)
            {
                return;
            }
            
            // At this point, we are just clicking on an empty space.
            CurrentSelection = null;
            selectedTableauPile?.UpdateSelection(false);
        }

        /// <summary>
        /// Handle card movement.
        /// </summary>
        /// <returns>A boolean indicating whether a card was successfully moved.</returns>
        private bool HandleCardMovement()
        {
            // If we aren't selecting anything, there is no card to move.
            if (!Input.GetMouseButtonDown(MouseButton.Left) || CurrentSelection == null) return false;

            if (freeCells.Any(TryMoveCard) || foundationPiles.Any(TryMoveCard)) return true;
            return HandleTableuPileCardMovement();
        }

        /// <summary>
        /// Handle card movement among tableau piles.
        /// </summary>
        private bool HandleTableuPileCardMovement()
        {
            // If we are moving from tableau pile to tableau pile, treat all movements as
            // moving a portion onto another tableau pile; otherwise, move the top card of the
            // selected pile to the tableau pile.
            if (CurrentSelection?.CardPile is TableauPile selectedTableauPile)
            {
                if (selectedTableauPile.SelectedPortion == null)
                {
                    // If our selection portion is null despite the fact that 
                    // this tableau pile is selected, something has gone terribly
                    // wrong so we should log this occurence.

                    Logger.LogFunctionEntry(string.Empty, "NULL portion in selected tableau pile!",
                        LoggerVerbosity.Warning);
                    return false;
                }

                int maximumPortionLength = freeCells.Count(freeCell => freeCell.Empty) + 1;
                foreach (TableauPile tableauPile in tableauPiles)
                {
                    if (!tableauPile.Contains(Input.MousePosition) || tableauPile == selectedTableauPile) continue;

                    // If we have more cards than we can move, take the most that we can.
                    int transferAmount = Math.Min(selectedTableauPile.SelectedPortion.Count, maximumPortionLength);
                    bool canTransfer = false;

                    // Check whether a part of the portion pile can be moved
                    // For example, suppose that the portion consisted of 4 and 3 and
                    // the user tried to move the pile onto a pile with a top card of 4.
                    // The intended result is that only the 3 is moved.
                    for (int i = selectedTableauPile.Count - transferAmount; i < selectedTableauPile.Count; i++)
                    {
                        if (!tableauPile.CanPush(selectedTableauPile[i])) continue;

                        // Recalculate the transfer amount (subtract by the amount of times we traversed down the pile).
                        transferAmount = selectedTableauPile.Count - i;
                        canTransfer = true;

                        break;
                    }

                    if (!canTransfer) return false;

                    // A temporary "stack" buffer to store the cards as we pop them from the tableau pile
                    Card[] buffer = new Card[transferAmount];
                    for (int i = 0; i < transferAmount; i++)
                    {
                        buffer[i] = selectedTableauPile.Pop();
                    }

                    // Push the cards onto the new tableau pile.
                    for (int i = transferAmount - 1; i >= 0; i--)
                    {
                        tableauPile.Push(buffer[i]);
                    }

                    CurrentSelection = null;
                    return true;
                }
            }
            else
            {
                return tableauPiles.Any(TryMoveCard);
            }

            return false;
        }

        /// <summary>
        /// Attempts to move a card onto the <paramref name="pile"/>.
        /// </summary>
        private bool TryMoveCard(CardPile pile) => TryMoveCard(pile, false);

        /// <summary>
        /// Attempts to move a card onto the <paramref name="pile"/>.
        /// </summary>
        /// <param name="pile">The <see cref="CardPile"/> to move the current selection to.</param>
        /// <param name="ignoreMousePosition">Ignores whether the mouse is over the <paramref name="pile"/>.</param>
        private bool TryMoveCard(CardPile pile, bool ignoreMousePosition)
        {
            if (CurrentSelection == null || !pile.Contains(Input.MousePosition) && !ignoreMousePosition) return false;

            // If we are trying to move a card from the tableau pile to
            // another pile, we need to make sure that it is the top card
            // that we are trying to move.
            if (CurrentSelection.CardPile is TableauPile &&
                CurrentSelection.Card != CurrentSelection.CardPile.Peek()) return false;

            if (!pile.CanPush(CurrentSelection.Card)) return false;

            // If we can move the card onto the pile, move it and clear the selection
            Card card = CurrentSelection.CardPile.Pop();
            LerpInformation<Vector2> animationLerp = new LerpInformation<Vector2>(card.Rectangle.GetValueOrDefault().Position,
                pile.GetCardRectangle(card).Position, 0.15f, Vector2.Lerp);

            CardMovementAnimation cardMovementAnimation = new CardMovementAnimation(card, animationLerp, pile);
            cardMovementAnimations.Add(cardMovementAnimation);
            CurrentSelection = null;

            if(pile is FoundationPile)
            {
                foundationPileAddSoundEffect.Play();
            }

            return true;
        }

        /// <summary>
        /// Handle moving the current selection to the foundation pile.
        /// </summary>
        private void HandleMoveToFoundationPile()
        {
            // If we double click on a card, try to put it on the foundation pile.
            if (DoubleClickHelper.HasDoubleClicked(MouseButton.Left) && foundationPiles.Any(pile => TryMoveCard(pile, true)))
            {
                CurrentSelection = null;
            }
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

            cardMovementAnimations.ForEach(animation => animation.Draw(spriteBatch));

            // Draw the UI
            DrawUI();
        }

        /// <summary>
        /// Update the UI.
        /// </summary>
        private void UpdateUI(float deltaTime)
        {
            if (!isNewGameModalActive)
            {
                newGameButton?.Update();
            }

            UpdateNewGameModal(deltaTime);
        }

        /// <summary>
        /// Draw the UI.
        /// </summary>
        private void DrawUI()
        {
            DrawTopBar();
            DrawNewGameModal();
        }

        /// <summary>
        /// Draw the top UI bar.
        /// </summary>
        private void DrawTopBar()
        {
            // spriteBatch.DrawLine applies uniform thickness to both vertical directions; therefore, we must offset the
            // line by half of its thickness (or in our case, by the bar height).
            const float lineY = TopUIBarHeight * 0.5f;

            // Draw the top bar as a line
            spriteBatch.DrawLine(new Vector2(0, lineY), new Vector2(MainGame.GameScreenWidth, lineY), UIBarColour, TopUIBarHeight);

            // Draw the timer
            string timerText = TimingHelper.ElapsedSecondsToTimerString(gameElapsedTime);

            // Horizontally centre the text on the screen and centre it in the bar.
            Vector2 timerStringSize = headerFont.MeasureString(timerText);
            Vector2 timerTextPosition = (new Vector2(MainGame.GameScreenWidth, TopUIBarHeight) - timerStringSize) * 0.5f;

            spriteBatch.DrawString(headerFont, timerText, timerTextPosition, Color.White);

            // Draw the game number
            string gameNumberText = $"Game #{GameSeed}";
            Vector2 gameNumberStringSize = headerFont.MeasureString(gameNumberText);

            // Centre the game number text vertically and align it to the right of the screen
            Vector2 gameNumberTextPosition = new Vector2(MainGame.GameScreenWidth - gameNumberStringSize.X - TextScreenPadding,
                (TopUIBarHeight - gameNumberStringSize.Y) * 0.5f);

            spriteBatch.DrawString(headerFont, gameNumberText, gameNumberTextPosition, Color.White);

            // Draw the new game button
            const string newGameButtonText = "New Game";
            if (newGameButton == null)
            {
                float newGameButtonY = (TopUIBarHeight - headerFont.MeasureString(newGameButtonText).Y) * 0.5f;

                // Initialize the new game button
                newGameButton = new TextButton(new Vector2(0, 0), headerFont, newGameButtonText, new Vector2(TextScreenPadding, newGameButtonY))
                {
                    RegularTextColour = Color.White,
                    HoverTextColour = ButtonHoverColour
                };

                newGameButton.Clicked += OnOpenModal;
            }

            // Draw the button
            newGameButton?.Draw(spriteBatch);
        }

        /// <summary>
        /// Update the new game modal.
        /// </summary>
        private void UpdateNewGameModal(float deltaTime)
        {
            if (!isNewGameModalActive) return;
            if (Input.GetKeyDown(Keys.Escape))
            {
                isNewGameModalActive = false;
            }

            if (Input.GetKeyDown(Keys.Enter))
            {
                OnNewGame();
            }

            gameSeedTextbox?.Update(deltaTime);
            newGameConfirmButton?.Update();
            randomizeSeedButton?.Update();
        }

        /// <summary>
        /// Draw the new game modal.
        /// </summary>
        private void DrawNewGameModal()
        {
            if (!isNewGameModalActive) return;

            // Draw a tint over the whole screen
            const float halfScreenHeight = MainGame.GameScreenHeight * 0.5f;

            spriteBatch.DrawLine(new Vector2(0, halfScreenHeight), new Vector2(MainGame.GameScreenWidth, halfScreenHeight),
                new Color(0, 0, 0, 0.8f), MainGame.GameScreenHeight);

            const string headerText = "Enter Game #";

            float headerTextPositionX = (MainGame.GameScreenWidth - headerFont.MeasureString(headerText).X) * 0.5f;
            float headerTextPositionY = (MainGame.GameScreenHeight - headerFont.LineSpacing) * 0.5f - 100;

            // Initialize the seed textbox
            if (gameSeedTextbox == null)
            {
                gameSeedTextbox = new Textbox(Vector2.Zero, headerFont, 6, GenerateSeed().ToString())
                {
                    Colour = Color.White,
                    Focused = true,
                    NumbersOnly = true
                };

                // Place the textbox below the header.
                float textboxX = (MainGame.GameScreenWidth - gameSeedTextbox.Rectangle.Width) * 0.5f;

                // Add some padding between the header and textbox
                float textboxY = headerTextPositionY + headerFont.LineSpacing + 3 * UIElementPadding;

                gameSeedTextbox.Rectangle.Position = new Vector2(textboxX, textboxY);
            }

            // Initialize the randomize seed button
            if (randomizeSeedButton == null)
            {
                const string randomizeButtonText = "Randomize";
                randomizeSeedButton = new TextButton(Vector2.Zero, buttonFont, randomizeButtonText, new Vector2(0, 0.5f * TextScreenPadding))
                {
                    RegularTextColour = Color.White,
                    HoverTextColour = Color.White,
                    RegularBackgroundColour = new Color(0, 130, 135),
                    HoverBackgroundColour = new Color(33, 146, 151)
                };

                // When the randomize button is clicked, simply update the game seed textbox
                // with a new random seed.
                randomizeSeedButton.Clicked += () => gameSeedTextbox.Text = GenerateSeed().ToString();

                float randomizeButtonY = gameSeedTextbox.Rectangle.Bottom + 2 * UIElementPadding;
                randomizeSeedButton.Rectangle.Position = new Vector2(0, randomizeButtonY);
            }

            // Initialize the play button
            if (newGameConfirmButton == null)
            {
                // Make the play button stretch to the modal size
                float innerPaddingX = 0.5f * (headerFont.MeasureString(headerText).X + UIElementPadding);
                newGameConfirmButton = new TextButton(Vector2.Zero, buttonFont, "Play", new Vector2(innerPaddingX, 0.5f * TextScreenPadding))
                {
                    RegularTextColour = Color.Black,
                    HoverTextColour = Color.Black,
                    RegularBackgroundColour = new Color(203, 203, 203),
                    HoverBackgroundColour = new Color(153, 153, 153)
                };

                float newGameConfirmButtonX = (MainGame.GameScreenWidth - newGameConfirmButton.Rectangle.Width) * 0.5f;
                float newGameConfirmButtonY = randomizeSeedButton.Rectangle.Bottom + 2 * UIElementPadding;

                newGameConfirmButton.Rectangle.Position = new Vector2(newGameConfirmButtonX, newGameConfirmButtonY);
                newGameConfirmButton.Clicked += OnNewGame;
            }

            // Initialize the randomize button inner padding and positioning
            if (randomizeSeedButton.InnerPadding.X == 0)
            {
                float randomizeButtonX = (MainGame.GameScreenWidth - newGameConfirmButton.Rectangle.Width) * 0.5f;
                randomizeSeedButton.Rectangle.Position += new Vector2(randomizeButtonX, 0);

                // Make the randomize button stretch to the play button size
                float innerPaddingX = (newGameConfirmButton.Rectangle.Width - randomizeSeedButton.Rectangle.Width) * 0.5f;
                randomizeSeedButton.InnerPadding = new Vector2(innerPaddingX, randomizeSeedButton.InnerPadding.Y);
            }

            // Draw the modal background
            float modalWidth = newGameConfirmButton.Rectangle.Width + TextScreenPadding * 2;
            float modalHeight = newGameConfirmButton.Rectangle.Bottom - headerTextPositionY + TextScreenPadding * 1.5f;

            float modalX = (MainGame.GameScreenWidth - modalWidth) * 0.5f;
            float modalY = headerTextPositionY - TextScreenPadding * 0.5f;
            
            // Offset by half the height to account for the bidirectional thickness that is applied.
            float modalLineY = modalY + modalHeight * 0.5f;
            spriteBatch.DrawLine(new Vector2(modalX, modalLineY), new Vector2(modalX + modalWidth, modalLineY), new Color(17, 55, 104), modalHeight);

            // Draw the header background
            float headerBackgroundHeight = headerFont.LineSpacing + UIElementPadding * 2;
            float headerLineY = modalY + headerBackgroundHeight * 0.5f;

            spriteBatch.DrawLine(new Vector2(modalX, headerLineY), new Vector2(modalX + modalWidth, headerLineY), new Color(37, 115, 236), headerBackgroundHeight);

            // Draw the modal UI components
            spriteBatch.DrawString(headerFont, headerText, new Vector2(headerTextPositionX, headerTextPositionY), Color.White);

            gameSeedTextbox?.Draw(spriteBatch);
            newGameConfirmButton?.Draw(spriteBatch);
            randomizeSeedButton?.Draw(spriteBatch);
        }

        /// <summary>
        /// Start a new game.
        /// </summary>
        private void OnNewGame()
        {
            if (string.IsNullOrEmpty(gameSeedTextbox.Text)) return;

            int gameSeed = int.Parse(gameSeedTextbox.Text);

            // If the game seed is out of bounds, we won't proceed with starting the new game
            // Instead, focus on the game seed textbox.
            if (gameSeed < Deck.MinimumGameSeed || gameSeed > Deck.MaximumGameSeed)
            {
                gameSeedTextbox.Focused = true;
                return;
            }

            MainGame.Context.GameScreenManager.ReloadScreen<GameplayScreen>(gameSeed);
        }

        /// <summary>
        /// Raised when the <see cref="newGameButton"/> is clicked.
        /// </summary>
        private void OnOpenModal()
        {
            // Open up a modal window that lets you enter a game number/randomly generate one.
            isNewGameModalActive = true;
        }

        /// <summary>
        /// The starting vertical position, in pixels, of the <see cref="TableauPile"/>s.
        /// </summary>
        private float GetTableauPileStartPositionY() => PileGroupPositionY + freeCellTexture.Height + TableauPileVerticalPadding;

        /// <summary>
        /// Generates a random game seed.
        /// </summary>
        /// <returns></returns>
        private static int GenerateSeed() => Random.Range(Deck.MinimumGameSeed, Deck.MaximumGameSeed + 1);
    }
}
