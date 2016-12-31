namespace Finsternis.Extensions
{
    using UnityEngine;

    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Destroys an object as quickly as possible, taking into account wheather the game is running or not and if it's running in the editor.
        /// </summary>
        /// <param name="obj">The object to destroy.</param>
        public static void DestroyNow(this Object obj)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                Object.DestroyImmediate(obj);
            else
                Object.Destroy(obj);
#else
                Object.Destroy(obj);
#endif
        }
    }
}