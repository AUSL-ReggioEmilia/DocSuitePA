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

    getWorkflowInstances(searchFilter: WorkflowInstanceSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("?$expand=WorkflowRepository($select=Name),WorkflowActivities($orderby=RegistrationDate)&$orderby=RegistrationDate desc");
        let oDataFilters: string = "&$filter=1 eq 1";
        if (searchFilter.name) {
            oDataFilters = oDataFilters.concat(` and contains(WorkflowRepository/Name, '${searchFilter.name}')`);
        }
        if (searchFilter.status) {
            oDataFilters = oDataFilters.concat(` and Status eq '${searchFilter.status}'`);
        }
        if (searchFilter.name) {
            oDataFilters = oDataFilters.concat(` and contains(WorkflowRepository/Name, '${searchFilter.name}')`);          
        }
        if (searchFilter.activeFrom) {
            oDataFilters = oDataFilters.concat(` and RegistrationDate gt ${searchFilter.activeFrom}`);
        }
        if (searchFilter.activeTo) {
            oDataFilters = oDataFilters.concat(` and RegistrationDate lt ${searchFilter.activeTo}`);
        }

        this.getRequest(url.concat(oDataFilters), null, (response: any) => {
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