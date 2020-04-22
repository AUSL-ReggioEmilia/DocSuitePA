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
    var ResolutionDocumentSeriesItemService = /** @class */ (function (_super) {
        __extends(ResolutionDocumentSeriesItemService, _super);
        /**
         * Costruttore
         * @param webApiUrl
         */
        function ResolutionDocumentSeriesItemService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        ResolutionDocumentSeriesItemService.prototype.getResolutionDocumentSeriesItemLinksCount = function (idDocumentUnit, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DocumentSeriesItem/Status ne 'Draft' and Resolution/UniqueId eq " + idDocumentUnit;
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        ResolutionDocumentSeriesItemService.prototype.getResolutionDocumentSeriesItemLinks = function (idDocumentUnit, callback, error) {
            var qs = "$expand=DocumentSeriesItem($expand=DocumentSeries)&$filter=DocumentSeriesItem/Status ne 'Draft' and Resolution/UniqueId eq " + idDocumentUnit;
            this.getRequest(this._configuration.ODATAUrl, qs, function (response) {
                if (callback && response) {
                    var resolutionDocumentSeriesItem = response.value;
                    callback(resolutionDocumentSeriesItem);
                }
            }, error);
        };
        return ResolutionDocumentSeriesItemService;
    }(BaseService));
    return ResolutionDocumentSeriesItemService;
});
//# sourceMappingURL=ResolutionDocumentSeriesItemService.js.map