using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Fascicles
{
    public class FascicleDocumentUnitMapper : BaseEntityMapper<FascicleDocumentUnit, FascicleDocumentUnit>, IFascicleDocumentUnitMapper
    {
        public override FascicleDocumentUnit Map(FascicleDocumentUnit entity, FascicleDocumentUnit entityTransformed)
        {
            entityTransformed.ReferenceType = entity.ReferenceType;
            return entityTransformed;
        }
    }
}
