using DSW = VecompSoftware.DocSuiteWeb.Data;
using APIPECMail = VecompSoftware.DocSuiteWeb.Entity.PECMails;
using NHibernate;
using System;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.PECMails
{
    public class MapperPECMailBoxEntity : BaseEntityMapper<DSW.PECMailBox, APIPECMail.PECMailBox>
    {
        #region [ Fields ]
        private readonly MapperLocationEntity _mapperLocation;
        #endregion

        #region [ Constructor ]
        public MapperPECMailBoxEntity()
        {
            _mapperLocation = new MapperLocationEntity();
        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.PECMailBox, DSW.PECMailBox> MappingProjection(IQueryOver<DSW.PECMailBox, DSW.PECMailBox> queryOver)
        {
            throw new System.NotImplementedException();
        }

        protected override APIPECMail.PECMailBox TransformDTO(DSW.PECMailBox entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare PECMailBox se l'entità non è inizializzata");

            APIPECMail.PECMailBox apiPECMailBox = new APIPECMail.PECMailBox();
            apiPECMailBox.EntityId = entity.Id;
            apiPECMailBox.MailBoxRecipient = entity.MailBoxName;
            apiPECMailBox.IdJeepServiceIncomingHost = entity.IdJeepServiceIncomingHost;
            apiPECMailBox.IdJeepServiceOutgoingHost = entity.IdJeepServiceOutgoingHost;
            apiPECMailBox.IncomingServer = entity.IncomingServerName;
            apiPECMailBox.IncomingServerPort = entity.IncomingServerPort;
            apiPECMailBox.IncomingServerProtocol = (int?)entity.IncomingServerProtocol;
            apiPECMailBox.IncomingServerUseSsl = entity.IncomingServerUseSsl;
            apiPECMailBox.IsDestinationEnabled = entity.IsDestinationEnabled;
            apiPECMailBox.IsForInterop = entity.IsForInterop;
            apiPECMailBox.IsHandleEnabled = entity.IsHandleEnabled;
            apiPECMailBox.IsProtocolBox = entity.IsProtocolBox;
            apiPECMailBox.IsProtocolBoxExplicit = entity.IsProtocolBoxExplicit;
            apiPECMailBox.Location = entity.Location != null ? _mapperLocation.MappingDTO(entity.Location) : null;
            apiPECMailBox.Managed = entity.Managed;
            apiPECMailBox.OutgoingServer = entity.OutgoingServerName;
            apiPECMailBox.OutgoingServerPort = entity.OutgoingServerPort;
            apiPECMailBox.OutgoingServerUseSsl = entity.OutgoingServerUseSsl;
            apiPECMailBox.Password = entity.Password;
            apiPECMailBox.Timestamp = entity.Timestamp;
            apiPECMailBox.UniqueId = entity.UniqueId;
            apiPECMailBox.Unmanaged = entity.Unmanaged;
            apiPECMailBox.Username = entity.Username;

            return apiPECMailBox;
        }
        #endregion
    }
}
