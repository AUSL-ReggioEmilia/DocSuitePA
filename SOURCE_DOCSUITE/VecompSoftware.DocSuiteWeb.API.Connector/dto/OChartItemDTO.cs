using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    internal class OChartItemDTO : IOChartItemDTO
    {

        #region [ Properties ]

        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string FullCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Acronym { get; set; }
        public string Mailboxes { get; set; }

        [JsonConverter(typeof(OChartItemDTOConverter))]
        public IOChartItemDTO Parent { get; set; }
        [JsonConverter(typeof(OChartItemDTOConverter))]
        public IOChartItemDTO[] Items { get; set; }

        #endregion

    }
}
