/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import TemplateDocumentRepositoryModel = require('App/Models/Templates/TemplateDocumentRepositoryModel');
import TemplateDocumentRepositoryStatus = require('App/Models/Templates/TemplateDocumentRepositoryStatus');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import TemplateDocumentRepositoryService = require('App/Services/Templates/TemplateDocumentRepositoryService');
import uscTemplateDocumentRepository = require('UserControl/uscTemplateDocumentRepository');
import RadTreeNodeViewModel = require('App/ViewModels/Telerik/RadTreeNodeViewModel');
import StringHelper = require('App/Helpers/StringHelper');
import WindowHelper = require('App/Helpers/WindowHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');



declare var Page_IsValid: any;

class TbltTemplateDocumentRepository {
    uscTemplateDocumentRepositoryId: string;
    ajaxLoadingPanelId: string;
    splitterPageId: string;
    pnlDetailsId: string;
    pnlInformationsId: string;
    lblVersionId: string;
    lblStatusId: string;
    lblObjectId: string;
    btnAggiungiId: string;
    btnModificaId: string;
    btnVisualizzaId: string;
    btnEliminaId: string;
    btnLogId: string;
    btnEliminaUniqueId: string;
    lblTagsId: string;
    previewSplitterId: string;
    previewPaneId: string;
    managerId: string;
    ajaxManagerId: string;
    uscNotificationId: string;
    lblIdentifierId: string;

    private _templateDocumentRepositoryService: TemplateDocumentRepositoryService;
    private _uscTemplateDocumentRepository: uscTemplateDocumentRepository;
    private _btnAggiungi: Telerik.Web.UI.RadButton;
    private _btnElimina: Telerik.Web.UI.RadButton;
    private _btnModifica: Telerik.Web.UI.RadButton;
    private _btnVisualizza: Telerik.Web.UI.RadButton;
    private _btnLog: Telerik.Web.UI.RadButton;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _previewSplitter: Telerik.Web.UI.RadSplitter;
    private _previewPane: Telerik.Web.UI.RadPane;
    private _resizeTO: any;

    /**
     * Costruttore
     * @param serviceConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateDocumentRepository");
        if (!serviceConfiguration) {
            this.showNotificationMessage(this.uscNotificationId,"Errore in inizializzazione. Nessun servizio configurato per il Deposito Documentale");
            return;
        }
        this._templateDocumentRepositoryService = new TemplateDocumentRepositoryService(serviceConfiguration);
    }

    /**
    *------------------------- Events -----------------------------
    */

