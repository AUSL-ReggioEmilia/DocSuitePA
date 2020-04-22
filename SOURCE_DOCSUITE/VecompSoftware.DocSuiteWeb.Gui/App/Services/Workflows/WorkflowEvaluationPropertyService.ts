import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowEvaluationProperty = require('App/Models/Workflows/WorkflowEvaluationProperty');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowEvaluationPropertyModelMapper = require('App/Mappers/Workflows/WorkflowEvaluationPropertyModelMapper');


class WorkflowEvaluationPropertyService extends BaseService {

    _configuration: ServiceConfiguration;
    _mapper: WorkflowEvaluationPropertyModelMapper;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
        this._mapper = new WorkflowEvaluationPropertyModelMapper();
    }

    insertWorkflowEvaluationProperty(model: WorkflowEvaluationProperty, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    getWorkflowEvaluationProperty(idWorkflowEvaluationProperty: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("?$filter=UniqueId eq ", idWorkflowEvaluationProperty);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    let instance = <WorkflowEvaluationProperty>{};
                    let mapper = new WorkflowEvaluationPropertyModelMapper();
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
    }

    updateWorkflowEvaluationProperty(model: WorkflowEvaluationProperty, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    deleteWorkflowEvaluationProperty(model: WorkflowEvaluationProperty, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }

}

export = WorkflowEvaluationPropertyService;