/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import TemplateCollaborationService = require('App/Services/Templates/TemplateCollaborationService');
import TemplateCollaborationModel = require('App/Models/Templates/TemplateCollaborationModel');
import TemplateCollaborationStatus = require('App/Models/Templates/TemplateCollaborationStatus');
import CollaborationDocumentType = require('App/Models/Collaborations/CollaborationDocumentType');
import AjaxModel = require('App/Models/AjaxModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');

declare var Page_IsValid: any;
class TemplateUserCollGestione {
    btnConfirmId: string;
    btnConfirmUniqueId: string;
    ajaxLoadingPanelId: string;
    pnlMainPanelId: string;
    ajaxManagerId: string;
    txtNameId: string;
    txtObjectId: string;
    txtNoteId: string;
    ddlDocumentTypeId: string;
    ddlDocumentTypeUniqueId: string;
    ddlSpecificDocumentTypeId: string;
    rblPriorityId: string;
    action: string;
    templateId: string;
    btnPublishId: string;
    btnPublishUniqueId: string;
    ajaxFlatLoadingPanelId: string;
    pnlHeaderId: string;
    pnlButtonsId: string;
    radWindowManagerId: string;
    btnDeleteId: string;
        uscNotificationId: string;

    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _btnPublish: Telerik.Web.UI.RadButton;
    private _btnDelete: Telerik.Web.UI.RadButton;
    private _service: TemplateCollaborationService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _flatLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _txtName: Telerik.Web.UI.RadTextBox;
    private _txtObject: Telerik.Web.UI.RadTextBox;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _ddlDocumentType: Telerik.Web.UI.RadDropDownList;
    private _ddlSpecificDocumentType: Telerik.Web.UI.RadDropDownList;
    private _rblPriority: JQuery;
    private _currentTemplateIsLocked: boolean;
    private _manager: Telerik.Web.UI.RadWindowManager;

    static INSERT_ACTION: string = "Insert";
    static EDIT_ACTION: string = "Edit";

