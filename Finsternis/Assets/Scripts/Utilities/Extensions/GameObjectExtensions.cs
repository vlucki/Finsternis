// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameObjects.cs" company="Nick Prühs">
//   Copyright (c) Nick Prühs. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Finsternis.Extensions
{
    using System.Linq;

    using UnityEngine;

    public static class GameObjectExtensions
    {
        #region Public Methods and Operators by Nick Prühs

        /// <summary>
        ///   Instantiates a new game object and parents it to this one.
        ///   Resets position, rotation and scale and inherits the layer.
        /// </summary>
        /// <param name="parent">Game object to add the child to.</param>
        /// <returns>New child.</returns>
        public static GameObject AddChild(this GameObject parent)
        {
            return parent.AddChild("New Game Object");
        }

        /// <summary>
        ///   Instantiates a new game object and parents it to this one.
        ///   Resets position, rotation and scale and inherits the layer.
        /// </summary>
        /// <param name="parent">Game object to add the child to.</param>
        /// <param name="name">Name of the child to add.</param>
        /// <returns>New child.</returns>
        public static GameObject AddChild(this GameObject parent, string name)
        {
            var go = AddChild(parent, (GameObject)null);
            go.name = name;
            return go;
        }

        /// <summary>
        ///   Instantiates a prefab and parents it to this one.
        ///   Resets position, rotation and scale and inherits the layer.
        /// </summary>
        /// <param name="parent">Game object to add the child to.</param>
        /// <param name="prefab">Prefab to instantiate.</param>
        /// <returns>New prefab instance.</returns>
        public static GameObject AddChild(this GameObject parent, GameObject prefab)
        {
            var go = prefab != null ? Object.Instantiate(prefab) : new GameObject();
            if (go == null || parent == null)
            {
                return go;
            }

            var transform = go.transform;
            transform.SetParent(parent.transform);
            transform.Reset();
            go.layer = parent.layer;
            return go;
        }

        /// <summary>
        ///   Destroys all children of a object.
        /// </summary>
        /// <param name="gameObject">Game object to destroy all children of.</param>
        public static void DestroyChildren(this GameObject gameObject)
        {
            foreach (Transform child in gameObject.transform)
            {
                // Hide immediately.
                child.gameObject.SetActive(false);
                child.gameObject.DestroyNow();
            }
        }

        /// <summary>
        ///   Gets the component of type <typeparamref name="T" /> if the game object has one attached,
        ///   and adds and returns a new one if it doesn't.
        /// </summary>
        /// <typeparam name="T">Type of the component to get or add.</typeparam>
        /// <param name="gameObject">Game object to get the component of.</param>
        /// <returns>
        ///   Component of type <typeparamref name="T" /> attached to the game object.
        /// </returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        /// <summary>
        ///   Resets the local position, rotation and scale of a transform.
        /// </summary>
        /// <param name="transform">Transform to reset.</param>
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        ///   Sets the layer of the game object.
        /// </summary>
        /// <param name="gameObject">Game object to set the layer of.</param>
        /// <param name="layerName">Name of the new layer.</param>
        public static void SetLayer(this GameObject gameObject, string layerName)
        {
            var layer = LayerMask.NameToLayer(layerName);
            gameObject.layer = layer;
        }

        #endregion


        #region Public Methods and Operators by Victor Lucki
        public static void Deactivate(this GameObject go)
        {
            go.SetActive(false);
        }

        public static void Activate(this GameObject go)
        {
            go.SetActive(true);
        }
        #endregion
    }
}