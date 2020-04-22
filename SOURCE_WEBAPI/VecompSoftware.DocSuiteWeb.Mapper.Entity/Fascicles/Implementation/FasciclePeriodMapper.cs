using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Fascicles
{
    public class FasciclePeriodMapper : BaseEntityMapper<FasciclePeriod, FasciclePeriod>, IFasciclePeriodMapper
    {
        public override FasciclePeriod Map(FasciclePeriod entity, FasciclePeriod entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.PeriodDays = entity.PeriodDays;
            entityTransformed.PeriodName = entity.PeriodName;
            #endregion

            return entityTransformed;
        }
    }
}
