using System.Collections.Generic;

namespace VecompSoftware.Common
{
    public static class ListExtensions
    {

        public static bool IsNullOrEmpty<T>(this List<T> source)
        {
            return source == null || source.Count < 1;
        }
        public static bool HasSingle<T>(this List<T> source)
        {
            return !source.IsNullOrEmpty() && source.Count == 1;
        }
        public static bool HasMany<T>(this List<T> source)
        {
            return !source.IsNullOrEmpty() && source.Count > 1;
        }

    }
}
