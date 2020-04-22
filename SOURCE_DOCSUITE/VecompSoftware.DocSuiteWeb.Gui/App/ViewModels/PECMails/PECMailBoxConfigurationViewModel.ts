class PECMailBoxConfigurationViewModel {
  EntityId: number;
  Name: string;
  MaxReadForSession: number;
  MaxSendForSession: number;
  UnzipAttachments: boolean;
  SslPort: number;
  ImapEnabled: boolean;
  UseImapSsl: boolean;
  ImapPort: number;
  MarkAsRead: boolean;
  MoveToFolder: string;
  MoveErrorToFolder: string;
  InboxFolder: string;
  UploadSent: boolean;
  FolderSent: string;
  ImapSearchFlag: number;
  ImapStartDate: Date;
  ImapEndDate: Date;
  NoSubjectDefaultText: string;
  DeleteMailFromServer: boolean;
  ReceiveDaysCap: number;
  MaxReceiveByteSize: number;
  MaxSendByteSize: number;
}

export = PECMailBoxConfigurationViewModel;