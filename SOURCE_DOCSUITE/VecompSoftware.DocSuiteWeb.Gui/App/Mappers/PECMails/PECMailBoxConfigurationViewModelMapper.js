define(["require", "exports"], function (require, exports) {
    var PECMailBoxConfigurationViewModelMapper = /** @class */ (function () {
        function PECMailBoxConfigurationViewModelMapper() {
        }
        PECMailBoxConfigurationViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
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
        };
        return PECMailBoxConfigurationViewModelMapper;
    }());
    return PECMailBoxConfigurationViewModelMapper;
});
//# sourceMappingURL=PECMailBoxConfigurationViewModelMapper.js.map