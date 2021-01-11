import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UscRoleRestEventType = require('App/Models/Commons/UscRoleRestEventType');
import DossierRoleModel = require('App/Models/Dossiers/DossierRoleModel');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import uscRoleRest = require('UserControl/uscRoleRest');
import RoleModel = require('App/Models/Commons/RoleModel');
import DossierBase = require('./DossierBase');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import DossierService = require('App/Services/Dossiers/DossierService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import DossierModel = require('App/Models/Dossiers/DossierModel');
import DossierRoleService = require('App/Services/Dossiers/DossierRoleService');
import DossierRoleStatus = require('App/Models/Dossiers/DossierRoleStatus');
import BaseEntityRoleViewModel = require('App/ViewModels/BaseEntityRoleViewModel');

class DossierAutorizza extends DossierBase {
    uscNotificationId: string;
    dossierPageContentId: string;
    currentDossierId: string
    btnConfirmId: string;
    lblStartDateId: string;
    lblYearId: string;
    lblNumberId: string;
    uscRoleRestId: string;
    ajaxLoadingPanelId: string;
    dossierRolesList: DossierRoleModel[] = [];
    dossierRolesToDeleteList: DossierRoleModel[] = [];

    private _lblStartDate: JQuery;
    private _lblNumber: JQuery;
    private _lblYear: JQuery;
    private _uscRoleRest: uscRoleRest;
    private _serviceConfigurations: ServiceConfiguration[];
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _DossierRoles: Array<BaseEntityRoleViewModel>;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _dossierRoleService: DossierRoleService;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }


    initialize() {
        super.initialize();
        this._lblStartDate = $("#".concat(this.lblStartDateId));
        this._lblYear = $("#".concat(this.lblYearId));
        this._lblNumber = $("#".concat(this.lblNumberId));
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.dossierPageContentId);

        this._uscRoleRest = <uscRoleRest>$(`#${this.uscRoleRestId}`).data();
        this.registerUscRoleRestEventHandlers();

        let dossierRoleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DossierRole");
        this._dossierRoleService = new DossierRoleService(dossierRoleConfiguration);

        (<DossierService>this.service).isManageableDossier(this.currentDossierId,
            (data: any) => {
                if (data == null) return;
                if (data) {
                    $.when(this.loadData(), this.loadRoles()).done(() => {
                        this._btnConfirm.set_enabled(true);
                        this._btnConfirm.add_clicked(this.btnConfirm_clicked);
                        this._loadingPanel.hide(this.dossierPageContentId);
                    }).fail((exception) => {
                        this._btnConfirm.set_enabled(false);
                        this._loadingPanel.hide(this.dossierPageContentId);
                        this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento del Dossier.");
                    });
                }
                else {
                    this._btnConfirm.set_enabled(false);
                    this._loadingPanel.hide(this.dossierPageContentId);
                    $("#".concat(this.dossierPageContentId)).hide();
                    this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione pagina.<br \> Utente non autorizzato alla modifica del Dossier.");
                }

            },
            (exception: ExceptionDTO) => {
                this._btnConfirm.set_enabled(false);
                this._loadingPanel.hide(this.dossierPageContentId);
                $("#".concat(this.dossierPageContentId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private registerUscRoleRestEventHandlers(): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRestId)
            .done((instance) => {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, (roleId: number) => {
                    let dossierRoleId: string = this._DossierRoles.filter(x => x.EntityShortId == roleId).map(y => y.UniqueId)[0];
                    this.dossierRolesToDeleteList.push(<DossierRoleModel>{ UniqueId: dossierRoleId, Role: <RoleModel>{ IdRole: roleId } })
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, (newAddedRoles: RoleModel[]) => {
                    let existedRole: RoleModel;
                    for (let dossierRole of this._DossierRoles) {
                        existedRole = newAddedRoles.filter(x => x.EntityShortId === dossierRole.EntityShortId)[0];
                        if (existedRole) {
                            alert(`Non è possibile selezionare il settore ${existedRole.Name} in quanto già presente come settore autorizzato del fascicolo`);
                            return;
                        }
                    }

                    let dossierRoles: DossierRoleModel[] = newAddedRoles.map(x => <DossierRoleModel>{
                        Role: x,
                        IsMaster: false,
                        AuthorizationRoleType: AuthorizationRoleType.Accounted
                    });

                    for (let dossierRole of dossierRoles) {
                        this.dossierRolesList.push(dossierRole);
                    }

                    return $.Deferred<RoleModel>().resolve(existedRole);
                });
            });
    }

    loadData(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this.service.getDossier(this.currentDossierId,
                (data: any) => {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        this._lblYear.html(data.Year.toString());
                        this._lblNumber.html(data.Number);
                        this._lblStartDate.html(data.FormattedStartDate);

                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                },
                (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    loadRoles(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this.service.getDossierRoles(this.currentDossierId,
                (data: any) => {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        //ritorna solo quello attivo
                        this._DossierRoles = data;
                        let roles: RoleModel[] = [];

                        for (let role of this._DossierRoles) {
                            if (role.IsMaster == true) { continue; }
                            let newRole: RoleModel = <RoleModel>{
                                UniqueId: role.UniqueId,
                                Name: role.Name,
                                IdRole: role.EntityShortId,
                                IsActive: 1
                            }
                            roles.push(newRole);
                        }

                        this._uscRoleRest.renderRolesTree(roles);
                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                },
                (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    btnConfirm_clicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._loadingPanel.show(this.dossierPageContentId);

        let roleToExclude:DossierRoleModel[] = this.findCommonRoles(this.dossierRolesList, this.dossierRolesToDeleteList);

        for (let roleIdToDelete of roleToExclude) {
            this.dossierRolesList = this.dossierRolesList.filter(x => x.Role.IdRole !== roleIdToDelete.Role.IdRole);
            this.dossierRolesToDeleteList = this.dossierRolesToDeleteList.filter(x => x.Role.IdRole !== roleIdToDelete.Role.IdRole);
        }

        $.when(this.insertDossierRoles(), this.removeDossierRoles())
            .done(() => {
                this._loadingPanel.show(this.dossierPageContentId);
                window.location.href = "DossierVisualizza.aspx?Type=Dossier&IdDossier=".concat(this.currentDossierId);
            })
            .fail((exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.dossierPageContentId);
                this.showNotificationException(this.uscNotificationId, exception, "E' avvenuto un errore durante la fase di salvataggio delle autorizzazioni.");
            });
    }

    private removeDossierRoles(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();

        try {
            let ajaxPromises: JQueryPromise<void>[] = [];

            let dossierModel: DossierModel = <DossierModel>{};
            dossierModel.DossierRoles = this.dossierRolesToDeleteList;

            for (let dossierRole of dossierModel.DossierRoles) {
                dossierRole.Dossier = <DossierModel>{};
                dossierRole.Dossier.UniqueId = this.currentDossierId;

                ajaxPromises.push(this.removeDossierRole(dossierRole));
            }

            $.when.apply(null, ajaxPromises)
                .done(() => promise.resolve())
                .fail((exception) => promise.reject(exception));
        } catch (error) {
            return promise.reject(error);
        }

        return promise.promise();
    }

    private removeDossierRole(dossierRole: DossierRoleModel): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        try {
            this._dossierRoleService.deleteDossierRole(dossierRole,
                (data: any) => {
                    promise.resolve();
                }, (exception: ExceptionDTO) => {
                    promise.reject(exception);
                });
        } catch (error) {
            return promise.reject(error);
        }
        return promise.promise();
    }

    private insertDossierRoles(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        try {
            let ajaxPromises: JQueryPromise<void>[] = [];
            let dossierModel: DossierModel = <DossierModel>{};
            dossierModel.DossierRoles = this.dossierRolesList;

            for (let dossierRole of dossierModel.DossierRoles) {
                dossierRole.Dossier = <DossierModel>{};
                dossierRole.Dossier.UniqueId = this.currentDossierId;
                dossierRole.Status = DossierRoleStatus[DossierRoleStatus.Active];

                ajaxPromises.push(this.insertDossierRole(dossierRole));
            }

            $.when.apply(null, ajaxPromises)
                .done(() => promise.resolve())
                .fail((exception) => promise.reject(exception));
        } catch (error) {
            return promise.reject(error);
        }

        return promise.promise();
    }

    private insertDossierRole(dossierRole: DossierRoleModel): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        try {
            this._dossierRoleService.insertDossierRole(dossierRole,
                (data: any) => {
                    promise.resolve();
                }, (exception: ExceptionDTO) => {
                    promise.reject(exception);
                });
        } catch (error) {
            return promise.reject(error);
        }
        return promise.promise();
    }

    private findCommonRoles(insertedRoles: DossierRoleModel[], deletedRoles: DossierRoleModel[]): DossierRoleModel[] {
        let sameDossierRolesModel: DossierRoleModel[] = [];
        for (let insertedRole of insertedRoles) {
            for (let deletedRole of deletedRoles) {
                if (insertedRole.Role.IdRole == deletedRole.Role.IdRole) {
                    let dossierRoleModel: DossierRoleModel = <DossierRoleModel>{
                        Role: <RoleModel>{
                            Name: insertedRole.Role.Name,
                            IdRole: insertedRole.Role.IdRole
                        }
                    }

                    sameDossierRolesModel.push(dossierRoleModel);
                }
            }
        }
        return sameDossierRolesModel;
    }
}

export = DossierAutorizza;