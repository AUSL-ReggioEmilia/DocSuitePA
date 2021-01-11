
namespace VecompSoftware.DocSuiteWeb.Common.Loggers
{
    public struct LogCategory
    {
        public static LogCategory NotifyToEmailCategory = new LogCategory(LogCategoryDefinition.NOTIFYEMAIL);
        private readonly string _categoryName;

        public LogCategory(string category)
            : this()
        {
            _categoryName = category;
        }

        public string Category => string.IsNullOrEmpty(_categoryName) ? LogCategoryDefinition.GENERAL : _categoryName;
    }
}
