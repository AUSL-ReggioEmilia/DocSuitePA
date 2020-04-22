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
    var TemplateReportService = /** @class */ (function (_super) {
        __extends(TemplateReportService, _super);
        /**
         * Costruttore
         */
        function TemplateReportService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        TemplateReportService.prototype.getById = function (templateId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=UniqueId eq ".concat(templateId);
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    callback(response.value[0]);
                }
            }, error);
        };
        TemplateReportService.prototype.find = function (name, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs;
            if (name) {
                qs = "$filter=contains(Name, '".concat(name, "')");
            }
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        /**
         * Inserisce un nuovo report template
         * @param model
         * @param callback
         * @param error
         */
        TemplateReportService.prototype.insertTemplateReport = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Modifica un report template esistente
         * @param model
         * @param callback
         * @param error
         */
        TemplateReportService.prototype.updateTemplateReport = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Cancellazione di un report template esistente
         * @param model
         * @param callback
         * @param error
         */
        TemplateReportService.prototype.deleteTemplateReport = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        return TemplateReportService;
    }(BaseService));
    return TemplateReportService;
});
//# sourceMappingURL=TemplateReportService.js.map