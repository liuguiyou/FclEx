using System;
using System.Collections.Generic;
using System.Linq;

namespace FclEx
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> col)
        {
            return col == null || col.Count == 0;
        }

        public static void AddWhenNotNull<T>(this ICollection<T> col, T item)
        {
            if (item != null) col.Add(item);
        }

        public static void AddRangeSafely<T>(this ICollection<T> col, IEnumerable<T> items)
        {
            if (items == null) return;
            if (col is List<T> list)
            {
                list.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    col.Add(item);
                }
            }
        }

        public static IEnumerable<T> Touch<T>(this IEnumerable<T> col)
        {
            return col ?? Array.Empty<T>();
        }

        /// <summary>
        /// Splits a collection of objects into an unknown number of pages with n items per page 
        /// <para>(for example, if I have a list of 45 shoes and say 'shoes.Partition(10)' I will now have 4 pages of 10 shoes and 1 page of 5 shoes.</para>>
        /// </summary>
        /// <typeparam name="T">The type of object the collection should contain.</typeparam>
        /// <param name="superset">The collection of objects to be divided into subsets.</param>
        /// <param name="pageSize">The maximum number of items each page may contain.</param>
        /// <returns>A subset of this collection of objects, split into pages of maximum size n.</returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this ICollection<T> superset, int pageSize)
        {
            if (superset.Count < pageSize) yield return superset;
            else
            {
                var numberOfPages = Math.Ceiling(superset.Count / (double)pageSize);
                for (var i = 0; i < numberOfPages; i++)
                    yield return superset.Skip(pageSize * i).Take(pageSize);
            }
        }
    }
}
