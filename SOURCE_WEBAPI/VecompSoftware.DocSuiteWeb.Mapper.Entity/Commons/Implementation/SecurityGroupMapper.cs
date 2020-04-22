using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class SecurityGroupMapper : BaseEntityMapper<SecurityGroup, SecurityGroup>, ISecurityGroupMapper
    {
        public SecurityGroupMapper()
        {

        }

        public override SecurityGroup Map(SecurityGroup entity, SecurityGroup entityTransformed)
        {
            #region [ Base ]
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.GroupName = entity.GroupName;
            entityTransformed.TenantId = entity.TenantId;
            entityTransformed.IdSecurityGroupTenant = entity.IdSecurityGroupTenant;
            entityTransformed.IsAllUsers = entity.IsAllUsers;
            #endregion

            return entityTransformed;
        }

    }
}
