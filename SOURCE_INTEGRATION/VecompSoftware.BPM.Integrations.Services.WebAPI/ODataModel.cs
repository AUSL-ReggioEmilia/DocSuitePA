using Newtonsoft.Json;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Services.WebAPI
{
    public class ODataModel<T>
        where T : class
    {
        [JsonProperty("@odata.count")]
        public int? TotalItems { get; set; }
        public ICollection<T> Value { get; set; }
    }
}
