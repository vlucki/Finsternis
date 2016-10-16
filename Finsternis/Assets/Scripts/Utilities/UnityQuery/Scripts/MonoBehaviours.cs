// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonoBehaviours.cs">
//   Copyright (c) Victor Lucki. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityQuery
{
    using System;
    using System.Collections;
    using UnityEngine;

    public static class MonoBehaviours
    {
        public static void Enable(this MonoBehaviour b)
        {
            b.enabled = true;
        }

        public static void Disable(this MonoBehaviour b)
        {
            b.enabled = false;
        }

        public static void CallDelayed(this MonoBehaviour mb, float delayInSeconds, Action callback)
        {
            mb.StartCoroutine(DelayRoutine(delayInSeconds, callback));
        }

        private static IEnumerator DelayRoutine(float delayInSeconds, Action callback)
        {
            yield return Wait.Sec(delayInSeconds);
            callback();
        }
    }
}