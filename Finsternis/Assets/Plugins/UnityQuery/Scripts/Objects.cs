// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Objects.cs">
//   Copyright (c) Victor Lucki. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace UnityQuery
{
    using UnityEngine;
    using System.Collections;

    public static class Objects
    {

        public static void DestroyNow(this Object obj)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                Object.DestroyImmediate(obj);
            else
                Object.Destroy(obj);
#else
                Object.Destroy(child);
#endif
        }

    }
}