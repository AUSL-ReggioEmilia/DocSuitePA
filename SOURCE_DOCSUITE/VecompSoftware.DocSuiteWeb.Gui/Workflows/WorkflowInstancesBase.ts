/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import WorkflowInstanceService = require('App/Services/Workflows/WorkflowInstanceService');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import WorkflowInstanceLogService = require('App/Services/Workflows/WorkflowInstancelogService')
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");

abstract class WorkflowInstancesBase {
    protected static WorkflowInstance_TYPE_NAME = "WorkflowInstance";
    protected static WorkflowInstanceLog_TYPE_NAME = "WorkflowInstanceLog";
    protected static WorkflowActivity_TYPE_NAME = "WorkflowActivity";

    protected _serviceConfigurations: ServiceConfiguration[];

    protected workflowInstanceService: WorkflowInstanceService;
    protected workflowActivityService: WorkflowActivityService;
    protected workflowInstanceLogService: WorkflowInstanceLogService;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize() {
        let workflowInstanceConfiguration: ServiceConfiguration
            = ServiceConfigurationHelper.getService(this._serviceConfigurations, WorkflowInstancesBase.WorkflowInstance_TYPE_NAME);
        let workflowActivityConfiguration: ServiceConfiguration
            = ServiceConfigurationHelper.getService(this._serviceConfigurations, WorkflowInstancesBase.WorkflowActivity_TYPE_NAME);
        let workflowInstanceLogConfiguration: ServiceConfiguration
            = ServiceConfigurationHelper.getService(this._serviceConfigurations, WorkflowInstancesBase.WorkflowInstanceLog_TYPE_NAME);

        this.workflowInstanceService = new WorkflowInstanceService(workflowInstanceConfiguration);
        this.workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);
        this.workflowInstanceLogService = new WorkflowInstanceLogService(workflowInstanceLogConfiguration);
    }
}

export = WorkflowInstancesBase;