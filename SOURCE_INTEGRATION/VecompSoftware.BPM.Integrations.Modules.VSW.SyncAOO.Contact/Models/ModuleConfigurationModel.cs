using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.SyncAOO.Contact.Models
{
    public class ModuleConfigurationModel
    {
        public string SyncTopicName { get; set; }
        public ICollection<ContactSubscriptionModel> ContactCreateSubscriptions { get; set; }
        public ICollection<ContactSubscriptionModel> ContactDeleteSubscriptions { get; set; }
        public ICollection<ContactSubscriptionModel> ContactUpdateSubscriptions { get; set; }

    }
}
