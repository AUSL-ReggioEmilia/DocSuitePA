using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolContactMapper : BaseEntityMapper<ProtocolContact, ProtocolContact>, IProtocolContactMapper
    {
        public override ProtocolContact Map(ProtocolContact entity, ProtocolContact entityTransformed)
        {
            #region [ Base ]

            //entityTransformed.Year = entity.Year;
            //entityTransformed.Number = entity.Number;
            entityTransformed.ComunicationType = entity.ComunicationType;
            entityTransformed.Type = entity.Type;

            #endregion

            return entityTransformed;
        }

    }
}
