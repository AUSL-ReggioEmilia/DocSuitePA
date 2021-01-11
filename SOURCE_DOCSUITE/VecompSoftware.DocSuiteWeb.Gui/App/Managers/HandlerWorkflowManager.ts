import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel')
import UpdateActionType = require("App/Models/UpdateActionType");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import WorkflowPropertyModel = require('App/Models/Workflows/WorkflowProperty');
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowNotifyService = require('App/Services/Workflows/WorkflowNotifyService');
import WorkflowNotifyModel = require('App/Models/Workflows/WorkflowNotifyModel');
import WorkflowArgumentModel = require('App/Models/Workflows/WorkflowArgumentModel');
import ArgumentType = require('App/Models/Workflows/ArgumentType');
import Environment = require('App/Models/Environment');

class HandlerWorkflowManager {

    private _serviceConfigurations: ServiceConfiguration[];
    private _workflowActivityService: WorkflowActivityService;
    private _workflowNotifyService: WorkflowNotifyService
    
    public static DOCSUITE_MODULE_NAME = "DocSuite";
    private static WORKFLOW_ACTIVITY_EXPAND_PROPERTIES: string[] =
        [
            "WorkflowProperties", "WorkflowInstance($expand=WorkflowRepository)"
        ];

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this.initialize()
    }

    initialize() {
        let workflowActivityConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
        this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);

        let workflowNotifyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowNotify");
        this._workflowNotifyService = new WorkflowNotifyService(workflowNotifyConfiguration);
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
    manageHandlingWorkflow(idWorkflowActivity: string): JQueryPromise<string>
    manageHandlingWorkflow(currentEnvironmentId: string, environment: Environment): JQueryPromise<string>
    manageHandlingWorkflow(environmentOrActivityId: any, environment?: Environment): JQueryPromise<string> {
        let promise: JQueryDeferred<string> = $.Deferred<string>();

        let wfAction: () => JQueryPromise<string> = () => $.Deferred<string>().resolve(environmentOrActivityId as string).promise();
        if (environment) {
            wfAction = () => {
                const promise: JQueryDeferred<string> = $.Deferred<string>();
                this._workflowActivityService.getActiveActivitiesByReferenceIdAndEnvironment(environmentOrActivityId as string, environment,
                    (data: any) => {
                        if (!data) {
                            promise.resolve(null);
                            return;
                        }
                        promise.resolve((data as WorkflowActivityModel).UniqueId);
                    },
                    (exception: ExceptionDTO) => promise.reject(exception));
                return promise.promise();
            }
        }

        wfAction()
            .done((activityId) => {
                if (!activityId) {
                    promise.resolve(null);
                    return;
                }

                this._workflowActivityService.hasHandler(activityId,
                    (workflowActivityHasHandler: boolean) => {
                        if (workflowActivityHasHandler) {
                            promise.resolve(activityId);
                            return;
                        }

                        this._workflowActivityService.getWorkflowActivityById(activityId,
                            (workflowActivityData: any) => {
                                if (!workflowActivityData) {
                                    let exception = {} as ExceptionDTO;
                                    exception.statusText = "Errore nel caricamento delle attività del fusso di lavoro associate al fascicolo.";
                                    promise.reject(exception);
                                    return;
                                }

                                if (workflowActivityData.WorkflowProperties && !this.handlingIsAutomatic(workflowActivityData.WorkflowProperties)) {
                                    promise.resolve(activityId);
                                    return;
                                }

                                this._updateWorkflowActivityAuthorization((workflowActivityData as WorkflowActivityModel))
                                    .done((data: string) => promise.resolve(activityId))
                                    .fail((exception: ExceptionDTO) => promise.reject(exception));
                            }, (exception: ExceptionDTO) => promise.reject(exception), HandlerWorkflowManager.WORKFLOW_ACTIVITY_EXPAND_PROPERTIES);
                    },
                    (exception: ExceptionDTO) => {
                        promise.reject(exception);
                    });
            })
            .fail((exception: ExceptionDTO) => promise.reject(exception));

        return promise.promise();
    }

    private _updateWorkflowActivityAuthorization = (workflowActivity: WorkflowActivityModel): JQueryPromise<string> => {
        let deffered: JQueryDeferred<string> = $.Deferred<string>();

        const workflowNotifyModel = {} as WorkflowNotifyModel;
        workflowNotifyModel.WorkflowActivityId = workflowActivity.UniqueId;
        workflowNotifyModel.WorkflowName = workflowActivity.WorkflowInstance?.WorkflowRepository?.Name;
        workflowNotifyModel.ModuleName = HandlerWorkflowManager.DOCSUITE_MODULE_NAME;

        const dsw_a_ToHandler = {} as WorkflowArgumentModel;
        dsw_a_ToHandler.Name = WorkflowPropertyHelper.DSW_ACTION_TO_HANDLER;
        dsw_a_ToHandler.PropertyType = ArgumentType.PropertyBoolean;
        dsw_a_ToHandler.ValueBoolean = true;

        workflowNotifyModel.OutputArguments = {};
        workflowNotifyModel.OutputArguments[WorkflowPropertyHelper.DSW_ACTION_TO_HANDLER] = dsw_a_ToHandler;

        this._workflowNotifyService.notifyWorkflow(workflowNotifyModel, (response: any) => {
            deffered.resolve(workflowActivity.UniqueId);
        }, (error: ExceptionDTO) => {
            deffered.reject(error);
        });

        return deffered.promise();
    }
}
export = HandlerWorkflowManager;