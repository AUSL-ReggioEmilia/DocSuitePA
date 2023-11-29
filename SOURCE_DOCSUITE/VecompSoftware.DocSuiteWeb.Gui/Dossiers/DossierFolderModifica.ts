/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import DossierBase = require('Dossiers/DossierBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierFolderService = require('App/Services/Dossiers/DossierFolderService');
import RoleService = require('App/Services/Commons/RoleService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import DossierFolderSummaryModelMapper = require('App/Mappers/Dossiers/DossierFolderSummaryModelMapper');
import AjaxModel = require('App/Models/AjaxModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import UscSettori = require('UserControl/uscSettori');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import DossierRoleStatus = require('App/Models/Dossiers/DossierRoleStatus');
import RoleModel = require('App/Models/Commons/RoleModel');
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import UpdateActionType = require("App/Models/UpdateActionType");
import ValidationExceptionDTO = require('App/DTOs/ValidationExceptionDTO');
import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');
import DossierFolderLocalService = require('App/Services/Dossiers/DossierFolderLocalService');
import IDossierFolderService = require('App/Services/Dossiers/IDossierFolderService');
import IRoleService = require('App/Services/Commons/IRoleService');
import RoleLocalService = require('App/Services/Commons/RoleLocalService');

class DossierFolderModifica extends DossierBase {

    currentPageId: string;
    btnConfermaId: string;
    txtNameId: string;
    loadingPanelId: string;
    uscNotificationId: string;
    uscCategoryId: string;
    ajaxManagerId: string;
    uscRoleId: string;
    currentDossierId: string;
    selectedCategoryId: number;
    fascRowId: string;
    nameRowId: string;
    lblFascNameId: string;
    persistanceDisabled: boolean;

    private _serviceConfigurations: ServiceConfiguration[];
    private _btnConferma: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _txtName: Telerik.Web.UI.RadTextBox;
    private _dossierFolderService: IDossierFolderService;
    private _currentFolder: DossierSummaryFolderViewModel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _roleService: IRoleService;
    private _dossierFolderRoles: DossierFolderRoleModel[];    

    /**
     * Costruttore
     * @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    /**
    * ---------------------------- Events ---------------------------------
    */

    /**
    * Evento al click del bottone conferma
    * @param sender
    * @param eventArgs
    * @returns 
    */
    btnConferma_ButtonClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this._loadingPanel.show(this.currentPageId);
        this._btnConferma.set_enabled(false);
        this.modifyDossierFolder();

    }

    /**
     * Inizializzazione
     */
    initialize() {
        super.initialize();
        this._btnConferma = <Telerik.Web.UI.RadButton>$find(this.btnConfermaId);
        this._btnConferma.add_clicked(this.btnConferma_ButtonClicked);
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.loadingPanelId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);

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

        this._dossierFolderRoles = [];
        this._loadingPanel.show(this.currentPageId);
        this._currentFolder = this.getFolderFromSessionStorage(this.currentDossierId);
        this.setData(this._currentFolder);


        this.bindLoaded();
    }

    /*
    * ---------------------------- Methods ----------------------------
    */

    /**
    * Recupero i dati della cartella dalla SessionStorage
    * @param idDossier
    */
    getFolderFromSessionStorage = (idDossier: string) => {
        let dossierFolder = <DossierSummaryFolderViewModel>{};
        let result: any = sessionStorage[idDossier];
        if (result == null) {
            return null;
        }
        let source = JSON.parse(result);
        if (source) {
            dossierFolder.UniqueId = source.UniqueId;
            dossierFolder.Name = source.Name;
            dossierFolder.Status = source.Status;
            dossierFolder.idCategory = source.idCategory;
            dossierFolder.idRole = source.idRole;
            dossierFolder.idFascicle = source.idFascicle;
        }

        return dossierFolder;
    }


    modifyDossierFolder() {
        let dossierFolder: DossierFolderModel = <DossierFolderModel>{};
        dossierFolder.Name = this._txtName.get_textBoxValue();
        dossierFolder.UniqueId = this._currentFolder.UniqueId;

        if (this._currentFolder.idFascicle) {
            let fasc: FascicleModel = new FascicleModel();
            fasc.UniqueId = this._currentFolder.idFascicle;
            dossierFolder.Fascicle = fasc;
        }

        if (this.selectedCategoryId) {
            let category: CategoryModel = new CategoryModel;
            category.EntityShortId = this.selectedCategoryId;
            dossierFolder.Category = category;
        }

        dossierFolder.DossierFolderRoles = this.getUscRoles(this.uscRoleId);       

        this._dossierFolderService.updateDossierFolder(dossierFolder, null,
            (data: any) => {
                if (data == null) return;
                this.propagateAuthorizations(data, dossierFolder);               
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.currentPageId);
                this.showNotificationException(this.uscNotificationId, exception);
                this._btnConferma.set_enabled(true);
            }
        );

    }

    /**
     *  Imposto i dati della cartella nella pagina di modifica
     * @param dossierFolder
     */
    setData(dossierFolder: DossierSummaryFolderViewModel) {
        this._txtName.set_value(dossierFolder.Name);

        $.when(this.getFolderRoles()).done(() => {
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();

        let roles: RoleModel[] = new Array<RoleModel>();
        if (this._dossierFolderRoles != null) {
            for (let role of this._dossierFolderRoles) {
                let roleModel: RoleModel = <RoleModel>{};
                roleModel.EntityShortId = role.Role.EntityShortId;
                roleModel.IdRole = role.Role.EntityShortId;
                roleModel.IdTenantAOO = role.Role.IdTenantAOO;
                roleModel.Name = role.Role.Name;
                roles.push(roleModel);
            }
        }
        ajaxModel.Value.push(JSON.stringify(roles));
        ajaxModel.Value.push(JSON.stringify(dossierFolder.idCategory));
        ajaxModel.ActionName = "LoadExternalData";

        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        }).fail((exception) => {
            this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento dei settori autorizzati alla cartella.");
        });
    }


    /**
    * Evento al cambio di classificatore
    */
    onCategoryChanged = (idCategory: number) => {
        this.selectedCategoryId = idCategory;

        $("#".concat(this.currentPageId)).data(this);
    }

    /**
     * Metodo chiamato al completamento del caricamento degli usercontrol
     */
    loadExternalDataCallback(): void {
        if (this._currentFolder.idFascicle) {
            $('#'.concat(this.fascRowId)).show();
            $('#'.concat(this.lblFascNameId)).text(this._currentFolder.Name);
            $('#'.concat(this.nameRowId)).hide();
        } 
        this._loadingPanel.hide(this.currentPageId);
    }

    /**
* Recupero il settore salvato nella session storage dallo usc dei settori
* @param uscId
*/
    getUscRoles = (uscId: string) => {
        let dossierFolderRoles: Array<DossierFolderRoleModel> = new Array<DossierFolderRoleModel>();
        let uscRoles: UscSettori = <UscSettori>$("#".concat(uscId)).data();

        if (!jQuery.isEmptyObject(uscRoles)) {
            let source = JSON.parse(uscRoles.getRoles());
            if (source) {                
                for (let s of source) {
                    let dossierFolderRole: DossierFolderRoleModel = <DossierFolderRoleModel>{};
                    dossierFolderRole.Role = s;
                    dossierFolderRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                    dossierFolderRole.Status = DossierRoleStatus.Active;

                    //devo mettere lo uniqueid ai settori che erano già presenti nel dossier
                    let roleFolder = this._dossierFolderRoles.filter(function (item) {
                        if (item.Role.IdRole == s.EntityShortId && item.Role.IdTenantAOO == s.IdTenantAOO) {
                            return item;
                        }
                    });

                    if (roleFolder && roleFolder[0]){
                        dossierFolderRole.UniqueId = roleFolder[0].UniqueId;
                    }
                    
                    dossierFolderRoles.push(dossierFolderRole);
                }
            }
        }

        return dossierFolderRoles;
    }

    /**
    * salvo lo stato corrente della pagina
    */
    private bindLoaded(): void {
        $("#".concat(this.currentPageId)).data(this);

    }

    private getFolderRoles = () => {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            let currentFolder: DossierSummaryFolderViewModel = this.getFolderFromSessionStorage(this.currentDossierId);
            this._roleService.getDossierFolderRole(currentFolder.UniqueId,
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
                        r.IdTenantAOO = role.IdTenantAOO;

                        dossierFolderRoleModel.UniqueId = role.DossierFolderRoles[0].UniqueId;
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

    private propagateAuthorizations = (data: DossierFolderModel, dossierFolder: DossierFolderModel) => {
        let updateActionType: UpdateActionType = null;

        let uscRoles: UscSettori = <UscSettori>$("#".concat(this.uscRoleId)).data();
        if (!jQuery.isEmptyObject(uscRoles)) {
            let checkedChoice = uscRoles.getPropagateAuthorizationsChecked();
            if (checkedChoice === 1) {
                updateActionType = UpdateActionType.DossierFolderAuthorizationsPropagation;
            }          
        }

        if (!updateActionType) {
            this.closeDossierFolderModifica(data, dossierFolder);
            return;
        }

        this._dossierFolderService.updateDossierFolder(data, updateActionType,
            (data: any) => {
                this.closeDossierFolderModifica(data, dossierFolder);
            },
            (exception: ExceptionDTO) => {
                this.exceptionWindow(data.Name, exception);
                this._loadingPanel.hide(this.currentPageId);                
                this._btnConferma.set_enabled(true);
            }
        );
    }

    closeDossierFolderModifica = (data: DossierFolderModel, dossierFolder: DossierFolderModel) => {
        let model = <AjaxModel>{};
        model.ActionName = "ModifyFolder";
        model.Value = [];
        let mapper = new DossierFolderSummaryModelMapper();
        let resultModel: DossierSummaryFolderViewModel = mapper.Map(data);
        if (dossierFolder.Fascicle) {
            resultModel.idFascicle = dossierFolder.Fascicle.UniqueId;
        }
        model.Value.push(JSON.stringify(resultModel));
        this._loadingPanel.hide(this.currentPageId);
        this.closeWindow(model);
    }

    exceptionWindow = (dossierFolderName: string, exception: ExceptionDTO) => {
        let message: string;
        let ex: ExceptionDTO = exception;
        if (dossierFolderName && dossierFolderName!="") {
            message = "Attenzione: la cartella ".concat(dossierFolderName, " è stata modificata correttamente, ma sono occorsi degli errori in fase di propagazione delle autorizzazioni.<br /> <br />");

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
}

export = DossierFolderModifica;