using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolRoleUserMapper : BaseEntityMapper<ProtocolRoleUser, ProtocolRoleUser>, IProtocolRoleUserMapper
    {
        public override ProtocolRoleUser Map(ProtocolRoleUser entity, ProtocolRoleUser entityTransformed)
        {
            #region [ Base ]
            //entityTransformed.Year = entity.Year;
            //entityTransformed.Number = entity.Number;
            entityTransformed.GroupName = entity.GroupName;
            entityTransformed.UserName = entity.UserName;
            entityTransformed.Account = entity.Account;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.Status = entity.Status;
            #endregion

            return entityTransformed;
        }

    }
}
