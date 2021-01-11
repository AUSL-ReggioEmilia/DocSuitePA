using System.Configuration;
using System.IO;
using System.Threading;

namespace VecompSoftware.ServiceBus.Receiver
{
    public static class ReceiverConfiguration
    {
        public static string QueueName => ConfigurationManager.AppSettings["QueueName"];

        public static string QueueConnectionString => ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];

        public static string ListenerAssemblyFullName => ConfigurationManager.AppSettings["VecompSoftware.ServiceBus.Receiver.Listener.Assembly.FullName"];

        public static string AddressesJsonConfigWebAPI
        {
            get
            {
                if (string.IsNullOrEmpty(_path_AddressesJsonConfigWebAPI))
                {
                    _path_AddressesJsonConfigWebAPI = Path.Combine(Thread.GetDomain().BaseDirectory, "WebApi.Client.Config.Addresses.json");
                }
                return _path_AddressesJsonConfigWebAPI;
            }
        }

        private static string _path_AddressesJsonConfigWebAPI = string.Empty;

    }
}
