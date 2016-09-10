// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonoBehaviours.cs">
//   Copyright (c) Victor Lucki. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityQuery
{
    using UnityEngine;

    public static class MonoBehaviours
    {
        public static void Error(this MonoBehaviour b, string message)
        {
            Debug.LogError(message, b);
        }

        public static void Warn(this MonoBehaviour b, string message)
        {
            Debug.LogWarning(message, b);
        }
    }
}