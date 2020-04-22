using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Monitors
{
    public class MonitoringSeriesRoleModel
    {
        #region [ Constructor ]
        public MonitoringSeriesRoleModel()
        {
            UniqueId = new Guid();
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string Role { get; set; }
        public string DocumentSeries { get; set; }
        public int? IdDocumentSeries { get; set; }
        public int? ActivePublished { get; set; }
        public int? Inserted { get; set; }
        public int? Published { get; set; }
        public int? Updated { get; set; }
        public int? Cancelled { get; set; }
        public int? Retired { get; set; }
        public DateTimeOffset? LastUpdated { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
