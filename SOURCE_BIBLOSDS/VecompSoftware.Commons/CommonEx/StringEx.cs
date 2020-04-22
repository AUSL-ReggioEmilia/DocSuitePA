using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace VecompSoftware.Common
{
    public static class StringEx
    {

        public static bool EqualsIgnoreCase(this string source, string value)
        {
            return source.Equals(value, StringComparison.InvariantCultureIgnoreCase);
        }
        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }
        public static byte[] ToByteArray(this string source)
        {
            var bytes = new byte[source.Length * sizeof(char)];
            Buffer.BlockCopy(source.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        public static string GetValueOrDefault(this string source, string defaultValue)
        {
            if (source.IsNullOrWhiteSpace())
                return defaultValue;
            return source;
        }

        public static T ChangeType<T>(this string source)
        {
            return (T)Convert.ChangeType(source, typeof(T));
        }

        public static string[] SplitNoEmpty(this string source, char separator)
        {
            return source.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }
        public static IEnumerable<KeyValuePair<string, string>> ToPairs(this string source, char pairSeparator, char keyValueSeparator)
        {
            var splitted = source.Split(new char[] { pairSeparator }, StringSplitOptions.RemoveEmptyEntries);
            var pairs = splitted.Select(s => s.Split(new char[] { keyValueSeparator })).Where(p => p.Length.Equals(2));
            return pairs.Select(p => new KeyValuePair<string, string>(p[0], p[1]));
        }


        public static PathUtil ToPath(this string source)
        {
            return new PathUtil(source);
        }

    }
}
