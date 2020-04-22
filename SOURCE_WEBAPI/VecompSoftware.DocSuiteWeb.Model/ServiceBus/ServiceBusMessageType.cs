
namespace VecompSoftware.DocSuiteWeb.Model.ServiceBus
{
    public enum ServiceBusMessageType
    {
        Message = 1,
        Command = 2,
        Event = 2 * Command
    }
}
