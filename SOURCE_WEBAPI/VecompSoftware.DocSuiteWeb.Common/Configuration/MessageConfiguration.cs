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
        private IDictionary<string, ServiceBusMessageConfiguration> _configurations = null;
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
            if (_configurations == null)
            {
                string configurationJson = File.ReadAllText(_pathFileConfig, Encoding.UTF8);
                _configurations = JsonConvert.DeserializeObject<Dictionary<string, ServiceBusMessageConfiguration>>(configurationJson);
            }
            return _configurations;
        }
        #endregion
    }
}