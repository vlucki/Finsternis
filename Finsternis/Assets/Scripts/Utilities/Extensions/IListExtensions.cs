namespace Finsternis.Extensions
{
    using System.Collections;
    using System.Collections.Generic;

    public static class IListExtensions
    {
        #region Code from BitStrap

        /// <summary>
        /// Behaves like System.Linq.Count however it does not generate garbage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int Count<T>(this List<T> collection, System.Predicate<T> predicate)
        {
            if (predicate == null)
                return 0;

            int count = 0;
            for (int i = 0; i < collection.Count; i++)
            {
                if (predicate(collection[i]))
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Behaves like System.Linq.All however it does not generate garbage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool All<T>(this List<T> collection, System.Predicate<T> predicate)
        {
            if (predicate == null)
                return false;

            for (int i = 0; i < collection.Count; i++)
            {
                if (!predicate(collection[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Behaves like System.Linq.Any however it does not generate garbage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Any<T>(this List<T> collection, System.Predicate<T> predicate)
        {
            if (predicate == null)
                return false;

            if (collection == null)
                return false;

            for (int i = 0; i < collection.Count; i++)
            {
                if (predicate(collection[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Behaves like System.Linq.FirstOrDefault however it does not generate garbage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T FirstOrDefault<T>(this List<T> collection)
        {
            return collection.Count > 0 ? collection[0] : default(T);
        }
        #endregion



        /// <summary>
        /// Behaves like System.Linq.FirstOrDefault however it does not generate garbage.
        /// </summary>
        /// <typeparam name="T">Type of objects in the list.</typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FirstOrDefault<T>(this List<T> list, System.Predicate<T> predicate)
        {
            int count = list.Count;
            for (int index = 0; index < count; index++)
            {
                T item = list[index];
                if (predicate(item))
                    return item;
            }

            return default(T);
        }
    }
}