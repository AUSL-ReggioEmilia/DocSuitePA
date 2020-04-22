import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import WorkflowInstanceLogViewModelMapper = require('App/Mappers/Workflows/WorkflowInstanceLogViewModelMapper');
import WorkflowInstanceLogViewModel = require('App/ViewModels/Workflows/WorkflowInstanceLogViewModel');

class WorkflowInstanceLogService extends BaseService {
    _configuration: ServiceConfiguration;
    _mapper: WorkflowInstanceLogViewModelMapper;

    /**
     * costruttore
     * @param configuration
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
        this._mapper = new WorkflowInstanceLogViewModelMapper();
    }

    getFascicleInstanceLogs(idFascicle: string, skip: number, top: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=Entity/Fascicles/any(d:d/UniqueId eq ".concat(idFascicle, ")&$orderby=RegistrationDate desc &$skip=", skip.toString(), "&$top=", top.toString(), "&$count=true");
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let responseModel: ODATAResponseModel<WorkflowInstanceLogViewModel> = new ODATAResponseModel<WorkflowInstanceLogViewModel>(response);
                    if (response) {
                        responseModel.value = this._mapper.MapCollection(response.value);
                    }
                    callback(responseModel);
                }
            }
        )
    }

    getWorkflowInstanceLogs(workflowInstanceId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=Entity/UniqueId eq ${workflowInstanceId}&$orderby=RegistrationDate desc`;
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let responseModel: ODATAResponseModel<WorkflowInstanceLogViewModel> = new ODATAResponseModel<WorkflowInstanceLogViewModel>(response);
                    if (response) {
                        responseModel.value = this._mapper.MapCollection(response.value);
                    }
                    callback(responseModel);
                }
            }
        )
    }
}
export = WorkflowInstanceLogService;