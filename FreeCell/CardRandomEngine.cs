/*
 * File Name: CardRandomEngine.cs
 * Project Name: FreeCell
 * Creation Date: 05/18/2019
 * Modified Date: 05/18/2019
 * Description: Pseudorandom number generator to emulate Microsoft's Game Seeding system.
 */

using System;

namespace FreeCell
{
    /// <summary>
    /// Pseudorandom number generator to emulate Microsoft's Game Seeding system.
    /// </summary>
    public class CardRandomEngine
    {
        private int seed;

        /// <summary>
        /// Initializes a new <see cref="CardRandomEngine"/> with the current time as the <see cref="seed"/>.
        /// </summary>
        public CardRandomEngine() : this((int)DateTime.Now.Ticks) { }

        /// <summary>
        /// Initializes a new <see cref="CardRandomEngine"/> with the specified <paramref name="seed"/>.
        /// </summary>
        /// <param name="seed">The integer value of the seed.</param>
        public CardRandomEngine(int seed)
        {
            this.seed = seed;
        }

        /// <summary>
        /// Generates a random integer.
        /// </summary>
        /// <returns>An integer with a seemingly arbitrary value.</returns>
        public int Next() => ((seed = 214013 * seed + 2531011) & int.MaxValue) >> 16;
    }
}
