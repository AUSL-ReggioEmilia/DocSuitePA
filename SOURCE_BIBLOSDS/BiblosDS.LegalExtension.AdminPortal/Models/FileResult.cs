using Newtonsoft.Json;

namespace BiblosDS.LegalExtension.AdminPortal.Models
{
    public class FileResult
    {
        [JsonProperty("uploaded")]
        public bool Uploaded { get; set; }
        [JsonProperty("fileUid")]
        public string FileUid { get; set; }
        [JsonIgnore]
        public string FileName { get; set; }
        [JsonProperty("extraDescription")]
        public string ExtraDescription { get; set; }
    }
}