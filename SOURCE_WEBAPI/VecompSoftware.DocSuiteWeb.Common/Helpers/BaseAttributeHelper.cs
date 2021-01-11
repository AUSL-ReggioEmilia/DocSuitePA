using System;
using System.Collections.Generic;
using System.Linq;

namespace VecompSoftware.DocSuiteWeb.Common.Helpers
{
    public abstract class BaseAttributeHelper<TAttribute> where TAttribute : Attribute
    {
        #region [ Methods ]

        public static ICollection<TAttribute> GetAttributes(Type sourceType)
        {
            try
            {
                IEnumerable<TAttribute> attributes = Attribute.GetCustomAttributes(sourceType, typeof(TAttribute)).Cast<TAttribute>();
                return attributes.Any() ? attributes.ToList() : new List<TAttribute>();
            }
            catch (Exception)
            {
                return new List<TAttribute>();
            }
        }

        #endregion
    }
}
