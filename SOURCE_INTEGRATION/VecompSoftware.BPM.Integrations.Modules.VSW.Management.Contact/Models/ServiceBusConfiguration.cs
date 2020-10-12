namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Contact.Models
{
    public class ServiceBusConfiguration
    {
        public string TopicName { get; set; }
        public string CreateContactSubscription { get; set; }
        public string DeleteContactSubscription { get; set; }
        public string UpdateContactSubscription { get; set; }
    }
}
