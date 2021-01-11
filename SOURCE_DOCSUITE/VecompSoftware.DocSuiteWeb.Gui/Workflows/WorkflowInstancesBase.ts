/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import WorkflowInstanceService = require('App/Services/Workflows/WorkflowInstanceService');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import WorkflowInstanceLogService = require('App/Services/Workflows/WorkflowInstancelogService')
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import WorkflowRepositoryService = require('App/Services/Workflows/WorkflowRepositoryService');

abstract class WorkflowInstancesBase {
    protected static WorkflowInstance_TYPE_NAME = "WorkflowInstance";
    protected static WorkflowInstanceLog_TYPE_NAME = "WorkflowInstanceLog";
    protected static WorkflowActivity_TYPE_NAME = "WorkflowActivity";
    protected static WorkflowRepository_TYPE_NAME = "WorkflowRepository";

    protected _serviceConfigurations: ServiceConfiguration[];

    protected workflowInstanceService: WorkflowInstanceService;
    protected workflowActivityService: WorkflowActivityService;
    protected workflowInstanceLogService: WorkflowInstanceLogService;
    protected workflowRepositoryService: WorkflowRepositoryService;

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
        let workflowRepositoryConfiguration: ServiceConfiguration
            = ServiceConfigurationHelper.getService(this._serviceConfigurations, WorkflowInstancesBase.WorkflowRepository_TYPE_NAME);

        this.workflowInstanceService = new WorkflowInstanceService(workflowInstanceConfiguration);
        this.workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);
        this.workflowInstanceLogService = new WorkflowInstanceLogService(workflowInstanceLogConfiguration);
        this.workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);
    }
}

export = WorkflowInstancesBase;