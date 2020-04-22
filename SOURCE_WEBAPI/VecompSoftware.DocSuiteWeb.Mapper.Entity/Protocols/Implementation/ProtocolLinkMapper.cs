using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolLinkMapper : BaseEntityMapper<ProtocolLink, ProtocolLink>, IProtocolLinkMapper
    {
        public override ProtocolLink Map(ProtocolLink entity, ProtocolLink entityTransformed)
        {
            #region [ Base ]

            entityTransformed.LinkType = entity.LinkType;

            #endregion

            return entityTransformed;
        }

    }
}
