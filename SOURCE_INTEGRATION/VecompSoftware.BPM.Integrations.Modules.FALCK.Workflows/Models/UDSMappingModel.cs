using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Configurations
{
    public class UDSMappingModel
    {
        public UDSMappingModel()
        {
            MappingProperties = new Dictionary<string, string>();
        }

        public string UDSRepositoryName { get; set; }
        public IDictionary<string, string> MappingProperties { get; set; }
    }
}
