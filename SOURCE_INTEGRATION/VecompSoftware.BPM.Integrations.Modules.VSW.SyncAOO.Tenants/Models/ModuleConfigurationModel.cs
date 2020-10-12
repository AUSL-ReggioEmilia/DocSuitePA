using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.SyncAOO.Tenants.Models
{
    public class ModuleConfigurationModel
    {
        public string SyncTopicName { get; set; }
        public ICollection<TenantSubscriptionModel> TenantAOOCreateSubscriptions { get; set; }
        public ICollection<TenantSubscriptionModel> TenantCreateSubscriptions { get; set; }
    }
}
