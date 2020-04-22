import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowPropertyModel = require('App/Models/Workflows/WorkflowProperty');
import WorkflowStatus = require('App/Models/Workflows/WorkflowStatus');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowPropertyModelMapper = require('App/Mappers/Workflows/WorkflowPropertyModelMapper')

class WorkflowPropertyService extends BaseService {
    private _configuration: ServiceConfiguration;
    _mapper: WorkflowPropertyModelMapper;
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
        this._mapper = new WorkflowPropertyModelMapper();

    }

    updateProperty(model: WorkflowPropertyModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    getPropertiesFromActivity(idWorkflowActivity: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=WorkflowActivity/UniqueId eq ".concat(idWorkflowActivity);
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let workflowPropertiesModel: WorkflowPropertyModel[] = [];
                    if (response) {
                        workflowPropertiesModel = this._mapper.MapCollection(response.value);
                    }
                    callback(workflowPropertiesModel);
                }
            }
        )
    }
}

export = WorkflowPropertyService;