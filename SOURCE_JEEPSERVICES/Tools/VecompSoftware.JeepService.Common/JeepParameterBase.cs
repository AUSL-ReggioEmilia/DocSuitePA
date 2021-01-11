using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using VecompSoftware.Helpers.ExtensionMethods;

namespace VecompSoftware.JeepService.Common
{
    public class JeepParametersBase : InfiniteMarshalByRefObject, IJeepParameter
    {
        #region [ Fields ]

        private string _serviceDirectory;

        #endregion

        #region [ Properties ]

        /// <summary> Cartella di esecuzione del Servizio. </summary>
        [Browsable(false)]
        public string ServiceDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_serviceDirectory))
                {
                    _serviceDirectory = Environment.CurrentDirectory;
                }
                return _serviceDirectory;
            }
            set { _serviceDirectory = value; }
        }

        #endregion

        #region [ Methods ]

        private static bool PropertyToPreserve(PropertyInfo info)
        {
            var browsableAttributes = info.GetCustomAttributes(typeof(BrowsableAttribute), false);
            if (browsableAttributes.Length > 0)
            {
                if (!((BrowsableAttribute)browsableAttributes[0]).Browsable)
                {
                    return false;
                }
            }

            var readOnlyAttributes = info.GetCustomAttributes(typeof(ReadOnlyAttribute), false);
            if (readOnlyAttributes.Length <= 0)
            {
                return true;
            }

            return !((ReadOnlyAttribute)readOnlyAttributes[0]).IsReadOnly;
        }

        public object GetDefaultValue(PropertyInfo info)
        {
            var myAttribute = info.GetCustomAttributes(typeof(DefaultValueAttribute), false);
            return myAttribute.Length <= 0 ? null : ((DefaultValueAttribute)myAttribute[0]).Value;
        }

        /// <summary> Inizializza la lista di parametri. </summary>
        public virtual void Initialize(List<Parameter> parameters)
        {
            // Recupero tutte le properties pubbliche
            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in props)
            {
                if (!PropertyToPreserve(info))
                {
                    continue;
                }

                // Se non esistono parametri o se la proprietà non è inclusa tra i parametri 
                if (parameters == null || !parameters.Exists(parameter => parameter.Key.Eq(info.Name)))
                {
                    // Recupero il valore di default
                    var temp = GetDefaultValue(info);
                    if (temp != null)
                    {
                        info.SetValue(this, Convert.ChangeType(temp, info.PropertyType), null);
                        continue;
                    }
                }

                if (parameters == null)
                {
                    continue;
                }

                // Estraggo il valore dal file di configurazione
                var myValue = parameters.FirstOrDefault(parameter => parameter.Key.Eq(info.Name));
                if (myValue != null)
                {
                    info.SetValue(this, ChangeType(myValue.Value, info.PropertyType), null);
                }
            }
        }

        /// <summary> Converte automaticamente tutti i tipi di dato, compreso ENUM. </summary>
        private static object ChangeType(object value, Type type)
        {
            if (type == typeof(bool))
            {
                if (value == null)
                {
                    value = false;
                }
                else if (value is bool)
                {
                    value = (bool)value;
                }
                else
                {
                    double d;
                    string s = value.ToString().Trim();
                    // t/f
                    // true/false
                    // y/n
                    // yes/no
                    // <>0/0
                    if (s.StartsWith("F", StringComparison.OrdinalIgnoreCase) ||
                        s.StartsWith("N", StringComparison.OrdinalIgnoreCase))
                    {
                        value = false;
                    }
                    else if (double.TryParse(s, out d) && d.AlmostEq(0.0))
                    {
                        value = false;
                    }
                    else
                    {
                        value = true;
                    }
                }
            }
            else if (type.IsEnum)
            {
                value = Enum.Parse(type, value.ToString(), true);
            }
            else if (type == typeof(Guid))
            {
                // If it's already a guid, return it.
                if (!(value is Guid))
                {
                    if (value is string)
                    {
                        value = new Guid(value.ToString());
                    }
                    else
                    {
                        value = new Guid((byte[])value);
                    }
                }
            }
            else
            {
                value = Convert.ChangeType(value, type);
            }
            return value;
        }

        public void DefaultInitialization()
        {
            Initialize(null);
        }

        public List<Parameter> Serialize()
        {
            var tor = new List<Parameter>();
            // Elenco delle proprietà pubbliche
            PropertyInfo[] props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in props)
            {
                if (!PropertyToPreserve(info))
                {
                    continue;
                }

                // Recupero il valore della property
                var myVal = info.GetValue(this, null);
                var defVal = GetDefaultValue(info);
                if (myVal != null && myVal != defVal)
                {
                    // Aggiungo alla Lista di parametri da restituire
                    tor.Add(new Parameter { Key = info.Name, Value = myVal.ToString() });
                }
            }
            return tor;
        }

        #endregion
    }
}
