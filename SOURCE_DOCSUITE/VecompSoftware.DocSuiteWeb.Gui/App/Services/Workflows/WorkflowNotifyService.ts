import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import WorkflowNotifyModel = require('App/Models/Workflows/WorkflowNotifyModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class WorkflowNotifyService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    notifyWorkflow(model: WorkflowNotifyModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

}
export = WorkflowNotifyService;