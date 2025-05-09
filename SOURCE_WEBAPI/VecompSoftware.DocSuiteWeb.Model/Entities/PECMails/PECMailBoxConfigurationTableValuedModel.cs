﻿using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.PECMails
{
    public class PECMailBoxConfigurationTableValuedModel
    {
        #region [ Constructor ]
        public PECMailBoxConfigurationTableValuedModel()
        {
        }
        #endregion

        #region [ Properties ]
        public int EntityId { get; set; }
        public string Name { get; set; }
        public int? MaxReadForSession { get; set; }
        public int? MaxSendForSession { get; set; }
        public bool? UnzipAttachments { get; set; }
        public int? SslPort { get; set; }
        public bool? ImapEnabled { get; set; }
        public bool? UseImapSsl { get; set; }
        public int? ImapPort { get; set; }
        public bool? MarkAsRead { get; set; }
        public string MoveToFolder { get; set; }
        public string MoveErrorToFolder { get; set; }
        public string InboxFolder { get; set; }
        public bool? UploadSent { get; set; }
        public string FolderSent { get; set; }
        public int? ImapSearchFlag { get; set; }
        public DateTime? ImapStartDate { get; set; }
        public DateTime? ImapEndDate { get; set; }
        public string NoSubjectDefaultText { get; set; }
        public bool? DeleteMailFromServer { get; set; }
        public short? ReceiveDaysCap { get; set; }
        public long? MaxReceiveByteSize { get; set; }
        public long? MaxSendByteSize { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
