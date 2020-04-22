using System;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.DocSuiteWeb.Common.Helpers
{
    public class LogDocumentNameHelper : BaseAttributeHelper<LogDocumentUnitAttribute>
    {
        #region [ Constructor ]
        public LogDocumentNameHelper() : base() { }
        #endregion

        #region [ Methods ]

        public static string GetAttributeDescription(Type source)
        {
            LogDocumentUnitAttribute attribute = (LogDocumentUnitAttribute)Attribute.GetCustomAttribute(source, typeof(LogDocumentUnitAttribute));
            if (attribute == null)
            {
                return LogDocumentUnitName.DOCUMENTUNIT;
            }

            return attribute.DocumentUnitName;
        }
        #endregion
    }
}
