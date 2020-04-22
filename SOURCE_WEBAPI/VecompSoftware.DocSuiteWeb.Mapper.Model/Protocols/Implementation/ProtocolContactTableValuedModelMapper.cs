using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Protocols
{
    public class ProtocolContactTableValueModelMapper : BaseModelMapper<ProtocolTableValuedModel, ProtocolContactModel>, IProtocolContactTableValueModelMapper
    {

        public override ProtocolContactModel Map(ProtocolTableValuedModel entity, ProtocolContactModel entityTransformed)
        {
            entityTransformed = null;
            if (entity.ProtocolContact_IDContact.HasValue)
            {
                entityTransformed = new ProtocolContactModel
                {
                    Description = entity.ProtocolContact_Description,
                    IdContact = entity.ProtocolContact_IDContact.Value
                };
            }

            return entityTransformed;
        }

    }
}
