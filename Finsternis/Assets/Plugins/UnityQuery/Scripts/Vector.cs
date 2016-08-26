namespace UnityQuery
{

    using UnityEngine;
    using System.Collections;

    public static class Vector
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
    }
}
