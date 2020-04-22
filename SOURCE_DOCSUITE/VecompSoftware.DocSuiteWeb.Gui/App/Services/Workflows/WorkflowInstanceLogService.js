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
define(["require", "exports", "App/Services/BaseService", "App/Models/ODATAResponseModel", "App/Mappers/Workflows/WorkflowInstanceLogViewModelMapper"], function (require, exports, BaseService, ODATAResponseModel, WorkflowInstanceLogViewModelMapper) {
    var WorkflowInstanceLogService = /** @class */ (function (_super) {
        __extends(WorkflowInstanceLogService, _super);
        /**
         * costruttore
         * @param configuration
         */
        function WorkflowInstanceLogService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            _this._mapper = new WorkflowInstanceLogViewModelMapper();
            return _this;
        }
        WorkflowInstanceLogService.prototype.getFascicleInstanceLogs = function (idFascicle, skip, top, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=Entity/Fascicles/any(d:d/UniqueId eq ".concat(idFascicle, ")&$orderby=RegistrationDate desc &$skip=", skip.toString(), "&$top=", top.toString(), "&$count=true");
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    if (response) {
                        responseModel.value = _this._mapper.MapCollection(response.value);
                    }
                    callback(responseModel);
                }
            });
        };
        WorkflowInstanceLogService.prototype.getWorkflowInstanceLogs = function (workflowInstanceId, callback, error) {
            var _this = this;
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=Entity/UniqueId eq " + workflowInstanceId + "&$orderby=RegistrationDate desc";
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var responseModel = new ODATAResponseModel(response);
                    if (response) {
                        responseModel.value = _this._mapper.MapCollection(response.value);
                    }
                    callback(responseModel);
                }
            });
        };
        return WorkflowInstanceLogService;
    }(BaseService));
    return WorkflowInstanceLogService;
});
//# sourceMappingURL=WorkflowInstanceLogService.js.map