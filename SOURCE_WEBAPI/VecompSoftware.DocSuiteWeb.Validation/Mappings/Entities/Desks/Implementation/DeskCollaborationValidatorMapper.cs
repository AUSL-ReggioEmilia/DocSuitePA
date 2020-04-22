using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks
{
    public class DeskCollaborationValidatorMapper : BaseMapper<DeskCollaboration, DeskCollaborationValidator>, IDeskCollaborationValidatorMapper
    {
        public DeskCollaborationValidatorMapper() { }

        public override DeskCollaborationValidator Map(DeskCollaboration entity, DeskCollaborationValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Collaboration = entity.Collaboration;
            entityTransformed.Desk = entity.Desk;
            #endregion

            return entityTransformed;
        }

    }
}
