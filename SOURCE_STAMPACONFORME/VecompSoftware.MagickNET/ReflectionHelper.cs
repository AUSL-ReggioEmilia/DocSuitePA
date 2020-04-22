using System;
using System.Linq;
using VecompSoftware.Commons.CommonEx;

namespace VecompSoftware.Commons
{
    public static class ReflectionHelper
    {

        public static object InvokeMethod(object obj, string name, object[] parameters, params Type[] extensions)
        {
            var types = parameters.IsNullOrEmpty() ? new Type[0] : parameters.Select(p => p.GetType()).ToArray();

            var objType = obj.GetType();
            var method = objType.GetMethod(name, types);
            if (method != null)
                return method.Invoke(obj, parameters);

            foreach (var item in extensions)
            {
                method = item.GetExtensionMethodFor(name, types, objType);
                if (method == null)
                    continue;

                return method.InvokeExtensionMethod(obj, parameters);
            }

            var message = "ReflectionHelper.InvokeMethod: Metodo non trovato. {0}, {1}";
            message = string.Format(message, name, string.Join(", ", types.Select(t => t.Name)));
            throw new InvalidOperationException(message);
        }

    }
}
