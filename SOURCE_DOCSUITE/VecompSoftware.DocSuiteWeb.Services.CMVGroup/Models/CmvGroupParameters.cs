using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.Services.CMVGroup.Models
{
    public class CMVGroupParameters
    {
        [JsonProperty("PostToRemoteUrl")]
        public string PostToRemoteUrl { get; set; }

        [JsonProperty("PostToRemotePublicationTesterUrl")]
        public string PostToRemotePublicationTesterUrl { get; set; }

        [JsonProperty("PostToRemoteDisablePublicationTester")]
        public bool PostToRemoteDisablePublicationTester { get; set; }
    }
}
