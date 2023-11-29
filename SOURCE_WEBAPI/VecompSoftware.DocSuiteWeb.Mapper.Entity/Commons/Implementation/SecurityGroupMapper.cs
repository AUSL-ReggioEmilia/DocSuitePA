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
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.GroupName = entity.GroupName;
            entityTransformed.IsAllUsers = entity.IsAllUsers;
            #endregion

            return entityTransformed;
        }

    }
}
