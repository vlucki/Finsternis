namespace UnityQuery
{

    using UnityEngine;
    using System.Collections;

    public static class VectorExtensions
    {
        public static float GetAngle(this Vector2 v, Vector3 other)
        {
            return Vector2.Angle(v, other);
        }

        public static float GetAngle(this Vector3 v, Vector3 other)
        {
            return Vector3.Angle(v, other);
        }

        public static bool IsZero(this Vector2 v)
        {
            return v == Vector2.zero;
        }

        public static bool IsZero(this Vector3 v)
        {
            return v == Vector3.zero;
        }

        public static bool IsZero(this Vector4 v)
        {
            return v == Vector4.zero;
        }

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

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            float tx = v.x;
            float ty = v.y;

            return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
        }
    }
}
