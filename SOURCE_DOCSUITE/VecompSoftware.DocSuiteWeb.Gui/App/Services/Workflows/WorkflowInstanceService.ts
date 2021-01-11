import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowInstanceModel = require('App/Models/Workflows/WorkflowInstanceModel');
import WorkflowInstanceModelMapper = require('App/Mappers/Workflows/WorkflowInstanceModelMapper');
import WorkflowInstanceSearchFilterDTO = require("App/DTOs/WorkflowInstanceSearchFilterDTO");

class WorkflowInstanceService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    private _buildOdataFilters(baseOdataOperator: string, searchFilter: WorkflowInstanceSearchFilterDTO): string {
        let baseOdataUrl: string = `${this._configuration.ODATAUrl}/${baseOdataOperator}`;

        let oDataFilters: string = "";
        if (searchFilter.workflowRepositoryId) {
            oDataFilters = `${oDataFilters} and WorkflowRepository/UniqueId eq ${searchFilter.workflowRepositoryId}`;
        }
        if (searchFilter.status) {
            oDataFilters = `${oDataFilters} and Status eq '${searchFilter.status}'`;
        }
        if (searchFilter.activeFrom) {
            oDataFilters = `${oDataFilters} and RegistrationDate gt ${searchFilter.activeFrom}`;
        }
        if (searchFilter.activeTo) {
            oDataFilters = `${oDataFilters} and RegistrationDate lt ${searchFilter.activeTo}`;
        }
        if (searchFilter.skip) {
            oDataFilters = `${oDataFilters}&$skip=${searchFilter.skip}`;
        }
        if (searchFilter.top) {
            oDataFilters = `${oDataFilters}&$top=${searchFilter.top}`;

        }

        return baseOdataUrl + oDataFilters;
    }

    countWorkflowInstances(searchFilter: WorkflowInstanceSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        searchFilter.skip = null;
        searchFilter.top = null;
        let url: string = this._buildOdataFilters("$count?$filter= 1 eq 1", searchFilter);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                callback(response);
            };
        }, error);
    }

    getWorkflowInstances(searchFilter: WorkflowInstanceSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._buildOdataFilters("?$expand=WorkflowRepository($select=Name),WorkflowActivities($orderby=RegistrationDate)&$orderby=RegistrationDate desc&$filter= 1 eq 1", searchFilter);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new WorkflowInstanceModelMapper();
                let workflowInstances: WorkflowInstanceModel[] = [];
                $.each(response.value, function (i, value) {
                    workflowInstances.push(modelMapper.Map(value));
                });
                callback(workflowInstances);
            };
        }, error);
    }

}

export = WorkflowInstanceService;