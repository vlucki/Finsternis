namespace Finsternis.Extensions
{
    using System.Collections.Generic;

    public static class ICollectionExtensions
    {
        public static bool AddUnique<T>(this ICollection<T> e, T value)
        {
            if (e.Contains(value))
                return false;
            e.Add(value);
            return true;
        }
    }
}