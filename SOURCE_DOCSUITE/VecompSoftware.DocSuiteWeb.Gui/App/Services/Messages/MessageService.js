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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Messages/MessageModelMapper"], function (require, exports, BaseService, MessagesModelMapper) {
    var MessageService = /** @class */ (function (_super) {
        __extends(MessageService, _super);
        function MessageService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        MessageService.prototype.countProtocolMessagesById = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=Protocols/any(d: d/UniqueId eq " + documentUnitId + ")";
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        MessageService.prototype.getProtocolMessagesByShortId = function (messageId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=MessageContacts,MessageEmails&$filter=EntityId eq " + messageId;
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new MessagesModelMapper();
                    var instance = {};
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        MessageService.prototype.countDocumentSeriesItemById = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DocumentSeriesItems/any(d: d/UniqueId eq " + documentUnitId + ")";
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        MessageService.prototype.countResolutionMessagesById = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=Resolutions/any(d: d/UniqueId eq " + documentUnitId + ")";
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        return MessageService;
    }(BaseService));
    return MessageService;
});
//# sourceMappingURL=MessageService.js.map