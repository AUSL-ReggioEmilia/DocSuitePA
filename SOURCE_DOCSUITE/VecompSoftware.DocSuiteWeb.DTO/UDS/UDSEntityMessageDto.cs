using Newtonsoft.Json;
using System;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityMessageDto
    {
        public Guid? UDSMessageId { get; set; }

        public int? IdMessage { get; set; }

        public Guid? UniqueId { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }

        [JsonProperty("MessageLabel")]
        public string Label { get; set; }
    }
}
