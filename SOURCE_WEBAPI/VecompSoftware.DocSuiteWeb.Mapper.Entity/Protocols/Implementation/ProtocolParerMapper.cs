using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolParerMapper : BaseEntityMapper<ProtocolParer, ProtocolParer>, IProtocolParerMapper
    {
        public override ProtocolParer Map(ProtocolParer entity, ProtocolParer entityTransformed)
        {
            #region [ Base ]
            //entityTransformed.Year = entity.Year;
            //entityTransformed.Number = entity.Number;
            entityTransformed.ArchiviedDate = entity.ArchiviedDate;
            entityTransformed.IsForArchive = entity.IsForArchive;
            entityTransformed.ParerUri = entity.ParerUri;
            entityTransformed.HasError = entity.HasError;
            entityTransformed.LastError = entity.LastError;
            entityTransformed.LastSendDate = entity.LastSendDate;

            #endregion

            return entityTransformed;
        }

    }
}
