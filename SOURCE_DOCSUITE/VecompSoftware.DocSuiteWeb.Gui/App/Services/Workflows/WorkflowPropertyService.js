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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Workflows/WorkflowPropertyModelMapper"], function (require, exports, BaseService, WorkflowPropertyModelMapper) {
    var WorkflowPropertyService = /** @class */ (function (_super) {
        __extends(WorkflowPropertyService, _super);
        function WorkflowPropertyService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            _this._mapper = new WorkflowPropertyModelMapper();
            return _this;
        }
        WorkflowPropertyService.prototype.updateProperty = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        WorkflowPropertyService.prototype.getPropertiesFromActivity = function (idWorkflowActivity, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl;
            var data = "$filter=WorkflowActivity/UniqueId eq ".concat(idWorkflowActivity);
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var workflowPropertiesModel = [];
                    if (response) {
                        workflowPropertiesModel = _this._mapper.MapCollection(response.value);
                    }
                    callback(workflowPropertiesModel);
                }
            });
        };
        return WorkflowPropertyService;
    }(BaseService));
    return WorkflowPropertyService;
});
//# sourceMappingURL=WorkflowPropertyService.js.map