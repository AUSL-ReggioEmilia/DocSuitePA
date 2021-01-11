using log4net;
using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace VecompSoftware.BiblosDS.WCF.Common
{
    public class WCFUtility
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(WCFUtility));
        public static IClientChannel GetClientConfigChannel<T>(string name, string host = "")
        {
            ClientSection clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;
            if (clientSection == null)
                throw new Exception("Errore nella configurazione WCF. Aggiungere la sezione system.serviceModel/client.");

            ChannelEndpointElement endpoint = clientSection.Endpoints.OfType<ChannelEndpointElement>().FirstOrDefault(x => x.Name == name);
            if (endpoint == null)
                throw new Exception($"Errore nella configurazione WCF. Aggiungere la sezione endpoint con chiave: {name}.");

            Binding binding = null;
            switch (endpoint.Binding)
	        {
                case "basicHttpBinding":
                    binding = (BasicHttpBinding)GetBinding(typeof(BasicHttpBinding), endpoint.BindingConfiguration);
                    break;
                case "wsHttpBinding":
                    binding = (WSHttpBinding)GetBinding(typeof(WSHttpBinding), endpoint.BindingConfiguration);
                    break;
                case "netNamedPipeBinding":
                    binding = (NetNamedPipeBinding)GetBinding(typeof(NetNamedPipeBinding), endpoint.BindingConfiguration);
                    break;
                case "netTcpBinding":
                    binding = (NetTcpBinding)GetBinding(typeof(NetTcpBinding), endpoint.BindingConfiguration);
                    break;
		        default:
                    throw new Exception($"Errore nella configurazione WCF. Binding non supportato: {endpoint.Binding}.");
            }

            string address = endpoint.Address.ToString();
            if (!string.IsNullOrEmpty(host))
            {
                string hostName = endpoint.Address.Host;
                address = address.Replace(hostName, host);
            }
            return (IClientChannel)CreateChannelFactory<T>(address, binding).CreateChannel();
        }

        public static IClientChannel GetClientConfigChannel<T>(string url, string binding, string bindingConfiguration)
        {
            Binding bindingInstance;
            switch (binding)
            {
                case "basicHttpBinding":
                    bindingInstance = (BasicHttpBinding)GetBinding(typeof(BasicHttpBinding), bindingConfiguration);
                    break;
                case "wsHttpBinding":
                    bindingInstance = (WSHttpBinding)GetBinding(typeof(WSHttpBinding), bindingConfiguration);
                    break;
                case "netNamedPipeBinding":
                    bindingInstance = (NetNamedPipeBinding)GetBinding(typeof(NetNamedPipeBinding), bindingConfiguration);
                    break;
                case "netTcpBinding":
                    bindingInstance = (NetTcpBinding)GetBinding(typeof(NetTcpBinding), bindingConfiguration);
                    break;
                default:
                    throw new Exception($"Errore nella configurazione WCF. Binding non supportato: {binding}.");
            }
            return (IClientChannel)CreateChannelFactory<T>(url, bindingInstance).CreateChannel();
        }

        private static ChannelFactory<T> CreateChannelFactory<T>(string url, Binding binding)
        {            
            return new ChannelFactory<T>(binding, new EndpointAddress(url));
        }

        private static Binding GetBinding(Type bindingType, string endpointConfigName)
        {
            return (Binding)Activator.CreateInstance(bindingType, endpointConfigName);
        }

        public static string GetSettingValue(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                logger.Warn("Impossibile trovare la chiave di configurazione: " + key);
                throw;
            }
        }
    }
}