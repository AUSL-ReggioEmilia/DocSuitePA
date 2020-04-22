using System;
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class DocumentSeriesItemDTO : IDocumentSeriesItemDTO
    {
        #region [ Properties ]

        public int? IdDocumentSeries { get; set; }

        public int? Status { get; set; }

        public DateTime? PublishingDate { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<CategoryDTO, ICategoryDTO>))]
        public ICategoryDTO Category  { get; set; }

        [JsonConverter(typeof(APIArgumentConverter<DocumentDTO, IDocumentDTO>))]
        public IDocumentDTO Document  { get; set; }

        public int MaxTimesError { get; set; }

        #endregion
    }
}
