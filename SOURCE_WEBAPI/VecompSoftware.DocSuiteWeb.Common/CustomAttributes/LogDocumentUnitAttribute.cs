using System;

namespace VecompSoftware.DocSuiteWeb.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class LogDocumentUnitAttribute : Attribute
    {
        public string DocumentUnitName { get; set; }

        public LogDocumentUnitAttribute(string documentUnitName)
        {
            DocumentUnitName = documentUnitName;
        }
    }
}
