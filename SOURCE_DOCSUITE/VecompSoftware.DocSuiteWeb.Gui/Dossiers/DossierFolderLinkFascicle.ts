/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import DossierBase = require('Dossiers/DossierBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import ErrorHelper = require('App/Helpers/ErrorHelper');
import DossierFolderService = require('App/Services/Dossiers/DossierFolderService');
import RoleService = require('App/Services/Commons/RoleService');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import AjaxModel = require('App/Models/AjaxModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import DossierModel = require('App/Models/Dossiers/DossierModel');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import UscFascicleLink = require('UserControl/uscFascicleLink')
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import DossierFolderStatus = require("App/Models/Dossiers/DossierFolderStatus");
import DossierFolderSummaryModelMapper = require('App/Mappers/Dossiers/DossierFolderSummaryModelMapper');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import InsertActionType = require("App/Models/InsertActionType");
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import DossierRoleStatus = require('App/Models/Dossiers/DossierRoleStatus');

class DossierFolderLinkFascicle extends DossierBase {

    currentDossierId: string;
    lblNameId: string;
    btnConfermaId: string;
    btnConfermaUniqueId: string;
    ajaxManagerId: string;
    managerId: string;
    uscFascLinkId: string;
    ajaxLoadingPanelId: string;
    currentPageId: string;
    uscNotificationId: string;

    private _manager: Telerik.Web.UI.RadWindowManager;
    private _serviceConfigurations: ServiceConfiguration[];
    private _dossierFolderService: DossierFolderService;
    private _rgvLinkedFascicles: Telerik.Web.UI.RadGrid;
    private _lblName: JQuery;
    private _btnConferma: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rcbOtherFascicles: Telerik.Web.UI.RadComboBox;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _lblRegistrationUser: JQuery;
    private _lblFascicleObject: JQuery;
    private _domainUserService: DomainUserService;
    private _lblViewFascicle: JQuery;
    private _fascicleSummary: JQuery;
    private _currentFascicleId: string;
    private _currentFolder: DossierSummaryFolderViewModel;
    private _roleService: RoleService;
    private _dossierFolderRoles: DossierFolderRoleModel[];    

    /**
    * Costruttore
    * @param serviceConfiguration
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;

    }



    /**
    *------------------------- Events -----------------------------
    */


    /**
    * Inizializzazione
    */

    initialize() {
        super.initialize();
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._btnConferma = <Telerik.Web.UI.RadButton>$find(this.btnConfermaId);
        this._btnConferma.add_clicked(this.btmConferma_ButtonClicked);
        this._lblName = $("#".concat(this.lblNameId));
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);

        let dossierFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DossierFolder");
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);

        let roleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.ROLE_TYPE_NAME);
        this._roleService = new RoleService(roleConfiguration);

        $("#".concat(this.uscFascLinkId)).bind(UscFascicleLink.LOADED_EVENT, (args) => {
            this.loadFolder(this.currentDossierId);
        });

        this.loadFolder(this.currentDossierId);
    }


    /**
    * Evento scatenato al click del pulsante ConfermaInserimento
    * @method
    * @param sender
    * @param eventArgs
    * @returns
    */
    btmConferma_ButtonClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this._loadingPanel.show(this.currentPageId);
        this._btnConferma.set_enabled(false);

        let uscFascLink: UscFascicleLink = <UscFascicleLink>$("#".concat(this.uscFascLinkId)).data();
        if (!jQuery.isEmptyObject(uscFascLink)) {
            if (!uscFascLink.currentFascicleId) {
                this._loadingPanel.hide(this.currentPageId);
                this.showNotificationException(this.uscNotificationId, null, "Selezionare un fascicolo");
                this._btnConferma.set_enabled(true);
                return;
            }
        }

        this.updateDossierFolder();

    }




    /**
    *------------------------- Methods -----------------------------
    */

    /*
    * Carico la cartella
    */

    loadFolder(idDossierFolder: string) {

        this._currentFolder = this.getFolder(this.currentDossierId);
        $.when(this.getFolderRoles()).done(() => {
            this.setData(this._currentFolder);
        }).fail((exception) => {
            this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento dei settori autorizzati alla cartella.");
        });


    }

    /**
    * Aggiorno la cartella
    */

    updateDossierFolder() {

        let uscFascLink: UscFascicleLink = <UscFascicleLink>$("#".concat(this.uscFascLinkId)).data();
        let dossierFolder = <DossierFolderModel>{};

        let dossier = <DossierModel>{};
        dossier.UniqueId = this.currentDossierId;
        dossierFolder.Status = DossierFolderStatus.InProgress;

        dossierFolder.Dossier = dossier;

        dossierFolder.ParentInsertId = this._currentFolder.UniqueId;
        dossierFolder.Status = DossierFolderStatus.InProgress;
        
        let dossierFolderRoles: Array<DossierFolderRoleModel> = new Array<DossierFolderRoleModel>();
        let dossierFolderRole: DossierFolderRoleModel = <DossierFolderRoleModel>{};
        dossierFolder.DossierFolderRoles = this._dossierFolderRoles;

        if (!jQuery.isEmptyObject(uscFascLink)) {
            if (uscFascLink.currentFascicleId) {
                var fascicle = new FascicleModel;
                fascicle.UniqueId = uscFascLink.currentFascicleId;
                dossierFolder.Status = DossierFolderStatus.Fascicle;
                dossierFolder.Fascicle = fascicle;                
            }
            if (uscFascLink.selectedCategoryId) {
                var category = new CategoryModel;
                category.EntityShortId = uscFascLink.selectedCategoryId;
                dossierFolder.Category = category;
            }
        }

        this._dossierFolderService.insertDossierFolder(dossierFolder, InsertActionType.InsertDossierFolderAssociatedToFascicle,
            (data: any) => {
                if (data == null) return;
                let model = <AjaxModel>{};
                model.ActionName = "AddFascicleLink";
                model.Value = [];
                let mapper = new DossierFolderSummaryModelMapper();

                //todo: da togliere quando riusciremo a farci tornare le navigation properties con la put
                let folder: DossierSummaryFolderViewModel = mapper.Map(data);
                folder.idFascicle = dossierFolder.Fascicle.UniqueId;
                model.Value.push(JSON.stringify(folder));
                this._loadingPanel.hide(this.currentPageId);
                this.closeWindow(model);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.currentPageId);
                this.showNotificationException(this.uscNotificationId, exception);
                this._btnConferma.set_enabled(true);
            }
        );
    }

    /**
       * Recupero la cartella salvata nella session storage
       * @param idDossier
       */
    getFolder = (idDossier: string) => {
        let dossierFolder = <DossierSummaryFolderViewModel>{};
        let result: any = sessionStorage[idDossier];
        if (result == null) {
            return null;
        }
        let source = JSON.parse(result);
        if (source) {
            dossierFolder.UniqueId = source.UniqueId;
            dossierFolder.idParent = source.idParent;
            dossierFolder.Name = source.Name;
            dossierFolder.Status = source.Status;
            dossierFolder.idRole = source.idRole;
            dossierFolder.idCategory = source.idCategory;
        }
        return dossierFolder;
    }

    setData(dossierFolder: DossierSummaryFolderViewModel) {
        this._lblName.html(dossierFolder.Name);
        let uscFascLink: UscFascicleLink = <UscFascicleLink>$("#".concat(this.uscFascLinkId)).data();

        if (!jQuery.isEmptyObject(uscFascLink)) {
            if (dossierFolder.idCategory) {
                uscFascLink.onExternalCategoryChange(dossierFolder.idCategory);
            }
        }
    }

    private getFolderRoles = () => {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._roleService.getDossierFolderRole(this._currentFolder.UniqueId,
                (data: any) => {
                    if (data == null) {
                        promise.resolve();
                        return;
                    }
                    let dossierFolderRoles: DossierFolderRoleModel[] = new Array<DossierFolderRoleModel>();
                    for (let role of data) {
                        let dossierFolderRoleModel: DossierFolderRoleModel = <DossierFolderRoleModel>{};
                        let r: RoleModel = <RoleModel>{};
                        r.EntityShortId = role.EntityShortId;
                        r.IdRole = role.EntityShortId;
                        r.Name = role.Name;
                        r.TenantId = role.TenantId;
                        r.IdRoleTenant = role.IdRoleTenant;

                        dossierFolderRoleModel.Role = r;
                        dossierFolderRoleModel.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                        dossierFolderRoleModel.Status = DossierRoleStatus.Active;
                        dossierFolderRoles.push(dossierFolderRoleModel);
                    }
                    this._dossierFolderRoles = dossierFolderRoles;
                    promise.resolve();
                },
                (exception: ExceptionDTO) => {
                    promise.reject(exception);
                }
            );
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }
}
export = DossierFolderLinkFascicle;