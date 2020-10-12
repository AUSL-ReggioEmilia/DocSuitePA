using System;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.PerformanceAnalyzer.Models
{
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string DestinationEventSource { get; set; }
        public List<PerformanceCounterModel> PerformanceCounters { get; set; }
    }
}
