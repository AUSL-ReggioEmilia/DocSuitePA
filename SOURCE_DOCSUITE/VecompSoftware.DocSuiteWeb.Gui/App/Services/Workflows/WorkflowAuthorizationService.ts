import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowAuthorization = require('App/Models/Workflows/WorkflowAuthorizationModel');
import WorkflowAuthorizationModelMapper = require('App/Mappers/Workflows/WorkflowAuthorizationModelMapper');

class WorkflowAuthorizationService extends BaseService {
    _configuration: ServiceConfiguration;
    _mapper: WorkflowAuthorizationModelMapper;
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
        this._mapper = new WorkflowAuthorizationModelMapper();
    }

    /**
     * Recupero le WorkflowAuthorizations con l'id dell'attività
     * @param workflowActivityId
     * @param callback
     * @param error
     */
    getAuthorizationsByActivity(workflowActivityId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "WorkflowAuthorizations?$filter=WorkflowActivity/UniqueId eq ".concat(workflowActivityId);
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let workflowAuthorizations: WorkflowAuthorization[] = [];
                    if (response){

                        workflowAuthorizations = this._mapper.MapCollection(response.value);

                        callback(workflowAuthorizations);
                    }
                }
            }, error);
    }

    /**
     * Verifica se un determinato utente è autorizzato in una specifica workflowactivity
     * @param account
     * @param workflowActivityId
     * @param callback
     * @param error
     */
    isUserAuthorized(account: string, workflowActivityId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=WorkflowActivity/UniqueId eq ${workflowActivityId} and Account eq '${account}'`;
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let workflowAuthorizations: WorkflowAuthorization[] = [];
                    if (response) {
                        workflowAuthorizations = this._mapper.MapCollection(response.value);
                        callback(workflowAuthorizations.length > 0);
                    }
                }
            }, error);
    }
}
export = WorkflowAuthorizationService;