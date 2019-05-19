/*
 * Author: Shon Verch
 * File Name: TimingHelper.cs
 * Project Name: FreeCell
 * Creation Date: 05/19/2019
 * Modified Date: 05/19/2019
 * Description: Extra timing functionality.
 */

using System;

namespace FreeCell
{
    /// <summary>
    /// Extra timing functionality.
    /// </summary>
    public static class TimingHelper
    {
        /// <summary>
        /// Converts <paramref name="seconds"/> to a formatted string representation.
        /// <remarks>
        /// The result is in the format minutes:seconds if <paramref name="seconds"/>
        /// is less than an hour; otherwise, the format is hours:minutes:seconds.
        /// </remarks>
        /// </summary>
        /// <param name="seconds">The elapsed time in <see cref="seconds"/>.</param>
        public static string ElapsedSecondsToTimerString(float seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

            string minutesSeconds = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            return timeSpan.Hours == 0 ? minutesSeconds : $"{timeSpan.Hours:D2}:{minutesSeconds}";
        }
    }
}
