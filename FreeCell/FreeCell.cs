/*
 * Author: Shon Verch
 * File Name: FreeCell.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: The free cell card pile.
 */

namespace FreeCell
{
    /// <summary>
    /// The free cell <see cref="CardPile"/>.
    /// </summary>
    public class FreeCell : CardPile
    {
        /// <summary>
        /// The <see cref="Card"/> occupying this <see cref="FreeCell"/>.
        /// </summary>
        public Card Value => Peek();

        /// <summary>
        /// Initializes a new <see cref="FreeCell"/>.
        /// </summary>
        public FreeCell() : base(1)
        {
        }

        /// <summary>
        /// A <see cref="Card"/> can only be pushed onto this <see cref="FreeCell"/> if
        /// another <see cref="Card"/> is not already occupying it (i.e. it is empty).
        /// </summary>
        /// <param name="card">The <see cref="Card"/> to be pushed.</param>
        /// <returns>A boolean value indicating whether the <see cref="Card"/> can be pushed onto this <see cref="FreeCell"/>.</returns>
        protected override bool CanPush(Card card) => Empty;
    }
}
