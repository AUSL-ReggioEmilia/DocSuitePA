using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Desks
{
    public class DeskCollaborationMapper : BaseEntityMapper<DeskCollaboration, DeskCollaboration>, IDeskCollaborationMapper
    {
        private readonly ICollaborationMapper _collaborationMapper;
        public DeskCollaborationMapper(ICollaborationMapper collaborationMapper)
        {
            _collaborationMapper = collaborationMapper;
        }

        public override DeskCollaboration Map(DeskCollaboration entity, DeskCollaboration entityTransformed)
        {
            return entityTransformed;
        }

    }
}
