using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Monitors
{
    public class MonitoringSeriesSectionTableValuedModel
    {
        #region [ Constructor ]
        public MonitoringSeriesSectionTableValuedModel()
        { }
        #endregion

        #region [ Properties ]
        public string Family { get; set; }
        public string Series { get; set; }
        public string SubSection { get; set; }
        public int ActivePublished { get; set; }
        public int Inserted { get; set; }
        public int Published { get; set; }
        public int Updated { get; set; }
        public int Canceled { get; set; }
        public int Retired { get; set; }
        public DateTimeOffset? LastUpdated { get; set; }
        #endregion
    }
}
