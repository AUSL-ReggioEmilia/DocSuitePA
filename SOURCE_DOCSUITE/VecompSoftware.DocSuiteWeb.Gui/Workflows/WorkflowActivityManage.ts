/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import WorkflowActivityModel = require("App/Models/Workflows/WorkflowActivityModel");
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import WorkflowAccountModel = require('App/Models/Workflows/WorkflowAccountModel');
import AjaxModel = require('App/Models/AjaxModel');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import FascicleDocumentService = require('App/Services/Fascicles/FascicleDocumentService');
import uscFascicleSearch = require('UserControl/uscFascicleSearch');
import WorkflowAuthorizationService = require('App/Services/Workflows/WorkflowAuthorizationService');
import UDSRepositoryService = require('App/Services/UDS/UDSRepositoryService');
import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import FascicleFolderService = require('App/Services/Fascicles/FascicleFolderService');
import WorkflowProperty = require('App/Models/Workflows/WorkflowProperty');
import WorkflowReferenceModel = require('App/Models/Workflows/WorkflowReferenceModel');
import Environment = require('App/Models/Environment');
import FascicleSummaryFolderViewModel = require('App/ViewModels/Fascicles/FascicleSummaryFolderViewModel');

declare var ValidatorEnable: any;
class WorkflowActivityManage {
    ddlNameWorkflowId: string;
    tblFilterStateId: string;
    uscProponenteId: string;
    uscDestinatariId: string;
    tdpDateId: string;
    rtbNoteId: string;
    rtbParereId: string;
    lblActivityDateId: string;
    cmdDocumentsId: string;
    documentContainerId: string;
    currentUser: string;
    ajaxManagerId: string;
    uniqueId: string;
    lblProponenteId: string;
    lblDestinatarioId: string;
    lblRegistrationDateId: string;
    lblSubjectId: string;
    ddlUDSArchivesId: string;
    grdUDId: string;
    uscNotificationId: string;
    rblDocumentUnitId: string;
    pnlUDSID: string;
    pnlFascicleSelectId: string;
    rfvUDSArchivesId: string;
    cmdInizializeId: string;
    ajaxLoadingPanelId: string;
    currentPageId: string;
    cmdManageActivityId: string;
    uscFascicleSearchId: string;
    btnConfirmId: string;
    rblDocumentUnitUniqueId: string;
    ddlUDSArchivesUniqueId: string;
    panelDocumentUnitSelectId: string;
    panelManageId: string;
    panelDocumentId: string;

    private static INSERT_MISCELLANEA: string = "InsertMiscellanea";
    private static WORKFLOW_ACTIVITY_EXPAND_PROPERTIES: string[] =
        [
            "WorkflowProperties"
        ];

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _serviceConfigurations: ServiceConfiguration[];
    private _service: WorkflowActivityService;
    private _ddlUDSArchives: Telerik.Web.UI.RadComboBox;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _fascicleDocumentService: FascicleDocumentService;
    private _loadingDeferreds: JQueryDeferred<void>[] = [];
    private _workflowAuthorizationService: WorkflowAuthorizationService;
    private _udsRepositoryService: UDSRepositoryService;
    private _gridUD: Telerik.Web.UI.RadGrid;
    private _fascicleFolderService: FascicleFolderService;
    private _btnConfirm: Telerik.Web.UI.RadButton;

    private pnlFascicleSelect(): JQuery {
        return $(`#${this.pnlFascicleSelectId}`);
    }

    private pnlArchives(): JQuery {
        return $(`#${this.pnlUDSID}`);
    }

    private lblProponente(): JQuery {
        return $(`#${this.lblProponenteId}`);
    }

    private lblDestinatario(): JQuery {
        return $(`#${this.lblDestinatarioId}`);
    }

    private lblRegistrationDate(): JQuery {
        return $(`#${this.lblRegistrationDateId}`);
    }

    private lblSubject(): JQuery {
        return $(`#${this.lblSubjectId}`);
    }

    private rblDocumentUnit(): JQuery {
        return $(`#${this.rblDocumentUnitId}`);
    }

    private rfvUDSArchives(): JQuery {
        return $(`#${this.rfvUDSArchivesId}`);
    }

    private panelDocumentUnitSelect(): JQuery {
        return $(`#${this.panelDocumentUnitSelectId}`);
    }

    private panelManage(): JQuery {
        return $(`#${this.panelManageId}`);
    }

