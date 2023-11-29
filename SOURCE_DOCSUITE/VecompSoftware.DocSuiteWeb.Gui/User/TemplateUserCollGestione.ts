/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import TemplateCollaborationService = require('App/Services/Templates/TemplateCollaborationService');
import TemplateCollaborationModel = require('App/Models/Templates/TemplateCollaborationModel');
import TemplateCollaborationStatus = require('App/Models/Templates/TemplateCollaborationStatus');
import CollaborationDocumentType = require('App/Models/Collaborations/CollaborationDocumentType');
import JsonParameter = require('App/Models/Commons/JsonParameter');
import AjaxModel = require('App/Models/AjaxModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import TemplateCollaborationRepresentationType = require('App/Models/Templates/TemplateCollaborationRepresentationType');
import CrossWindowMessagingSender = require('App/Core/Messaging/CrossWindowMessagingSender');
import TemplatesConstants = require('App/Core/Templates/TemplatesConstants');

declare var Page_IsValid: any;
class TemplateUserCollGestione {
    btnConfirmId: string;
    btnConfirmUniqueId: string;
    ajaxLoadingPanelId: string;
    pnlMainPanelId: string;
    pnlDocumentUnitDraftId: string;
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
    parentId: string;
    btnPublishId: string;
    btnPublishUniqueId: string;
    ajaxFlatLoadingPanelId: string;
    pnlHeaderId: string;
    pnlButtonsId: string;
    radWindowManagerId: string;
    btnDeleteId: string;
    uscNotificationId: string;
    chkDocumentUnitDraftEnabledId: string;
    chkSecretaryViewRightEnabledId: string;
    chkPopupDocumentNotSignedAlertEnabledId: string;
    chkBtnCheckoutEnabledId: string;
    rowDocumentUnitDraftId: string;

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
    private _currentTemplateRepresentationType: TemplateCollaborationRepresentationType;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _chkDocumentUnitDraftEnabled: JQuery;
    private _messageSender: CrossWindowMessagingSender;
    // model loaded when Action=Edit
    private _editingModel: TemplateCollaborationModel;

    // model loaded when Action=Insert
    // the parent can be a folder or the fixed template
    private _parentModel: TemplateCollaborationModel;
    // the fixed template is a top level template which provides the document type for the current new template
    private _parentTopLevel: TemplateCollaborationModel;

    private _chkSecretaryViewRightEnabled(): JQuery {
        return $(`#${this.chkSecretaryViewRightEnabledId}`);
    }
    private _chkPopupDocumentNotSignedAlertEnabled(): JQuery {
        return $(`#${this.chkPopupDocumentNotSignedAlertEnabledId}`);
    }
    private _chkBtnCheckoutEnabled(): JQuery {
        return $(`#${this.chkBtnCheckoutEnabledId}`);
    }

    static INSERT_ACTION: string = "Insert";
    static EDIT_ACTION: string = "Edit";
    static PRECOMPILE_PARAM: string = "DocumentUnitDraftEditorEnabled";
    static SECRETARY_PARAM: string = "SecretaryViewRightEnabled";
    static POPUP_PARAM: string = "PopUpDocumentNotSignedAlertEnabled";
    static BTNCHEKOUT_PARAM: string = "BtnCheckoutEnabled";

    /**
     * Costruttore
     * @param serviceConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateCollaboration");
        if (!serviceConfiguration) {
            this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione. Nessun servizio configurato per il Template di collaborazione");
            return;
        }

        this._service = new TemplateCollaborationService(serviceConfiguration);
        this._messageSender = new CrossWindowMessagingSender(window.parent);
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato al click del pulsante Salva Bozza
     * @param sender
     * @param args
     */
    btnConfirm_OnClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
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
    btnPublish_OnClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        if (Page_IsValid) {
            this.showLoadingPanels();
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequestWithTarget(this.btnPublishUniqueId, '');
        }
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
    btnDelete_OnClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._manager.radconfirm("Sei sicuro di voler eliminare il template corrente?", (arg) => {
            if (arg) {
                this.showLoadingPanels();
                try {
                    this._service.getById(this.templateId,
                        (data: any) => {
                            this._service.deleteTemplateCollaboration(data,
                                (data: any) => {
                                    this._messageSender.SendMessage(TemplatesConstants.Events.EventTemplateDeleted, this.templateId);
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
                    this.showNotificationMessage(this.uscNotificationId, "Errore in eliminazione del template");
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
        // document type drop down values are binded in the aspx vb code of the current page
        this._ddlDocumentType = <Telerik.Web.UI.RadDropDownList>$find(this.ddlDocumentTypeId);
        this._ddlDocumentType.set_enabled(false);
        this._ddlDocumentType.findItemByValue('P').select();
        this._ddlSpecificDocumentType = <Telerik.Web.UI.RadDropDownList>$find(this.ddlSpecificDocumentTypeId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        this._rblPriority = $("#".concat(this.rblPriorityId));
        this._currentTemplateIsLocked = false;
        this._chkDocumentUnitDraftEnabled = $("#".concat(this.chkDocumentUnitDraftEnabledId));
        $("#".concat(this.rowDocumentUnitDraftId)).hide();
        $("#specificTypeRow").hide();

        try {
            if (this.TemplateIsBeingEdited()) {
                this.LoadEditingTemplate()
                    .then(() => {
                        this.EvaluatePageStyleBasedOnDocumentType();
                        this.EvaluateDropdownBasedOnDocumentType();
                    });
            }
            else if (this.TemplateIsBeingCreated()) {
                this.LoadParentOfNewTemplate()
                    .then(() => {
                        this.EvaluatePageStyleBasedOnDocumentType();
                        this.EvaluateDropdownBasedOnDocumentType();
                    });
            }

        } catch (err) {
            this.showNotificationMessage(this.uscNotificationId, 'Errore in caricamento dati del Template');
            console.error(JSON.stringify(err));
        }

    }

    private LoadParentOfNewTemplate(): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred();
        this.showLoadingPanels();

        try {
            //parentId is available only when creating a new element
            this._service.getById(this.parentId,
                model => {
                    this._parentModel = model;
                    if (this._parentModel === null || this._parentModel === undefined) {
                        this._btnConfirm.set_enabled(false);
                        this._btnPublish.set_enabled(false);
                        this._btnDelete.set_enabled(false);
                        throw 'Nessun template trovato';
                    }

                    // the hierarchic structure allows us to place a template under a folder under a fixed template
                    // the folder can be the parent but it's not aware of the document type of the parent (which needs
                    // to be passed to the new child)

                    // we get the parent because we need to validate he exists to insert a new node
                    // we get the top level parent (under the root) to get the document type and pass it on
                    let fixedTemplatePath = this._parentModel.TemplateCollaborationPath.substr(0, 3);
                    this._service.getByTemplateCollaborationPath(fixedTemplatePath,
                        model => {
                            this._parentTopLevel = model;

                            if (this._parentTopLevel === null || this._parentTopLevel === undefined) {
                                this._btnConfirm.set_enabled(false);
                                this._btnPublish.set_enabled(false);
                                this._btnDelete.set_enabled(false);
                                throw 'Nessun template trovato';
                            }
                            this.hideLoadingPanels();

                            deferred.resolve();
                        },
                        err => {
                            this.hideLoadingPanels();
                            this.showNotificationException(this.uscNotificationId, err);
                            deferred.reject(err);
                        });

                }, err => {
                    this.hideLoadingPanels();
                    this.showNotificationException(this.uscNotificationId, err);
                    deferred.reject(err);
                });

        } catch (err) {
            this.hideLoadingPanels();
            this.showNotificationMessage(this.uscNotificationId, 'Errore in caricamento dati del Template');
            deferred.reject(err);
        }

        return deferred;
    }

    private LoadEditingTemplate(): JQueryDeferred<void> {
        let deferred: JQueryDeferred<void> = $.Deferred()
        this.showLoadingPanels();
        try {
            this._service.getById(this.templateId,
                model => {
                    this._editingModel = model;
                    if (this._editingModel == undefined) {
                        this._btnConfirm.set_enabled(false);
                        this._btnPublish.set_enabled(false);
                        this._btnDelete.set_enabled(false);
                        throw 'Nessun template trovato';
                    }

                    this._currentTemplateIsLocked = this._editingModel.IsLocked;
                    this._currentTemplateRepresentationType = this._editingModel.RepresentationType;
                    this._btnPublish.set_enabled(<any>TemplateCollaborationStatus[this._editingModel.Status] != TemplateCollaborationStatus.Active);
                    this._btnDelete.set_enabled(!this._editingModel.IsLocked);
                    this.fillPageFromEntity(this._editingModel);
                    var ajaxmodel: AjaxModel = { ActionName: 'LoadFromEntity', Value: [JSON.stringify(this._editingModel)] };
                    (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxmodel));
                    this.hideLoadingPanels();

                    deferred.resolve();
                }, err => {
                    this.hideLoadingPanels();
                    this.showNotificationException(this.uscNotificationId, err);
                    deferred.reject(err);
                });

        } catch (error) {
            this.hideLoadingPanels();
            this.showNotificationMessage(this.uscNotificationId, 'Errore in caricamento dati del Template');
            deferred.reject(error);
        }
        return deferred;
    }

    private EvaluatePageStyleBasedOnDocumentType(): void {
        if (this.TemplateIsBeingCreated()) {
            let documentType = this._parentTopLevel.DocumentType;
            let pageType = this.getPageTypeFromDocumentType(documentType);
            this.changeBodyClass(pageType);
            $("#specificTypeRow").hide();

            if (documentType === CollaborationDocumentType[CollaborationDocumentType.UDS]) {
                $("#specificTypeRow").show();
            }
        }

        if (this.TemplateIsBeingEdited()) {
            let documentType: string = this._editingModel.DocumentType;
            let pageType = this.getPageTypeFromDocumentType(documentType);
            this.changeBodyClass(pageType);
            $("#specificTypeRow").hide();

            if (documentType === CollaborationDocumentType[CollaborationDocumentType.UDS] || +documentType >= 100) {
                $("#specificTypeRow").show();
            }
        }
    }

    private EvaluateDropdownBasedOnDocumentType(): void {
        let documentType = '';
        if (this.TemplateIsBeingCreated()) {
            documentType = this._parentTopLevel.DocumentType;
        }

        if (this.TemplateIsBeingEdited()) {
            documentType = this._editingModel.DocumentType;

        }
        for (let node of this._ddlDocumentType.get_items().toArray()) {
            if (node.get_value() === documentType) {
                node.set_selected(true);
            }
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
        let documentType = this._ddlDocumentType.get_selectedItem().get_value();

        if (documentType == CollaborationDocumentType[CollaborationDocumentType.UDS]) {
            if (this._ddlSpecificDocumentType.get_selectedItem().get_value() != '') {
                entity.DocumentType = this._ddlSpecificDocumentType.get_selectedItem().get_value();
            } else {
                entity.DocumentType = documentType;
            }
        } else {
            entity.DocumentType = this._ddlDocumentType.get_selectedItem().get_value();
        }
        entity.IdPriority = this._rblPriority.find(":checked").val();
        if (this.action == TemplateUserCollGestione.EDIT_ACTION) {
            entity.IsLocked = this._currentTemplateIsLocked;
            entity.RepresentationType = this._currentTemplateRepresentationType;
        } else {
            entity.IsLocked = false;
            entity.ParentInsertId = this._parentModel.UniqueId;
            entity.RepresentationType = TemplateCollaborationRepresentationType.Template;
        }
        entity.Status = TemplateCollaborationStatus.Draft;

        let jpars: JsonParameter[] = [];
        let documentUnitDraftParam: JsonParameter = new JsonParameter();
        documentUnitDraftParam.Name = TemplateUserCollGestione.PRECOMPILE_PARAM;
        documentUnitDraftParam.PropertyType = 16;
        documentUnitDraftParam.ValueBoolean = $("#".concat(this.chkDocumentUnitDraftEnabledId)).is(":checked");

        let secretaryRightParam: JsonParameter = new JsonParameter();
        secretaryRightParam.Name = TemplateUserCollGestione.SECRETARY_PARAM;
        secretaryRightParam.PropertyType = 16;
        secretaryRightParam.ValueBoolean = this._chkSecretaryViewRightEnabled().is(":checked");

        let popupDocumentNotSignedAlertParam: JsonParameter = new JsonParameter();
        popupDocumentNotSignedAlertParam.Name = TemplateUserCollGestione.POPUP_PARAM;
        popupDocumentNotSignedAlertParam.PropertyType = 16;
        popupDocumentNotSignedAlertParam.ValueBoolean = this._chkPopupDocumentNotSignedAlertEnabled().is(":checked");

        let btncheckoutParam: JsonParameter = new JsonParameter();
        btncheckoutParam.Name = TemplateUserCollGestione.BTNCHEKOUT_PARAM;
        btncheckoutParam.PropertyType = 16;
        btncheckoutParam.ValueBoolean = this._chkBtnCheckoutEnabled().is(":checked");

        jpars.push(documentUnitDraftParam);
        jpars.push(secretaryRightParam);
        jpars.push(popupDocumentNotSignedAlertParam);
        jpars.push(btncheckoutParam);

        entity.JsonParameters = JSON.stringify(jpars);

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
            let apiAction: any = this.TemplateIsBeingCreated() ?
                (m, c, e) => this._service.insertTemplateCollaboration(m, c, e)
                : (m, c, e) => this._service.updateTemplateCollaboration(m, c, e);

            apiAction(entity,
                (data: any) => {
                    if (publishing) {
                        this._service.publishTemplate(data,
                            (data: any) => {
                                this.resetControlState();
                                this._btnPublish.set_enabled(false);
                                this.hideLoadingPanels();
                                alert("Template pubblicato correttamente");
                                this._messageSender.SendMessage(TemplatesConstants.Events.EventTemplateCreated, data);
                            },
                            (exception: ExceptionDTO) => {
                                this.resetControlState();
                                this.hideLoadingPanels();
                                this.showNotificationException(this.uscNotificationId, exception);
                            }
                        );
                    } else {
                        alert("Template salvato correttamente");
                        if (this.TemplateIsBeingCreated()) {
                            // ParentId is a fake field instructing WebApi where to insert the element. It's not saved in the database(TODO: why is it not saved?)
                            // We need this id when we send the message because in TemplateCollaboration the tree must know what to refresh
                            data.ParentInsertId = entity.ParentInsertId;

                        }
                        this.resetControlState();
                        this._btnPublish.set_enabled(true);
                        this.hideLoadingPanels();
                        this._messageSender.SendMessage(TemplatesConstants.Events.EventTemplateCreated, data);
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
            this.showNotificationMessage(this.uscNotificationId, "Errore in esecuzione dell'attività di salvataggio.");
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
        if (entity.JsonParameters) {
            let jsonParms: JsonParameter[] = JSON.parse(entity.JsonParameters);
            let draftParm: JsonParameter[] = jsonParms.filter(f => f.Name == TemplateUserCollGestione.PRECOMPILE_PARAM);
            if (draftParm && draftParm.length > 0) {
                $("#".concat(this.chkDocumentUnitDraftEnabledId)).prop("checked", draftParm[0].ValueBoolean);
            }

            let secreataryParm: JsonParameter[] = jsonParms.filter(f => f.Name == TemplateUserCollGestione.SECRETARY_PARAM);
            if (secreataryParm && secreataryParm.length > 0) {
                this._chkSecretaryViewRightEnabled().prop("checked", secreataryParm[0].ValueBoolean);
            }
            let popupDocumentNotSignedAlertParam: JsonParameter[] = jsonParms.filter(f => f.Name == TemplateUserCollGestione.POPUP_PARAM);
            if (popupDocumentNotSignedAlertParam && popupDocumentNotSignedAlertParam.length > 0) {
                this._chkPopupDocumentNotSignedAlertEnabled().prop("checked", popupDocumentNotSignedAlertParam[0].ValueBoolean);
            }
            let btncheckoutParam: JsonParameter[] = jsonParms.filter(f => f.Name == TemplateUserCollGestione.BTNCHEKOUT_PARAM);
            if (btncheckoutParam && btncheckoutParam.length > 0) {
                this._chkBtnCheckoutEnabled().prop("checked", btncheckoutParam[0].ValueBoolean);
            }

        }
        let collaborationDocumentTypeName: string = this.getCollaborationDocumentTypeFromEntity(entity)
        this._ddlDocumentType.findItemByValue(collaborationDocumentTypeName).select();
        if (collaborationDocumentTypeName == CollaborationDocumentType[CollaborationDocumentType.UDS] && !isNaN(parseInt(entity.DocumentType))) {
            this._ddlSpecificDocumentType.findItemByValue(entity.DocumentType).select();
        }
        this.showPanel(entity.DocumentType);

        $("#".concat(this.rblPriorityId, " :radio[value='", entity.IdPriority, "']")).prop("checked", true);
    }
    private showPanel(documentType: string): void {
        if (documentType == CollaborationDocumentType[CollaborationDocumentType.P] || !isNaN(parseInt(documentType))) {
            $(`#${this.rowDocumentUnitDraftId}`).show();
        }

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

    private TemplateIsBeingEdited(): boolean {
        return this.action == TemplateUserCollGestione.EDIT_ACTION;
    }

    private TemplateIsBeingCreated(): boolean {
        return this.action == TemplateUserCollGestione.INSERT_ACTION;
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