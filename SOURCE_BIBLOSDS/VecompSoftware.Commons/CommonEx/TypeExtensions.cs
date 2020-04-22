using System;
using System.Collections.Generic;
using System.Reflection;

namespace VecompSoftware.Common
{
    public static class TypeExtensions
    {

        public static MethodInfo GetExtensionMethodFor(this Type source, string name, Type[] types, Type type)
        {
            var extended = new List<Type> { type };
            if (!types.IsNullOrEmpty())
                extended.AddRange(types);
            return source.GetMethod(name, BindingFlags.Public | BindingFlags.Static, null, extended.ToArray(), null);
        }
        public static MethodInfo GetExtensionMethodFor<T>(this Type source, string name, Type[] types)
        {
            return source.GetExtensionMethodFor(name, types, typeof(T));
        }

    }
}
