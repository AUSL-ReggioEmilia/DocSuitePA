namespace VecompSoftware.BPM.Integrations.Model.Configurations
{
    public class ModuleConfiguration
    {
        /// <summary>
        /// Nome univoco del modulo
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Percorso della dll del modulo da caricare (I moduli devono essere inseriti nella cartella Modules del servizio)
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Nome della configurazione del Timer da utilizzare
        /// </summary>
        public string Timer { get; set; }
        /// <summary>
        /// Nome della configurazione del ServiceBus da utilizzare
        /// </summary>
        public string ServiceBus { get; set; }
        /// <summary>
        /// Tipo di "risveglio" del modulo
        /// </summary>
        public ServiceWakeType ServiceWakeType { get; set; }
        /// <summary>
        /// Indica se il modulo è attivo
        /// </summary>
        public bool Enabled { get; set; }
    }
}
