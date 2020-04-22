using Newtonsoft.Json;
using System;

namespace VecompSoftware.JeepService.LogConservation.Models
{
    public class UDSEntityModel
    {
        [JsonProperty("UDSId")]
        public Guid? Id { get; set; }

        [JsonProperty("_year")]
        public int? Year { get; set; }

        [JsonProperty("_number")]
        public int? Number { get; set; }        

        [JsonProperty("_subject")]
        public string Subject { get; set; }
    }
}
