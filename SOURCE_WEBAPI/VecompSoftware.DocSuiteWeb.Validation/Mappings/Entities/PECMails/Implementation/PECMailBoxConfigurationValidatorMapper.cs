using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails
{
    public class PECMailBoxConfigurationValidatorMapper : BaseMapper<PECMailBoxConfiguration, PECMailBoxConfigurationValidator>, IPECMailBoxConfigurationValidatorMapper
    {
        public PECMailBoxConfigurationValidatorMapper() { }

        public override PECMailBoxConfigurationValidator Map(PECMailBoxConfiguration entity,
            PECMailBoxConfigurationValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.Name = entity.Name;
            entityTransformed.MaxReadForSession = entity.MaxReadForSession;
            entityTransformed.MaxSendForSession = entity.MaxSendForSession;
            entityTransformed.UnzipAttachments = entity.UnzipAttachments;
            entityTransformed.SslPort = entity.SslPort;
            entityTransformed.ImapEnabled = entity.ImapEnabled;
            entityTransformed.UseImapSsl = entity.UseImapSsl;
            entityTransformed.ImapPort = entity.ImapPort;
            entityTransformed.MarkAsRead = entity.MarkAsRead;
            entityTransformed.MoveToFolder = entity.MoveToFolder;
            entityTransformed.MoveErrorToFolder = entity.MoveErrorToFolder;
            entityTransformed.InboxFolder = entity.InboxFolder;
            entityTransformed.UploadSent = entity.UploadSent;
            entityTransformed.FolderSent = entity.FolderSent;
            entityTransformed.ImapSearchFlag = entity.ImapSearchFlag;
            entityTransformed.ImapStartDate = entity.ImapStartDate;
            entityTransformed.ImapEndDate = entity.ImapEndDate;
            entityTransformed.NoSubjectDefaultText = entity.NoSubjectDefaultText;
            entityTransformed.DeleteMailFromServer = entity.DeleteMailFromServer;
            entityTransformed.ReceiveDaysCap = entity.ReceiveDaysCap;
            entityTransformed.MaxReceiveByteSize = entity.MaxReceiveByteSize;
            entityTransformed.MaxSendByteSize = entity.MaxSendByteSize;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }
    }
}
