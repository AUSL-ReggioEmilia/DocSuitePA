using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.PECMails
{
    public class PECMailBoxConfigurationModelMapper : BaseModelMapper<PECMailBoxConfiguration, PECMailBoxConfigurationModel>, IPECMailBoxConfigurationModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public PECMailBoxConfigurationModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #region [ Methods ]

        public override PECMailBoxConfigurationModel Map(PECMailBoxConfiguration entity,
            PECMailBoxConfigurationModel modelTransformed)
        {
            modelTransformed.EntityId = entity.EntityId;
            modelTransformed.Name = entity.Name;
            modelTransformed.MaxReadForSession = entity.MaxReadForSession;
            modelTransformed.MaxSendForSession = entity.MaxSendForSession;
            modelTransformed.UnzipAttachments = entity.UnzipAttachments;
            modelTransformed.SslPort = entity.SslPort;
            modelTransformed.ImapEnabled = entity.ImapEnabled;
            modelTransformed.UseImapSsl = entity.UseImapSsl;
            modelTransformed.ImapPort = entity.ImapPort;
            modelTransformed.MarkAsRead = entity.MarkAsRead;
            modelTransformed.MoveToFolder = entity.MoveToFolder;
            modelTransformed.MoveErrorToFolder = entity.MoveErrorToFolder;
            modelTransformed.InboxFolder = entity.InboxFolder;
            modelTransformed.UploadSent = entity.UploadSent;
            modelTransformed.FolderSent = entity.FolderSent;
            modelTransformed.ImapSearchFlag = entity.ImapSearchFlag;
            modelTransformed.ImapStartDate = entity.ImapStartDate;
            modelTransformed.ImapEndDate = entity.ImapEndDate;
            modelTransformed.NoSubjectDefaultText = entity.NoSubjectDefaultText;
            modelTransformed.DeleteMailFromServer = entity.DeleteMailFromServer;
            modelTransformed.ReceiveDaysCap = entity.ReceiveDaysCap;
            modelTransformed.MaxReceiveByteSize = entity.MaxReceiveByteSize;
            modelTransformed.MaxSendByteSize = entity.MaxSendByteSize;

            return modelTransformed;
        }
        #endregion
    }
}
