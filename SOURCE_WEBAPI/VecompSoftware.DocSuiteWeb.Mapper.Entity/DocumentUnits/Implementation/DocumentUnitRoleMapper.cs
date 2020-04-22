using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentUnits
{
    public class DocumentUnitRoleMapper : BaseEntityMapper<DocumentUnitRole, DocumentUnitRole>, IDocumentUnitRoleMapper
    {
        public DocumentUnitRoleMapper()
        { }

        public override DocumentUnitRole Map(DocumentUnitRole entity, DocumentUnitRole entityTransformed)
        {
            #region [ Base ]
            entityTransformed.RoleLabel = entity.RoleLabel;
            entityTransformed.UniqueIdRole = entity.UniqueIdRole;
            entityTransformed.AssignUser = entity.AssignUser;
            entityTransformed.AuthorizationRoleType = entity.AuthorizationRoleType;
            #endregion

            return entityTransformed;
        }

    }
}
