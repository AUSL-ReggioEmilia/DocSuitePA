import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');
import UpdateActionType = require("App/Models/UpdateActionType");
import WorkflowActivityModelMapper = require('App/Mappers/Workflows/WorkflowActivityModelMapper');
import WorkflowStatus = require('App/Models/Workflows/WorkflowStatus');

class WorkflowActivityService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    /**
 * Recupera un'attività per ID
 * @param id
 * @param callback
 * @param error
 */
    getWorkflowActivity(id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        const url: string = this._configuration.ODATAUrl;
        const data = `$filter=UniqueId eq ${id}&$expand=WorkflowAuthorizations,WorkflowProperties,WorkflowInstance($expand=WorkflowRepository)`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let instance = {} as WorkflowActivityModel;
                    const mapper = new WorkflowActivityModelMapper();
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
    }

    getWorkflowActivityByLogType(id: string, workflowStatus: WorkflowStatus, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        const url: string = this._configuration.ODATAUrl;
        const data = `$filter=UniqueId eq ${id}&$expand=WorkflowActivityLogs($filter=LogType eq '${WorkflowStatus[workflowStatus]}')`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let instance = {} as WorkflowActivityModel;
                    const mapper = new WorkflowActivityModelMapper();
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
    }

    getWorkflowActivityById(id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, expandProperties?: string[]): void {
        const url: string = this._configuration.ODATAUrl;
        let odataQuery = `$filter=UniqueId eq ${id}`;
        if (expandProperties && expandProperties.length) {
            odataQuery = `${odataQuery}&$expand=${(expandProperties.join(','))}`;
        }

        this.getRequest(url, odataQuery,
            (response: any) => {
                if (callback) {
                    let instance = {} as WorkflowActivityModel;
                    const mapper = new WorkflowActivityModelMapper();
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
    }

    isWorkflowActivityHandler(workflowActivityId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("/WorkflowActivityService.IsWorkflowActivityHandler(workflowActivityId=", workflowActivityId, ")");

        this.getRequest(url, null, (response: any) => {
            if (callback) {
                callback(response.value);
            }
        }, error);
    }

    hasHandler(workflowActivityId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("/WorkflowActivityService.HasHandler(workflowActivityId=", workflowActivityId, ")");

        this.getRequest(url, null, (response: any) => {
            if (callback) {
                callback(response.value);
            }
        }, error);
    }

    /**
   * Effettuo la presa in carico dell'attività di workflow
   * @param model
   * @param callback
   * @param error
   */
    updateHandlingWorkflowActivity(workflowActivity: WorkflowActivityModel, updateAction: UpdateActionType, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl.concat("?actionType=", updateAction.toString());
        this.putRequest(url, JSON.stringify(workflowActivity), callback, error);
    }

    /**
     *  Recupero l'attività di Workflow associata all'entità desiderata
     * @param environmentId
     * @param type
     * @param callback
     * @param error
     */
    getActiveActivitiesByReferenceIdAndEnvironment(referenceId: string, type: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("/WorkflowActivityService.GetActiveActivitiesByReferenceIdAndEnvironment(referenceId=", referenceId, ",type=VecompSoftware.DocSuiteWeb.Entity.Commons.DSWEnvironmentType'", type.toString(), "')");
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    let mapper = new WorkflowActivityModelMapper();
                    let instance = <WorkflowActivityModel>{};
                    //TODO: Va ancora gestito il caso di più istanze presente sullo stesso referenceId nella pagina di visualizzazione
                    //dell'environment specifico (fascVisualizza, DossierVisualizza)
                    instance = mapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
    }

    getWorkflowActivitiesByWorkflowInstanceId(instanceId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`?$expand=WorkflowInstance&$filter=WorkflowInstance/InstanceId eq ${instanceId} &$orderby=Name`);
        this.getRequest(url, null,
          (response: any) => {
            if (callback) {
              let modelMapper = new WorkflowActivityModelMapper();
              let workflowActivities: WorkflowActivityModel[] = [];
              $.each(response.value, function (i, value) {
                workflowActivities.push(modelMapper.Map(value));
              });

              callback(workflowActivities);
            }
          }, error);
    }

    getActiveByReferenceDocumentUnitId(uniqueId: string, callback?: (data: WorkflowActivityModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`?$filter=DocumentUnitReferenced/UniqueId eq ${uniqueId} and (Status eq 'Todo' or Status eq 'Progress')`);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    let modelMapper = new WorkflowActivityModelMapper();
                    let workflowActivities: WorkflowActivityModel[] = [];
                    $.each(response.value, function (i, value) {
                        workflowActivities.push(modelMapper.Map(value));
                    });

                    callback(workflowActivities);
                }
            }, error);
    }

    countActiveByReferenceDocumentUnitId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=DocumentUnitReferenced/UniqueId eq ${uniqueId} and (Status eq 'Todo' or Status eq 'Progress')`;
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    countByReferenceDocumentUnitId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=DocumentUnitReferenced/UniqueId eq ${uniqueId} and (Status eq 'Todo' or Status eq 'Progress' or Status eq 'Done' or Status eq 'Error') and not (ActivityType in ('BuildProtocol','BuildAchive','BuildPECMail','BuildMessages')) and IsVisible eq true`;
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    getByReferenceDocumentUnitId(uniqueId: string, callback?: (data: WorkflowActivityModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`?$filter=DocumentUnitReferenced/UniqueId eq ${uniqueId} and (Status eq 'Todo' or Status eq 'Progress' or Status eq 'Done' or Status eq 'Error') and not (ActivityType in ('BuildProtocol','BuildAchive','BuildPECMail','BuildMessages')) and IsVisible eq true&$orderby=RegistrationDate desc`);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    let modelMapper = new WorkflowActivityModelMapper();
                    let workflowActivities: WorkflowActivityModel[] = [];
                    $.each(response.value, function (i, value) {
                        workflowActivities.push(modelMapper.Map(value));
                    });

                    callback(workflowActivities);
                }
            }, error);
    }

    getByStatusReferenceDocumentUnitId(status:string, uniqueId: string, callback?: (data: WorkflowActivityModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`?$filter=DocumentUnitReferenced/UniqueId eq ${uniqueId} and Status eq '${status}'`);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    let modelMapper = new WorkflowActivityModelMapper();
                    let workflowActivities: WorkflowActivityModel[] = [];
                    $.each(response.value, function (i, value) {
                        workflowActivities.push(modelMapper.Map(value));
                    });

                    callback(workflowActivities);
                }
            }, error);
    }

    countByStatusReferenceDocumentUnitId(status: string, uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=DocumentUnitReferenced/UniqueId eq ${uniqueId} and Status eq '${status}'`;
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }
}
export = WorkflowActivityService;