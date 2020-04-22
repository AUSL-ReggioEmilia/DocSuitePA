/// <amd-dependency path="../../core/extensions/string" />

import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowRoleMappingModel = require('App/Models/Workflows/WorkflowRoleMappingModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');


class WorkflowRoleMappingService extends BaseService {
    private _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    deleteWorkflowRoleMapping(model: WorkflowRoleMappingModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }

    getByName(name: string, idRepository: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("?$filter=WorkflowRepository/UniqueId eq ", idRepository);
        if (!String.isNullOrEmpty(name)) {
            url = url.concat(" and contains(MappingTag, '", name, "')");
        }
        url = url.concat("&$apply=groupby((MappingTag,WorkflowRepository/UniqueId))");
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                callback(response.value);
            }
        }, error);
    }
}

export = WorkflowRoleMappingService;