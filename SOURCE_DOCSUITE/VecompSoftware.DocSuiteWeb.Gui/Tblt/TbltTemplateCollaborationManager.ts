/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import TemplateCollaborationService = require('App/Services/Templates/TemplateCollaborationService');
import TemplateCollaborationModel = require('App/Models/Templates/TemplateCollaborationModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class TbltTemplateCollaborationManager {
    btnNewId: string;
    grdTemplateCollaborationId: string;
    ajaxLoadingPanelId: string;
    ajaxManagerId: string;
    btnDeleteId: string;
    radWindowManagerId: string;
    uscNotificationId: string;

    private _btnNew: Telerik.Web.UI.RadButton;
    private _btnDelete: Telerik.Web.UI.RadButton;
    private _grdTemplateCollaboration: Telerik.Web.UI.RadGrid;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _service: TemplateCollaborationService;
    private _uscNotification: UscErrorNotification;

    private static TEMPLATE_GESTIONE_URL: string = "../User/TemplateUserCollGestione.aspx?Action=Insert&Type=Prot";

    /**
     * Costruttore della classe
     * @param serviceConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateCollaboration");
        if (!serviceConfiguration) {
            this.showNotificationMessage(this.uscNotificationId,"Errore in inizializzazione. Nessun servizio configurato per il Template di collaborazione");
            return;
        }

        this._service = new TemplateCollaborationService(serviceConfiguration);
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato al click del pulsante Nuovo
     * @param sender
     * @param args
     */
    btnNew_OnClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._loadingPanel.show(this.grdTemplateCollaborationId);
        window.location.href = TbltTemplateCollaborationManager.TEMPLATE_GESTIONE_URL;
    }

    /**
     * Evento scatenato al click del pulsante Elimina
     * @param sender
     * @param args
     */
    btnDelete_OnClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        let selectedTemplates: Telerik.Web.UI.GridDataItem[] = this._grdTemplateCollaboration.get_selectedItems();
        if (selectedTemplates.length == 0) {
            this.showWarningMessage(this.uscNotificationId,"Selezionare un template");
            return;
        }

        this._manager.radconfirm("Sei sicuro di voler eliminare il template selezionato?", (arg) => {
            if (arg) {
                this._loadingPanel.show(this.grdTemplateCollaborationId);
                try
                {
                    let template: Telerik.Web.UI.GridDataItem = selectedTemplates[0];
                    this._service.getById(template.getDataKeyValue("Entity.UniqueId"),
                        (data: any) => {
                            this._service.deleteTemplateCollaboration(data,
                                (data: any) => {
                                    this._grdTemplateCollaboration.get_masterTableView().deleteSelectedItems();
                                    this.resetControlState();
                                    this._loadingPanel.hide(this.grdTemplateCollaborationId);
                                },
                                (exception: ExceptionDTO) => {
                                    this._loadingPanel.hide(this.grdTemplateCollaborationId);
                                     this._uscNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                                   if (!jQuery.isEmptyObject(this._uscNotification)) {
                                   this._uscNotification.showNotification(exception);
                }  
                                }
                            );
                        },
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.grdTemplateCollaborationId);
                            this._uscNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                            if (!jQuery.isEmptyObject(this._uscNotification)) {
                            this._uscNotification.showNotification(exception);
                }  
                        }
                    );
                }
                catch (error)
                {                    
                    this._loadingPanel.hide(this.grdTemplateCollaborationId);
                    this.showNotificationMessage(this.uscNotificationId,"Errore in eliminazione del template");
                    console.log(JSON.stringify(error));
                }                
            }
        }, 300, 160);
    }

    /**
     *------------------------- Methods -----------------------------
     */

    /**
     * Metodo di inizializzazione della classe
     */
    initialize(): void {
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._btnNew = <Telerik.Web.UI.RadButton>$find(this.btnNewId);
        this._btnNew.add_clicked(this.btnNew_OnClicked);
        this._btnDelete = <Telerik.Web.UI.RadButton>$find(this.btnDeleteId);
        this._btnDelete.add_clicked(this.btnDelete_OnClicked);
        this._grdTemplateCollaboration = <Telerik.Web.UI.RadGrid>$find(this.grdTemplateCollaborationId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
      }

    private resetControlState(): void {
        this._btnNew = <Telerik.Web.UI.RadButton>$find(this.btnNewId);
        this._btnDelete = <Telerik.Web.UI.RadButton>$find(this.btnDeleteId);
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }

    protected showWarningMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showWarningMessage(customMessage)
        }
    }


}
export = TbltTemplateCollaborationManager;