using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.PECMails
{
    public class PECMailBoxConfigurationTableValuedModelMapper : BaseModelMapper<PECMailBoxConfigurationTableValuedModel, PECMailBoxConfigurationModel>, IPECMailBoxConfigurationTableValuedModelMapper
    {
        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public PECMailBoxConfigurationTableValuedModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public override PECMailBoxConfigurationModel Map(PECMailBoxConfigurationTableValuedModel model,
            PECMailBoxConfigurationModel modelTransformed)
        {
            modelTransformed.EntityId = model.EntityId;
            modelTransformed.Name = model.Name;
            modelTransformed.MaxReadForSession = model.MaxReadForSession;
            modelTransformed.MaxSendForSession = model.MaxSendForSession;
            modelTransformed.UnzipAttachments = model.UnzipAttachments;
            modelTransformed.SslPort = model.SslPort;
            modelTransformed.ImapEnabled = model.ImapEnabled;
            modelTransformed.UseImapSsl = model.UseImapSsl;
            modelTransformed.ImapPort = model.ImapPort;
            modelTransformed.MarkAsRead = model.MarkAsRead;
            modelTransformed.MoveToFolder = model.MoveToFolder;
            modelTransformed.MoveErrorToFolder = model.MoveErrorToFolder;
            modelTransformed.InboxFolder = model.InboxFolder;
            modelTransformed.UploadSent = model.UploadSent;
            modelTransformed.FolderSent = model.FolderSent;
            modelTransformed.ImapSearchFlag = model.ImapSearchFlag;
            modelTransformed.ImapStartDate = model.ImapStartDate;
            modelTransformed.ImapEndDate = model.ImapEndDate;
            modelTransformed.NoSubjectDefaultText = model.NoSubjectDefaultText;
            modelTransformed.DeleteMailFromServer = model.DeleteMailFromServer;
            modelTransformed.ReceiveDaysCap = model.ReceiveDaysCap;
            modelTransformed.MaxReceiveByteSize = model.MaxReceiveByteSize;
            modelTransformed.MaxSendByteSize = model.MaxSendByteSize;

            return modelTransformed;
        }

        public override ICollection<PECMailBoxConfigurationModel> MapCollection(ICollection<PECMailBoxConfigurationTableValuedModel> model)
        {
            if (model == null)
            {
                return new List<PECMailBoxConfigurationModel>();
            }
            List<PECMailBoxConfigurationModel> modelsTransformed = new List<PECMailBoxConfigurationModel>();
            PECMailBoxConfigurationModel modelTransformed = null;
            foreach (IGrouping<int, PECMailBoxConfigurationTableValuedModel> pecMailBoxLookup in model.ToLookup(x => x.EntityId))
            {
                modelTransformed = Map(pecMailBoxLookup.First(), new PECMailBoxConfigurationModel());
                modelsTransformed.Add(modelTransformed);
            }
            return modelsTransformed;
        }
    }
}
