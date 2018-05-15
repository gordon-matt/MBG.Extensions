using System.Collections.Generic;
using System.Linq;

namespace MBG.Extensions.Collections
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, params T[] items)
        {
            foreach (T item in items)
            {
                collection.Add(item);
            }
        }

        public static void AddIfNew<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            IEnumerable<T> newItems = (from x in enumerable
                                       where !collection.Contains(x)
                                       select x);

            foreach (T item in newItems)
            {
                collection.Add(item);
            }
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                if (collection.Contains(item))
                {
                    collection.Remove(item);
                }
            }
        }
    }
}