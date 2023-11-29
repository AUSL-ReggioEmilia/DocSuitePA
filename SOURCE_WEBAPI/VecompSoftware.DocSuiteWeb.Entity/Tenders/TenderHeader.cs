using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Entity.Tenders
{
    public class TenderHeader : DSWBaseEntity
    {
        #region [ Constructor ]
        public TenderHeader() : this(Guid.NewGuid()) { }

        public TenderHeader(Guid uniqueId) : base(uniqueId)
        {
            TenderLots = new HashSet<TenderLot>();
        }
        #endregion

        #region [ Properties ]
        public string Title { get; set; }
        public string Abstract { get; set; }
        public int? Year { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual DocumentSeriesItem DocumentSeriesItem { get; set; }
        public virtual Resolution Resolution { get; set; }
        public virtual ICollection<TenderLot> TenderLots { get; set; }
        #endregion
    }
}
