namespace Finsternis.Extensions
{
    using UnityEngine;

    public static class ColorExtensions
    {
        /// <summary>
        /// Compress a color object to an int.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Encode(this Color self)
        {
            Color32 color32 = self;

            int c = 0;

            c |= color32.a << 24;
            c |= color32.r << 16;
            c |= color32.g << 8;
            c |= color32.b;

            return c;
        }

        public static Color Set(this Color c, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            if (r.HasValue)
                c.r = r.Value;
            if (g.HasValue)
                c.g = g.Value;
            if (b.HasValue)
                c.b = b.Value;
            if (a.HasValue)
                c.a = a.Value;
            return c;
        }
    }
}