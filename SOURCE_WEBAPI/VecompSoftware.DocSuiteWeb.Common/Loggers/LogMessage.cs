
namespace VecompSoftware.DocSuiteWeb.Common.Loggers
{
    public struct LogMessage
    {
        private readonly string _message;

        public LogMessage(string message = "<no message>")
            : this()
        {
            _message = message;
        }

        public string Message => string.IsNullOrEmpty(_message) ? "<no message>" : _message;
    }
}
