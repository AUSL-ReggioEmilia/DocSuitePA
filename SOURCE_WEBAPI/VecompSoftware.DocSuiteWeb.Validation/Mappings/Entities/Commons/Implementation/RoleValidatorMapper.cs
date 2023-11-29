using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class RoleValidatorMapper : BaseMapper<Role, RoleValidator>, IRoleValidatorMapper
    {
        public RoleValidatorMapper() { }

        public override RoleValidator Map(Role entity, RoleValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityShortId = entity.EntityShortId;
            entityTransformed.Name = entity.Name;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.Collapsed = entity.Collapsed;
            entityTransformed.EMailAddress = entity.EMailAddress;
            entityTransformed.ServiceCode = entity.ServiceCode;
            entityTransformed.UniqueId = entity.UniqueId;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Father = entity.Father;
            entityTransformed.TenantAOO = entity.TenantAOO;
            entityTransformed.CollaborationUsers = entity.CollaborationUsers;
            entityTransformed.Contacts = entity.Contacts;
            entityTransformed.OChartItems = entity.OChartItems;
            entityTransformed.WorkflowRepositories = entity.WorkflowRepositories;
            entityTransformed.ProtocolRoles = entity.ProtocolRoles;
            entityTransformed.RoleGroups = entity.RoleGroups;
            entityTransformed.RoleUsers = entity.RoleUsers;
            entityTransformed.ProtocolRoleUsers = entity.ProtocolRoleUsers;
            entityTransformed.ResolutionRoles = entity.ResolutionRoles;
            entityTransformed.WorkflowInstanceRoles = entity.WorkflowInstanceRoles;
            entityTransformed.FascicleRoles = entity.FascicleRoles;
            entityTransformed.TemplateCollaborationUsers = entity.TemplateCollaborationUsers;
            entityTransformed.TemplateCollaborations = entity.TemplateCollaborations;
            entityTransformed.DossierRoles = entity.DossierRoles;
            entityTransformed.DossierFolderRoles = entity.DossierFolderRoles;
            entityTransformed.UDSAuthorizations = entity.UDSAuthorizations;
            #endregion

            return entityTransformed;
        }

    }
}
