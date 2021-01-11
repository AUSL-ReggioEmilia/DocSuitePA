
using Newtonsoft.Json;

namespace BiblosDS.LegalExtension.AdminPortal.Models
{
    public class ChunkMetaData
    {
        [JsonProperty("uploadUid")]
        public string UploadUid { get; set; }
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("contentType")]
        public string ContentType { get; set; }
        [JsonProperty("chunkIndex")]
        public long ChunkIndex { get; set; }
        [JsonProperty("totalChunks")]
        public long TotalChunks { get; set; }
        [JsonProperty("totalFileSize")]
        public long TotalFileSize { get; set; }
        [JsonIgnore]
        public bool IsUploaded
        {
            get
            {
                return (TotalChunks - 1) <= ChunkIndex;
            }
        }
    }
}