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
define(["require", "exports", "App/Services/BaseService"], function (require, exports, BaseService) {
    var ProtocolService = /** @class */ (function (_super) {
        __extends(ProtocolService, _super);
        /**
         * Costruttore
         * @param webApiUrl
         */
        function ProtocolService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /**
         * Recupera un protocollo per UniqueId
         * @param uniqueId
         * @param callback
         * @param error
         */
        ProtocolService.prototype.getProtocolByUniqueId = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq ".concat(uniqueId.toString(), "&$expand=Category,Container");
            this.getRequest(url, data, function (response) {
                if (callback)
                    callback(response.value[0]);
            }, error);
        };
        ProtocolService.prototype.getDocumentSerieslByUniqueId = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=UniqueId eq ".concat(uniqueId.toString(), "&$expand=documentseriesitems");
            this.getRequest(url, data, function (response) {
                if (callback)
                    callback(response.value[0]);
            }, error);
        };
        ProtocolService.prototype.getProtocolMessageById = function (Id, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=Messages($expand=MessageContacts, MessageEmails)&$filter=UniqueId eq ".concat(Id, "&$select=Messages");
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        ProtocolService.prototype.countDocumentSeriesItemProtocolById = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DocumentSeriesItems/any(d: d/UniqueId eq " + documentUnitId + ")";
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        return ProtocolService;
    }(BaseService));
    return ProtocolService;
});
//# sourceMappingURL=ProtocolService.js.map