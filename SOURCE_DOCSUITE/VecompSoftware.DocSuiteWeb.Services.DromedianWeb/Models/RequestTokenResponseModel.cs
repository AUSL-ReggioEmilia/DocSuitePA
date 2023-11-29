using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.Services.DromedianWeb.Models
{
    public class RequestTokenResponseModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("user_email")]
        public string UserEmail { get; set; }

        [JsonProperty("user_nicename")]
        public string UserName { get; set; }

        [JsonProperty("user_display_name")]
        public string UserDisplayName { get; set; }
    }
}
