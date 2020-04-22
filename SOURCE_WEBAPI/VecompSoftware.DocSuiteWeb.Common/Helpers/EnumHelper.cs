using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace VecompSoftware.DocSuiteWeb.Common.Helpers
{
    public class EnumHelper
    {
        /// <summary> Estrae la descrizione dell'enum dall'attributo. </summary>
        public static string GetDescription(Enum enumConstant)
        {
            FieldInfo field = enumConstant.GetType().GetField(enumConstant.ToString());
            string ret = enumConstant.ToString();
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute != null)
            {
                ret = attribute.Description;
            }
            return ret;
        }

        /// <summary>
        /// Recupera l'enumeratore tramite la relativa descrizione confrontato con una stringa
        /// </summary>
        /// <typeparam name="T">Enumeratore</typeparam>
        /// <param name="description">stringa da controllare</param>
        /// <returns></returns>
        public static T ParseDescriptionToEnum<T>(string description)
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .FirstOrDefault(v => GetDescription(v as Enum) == description);
        }

        /// <summary>
        /// Recupero una lista di enumeratori con i valori flaggati da un intero
        /// </summary>
        /// <typeparam name="T">Enumeratore</typeparam>
        /// <param name="intValue">intero da cui voglio estrarre la lista di enumeratori enum</param>
        /// <returns></returns>
        public static IList<T> GetFlaggedValues<T>(int intValue) where T : struct, IConvertible
        {

            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            IList<T> listToReturn = new List<T>();
            Enum enumToParse = Enum.Parse(typeof(T), intValue.ToString()) as Enum;
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                Enum itemAsEnumValue = (Enum)Enum.Parse(typeof(T), item.ToString());

                if (intValue == 0 || enumToParse.HasFlag(itemAsEnumValue))
                {
                    listToReturn.Add(item);
                }
            }

            return listToReturn;
        }
    }
}
