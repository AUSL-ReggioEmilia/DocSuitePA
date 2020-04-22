using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VecompSoftware.DocSuiteWeb.Common.Configuration
{
    public class MessageConfiguration : IMessageConfiguration
    {
        #region [ Fields ]
        private readonly string _pathFileConfig;
        #endregion

        #region [ Constructor ]
        public MessageConfiguration(string pathFileConfig)
        {
            _pathFileConfig = pathFileConfig;
        }
        #endregion

        #region [ Methods ]

        public IDictionary<string, ServiceBusMessageConfiguration> GetConfigurations()
        {
            string configurationJson = string.Empty;
            IDictionary<string, ServiceBusMessageConfiguration> configurations = null;
            try
            {
                configurationJson = File.ReadAllText(_pathFileConfig, Encoding.UTF8);
                configurations = JsonConvert.DeserializeObject<IDictionary<string, ServiceBusMessageConfiguration>>(configurationJson);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return configurations;
        }
        #endregion
    }
}