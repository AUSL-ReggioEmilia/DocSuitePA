/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import ServiceConfiguration=require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import WorkflowNotifyService = require('App/Services/Workflows/WorkflowNotifyService');
import WorkflowActivityStatus = require('App/Models/Workflows/WorkflowActivityStatus');
import WorkflowNotifyModel = require('App/Models/Workflows/WorkflowNotifyModel');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import AcceptanceStatus = require('App/Models/Workflows/AcceptanceStatus');
import WorkflowAcceptanceModel = require('App/Models/Workflows/WorkflowAcceptanceModel');
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import WorkflowPropertyModel = require('App/Models/Workflows/WorkflowProperty');
import RoleModel = require('App/Models/Commons/RoleModel');
import UpdateActionType = require("App/Models/UpdateActionType");
import AjaxModel = require('App/Models/AjaxModel');
import HandlerWorkflowManager = require('App/Managers/HandlerWorkflowManager');
import WorkflowArgumentModel = require('App/Models/Workflows/WorkflowArgumentModel');
import ArgumentType = require('App/Models/Workflows/ArgumentType');

declare var Page_IsValid: any;
declare var ValidatorEnable: any;
class uscCompleteWorkflow {
    workflowActivityId: string;
    rblActivityStatusId: string;
    btnConfirmId: string;
    uscNotificationId: string;
    contentId: string;
    ajaxLoadingPanelId: string;
    currentUser: string;
    txtWfId: string;
    pnlWorkflowNoteId: string;
    ctrlTxtWfId: string;
    lblMotivationId: string;

    private _rblActivityStatus: JQuery;
    private _serviceConfigurations: ServiceConfiguration[];
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _workflowActivityService: WorkflowActivityService;
    private _workflowNotifyService: WorkflowNotifyService;
    private _lblMotivation: JQuery;
    private _workflowActivity: WorkflowActivityModel;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _motivationEnabled: boolean;
    /**
     *
    *Costruttore
     * @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    /**
    *---------------------Events---------------------
    */

    btnConfirm_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (!Page_IsValid) {
            return;
        }
        this._loadingPanel.show(this.contentId);
        let checkedChoice = Number(this._rblActivityStatus.find('input:checked').val());
        
