using System.Linq;

namespace VecompSoftware.Common
{
    public static class TEx
    {

        public static bool IsNullOrEmpty<T>(this T[] source)
        {
            return source == null || !source.Any();
        }
        public static bool HasValue<T>(this T? source, T obj) where T : struct
        {
            return source.HasValue && source.Value.Equals(obj);
        }

    }
}
