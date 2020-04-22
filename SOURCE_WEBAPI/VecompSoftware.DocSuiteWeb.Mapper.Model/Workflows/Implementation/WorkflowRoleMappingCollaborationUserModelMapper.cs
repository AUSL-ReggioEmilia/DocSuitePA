using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Workflows
{
    public class WorkflowRoleMappingCollaborationUserModelMapper : BaseModelMapper<WorkflowRoleMapping, CollaborationUserModel>, IWorkflowRoleMappingCollaborationUserModelMapper
    {
        public override CollaborationUserModel Map(WorkflowRoleMapping entity, CollaborationUserModel entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Account = string.Empty;
            entityTransformed.DestinationFirst = true;
            entityTransformed.DestinationEmail = entity.Role.EMailAddress;
            entityTransformed.DestinationName = entity.Role.Name;
            entityTransformed.DestinationType = "S";
            entityTransformed.IdRole = entity.Role.EntityShortId;
            #endregion

            return entityTransformed;
        }
    }
}