    /**
     * Evento scatenato al click del pulsante Aggiungi
     * @param sender
     * @param eventArgs
     */
    btnAggiungi_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        this._manager.add_close(this.closeInsertWindow);
        this._manager.open('../Tblt/TbltTemplateDocumentRepositoryGes.aspx?Action=Insert', 'windowManageTemplate', null);
        this._manager.center();
    }

    /**
     * Evento scatenato al click del pulsante Modifica
     * @param sender
     * @param eventArgs
     */
    btnModifica_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        let userControl: uscTemplateDocumentRepository = $('#'.concat(this.uscTemplateDocumentRepositoryId)).data();
        let selectedTemplate: TemplateDocumentRepositoryModel = userControl.getSelectedTemplateDocument();
        if (selectedTemplate == undefined) {
            this.showWarningMessage(this.uscNotificationId,'Selezionare almeno un template');
            return;
        }

        this._manager.add_close(this.closeEditWindow);
        this._manager.open('../Tblt/TbltTemplateDocumentRepositoryGes.aspx?Action=Edit&TemplateId='.concat(selectedTemplate.UniqueId, "&TemplateIdArchiveChain=", selectedTemplate.IdArchiveChain), 'windowManageTemplate', null);
        this._manager.center();
    }

    /**
     * Evento scatenato al click del pulsante Visualizza
     * @param sender
     * @param eventArgs
     */
    btnVisualizza_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        let userControl: uscTemplateDocumentRepository = $('#'.concat(this.uscTemplateDocumentRepositoryId)).data();
        let selectedTemplate: TemplateDocumentRepositoryModel = userControl.getSelectedTemplateDocument();
        if (selectedTemplate == undefined) {
            this.showNotificationMessage(this.uscNotificationId,'Selezionare almeno un template');
            return;
        }

        let url: string = "../Viewers/TemplateDocumentViewer.aspx?IsPreview=false&IdChain=".concat(selectedTemplate.IdArchiveChain, "&Label=", selectedTemplate.Name);
        window.location.href = url;
    }

    /**
     * Evento scatenato al click del pulsante Elimina
     * @param sender
     * @param eventArgs
     */
    btnElimina_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        let userControl: uscTemplateDocumentRepository = $('#'.concat(this.uscTemplateDocumentRepositoryId)).data();
        let templateToDelete: TemplateDocumentRepositoryModel = userControl.getSelectedTemplateDocument();
        if (templateToDelete == undefined) {
            this.showNotificationMessage(this.uscNotificationId,'Selezionare almeno un elemento');
            return;
        }

        this._manager.remove_close(this.closeEditWindow);
        this._manager.remove_close(this.closeInsertWindow);
        this._manager.radconfirm("Sei sicuro di voler eliminare l'elemento selezionato?", (arg) => {
            if (arg) {
                this.showLoadingPanel(this.splitterPageId);
                (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequestWithTarget(this.btnEliminaUniqueId, templateToDelete.IdArchiveChain);
            }
        }, 300, 160);
    }

    /**
     * Evento scatenato al click del pulsante Log
     * @param sender
     * @param eventArgs
     */
    btnLog_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        let url: string = "../Tblt/TbltLog.aspx?Type=Comm&TableName=TemplateDocumentRepository";
        let userControl: uscTemplateDocumentRepository = $('#'.concat(this.uscTemplateDocumentRepositoryId)).data();
        let selectedTemplate: TemplateDocumentRepositoryModel = userControl.getSelectedTemplateDocument();

        if (selectedTemplate != null) {
            url += "&entityUniqueId=".concat(selectedTemplate.UniqueId);
        }

        this._manager.open(url, 'windowLogTemplate', null);
        this._manager.center();
    }


    /**
    *------------------------- Methods -----------------------------
    */

    /**
     * Metodo di inizializzazione
     */
    initialize() {
        this.hideDetailsPanel();
    
        this._btnAggiungi = <Telerik.Web.UI.RadButton>$find(this.btnAggiungiId);
        this._btnAggiungi.add_clicked(this.btnAggiungi_Clicked);
        this._btnModifica = <Telerik.Web.UI.RadButton>$find(this.btnModificaId);
        this._btnModifica.add_clicked(this.btnModifica_Clicked);
        this._btnVisualizza = <Telerik.Web.UI.RadButton>$find(this.btnVisualizzaId);
        this._btnVisualizza.add_clicked(this.btnVisualizza_Clicked);
        this._btnElimina = <Telerik.Web.UI.RadButton>$find(this.btnEliminaId);
        this._btnElimina.add_clicked(this.btnElimina_Clicked);
        this._btnLog = <Telerik.Web.UI.RadButton>$find(this.btnLogId);
        this._btnLog.add_clicked(this.btnLog_Clicked);
        this._previewSplitter = <Telerik.Web.UI.RadSplitter>$find(this.previewSplitterId);
        this._previewPane = <Telerik.Web.UI.RadPane>$find(this.previewPaneId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);

        this.showLoadingPanel(this.splitterPageId);
        //parte caricamento treeview e dati gia presenti in tabella
        $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_SELECTED_NODE_EVENT, (args, data) => {
            if (data != undefined) {
                try {
                    let node: RadTreeNodeViewModel = new RadTreeNodeViewModel();
                    node.fromJson(data);
                    if (!!node.value) {
                        this.loadTemplateDocumentRepositoryDetails(node.attributes.UniqueId);
                        this.setButtonVisibility(true);
                        this.setPreviewSize();
                    } else {
                        this.hideDetailsPanel();
                        this.setButtonVisibility(false);
                    }
                } catch (error) {
                    this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                    console.log((<Error>error).message);
                }
            }
        });

        $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_START_LOAD_EVENT, (args) => {
            this.hideDetailsPanel();
            this.showLoadingPanel(this.splitterPageId);
        });

        $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_END_LOAD_EVENT, (args) => {
            this.hideLoadingPanel(this.splitterPageId);
        });

        $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_ERROR_EVENT, (args, error) => {
            this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
            console.log((<Error>error).message);
            this.hideLoadingPanel(this.splitterPageId);
        });

        this.setPreviewSize();
        $(window).resize((eventObject: JQueryEventObject) => {
            if (this._resizeTO) clearTimeout(this._resizeTO);
            this._resizeTO = setTimeout(() => {
                this.setPreviewSize();
            }, 500);
        });
    }

    setPreviewSize(): void {
        let panelInformationHeight: number = $('#'.concat(this.pnlInformationsId)).height();
        let contentHeight: number = $('#divContent').height();
        let previewHeightComputed: number = contentHeight - (panelInformationHeight + 85);
        this._previewSplitter.set_height(previewHeightComputed.toString().concat('px'));
        this._previewSplitter.set_width($('.rpTemplate').width().toString().concat('px'));
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
     * Nasconde il pannello dei dettagli
     */
    private hideDetailsPanel(): void {
        $('#'.concat(this.pnlDetailsId)).hide();
    }

    /**
     * Visualizza il pannello dei dettagli
     */
    private showDetailsPanel(): void {
        $('#'.concat(this.pnlDetailsId)).show();
    }

    /**
     * Metodo che recupera i metadati di un template e li imposta nella pagina.
     * Gestisce anche le logiche di visualizzazione dei pulsanti e pannelli nella pagina.
     * @param templateDocumentId
     */
    private loadTemplateDocumentRepositoryDetails(templateDocumentId: string) {
        this.showLoadingPanel(this.pnlDetailsId);
        this._templateDocumentRepositoryService.getTemplateById(templateDocumentId,
            (data: any) => {
                if (data == null) return;
                let templateDocument: TemplateDocumentRepositoryModel = <TemplateDocumentRepositoryModel>data;
                this.setDetailPanelControls(templateDocument);
                let url: string = "../Viewers/TemplateDocumentViewer.aspx?IsPreview=true&IdChain=".concat(templateDocument.IdArchiveChain, "&Label=", templateDocument.Name);
                this._previewPane.set_contentUrl(url);
                this.hideLoadingPanel(this.pnlDetailsId);
            },
            (exception: ExceptionDTO) => {
                this.hideLoadingPanel(this.pnlDetailsId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );

        this.showDetailsPanel();
    }

    /**
     * Imposta i valori del Template selezionato nel pannelo dei dettagli
     * @param templateDocument
     */
    private setDetailPanelControls(templateDocument: TemplateDocumentRepositoryModel) {
        $("#".concat(this.lblVersionId)).html(templateDocument.Version.toString());
        $("#".concat(this.lblStatusId)).html(this.mappingStatusLabel(templateDocument));
        $("#".concat(this.lblObjectId)).html(templateDocument.Object);
        $("#".concat(this.lblTagsId)).html(templateDocument.QualityTag.split(';').join(', '));
        $("#".concat(this.lblIdentifierId)).html(templateDocument.UniqueId);
    }

    /**
     * Metodo che prepara il modello da passare al datasource del RadTagCloud
     * @param tags
     */
    private mappingStatusLabel(templateDocument: TemplateDocumentRepositoryModel): string {
        switch (templateDocument.Status) {
            case TemplateDocumentRepositoryStatus.Available:
                return 'Attivo';
            case TemplateDocumentRepositoryStatus.Draft:
                return 'Bozza';
            case TemplateDocumentRepositoryStatus.NotAvailable:
                return 'Non attivo';
        }
    }

    /**
     * Metodo che setta la visibilità dei pulsanti
     * @param templateDocumentNodeSelected
     */
    private setButtonVisibility(templateDocumentNodeSelected: boolean) {
        this._btnAggiungi.set_enabled(!templateDocumentNodeSelected);
        this._btnModifica.set_enabled(templateDocumentNodeSelected);
        this._btnElimina.set_enabled(templateDocumentNodeSelected);
        this._btnVisualizza.set_enabled(templateDocumentNodeSelected);
        this._btnLog.set_enabled(templateDocumentNodeSelected);
    }

    /**
     * Metodo che resetta lo stato dei pulsanti dato dall'ajaxificazione degli stessi
     */
    private resetButtonsState() {
        this._btnAggiungi = <Telerik.Web.UI.RadButton>$find(this.btnAggiungiId);
        this._btnModifica = <Telerik.Web.UI.RadButton>$find(this.btnModificaId);
        this._btnVisualizza = <Telerik.Web.UI.RadButton>$find(this.btnVisualizzaId);
        this._btnElimina = <Telerik.Web.UI.RadButton>$find(this.btnEliminaId);
        //Reset in quanto il pulsante viene ricreato
        this._btnElimina.add_clicked(this.btnElimina_Clicked);
        this._btnLog = <Telerik.Web.UI.RadButton>$find(this.btnLogId);
    }

    /**
     * Callback per la cancellazione di un template
     */
    private deleteCallback() {
        try {
            let userControl: uscTemplateDocumentRepository = $('#'.concat(this.uscTemplateDocumentRepositoryId)).data();
            let templateToDelete: TemplateDocumentRepositoryModel = userControl.getSelectedTemplateDocument();

            this._templateDocumentRepositoryService.deleteTemplateDocument(templateToDelete,
                (data: any) => {
                    try {
                        userControl.removeTemplate(templateToDelete);
                        this.hideDetailsPanel();
                        this.setButtonVisibility(false);
                        this.hideLoadingPanel(this.splitterPageId);
                    } catch (error) {
                        this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                        console.log((<Error>error).message);
                    }
                },
                (exception: ExceptionDTO): void => {
                    this.hideLoadingPanel(this.splitterPageId);
                    this.showNotificationException(this.uscNotificationId, exception);
                });
        } catch (error) {
            this.hideLoadingPanel(this.splitterPageId);
            this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
            console.log((<Error>error).message);
        }
    }

    closeInsertWindow = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument() != null) {
            let userControl: uscTemplateDocumentRepository = $('#'.concat(this.uscTemplateDocumentRepositoryId)).data();
            userControl.refreshTemplates(true);
        }
    }

    closeEditWindow = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument() != undefined) {
            let userControl: uscTemplateDocumentRepository = $('#'.concat(this.uscTemplateDocumentRepositoryId)).data();
            let entity: TemplateDocumentRepositoryModel = <TemplateDocumentRepositoryModel>{};
            entity = JSON.parse(args.get_argument())
            userControl.refreshTemplates(true);
            this.setDetailPanelControls(entity);        
        }
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
    protected showWarningMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showWarningMessage(customMessage)
        }
    }

}

export = TbltTemplateDocumentRepository;