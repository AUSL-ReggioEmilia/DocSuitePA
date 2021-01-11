using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Fascicles
{
    public class FascicleLinkMapper : BaseEntityMapper<FascicleLink, FascicleLink>, IFascicleLinkMapper
    {
        public override FascicleLink Map(FascicleLink entity, FascicleLink entityTransformed)
        {
            #region [ Base ]
            entityTransformed.FascicleLinkType = entity.FascicleLinkType;
            #endregion



            return entityTransformed;
        }
    }
}
