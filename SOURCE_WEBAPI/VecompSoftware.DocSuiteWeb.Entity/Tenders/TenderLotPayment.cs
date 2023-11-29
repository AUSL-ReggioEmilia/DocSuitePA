using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Tenders
{
    public class TenderLotPayment : DSWBaseEntity
    {
        #region [ Constructor ]
        public TenderLotPayment() : this(Guid.NewGuid()) { }

        public TenderLotPayment(Guid uniqueId) : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]
        public string PaymentKey { get; set; }
        public decimal? Amount { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual TenderLot TenderLot { get; set; }
        #endregion
    }
}
