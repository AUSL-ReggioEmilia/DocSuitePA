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
define(["require", "exports", "App/Services/BaseService", "../../Mappers/Series/DocumentSeriesItemModelMapper"], function (require, exports, BaseService, DocumentSeriesItemModelMapper) {
    var DocumentSeriesItemService = /** @class */ (function (_super) {
        __extends(DocumentSeriesItemService, _super);
        function DocumentSeriesItemService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        DocumentSeriesItemService.prototype.countDocumentSeriesItemById = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=Protocols/any(d: d/UniqueId eq " + documentUnitId + ")";
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        DocumentSeriesItemService.prototype.getDocumentSeriesItemById = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=Messages($expand=MessageContacts,MessageEmails)&$filter=UniqueId eq " + documentUnitId;
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new DocumentSeriesItemModelMapper();
                    var instance = {};
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        DocumentSeriesItemService.prototype.getDocumentSeriesItemProtocolById = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=Protocols&$filter=UniqueId eq " + documentUnitId;
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new DocumentSeriesItemModelMapper();
                    var instance = {};
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        return DocumentSeriesItemService;
    }(BaseService));
    return DocumentSeriesItemService;
});
//# sourceMappingURL=DocumentSeriesItemService.js.map