/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import WorkflowRepositoryService = require('App/Services/Workflows/WorkflowRepositoryService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import WorkflowStartService = require('App/Services/Workflows/WorkflowStartService');

class WorkflowActivityInsertBase {
    protected static WorkflowRepository_TYPE_NAME: string = "WorkflowRepository";
    protected static WORKFLOWSTART_TYPE_NAME: string = "WorkflowStart";

    protected _serviceConfigurations: ServiceConfiguration[];

    protected workflowRepositoryService: WorkflowRepositoryService;

    protected workflowStartService: WorkflowStartService;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize() {
        let workflowRepositoryConfiguration: ServiceConfiguration
            = ServiceConfigurationHelper.getService(this._serviceConfigurations, WorkflowActivityInsertBase.WorkflowRepository_TYPE_NAME);

        let workflowStartConfiguration: ServiceConfiguration
            = ServiceConfigurationHelper.getService(this._serviceConfigurations, WorkflowActivityInsertBase.WORKFLOWSTART_TYPE_NAME);

        this.workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);
        this.workflowStartService = new WorkflowStartService(workflowStartConfiguration);
    }
}

export = WorkflowActivityInsertBase;