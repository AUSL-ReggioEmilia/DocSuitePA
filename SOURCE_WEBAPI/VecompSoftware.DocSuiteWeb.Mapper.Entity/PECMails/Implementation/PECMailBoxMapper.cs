using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.PECMails
{
    public class PECMailBoxMapper : BaseEntityMapper<PECMailBox, PECMailBox>, IPECMailBoxMapper
    {
        public override PECMailBox Map(PECMailBox entity, PECMailBox entityTransformed)
        {
            #region [ Base ]
            entityTransformed.MailBoxRecipient = entity.MailBoxRecipient;
            entityTransformed.IncomingServer = entity.IncomingServer;
            entityTransformed.IncomingServerProtocol = entity.IncomingServerProtocol;
            entityTransformed.IncomingServerPort = entity.IncomingServerPort;
            entityTransformed.IncomingServerUseSsl = entity.IncomingServerUseSsl;
            entityTransformed.OutgoingServer = entity.OutgoingServer;
            entityTransformed.OutgoingServerPort = entity.OutgoingServerPort;
            entityTransformed.OutgoingServerUseSsl = entity.OutgoingServerUseSsl;
            entityTransformed.Username = entity.Username;
            entityTransformed.Password = entity.Password;
            entityTransformed.Managed = entity.Managed;
            entityTransformed.Unmanaged = entity.Unmanaged;
            entityTransformed.IsDestinationEnabled = entity.IsDestinationEnabled;
            entityTransformed.IsForInterop = entity.IsForInterop;
            entityTransformed.IdConfiguration = entity.IdConfiguration;
            entityTransformed.DeleteMailFromServer = entity.DeleteMailFromServer;
            entityTransformed.ReceiveDaysCap = entity.ReceiveDaysCap;
            entityTransformed.RedirectAnomaliesSMTP = entity.RedirectAnomaliesSMTP;
            entityTransformed.RedirectAnomaliesUsername = entity.RedirectAnomaliesUsername;
            entityTransformed.RedirectAnomaliesPassword = entity.RedirectStoragePassword;
            entityTransformed.RedirectAnomaliesRecipient = entity.RedirectAnomaliesRecipient;
            entityTransformed.RedirectStoragePassword = entity.RedirectStoragePassword;
            entityTransformed.RedirectStorageUsername = entity.RedirectStorageUsername;
            entityTransformed.RedirectStorageSMTP = entity.RedirectStorageSMTP;
            entityTransformed.RedirectStorageRecipient = entity.RedirectStorageRecipient;
            entityTransformed.IsHandleEnabled = entity.IsHandleEnabled;
            entityTransformed.IsProtocolBox = entity.IsProtocolBox;
            entityTransformed.IsProtocolBoxExplicit = entity.IsProtocolBoxExplicit;
            entityTransformed.IdJeepServiceIncomingHost = entity.IdJeepServiceIncomingHost;
            entityTransformed.IdJeepServiceOutgoingHost = entity.IdJeepServiceOutgoingHost;
            entityTransformed.RulesetDefinition = entity.RulesetDefinition;
            entityTransformed.InvoiceType = entity.InvoiceType;
            entityTransformed.HumanEnabled = entity.HumanEnabled;
            #endregion

            return entityTransformed;
        }

    }
}
