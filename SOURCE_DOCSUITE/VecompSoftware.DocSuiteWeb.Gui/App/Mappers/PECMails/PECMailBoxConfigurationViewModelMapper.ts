import PECMailBoxConfigurationViewModel = require('App/ViewModels/PECMails/PECMailBoxConfigurationViewModel');
import IMapper = require('App/Mappers/IMapper');

class PECMailBoxConfigurationViewModelMapper implements IMapper<PECMailBoxConfigurationViewModel> {
  constructor() {
  }
  public Map(source: any): PECMailBoxConfigurationViewModel {
    let toMap: PECMailBoxConfigurationViewModel = <PECMailBoxConfigurationViewModel>{};

    if (!source) {
      return null;
    }

    toMap.EntityId = source.EntityId;
    toMap.Name = source.Name;
    toMap.MaxReadForSession = source.MaxReadForSession;
    toMap.MaxSendForSession = source.MaxSendForSession;
    toMap.UnzipAttachments = source.UnzipAttachments;
    toMap.SslPort = source.SslPort;
    toMap.ImapEnabled = source.ImapEnabled;
    toMap.UseImapSsl = source.UseImapSsl;
    toMap.ImapPort = source.ImapPort;
    toMap.MarkAsRead = source.MarkAsRead;
    toMap.MoveToFolder = source.MoveToFolder;
    toMap.MoveErrorToFolder = source.MoveErrorToFolder;
    toMap.InboxFolder = source.InboxFolder;
    toMap.UploadSent = source.UploadSent;
    toMap.FolderSent = source.FolderSent;
    toMap.ImapSearchFlag = source.ImapSearchFlag;
    toMap.ImapStartDate = source.ImapStartDate;
    toMap.ImapEndDate = source.ImapEndDate;
    toMap.NoSubjectDefaultText = source.NoSubjectDefaultText;
    toMap.DeleteMailFromServer = source.DeleteMailFromServer;
    toMap.ReceiveDaysCap = source.ReceiveDaysCap;
    toMap.MaxReceiveByteSize = source.MaxReceiveByteSize;
    toMap.MaxSendByteSize = source.MaxSendByteSize;

    return toMap;
  }
}

export = PECMailBoxConfigurationViewModelMapper;