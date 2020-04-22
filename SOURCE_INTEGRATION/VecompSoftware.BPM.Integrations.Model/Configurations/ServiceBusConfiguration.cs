namespace VecompSoftware.BPM.Integrations.Model.Configurations
{
    public class ServiceBusConfiguration
    {
        /// <summary>
        /// Nome della configurazione
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tipo di connession ServiceBus (Topic o Queue)
        /// </summary>
        public ServiceBusMessageType ServiceBusMessageType { get; set; }
        /// <summary>
        /// Nome della coda
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// Nome della Topic
        /// </summary>
        public string TopicName { get; set; }
        /// <summary>
        /// Nome della sottoscrizione associata alla Topic
        /// </summary>
        public string Subscription { get; set; }
    }
}
