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
    var DocumentSeriesConstraintService = /** @class */ (function (_super) {
        __extends(DocumentSeriesConstraintService, _super);
        /**
         * Costruttore
         * @param webApiUrl
         */
        function DocumentSeriesConstraintService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        DocumentSeriesConstraintService.prototype.getByIdSeries = function (idSeries, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$orderby=Name&$filter=DocumentSeries/EntityId eq ".concat(idSeries.toString());
            this.getRequest(url, data, function (response) {
                if (callback && response) {
                    var constraints = response.value;
                    callback(constraints);
                }
            }, error);
        };
        DocumentSeriesConstraintService.prototype.insertDocumentSeriesConstraint = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        DocumentSeriesConstraintService.prototype.updateDocumentSeriesConstraint = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        DocumentSeriesConstraintService.prototype.deleteDocumentSeriesConstraint = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        return DocumentSeriesConstraintService;
    }(BaseService));
    return DocumentSeriesConstraintService;
});
//# sourceMappingURL=DocumentSeriesConstraintService.js.map