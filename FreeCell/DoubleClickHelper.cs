/*
 * Author: Shon Verch
 * File Name: DoubleClickHelper.cs
 * Project Name: FreeCell
 * Creation Date: 05/19/2019
 * Modified Date: 05/19/2019
 * Description: Manages double clicks.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGameUtilities;
using MonoGameUtilities.Logging;

namespace FreeCell
{
    /// <summary>
    /// Manages double clicks.
    /// </summary>
    public static class DoubleClickHelper
    {
        /// <summary>
        /// The time, in seconds, that multiple clicks can occur for them to be registered as consecutive clicks.
        /// </summary>
        public static double ClickDelay { get; set; } = 0.5;

        /// <summary>
        /// A mapping of <see cref="MouseButton"/> to <see cref="int"/> representing how many times the <see cref="MouseButton"/> has been clicked
        /// within the specified click delay.
        /// </summary>
        private static readonly Dictionary<MouseButton, int> clicks;

        /// <summary>
        /// A mapping of <see cref="MouseButton"/> to <see cref="float"/> representing the time that the <see cref="MouseButton"/> was clicked.
        /// </summary>
        private static readonly Dictionary<MouseButton, double> clickTimes;

        /// <summary>
        /// A mapping of <see cref="MouseButton"/> to <see cref="bool"/> representing the state of the <see cref="MouseButton"/>.
        /// </summary>
        private static readonly Dictionary<MouseButton, bool> states;

        /// <summary>
        /// Initialize this <see cref="DoubleClickHelper"/>.
        /// </summary>
        static DoubleClickHelper()
        {
            clicks = new Dictionary<MouseButton, int>();
            clickTimes = new Dictionary<MouseButton, double>();
            states = new Dictionary<MouseButton, bool>();

            IEnumerable<MouseButton> mouseButtons = Enum.GetValues(typeof(MouseButton)).Cast<MouseButton>();
            foreach (MouseButton mouseButton in mouseButtons)
            {
                clicks[mouseButton] = 0;
                clickTimes[mouseButton] = 0;
                states[mouseButton] = false;
            }
        }

        /// <summary>
        /// Update the <see cref="DoubleClickHelper"/>.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            IEnumerable<MouseButton> mouseButtons = Enum.GetValues(typeof(MouseButton)).Cast<MouseButton>();
            foreach (MouseButton mouseButton in mouseButtons)
            {
                // Update the click states to ensure that there are no "rogue" clicks
                // (i.e. if the user clicks once, and then waits a few seconds, and THEN double clicks).
                double dt = gameTime.TotalGameTime.TotalSeconds - clickTimes[mouseButton];
                if (clicks[mouseButton] == 1 && dt > ClickDelay)
                {
                    clicks[mouseButton] = 0;
                    clickTimes[mouseButton] = 0;
                }

                states[mouseButton] = false;
                if (!Input.GetMouseButtonDown(mouseButton)) return;

                clicks[mouseButton]++;
                if (clicks[mouseButton] == 1)
                {
                    clickTimes[mouseButton] = gameTime.TotalGameTime.TotalSeconds;
                }

                if (clicks[mouseButton] > 1 &&  dt < ClickDelay)
                {
                    clicks[mouseButton] = 0;
                    clickTimes[mouseButton] = 0;
                    states[mouseButton] = true;
                }
                else if (clicks[mouseButton] > 2 && dt > 1)
                {
                    clicks[mouseButton] = 0;
                    states[mouseButton] = false;
                }
            }
        }

        /// <summary>
        /// Determines whether the user has double clicked with the specified <see cref="MouseButton"/>.
        /// </summary>
        public static bool HasDoubleClicked(MouseButton mouseButton) => states[mouseButton];
    }
}
