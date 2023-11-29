using VecompSoftware.DocSuiteWeb.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenders;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenders
{
    public class TenderLotPaymentValidatorMapper : BaseMapper<TenderLotPayment, TenderLotPaymentValidator>, ITenderLotPaymentValidatorMapper
    {
        public TenderLotPaymentValidatorMapper(){}

        public override TenderLotPaymentValidator Map(TenderLotPayment entity, TenderLotPaymentValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Amount = entity.Amount;
            entityTransformed.PaymentKey = entity.PaymentKey;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.TenderLot = entity.TenderLot;
            #endregion

            return entityTransformed;
        }
    }
}
