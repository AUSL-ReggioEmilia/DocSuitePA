using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.Tenders
{
    public class TenderLot : DSWBaseEntity
    {
        #region [ Constructor ]
        public TenderLot() : this(Guid.NewGuid()) { }

        public TenderLot(Guid uniqueId) : base(uniqueId)
        {
            TenderLotPayments = new HashSet<TenderLotPayment>();
        }
        #endregion

        #region [ Properties ]
        public string CIG { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual TenderHeader TenderHeader { get; set; }
        public virtual ICollection<TenderLotPayment> TenderLotPayments { get; set; }
        #endregion
    }
}
