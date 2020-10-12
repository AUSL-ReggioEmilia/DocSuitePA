namespace VecompSoftware.BPM.Integrations.Modules.VSW.Management.Audit.Models
{
    public class ModuleConfigurationModel
    {
        public string DatabaseConnectionString { get; set; }
        public ServiceBusConfiguration ServiceBusConfiguration { get; set; }
    }
}
