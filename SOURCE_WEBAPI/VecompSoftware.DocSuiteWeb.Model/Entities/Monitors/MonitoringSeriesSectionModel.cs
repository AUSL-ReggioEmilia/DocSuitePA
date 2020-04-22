using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Monitors
{
    public class MonitoringSeriesSectionModel
    {
        #region [ Constructor ]
        public MonitoringSeriesSectionModel()
        {
            UniqueId = new Guid();
        }
        #endregion

        #region [ Proprieties ]
        public Guid UniqueId { get; set; }
        public string Family { get; set; }
        public string Series { get; set; }
        public string SubSection { get; set; }
        public int? ActivePublished { get; set; }
        public int? Inserted { get; set; }
        public int? Published { get; set; }
        public int? Updated { get; set; }
        public int? Canceled { get; set; }
        public int? Retired { get; set; }
        public DateTimeOffset? LastUpdated { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
