using VecompSoftware.DocSuiteWeb.Entity.Tenders;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Tenders
{
    public class TenderLotPaymentMapper : BaseEntityMapper<TenderLotPayment, TenderLotPayment>, ITenderLotPaymentMapper
    {
        public override TenderLotPayment Map(TenderLotPayment entity, TenderLotPayment entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Amount = entity.Amount;
            entityTransformed.PaymentKey = entity.PaymentKey;
            #endregion

            return entityTransformed;
        }

    }
}
