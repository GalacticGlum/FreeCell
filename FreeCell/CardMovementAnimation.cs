/*
 * Author: Shon Verch
 * File Name: CardMovementAnimation.cs
 * Project Name: FreeCell
 * Creation Date: 05/19/2019
 * Modified Date: 05/19/2019
 * Description: Information about a card selection.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities;

namespace FreeCell
{
    /// <summary>
    /// Information about a card movement animation.
    /// </summary>
    public class CardMovementAnimation
    {
        /// <summary>
        /// The <see cref="FreeCell.Card"/> that is being moved.
        /// </summary>
        public Card Card { get; }

        /// <summary>
        /// The lerp information for a card movement animation.
        /// (i.e. lerping from the old position to the new position).
        /// </summary>
        public LerpInformation<Vector2> LerpInformation { get; private set; }

        /// <summary>
        /// The target card pile.
        /// </summary>
        public CardPile TargetPile { get; }

        /// <summary>
        /// Initializes a new <see cref="CardMovementAnimation"/>.
        /// </summary>
        public CardMovementAnimation(Card card, LerpInformation<Vector2> lerpInformation, CardPile targetPile)
        {
            Card = card;
            LerpInformation = lerpInformation;
            TargetPile = targetPile;

            lerpInformation.Finished += OnLerpFinished;
        }

        /// <summary>
        /// Handle the lerp finished event.
        /// </summary>
        private void OnLerpFinished(object sender, LerpInformationEventArgs<Vector2> args)
        {
            // When the lerp has finished, we want to add the card to its target pile
            TargetPile.Push(Card);

            // Set the lerp information to null now that we are done.
            LerpInformation = null;
        }

        /// <summary>
        /// Update this <see cref="CardMovementAnimation"/>.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (LerpInformation == null) return;
            Card.Rectangle = new RectangleF(LerpInformation.Step(deltaTime), Card.Rectangle.GetValueOrDefault().Size);
        }

        /// <summary>
        /// Draw this <see cref="CardMovementAnimation"/>.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (LerpInformation == null) return;
            Card.Draw(spriteBatch);
        }
    }
}
