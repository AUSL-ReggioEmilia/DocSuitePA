using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Principal;
using System.Security.Permissions;
using System.Reflection;
using System.Security.Cryptography;
using System.IO;
using log4net;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace BiblosDS.Library.Common.Utility
{
    public class AzureService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AzureService)); 
        public static bool IsAvailable
        {
            get
            {
                try
                {
                    return RoleEnvironment.IsAvailable;
                }
                catch
                {                
                    return false;                    
                }
            }
        }

        public static string GetSettingValue(string key)
        {
            try
            {
                if (IsAvailable)
                    return RoleEnvironment.GetConfigurationSettingValue(key);
                else
                    return ConfigurationManager.AppSettings[key];
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                logger.Warn("Impossibile trovare la chiave di configurazione: " + key);
                throw;
            }
        }

        public static string GetWcfUrl(string key, string endpointName)
        {
            try
            {
                if (IsAvailable)
                    return RoleEnvironment.GetConfigurationSettingValue(key);
                else
                {
                    ClientSection clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;

                    if (clientSection == null)
                        throw new BiblosDS.Library.Common.Exceptions.Generic_Exception("Errore nella configurazione WCF. Aggiungere la sezione: \"system.serviceModel/client\".");

                    ChannelEndpointElement endpoint = clientSection.Endpoints.OfType<ChannelEndpointElement>().FirstOrDefault(x => x.Name == endpointName);

                    if (endpoint == null)
                        throw new Exception("Nessun endpoint definito con chiave:" + endpointName);
                    return endpoint.Address.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                logger.Warn("Impossibile trovare la chiave di configurazione: " + key);
                throw;
            }
        }

        public static string GetConfigurationSettingValue(string key)
        {
            try
            {
                return RoleEnvironment.GetConfigurationSettingValue(key);
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
