/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import TemplateCollaborationModel = require('App/Models/Templates/TemplateCollaborationModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import TemplateCollaborationService = require('App/Services/Templates/TemplateCollaborationService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import UscTemplateCollaborationSelRest = require('UserCOntrol/UscTemplateCollaborationSelRest');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import AjaxModel = require('App/Models/AjaxModel');
import WorkflowReferenceModel = require('../App/Models/Workflows/WorkflowReferenceModel');

declare var ShowLoadingPanel: any;
declare var HideLoadingPanel: any;
class UserCollGestione {
    uscNotificationId: string;
    uscTemplateCollaborationSelRestId: string;
    ajaxManagerId: string;
    hfCurrentTemplateId: string;
    hfCurrentFixedTemplateId: string;
    defaultTemplateId: string;
    fromWorkflowUI: boolean;
    isInsertAction: boolean;

    // names used to send ajax request to backend
    private static AJAX_ACTION_NAME_FOLDER_SELECTED = 'DropdownFolderSelected';
    private static AJAX_ACTION_NAME_FIXED_TEMPLATE_SELECTED = 'DropdownFixedTemplateSelected';
    private static AJAX_ACTION_NAME_TEMPLATE_SELECTED = 'DropdownTemplateSelected';

    private _service: TemplateCollaborationService;
    private _serviceConfigurations: ServiceConfiguration[];
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _hfCurrentTemplate: HTMLElement;
    private _hfCurrentFixedTemplate: HTMLElement;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    public initialize(): void {
        this.InitializeServices();
        this.InitializeReferences();        
        if (this.isInsertAction) {
            this.InitializeEvents();            
        }        
        if (this.fromWorkflowUI) {
            this.bindingFromWorkflowUI();
        } else {
            if (this.defaultTemplateId) {
                this.SetDefaultTemplateCollaboration(this.defaultTemplateId);
            }
        }
    }

    bindingFromWorkflowUI(): void {
        ShowLoadingPanel();
        const workflowReferenceModelSerialized: string = sessionStorage.getItem("WorkflowReferenceModel");
        const workflowStartModelSerialized: string = sessionStorage.getItem("WorkflowStartModel");
        if (workflowReferenceModelSerialized && workflowStartModelSerialized) {
            const ajaxModel: AjaxModel = {} as AjaxModel;
            ajaxModel.Value = [];
            ajaxModel.ActionName = "BindingFromWorkflowUI";
            ajaxModel.Value.push(workflowReferenceModelSerialized);
            ajaxModel.Value.push(workflowStartModelSerialized);
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        }
    }

    bindFromWorkflowUICompletedCallback(): void {
        HideLoadingPanel();
        this.SetDefaultTemplateCollaboration(this.defaultTemplateId);
    }

    public InitializeReferences(): void {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._hfCurrentTemplate = <HTMLElement><any>$get(this.hfCurrentTemplateId);
        this._hfCurrentFixedTemplate = <HTMLElement><any>$get(this.hfCurrentFixedTemplateId);
    }

    public InitializeServices() {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "TemplateCollaboration");
        if (!serviceConfiguration) {
            this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione. Nessun servizio configurato per il Template di collaborazione");
            return;
        }

        this._service = new TemplateCollaborationService(serviceConfiguration);
    }

    public InitializeEvents(): void {
        PageClassHelper.callUserControlFunctionSafe<UscTemplateCollaborationSelRest>(this.uscTemplateCollaborationSelRestId)
            .done((instance) => {
                instance.OnFixedTemplateClick(this.uscTemplateCollaborationSelRestId, (fixedTemplate) => {
                    this._hfCurrentFixedTemplate.setAttribute('value', JSON.stringify(fixedTemplate));
                    this._ajaxManager.ajaxRequest(UserCollGestione.AJAX_ACTION_NAME_FIXED_TEMPLATE_SELECTED);
                });
                instance.OnTemplateClick(this.uscTemplateCollaborationSelRestId, (fixedTemplate, template) => {
                    this._hfCurrentFixedTemplate.setAttribute('value', JSON.stringify(fixedTemplate));
                    this._hfCurrentTemplate.setAttribute('value', JSON.stringify(template));
                    this._ajaxManager.ajaxRequest(UserCollGestione.AJAX_ACTION_NAME_TEMPLATE_SELECTED);
                });
            });
    }

    public SetDefaultTemplateCollaboration = (defaultTemplateId: string): void => {
        PageClassHelper.callUserControlFunctionSafe<UscTemplateCollaborationSelRest>(this.uscTemplateCollaborationSelRestId)
            .done((instance) => {
                instance.SelectAndForceLoadNode(defaultTemplateId, false);
            });
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
            uscNotification.showNotificationMessage(customMessage)
        }
    }
}

export = UserCollGestione;