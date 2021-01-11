using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.DocSuiteWeb.Common.Helpers
{
    public class LogCategoryHelper : BaseAttributeHelper<LogCategoryAttribute>
    {
        #region [ Constructor ]
        public LogCategoryHelper() : base() { }
        #endregion

        #region [ Methods ]

        public static ICollection<LogCategory> GetCategoriesAttribute(Type source)
        {
            ICollection<LogCategoryAttribute> attributes = GetAttributes(source);
            if (attributes == null || !attributes.Any())
            {
                return new List<LogCategory>() { new LogCategory(LogCategoryDefinition.GENERAL) };
            }

            return attributes
                .Single().Categories
                         .Select(category => new LogCategory(category))
                         .ToList();
        }
        #endregion
    }
}
