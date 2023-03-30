using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class CollectionsExtensions
    {

#region SelectElements

        /// <summary>
        /// Returns a random element.
        /// </summary>
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            // check if the Enumerable is null, if so return the default value for type T
            if(enumerable == null)
            {
                return default(T);
            }
            
            int count = enumerable.Count();

            // check if the Enumerable is empty, if so return the default value for type T
            if(count == 0)
            {
                return default(T);
            }

            return enumerable.ElementAt(UnityEngine.Random.Range(0, count));
        }

        /// <summary>
        /// Returns a random element that satisfies the specified predicate condition.
        /// </summary>
        /// <param name="predicate">The predicate that provides the criteria for the random selection.</param>
        public static T Random<T>(this IEnumerable<T> enumerable, System.Predicate<T> predicate)
        {
            // check if the Enumerable is null, if so return the default value for type T
            if (enumerable == null)
            {
                return default(T);
            }

            int count = enumerable.Count();

            // check if the Enumerable is empty, if so return the default value for type T
            if (count == 0)
            {
                return default(T);
            }

            // check if the predicate is null, if so return a random element from the enumerable
            if (predicate == null)
            {
                return Random(enumerable);
            }

            // filter all items that don't satisfy the predicate
            var subset = enumerable.Filter(predicate);
            int subsetCount = subset.Count();

            // check if the filter is empty, if so return the default value for type T
            if (subsetCount == 0)
            {
                return default(T);
            }

            return subset.ElementAt(UnityEngine.Random.Range(0, subsetCount));
        }

        /// <summary>
        /// Returns a subset of elements that satisfies the specified predicate condition.
        /// </summary>
        /// <param name="predicate">The predicate that provides the criteria for the subset selection.</param>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> enumerable, System.Predicate<T> predicate)
        {
            // check if the Enumerable is null, if so return null
            if (enumerable == null)
            {
                return null;
            }

            int count = enumerable.Count();

            // check if the Enumerable is empty, if so return null
            if (count == 0)
            {
                return null;
            }
            
            // check if the predicate is null, if so return the Enumerable
            if (predicate == null)
            {
                return enumerable;
            }

            var subset = Enumerable.Empty<T>();

            // for each element in the Enumerable test: if the element isn't null and satisfies the predicate, add it to the subset
            foreach (var item in enumerable)
            {
                if (item == null)
                {
                    continue;
                }

                if (!predicate(item))
                {
                    continue;
                }

                subset.Append(item);
            }

            // check if the subset is empty, if so return null
            if (subset.Count() == 0)
            {
                return null;
            }

            return subset;
        }

#endregion

#region InsertElements
        
        /// <summary>
        /// Adds the specified element to the collection a specified number of times.
        /// </summary>
        /// <param name="element">The element to add to the collection.</param>
        /// <param name="times">The number of times to add the element to the collection.</param>
        public static void AddElementMultipleTimes<T>(this ICollection<T> collection, T element, int times)
        {
            // check if times is 0 or negative or if the collection is null, if so return
            if(times <= 0 || collection == null)
            {
                return;
            }
            
            // add the element to the collection the specified number of times
            for(int i=0; i<times; i++)
            {
                collection.Add(element);
            }
        }


        /// <summary>
        /// Adds the specified range of elements to the collection.
        /// </summary>
        /// <param name="range">The elements to add to the collection.</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            // check if the collection is null or if the range is null, if so return
            if(collection == null || range == null)
            {
                return;
            }

            // add the range to the collection
            foreach(T element in range)
            {
                collection.Add(element);
            }
        }

        /// <summary>
        /// Adds the specified range of elements to the collection a specified number of times.
        /// </summary>
        /// <param name="range">The elements to add to the collection.</param>
        /// <param name="times">The number of times to add the element to the collection.</param>
        public static void AddRangeMultipleTimes<T>(this ICollection<T> collection, IEnumerable<T> range, int times)
        {   
            // check if times is 0 or negative or the collection is null or if the range is null, if so return
            if(times <= 0 || collection == null || range == null)
            {
                return;
            }

            // add the range to the collection the specified number of times
            for(int i=0; i<times; i++)
            {
                foreach(T element in range)
                {
                    collection.Add(element);
                }
            }
        }

#endregion

    }
}