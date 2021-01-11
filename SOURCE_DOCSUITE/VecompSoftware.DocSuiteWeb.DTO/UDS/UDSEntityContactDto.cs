using System;
using Newtonsoft.Json;
using VecompSoftware.Helpers.UDS;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityContactDto
    {
        [JsonProperty("ContactLabel")]
        public string Label { get; set; }

        public Guid? UDSContactId { get; set; }

        public UDSContactType? ContactType { get; set; }

        public int? IdContact { get; set; }

        public string ContactManual { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }
    }
}
