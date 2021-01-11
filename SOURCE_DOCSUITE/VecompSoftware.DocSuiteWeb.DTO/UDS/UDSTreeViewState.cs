
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    public struct UDSTreeViewState
    {
        [JsonProperty(PropertyName = "selected")]
        public bool? Selected { get; set; }

        [JsonProperty(PropertyName = "opened")]
        public bool? Opened { get; set; }

        [JsonProperty(PropertyName = "disabled")]
        public bool? Disabled { get; set; }

        [JsonProperty(PropertyName = "checked")]
        public bool? Checked { get; set; }
    }
}
