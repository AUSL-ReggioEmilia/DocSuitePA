using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks
{
    public class DeskRoleUserValidatorMapper : BaseMapper<DeskRoleUser, DeskRoleUserValidator>, IDeskRoleUserValidatorMapper
    {
        public DeskRoleUserValidatorMapper() { }

        public override DeskRoleUserValidator Map(DeskRoleUser entity, DeskRoleUserValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.AccountName = entity.AccountName;
            entityTransformed.PermissionType = entity.PermissionType;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Desk = entity.Desk;
            entityTransformed.DeskDocumentEndorsements = entity.DeskDocumentEndorsements;
            entityTransformed.DeskStoryBoards = entity.DeskStoryBoards;
            #endregion

            return entityTransformed;
        }

    }
}
