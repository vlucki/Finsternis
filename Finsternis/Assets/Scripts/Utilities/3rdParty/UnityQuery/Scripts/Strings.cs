// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Strings.cs">
//   Copyright (c) Victor Lucki. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityQuery
{

    public static class Strings
    {

        public static string Remove(this string s, string toRemove)
        {
            return s.Replace(toRemove, "");
        }
    }

}