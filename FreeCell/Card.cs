/*
 * Author: Shon Verch
 * File Name: Card.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/19/2019
 * Description: A singular playing card.
 */

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities;
using MonoGameUtilities.Logging;

namespace FreeCell
{
    /// <summary>
    /// A singular playing card.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// The identifier which represents the specific card suit when loading the card sprite.
        /// </summary>
        public static readonly Dictionary<CardSuit, string> CardSuitIdentifier = new Dictionary<CardSuit, string>
        {
            {CardSuit.Diamonds, "Diamonds"},
            {CardSuit.Clubs, "Clubs"},
            {CardSuit.Hearts, "Hearts"},
            {CardSuit.Spades, "Spades"}
        };

        /// <summary>
        /// The identifier which represents the specific card value when loading the card sprite.
        /// </summary>
        public static readonly Dictionary<CardRank, string> CardRankIdentifier = new Dictionary<CardRank, string>
        {
            {CardRank.Ace,  "A"},
            {CardRank.Two,  "2"},
            {CardRank.Three,  "3"},
            {CardRank.Four,  "4"},
            {CardRank.Five,  "5"},
            {CardRank.Six,  "6"},
            {CardRank.Seven,  "7"},
            {CardRank.Eight,  "8"},
            {CardRank.Nine,  "9"},
            {CardRank.Ten,  "10"},
            {CardRank.Jack,  "J"},
            {CardRank.Queen,  "Q"},
            {CardRank.King,  "K" }
        };

        /// <summary>
        /// The colour of the selection border glow.
        /// </summary>
        private static readonly Color SelectionBorderColour = new Color(0, 138, 255);

        /// <summary>
        /// The <see cref="CardRank"/> of this <see cref="Card"/>.
        /// </summary>
        public CardRank Rank { get; }

        /// <summary>
        /// The <see cref="CardSuit"/> of this <see cref="Card"/>.
        /// </summary>
        public CardSuit Suit { get; }

        /// <summary>
        /// The bounding <see cref="RectangleF"/> of this <see cref="Card"/>.
        /// </summary>
        public RectangleF? Rectangle { get; set; } = null;

        /// <summary>
        /// A boolean value indicating whether this <see cref="Card"/> is red: <value>true</value> if it is; <value>false</value> otherwise.
        /// </summary>
        public bool IsRed => Suit == CardSuit.Diamonds || Suit == CardSuit.Hearts;

        /// <summary>
        /// Gets the <see cref="Texture2D"/> for this <see cref="Card"/>.
        /// </summary>
        public Texture2D Texture => GetTexture(Suit, Rank);

        private readonly Texture2D glowTexture;

        /// <summary>
        /// Initializes a new <see cref="Card"/> with the specified <see cref="Rank"/> and <see cref="Suit"/>.
        /// </summary>
        /// <param name="rank">The <see cref="CardRank"/> of this <see cref="Card"/>.</param>
        /// <param name="suit">The <see cref="CardSuit"/> of this <see cref="Card"/>.</param>
        public Card(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;

            glowTexture = MainGame.Context.Content.Load<Texture2D>("Cards/cardGlow");
        }

        /// <summary>
        /// Draws this <see cref="Card"/>.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, float layerDepth = 0, bool isGlowing = false, float glowLayerDepth = 0)
        {
            // If the card has a null rectangle, something has gone terribly wrong.
            if (!Rectangle.HasValue)
            {
                Logger.LogFunctionEntry(string.Empty, "Tried to draw card with null rectangle.", LoggerVerbosity.Error);
                return;
            }

            spriteBatch.Draw(Texture, Rectangle.Value.Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layerDepth);

            if (isGlowing)
            {
                // Centre the card inside the glow texture, or conversely, offset
                // the glow texture so that the card is centered inside it.
                float glowOffsetX = (glowTexture.Width - Rectangle.Value.Width) * 0.5f;
                float glowOffsetY = (glowTexture.Height - Rectangle.Value.Height) * 0.5f;
                Vector2 glowPosition = Rectangle.Value.Position - new Vector2(glowOffsetX, glowOffsetY);

                spriteBatch.Draw(glowTexture, glowPosition, null, SelectionBorderColour, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, glowLayerDepth);
            }
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

        /// <summary>
        /// Gets the string representation of this <see cref="Card"/>.
        /// </summary>
        public override string ToString() => $"{Rank.ToString()} of {Suit.ToString()}";

        /// <summary>
        /// Gets the name of the card texture corresponding to the specified <see cref="CardSuit"/> and <see cref="CardRank"/>.
        /// </summary>
        /// <param name="suit">The <see cref="CardSuit"/>.</param>
        /// <param name="rank">The <see cref="CardRank"/>.</param>
        /// <returns>A <see cref="string"/> value representing the name of the card texture.</returns>
        public static string GetTextureName(CardSuit suit, CardRank rank) =>
            $"card{CardSuitIdentifier[suit]}{CardRankIdentifier[rank]}";

        /// <summary>
        /// Gets the card texture corresponding to the specified <see cref="CardSuit"/> and <see cref="CardRank"/>.
        /// </summary>
        /// <param name="suit">The <see cref="CardSuit"/>.</param>
        /// <param name="rank">The <see cref="CardRank"/>.</param>
        /// <returns>A <see cref="Texture2D"/> value representing the card texture.</returns>
        public static Texture2D GetTexture(CardSuit suit, CardRank rank) =>
            MainGame.Context.Content.Load<Texture2D>($"Cards/{GetTextureName(suit, rank)}");
    }
}
