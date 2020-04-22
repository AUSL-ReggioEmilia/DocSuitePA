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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Workflows/WorkflowEvaluationPropertyModelMapper"], function (require, exports, BaseService, WorkflowEvaluationPropertyModelMapper) {
    var WorkflowEvaluationPropertyService = /** @class */ (function (_super) {
        __extends(WorkflowEvaluationPropertyService, _super);
        function WorkflowEvaluationPropertyService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            _this._mapper = new WorkflowEvaluationPropertyModelMapper();
            return _this;
        }
        WorkflowEvaluationPropertyService.prototype.insertWorkflowEvaluationProperty = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        WorkflowEvaluationPropertyService.prototype.getWorkflowEvaluationProperty = function (idWorkflowEvaluationProperty, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$filter=UniqueId eq ", idWorkflowEvaluationProperty);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var instance = {};
                    var mapper = new WorkflowEvaluationPropertyModelMapper();
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        WorkflowEvaluationPropertyService.prototype.updateWorkflowEvaluationProperty = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        WorkflowEvaluationPropertyService.prototype.deleteWorkflowEvaluationProperty = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        return WorkflowEvaluationPropertyService;
    }(BaseService));
    return WorkflowEvaluationPropertyService;
});
//# sourceMappingURL=WorkflowEvaluationPropertyService.js.map