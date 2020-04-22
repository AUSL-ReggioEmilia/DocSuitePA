using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Common.Configuration
{
    public interface IMessageConfiguration
    {
        IDictionary<string, ServiceBusMessageConfiguration> GetConfigurations();
    }
}
