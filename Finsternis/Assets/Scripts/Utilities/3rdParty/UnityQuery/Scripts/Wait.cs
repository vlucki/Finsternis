// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Wait.cs">
//   Copyright (c) Victor Lucki. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityQuery
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class Wait
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
            if (Wait.seconds.IsNullOrEmpty())
                Wait.seconds = new Dictionary<float, WaitForSeconds>();

            if (!Wait.seconds.ContainsKey(seconds))
                Wait.seconds[seconds] = new WaitForSeconds(seconds);

            return Wait.seconds[seconds];
        }

        /// <summary>
        /// Shorthand for yield statements that need to wait for the end of frame.
        /// </summary>
        /// <returns>A WaitForEnfOfFrame object.</returns>
        public static WaitForEndOfFrame Frame()
        {
            if (Wait.endOfFrame == null)
                Wait.endOfFrame = new WaitForEndOfFrame();
            return Wait.endOfFrame;
        }

        /// <summary>
        /// Shorthand for yield statements that need to wait for the fixed update.
        /// </summary>
        /// <returns>A WaitForFixedUpdate object.</returns>
        public static WaitForFixedUpdate Fixed()
        {
            if (Wait.fixedUpdate == null)
                Wait.fixedUpdate = new WaitForFixedUpdate();
            return Wait.fixedUpdate;
        }
        
    }
}