namespace Finsternis.Extensions
{
    using UnityEngine;

    public static class VectorExtensions
    {
        public static readonly Vector2 Half2 = new Vector2(.5f, .5f);

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

        /// <summary>
        /// Creates a Vector2 from the original, updating the specified components.
        /// </summary>
        /// <param name="v2">The Vector to be modified.</param>
        /// <param name="x">Replacement for the X component.</param>
        /// <param name="y">Replacement for the Y component.</param>
        /// <returns>A Vector2 updated with the specified components.</returns>
        public static Vector2 Set(this Vector2 v2, float? x = null, float? y = null)
        {
            Vector2 newVector = new Vector2(v2.x, v2.y);
            if (x.HasValue)
                newVector.x = x.Value;
            if (y.HasValue)
                newVector.y = y.Value;
            return newVector;
        }

        /// <summary>
        /// Creates a Vector3 from the original, updating the specified components.
        /// </summary>
        /// <param name="v3">The Vector to be modified.</param>
        /// <param name="x">Replacement for the X component.</param>
        /// <param name="y">Replacement for the Y component.</param>
        /// <param name="z">Replacement for the Z component.</param>
        /// <returns>A Vector3 updated with the specified components.</returns>
        public static Vector3 Set(this Vector3 v3, float? x = null, float? y = null, float? z = null)
        {
            if (x.HasValue)
                v3.x = x.Value;
            if (y.HasValue)
                v3.y = y.Value;
            if (z.HasValue)
                v3.z = z.Value;
            return v3;
        }

        /// <summary>
        /// Creates a Vector4 from the original, updating the specified components.
        /// </summary>
        /// <param name="v4">The Vector to be modified.</param>
        /// <param name="x">Replacement for the X component.</param>
        /// <param name="y">Replacement for the Y component.</param>
        /// <param name="z">Replacement for the Z component.</param>
        /// <param name="w">Replacement for the W component.</param>
        /// <returns>A Vector4 updated with the specified components.</returns>
        public static Vector4 Set(this Vector4 v4, float? x = null, float? y = null, float? z = null, float? w = null)
        {
            if (x.HasValue)
                v4.x = x.Value;
            if (y.HasValue)
                v4.y = y.Value;
            if (z.HasValue)
                v4.z = z.Value;
            if (w.HasValue)
                v4.w = w.Value;
            return v4;
        }

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
        /// Calculates the largest between three axis of a given Vector3.
        /// </summary>
        /// <param name="v">The vector to be used.</param>
        /// <returns>The largest value between the x, y and z coordinates of the given Vector3.</returns>
        public static float Max(this Vector3 v)
        {
            return Mathf.Max(v.x, v.y, v.z);
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

        public static Vector2 Add(this Vector2 v, float? x = null, float? y = null)
        {
            Vector2 newVector = new Vector2(v.x + x.ValueOrDefault(), v.y + y.ValueOrDefault());
            return newVector;
        }

        public static Vector3 Add(this Vector3 v, float? x = null, float? y = null, float? z = null)
        {
            Vector3 newVector = new Vector3(v.x + x.ValueOrDefault(), v.y + y.ValueOrDefault(), v.z + z.ValueOrDefault());
            return newVector;
        }
        
        public static bool Compare(this Vector2 v, float x, float y, bool approximate = false)
        {
            if (approximate)
                return Mathf.Approximately(v.x, x) && Mathf.Approximately(v.y, y);
            else
                return v.x == x && v.y == y;
        }

        public static float Dot(this Vector3 v, Vector3 other)
        {
            return Vector3.Dot(v, other);
        }

        public static Vector2 YX(this Vector2 v)
        {
            return new Vector2(v.y, v.x);
        }
    }
}