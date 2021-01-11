using System.Collections.Generic;

namespace VecompSoftware.DocSuite.SPID.Common.Extensions
{
    public static class DictionaryExtension
    {
        public static T SafeGet<K, T>(this IDictionary<K, T> dictionary, K key)
        {
            dictionary.TryGetValue(key, out T value);
            return value;
        }
    }
}
