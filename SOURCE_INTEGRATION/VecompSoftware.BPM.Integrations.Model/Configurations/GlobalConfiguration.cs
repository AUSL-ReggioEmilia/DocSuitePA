using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Model.Configurations
{
    public class GlobalConfiguration
    {
        public ICollection<ModuleConfiguration> ModuleConfigurations { get; set; }

        public ICollection<PeriodConfiguration> PeriodConfigurations { get; set; }
    }
}
