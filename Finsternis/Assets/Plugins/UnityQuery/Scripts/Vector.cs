namespace UnityQuery
{

    using UnityEngine;
    using System.Collections;

    public static class Vector
    {
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
