// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Swizzling.cs" company="Nick Prühs">
//   Copyright (c) Nick Prühs. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityQuery
{
    using UnityEngine;

    public static class Swizzling
    {
        #region Public Methods and Operators

        public static Vector3 AddZ(this Vector2 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector4 AddZW(this Vector2 v, float z, float w)
        {
            return new Vector4(v.x, v.y, z, w);
        }

        public static Vector4 AddW(this Vector3 v, float w)
        {
            return new Vector4(v.x, v.y, v.z, w);
        }

        public static Vector2 OnlyX(this Vector2 v)
        {
            return new Vector2(v.x, 0);
        }

        public static Vector3 OnlyX(this Vector3 v)
        {
            return new Vector3(v.x, 0, 0);
        }

        public static Vector4 OnlyX(this Vector4 v)
        {
            return new Vector4(v.x, 0, 0, 0);
        }

        public static Vector2 OnlyY(this Vector2 v)
        {
            return new Vector2(0, v.y);
        }

        public static Vector3 OnlyY(this Vector3 v)
        {
            return new Vector3(0, v.y, 0);
        }

        public static Vector4 OnlyY(this Vector4 v)
        {
            return new Vector4(0, v.y, 0, 0);
        }

        public static Vector3 OnlyZ(this Vector3 v)
        {
            return new Vector3(0, 0, v.z);
        }

        public static Vector4 OnlyZ(this Vector4 v)
        {
            return new Vector4(0, 0, v.z, 0);
        }

        public static Vector4 OnlyW(this Vector4 v)
        {
            return new Vector4(0, 0, 0, v.w);
        }

        public static Vector2 WithX(this Vector2 v, float newX)
        {
            return new Vector2(newX, v.y);
        }

        public static Vector3 WithX(this Vector3 v, float newX)
        {
            return new Vector3(newX, v.y, v.z);
        }

        public static Vector2 WithY(this Vector2 v, float newY)
        {
            return new Vector2(v.x, newY);
        }

        public static Vector3 WithY(this Vector3 v, float newY)
        {
            return new Vector3(v.x, newY, v.z);
        }

        public static Vector3 WithZ(this Vector3 v, float newZ)
        {
            return new Vector3(v.x, v.y, newZ);
        }

        public static Vector2 XY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 XY(this Vector4 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector2 XZ(this Vector4 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector2 YX(this Vector2 v)
        {
            return new Vector2(v.y, v.x);
        }

        public static Vector2 YZ(this Vector3 v)
        {
            return new Vector2(v.y, v.z);
        }

        public static Vector2 YZ(this Vector4 v)
        {
            return new Vector2(v.y, v.z);
        }

        public static Vector2 ZW(this Vector4 v)
        {
            return new Vector2(v.z, v.w);
        }

        #endregion
    }
}