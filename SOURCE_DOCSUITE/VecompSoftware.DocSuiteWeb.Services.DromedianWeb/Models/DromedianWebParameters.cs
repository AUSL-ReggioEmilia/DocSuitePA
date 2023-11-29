using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.Services.DromedianWeb.Models
{
    public class DromedianWebParameters
    {
        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("BaseUrl")]
        public string BaseUrl { get; set; }

        [JsonProperty("RequestTokenUrl")]
        public string RequestTokenUrl { get; set; }

        [JsonProperty("PublishResolutionUrl")]
        public string PublishResolutionUrl { get; set; }
    }
}
