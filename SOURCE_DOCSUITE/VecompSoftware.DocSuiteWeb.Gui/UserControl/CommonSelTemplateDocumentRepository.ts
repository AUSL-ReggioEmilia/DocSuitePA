/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import uscTemplateDocumentRepository = require('UserControl/uscTemplateDocumentRepository');
import TemplateDocumentRepositoryModel = require('App/Models/Templates/TemplateDocumentRepositoryModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class CommonSelTemplateDocumentRepository {
    btnConfirmId: string;
    uscTemplateDocumentRepositoryId: string;
    ajaxLoadingPanelId: string;
    pnlPageId: string;
    callerId: string;
    uscNotificationId: string;

    private _btnConfirm: Telerik.Web.UI.RadButton;

    /**
     * Costruttore
     */
    constructor() {

    }

    /**
     * Evento scatenato al click del pulsante conferma
     * @param sender
     * @param eventArgs
     */
    btnConfirm_OnClicked = (sender: Telerik.Web.UI.RadButton, arg: Telerik.Web.UI.RadButtonEventArgs) => {
        this.returnValuesJson();
    }

    /**
     * Gestisce il callback degli errori
     */
    private errorEventCallback(exception): void {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception && exception instanceof ExceptionDTO) {
                uscNotification.showNotification(exception);
            }
            else {
                console.log(JSON.stringify(exception));
                uscNotification.showNotificationMessage("Errore nel caricamento.");
            }
          
        }
        
    }

    /**
     * Inizializzazione
     */
    initialize(): void {
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_OnClicked);

        this.showLoadingPanel(this.pnlPageId);
        $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_START_LOAD_EVENT, (args) => {
            this.showLoadingPanel(this.pnlPageId);
        });

        $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_END_LOAD_EVENT, (args) => {
            this.hideLoadingPanel(this.pnlPageId);
        });

        $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_ERROR_EVENT, (args, data) => {
            this.errorEventCallback(data);
            this.hideLoadingPanel(this.pnlPageId);
        });
    }

    /**
     * Visualizza un nuovo loading panel nella pagina
     */
    private showLoadingPanel(updatedElementId: string): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.show(updatedElementId);
    }

    /**
     * Nasconde il loading panel nella pagina
     */
    private hideLoadingPanel(updatedElementId: string): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.hide(updatedElementId);
    }

    /**
     * Metodo che gestire il ritorno dei valori alla pagina chiamante
     * @param close
     */
    private returnValuesJson(): void {
        let userControl: uscTemplateDocumentRepository = <uscTemplateDocumentRepository>$('#'.concat(this.uscTemplateDocumentRepositoryId)).data();
        let selectedTemplate: TemplateDocumentRepositoryModel = userControl.getSelectedTemplateDocument();
        if (!selectedTemplate) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage('Nessun template selezionato');
            }
            return;
        }

        let returnArgs: string = selectedTemplate.IdArchiveChain.concat('|', selectedTemplate.Name);
        this.closeWindow(returnArgs);
    }

    /**
     * Metodo per il recupero di una specifica radwindow
     */
    getRadWindow(): Telerik.Web.UI.RadWindow {
        let radWindow: Telerik.Web.UI.RadWindow;
        if ((<any>window).radWindow) {
            radWindow = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        }
        else if ((<any>window.frameElement).radWindow) {
            radWindow = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        }
        return radWindow;
    }

    /**
     * Metodo per chiudere una radwindow con passaggio di argomenti
     * @param args
     */
    private closeWindow(args: string): void {
        this.getRadWindow().close(args);
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }
}

export = CommonSelTemplateDocumentRepository;