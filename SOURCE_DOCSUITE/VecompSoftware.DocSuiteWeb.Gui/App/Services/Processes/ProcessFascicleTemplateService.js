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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Processes/ProcessFascicleTemplateModelMapper"], function (require, exports, BaseService, ProcessFascicleTemplateModelMapper) {
    var ProcessFascicleTemplateService = /** @class */ (function (_super) {
        __extends(ProcessFascicleTemplateService, _super);
        function ProcessFascicleTemplateService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        ProcessFascicleTemplateService.prototype.getAll = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper = new ProcessFascicleTemplateModelMapper();
                    var processFascicleTemplates = [];
                    for (var _i = 0, _a = response.value; _i < _a.length; _i++) {
                        var value = _a[_i];
                        processFascicleTemplates.push(modelMapper.Map(value));
                    }
                    callback(processFascicleTemplates);
                }
            }, error);
        };
        ProcessFascicleTemplateService.prototype.getById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl + "?$filter=UniqueId eq " + uniqueId;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper = new ProcessFascicleTemplateModelMapper();
                    var processFascicleTemplate = modelMapper.Map(response.value[0]);
                    callback(processFascicleTemplate);
                }
            }, error);
        };
        ProcessFascicleTemplateService.prototype.getFascicleTemplateByDossierFolderId = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "?$expand=DossierFolder&$filter=DossierFolder/UniqueId eq " + uniqueId;
            this.getRequest(url, data, function (response) {
                if (callback)
                    callback(response.value);
            }, error);
        };
        ProcessFascicleTemplateService.prototype.insert = function (processFascicleTemplate, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(processFascicleTemplate), callback, error);
        };
        ProcessFascicleTemplateService.prototype.update = function (processFascicleTemplate, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(processFascicleTemplate), callback, error);
        };
        ProcessFascicleTemplateService.prototype.delete = function (processFascicleTemplate, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(processFascicleTemplate), callback, error);
        };
        return ProcessFascicleTemplateService;
    }(BaseService));
    return ProcessFascicleTemplateService;
});
//# sourceMappingURL=ProcessFascicleTemplateService.js.map