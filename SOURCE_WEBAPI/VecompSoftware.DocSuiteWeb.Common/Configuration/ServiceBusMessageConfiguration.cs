namespace VecompSoftware.DocSuiteWeb.Common.Configuration
{
    public class ServiceBusMessageConfiguration
    {
        #region [ Properties ]

        public string QueueName { get; set; }

        public string TopicName { get; set; }

        public MessageType MessageType { get; set; }

        public string DefaultFilterEvent { get; set; }
        #endregion
    }
}