    private panelDocument(): JQuery {
        return $(`#${this.panelDocumentId}`);
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     *------------------------- Events -----------------------------
     */

    protected radioListButtonChanged = () => {
        let checkedChoice: string = this.rblDocumentUnit().find('input:checked').val();

        this.rfvUDSArchives().hide();
        this.pnlArchives().hide();
        this.pnlFascicleSelect().hide();
        this.panelManage().hide();
        this.panelDocument().show()
        this.panelDocumentUnitSelect().addClass(" t-col-12");
        this.panelDocumentUnitSelect().removeClass(" t-col-4");



        this._gridUD.get_masterTableView().showColumn(3);

        ValidatorEnable(document.getElementById(this.rfvUDSArchivesId), false);

        switch (checkedChoice) {
            case "Archivi": {
                this.panelManage().show();
                this.pnlArchives().show();
                this.rfvUDSArchives().show();

                this.panelDocumentUnitSelect().addClass(" t-col-4");
                this.panelDocumentUnitSelect().removeClass(" t-col-12");


                ValidatorEnable(document.getElementById(this.rfvUDSArchivesId), true);
                break;
            }
            case "Fascicolo": {
                this.pnlFascicleSelect().show();
                this.panelManage().show();

                this.panelDocumentUnitSelect().addClass(" t-col-4");
                this.panelDocumentUnitSelect().removeClass(" t-col-12");

                this._gridUD.get_masterTableView().hideColumn(3);
                break;
            }
            case "PEC": {
                this.panelDocument().hide()
            }
        }

        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxManager.ajaxRequestWithTarget(this.rblDocumentUnitUniqueId, '');
    }

    initializeRequest = (sender: any, args: Sys.WebForms.InitializeRequestEventArgs) => {
        if (args.get_postBackElement().id.indexOf(this.btnConfirmId) != -1) {
            (<any>args).set_cancel(true);
            sender._form.__EVENTTARGET.value = args.get_postBackElement().id.replace(/\_/g, "$");
            sender._form.__EVENTARGUMENT.value
            sender._form.submit();
            return;
        }
    }

    ddlUDSArchives_selectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxManager.ajaxRequestWithTarget(this.ddlUDSArchivesUniqueId, '');
    }

    /**
     *------------------------- Methods -----------------------------
     */

    initialize(): void {
        Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(this.initializeRequest);

        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
        this._service = new WorkflowActivityService(serviceConfiguration);

        serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowAuthorization");
        this._workflowAuthorizationService = new WorkflowAuthorizationService(serviceConfiguration);

        serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleDocument");
        this._fascicleDocumentService = new FascicleDocumentService(serviceConfiguration);

        serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "UDSRepository");
        this._udsRepositoryService = new UDSRepositoryService(serviceConfiguration);

        let fascicleFolderServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleFolder");
        this._fascicleFolderService = new FascicleFolderService(fascicleFolderServiceConfiguration);

        this._loadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
        this._ddlUDSArchives = $find(this.ddlUDSArchivesId) as Telerik.Web.UI.RadComboBox;
        this._ddlUDSArchives.add_selectedIndexChanged(this.ddlUDSArchives_selectedIndexChanged);
        this._btnConfirm = $find(this.btnConfirmId) as Telerik.Web.UI.RadButton;
        this._gridUD = $find(this.grdUDId) as Telerik.Web.UI.RadGrid;

        this.rblDocumentUnit().on('change', this.radioListButtonChanged);

        let checkedChoice: string = this.rblDocumentUnit().find('input:checked').val();
        if (checkedChoice === "PEC") {
            this.panelDocument().hide();
        }
        else {
            this.panelDocument().show();
        }

