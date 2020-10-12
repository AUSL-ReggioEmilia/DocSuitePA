namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Tenants.Models
{
    public class ModuleConfigurationModel
    {
        public string DatabaseConnectionString { get; set; }
        public ServiceBusConfiguration ServiceBusConfiguration { get; set; }
    }
}
