using UnityEngine;

namespace Finsternis.Extensions
{
    public static class NumberExtensions
    {
        public static float ValueOrDefault(this float? f, float defaultValue = 0)
        {
            return f.HasValue ? f.Value : defaultValue;
        }

        public static float Pow(this float value, float exponent)
        {
            if (exponent == 0)
                return 1;
            if (exponent == 1)
                return value;
            return Mathf.Pow(value, exponent);
        }

        public static float Pow(this int value, float exponent)
        {
            if (exponent == 0)
                return 1;
            if (exponent == 1)
                return value;
            return Mathf.Pow(value, exponent);
        }

        public static int Ceil(this float value)
        {
            return Mathf.CeilToInt(value);
        }

        public static int Abs(this int value)
        {
            return Mathf.Abs(value);
        }

        public static float Abs(this float value)
        {
            return Mathf.Abs(value);
        }
    }
}
