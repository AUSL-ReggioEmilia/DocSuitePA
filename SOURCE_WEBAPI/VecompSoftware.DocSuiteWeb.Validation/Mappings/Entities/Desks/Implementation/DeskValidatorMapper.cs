using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks
{
    public class DeskValidatorMapper : BaseMapper<Desk, DeskValidator>, IDeskValidatorMapper
    {
        public DeskValidatorMapper() { }

        public override DeskValidator Map(Desk entity, DeskValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Description = entity.Description;
            entityTransformed.ExpirationDate = entity.ExpirationDate;
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DeskCollaborations = entity.DeskCollaborations;
            entityTransformed.DeskDocuments = entity.DeskDocuments;
            entityTransformed.DeskLogs = entity.DeskLogs;
            entityTransformed.DeskMessages = entity.DeskMessages;
            entityTransformed.DeskRoleUsers = entity.DeskRoleUsers;
            entityTransformed.DeskStoryBoards = entity.DeskStoryBoards;
            #endregion

            return entityTransformed;
        }

    }
}
