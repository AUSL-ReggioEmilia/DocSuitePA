using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails
{
    public class PECMailBoxValidatorMapper : BaseMapper<PECMailBox, PECMailBoxValidator>, IPECMailBoxValidatorMapper
    {
        public PECMailBoxValidatorMapper() { }

        public override PECMailBoxValidator Map(PECMailBox entity, PECMailBoxValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityShortId = entity.EntityShortId;
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
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.InvoiceType = entity.InvoiceType;
            entityTransformed.HumanEnabled = entity.HumanEnabled;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Location = entity.Location;
            entityTransformed.PECMails = entity.PECMails;
            entityTransformed.OChartItems = entity.OChartItems;
            entityTransformed.RulesetDefinition = entity.RulesetDefinition;
            #endregion



            return entityTransformed;
        }

    }
}
