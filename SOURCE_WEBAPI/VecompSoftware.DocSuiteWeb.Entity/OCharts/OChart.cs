using System;
using System.Collections.Generic;


namespace VecompSoftware.DocSuiteWeb.Entity.OCharts
{

    public class OChart : DSWBaseEntity
    {
        #region [ Constructor ]

        public OChart() : this(Guid.NewGuid()) { }

        public OChart(Guid uniqueId)
            : base(uniqueId)
        {
            OChartItems = new HashSet<OChartItem>();
        }
        #endregion

        #region [ Properties ]
        public bool? Enabled { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? Imported { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<OChartItem> OChartItems { get; set; }

        #endregion
    }
}