        this._loadingPanel.show(this.currentPageId);
        this.checkUserRights()
            .done((isValid) => {
                if (!isValid) {
                    this.showNotificationException(this.uscNotificationId, "Non è possibile gestire l'attività richiesta. Verificare se si dispone di sufficienti autorizzazioni");
                    return;
                }

                this.initializeArchivePanel()
                    .done(() => this.loadData(this.uniqueId)
                        .fail((exception: any) => this.showNotificationException(this.uscNotificationId, exception))
                        .always(() => this._loadingPanel.hide(this.currentPageId)))
                    .fail((exception: any) => {
                        this._loadingPanel.hide(this.currentPageId);
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            })
            .fail((exception: any) => {
                this._loadingPanel.hide(this.currentPageId);
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private initializeArchivePanel(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this.pnlArchives().hide();
        this.rfvUDSArchives().hide();

        ValidatorEnable(document.getElementById(this.rfvUDSArchivesId), false);

        this._udsRepositoryService.getInsertableRepositoriesByTypology(this.currentUser.split("\\")[1], this.currentUser.split("\\")[0], null, false,
            (data: any) => {
                if (!data) {
                    return promise.resolve();
                }

                let repositories: UDSRepositoryModel[] = data as UDSRepositoryModel[];
                let comboItem: Telerik.Web.UI.RadComboBoxItem;
                for (let repository of repositories) {
                    comboItem = new Telerik.Web.UI.RadComboBoxItem();
                    comboItem.set_text(repository.Name);
                    comboItem.set_value(repository.UniqueId);
                    this._ddlUDSArchives.get_items().add(comboItem);
                }

                this._ddlUDSArchives.clearSelection();
                if (repositories.length == 1) {
                    this._ddlUDSArchives.set_selectedIndex(0);
                } else {
                    let emptyComboItem = new Telerik.Web.UI.RadComboBoxItem();
                    emptyComboItem.set_text("");
                    emptyComboItem.set_value("");
                    this._ddlUDSArchives.get_items().insert(0, emptyComboItem);
                }
                promise.resolve();
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        );

        return promise.promise();
    }

    private checkUserRights(): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this._workflowAuthorizationService.isUserAuthorized(this.currentUser, this.uniqueId,
            (data: any) => {
                if (data == undefined) {
                    return promise.reject("Errore nella ricerca delle autorizzazioni utente");
                }
                promise.resolve(data as boolean);
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    private loadData(uniqueId: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._service.getWorkflowActivityById(uniqueId,
            (data) => {
                if (!data) {
                    return promise.reject(`Nessuna attività di workflow trovata con ID ${uniqueId}`);
                }

                try {
                    let workflowActivity: WorkflowActivityModel = data as WorkflowActivityModel;
                    let workflowProponenteJson = workflowActivity.WorkflowProperties.filter(x => x.Name === WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER)[0];

                    if (workflowProponenteJson && workflowProponenteJson.ValueString) {
                        let workflowProponente: WorkflowAccountModel = JSON.parse(workflowProponenteJson.ValueString);
                        this.lblProponente().html(workflowProponente.DisplayName);
                    }

                    let workflowNoteJson = workflowActivity.WorkflowProperties.filter(x => x.Name === WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION)[0];
                    if (workflowNoteJson) {
                        this.lblSubject().html(workflowNoteJson.ValueString);
                    }

                    this.lblDestinatario().html(this.currentUser);
                    this.lblRegistrationDate().html(moment(workflowActivity.RegistrationDate).format("DD/MM/YYYY"));

                    let defaultProperty: WorkflowProperty = workflowActivity.WorkflowProperties.filter(x => x.Name === WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_ACTIVITY_MANAGE_DEFAULT_TYPE)[0];

                    if (defaultProperty) {
                        this.rblDocumentUnit().find(`[value=${defaultProperty.ValueString}]`).prop("checked", true);

                        this.radioListButtonChanged();
                        let referenceModel: WorkflowProperty = workflowActivity.WorkflowProperties.filter(x => x.Name === WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL)[0];

                        if (defaultProperty.ValueString === "Fascicolo" && referenceModel) {
                            let fascicleReferenceModel: WorkflowReferenceModel = JSON.parse(referenceModel.ValueString);
                            if (fascicleReferenceModel.ReferenceType === Environment.Fascicle) {
                                let fascicleModel: FascicleModel = JSON.parse(fascicleReferenceModel.ReferenceModel);
                                let uscFascicleSearch: uscFascicleSearch = $(`#${this.uscFascicleSearchId}`).data() as uscFascicleSearch;
                                uscFascicleSearch.loadFascicle(fascicleModel.UniqueId, true);
                                uscFascicleSearch._loadFascFoldersData(fascicleModel.UniqueId);
                                uscFascicleSearch.showContentPanel();
                            }
                        }
                    }

                    let ajaxModel: AjaxModel = <AjaxModel>{};
                    ajaxModel.Value = new Array<string>();
                    ajaxModel.Value.push(JSON.stringify(workflowActivity.IdArchiveChain));
                    ajaxModel.ActionName = "LoadWorkFlowDocument";

                    this._loadingDeferreds.push(promise);
                    this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
                    this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                } catch (error) {
                    console.error(JSON.stringify(error));
                    promise.reject("E' avvenuto un errore durante il caricamento delle informazioni dell'attività");
                }
            },
            (exception: ExceptionDTO) => promise.reject(exception), WorkflowActivityManage.WORKFLOW_ACTIVITY_EXPAND_PROPERTIES);
        return promise.promise();
    }

    loadCallback(errorMessage: string): void {
        if (errorMessage) {
            this._loadingDeferreds.forEach((promise: JQueryDeferred<void>) => {
                let errorDto: ExceptionDTO = new ExceptionDTO();
                errorDto.statusText = errorMessage;
                promise.reject(errorDto);
            });
            return;
        }

        this._loadingDeferreds.forEach((promise: JQueryDeferred<void>) => promise.resolve());
    }

    protected showNotificationException(uscNotificationId: string, exception: string)
    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO)
    protected showNotificationException(uscNotificationId: string, exception: any) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception && exception instanceof ExceptionDTO) {
                uscNotification.showNotification(exception);
            }
            else {
                uscNotification.showNotificationMessage(exception);
            }
        }
    }

    hasSelectedFascicle() {
        let uscFascicleSearch: uscFascicleSearch = $(`#${this.uscFascicleSearchId}`).data() as uscFascicleSearch;
        if (!jQuery.isEmptyObject(uscFascicleSearch)) {
            let selectedFascicle: FascicleModel = uscFascicleSearch.getSelectedFascicle();
            return selectedFascicle != null;
        }
    }

    cmdFascMiscellaneaInsert_Click = (sender: any, args: any) => {
        let uscFascicleSearch: uscFascicleSearch = $(`#${this.uscFascicleSearchId}`).data() as uscFascicleSearch;
        if (!jQuery.isEmptyObject(uscFascicleSearch)) {
            let selectedFascicle: FascicleModel = uscFascicleSearch.getSelectedFascicle();
            if (selectedFascicle) {
                let selectedFascicleFolder: FascicleSummaryFolderViewModel = uscFascicleSearch.getSelectedFascicleFolder();
                let ajaxModel: AjaxModel = <AjaxModel>{};
                ajaxModel.Value = new Array<string>();
                ajaxModel.Value.push(selectedFascicle.UniqueId);
                ajaxModel.Value.push(selectedFascicleFolder.UniqueId);
                ajaxModel.ActionName = WorkflowActivityManage.INSERT_MISCELLANEA;
                if (!selectedFascicleFolder) {
                    this.showNotificationException(this.uscNotificationId, "Nessuna cartella fascicolo selezionata");
                    return;
                }

                (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
            } else {
                this._btnConfirm.enableAfterSingleClick();
                this.showNotificationException(this.uscNotificationId, "Nessun fascicolo selezionato");
            }
        }
    }

    confirmCallback(idChain: string, idFascicle: string, isNewArchiveChain: boolean, idFasciclefolder: string, errorMessage: string) {
        if (errorMessage) {
            this.showNotificationException(this.uscNotificationId, errorMessage);
            this._loadingPanel.hide(this.currentPageId);
            this._btnConfirm = $find(this.btnConfirmId) as Telerik.Web.UI.RadButton;
            this._btnConfirm.enableAfterSingleClick();

            return;
        }

        if (isNewArchiveChain) {
            let fascicleDocumentModel: FascicleDocumentModel = <FascicleDocumentModel>{};
            fascicleDocumentModel.ChainType = ChainType.Miscellanea;
            fascicleDocumentModel.IdArchiveChain = idChain;
            fascicleDocumentModel.Fascicle = new FascicleModel();
            fascicleDocumentModel.Fascicle.UniqueId = idFascicle;
            this._fascicleFolderService.getById(idFasciclefolder,
                (data: any) => {
                    if (!data) {
                        this._loadingPanel.hide(this.currentPageId);
                        this.showNotificationException(this.uscNotificationId, "E' avvenuto un errore durante il processo di fascicolazione");
                        return;
                    }

                    fascicleDocumentModel.FascicleFolder = data as FascicleFolderModel;
                    this._fascicleDocumentService.insertFascicleDocument(fascicleDocumentModel,
                        (data: any) => window.location.href = `../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${idFascicle}`,
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.currentPageId);
                            this.showNotificationException(this.uscNotificationId, exception);
                        }
                    );
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.currentPageId);
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        } else {
            window.location.href = `../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${idFascicle}`;
        }
    }
}
export = WorkflowActivityManage;