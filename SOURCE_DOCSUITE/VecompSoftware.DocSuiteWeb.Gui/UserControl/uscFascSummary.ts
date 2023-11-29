/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleService = require('App/Services/Fascicles/FascicleService');
import DossierFolderService = require('App/Services/Dossiers/DossierFolderService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierFolderStatus = require('App/Models/Dossiers/DossierFolderStatus');

class uscFascSummary {
    lblTitleId: string;
    lblFascicleTypeId: string;
    lblStartDateId: string;
    lblEndDateId: string;
    lblFascicleObjectId: string;
    lblFascicleNoteId: string;
    ajaxLoadingPanelId: string;
    isEditPage: boolean;
    isAuthorizePage: boolean;
    isSummaryLink: boolean;
    pageId: string;
    ajaxManagerId: string;
    btnExpandUDFascicleId: string;
    lblRegistrationUserId: string;
    lblLastChangedDateId: string;
    lblRegistrationDateId: string;
    uscNotificationId: string;
    workflowActivityId: string;
    currentFascicleId: string;
    btnExpandFascInfoId: string;
    fascInfoId: string;
    lblLastChangedUserId: string;
    lblViewFascicleId: string;
    fascCaptionId: string;
    containerRowId: string;
    lblContainerId: string;
    serieLabelId: string;
    serieLabelRowId: string;
    fascicleContainerEnabled: boolean;
    processEnabled: boolean;

    public static LOADED_EVENT: string = "onLoaded";
    public static DATA_LOADED_EVENT: string = "onDataLoaded";
    public static REBIND_EVENT: string = "onRebind";

    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _notification: Telerik.Web.UI.RadNotification;
    private _txtTitleUD: Telerik.Web.UI.RadTextBox;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _lblTitle: JQuery;
    private _lblFascicleType: JQuery;
    private _lblFascicleObject: JQuery;
    private _lblStartDate: JQuery;
    private _lblEndDate: JQuery;
    private _lblFascicleNote: JQuery;
    private _lblRegistrationUser: JQuery;
    private _lblLastChangedDate: JQuery;
    private _lblLastChangedUser: JQuery;
    private _lblRegistrationDate: JQuery;
    private _lblSerieName: JQuery;
    private _service: FascicleService;
    private _dossierFolderService: DossierFolderService;
    private _domainUserService: DomainUserService;
    private _serviceConfigurations: ServiceConfiguration[];
    private _uscNotification: UscErrorNotification
    private _workflowActivityService: WorkflowActivityService;
    private _workflowActivity: WorkflowActivityModel;
    private _btnExpandFascInfo: Telerik.Web.UI.RadButton;
    private _isFascInfoOpen: boolean;
    private _fascInfoContent: JQuery;
    private _lblViewFascicle: JQuery;

    private lblContainer(): JQuery {
        return $(`#${this.lblContainerId}`);
    }

    private containerRow(): JQuery {
        return $(`#${this.containerRowId}`);
    }

    private lblSerieNameRow(): JQuery {
        return $(`#${this.serieLabelRowId}`);
    }

    /**
     * Costruttore
     * @param webApiConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._service = new FascicleService(ServiceConfigurationHelper.getService(serviceConfigurations, "Fascicle"));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    /**
     * Inizializzazione
     */
    initialize() {
        this._lblTitle = $("#".concat(this.lblTitleId));
        this._lblFascicleType = $("#".concat(this.lblFascicleTypeId));
        this._lblFascicleObject = $("#".concat(this.lblFascicleObjectId));
        this._lblStartDate = $("#".concat(this.lblStartDateId));
        this._lblEndDate = $("#".concat(this.lblEndDateId));
        this._lblFascicleNote = $("#".concat(this.lblFascicleNoteId));
        this._lblRegistrationUser = $("#".concat(this.lblRegistrationUserId));
        this._lblLastChangedDate = $("#".concat(this.lblLastChangedDateId));
        this._lblLastChangedUser = $("#".concat(this.lblLastChangedUserId));
        this._lblRegistrationDate = $("#".concat(this.lblRegistrationDateId));
        this._lblSerieName = $("#".concat(this.serieLabelId));
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._btnExpandFascInfo = <Telerik.Web.UI.RadButton>$find(this.btnExpandFascInfoId);
        this._btnExpandFascInfo.addCssClass("dsw-arrow-down");
        this._btnExpandFascInfo.add_clicking(this.btnExpandFascInfo_OnClick);
        this._isFascInfoOpen = true;
        this._fascInfoContent = $("#".concat(this.fascInfoId));
        this._fascInfoContent.show();
        this._lblViewFascicle = $("#".concat(this.lblViewFascicleId));

        const dossierFolderServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DossierFolder");
        this._dossierFolderService = new DossierFolderService(dossierFolderServiceConfiguration);

        const domainUserConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
        this._domainUserService = new DomainUserService(domainUserConfiguration);

        const workflowActivityConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowActivity');
        this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);

        if (this.isSummaryLink) {
            $("#".concat(this.fascCaptionId)).hide();
        }

        if (this.processEnabled) {
            this.lblSerieNameRow().show();
        }

        this.bindLoaded();
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
    * Evento al click del pulsante per la espandere o comprimere il sommario
    */
    btnExpandFascInfo_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (this._isFascInfoOpen) {
            this._isFascInfoOpen = false;
            this._fascInfoContent.hide();
            this._btnExpandFascInfo.removeCssClass("dsw-arrow-down");
            this._btnExpandFascInfo.addCssClass("dsw-arrow-up");
        }
        else {
            this._isFascInfoOpen = true;
            this._fascInfoContent.show();
            this._btnExpandFascInfo.removeCssClass("dsw-arrow-up");
            this._btnExpandFascInfo.addCssClass("dsw-arrow-down");
        }
    }


    /**
     *------------------------- Methods -----------------------------
     */

    /**
     * Scatena l'evento di "load completed" del controllo
     */
    private bindLoaded(): void {
        $("#".concat(this.pageId)).data(this);
        $("#".concat(this.pageId)).triggerHandler(uscFascSummary.LOADED_EVENT);
    }

    /**
     * Carica i dati dello user control
     */
    loadData(fascicle: FascicleModel): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (fascicle == null) {
            return promise.resolve();
        }

        $.when(this.getFascicleUserDisplayName(fascicle.RegistrationUser), this.getFascicleUserDisplayName(fascicle.LastChangedUser))
            .done((registrationUser, lastChangedUser) => {
                fascicle.RegistrationUser = registrationUser;
                fascicle.LastChangedUser = lastChangedUser;
                this.setSummaryData(fascicle)
                    .done(() => promise.resolve())
                    .fail((exception) => {
                        this._uscNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                        if (!jQuery.isEmptyObject(this._uscNotification)) {
                            if (exception instanceof ExceptionDTO) {
                                this._uscNotification.showNotification(exception);
                            } else {
                                this._uscNotification.showNotificationMessage(`E' avvenuto un errore durante il caricamento delle informazioni del Fascicolo: ${exception.message}`);
                            }                            
                        }
                    })
            })

        return promise.promise();
    }

    private getFascicleUserDisplayName(account: string): JQueryPromise<string> {
        let promise: JQueryDeferred<string> = $.Deferred<string>();
        if (!account) {
            return promise.resolve(account);
        }

        this._domainUserService.getUser(account,
            (user: DomainUserModel) => {
                if (user) {
                    return promise.resolve(user.DisplayName);
                }
                return promise.resolve(account);
            },
            (exception: ExceptionDTO) => {
                console.warn(`E' avvenuto un errore durante la ricerca dell'utente ${account}. Viene restituito l'account name.`)
                return promise.resolve(account);
            }
        );
        return promise.promise();
    }

    /**
     * Imposta i dati nel sommario
     * @param fascicle
     */
    private setSummaryData(fascicle: FascicleModel): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        try {
            this._lblViewFascicle.hide();
            let title: string = `${fascicle.Title} - ${fascicle.Category.Name}`;
            this._lblTitle.html(title);
            this._lblSerieName.html("");
            if (fascicle.ProcessLabel && fascicle.DossierFolderLabel) {
                this._lblSerieName.html(`${fascicle.ProcessLabel}/${fascicle.DossierFolderLabel}`);
            }

            if (this.isSummaryLink) {
                this._lblTitle.hide();
                this._lblViewFascicle.show();
                this._lblViewFascicle.html(title);
                this._lblViewFascicle.attr("href", "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(fascicle.UniqueId));
            }

            this._lblFascicleObject.html(fascicle.FascicleObject);

            if ($.type(fascicle.FascicleType) === "string") {
                fascicle.FascicleType = FascicleType[fascicle.FascicleType.toString()];
            }

            let fascicleTypeName: string = "";
            switch (fascicle.FascicleType) {
                case FascicleType.Procedure:
                    fascicleTypeName = "Fascicolo di procedimento";
                    break;
                case FascicleType.Period:
                    fascicleTypeName = "Fascicolo periodico";
                    break;
                case FascicleType.Legacy:
                    fascicleTypeName = "Fascicolo non a norma";
                    break;
                case FascicleType.Activity:
                    fascicleTypeName = "Fascicolo di attività";
                    break;
            }
            this._lblFascicleType.html(fascicleTypeName);

            this._lblStartDate.html(moment(fascicle.StartDate).format("DD/MM/YYYY"));

            this._lblEndDate.html("");
            if (fascicle.EndDate) {
                this._lblEndDate.html(moment(fascicle.EndDate).format("DD/MM/YYYY"));
            }

            this._lblFascicleNote.html(fascicle.Note);

            this._lblLastChangedDate.html("");
            if (fascicle.LastChangedDate) {
                this._lblLastChangedDate.html(moment(fascicle.LastChangedDate).format("DD/MM/YYYY"));
            }

            this._lblLastChangedUser.html("");
            if (fascicle.LastChangedUser) {
                this._lblLastChangedUser.html(fascicle.LastChangedUser);
            }

            this._lblRegistrationDate.html(moment(fascicle.RegistrationDate).format("DD/MM/YYYY"));
            this._lblRegistrationUser.html(fascicle.RegistrationUser);

            this.containerRow().hide();
            if (this.fascicleContainerEnabled && (fascicle.FascicleType == FascicleType.Period
                || fascicle.FascicleType == FascicleType.Procedure)
                && fascicle.Container) {
                this.containerRow().show();
                this.lblContainer().html(fascicle.Container.Name);
            }

            if (!this.workflowActivityId) {
                return promise.resolve();
            }

            this._workflowActivityService.getWorkflowActivity(this.workflowActivityId,
                (data: any) => {
                    if (data == null) return;
                    this._workflowActivity = data;
                    let subject: string;
                    if (this._workflowActivity.WorkflowProperties != null) {

                        subject = this._workflowActivity.WorkflowProperties.filter(function (item) { if (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT) return item; })[0].ValueString;
                        this._lblFascicleNote.html(subject);
                    }
                    promise.resolve();
                },
                (exception: ExceptionDTO) => {
                    promise.reject(exception);
                }
            );
            return promise.promise();
        } catch (exception) {
            return promise.reject(exception);
        }        
    }

    private buildDossierProcessFullNameRecursive(source: DossierFolderModel[], dossierFolder: DossierFolderModel): string {
        let fullName: string = "";
        let paths: string[] = dossierFolder.DossierFolderPath.split('/').filter((item, index) => {
            return !!item;
        });

        if (paths.length > 1) {
            paths.pop();
            let folderPathToCheck: string = `/${paths.join('/')}/`;
            let parentFolder: DossierFolderModel = source.filter((dossierFolder, index) => {
                return dossierFolder.DossierFolderPath == folderPathToCheck;
            })[0];

            if (parentFolder && DossierFolderStatus[parentFolder.Status.toString()] != DossierFolderStatus.InProgress) {
                let parentName: string = this.buildDossierProcessFullNameRecursive(source, parentFolder);
                fullName = parentFolder.Name;
                if (parentName) {
                    fullName = `${parentName}/${parentFolder.Name}`;
                }
            }
        }
        return fullName;
    }
}

export = uscFascSummary;
