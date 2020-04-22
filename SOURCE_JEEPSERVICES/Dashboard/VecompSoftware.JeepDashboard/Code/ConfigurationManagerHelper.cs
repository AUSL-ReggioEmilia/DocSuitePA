using System.Configuration;
using System.IO;
using System.Xml;
using VecompSoftware.JeepDashboard.Properties;
using Vecompsoftware.FileServer.Services;
using Vecompsoftware.FileServer.Services.ActionMessages;

namespace VecompSoftware.JeepDashboard.Code
{
    static class ConfigurationManagerHelper
    {

        public static Configuration LoadConfiguration(string serviceFullPath, IFileRepositoryService client, ref bool isProtected)
        {
            if (client != null)
            {
                var retrievedConfig = client.LoadServiceConfiguration(serviceFullPath);
                isProtected = retrievedConfig.IsProtected;
                File.WriteAllBytes("JeepService.config", retrievedConfig.JeepServiceConfig);
                var map = new ExeConfigurationFileMap {ExeConfigFilename = "JeepService.config"};
                return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            }
            var currentConfiguration = ConfigurationManager.OpenExeConfiguration(serviceFullPath);
            isProtected = currentConfiguration.GetSection("appSettings").SectionInformation.IsProtected;
            return currentConfiguration;
        }

        public static void SaveConfiguration(Configuration configuration, IFileRepositoryService client, bool encrypt)
        {
            if (client != null)
            {
                configuration.Save(ConfigurationSaveMode.Full);
                var xmlConfiguration = File.ReadAllBytes("JeepService.config");
                File.Delete("JeepService.config");
                client.SaveServiceConfiguration(new ServiceConfigurationMessage { JeepServiceConfig = xmlConfiguration, JeepServicePath = Settings.Default.JeepServicePath }, encrypt);
            }
            else
            {
                if (encrypt)
                {
                    EncryptConfigSection("appSettings", configuration);
                }
                else
                {
                    DecryptConfigSection("appSettings", configuration);
                }
                configuration.Save(ConfigurationSaveMode.Full);
            }
        }

        private static void EncryptConfigSection(string sectionKey, Configuration targetConfig)
        {
            var section = targetConfig.GetSection(sectionKey);
            if (section == null || section.SectionInformation.IsProtected || section.ElementInformation.IsLocked)
            {
                // TODO: esplicitare errore e correzione all'operatore
                return;
            }

            section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            section.SectionInformation.ForceSave = true;
        }

        private static void DecryptConfigSection(string sectionKey, Configuration targetConfig)
        {
            var section = targetConfig.GetSection(sectionKey);
            if (section == null || !section.SectionInformation.IsProtected || section.ElementInformation.IsLocked)
            {
                // TODO: esplicitare errore e correzione all'operatore
                return;
            }

            section.SectionInformation.UnprotectSection();
            section.SectionInformation.ForceSave = true;
        }
    }
}
