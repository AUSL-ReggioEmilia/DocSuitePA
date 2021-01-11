using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Protocols
{
    public class ProtocolParerTableValuedModelMapper : BaseModelMapper<ProtocolTableValuedModel, ProtocolParerModel>, IProtocolParerTableValuedModelMapper
    {

        public override ProtocolParerModel Map(ProtocolTableValuedModel entity, ProtocolParerModel entityTransformed)
        {
            entityTransformed = null;
            /*if(entity.ProtocolParer_Year.HasValue && entity.ProtocolParer_Number.HasValue)
            {
                entityTransformed = new ProtocolParerModel();
                entityTransformed.HasError = entity.ProtocolParer_HasError;
                entityTransformed.LastError = entity.ProtocolParer_LastError;
                entityTransformed.ParerUri = entity.ProtocolParer_ParerUri;
            }   */

            return entityTransformed;
        }

    }
}
