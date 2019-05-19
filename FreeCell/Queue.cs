/*
 * Author: Shon Verch
 * File Name: Queue.cs
 * Project Name: FreeCell
 * Creation Date: 05/19/2019
 * Modified Date: 05/19/2019
 * Description: An array-based implementation of a queue.
 */

using MonoGameUtilities.Logging;

namespace FreeCell
{
    /// <summary>
    /// An array-based implementation of a queue.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the data that this <see cref="Queue{T}"/> stores.</typeparam>
    public class Queue<T>
    {
        /// <summary>
        /// The maximum size of this <see cref="Queue{T}"/>.
        /// </summary>
        public int MaximumSize { get; }

        /// <summary>
        /// The index of the rear element of this <see cref="Queue{T}"/>.
        /// </summary>
        private int rearIndex;

        /// <summary>
        /// The array data of this <see cref="Queue{T}"/>.
        /// </summary>
        private readonly T[] data;

        /// <summary>
        /// Initializes a new <see cref="Queue{T}"/> with the specified <see cref="MaximumSize"/>.
        /// </summary>
        public Queue(int maximumSize)
        {
            MaximumSize = maximumSize;
            rearIndex = 0;

            data = new T[maximumSize];
        }

        /// <summary>
        /// Enqueue the specified <paramref name="value"/> onto this <see cref="Queue{T}"/>.
        /// </summary>
        /// <param name="value">The value of type <see cref="T"/> to enqueue.</param>
        public void Enqueue(T value)
        {
            if (rearIndex == MaximumSize)
            {
                Logger.LogFunctionEntry(string.Empty, "Attempted to enqueue value onto full queue.", LoggerVerbosity.Warning);
                return;
            }

            data[rearIndex++] = value;
        }

        /// <summary>
        /// Dequeues an element from the front of this <see cref="Queue{T}"/>.
        /// </summary>
        /// <returns>
        /// The value of type <see cref="T"/> at the front of this <see cref="Queue{T}"/>.
        /// Returns <c>default(T)</c> if this <see cref="Queue{T}"/> has no elements.
        /// </returns>
        public T Dequeue()
        {
            // Empty queue
            if (rearIndex == 0) return default(T);

            T front = data[0];
            for (int i = 0; i < rearIndex - 1; i++)
            {
                data[i] = data[i + 1];
            }

            // Set the element at the rear of the queue to be "null"
            if (rearIndex < MaximumSize)
            {
                data[rearIndex--] = default(T);
            }

            return front;
        }

        /// <summary>
        /// Get the element in the front of this <see cref="Queue{T}"/>.
        /// </summary>
        /// <returns>
        /// The value of type <see cref="T"/> at the front of this <see cref="Queue{T}"/>.
        /// Returns <c>default(T)</c> if this <see cref="Queue{T}"/> has no elements.
        /// </returns>
        public T Peek() => rearIndex == 0 ? default(T) : data[0];
    }
}
