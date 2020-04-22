using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class SecurityGroupValidatorMapper : BaseMapper<SecurityGroup, SecurityGroupValidator>, ISecurityGroupValidatorMapper
    {
        public SecurityGroupValidatorMapper() { }

        public override SecurityGroupValidator Map(SecurityGroup entity, SecurityGroupValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.IsAllUsers = entity.IsAllUsers;
            entityTransformed.GroupName = entity.GroupName;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.TenantId = entity.TenantId;
            entityTransformed.IdSecurityGroupTenant = entity.IdSecurityGroupTenant;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.GroupChildren = entity.GroupChildren;
            entityTransformed.GroupFather = entity.GroupFather;
            entityTransformed.SecurityUsers = entity.SecurityUsers;
            entityTransformed.ContainerGroups = entity.ContainerGroups;
            entityTransformed.RoleGroups = entity.RoleGroups;
            #endregion

            return entityTransformed;
        }

    }
}
