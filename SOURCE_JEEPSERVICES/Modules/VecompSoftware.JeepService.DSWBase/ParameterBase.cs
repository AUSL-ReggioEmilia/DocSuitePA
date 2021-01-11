using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService
{
    public class ParameterBase
    {
        public string GetString(List<Parameter> parameters, string name)
        {
            return GetString(parameters, name, string.Empty);
        }

        public string GetString(List<Parameter> parameters, string name, string defaultValue)
        {
            return parameters.Exists(parameter => parameter.Key.Eq(name)) ? parameters.First(parameter => parameter.Key.Eq(name)).Value : defaultValue;
        }

        public bool GetBoolean(List<Parameter> parameters, string name)
        {
            return GetBoolean(parameters, name, false);
        }

        public bool GetBoolean(List<Parameter> parameters, string name, bool defaultValue)
        {
            var val = GetString(parameters, name, string.Empty);
            return string.IsNullOrEmpty(val) ? defaultValue : val.Eq("True");
        }

        public int GetInteger(List<Parameter> parameters, string name)
        {
            return GetInteger(parameters, name, 0);
        }

        public int GetInteger(List<Parameter> parameters, string name, int defaultValue)
        {
            var val = GetString(parameters, name, string.Empty);
            return string.IsNullOrEmpty(val) ? defaultValue : int.Parse(val);
        }

        public DateTime GetDateTime(List<Parameter> parameters, string name)
        {
            return GetDateTime(parameters, name, DateTime.Now);
        }

        public DateTime GetDateTime(List<Parameter> parameters, string name, DateTime defaultValue)
        {
            var val = GetString(parameters, name, string.Empty);
            return string.IsNullOrEmpty(val) ? defaultValue : DateTime.ParseExact(val, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
