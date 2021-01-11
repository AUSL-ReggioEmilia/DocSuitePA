
namespace VecompSoftware.DocSuiteWeb.Common.Infrastructures
{
    public enum ObjectEventType
    {
        Critical = 1,
        Error = 2,
        Warning = 2 * Error,
        Information = 2 * Warning,
        Debug = 2 * Information
    }
}
