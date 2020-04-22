using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VecompSoftware.Common
{
    public static class MethodInfoExtensions
    {

        public static object InvokeExtensionMethod(this MethodInfo source, object obj, object[] parameters)
        {
            var extended = new List<object> { obj };
            if (!parameters.IsNullOrEmpty())
                extended.AddRange(parameters);
            return source.Invoke(obj, extended.ToArray());
        }

    }
}
