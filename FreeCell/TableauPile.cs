/*
 * Author: Shon Verch
 * File Name: TableauPile.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: The tableau card pile.
 */

namespace FreeCell
{
    /// <summary>
    /// The tableau <see cref="CardPile"/>.
    /// </summary>
    public class TableauPile : CardPile
    {
        /// <summary>
        /// Initializes a new <see cref="TableauPile"/>.
        /// <remarks>
        /// A <see cref="TableauPile"/> has a maximum size of 19; however, it is very rare
        /// for the full tableau pile to become full.
        /// </remarks>
        /// </summary>
        public TableauPile() : base(19)
        {
        }

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
    }
}
