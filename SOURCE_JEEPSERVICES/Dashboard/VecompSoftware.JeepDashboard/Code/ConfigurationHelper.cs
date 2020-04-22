using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using VecompSoftware.JeepDashboard.Properties;
using VecompSoftware.JeepService.Common;
using Vecompsoftware.FileServer.Services;
using Vecompsoftware.FileServer.Services.ActionMessages;

namespace VecompSoftware.JeepDashboard.Code
{
    static class ConfigurationHelper
    {

        public static Configuration LoadConfiguration(string serviceConfigurationFullPath, IFileRepositoryService client)
        {
            if (client != null)
            {
                ConfigurationUtil.ModuleVersions = new Dictionary<string, string>();
                var configuration = client.LoadXmlConfiguration(serviceConfigurationFullPath).JeepConfig;
                // Per ogni modulo carico i dati relativi alla versione
                foreach (Module module in configuration.Modules)
                {
                    ConfigurationUtil.ModuleVersions.Add(module.Id, client.LoadModuleAssemblyProperties(module.FullAssemblyPath).Properties["FileVersion"]);
                }
                return configuration;
            }

            if (!File.Exists(serviceConfigurationFullPath))
            {
                throw new FileNotFoundException("Impossibile trovare il file di configurazione JeepConfig.xml.", serviceConfigurationFullPath);
            }

            var serializer = new XmlSerializer(typeof(Configuration));
            using (Stream stream = new FileStream(serviceConfigurationFullPath, FileMode.Open))
            {
                return (Configuration)serializer.Deserialize(stream);
            }
        }

        public static void SaveConfiguration(Configuration configuration, IFileRepositoryService client)
        {
            if (client != null)
            {
                client.SaveXmlConfiguration(new XmlConfigurationMessage { JeepConfig = configuration, JeepConfigPath = Settings.Default.JeepServiceConfig });
            }
            else
            {
                File.WriteAllText(Settings.Default.JeepServiceConfig, configuration.ToString());
            }
        }
    }
}
