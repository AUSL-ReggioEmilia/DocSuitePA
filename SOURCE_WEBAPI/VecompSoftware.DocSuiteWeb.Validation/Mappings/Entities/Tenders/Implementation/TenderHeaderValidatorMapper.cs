using VecompSoftware.DocSuiteWeb.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenders;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenders
{
    public class TenderHeaderValidatorMapper : BaseMapper<TenderHeader, TenderHeaderValidator>, ITenderHeaderValidatorMapper
    {
        public TenderHeaderValidatorMapper(){}

        public override TenderHeaderValidator Map(TenderHeader entity, TenderHeaderValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Title = entity.Title;
            entityTransformed.Abstract = entity.Abstract;
            entityTransformed.Year = entity.Year;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DocumentSeriesItem = entity.DocumentSeriesItem;
            entityTransformed.Resolution = entity.Resolution;
            entityTransformed.TenderLots = entity.TenderLots;            
            #endregion

            return entityTransformed;
        }
    }
}
