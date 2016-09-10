// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Yields.cs">
//   Copyright (c) Victor Lucki. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityQuery
{
    using UnityEngine;

    public static class Yields
    {
        /// <summary>
        /// Shorthand for yield statements that need to wait some amount of time.
        /// </summary>
        /// <param name="seconds">How many seconds to wait for.</param>
        /// <returns>A WaitForSeconds object with the specified amount of time.</returns>
        public static WaitForSeconds SEC(float seconds)
        {
            return new WaitForSeconds(seconds);
        }

        /// <summary>
        /// Shorthand for yield statements that need to wait for the end of frame.
        /// </summary>
        /// <returns>A WaitForEnfOfFrame object.</returns>
        public static WaitForEndOfFrame EoF()
        {
            return new WaitForEndOfFrame();
        }

        /// <summary>
        /// Shorthand for yield statements that need to wait for the fixed update.
        /// </summary>
        /// <returns>A WaitForFixedUpdate object.</returns>
        public static WaitForFixedUpdate F_UPD()
        {
            return new WaitForFixedUpdate();
        }
        
    }
}