using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReadAOO.Tenants.Models
{
    public class ModuleConfigurationModel
    {
        public ICollection<Endpoint> Endpoints { get; set; }
        public string ManagementAOOUrl { get; set; }
    }
}
