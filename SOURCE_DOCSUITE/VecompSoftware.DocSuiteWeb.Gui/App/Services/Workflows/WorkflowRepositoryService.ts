/// <amd-dependency path="../../core/extensions/string" />

import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import WorkflowStatus = require('App/Models/Workflows/WorkflowStatus');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowRepositoryModelMapper = require('App/Mappers/Workflows/WorkflowRepositoryModelMapper');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import WorkflowDSWEnvironmentType = require('App/Models/Workflows/WorkflowDSWEnvironmentType');
import RoleModelMapper = require("../../Mappers/Commons/RoleModelMapper");
import RoleModel = require("../../Models/Commons/RoleModel");


class WorkflowRepositoryService extends BaseService {
    private _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getById(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("?$filter=UniqueId eq ", uniqueId, "&$expand=WorkflowRoleMappings($expand=Role),WorkflowEvaluationProperties($orderby=Name),Roles($expand=Father)");
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                callback(response.value[0]);
            }
        }, error);
    }

    getByName(name: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("?$orderby=Name&$expand=Roles($expand=Father)");
        if (!String.isNullOrEmpty(name)) {
            url = url.concat("&$filter=contains(Name, '", name, "')");
        }
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                callback(response.value);
            }
        }, error);
    }

    getByEnvironment(environment: number, filters: string, anyEnv: boolean, documentRequired: boolean, showOnlyNoInstanceWorkflows: boolean, showOnlyHasIsFascicleClosedRequired: boolean, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat(`/WorkflowRepositoryService.GetAuthorizedActiveWorkflowRepositories(environment=${environment},anyEnv=${anyEnv},documentRequired=${documentRequired},showOnlyNoInstanceWorkflows=${showOnlyNoInstanceWorkflows},showOnlyHasIsFascicleClosedRequired=${showOnlyHasIsFascicleClosedRequired})?$orderby=Name`);
        if (!String.isNullOrEmpty(filters)) {
            url = url.concat("&$filter=contains(tolower(Name), '", filters.toLowerCase(), "')");
        }
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                callback(response.value);
            }
        }, error);
    }

    getRepositoryByEnvironment(environment: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat(`?$filter=DSWEnvironment eq '${WorkflowDSWEnvironmentType[environment]}'`);

        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new WorkflowRepositoryModelMapper();
                let workflowRepositories: WorkflowRepositoryModel[] = [];
                $.each(response.value, function (i, value) {
                    workflowRepositories.push(modelMapper.Map(value));
                });
                callback(workflowRepositories);
            }
        }, error);
    }

    getByWorkflowActivityId(workflowActivityId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("/WorkflowRepositoryService.GetByWorkflowActivityId(workflowActivityId=", workflowActivityId.toString(), ")");
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                callback(response.value[0]);
            }
        }, error);
    }

    getWorkflowRepositories(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let workflowRepositories: WorkflowRepositoryModel[] = response.value;
                callback(workflowRepositories);
            };
        }, error);
    }

    getAllWorkflowRepositories(name: string, topElement: string,
        skipElement: number,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=contains(Name,'${name}')&$count=true&$top=${topElement}&$skip=${skipElement.toString()}`;

        this.getRequest(url,
            qs,
            (response: any) => {
                if (callback) {
                    let responseModel: ODATAResponseModel<WorkflowRepositoryModel> = new ODATAResponseModel<WorkflowRepositoryModel>(response);

                    let mapper = new WorkflowRepositoryModelMapper();
                    responseModel.value = mapper.MapCollection(response.value);;

                    callback(responseModel);
                }
            },
            error);
    }

    getWorkflowRepositoryRoles(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`?$expand=Roles($expand=Father)&$filter=UniqueId eq ${uniqueId}`);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new RoleModelMapper();
                let workflowRoles: RoleModel[] = [];
                $.each(response.value[0].Roles, function (i, value) {
                    workflowRoles.push(modelMapper.Map(value));
                });
                callback(workflowRoles);
            };
        }, error);
    }

    hasAuthorizedWorkflowRepositories(environment: number, anyenv: boolean, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("/WorkflowRepositoryService.HasAuthorizedWorkflowRepositories(environment=", environment.toString(), ",anyEnv=", anyenv.toString(), ")");
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }

                    callback(response.value);
                }
            }, error);
    }

    updateWorkflowRepository(model: WorkflowRepositoryModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    insertWorkflowRepository(model: WorkflowRepositoryModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }
}

export = WorkflowRepositoryService;