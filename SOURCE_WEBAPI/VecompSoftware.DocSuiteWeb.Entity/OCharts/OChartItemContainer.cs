using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.OCharts
{

    public class OChartItemContainer : DSWBaseEntity
    {
        #region [ Constructor ]

        public OChartItemContainer() : this(Guid.NewGuid()) { }

        public OChartItemContainer(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]
        public bool? Master { get; set; }
        public bool? Rejection { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual OChartItem OChartItem { get; set; }
        public virtual Container Container { get; set; }

        #endregion
    }
}
