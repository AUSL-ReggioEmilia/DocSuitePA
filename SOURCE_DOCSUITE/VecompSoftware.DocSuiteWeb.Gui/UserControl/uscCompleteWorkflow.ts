/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import ServiceConfiguration=require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import WorkflowPropertyService = require('App/Services/Workflows/WorkflowPropertyService');
import WorkflowRepositoryService = require('App/Services/Workflows/WorkflowRepositoryService');
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
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import UpdateActionType = require("App/Models/UpdateActionType");
import AjaxModel = require('App/Models/AjaxModel');

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
    private _workflowPropertyService: WorkflowPropertyService;
    private _workflowRepositoryService: WorkflowRepositoryService;
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

    btnConfirm_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
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
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.contentId);
        this._rblActivityStatus = $("#".concat(this.rblActivityStatusId));
        this._rblActivityStatus.on('change', this.radioListButtonChanged);
        this._lblMotivation = $("#".concat(this.lblMotivationId));
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicking(this.btnConfirm_OnClick);
        let workflowActivityConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowActivity');
        this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);

        let workflowPropertyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowProperty');
        this._workflowPropertyService = new WorkflowPropertyService(workflowPropertyConfiguration);

        let workflowRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowRepository');
        this._workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);

        let workflowNotifyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowNotify');
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
            let acceptanceModel: WorkflowAcceptanceModel = <WorkflowAcceptanceModel>{};
            acceptanceModel.Status = response;
            acceptanceModel.Owner = this.currentUser;
            acceptanceModel.Executor = this.currentUser;
            let txt = <Telerik.Web.UI.RadTextBox>$find(this.txtWfId);
            acceptanceModel.AcceptanceReason = txt.get_textBoxValue();

            let proposerRole: string;

            this._workflowActivity.WorkflowProperties.forEach(function (item) {
                if (item.Name === WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE) {
                    proposerRole = item.ValueString;
                    return;
                }
            });

            let proposerProperty = JSON.parse(proposerRole);
            acceptanceModel.ProposedRole = <RoleModel>proposerProperty;
            acceptanceModel.AcceptanceDate = new Date();          


            let propertyToUpdate: WorkflowPropertyModel = <WorkflowPropertyModel>{};
            propertyToUpdate.WorkflowActivity = this._workflowActivity;

            this._workflowActivity.WorkflowProperties.forEach(function (item: WorkflowPropertyModel){
                if (item.Name === WorkflowPropertyHelper.DSW_FIELD_ACCEPTANCE){
                    item.ValueString = JSON.stringify(acceptanceModel);
                    propertyToUpdate = item;
                    return;
                }
            });

            this._workflowPropertyService.updateProperty(propertyToUpdate,
                (data: any) => {
                    if (data == null) return;
                    this._loadingPanel.show(this.contentId);
                    //mandare workflow notify           
                    this._workflowRepositoryService.getByWorkflowActivityId(this.workflowActivityId,
                        (data: any) => {
                            if (data == null) return;
                            let workflowRepository: WorkflowRepositoryModel = <WorkflowRepositoryModel>data;
                            let notify: WorkflowNotifyModel = <WorkflowNotifyModel>{};
                            notify.WorkflowActivityId = this.workflowActivityId;
                            notify.WorkflowName = workflowRepository.Name;
                            this._workflowNotifyService.notifyWorkflow(notify,
                                (data: any) => {
                                    this._loadingPanel.hide(this.contentId);
                                    let result: AjaxModel = <AjaxModel>{};
                                    result.ActionName = "Stato attività aggiornato"
                                    this.closeWindow(result);
                                },
                                (exception: ExceptionDTO) => {
                                    this._loadingPanel.hide(this.contentId);
                                    this._btnConfirm.set_enabled(true);
                                    this.showNotificationException(this.uscNotificationId, exception);
                                }
                            );
                        },
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.contentId);
                            this._btnConfirm.set_enabled(true);
                            this.showNotificationException(this.uscNotificationId, exception);
                        }
                    );

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
                    let result: AjaxModel = <AjaxModel>{};
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
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    /**
     * Chiude la RadWindow
    */
    closeWindow = (message?: AjaxModel) => {        
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(message);
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }

    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    protected radioListButtonChanged = () => {
        let checkedChoice = Number(this._rblActivityStatus.find('input:checked').val());

        switch (checkedChoice) {
            case WorkflowActivityStatus.Done:
            case WorkflowActivityStatus.Rejected: {

                this._motivationEnabled = false;

                let motivationProp: Array<WorkflowPropertyModel> = this._workflowActivity.WorkflowProperties.filter(function (item) {
                    if (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_END_MOTIVATION_REQUIRED) {
                        return item;                        
                    }
                });              

                if (motivationProp && motivationProp.length>0) {
                    this._motivationEnabled = motivationProp[0].ValueBoolean;
                }

                ValidatorEnable(document.getElementById(this.ctrlTxtWfId), this._motivationEnabled);

                $('#'.concat(this.pnlWorkflowNoteId)).hide();
                let motivationProperties: WorkflowPropertyModel[] = this._workflowActivity.WorkflowProperties.filter(item => item.Name == WorkflowPropertyHelper.DSW_ACTION_METADATA_MOTIVATION_LABEL);
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