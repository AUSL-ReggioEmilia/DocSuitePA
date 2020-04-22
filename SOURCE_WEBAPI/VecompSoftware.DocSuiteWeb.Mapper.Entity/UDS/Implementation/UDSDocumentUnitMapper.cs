using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS
{
    public class UDSDocumentUnitMapper : BaseEntityMapper<UDSDocumentUnit, UDSDocumentUnit>, IUDSDocumentUnitMapper
    {
        public override UDSDocumentUnit Map(UDSDocumentUnit entity, UDSDocumentUnit entityTransformed)
        {
            #region [ Base ]

            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.RelationType = entity.RelationType;

            #endregion

            return entityTransformed;
        }

    }
}
