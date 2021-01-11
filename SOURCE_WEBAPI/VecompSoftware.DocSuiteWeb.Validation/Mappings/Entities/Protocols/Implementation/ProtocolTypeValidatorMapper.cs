using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolTypeValidatorMapper : BaseMapper<ProtocolType, ProtocolTypeValidator>, IProtocolTypeValidatorMapper
    {
        public ProtocolTypeValidatorMapper() { }

        public override ProtocolTypeValidator Map(ProtocolType entity, ProtocolTypeValidator entityTransformed)
        {
            #region[ Base ]
            entityTransformed.EntityShortId = entity.EntityShortId;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Description = entity.Description;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Containers = entity.Containers;
            entityTransformed.Protocols = entity.Protocols;
            #endregion

            return entityTransformed;
        }
    }
}
