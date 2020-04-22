using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentUnits
{
    public class DocumentUnitRoleValidatorMapper : BaseMapper<DocumentUnitRole, DocumentUnitRoleValidator>, IDocumentUnitRoleValidatorMapper
    {
        public DocumentUnitRoleValidatorMapper() { }

        public override DocumentUnitRoleValidator Map(DocumentUnitRole entity, DocumentUnitRoleValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RoleLabel = entity.RoleLabel;
            entityTransformed.UniqueIdRole = entity.UniqueIdRole;
            entityTransformed.AssignUser = entity.AssignUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.RoleAuthorizationType = entity.AuthorizationRoleType;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DocumentUnit = entity.DocumentUnit;
            #endregion

            return entityTransformed;
        }

    }
}
