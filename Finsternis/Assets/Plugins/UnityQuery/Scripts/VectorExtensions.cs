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

        public static float Max(this Vector2 v)
        {
            return Mathf.Max(v.x, v.y);
        }
    }
}
