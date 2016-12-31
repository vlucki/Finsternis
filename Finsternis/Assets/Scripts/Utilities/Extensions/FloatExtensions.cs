namespace Finsternis.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class FloatExtensions
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
            return UnityEngine.Mathf.Pow(value, exponent);
        }
    }
}
