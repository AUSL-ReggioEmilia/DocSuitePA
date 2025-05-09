﻿/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />


import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import DossierService = require('App/Services/Dossiers/DossierService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DossierSummaryViewModel = require('App/ViewModels/Dossiers/DossierSummaryViewModel');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import DossierBase = require('Dossiers/DossierBase');
import AjaxModel = require('App/Models/AjaxModel');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');
import WorkflowRoleModel = require('App/Models/Workflows/WorkflowRoleModel');
import WorkflowAuthorization = require('App/Models/Workflows/WorkflowAuthorizationModel');
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import WorkflowRoleModelMapper = require('App/Mappers/Workflows/WorkflowRoleModelMapper');
import WorkflowPropertyModel = require('App/Models/Workflows/WorkflowProperty');
import UscDynamicMetadataSummaryRest = require('UserControl/uscDynamicMetadataSummaryRest');
import uscRoleRest = require('uscRoleRest');
import RoleModel = require('App/Models/Commons/RoleModel');
import uscContattiSelRest = require('UserControl/uscContattiSelRest');
import ContactModel = require('APP/Models/Commons/ContactModel');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import uscCategoryRest = require('UserControl/uscCategoryRest');
import EnumHelper = require('App/Helpers/EnumHelper');
import DossierType = require('App/Models/Dossiers/DossierType');
import DossierStatus = require('App/Models/Dossiers/DossierStatus');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

class uscDossier extends DossierBase {

    lblDossierSubjectId: string;
    lblRegistrationUserId: string;
    lblStartDateId: string;
    lblDossierNoteId: string;
    lblNumberId: string;
    lblYearId: string;
    lblModifiedUserId: string;
    pageId: string;
    lblContainerId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    lblWorkflowProposerRoleId: string;
    lblWorkflowHandlerUserId: string;
    rowWorkflowProposerId: string;
    workflowActivityId: string;
    uscDynamicMetadataSummaryRestId: string;
    metadataRepositoryEnabled: boolean;
    rowMetadataId: string;
    uscRoleRestId: string;
    uscResponsableRoleRestId: string;
    uscContattiSelRestId: string;
    uscCategoryRestId: string;
    lblDossierTypeId: string;
    lblDossierStatusId: string;
    dossierTypologyEnabled: boolean;
    columnDossierTypeKeyId: string;
    columnDossierTypeValueId: string;

    public static LOADED_EVENT: string = "onLoaded";
    public static DATA_LOADED_EVENT: string = "onDataLoaded"

    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _serviceConfigurations: ServiceConfiguration[]
    private _service: DossierService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _domainUserService: DomainUserService;
    private _lblRegistrationUser: JQuery;
    private _lblDossierSubject: JQuery;
    private _lblStartDate: JQuery;
    private _lblDossierNote: JQuery;
    private _lblNumber: JQuery;
    private _lblYear: JQuery;
    private _lblModifiedUser: JQuery;
    private _lblContainer: JQuery;
    private _lblWorkflowHandlerUser: JQuery;
    private _lblWorkflowProposerRole: JQuery;
    private _workflowActivityService: WorkflowActivityService;
    private _workflowActivity: WorkflowActivityModel;
    private _uscNotification: UscErrorNotification;
    private _rowMetadataRepository: JQuery;
    private _uscRoleRest: uscRoleRest;
    private _uscResponsableRoleRest: uscRoleRest;
    private _uscContattiSelRest: uscContattiSelRest;
    private _lblDossierType: JQuery;
    private _lblDossierStatus: JQuery;
    private _enumHelper: EnumHelper;

    /**
    * Costruttore
    * @param webApiConfiguration
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    /**
    * Inizializzazione
    */
    initialize() {

        this._enumHelper = new EnumHelper();
        this._lblDossierSubject = $("#".concat(this.lblDossierSubjectId));
        this._lblStartDate = $("#".concat(this.lblStartDateId));
        this._lblRegistrationUser = $("#".concat(this.lblRegistrationUserId));
        this._lblDossierNote = $("#".concat(this.lblDossierNoteId));
        this._lblYear = $("#".concat(this.lblYearId));
        this._lblNumber = $("#".concat(this.lblNumberId));
        this._lblModifiedUser = $("#".concat(this.lblModifiedUserId));
        this._lblContainer = $("#".concat(this.lblContainerId));
        this._lblWorkflowProposerRole = $("#".concat(this.lblWorkflowProposerRoleId));
        this._lblWorkflowHandlerUser = $("#".concat(this.lblWorkflowHandlerUserId));
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._lblDossierType = $(`#${this.lblDossierTypeId}`);
        this._lblDossierStatus = $(`#${this.lblDossierStatusId}`);

        $(`#${this.columnDossierTypeKeyId}`).hide();
        $(`#${this.columnDossierTypeValueId}`).hide();

        this._loadingPanel.show(this.pageId);
        this._rowMetadataRepository = $("#".concat(this.rowMetadataId));
        this._rowMetadataRepository.hide();
        $(`#${this.uscCategoryRestId}`).hide();

        this._uscRoleRest = <uscRoleRest>$(`#${this.uscRoleRestId}`).data();
        this._uscResponsableRoleRest = <uscRoleRest>$(`#${this.uscResponsableRoleRestId}`).data();
        this._uscContattiSelRest = <uscContattiSelRest>$(`#${this.uscContattiSelRestId}`).data();

        let domainUserConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
        this._domainUserService = new DomainUserService(domainUserConfiguration);

        let workflowActivityConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowActivity');
        this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);

        this.bindLoaded();
    }



    /**
    *------------------------- Events -----------------------------
    */



    /**
    *------------------------- Methods -----------------------------
    */

    /**
    * Carica i dati dello user control
    */
    loadData(dossier: DossierSummaryViewModel): void {
        if (dossier == null) return;
        let roles: RoleModel[] = [];
        let responsableRole: RoleModel[] = [];

        for (let role of dossier.Roles) {
            let newRole: RoleModel = <RoleModel>{
                UniqueId: role.UniqueId,
                Name: role.Name,
                EntityShortId: role.EntityShortId
            }

            if (role.IsMaster == true) {
                responsableRole.push(newRole);
            } else {
                roles.push(newRole);
            }
        }

        this._uscResponsableRoleRest.renderRolesTree(responsableRole);


        if (roles.length == 0) {
            $(`#${this.uscRoleRestId}`).hide();
        } else {
            this._uscRoleRest.renderRolesTree(roles);
        }


        let contacts: ContactModel[] = [];
        for (let contact of dossier.Contacts) {
            let newContact: ContactModel = <ContactModel>{
                UniqueId: contact.UniqueId,
                EntityId: contact.EntityShortId,
                Description: contact.Name,
                IdContactType: contact.Type,
                IncrementalFather: contact.IncrementalFather
            };
            contacts.push(newContact);
        }
        this._uscContattiSelRest.renderContactsTree(contacts);

        if (dossier.Category.IdParent) {
            $(`#${this.uscCategoryRestId}`).show();
            let uscCategoryRest: uscCategoryRest = <uscCategoryRest>$(`#${this.uscCategoryRestId}`).data();
            uscCategoryRest.setToolbarVisibilityButtons();
            uscCategoryRest.populateCategotyTree(dossier.Category);
        }

        this._domainUserService.getUser(dossier.RegistrationUser,
            (user: DomainUserModel) => {
                this.setSummaryData(dossier);
                if (user) {
                    this._lblRegistrationUser.html(user.DisplayName.concat(" ").concat(dossier.RegistrationDate));
                }
            },
            (exception: ExceptionDTO) => {
                this.setSummaryData(dossier);
            }
        );

        if (dossier.LastChangedUser) {
            this._domainUserService.getUser(dossier.LastChangedUser,
                (user: DomainUserModel) => {
                    if (user) {
                        this._lblModifiedUser.html(user.DisplayName.concat(" ").concat(dossier.LastChangedDate));
                    }
                },
                (exception: ExceptionDTO) => {
                    console.log("Anomalia nel recupero del LastChangedUser del dossier.");
                }
            );
        }

        if (this.metadataRepositoryEnabled && dossier.MetadataDesigner) {
            this._rowMetadataRepository.show();
            let uscDynamicMetadataSummaryRest: UscDynamicMetadataSummaryRest = <UscDynamicMetadataSummaryRest>$("#".concat(this.uscDynamicMetadataSummaryRestId)).data();
            if (!jQuery.isEmptyObject(uscDynamicMetadataSummaryRest)) {
                uscDynamicMetadataSummaryRest.loadMetadatas(dossier.MetadataDesigner, dossier.MetadataValues);
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_CURRENT_METADATA_VALUES, dossier.MetadataDesigner);
            }
        }
        this.hideLoadingPanel();
    }

    /**
 * Imposta i dati nel sommario
 * @param dossier
 */
    setSummaryData(dossier: DossierSummaryViewModel) {
        this._lblDossierSubject.html(dossier.Subject);
        this._lblDossierNote.html(dossier.Note);
        this._lblYear.html(dossier.Year.toString());
        this._lblNumber.html(dossier.Number);
        this._lblContainer.html(dossier.ContainerName);
        this._lblStartDate.html(dossier.FormattedStartDate);
        if (this.dossierTypologyEnabled) {
            $(`#${this.columnDossierTypeKeyId}`).show();
            $(`#${this.columnDossierTypeValueId}`).show();
            this._lblDossierType.html(this._enumHelper.getDossierTypeDescription(dossier.DossierType));
        }
        this._lblDossierStatus.html(this._enumHelper.getDossierStatusDescription(dossier.Status));

        this._lblWorkflowHandlerUser.html("");
        this._lblWorkflowProposerRole.html("");
        $("#".concat(this.rowWorkflowProposerId)).hide();

        if (!String.isNullOrEmpty(this.workflowActivityId)) {
            this._workflowActivityService.getWorkflowActivity(this.workflowActivityId,
                (data: any) => {
                    if (data == null) return;
                    this._workflowActivity = data;
                    let subject: string;
                    let handler: string;
                    let role: WorkflowRoleModel;
                    if (this._workflowActivity.WorkflowAuthorizations) {

                        let authorization: WorkflowAuthorization = this._workflowActivity.WorkflowAuthorizations.filter(function (item) { if (item.IsHandler == true) return item; })[0];

                        if (authorization) {
                            handler = authorization.Account;
                            this._domainUserService.getUser(handler,
                                (user: DomainUserModel) => {
                                    if (user) {
                                        this._lblWorkflowHandlerUser.html(user.DisplayName);
                                    }
                                },
                                (exception: ExceptionDTO) => {
                                    this._uscNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                                    if (!jQuery.isEmptyObject(this._uscNotification)) {
                                        this._uscNotification.showNotification(exception);
                                    }
                                }
                            );
                        }
                    }

                    if (this._workflowActivity.WorkflowProperties != null) {

                        subject = this._workflowActivity.WorkflowProperties.filter(function (item) { if (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT) return item; })[0].ValueString;
                        this._lblDossierNote.html(subject);

                        let mapper: WorkflowRoleModelMapper = new WorkflowRoleModelMapper();
                        let propertyRole: WorkflowPropertyModel = this._workflowActivity.WorkflowProperties.filter(function (item) { if (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE) return item; })[0];
                        role = mapper.Map(JSON.parse(propertyRole.ValueString));
                        this._lblWorkflowProposerRole.html(role.Name);
                    }

                    $("#".concat(this.rowWorkflowProposerId)).show();
                },
                (exception: ExceptionDTO) => {
                    this._uscNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                    if (!jQuery.isEmptyObject(this._uscNotification)) {
                        this._uscNotification.showNotification(exception);
                    }
                }
            );
        }

        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(JSON.stringify(dossier.Roles));
        ajaxModel.Value.push(JSON.stringify(dossier.Contacts));
        ajaxModel.ActionName = "LoadExternalData";

        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }


    /**
    * Scateno l'evento di "Load Completed" del controllo
    */
    private bindLoaded(): void {
        $("#".concat(this.pageId)).data(this);
        $("#".concat(this.pageId)).triggerHandler(uscDossier.LOADED_EVENT);
    }

    /**
    * Metodo che nasconde il loading 
    */
    hideLoadingPanel = () => {
        this._loadingPanel.hide(this.pageId);
    }
}

export = uscDossier;