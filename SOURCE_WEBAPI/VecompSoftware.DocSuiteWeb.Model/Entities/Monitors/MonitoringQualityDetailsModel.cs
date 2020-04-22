using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Monitors
{
    public class MonitoringQualityDetailsModel
    {
        #region [ Constructor ]
        public MonitoringQualityDetailsModel()
        {
            UniqueId = new Guid();
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public int? IdDocumentSeriesItem { get; set; }
        public string Identifier { get; set; }
        public int? Year { get; set; }
        public int? Number { get; set; }
        public int? Published { get; set; }
        public int? FromResolution { get; set; }
        public int? FromProtocol { get; set; }
        public int? WithoutLink { get; set; }
        public int? WithoutDocument { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
