/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import TemplateCollaborationService = require('App/Services/Templates/TemplateCollaborationService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import TemplateCollaborationModel = require('App/Models/Templates/TemplateCollaborationModel');
import TemplateCollaborationRepresentationType = require('App/Models/Templates/TemplateCollaborationRepresentationType');
import TemplateCollaborationStatus = require('App/Models/Templates/TemplateCollaborationStatus');
import CrossWindowMessagingSender = require('App/Core/Messaging/CrossWindowMessagingSender');
import TemplatesConstants = require('App/Core/Templates/TemplatesConstants');

class TemplateUserCollCartellaGestione {
    btnConfirmId: string;
    ajaxManagerId: string;
    txtNameId: string;
    btnDeleteId: string;
    uscNotificationId: string;
    action: string;
    templateId: string;
    parentId: string;
    radWindowManagerId: string;
    pnlHeaderId: string;
    pnlButtonsId: string;
    ajaxFlatLoadingPanelId: string;

    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _btnDelete: Telerik.Web.UI.RadButton;
    private _service: TemplateCollaborationService;
    private _txtName: Telerik.Web.UI.RadTextBox;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _flatLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _configuration: ServiceConfiguration;
    // model loaded when Action=Edit
    private _editingModel: TemplateCollaborationModel;
    // model loaded when Action=Insert
    private _parentModel: TemplateCollaborationModel;
    private _messageSender: CrossWindowMessagingSender;

    static INSERT_ACTION: string = "Insert";
    static EDIT_ACTION: string = "Edit";

    static EVENT_ON_FOLDER_CREATED = 'EVENT_ON_FOLDER_CREATED';
    static EVENT_ON_FOLDER_DELETED = 'EVENT_ON_FOLDER_DELETED';

    /**
     * Costruttore
     * @param serviceConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateCollaboration");
        if (!serviceConfiguration) {
            this.ShowNotificationMessage(this.uscNotificationId, "Errore in inizializzazione. Nessun servizio configurato per il Template di collaborazione");
            return;
        }
        this._configuration = serviceConfiguration;
        this._service = new TemplateCollaborationService(serviceConfiguration);
        this._messageSender = new CrossWindowMessagingSender(window.parent);
    }

    public initialize(): void {
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_OnClicked);

        //TODO FT25745: uncomment when delete folder functionality is implemented in WebApi
        //this._btnDelete = <Telerik.Web.UI.RadButton>$find(this.btnDeleteId);
        //this._btnDelete.add_clicked(this.btnDelete_OnClicked);
        //this._btnDelete.set_visible(this.action == TemplateUserCollCartellaGestione.EDIT_ACTION);

        this._flatLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxFlatLoadingPanelId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);

        if (this.action === TemplateUserCollCartellaGestione.INSERT_ACTION) {
            this.LoadParentOfNewFolderTemplate();
        }

        if (this.action === TemplateUserCollCartellaGestione.EDIT_ACTION) {
            this.LoadEditingFolderTemplate();
        }
    }

    private LoadEditingFolderTemplate(): void {
        try {
            this.ShowLoadingPanels();
            this._service.getById(this.templateId,
                model => {
                    this._editingModel = model;

                    if (this._editingModel === null || this._editingModel === undefined) {
                        this._btnConfirm.set_enabled(false);
                        this._btnDelete.set_enabled(false);
                        throw 'Nessun template trovato';
                    }

                    this._txtName.set_value(this._editingModel.Name);
                    this.HideLoadingPanels();
                },
                err => {
                    this.HideLoadingPanels();
                    this.ShowNotificationException(this.uscNotificationId, err);
                    console.error(JSON.stringify(err));
                });

        } catch (err) {
            this.ShowNotificationMessage(this.uscNotificationId, 'Errore in caricamento dati del Template');
            console.error(JSON.stringify(err));
            this.HideLoadingPanels();
        }
    }

    private LoadParentOfNewFolderTemplate(): void {
        this.ShowLoadingPanels();

        try {
            this._service.getById(this.parentId,
                model => {
                    // parentId is available only when creating a new element
                    // we query for the parent model to determine that it exists
                    this._parentModel = model;
                    if (this._parentModel === null || this._parentModel === undefined) {
                        this._btnConfirm.set_enabled(false);
                        this._btnDelete.set_enabled(false);
                        throw 'Nessun template trovato';
                    }
                    this.HideLoadingPanels();
                }, err => {
                    this.HideLoadingPanels();
                    this.ShowNotificationException(this.uscNotificationId, err);
                    console.error(JSON.stringify(err));
                });

        } catch (err) {
            this.HideLoadingPanels();
            this.ShowNotificationMessage(this.uscNotificationId, 'Errore in caricamento dati del Template');
            console.error(JSON.stringify(err));
        }
    }

    public btnConfirm_OnClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        if (this.TemplateFolderIsBeingCreated()) {
            return this.ConfirmInsertTemplate();
        }

        if (this.TemplateFolderIsBeingEdited()) {
            this.ConfirmEditTemplate();
        }
    }

    public btnDelete_OnClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        if (this.TemplateFolderIsBeingCreated()) {
            // this should never happen because the button is disabled
            return;
        }

        this._manager.radconfirm("Sei sicuro di voler eliminare il template cartella corrente?", (arg) => {
            if (arg) {
                this.ShowLoadingPanels();
                try {
                    this._service.getById(this.templateId,
                        (data: any) => {
                            this._service.deleteTemplateCollaboration(data,
                                (data: any) => {
                                    this._messageSender.SendMessage(TemplatesConstants.Events.EventFolderDeleted, this.templateId);
                                    this.HideLoadingPanels();
                                },
                                (exception: ExceptionDTO) => {
                                    this.HideLoadingPanels();
                                    this.ShowNotificationException(this.uscNotificationId, exception);
                                }
                            );
                        },
                        (exception: ExceptionDTO) => {
                            this.HideLoadingPanels();
                            this.ShowNotificationException(this.uscNotificationId, exception);
                        }
                    );

                } catch (err) {
                    this.ShowNotificationMessage(this.uscNotificationId, 'Errore in caricamento dati del Template');
                    console.error(JSON.stringify(err));
                    this.HideLoadingPanels();
                }
            }
        });
    }

    private ConfirmEditTemplate() {
        this._editingModel.Name = this._txtName.get_value();

        this.ShowLoadingPanels();

        try {
            this._service.updateTemplateCollaboration(this._editingModel,
                (data: TemplateCollaborationModel) => {
                    this.HideLoadingPanels();
                    this._messageSender.SendMessage(TemplatesConstants.Events.EventFolderCreated, data);
                    // model edit saved succesfully
                },
                (exception: ExceptionDTO) => {
                    this.HideLoadingPanels();
                    this.ShowNotificationException(this.uscNotificationId, exception);
                });
        } catch (error) {
            this.HideLoadingPanels();
            this.ShowNotificationMessage(this.uscNotificationId, "Error editing the template");
            console.log(JSON.stringify(error));
        }
    }

    private ConfirmInsertTemplate() {
        let insertModel = new TemplateCollaborationModel();
        insertModel.Name = this._txtName.get_value();
        insertModel.RepresentationType = TemplateCollaborationRepresentationType.Folder;
        insertModel.Status = TemplateCollaborationStatus.Active;
        insertModel.ParentInsertId = this.parentId;

        this.ShowLoadingPanels();

        try {
            this._service.insertTemplateCollaboration(insertModel,
                (data: TemplateCollaborationModel) => {
                    // in the database this is a fake id which may not be loaded back
                    // in the message dispatacher we need it to reload the parent to include the child
                    data.ParentInsertId = insertModel.ParentInsertId;
                    this.HideLoadingPanels();
                    this._messageSender.SendMessage(TemplatesConstants.Events.EventFolderCreated, data);
                },
                (exception: ExceptionDTO) => {
                    this.HideLoadingPanels();
                    this.ShowNotificationException(this.uscNotificationId, exception);
                });
        } catch (error) {
            this.HideLoadingPanels();
            this.ShowNotificationMessage(this.uscNotificationId, "Error inserting the template");
            console.log(JSON.stringify(error));
        }
    }

    private TemplateFolderIsBeingEdited(): boolean {
        return this.action === TemplateUserCollCartellaGestione.EDIT_ACTION;
    }

    private TemplateFolderIsBeingCreated(): boolean {
        return this.action === TemplateUserCollCartellaGestione.INSERT_ACTION;
    }

    /**
     * Visualizza i pannelli di loading nella pagina
     */
    private ShowLoadingPanels(): void {
        this._flatLoadingPanel.show(this.pnlHeaderId);
        this._flatLoadingPanel.show(this.pnlButtonsId);
    }

    /**
     * Nasconde i pannelli di loading della pagina
     */
    private HideLoadingPanels(): void {
        this._flatLoadingPanel.hide(this.pnlHeaderId);
        this._flatLoadingPanel.hide(this.pnlButtonsId);
    }

    protected ShowNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.ShowNotificationMessage(uscNotificationId, customMessage)
        }

    }

    protected ShowNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }
}

export = TemplateUserCollCartellaGestione;