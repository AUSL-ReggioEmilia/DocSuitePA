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
define(["require", "exports", "App/Services/BaseService", "../../Mappers/Series/DocumentSeriesItemLinksModelMapper"], function (require, exports, BaseService, DocumentSeriesItemLinksModelMapper) {
    var DocumentSeriesItemLinksService = /** @class */ (function (_super) {
        __extends(DocumentSeriesItemLinksService, _super);
        function DocumentSeriesItemLinksService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        DocumentSeriesItemLinksService.prototype.countDocumentSeriesItemLinksById = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DocumentSeriesItem/UniqueId eq " + documentUnitId;
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        DocumentSeriesItemLinksService.prototype.getDocumentSeriesItemLinksById = function (documentUnitId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=Resolution&$filter=DocumentSeriesItem/UniqueId eq " + documentUnitId;
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new DocumentSeriesItemLinksModelMapper();
                    var instance = {};
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        return DocumentSeriesItemLinksService;
    }(BaseService));
    return DocumentSeriesItemLinksService;
});
//# sourceMappingURL=DocumentSeriesItemLinksService.js.map