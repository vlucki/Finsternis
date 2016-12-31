// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Wait.cs">
//   Copyright (c) Victor Lucki. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Finsternis.Extensions
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class WaitHelpers
    {
        private static WaitForEndOfFrame endOfFrame;
        private static WaitForFixedUpdate fixedUpdate;
        private static Dictionary<float, WaitForSeconds> seconds;

        /// <summary>
        /// Shorthand for yield statements that need to wait some amount of time.
        /// </summary>
        /// <param name="seconds">How many seconds to wait for.</param>
        /// <returns>A WaitForSeconds object with the specified amount of time.</returns>
        public static WaitForSeconds Sec(float seconds)
        {
            if (WaitHelpers.seconds.IsNullOrEmpty())
                WaitHelpers.seconds = new Dictionary<float, WaitForSeconds>();

            if (!WaitHelpers.seconds.ContainsKey(seconds))
                WaitHelpers.seconds[seconds] = new WaitForSeconds(seconds);

            return WaitHelpers.seconds[seconds];
        }

        /// <summary>
        /// Shorthand for yield statements that need to wait for the end of frame.
        /// </summary>
        /// <returns>A WaitForEnfOfFrame object.</returns>
        public static WaitForEndOfFrame Frame()
        {
            if (WaitHelpers.endOfFrame == null)
                WaitHelpers.endOfFrame = new WaitForEndOfFrame();
            return WaitHelpers.endOfFrame;
        }

        /// <summary>
        /// Shorthand for yield statements that need to wait for the fixed update.
        /// </summary>
        /// <returns>A WaitForFixedUpdate object.</returns>
        public static WaitForFixedUpdate Fixed()
        {
            if (WaitHelpers.fixedUpdate == null)
                WaitHelpers.fixedUpdate = new WaitForFixedUpdate();
            return WaitHelpers.fixedUpdate;
        }
        
    }
}