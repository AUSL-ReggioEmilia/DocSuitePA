var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Mappers/BaseMapper"], function (require, exports, BaseMapper) {
    var PECMailBoxViewModelMapper = /** @class */ (function (_super) {
        __extends(PECMailBoxViewModelMapper, _super);
        function PECMailBoxViewModelMapper() {
            return _super.call(this) || this;
        }
        PECMailBoxViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.EntityShortId = source.EntityShortId;
            toMap.MailBoxRecipient = source.MailBoxRecipient;
            toMap.IncomingServer = source.IncomingServer;
            toMap.OutgoingServer = source.OutgoingServer;
            toMap.Username = source.Username;
            toMap.Password = source.Password;
            toMap.RedirectAnomaliesSMTP = source.RedirectAnomaliesSMTP;
            toMap.RedirectAnomaliesUsername = source.RedirectAnomaliesUsername;
            toMap.RedirectAnomaliesPassword = source.RedirectAnomaliesPassword;
            toMap.RedirectAnomaliesRecipient = source.RedirectAnomaliesRecipient;
            toMap.RedirectStorageSMTP = source.RedirectStorageSMTP;
            toMap.RedirectStorageUsername = source.RedirectStorageUsername;
            toMap.RedirectStoragePassword = source.RedirectStoragePassword;
            toMap.RedirectStorageRecipient = source.RedirectStorageRecipient;
            toMap.IsForInterop = source.IsForInterop;
            toMap.IsDestinationEnabled = source.IsDestinationEnabled;
            toMap.DeleteMailFromServer = source.DeleteMailFromServer;
            toMap.ReceiveDaysCap = source.ReceiveDaysCap;
            toMap.Managed = source.Managed;
            toMap.Unmanaged = source.Unmanaged;
            toMap.IdConfiguration = source.IdConfiguration;
            toMap.IncomingServerProtocol = source.IncomingServerProtocol;
            toMap.IncomingServerPort = source.IncomingServerPort;
            toMap.IncomingServerUseSsl = source.IncomingServerUseSsl;
            toMap.OutgoingServerPort = source.OutgoingServerPort;
            toMap.OutgoingServerUseSsl = source.OutgoingServerUseSsl;
            toMap.IsHandleEnabled = source.IsHandleEnabled;
            toMap.IsProtocolBox = source.IsProtocolBox;
            toMap.IsProtocolBoxExplicit = source.IsProtocolBoxExplicit;
            toMap.IdJeepServiceIncomingHost = source.IdJeepServiceIncomingHost;
            toMap.IdJeepServiceOutgoingHost = source.IdJeepServiceOutgoingHost;
            toMap.RulesetDefinition = source.RulesetDefinition;
            toMap.EntityId = source.EntityId;
            toMap.UniqueId = source.UniqueId;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.LastChangedDate = source.LastChangedDate;
            toMap.LoginError = source.LoginError;
            toMap.Location = source.Location;
            toMap.InvoiceType = source.InvoiceType;
            return toMap;
        };
        return PECMailBoxViewModelMapper;
    }(BaseMapper));
    return PECMailBoxViewModelMapper;
});
//# sourceMappingURL=PECMailBoxViewModelMapper.js.map