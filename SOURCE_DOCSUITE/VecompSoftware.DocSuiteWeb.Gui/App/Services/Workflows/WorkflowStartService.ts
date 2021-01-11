import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import WorkflowStartModel = require('App/Models/Workflows/WorkflowStartModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class WorkflowStartService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    startWorkflow(model: WorkflowStartModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

}
export = WorkflowStartService;