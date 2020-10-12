using System.Threading.Tasks;

namespace VecompSoftware.ServiceBus.ServiceBus
{
    public interface IServiceBusClient
    {
        Task CreateSubscriptionAsync(string topicName, string subscriptionName, string filter, bool enableSession = false);
        Task DeleteSubscriptionAsync(string topicName, string subscriptionName);
    }
}
