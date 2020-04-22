/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Workflows/WorkflowRepositoryService", "App/Helpers/ServiceConfigurationHelper", "App/Services/Workflows/WorkflowStartService"], function (require, exports, WorkflowRepositoryService, ServiceConfigurationHelper, WorkflowStartService) {
    var WorkflowActivityInsertBase = /** @class */ (function () {
        function WorkflowActivityInsertBase(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
        }
        WorkflowActivityInsertBase.prototype.initialize = function () {
            var workflowRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, WorkflowActivityInsertBase.WorkflowRepository_TYPE_NAME);
            var workflowStartConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, WorkflowActivityInsertBase.WORKFLOWSTART_TYPE_NAME);
            this.workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);
            this.workflowStartService = new WorkflowStartService(workflowStartConfiguration);
        };
        WorkflowActivityInsertBase.WorkflowRepository_TYPE_NAME = "WorkflowRepository";
        WorkflowActivityInsertBase.WORKFLOWSTART_TYPE_NAME = "WorkflowStart";
        return WorkflowActivityInsertBase;
    }());
    return WorkflowActivityInsertBase;
});
//# sourceMappingURL=WorkflowActivityInsertBase.js.map