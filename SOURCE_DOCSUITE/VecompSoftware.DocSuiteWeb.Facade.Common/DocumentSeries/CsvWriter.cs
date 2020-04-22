using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace VecompSoftware.DocSuiteWeb.Facade.Common.DocumentSeries
{
    public class CsvWriter : IDisposable
    {
        #region [ Fields ]

        private string _commaSeparated = ",";
        #endregion

        #region [ Properties ]
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypeCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private Stream OutputStream { get; set; }
        private Encoding Encoding { get; set; }

        public string Separator
        {
            get { return _commaSeparated; }
            set { _commaSeparated = value; }
        }

        public List<string> IgnoreProperties { get; private set; }
        public const string EndOfLine = "\r\n";
        #endregion

        #region [ Constructor ]
        private CsvWriter()
        {
            this.Encoding = Encoding.GetEncoding(1252);
        }

        public CsvWriter(Stream outputStream)
            : this()
        {
            this.OutputStream = outputStream;
            this.IgnoreProperties = new List<string>();
        }

        public CsvWriter(Stream outputStream, string commaSeparator, List<string> ignoreProperties)
            : this()
        {
            this.OutputStream = outputStream;
            this.Separator = commaSeparator;
            this.IgnoreProperties = ignoreProperties;
        }
        #endregion

        #region [ Methods ]
        public void WriteLine(object item)
        {
            var objType = item.GetType();
            var properties = GetFilteredProperties(objType);
            var valuesList = new List<object>();

            foreach (var property in properties)
            {
                try
                {
                    if (property.PropertyType.IsGenericType)
                    {
                        var genericType = property.PropertyType.GetGenericTypeDefinition();
                        if (genericType == typeof(IDictionary<,>))
                        {

                            var dictionaryValues = property.PropertyType.GetProperty("Values");
                            IEnumerable<object> values = Enumerable.Empty<object>();
                            try
                            {
                                var data = property.GetValue(item, null);
                                values = (IEnumerable<object>)dictionaryValues.GetValue(data, null);
                            }
                            catch
                            {
                                valuesList.Add(string.Empty);
                                continue;
                            }

                            var enumerable = values.ToList();
                            if (!enumerable.Any())
                                continue;

                            valuesList.AddRange(enumerable);
                            continue;
                        }
                    }

                    var value = property.GetValue(item, null);
                    valuesList.Add(value);
                }
                catch
                {
                    valuesList.Add(null);
                }
            }

            WriteValues(valuesList.ToArray());

        }

        public void WriteHeader(object item)
        {
            var objType = item.GetType();
            var properties = GetFilteredProperties(objType);
            var valuesList = new List<object>();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.PropertyType.IsGenericType)
                {
                    var genericType = propertyInfo.PropertyType.GetGenericTypeDefinition();
                    if (genericType == typeof(IDictionary<,>))
                    {
                        var dictionaryKeys = propertyInfo.PropertyType.GetProperty("Keys");
                        IEnumerable<string> keys = Enumerable.Empty<string>();
                        try
                        {
                            var data = propertyInfo.GetValue(item, null);
                            keys = (IEnumerable<string>)dictionaryKeys.GetValue(data, null);
                        }
                        catch
                        {
                            valuesList.Add(null);
                            continue;
                        }

                        var enumerable = keys.ToList();
                        if (!enumerable.Any())
                            continue;

                        valuesList.AddRange(enumerable);
                        continue;
                    }
                }

                var attribute =
                    (CsvExportAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(CsvExportAttribute), true);

                if (attribute != null)
                {
                    if (!String.IsNullOrEmpty(attribute.FieldDescription))
                    {
                        valuesList.Add(attribute.FieldDescription);
                        continue;
                    }
                    valuesList.Add(propertyInfo.Name);
                }

            }
            WriteValues(valuesList);
        }

        private void WriteValues(IList<object> values)
        {
            var length = values.Count();
            var separatorBytes = Encoding.GetBytes(Separator);
            var endOfLineBytes = Encoding.GetBytes(EndOfLine);

            for (var i = 0; i < length; i++)
            {
                var value = values[i];
                var stringVal = value == null ? string.Empty : value.ToString();

                var enclose = stringVal.IndexOf(',') >= 0 || stringVal.IndexOf('"') >= 0 || stringVal.IndexOf(Separator, StringComparison.InvariantCulture) >= 0;
                stringVal = stringVal.Replace("\"", "\"\"");

                if (enclose)
                    stringVal = string.Format("\"{0}\"", stringVal);

                var output = Encoding.GetBytes(stringVal);
                this.OutputStream.Write(output, 0, output.Length);

                if (i <= length - 1)
                    this.OutputStream.Write(separatorBytes, 0, separatorBytes.Length);
            }

            this.OutputStream.Write(endOfLineBytes, 0, endOfLineBytes.Length);
        }

        private IEnumerable<PropertyInfo> GetFilteredProperties(Type type)
        {
            var properties = GetPropertyInfo(type);
            properties = properties.Where(p => !IgnoreProperties.Contains(p.Name) && p.CanRead).ToArray();
            return properties;
        }

        private static PropertyInfo[] GetPropertyInfo(Type type)
        {
            if (TypeCache.ContainsKey(type))
                return TypeCache[type];

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => new { Property = x, Attribute = (CsvExportAttribute)Attribute.GetCustomAttribute(x, typeof(CsvExportAttribute), true) })
                .OrderBy(x => x.Attribute != null ? x.Attribute.FieldIndex : -1)
                .Select(x => x.Property)
                .ToArray();

            TypeCache[type] = properties;
            return properties;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                //if (managedResource != null)
                //{
                //    managedResource.Dispose();
                //    managedResource = null;
                //}
            }
        }
        #endregion
    }
}
