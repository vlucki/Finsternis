namespace UnityQuery
{
    using UnityEngine;
    using System.Collections.Generic;
    public static class Components
    {

        /// <summary>
        /// Searches for a component of the given type, looking for it in the children and parents.
        /// </summary>
        /// <typeparam name="T">Type of component searched.</typeparam>
        /// <param name="c">Component that is calling this method.</param>
        /// <returns>The first component of type T found, if any.</returns>
        public static T GetComponentInParentsOrChildren<T>(this Component c) where T : Component
        {
            T siblingComponent = c.GetComponentInParent<T>();
            if (!siblingComponent)
                siblingComponent = c.GetComponentInChildren<T>();
            return siblingComponent;
        }

        /// <summary>
        /// Searches for a component of the given type, looking for it in the children and parents.
        /// </summary>
        /// <typeparam name="T">Type of component searched.</typeparam>
        /// <param name="c">Component that is calling this method.</param>
        /// <returns>The first component of type T found, if any.</returns>
        public static T GetComponentInParentsOrChildren<T>(this GameObject c) where T : Component
        {
            T siblingComponent = c.GetComponentInParent<T>();
            if (!siblingComponent)
                siblingComponent = c.GetComponentInChildren<T>();
            return siblingComponent;
        }

        /// <summary>
        /// Searches for a component of the given type, looking for it in the children and parents.
        /// </summary>
        /// <typeparam name="T">Type of component searched.</typeparam>
        /// <param name="c">Component that is calling this method.</param>
        /// <returns>Every component of type T found, if any.</returns>
        public static List<T> GetComponentsInParentsOrChildren<T>(this Component c) where T : Component
        {
            List<T> siblingComponents = new List<T>();
            c.GetComponentsInParent<T>(true, siblingComponents);
            siblingComponents.AddRange(c.GetComponentsInChildren<T>());
            return siblingComponents;
        }

    }
}