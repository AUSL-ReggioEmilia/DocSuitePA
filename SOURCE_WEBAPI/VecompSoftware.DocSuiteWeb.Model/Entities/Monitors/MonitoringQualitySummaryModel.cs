using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Monitors
{
    public class MonitoringQualitySummaryModel
    {
        #region [ Constructr ]
        public MonitoringQualitySummaryModel()
        {
            UniqueId = new Guid();
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string DocumentSeries { get; set; }
        public string Role { get; set; }
        public int? IdDocumentSeries { get; set; }
        public short? IdRole { get; set; }
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
