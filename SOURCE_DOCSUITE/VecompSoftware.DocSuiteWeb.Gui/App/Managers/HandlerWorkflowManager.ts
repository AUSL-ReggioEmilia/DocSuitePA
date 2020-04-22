import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel')
import UpdateActionType = require("App/Models/UpdateActionType");
import WorkflowStatus = require('App/Models/Workflows/WorkflowStatus');
import WorkflowAuthorizationModel = require('App/Models/Workflows/WorkflowAuthorizationModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import Environment = require('App/Models/Environment');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import WorkflowPropertyService = require('App/Services/Workflows/WorkflowPropertyService');
import WorkflowPropertyModel = require('App/Models/Workflows/WorkflowProperty');
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import WorkflowAuthorizationService = require('App/Services/Workflows/WorkflowAuthorizationService')
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class HandlerWorkflowManager {

    private _serviceConfigurations: ServiceConfiguration[];
    private _workflowActivityService: WorkflowActivityService;
    private _workflowPropertyService: WorkflowPropertyService;
    private _workflowAuthorizationService: WorkflowAuthorizationService;
    private _workflowActivity: WorkflowActivityModel;
    private _workflowProperties: WorkflowPropertyModel[];

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this.initialize()
    }

    initialize() {
        let workflowActivityConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
        this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);

        let workflowPropertyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowProperty");
        this._workflowPropertyService = new WorkflowPropertyService(workflowPropertyConfiguration);

        let workflowAuthorizationConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowAuthorizations");
        this._workflowAuthorizationService = new WorkflowAuthorizationService(workflowPropertyConfiguration);
    }


    /**
     *  ------------------------------- Methods ----------------------------
     */



    /**
     * Verifico se e' prevista la presa in carico automatica dei workflow
     * @param properties
     */
    private handlingIsAutomatic(properties: WorkflowPropertyModel[]): boolean {
        var automaticHandling: boolean = false;

        properties.forEach(function (item: WorkflowPropertyModel) {
            if (item.Name === WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_AUTO_HANDLING) {
                automaticHandling = item.ValueBoolean;
                return;
            }
        });

        return automaticHandling;
    }

    /**
     * Gestisco l'attivita' di workflow corrente se non ho gia' l'id
     * @param currentEnvironmentId
     * @param environment
     */
    manageHandlingWorkflow(currentEnvironmentId: string, environment: number): JQueryPromise<string> {
        let promise: JQueryDeferred<string> = $.Deferred<string>();
        var workflowActivity: WorkflowActivityModel;

        this._workflowActivityService.getActiveActivitiesByReferenceIdAndEnvironment(currentEnvironmentId, environment,
            (data: any) => {
                if (data) {
                    workflowActivity = <WorkflowActivityModel>data;
                    this._workflowActivityService.hasHandler(workflowActivity.UniqueId,
                        (data: boolean) => {
                            if (!data) {
                                this._workflowPropertyService.getPropertiesFromActivity(workflowActivity.UniqueId,
                                    (data: any) => {
                                        if (data) {
                                            if (!this.handlingIsAutomatic(data)) {
                                                promise.resolve(workflowActivity.UniqueId);
                                                return;
                                            }
                                            this._workflowActivityService.getWorkflowActivity(workflowActivity.UniqueId,
                                                (data: any) => {
                                                    if (data) {
                                                        workflowActivity = data;
                                                        this._workflowActivityService.updateHandlingWorkflowActivity(workflowActivity, UpdateActionType.HandlingWorkflow,
                                                            (data: any) => {
                                                                promise.resolve(workflowActivity.UniqueId);
                                                            },
                                                            (exception: ExceptionDTO) => {
                                                                promise.reject(exception);
                                                            });
                                                    }
                                                    promise.resolve(workflowActivity.UniqueId);
                                                },
                                                (exception: ExceptionDTO) => {
                                                    promise.reject(exception);
                                                });
                                        }
                                    },
                                    (exception: ExceptionDTO) => {
                                        promise.reject(exception);
                                    });
                            } else {
                                promise.resolve(workflowActivity.UniqueId);
                            }
                        },
                        (exception: ExceptionDTO) => {
                            promise.reject(exception);
                        });
                } else {
                    promise.resolve(null);
                }
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            });
        return promise.promise();
    }

    /**
     * Gestisco l'attivita' di workflow corrente se ho gia' l'id
     * @param idWorkflowActivityId
     */
    manageHandlingWorkflowWithActivity(idWorkflowActivityId: string): JQueryPromise<string> {
        let promise: JQueryDeferred<string> = $.Deferred<string>();
        var workflowActivity: WorkflowActivityModel;

        this._workflowActivityService.hasHandler(idWorkflowActivityId,
            (data: boolean) => {
                if (!data) {
                    this._workflowActivityService.getWorkflowActivity(idWorkflowActivityId,
                        (data: any) => {
                            if (data) {
                                workflowActivity = data;
                                if (!this.handlingIsAutomatic(workflowActivity.WorkflowProperties)) {
                                    promise.resolve(workflowActivity.UniqueId);
                                    return;
                                }
                                this._workflowActivityService.updateHandlingWorkflowActivity(workflowActivity, UpdateActionType.HandlingWorkflow,
                                    (data: any) => {
                                        promise.resolve(workflowActivity.UniqueId);
                                    },
                                    (exception: ExceptionDTO) => {
                                        promise.reject(exception);
                                    });
                            }
                        },
                        (exception: ExceptionDTO) => {
                            promise.reject(exception);
                        });
                } else {
                    promise.resolve(idWorkflowActivityId);
                }
                
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            });

        return promise.promise();
    }

}
export = HandlerWorkflowManager;