using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.Services.DromedianWeb.Models
{
    public class PublishResolutionResponseModel
    {
        [JsonProperty("response")]
        public bool Response { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("document")]
        public string Document { get; set; }
    }
}
