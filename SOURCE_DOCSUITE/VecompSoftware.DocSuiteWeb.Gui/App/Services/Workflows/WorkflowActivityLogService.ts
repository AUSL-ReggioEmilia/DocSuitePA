import BaseService = require('App/Services/BaseService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowActivityLogModel = require('App/Models/Workflows/WorkflowActivityLogModel');

class WorkflowActivityLogService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
    * Costruttore 
    */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    insertWorkflowActivityLog(model: WorkflowActivityLogModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }
}

export = WorkflowActivityLogService;