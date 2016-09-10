// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vectors.cs">
//   Copyright (c) Victor Lucki. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityQuery
{

    using UnityEngine;

    public static class Vectors
    {
        /// <summary>
        /// Calculates the angle between two Vector2.
        /// </summary>
        /// <param name="v">Caller of the mwthod.</param>
        /// <param name="other">Vector to be used in order to obtain the angle.</param>
        /// <returns>Float representing the angle, in degrees.</returns>
        public static float Angle(this Vector2 v, Vector3 other)
        {
            return Vector2.Angle(v, other);
        }

        /// <summary>
        /// Calculates the angle between two Vector3.
        /// </summary>
        /// <param name="v">Caller of the mwthod.</param>
        /// <param name="other">Vector to be used in order to obtain the angle.</param>
        /// <returns>Float representing the angle, in degrees.</returns>
        public static float Angle(this Vector3 v, Vector3 other)
        {
            return Vector3.Angle(v, other);
        }

        /// <summary>
        /// Calculates the direction between two Vector2.
        /// </summary>
        /// <param name="v">Starting point.</param>
        /// <param name="other">Target.</param>
        /// <returns>Normalized vector from v to other.</returns>
        public static Vector2 Towards(this Vector2 v, Vector2 other)
        {
            return (other - v).normalized;
        }

        /// <summary>
        /// Calculates the direction between two Vector3.
        /// </summary>
        /// <param name="v">Starting point.</param>
        /// <param name="other">Target.</param>
        /// <returns>Normalized vector from v to other.</returns>
        public static Vector3 Towards(this Vector3 v, Vector3 other)
        {
            return (other - v).normalized;
        }

        public static float Distance(this Vector2 v, Vector2 other)
        {
            return Vector2.Distance(v, other);
        }

        public static float Distance(this Vector3 v, Vector3 other)
        {
            return Vector3.Distance(v, other);
        }

        /// <summary>
        /// Compares a given Vector2 with Vector2.Zero.
        /// </summary>
        /// <param name="v">Vector to compare.</param>
        /// <returns>True if every component of v is 0.</returns>
        public static bool IsZero(this Vector2 v)
        {
            return v == Vector2.zero;
        }

        /// <summary>
        /// Compares a given Vector3 with Vector3.Zero.
        /// </summary>
        /// <param name="v">Vector to compare.</param>
        /// <returns>True if every component of v is 0.</returns>
        public static bool IsZero(this Vector3 v)
        {
            return v == Vector3.zero;
        }

        /// <summary>
        /// Compares a given Vector4 with Vector4.Zero.
        /// </summary>
        /// <param name="v">Vector to compare.</param>
        /// <returns>True if every component of v is 0.</returns>
        public static bool IsZero(this Vector4 v)
        {
            return v == Vector4.zero;
        }

        /// <summary>
        /// Clamps the x and y values of a Vector.
        /// </summary>
        /// <param name="v">Vector with the values to be clamped.</param>
        /// <param name="min">Vector representing the minimum x and y values.</param>
        /// <param name="max">Vector representing the maximum x and y values.</param>
        /// <returns></returns>
        public static Vector2 LinearClamp(this Vector2 v, Vector2 min, Vector2 max)
        {
            Vector2 clampedVector = new Vector3(
            Mathf.Clamp(v.x, min.x, max.x),
            Mathf.Clamp(v.y, min.y, max.y));
            return clampedVector;
        }

        /// <summary>
        /// Calculates the largest between two axis of a given Vector2.
        /// </summary>
        /// <param name="v">The vector to be used.</param>
        /// <returns>The largest value between the x and y coordinates of the given Vector2.</returns>
        public static float Max(this Vector2 v)
        {
            return Mathf.Max(v.x, v.y);
        }

        /// <summary>
        /// Calculates the smallest between two axis of a given Vector2.
        /// </summary>
        /// <param name="v">The vector to be used.</param>
        /// <returns>The smallest value between the x and y coordinates of the given Vector2.</returns>
        public static float Min(this Vector2 v)
        {
            return Mathf.Min(v.x, v.y);
        }

        /// <summary>
        /// Applies a clockwise rotation to a Vector2 around the z axis.
        /// </summary>
        /// <param name="v">Vector to rotate.</param>
        /// <param name="degrees">How many degrees to rotate.</param>
        /// <returns>Rotated vector.</returns>
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            return Quaternion.Euler(0, 0, degrees) * v;
        }
    }
}
