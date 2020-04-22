using System;

namespace VecompSoftware.DocSuiteWeb.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class LogCategoryAttribute : Attribute
    {
        public string[] Categories { get; set; }

        public LogCategoryAttribute(params string[] categories)
        {
            Categories = categories;
        }
    }
}
