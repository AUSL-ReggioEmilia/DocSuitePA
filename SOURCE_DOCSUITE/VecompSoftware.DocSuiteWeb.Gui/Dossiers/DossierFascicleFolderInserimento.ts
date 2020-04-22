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
import DossierFolderStatus = require("App/Models/Dossiers/DossierFolderStatus");
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import UscSettori = require('UserControl/uscSettori');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import DossierRoleStatus = require('App/Models/Dossiers/DossierRoleStatus');
import DossierFolderSummaryModelMapper = require('App/Mappers/Dossiers/DossierFolderSummaryModelMapper');
import UscFascicleInsert = require('UserControl/uscFascicleInsert');
import ContactModel = require('App/Models/Commons/ContactModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import FascicleService = require('App/Services/Fascicles/FascicleService');
import ValidationExceptionDTO = require('App/DTOs/ValidationExceptionDTO');
import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');
import RoleModel = require('App/Models/Commons/RoleModel');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import DossierFolderLocalService = require('App/Services/Dossiers/DossierFolderLocalService');
import IDossierFolderService = require('App/Services/Dossiers/IDossierFolderService');
import FascicleLocalService = require('App/Services/Fascicles/FascicleLocalService');
import IFascicleService = require('App/Services/Fascicles/IFascicleService');
import IRoleService = require('App/Services/Commons/IRoleService');
import RoleLocalService = require('App/Services/Commons/RoleLocalService');

declare var Page_IsValid: any;
class DossierFascicleFolderInserimento extends DossierBase {

    pageId: string;
    txtNameId: string;
    btnConfermaId: string;
    uniqueId: string;
    currentDossierId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    currentPageId: string;
    managerId: string;
    uscNotificationId: string;
    fascicleTypeRow: string;
    rdlFascicleType: string;
    uscFascInsertId: string;
    persistanceDisabled: boolean;

    private _manager: Telerik.Web.UI.RadWindowManager;
    private _serviceConfigurations: ServiceConfiguration[];
    private _dossierFolderService: IDossierFolderService;
    private _roleService: IRoleService;
    private _rgvLinkedFascicles: Telerik.Web.UI.RadGrid;
    private _btnConferma: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rcbOtherFascicles: Telerik.Web.UI.RadComboBox;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _domainUserService: DomainUserService;
    private _uscFascInsertId: string;
    private _fascicleService: IFascicleService;
    private _dossierParentFolderRoles: DossierFolderRoleModel[];

    /**
* Costruttore
* @param serviceConfiguration
*/
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    /**
*------------------------- Events -----------------------------
*/

    /**
   * Evento scatenato al click del pulsante ConfermaInserimento
   * @method
   * @param sender
   * @param eventArgs
   * @returns
   */
    btmConferma_ButtonClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        let selectedFascicleType: string;

        let isFascValid: boolean = false;
        let uscFascInsert: UscFascicleInsert = <UscFascicleInsert>$("#".concat(this._uscFascInsertId)).data();
        if (!jQuery.isEmptyObject(uscFascInsert)) {
            isFascValid = uscFascInsert.isPageValid();
            selectedFascicleType = uscFascInsert.getSelectedFascicleType();
            if (String.isNullOrEmpty(selectedFascicleType)) {
                this.showNotificationMessage(this.uscNotificationId, 'Selezionare una tipologia di fascicolo');
            }
        }
        if (!isFascValid || String.isNullOrEmpty(selectedFascicleType)) {
            return;
        }

        if (!Page_IsValid) {
            return;
        }

        this._loadingPanel.show(this.currentPageId);
        this._btnConferma.set_enabled(false);

        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.ActionName = "Insert";
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }


    /**
    * Inizializzazione
    */

    initialize() {
        super.initialize();
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._btnConferma = <Telerik.Web.UI.RadButton>$find(this.btnConfermaId);
        this._btnConferma.add_clicked(this.btmConferma_ButtonClicked);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);
        this._uscFascInsertId = this.uscFascInsertId;

        if (this.persistanceDisabled) {
            this._dossierFolderService = new DossierFolderLocalService();
        } else {
            let dossierFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME);
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
        }        

        let roleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.ROLE_TYPE_NAME);
        if (this.persistanceDisabled) {
            this._roleService = new RoleLocalService(roleConfiguration);
        } else {            
            this._roleService = new RoleService(roleConfiguration);
        }        

        if (this.persistanceDisabled) {
            this._fascicleService = new FascicleLocalService();
        } else {
            let fascicleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.FASCICLE_TYPE_NAME);
            this._fascicleService = new FascicleService(fascicleConfiguration);
        }        

        $("#".concat(this._uscFascInsertId)).bind(UscFascicleInsert.LOADED_EVENT, (args) => {
            $("#".concat(this.fascicleTypeRow)).show();
        });

        $.when(this.getFolderParentRoles()).done(() => {
        }).fail((exception) => {
            this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento dei settori autorizzati alla cartella madre.");
        });

        this._loadingPanel.hide(this.currentPageId);
    }


    /**
    *---------------------------- Methods ---------------------------
    */

    insertDossierFolder(responsibleContact: number, metadataModel: string) {

        let uscFascInsert: UscFascicleInsert = <UscFascicleInsert>$("#".concat(this._uscFascInsertId)).data();

        let dossierFolder = <DossierFolderModel>{};
        let dossier = <DossierModel>{};
        dossier.UniqueId = this.currentDossierId;
        dossierFolder.Status = DossierFolderStatus.InProgress;

        dossierFolder.Dossier = dossier;

        let dossierFolderToUpdate: DossierFolderModel = this.getFolderParent(this.currentDossierId);
        if (dossierFolderToUpdate) {
            dossierFolder.ParentInsertId = dossierFolderToUpdate.UniqueId;
        };

        if (!jQuery.isEmptyObject(uscFascInsert)) {
            let fascicle: FascicleModel = new FascicleModel;
            fascicle = uscFascInsert.getFascicle();

            if (!!metadataModel) {
                fascicle.MetadataValues = metadataModel;
                if (sessionStorage.getItem("MetadataRepository")) {
                    let metadataRepository: MetadataRepositoryModel = new MetadataRepositoryModel();
                    metadataRepository.UniqueId = sessionStorage.getItem("MetadataRepository");
                    fascicle.MetadataRepository = metadataRepository;
                }
            }

            if (fascicle.FascicleType != FascicleType.Activity) {
                let contactModel: ContactModel = <ContactModel>{};
                contactModel.EntityId = responsibleContact;
                fascicle.Contacts.push(contactModel);
            }

            //imprimo il settore della cartella madre
            dossierFolder.DossierFolderRoles = this._dossierParentFolderRoles;

            this._fascicleService.insertFascicle(fascicle,
                (data: FascicleModel) => {

                    let savedFascicle: FascicleModel = data;
                    dossierFolder.Status = DossierFolderStatus.Fascicle;
                    dossierFolder.Fascicle = savedFascicle;
                    let category = new CategoryModel();
                    category.EntityShortId = savedFascicle.Category.EntityShortId;
                    dossierFolder.Category = category;
                    this.callInsertDossierFolderService(dossierFolder, savedFascicle.Title);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.currentPageId);
                    this.showNotificationException(this.uscNotificationId, exception);
                    this._btnConferma.set_enabled(true);
                }
            );
        }

    }

    callInsertDossierFolderService = (dossierFolder: DossierFolderModel, fascicleTitle: string) => {
        this._dossierFolderService.insertDossierFolder(dossierFolder, null,
            (data: any) => {
                if (data == null) return;
                let model = <AjaxModel>{};
                model.ActionName = "ManageParent";
                model.Value = [];
                let mapper = new DossierFolderSummaryModelMapper();
                let modelDossierFolderSummary: DossierSummaryFolderViewModel = mapper.Map(data);
                if (dossierFolder.Fascicle != null && dossierFolder.Fascicle.UniqueId != null) {
                    modelDossierFolderSummary.idFascicle = dossierFolder.Fascicle.UniqueId;
                }
                model.Value.push(JSON.stringify(modelDossierFolderSummary));
                this._loadingPanel.hide(this.currentPageId);
                this.closeWindow(model);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.currentPageId);
                this.exceptionWindow(fascicleTitle, exception);
                this._btnConferma.set_enabled(true);
            }
        );
    }

    exceptionWindow = (fascicleTitle: string, exception: ExceptionDTO) => {
        let message: string;
        let ex: ExceptionDTO = exception;
        if (!String.isNullOrEmpty(fascicleTitle)) {
            message = "Attenzione: il fascicolo ".concat(fascicleTitle, " è stato creato correttamente ma sono occorsi degli errori in fase di creazione della cartella.<br /> <br />");

            if (exception && exception instanceof ValidationExceptionDTO && exception.validationMessages.length > 0) {
                message = message.concat("Gli errori sono i seguenti: <br />");
                exception.validationMessages.forEach(function (item: ValidationMessageDTO) {
                    message = message.concat(item.message, "<br />");
                })
            }

            ex = null;
        }
        this.showNotificationException(this.uscNotificationId, ex, message);
    }

    /**
    * Recupero la cartella salvata nella session storage
    * @param idDossier
    */
    getFolderParent = (idDossier: string) => {
        let dossierFolder = <DossierFolderModel>{};
        let dossierFolderRole = <DossierFolderRoleModel>{};
        dossierFolder.DossierFolderRoles = new Array<DossierFolderRoleModel>();
        let result: any = sessionStorage[idDossier];
        if (result == null) {
            return null;
        }
        let source = JSON.parse(result);
        if (source) {
            dossierFolder.UniqueId = source.UniqueId;
        }
        return dossierFolder;
    }

    private getFolderParentRoles = () => {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            let parentFolder: DossierFolderModel = this.getFolderParent(this.currentDossierId);
            this._roleService.getDossierFolderRole(parentFolder.UniqueId,
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
                    this._dossierParentFolderRoles = dossierFolderRoles;
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

export = DossierFascicleFolderInserimento;