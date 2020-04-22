/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Workflows/WorkflowInstanceService", "App/Services/Workflows/WorkflowActivityService", "App/Services/Workflows/WorkflowInstancelogService", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, WorkflowInstanceService, WorkflowActivityService, WorkflowInstanceLogService, ServiceConfigurationHelper) {
    var WorkflowInstancesBase = /** @class */ (function () {
        function WorkflowInstancesBase(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
        }
        WorkflowInstancesBase.prototype.initialize = function () {
            var workflowInstanceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, WorkflowInstancesBase.WorkflowInstance_TYPE_NAME);
            var workflowActivityConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, WorkflowInstancesBase.WorkflowActivity_TYPE_NAME);
            var workflowInstanceLogConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, WorkflowInstancesBase.WorkflowInstanceLog_TYPE_NAME);
            this.workflowInstanceService = new WorkflowInstanceService(workflowInstanceConfiguration);
            this.workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);
            this.workflowInstanceLogService = new WorkflowInstanceLogService(workflowInstanceLogConfiguration);
        };
        WorkflowInstancesBase.WorkflowInstance_TYPE_NAME = "WorkflowInstance";
        WorkflowInstancesBase.WorkflowInstanceLog_TYPE_NAME = "WorkflowInstanceLog";
        WorkflowInstancesBase.WorkflowActivity_TYPE_NAME = "WorkflowActivity";
        return WorkflowInstancesBase;
    }());
    return WorkflowInstancesBase;
});
//# sourceMappingURL=WorkflowInstancesBase.js.map