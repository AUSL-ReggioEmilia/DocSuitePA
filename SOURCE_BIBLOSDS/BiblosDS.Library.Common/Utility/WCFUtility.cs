
using System.ServiceModel;
using System.ServiceModel.Channels;
using System;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Configuration;
using BiblosDS.Library.Common.Services;

namespace BiblosDS.Library.Common.Utility
{
    public static class WCFUtility
    {      
        public static IClientChannel GetClientConfigChannel<T>(string name, string host = "")
        {
            ClientSection clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;

            if (clientSection == null)
                throw new BiblosDS.Library.Common.Exceptions.Generic_Exception("Errore nella configurazione WCF. Aggiungere la sezione: \"system.serviceModel/client\".");

            ChannelEndpointElement endpoint = clientSection.Endpoints.OfType<ChannelEndpointElement>().FirstOrDefault(x => x.Name == name);
            if (endpoint == null)
                throw new BiblosDS.Library.Common.Exceptions.Generic_Exception("Errore nella configurazione WCF. Aggiungere la sezione endpoint con chiave: \""+name+"\".");

            Binding binding = null;
            switch (endpoint.Binding)
	        {
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
                    throw new BiblosDS.Library.Common.Exceptions.Generic_Exception("Errore nella configurazione WCF. Binding non supportato:" + endpoint.Binding + ".");
            }
            string addres = AzureService.IsAvailable ? AzureService.GetConfigurationSettingValue("BiblosDS_StorageUrl") : endpoint.Address.ToString();
            if (!string.IsNullOrEmpty(host))
            {                
                var index = addres.IndexOf("//") + 2;
                var index1 = addres.IndexOf(":", index);
                if (index1 < 0)
                    index1 = addres.IndexOf("/", index);
                addres = addres.Substring(0, index) + host + addres.Substring(index1, addres.Length - index1);
            }
            return (IClientChannel)CreateChannelFactory<T>(addres, binding).CreateChannel();
        }

        private static ChannelFactory<T> CreateChannelFactory<T>(string url, Binding binding)
        {            
            return new ChannelFactory<T>(binding, new EndpointAddress(url));
        }

        private static Binding GetBinding(Type bindingType, string endpointConfigName)
        {
            return (Binding)Activator.CreateInstance(bindingType, endpointConfigName);
        }
    }
}