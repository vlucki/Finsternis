namespace Finsternis.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine;
    using System.Linq;

    public static class IEnumerableExtensions
    {
        #region Public Methods and Operators by Nick Prühs (UnityQuery)

        /// <summary>
        ///   Checks whether a sequence contains all elements of another one.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the sequence to check.</typeparam>
        /// <param name="first">Containing sequence.</param>
        /// <param name="second">Contained sequence.</param>
        /// <returns>
        ///   <c>true</c>, if the sequence contains all elements of the other one, and
        ///   <c>false</c> otherwise.
        /// </returns>
        public static bool ContainsAll<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return second.All(first.Contains);
        }

        /// <summary>
        ///   Determines whether a sequence is null or doesn't contain any elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the sequence to check.</typeparam>
        /// <param name="sequence">Sequence to check. </param>
        /// <returns>
        ///   <c>true</c> if the sequence is null or empty, and
        ///   <c>false</c> otherwise.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                return true;
            }

            return !sequence.Any();
        }

        /// <summary>
        ///   Returns a comma-separated string that represents a sequence.
        /// </summary>
        /// <param name="sequence">Sequence to get a textual representation of.</param>
        /// <returns>Comma-separated string that represents the sequence.</returns>
        public static string SequenceToString(this IEnumerable sequence)
        {
            // Check empty sequence.
            if (sequence == null)
            {
                return "null";
            }

            var stringBuilder = new System.Text.StringBuilder();

            // Add opening bracket.
            stringBuilder.Append("[");

            foreach (var element in sequence)
            {
                var elementString = element as string;
                if (elementString == null && element == null)
                    elementString = "null";
                else if (elementString == null)
                {
                    // Handle nested enumerables.
                    var elementEnumerable = element as IEnumerable;
                    elementString = elementEnumerable != null
                        ? elementEnumerable.SequenceToString()
                        : element.ToString();
                }

                // Add comma.
                stringBuilder.AppendFormat("{0},", elementString);
            }

            // Empty sequence.
            if (stringBuilder.Length <= 1)
            {
                return "[]";
            }

            // Add closing bracket.
            stringBuilder[stringBuilder.Length - 1] = ']';
            return stringBuilder.ToString();
        }

        #endregion

        #region Public Methods and Operators by Victor Lucki
        public static T GetRandom<T>(this IEnumerable<T> e, Func<int, int, int> randomFunction, int min = 0, int max = -1)
        {
            if (e.IsNullOrEmpty())
                return default(T);

            int count = 0;
            var collection = e as ICollection<T>;

            if (collection != null)
                count = collection.Count;
            else
                count = e.Count();

            min = Mathf.Clamp(min, 0, count - 1);

            if (max >= 0)
                max = Mathf.Clamp(max, min, count - 1);
            else
                max = count - 1;

            int random = (min == max ? min : randomFunction(min, max));

            if (collection != null)
            {
                var list = e as IList<T>;
                if (list != null)
                    return list[random];
            }

            return e.ElementAtOrDefault(random);
        }
        #endregion
    }
}