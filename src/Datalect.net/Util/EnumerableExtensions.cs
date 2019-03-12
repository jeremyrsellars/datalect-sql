using System.Collections.Generic;

namespace Datalect.Sql
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Interpose<T>(this IEnumerable<T> items, T value)
        {
            bool first = true;
            foreach(var item in items)
            {
                if (!first)
                    yield return value;
                first = false;
                yield return item;
            }
        }

        public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (item != null)
                    yield return item;
            }
        }
    }
}
