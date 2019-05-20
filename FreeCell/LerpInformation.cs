/*
 * Author: Shon Verch
 * File Name: LerpInformation.cs
 * Project Name: FreeCell
 * Creation Date: 05/19/2019
 * Modified Date: 05/19/2019
 * Description: A simple interface for linearly interpolating between two values.
 */

using System;

namespace FreeCell
{
    /// <summary>
    /// Delegate for a <see cref="LerpInformation{T}"/> event.
    /// </summary>
    public delegate void LerpInformationEventHandler<T>(object sender, LerpInformationEventArgs<T> args);

    /// <summary>
    /// Event arguments for a <see cref="FreeCell.LerpInformation"/> event.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LerpInformationEventArgs<T> : EventArgs
    {
        public LerpInformation<T> LerpInformation { get; }
        public LerpInformationEventArgs(LerpInformation<T> lerpInformation)
        {
            LerpInformation = lerpInformation;
        }
    }

    /// <summary>
    /// A simple interface for linearly interpolating between two values.
    /// </summary>
    public class LerpInformation<T>
    {
        public T Source { get; }
        public T Destination { get; }

        public float Duration { get; }
        public float TimeLeft { get; private set; }
        public float LerpFactor => 1 - TimeLeft / Duration;

        /// <summary>
        /// Called when this <see cref="LerpInformation{T}"/> starts.
        /// </summary>
        public event LerpInformationEventHandler<T> Started;

        /// <summary>
        /// Called when this <see cref="LerpInformation{T}"/> ends.
        /// </summary>
        public event LerpInformationEventHandler<T> Finished;

        /// <summary>
        /// The lerp function.
        /// </summary>
        private readonly Func<T, T, float, T> lerpHandler;
        private bool hasCalledFinishedCallback;

        /// <summary>
        /// Initializes a new <see cref="LerpInformation{T}"/>.
        /// </summary>
        /// <param name="source">The source value.</param>
        /// <param name="destination">The destination value.</param>
        /// <param name="duration">The duration of the lerp.</param>
        /// <param name="lerpHandler">The handler function for lerping between the two values.</param>
        /// <param name="startedEventHandler">Event that is raised when the lerp starts.</param>
        /// <param name="finishedEventHandler">Event that is raised when the lerp ends.</param>
        public LerpInformation(T source, T destination, float duration, Func<T, T, float, T> lerpHandler, 
            LerpInformationEventHandler<T> startedEventHandler = null, LerpInformationEventHandler<T> finishedEventHandler = null)
        {
            Source = source;
            Destination = destination;
            Duration = duration;
            TimeLeft = duration;

            Started = startedEventHandler;
            Finished = finishedEventHandler;

            this.lerpHandler = lerpHandler;
        }

        /// <summary>
        /// Continue the lerp by one frame.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns>The value of the lerp after this frame step.</returns>
        public T Step(float deltaTime)
        {
            // First time we ever run Step
            if (TimeLeft == 0)
            {
                Started?.Invoke(this, new LerpInformationEventArgs<T>(this));
            }

            // If the lerp has finished
            if (TimeLeft <= 0)
            {
                if (!hasCalledFinishedCallback)
                {
                    Finished?.Invoke(this, new LerpInformationEventArgs<T>(this));
                    hasCalledFinishedCallback = true;
                }
                return Destination;
            }

            T result = lerpHandler(Source, Destination, LerpFactor);
            TimeLeft -= deltaTime;

            return result;
        }
    }
}
