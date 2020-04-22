using System.Collections.Generic;
using System.Linq;

namespace VecompSoftware.Common
{
    public static class IEnumerableExtensions
    {

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || source.Count() < 1;
        }

    }
}
