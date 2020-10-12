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
define(["require", "exports", "App/Services/BaseService", "App/Models/Workflows/WorkflowStatus"], function (require, exports, BaseService, WorkflowStatus) {
    var WorkflowActivityLogService = /** @class */ (function (_super) {
        __extends(WorkflowActivityLogService, _super);
        /**
        * Costruttore
        */
        function WorkflowActivityLogService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        WorkflowActivityLogService.prototype.insertWorkflowActivityLog = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        WorkflowActivityLogService.prototype.countWorkflowActivityByLogType = function (id, workflowStatus, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=LogType eq '" + WorkflowStatus[workflowStatus] + "' and Entity/UniqueId eq " + id;
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        return WorkflowActivityLogService;
    }(BaseService));
    return WorkflowActivityLogService;
});
//# sourceMappingURL=WorkflowActivityLogService.js.map