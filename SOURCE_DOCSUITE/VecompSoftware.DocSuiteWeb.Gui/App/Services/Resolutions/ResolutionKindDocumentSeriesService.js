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
    var ResolutionKindDocumentSeriesService = /** @class */ (function (_super) {
        __extends(ResolutionKindDocumentSeriesService, _super);
        /**
         * Costruttore
         * @param webApiUrl
         */
        function ResolutionKindDocumentSeriesService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        ResolutionKindDocumentSeriesService.prototype.getByResolutionKind = function (idResolutionKind, callback, error) {
            var qs = "$filter=ResolutionKind/UniqueId eq ".concat(idResolutionKind, "&$expand=DocumentSeries,DocumentSeriesConstraint");
            this.getRequest(this._configuration.ODATAUrl, qs, function (response) {
                if (callback && response) {
                    var resolutionKinds = response.value;
                    callback(resolutionKinds);
                }
            }, error);
        };
        ResolutionKindDocumentSeriesService.prototype.insertResolutionKindDocumentSeriesModel = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        ResolutionKindDocumentSeriesService.prototype.updateResolutionKindDocumentSeriesModel = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        ResolutionKindDocumentSeriesService.prototype.deleteResolutionKindDocumentSeriesModel = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        return ResolutionKindDocumentSeriesService;
    }(BaseService));
    return ResolutionKindDocumentSeriesService;
});
//# sourceMappingURL=ResolutionKindDocumentSeriesService.js.map