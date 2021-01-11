using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolUserMapper : BaseEntityMapper<ProtocolUser, ProtocolUser>, IProtocolUserMapper
    {
        public override ProtocolUser Map(ProtocolUser entity, ProtocolUser entityTransformed)
        {
            #region [ Base ]
            //entityTransformed.Year = entity.Year;
            //entityTransformed.Number = entity.Number;
            entityTransformed.Account = entity.Account;
            entityTransformed.Type = entity.Type;
            #endregion

            return entityTransformed;
        }

    }
}
