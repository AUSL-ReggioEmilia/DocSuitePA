using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Protocols
{
    public class ProtocolTypeTableValuedModelMapper : BaseModelMapper<ProtocolTableValuedModel, ProtocolTypeModel>, IProtocolTypeTableValuedModelMapper
    {

        public override ProtocolTypeModel Map(ProtocolTableValuedModel entity, ProtocolTypeModel entityTransformed)
        {
            entityTransformed = entityTransformed ?? new ProtocolTypeModel();
            entityTransformed.EntityShortId = entity.ProtocolType_IdType;
            entityTransformed.Description = entity.ProtocolType_Description;

            return entityTransformed;
        }

    }
}
