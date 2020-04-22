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
    var DocumentSeriesService = /** @class */ (function (_super) {
        __extends(DocumentSeriesService, _super);
        /**
         * Costruttore
         * @param webApiUrl
         */
        function DocumentSeriesService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        DocumentSeriesService.prototype.getAll = function (callback, error) {
            this.getRequest(this._configuration.ODATAUrl, "$orderby=Name", function (response) {
                if (callback && response) {
                    var documentSeries = response.value;
                    callback(documentSeries);
                }
            }, error);
        };
        DocumentSeriesService.prototype.getById = function (idDocumentSeries, callback, error) {
            var qs = "$filter=EntityId eq ".concat(idDocumentSeries.toString());
            this.getRequest(this._configuration.ODATAUrl, qs, function (response) {
                if (callback && response) {
                    var documentSeries = response.value;
                    callback(documentSeries[0]);
                }
            }, error);
        };
        return DocumentSeriesService;
    }(BaseService));
    return DocumentSeriesService;
});
//# sourceMappingURL=DocumentSeriesService.js.map