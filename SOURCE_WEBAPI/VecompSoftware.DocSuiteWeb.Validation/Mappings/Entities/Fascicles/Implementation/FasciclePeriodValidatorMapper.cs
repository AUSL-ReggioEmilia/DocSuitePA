
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles
{
    public class FasciclePeriodValidatorMapper : BaseMapper<FasciclePeriod, FasciclePeriodValidator>, IFasciclePeriodValidatorMapper
    {
        public FasciclePeriodValidatorMapper() { }


        public override FasciclePeriodValidator Map(FasciclePeriod entity, FasciclePeriodValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.PeriodName = entity.PeriodName;
            entityTransformed.PeriodDays = entity.PeriodDays;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.CategoryFascicles = entity.CategoryFascicles;
            #endregion

            return entityTransformed;
        }

    }
}