    /**
     * Costruttore
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
     * Evento scatenato al click del pulsante Salva Bozza
     * @param sender
     * @param args
     */
    btnConfirm_OnClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (Page_IsValid) {
            this.showLoadingPanels();
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequestWithTarget(this.btnConfirmUniqueId, '');
        }
    }

    /**
     * Evento scatenato al click del pulsante Pubblica
     * @param sender
     * @param args
     */
    btnPublish_OnClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (Page_IsValid) {
            this.showLoadingPanels();
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequestWithTarget(this.btnPublishUniqueId, '');
        }
    }

    /**
     * Evento scatenato al cambio di selezione della tipologia documento
     * @param sender
     * @param args
     */
    ddlDocumentType_selectedIndexChanged = (sender: Telerik.Web.UI.RadDropDownList, args: Telerik.Web.UI.DropDownListIndexChangedEventArgs) => {
        let documentType: string = this.getPageTypeFromDocumentType(sender.get_selectedItem().get_value());
        this.changeBodyClass(documentType);
        $("#specificTypeRow").hide();
        if (sender.get_selectedItem().get_value() == CollaborationDocumentType[CollaborationDocumentType.UDS]) {
            $("#specificTypeRow").show();
        }
        var ajaxmodel: AjaxModel = { ActionName: 'DocumentTypeChanged', Value: [documentType] };
        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxmodel));
    }

    /**
     * Evento scatenato all'uscita del focus di una RadTextbox per validare i caratteri inseriti
     * @param sender
     * @param args
     */
    changeStrWithValidCharacterHandler = (sender: Telerik.Web.UI.RadTextBox, args: any): void => {
        (<any>window).ChangeStrWithValidCharacter(sender.get_element());
    }

    /**
     * Evento scatenato al click del pulsante Elimina
     * @param sender
     * @param args
     */
    btnDelete_OnClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._manager.radconfirm("Sei sicuro di voler eliminare il template corrente?", (arg) => {
            if (arg) {
                this.showLoadingPanels();
                try {
                    this._service.getById(this.templateId,
                        (data: any) => {
                            this._service.deleteTemplateCollaboration(data,
                                (data: any) => {
                                    window.location.href = "../Tblt/TbltTemplateCollaborationManager.aspx?Type=Comm";
                                },
                                (exception: ExceptionDTO) => {
                                    this.hideLoadingPanels();
                                    this.showNotificationException(this.uscNotificationId, exception);
                                }
                            );
                        },
                        (exception: ExceptionDTO) => {
                            this.hideLoadingPanels();
                            this.showNotificationException(this.uscNotificationId, exception);
                        }
                    );
                }
                catch (error) {
                    this.hideLoadingPanels();
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
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_OnClicked);
        this._btnPublish = <Telerik.Web.UI.RadButton>$find(this.btnPublishId);
        this._btnPublish.add_clicked(this.btnPublish_OnClicked);
        this._btnPublish.set_visible(this.action == TemplateUserCollGestione.EDIT_ACTION);
        this._btnDelete = <Telerik.Web.UI.RadButton>$find(this.btnDeleteId);
        this._btnDelete.add_clicked(this.btnDelete_OnClicked);
        this._btnDelete.set_visible(this.action == TemplateUserCollGestione.EDIT_ACTION);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._flatLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxFlatLoadingPanelId);
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
        this._txtObject = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._ddlDocumentType = <Telerik.Web.UI.RadDropDownList>$find(this.ddlDocumentTypeId);
        this._ddlDocumentType.add_selectedIndexChanged(this.ddlDocumentType_selectedIndexChanged);
        this._ddlDocumentType.findItemByValue('P').select();
        this._ddlSpecificDocumentType = <Telerik.Web.UI.RadDropDownList>$find(this.ddlSpecificDocumentTypeId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        this._rblPriority = $("#".concat(this.rblPriorityId));
        this._currentTemplateIsLocked = false;

        $("#specificTypeRow").hide();

        if (this.action == TemplateUserCollGestione.EDIT_ACTION) {
            this.showLoadingPanels();
            try {
                this._service.getById(this.templateId,
                    (data: any) => {
                        try {
                            if (data == undefined) {
                                this._btnConfirm.set_enabled(false);
                                this._btnPublish.set_enabled(false);
                                this._btnDelete.set_enabled(false);
                                throw 'Nessun template trovato';
                            }

                            this._currentTemplateIsLocked = data.IsLocked;
                            this._btnPublish.set_enabled(<any>TemplateCollaborationStatus[data.Status] != TemplateCollaborationStatus.Active);
                            this._btnDelete.set_enabled(!data.IsLocked);
                            this.fillPageFromEntity(data);
                            var ajaxmodel: AjaxModel = { ActionName: 'LoadFromEntity', Value: [JSON.stringify(data)] };
                            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxmodel));
                        } catch (error) {
                            this.hideLoadingPanels();
                            this.showNotificationMessage(this.uscNotificationId,'Errore in caricamento dati del Template');
                            console.log(JSON.stringify(error));
                        }
                    },
                    (exception: ExceptionDTO) => {
                        this.hideLoadingPanels();
                        this.showNotificationException(this.uscNotificationId, exception);
                    }
                );
            } catch (error) {
                this.hideLoadingPanels();
                this.showNotificationMessage(this.uscNotificationId,'Errore in caricamento dati del Template');
                console.log(JSON.stringify(error));
            }
        } else {
            this.ddlDocumentType_selectedIndexChanged(this._ddlDocumentType, undefined);
        }
    }

    /**
     * Metodo che esegue il fill dei dati dalla pagina nell'entità TemplateCollaborationModel
     * @param entity
     */
    private fillEntity(entity: TemplateCollaborationModel): TemplateCollaborationModel {
        entity.Name = this._txtName.get_value();
        entity.Object = this._txtObject.get_value();
        entity.Note = this._txtNote.get_value();
        if (this._ddlDocumentType.get_selectedItem().get_value() == CollaborationDocumentType[CollaborationDocumentType.UDS]) {
            if (this._ddlSpecificDocumentType.get_selectedItem().get_value() != '') {
                entity.DocumentType = this._ddlSpecificDocumentType.get_selectedItem().get_value();
            } else {
                entity.DocumentType = this._ddlDocumentType.get_selectedItem().get_value();
            }
        } else {
            entity.DocumentType = this._ddlDocumentType.get_selectedItem().get_value();
        }
        entity.IdPriority = this._rblPriority.find(":checked").val();
        if (this.action == TemplateUserCollGestione.EDIT_ACTION) {
            entity.IsLocked = this._currentTemplateIsLocked;
        } else {
            entity.IsLocked = false;
        }
        entity.Status = TemplateCollaborationStatus.Draft;
        return entity;
    }

    /**
     * Ripristina lo stato dei controlli della pagina
     */
    private resetControlState(): void {
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
        this._txtObject = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
    }

    /**
     * Callback per l'inserimento/aggiornamento di un TemplateCollaborationModel
     * @param entity
     */
    confirmCallback(entity: TemplateCollaborationModel, publishing: boolean) {
        try {
            entity = this.fillEntity(entity);
            let apiAction: any = this.action == TemplateUserCollGestione.INSERT_ACTION ? (m, c, e) => this._service.insertTemplateCollaboration(m, c, e) : (m, c, e) => this._service.updateTemplateCollaboration(m, c, e);

            apiAction(entity,
                (data: any) => {
                    if (publishing) {
                        this._service.publishTemplate(data,
                            (data: any) => {
                                this.resetControlState();
                                this._btnPublish.set_enabled(false);
                                this.hideLoadingPanels();
                                alert("Template pubblicato correttamente");
                            },
                            (exception: ExceptionDTO) => {
                                this.resetControlState();
                                this.hideLoadingPanels();
                              this.showNotificationException(this.uscNotificationId, exception);
                            }
                        );
                    } else {
                       alert("Template salvato correttamente");
                        if (this.action == 'Insert') {
                            window.location.href = "../User/TemplateUserCollGestione.aspx?Action=Edit&Type=".concat(this.getPageTypeFromDocumentType(data.Environment), "&TemplateId=", data.UniqueId);
                        }
                        this.resetControlState();
                        this._btnPublish.set_enabled(true);
                        this.hideLoadingPanels();
                    }
                },
                (exception: ExceptionDTO) => {
                    this.hideLoadingPanels();
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        }
        catch (error) {
            this.hideLoadingPanels();
            this.showNotificationMessage(this.uscNotificationId,"Errore in esecuzione dell'attività di salvataggio.");
            console.log(JSON.stringify(error));
        }
    }

    /**
     * Metodo che permette di modificare lo stile della pagina
     * @param classType
     */
    changeBodyClass(classType: string): void {
        $("body").attr("class", classType.toLowerCase());
    }

    getCollaborationDocumentTypeFromEntity(entity: TemplateCollaborationModel) {
        if (!isNaN(parseInt(entity.DocumentType))) {
            return CollaborationDocumentType[CollaborationDocumentType.UDS];
        }

        return entity.DocumentType;
    }

    /**
     * Metodo che recupera il type corretto in base alla selezione della Tipologia di documento
     * @param documentType
     */
    getPageTypeFromDocumentType(documentType: string): string {
        if (documentType == CollaborationDocumentType[CollaborationDocumentType.A] || documentType == CollaborationDocumentType[CollaborationDocumentType.D]) {
            return 'resl';
        }

        if (documentType == CollaborationDocumentType[CollaborationDocumentType.S]) {
            return 'series';
        }

        if (documentType == CollaborationDocumentType[CollaborationDocumentType.UDS] || !isNaN(parseInt(documentType))) {
            return 'uds';
        }
        return 'prot';
    }

    /**
     * Esegue il fill dei dati dalle WebAPI nella pagina
     * @param entity
     */
    private fillPageFromEntity(entity: TemplateCollaborationModel): void {
        this._txtName.set_value(entity.Name);
        this._txtNote.set_value(entity.Note);
        this._txtObject.set_value(entity.Object);
        let collaborationDocumentTypeName: string = this.getCollaborationDocumentTypeFromEntity(entity)
        this._ddlDocumentType.findItemByValue(collaborationDocumentTypeName).select();
        if (collaborationDocumentTypeName == CollaborationDocumentType[CollaborationDocumentType.UDS] && !isNaN(parseInt(entity.DocumentType))) {
            this._ddlSpecificDocumentType.findItemByValue(entity.DocumentType).select();
        }
        $("#".concat(this.rblPriorityId, " :radio[value='", entity.IdPriority, "']")).prop("checked", true);
    }

    /**
     * Callback scatenato al load dei dati dalle WebAPI lato code-behind
     */
    loadFromEntityCallback(): void {
        this.hideLoadingPanels();
    }

    /**
     * Visualizza i pannelli di loading nella pagina
     */
    private showLoadingPanels(): void {
        this._loadingPanel.show(this.pnlMainPanelId);
        this._flatLoadingPanel.show(this.pnlHeaderId);
        this._flatLoadingPanel.show(this.pnlButtonsId);
    }

    /**
     * Nasconde i pannelli di loading della pagina
     */
    private hideLoadingPanels(): void {
        this._loadingPanel.hide(this.pnlMainPanelId);
        this._flatLoadingPanel.hide(this.pnlHeaderId);
        this._flatLoadingPanel.hide(this.pnlButtonsId);
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

export = TemplateUserCollGestione;