using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.DTO.DocumentSeries
{
    public class SeriesObjectFormat
    {
        #region [ Constructor ]
        public SeriesObjectFormat() { }
        #endregion

        #region [ Properties ]
        [JsonProperty("SerieDocumentale")]
        public int IdDocumentSeries { get; set; }

        [JsonProperty("FormatoOggetto")]
        public string ObjectFormat { get; set; }
        #endregion
    }
}
