/*
 * Author: Shon Verch
 * File Name: SelectionInformation.cs
 * Project Name: FreeCell
 * Creation Date: 05/19/2019
 * Modified Date: 05/19/2019
 * Description: Information about a card selection.
 */

namespace FreeCell
{
    /// <summary>
    /// Information about a card selection.
    /// </summary>
    public class CardSelectionInformation
    {
        /// <summary>
        /// The selected <see cref="FreeCell.Card"/>.
        /// </summary>
        public Card Card { get; }

        /// <summary>
        /// The <see cref="FreeCell.CardPile"/> which the <see cref="Card"/> belongs to.
        /// </summary>
        public CardPile CardPile { get; }

        /// <summary>
        /// Initializes a new <see cref="CardSelectionInformation"/>.
        /// </summary>
        public CardSelectionInformation(Card card, CardPile cardPile)
        {
            Card = card;
            CardPile = cardPile;
        }
    }
}
