import BaseService = require('App/Services/BaseService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowActivityLogModel = require('App/Models/Workflows/WorkflowActivityLogModel');
import WorkflowStatus = require('App/Models/Workflows/WorkflowStatus');

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

    countWorkflowActivityByLogType(id: string, workflowStatus: WorkflowStatus, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=LogType eq '${WorkflowStatus[workflowStatus]}' and Entity/UniqueId eq ${id}`;
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }
}

export = WorkflowActivityLogService;