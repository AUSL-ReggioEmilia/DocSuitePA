using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace System
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Invokes a public method by reflection.
        /// </summary>
        public static object PublicInvokeMethod(this object instance, string methodName, params object[] args)
        {
            return instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance).Invoke(instance, args);
        }

        /// <summary>
        /// Gets a public property by reflection.
        /// </summary>
        public static object PublicGetProperty(this object instance, string propertyName)
        {
            return instance.GetType().GetProperty(propertyName).GetValue(instance, null);
        }

        /// <summary>
        /// Sets a public property by reflection.
        /// </summary>
        public static void PublicSetProperty(this object instance, string propertyName, object value)
        {
            PropertyInfo info = instance.GetType().GetProperty(propertyName);
            if (info != null)
                info.SetValue(instance, value, null);
            else
                Trace.Write("Property not found " + propertyName);
        }

        public static string GetValue(this Dictionary<string, string> instance, string key)
        {
            if (instance.ContainsKey(key))
                return instance[key];
            return "";            
        }

        public static string ToStringExt(this object obj)
        {
            if (obj == null)
                return string.Empty;
            return obj.ToString();
        }

        public static string GetTypeName(this object obj)
        {
            if (obj == null)
                return string.Empty;

            if (obj.TryConvert<Int64>(-1) != -1)
            {
                return typeof(Int64).ToString();
            }

            if (obj.TryConvert<Double>(-1) != -1)
            {
                return typeof(Double).ToString();
            }

            if (obj.TryConvert<DateTime>(DateTime.MinValue) != DateTime.MinValue)
            {
                return typeof(DateTime).ToString();
            }

            return typeof(String).ToString();
        }
    }  
}