        switch (checkedChoice) {
            case WorkflowActivityStatus.Done: {
                this.notifyWorkflow(AcceptanceStatus.Accepted);
                break;
            }
            case WorkflowActivityStatus.Handling: {
                this.updateHandlingWorkflowActivity(UpdateActionType.HandlingWorkflow);
                break;
            }
            case WorkflowActivityStatus.Todo: {
                this.updateHandlingWorkflowActivity(UpdateActionType.RelaseHandlingWorkflow);
                break;
            }
            case WorkflowActivityStatus.Rejected: {
                this.notifyWorkflow(AcceptanceStatus.Refused);
                break;
            }
        }

    }

    /**
    *---------------------Methods---------------------
    */

    /**
    *Initialize
    */
    initialize() {
        this._loadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
        this._loadingPanel.show(this.contentId);
        this._rblActivityStatus = $("#".concat(this.rblActivityStatusId));
        this._rblActivityStatus.on('change', this.radioListButtonChanged);
        this._lblMotivation = $("#".concat(this.lblMotivationId));
        this._btnConfirm = $find(this.btnConfirmId) as Telerik.Web.UI.RadButton;
        this._btnConfirm.add_clicking(this.btnConfirm_OnClick);
        const workflowActivityConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowActivity');
        this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);

        const workflowNotifyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowNotify');
        this._workflowNotifyService = new WorkflowNotifyService(workflowNotifyConfiguration);

        ValidatorEnable(document.getElementById(this.ctrlTxtWfId), false);

        this._workflowActivityService.getWorkflowActivity(this.workflowActivityId,
            (data: any) => {
                if (data == null) return;
                this._workflowActivity = data;               

                this.initializeRclActivityStatus();
                this.radioListButtonChanged();
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.contentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );         
    }

    initializeRclActivityStatus = () => {
        this._workflowActivityService.isWorkflowActivityHandler(this.workflowActivityId,
            (data: any) => {
                if (data == null) return;
                let toCheck = this._rblActivityStatus.find("input[value=".concat(WorkflowActivityStatus.Handling.toString(), "]"));
                toCheck.prop('checked', true);
                if (data === false) {                    
                    let toDisable = this._rblActivityStatus.find("input[value!=".concat(WorkflowActivityStatus.Handling.toString(), "]"));
                    toDisable.each(function () {
                        $(this).prop('disabled', true);
                    });                    
                }
                this._loadingPanel.hide(this.contentId);
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception, "Anomalia nel recupero dell'informazione isHandler.");
            }
        );
       
    }

    notifyWorkflow = (response: AcceptanceStatus) => {        
        if (confirm("Conferma il completamento e restituisci al richiedente?")) {

            //update della proprietà acceptance activity
            const acceptanceModel: WorkflowAcceptanceModel = {} as WorkflowAcceptanceModel;
            acceptanceModel.Status = response;
            acceptanceModel.Owner = this.currentUser;
            acceptanceModel.Executor = this.currentUser;
            const txt: Telerik.Web.UI.RadTextBox = $find(this.txtWfId) as Telerik.Web.UI.RadTextBox;
            acceptanceModel.AcceptanceReason = txt.get_textBoxValue();

            let proposerRole: string;

            this._workflowActivity.WorkflowProperties.forEach(function (item) {
                if (item.Name === WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE) {
                    proposerRole = item.ValueString;
                    return;
                }
            });

            const proposerProperty = JSON.parse(proposerRole);
            acceptanceModel.ProposedRole = proposerProperty as RoleModel;
            acceptanceModel.AcceptanceDate = new Date();          

            this._loadingPanel.show(this.contentId);

            const workflowNotifyModel: WorkflowNotifyModel = {} as WorkflowNotifyModel;
            workflowNotifyModel.WorkflowActivityId = this.workflowActivityId;
            workflowNotifyModel.WorkflowName = this._workflowActivity.WorkflowInstance?.WorkflowRepository?.Name;
            workflowNotifyModel.ModuleName = HandlerWorkflowManager.DOCSUITE_MODULE_NAME;
            workflowNotifyModel.OutputArguments = {};

            const propAcceptance: WorkflowArgumentModel = {} as WorkflowArgumentModel;
            propAcceptance.Name = WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE;
            propAcceptance.PropertyType = ArgumentType.Json;
            propAcceptance.ValueString = JSON.stringify(acceptanceModel);
            workflowNotifyModel.OutputArguments[WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE] = propAcceptance;

            const propManualComplete: WorkflowArgumentModel = {} as WorkflowArgumentModel;
            propManualComplete.Name = WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE;
            propManualComplete.PropertyType = ArgumentType.PropertyBoolean;
            propManualComplete.ValueBoolean = true;
            workflowNotifyModel.OutputArguments[WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE] = propManualComplete;

            this._workflowNotifyService.notifyWorkflow(workflowNotifyModel,
                (data: any) => {
                    this._loadingPanel.hide(this.contentId);
                    const result: AjaxModel = {} as AjaxModel;
                    result.ActionName = "Stato attività aggiornato"
                    this.closeWindow(result);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.contentId);
                    this._btnConfirm.set_enabled(true);
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        }
        this._loadingPanel.hide(this.contentId);
        this.radioListButtonChanged();
    }
     

    updateHandlingWorkflowActivity = (activity: UpdateActionType) => {
        let message: string;
        switch (activity){
            case UpdateActionType.HandlingWorkflow: {
                message = "Attività presa in carico";
                break;
            }
            case UpdateActionType.RelaseHandlingWorkflow: {
                message = "Stato attività impostato 'da lavorare'";
                break;
            }
        }
        if (this._workflowActivity.WorkflowAuthorizations) {
            this._workflowActivityService.updateHandlingWorkflowActivity(this._workflowActivity, activity,
                (data: any) => {
                    const result: AjaxModel = {} as AjaxModel;
                    result.ActionName = message;
                    result.Value = new Array<string>();
                    result.Value.push(this.workflowActivityId);
                    this.closeWindow(result);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.contentId);
                    $("#".concat(this.contentId)).hide();
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        }
    }

    /**
* Recupera una RadWindow dalla pagina
*/
    getRadWindow = () => {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = (<any>window).radWindow as Telerik.Web.UI.RadWindow;
        else if ((<any>window.frameElement).radWindow) wnd = (<any>window.frameElement).radWindow as Telerik.Web.UI.RadWindow;
        return wnd;
    }

    /**
     * Chiude la RadWindow
    */
    closeWindow = (message?: AjaxModel) => {        
        const wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(message);
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            const uscNotification: UscErrorNotification = $("#".concat(uscNotificationId)).data() as UscErrorNotification;
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }

    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        const uscNotification: UscErrorNotification = $("#".concat(uscNotificationId)).data() as UscErrorNotification;
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    protected radioListButtonChanged = () => {
        const checkedChoice = Number(this._rblActivityStatus.find('input:checked').val());

        switch (checkedChoice) {
            case WorkflowActivityStatus.Done:
            case WorkflowActivityStatus.Rejected: {

                this._motivationEnabled = false;

                const motivationProp: Array<WorkflowPropertyModel> = this._workflowActivity.WorkflowProperties.filter(function (item) {
                    if (item.Name === WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_END_MOTIVATION_REQUIRED) {
                        return item;                        
                    }
                });              

                if (motivationProp && motivationProp.length>0) {
                    this._motivationEnabled = motivationProp[0].ValueBoolean;
                }

                ValidatorEnable(document.getElementById(this.ctrlTxtWfId), this._motivationEnabled);

                $('#'.concat(this.pnlWorkflowNoteId)).hide();
                const motivationProperties: WorkflowPropertyModel[] = this._workflowActivity.WorkflowProperties.filter(item => item.Name === WorkflowPropertyHelper.DSW_ACTION_METADATA_MOTIVATION_LABEL);
                if (motivationProperties && motivationProperties.length > 0) {
                    this._lblMotivation.html(motivationProperties[0].ValueString);
                    $('#'.concat(this.pnlWorkflowNoteId)).show();
                }

                
                break;
            }
            case WorkflowActivityStatus.Handling:
            case WorkflowActivityStatus.Todo: {
                ValidatorEnable(document.getElementById(this.ctrlTxtWfId), false);
                $('#'.concat(this.pnlWorkflowNoteId)).hide();
                break;
            }            
        }
    }
}
export = uscCompleteWorkflow;