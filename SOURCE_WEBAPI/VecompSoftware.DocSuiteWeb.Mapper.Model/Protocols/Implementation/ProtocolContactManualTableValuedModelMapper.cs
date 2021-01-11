using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Protocols
{
    public class ProtocolContactManualTableValueModelMapper : BaseModelMapper<ProtocolTableValuedModel, ProtocolContactManualModel>, IProtocolContactManualTableValueModelMapper
    {

        public override ProtocolContactManualModel Map(ProtocolTableValuedModel entity, ProtocolContactManualModel entityTransformed)
        {
            entityTransformed = null;
            if (entity.ProtocolContactManual_Incremental.HasValue)
            {
                entityTransformed = new ProtocolContactManualModel
                {
                    Incremental = entity.ProtocolContactManual_Incremental.Value,
                    Description = entity.ProtocolContactManual_Description
                };
            }

            return entityTransformed;
        }

    }
}
