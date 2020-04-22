using Newtonsoft.Json;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public class UDSUserModel
    {
        [JsonProperty(PropertyName = "Code")]
        public string Account { get; set; }

        [JsonProperty(PropertyName = "EmailAddress")]
        public string Mail { get; set; }

    }
}