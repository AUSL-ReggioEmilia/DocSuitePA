using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks
{
    public class DeskDocumentEndorsementValidatorMapper : BaseMapper<DeskDocumentEndorsement, DeskDocumentEndorsementValidator>, IDeskDocumentEndorsementValidatorMapper
    {
        public DeskDocumentEndorsementValidatorMapper() { }

        public override DeskDocumentEndorsementValidator Map(DeskDocumentEndorsement entity, DeskDocumentEndorsementValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Endorsement = entity.Endorsement;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DeskDocumentVersion = entity.DeskDocumentVersion;
            entityTransformed.DeskRoleUser = entity.DeskRoleUser;

            #endregion

            return entityTransformed;
        }

    }
}
