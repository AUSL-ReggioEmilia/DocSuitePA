using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolTypeMapper : BaseEntityMapper<ProtocolType, ProtocolType>, IProtocolTypeMapper
    {
        public override ProtocolType Map(ProtocolType entity, ProtocolType entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Description = entity.Description;
            #endregion

            return entityTransformed;
        }

    }
}
