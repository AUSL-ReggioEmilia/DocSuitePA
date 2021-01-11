
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    public sealed class UDSTreeViewDto
    {
        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "state")]
        public UDSTreeViewState State { get; set; }

        [JsonProperty(PropertyName = "children")]
        public object Children { get; set; }

        public bool HasChildren()
        {
            return (!(Children is bool) && (Children as IEnumerable<UDSTreeViewDto>).Any());
        }
    }
}
