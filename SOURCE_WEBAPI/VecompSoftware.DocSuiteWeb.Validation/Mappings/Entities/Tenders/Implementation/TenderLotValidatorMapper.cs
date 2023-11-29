using VecompSoftware.DocSuiteWeb.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenders;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenders
{
    public class TenderLotValidatorMapper : BaseMapper<TenderLot, TenderLotValidator>, ITenderLotValidatorMapper
    {
        public TenderLotValidatorMapper(){}

        public override TenderLotValidator Map(TenderLot entity, TenderLotValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.CIG = entity.CIG;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.TenderHeader = entity.TenderHeader;
            entityTransformed.TenderLotPayments = entity.TenderLotPayments;            
            #endregion

            return entityTransformed;
        }
    }
}
