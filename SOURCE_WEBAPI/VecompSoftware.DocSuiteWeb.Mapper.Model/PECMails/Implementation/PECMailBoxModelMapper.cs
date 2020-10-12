using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;
using InvoiceType = VecompSoftware.DocSuiteWeb.Model.Entities.PECMails.InvoiceType;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.PECMails
{
    public class PECMailBoxModelMapper : BaseModelMapper<PECMailBox, PECMailBoxModel>, IPECMailBoxModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public PECMailBoxModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #region [ Methods ]
        public override PECMailBoxModel Map(PECMailBox entity, PECMailBoxModel modelTransformed)
        {
            modelTransformed.MailBoxRecipient = entity.MailBoxRecipient;
            modelTransformed.IncomingServer = entity.IncomingServer;
            modelTransformed.IncomingServerProtocol = entity.IncomingServerProtocol;
            modelTransformed.IncomingServerPort = entity.IncomingServerPort;
            modelTransformed.IncomingServerUseSsl = entity.IncomingServerUseSsl;
            modelTransformed.OutgoingServer = entity.OutgoingServer;
            modelTransformed.OutgoingServerPort = entity.OutgoingServerPort;
            modelTransformed.OutgoingServerUseSsl = entity.OutgoingServerUseSsl;
            modelTransformed.Username = entity.Username;
            modelTransformed.Password = entity.Password;
            modelTransformed.Managed = entity.Managed;
            modelTransformed.Unmanaged = entity.Unmanaged;
            modelTransformed.IsDestinationEnabled = entity.IsDestinationEnabled;
            modelTransformed.IsForInterop = entity.IsForInterop;
            modelTransformed.IdConfiguration = entity.IdConfiguration;
            modelTransformed.DeleteMailFromServer = entity.DeleteMailFromServer;
            modelTransformed.ReceiveDaysCap = entity.ReceiveDaysCap;
            modelTransformed.RedirectAnomaliesSMTP = entity.RedirectAnomaliesSMTP;
            modelTransformed.RedirectAnomaliesUsername = entity.RedirectAnomaliesUsername;
            modelTransformed.RedirectAnomaliesPassword = entity.RedirectStoragePassword;
            modelTransformed.RedirectAnomaliesRecipient = entity.RedirectAnomaliesRecipient;
            modelTransformed.RedirectStoragePassword = entity.RedirectStoragePassword;
            modelTransformed.RedirectStorageUsername = entity.RedirectStorageUsername;
            modelTransformed.RedirectStorageSMTP = entity.RedirectStorageSMTP;
            modelTransformed.RedirectStorageRecipient = entity.RedirectStorageRecipient;
            modelTransformed.IsHandleEnabled = entity.IsHandleEnabled;
            modelTransformed.IsProtocolBox = entity.IsProtocolBox;
            modelTransformed.IsProtocolBoxExplicit = entity.IsProtocolBoxExplicit;
            modelTransformed.IdJeepServiceIncomingHost = entity.IdJeepServiceIncomingHost;
            modelTransformed.IdJeepServiceOutgoingHost = entity.IdJeepServiceOutgoingHost;
            modelTransformed.RulesetDefinition = entity.RulesetDefinition;
            modelTransformed.InvoiceType = (InvoiceType?)entity.InvoiceType;
            modelTransformed.LoginError = entity.LoginError;

            return modelTransformed;
        }
        #endregion
    }
}
