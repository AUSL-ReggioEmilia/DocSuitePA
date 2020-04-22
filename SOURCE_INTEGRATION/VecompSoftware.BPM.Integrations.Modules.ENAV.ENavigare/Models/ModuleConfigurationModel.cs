using System;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.ENAV.ENavigare.Configurations
{
    public class ModuleConfigurationModel
    {
        public string ConnectionString { get; set; }
        public string DocumentSeriesItemTopic { get; set; }
        public string DocumentSeriesItemSubscriptionCreate { get; set; }
        public string DocumentSeriesItemSubscriptionUpdate { get; set; }
        public string WorkflowIntegrationTopic { get; set; }
        public string DocumentSeriesItemSubscriptionRetired { get; set; }
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public IDictionary<string, string> ArchiveMappingUrls { get; set; }
    }
}
