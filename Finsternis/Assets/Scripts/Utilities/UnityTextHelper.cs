namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public static class UnityTextHelper
    {

        public static string Colorize(this string s, Color c)
        {
            return "<color=" + c.ToHex() + ">" + s + "</color>";
        }

        public static string SetSize(this string s, int size)
        {
            return "<size=" + size.ToString() + ">" + s + "</size>";
        }

        public static string Bold(this string s)
        {
            return "<b>" + s + "</b>";
        }

        private static string Tag(string s, string tagName, string extras = null)
        {
            return "<" + tagName + (extras?? "") + ">" + s + "</"+tagName+">";
        }
    }
}