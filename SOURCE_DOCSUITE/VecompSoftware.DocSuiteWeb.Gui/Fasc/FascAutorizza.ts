/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel')
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import FascicleType = require('App/Models/Fascicles/FascicleType')
import FascicleRoleService = require('App/Services/Fascicles/FascicleRoleService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicleBase = require('Fasc/FascBase');
import UscFascicolo = require('UserControl/uscFascicolo');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import VisibilityType = require('App/Models/Fascicles/VisibilityType');
declare var Page_IsValid: any;

class FascAutorizza extends FascicleBase {
    currentFascicleId: string;
    fasciclePageContentId: string;
    ajaxManagerId: string;
    currentPageId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    uscNotificationId: string;
    uscFascicoloId: string;
    actionPage: string;
    radWindowManagerId: string;
    btnConfirmId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _fascicleRoleService: FascicleRoleService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _fascicleModel: FascicleModel;


    /**
     * Costruttore
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
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
    btnConfirm_OnClick = (sender: any, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        if (!Page_IsValid) {
            args.set_cancel(true);
            return;
        }
        this._btnConfirm.set_enabled(false);
        if (Page_IsValid) {

            PageClassHelper.callUserControlFunctionSafe<UscFascicolo>(this.uscFascicoloId)
                .done((instance) => {
                    instance.getRaciRoles().done((raciRoles: RoleModel[]) => {
                        this._loadingPanel.show(this.pageContentId);
                        $.when(this.insertFascicleRoles(instance.getAddedRolesIds(), raciRoles, instance.getRemovedRolesIds()),
                            this.removeFascicleRoles(instance.getRemovedRolesIds()),
                            this.setRaciRole(raciRoles, "", instance.getRemovedRolesIds()),
                            this.updateFascicleVisibilityType(instance.getSelectedVisibilityType()))
                            .done(() => {
                                this._loadingPanel.hide(this.pageContentId);
                                window.location.href = `../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${this._fascicleModel.UniqueId}`;
                            })
                            .fail((exception: ExceptionDTO) => {
                                this._loadingPanel.hide(this.pageContentId);
                                this.showNotificationException(this.uscNotificationId, exception, "E' avvenuto un errore durante la fase di salvataggio delle autorizzazioni.");
                            });
                    });
                });
            args.set_cancel(true);
            return;
        }
        this._btnConfirm.set_enabled(true);
        args.set_cancel(true);
    }

    /**
*------------------------- Methods-----------------------------
*/

    /**
     * Initialize
     */
    initialize() {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicking(this.btnConfirm_OnClick);

        let fascicleRoleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLEROLE_TYPE_NAME);
        this._fascicleRoleService = new FascicleRoleService(fascicleRoleConfiguration);

        this.setButtonEnable(false);
        this._loadingPanel.show(this.pageContentId);
        this.service.getFascicle(this.currentFascicleId,
            (data: any) => {
                if (data == null) {
                    $("#".concat(this.pageContentId)).hide();
                    this._loadingPanel.hide(this.pageContentId);
                    this.showNotificationMessage(this.uscNotificationId, "Fascicolo non trovato con i parametri passati");
                    return;
                }

                this._fascicleModel = data;

                PageClassHelper.callUserControlFunctionSafe<UscFascicolo>(this.uscFascicoloId)
                    .done((instance) => {
                        UscFascicolo.masterRole = this._fascicleModel.FascicleRoles.filter(x => x.IsMaster === true).map(x => x.Role)[0];
                        instance.setFascicleVisibilityTypeButtonCheck(this._fascicleModel.VisibilityType);
                    });
                this.checkFascicleRight(data.UniqueId)
                    .done((result) => {
                        if (!result) {
                            $("#".concat(this.pageContentId)).hide();
                            this.showNotificationMessage(this.uscNotificationId, `Fascicolo n. ${this._fascicleModel.Title}, <br />Impossibile visualizzare il fascicolo. Non si dispone dei diritti necessari.`);
                            return;
                        }
                        this.setButtonEnable(true);
                        this.loadFascicoloSummary();
                    })
                    .fail((exception: ExceptionDTO) => {
                        $("#".concat(this.pageContentId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    })
                    .always(() => this._loadingPanel.hide(this.pageContentId));
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                $("#".concat(this.pageContentId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private checkFascicleRight(idFascicle: string): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this.service.hasManageableRight(idFascicle,
            (data: any) => promise.resolve(!!data),
            (exception: ExceptionDTO) => promise.reject(exception));
        return promise.promise();
    }

    /**
     * Inizializza lo user control del sommario di fascicolo
     */
    loadFascicoloSummary(): void {
        PageClassHelper.callUserControlFunctionSafe<UscFascicolo>(this.uscFascicoloId)
            .done((instance) => {
                instance.loadDataWithoutFolders(this._fascicleModel);
            });
    }

    private insertFascicleRoles(roleAddedIds: number[], raciRole: RoleModel[], removedRoleIds: number[]): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        if (!roleAddedIds || !roleAddedIds.length) {
            return promise.resolve();
        }

        try {
            let role: RoleModel;
            let fascicleRole: FascicleRoleModel;
            let authorizationType: AuthorizationRoleType;
            if (this._fascicleModel.FascicleType == FascicleType.Procedure) {
                authorizationType = AuthorizationRoleType.Accounted;
            }

            let ajaxPromises: JQueryPromise<void>[] = [];
            if (removedRoleIds) {
                roleAddedIds = roleAddedIds.filter(roleAddedId => !removedRoleIds.some(removedRoleId => roleAddedId === removedRoleId));
            }
            for (let roleId of roleAddedIds) {
                role = <RoleModel>{};
                role.EntityShortId = roleId;
                fascicleRole = <FascicleRoleModel>{};
                fascicleRole.AuthorizationRoleType = authorizationType;
                fascicleRole.Role = role;
                fascicleRole.Fascicle = this._fascicleModel;
                ajaxPromises.push(this.insertFascicleRole(fascicleRole, raciRole));
            }

            $.when.apply(null, ajaxPromises)
                .done(() => promise.resolve())
                .fail((exception) => promise.reject(exception));
        } catch (error) {
            return promise.reject(error);
        }

        return promise.promise();
    }

    private insertFascicleRole(fascicleRole: FascicleRoleModel, raciRole: RoleModel[]): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        try {
            this._fascicleRoleService.insertFascicleRole(fascicleRole,
                (data: any) => {
                    if (raciRole && raciRole.some(x => x.EntityShortId === fascicleRole.Role.EntityShortId)) {
                        this.setRaciRole(raciRole, data.UniqueId, null).then(() => {
                            promise.resolve();
                        }, (exception: ExceptionDTO) => {
                            promise.reject(exception);
                        });
                    }
                    else {
                        promise.resolve();
                    }
                },
                (exception: ExceptionDTO) => {
                    promise.reject(exception);
                }
            );
        } catch (error) {
            return promise.reject(error);
        }
        return promise.promise();
    }

    private removeFascicleRoles(roleRemovedIds: number[]): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        if (!roleRemovedIds || !roleRemovedIds.length) {
            return promise.resolve();
        }

        try {
            let role: RoleModel;
            let fascicleRole: FascicleRoleModel;

            let ajaxPromises: JQueryPromise<void>[] = [];
            for (let roleId of roleRemovedIds) {
                role = <RoleModel>{};
                fascicleRole = this._fascicleModel.FascicleRoles.filter((x) => x.Role.EntityShortId == roleId)[0];
                if (fascicleRole) {
                    ajaxPromises.push(this.removeFascicleRole(fascicleRole));
                }
            }

            $.when.apply(null, ajaxPromises)
                .done(() => promise.resolve())
                .fail((exception) => promise.reject(exception));
        } catch (error) {
            return promise.reject(error);
        }

        return promise.promise();
    }

    private removeFascicleRole(fascicleRole: FascicleRoleModel): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        try {
            this._fascicleRoleService.deleteFascicleRole(fascicleRole,
                (data: any) => {
                    promise.resolve();
                },
                (exception: ExceptionDTO) => {
                    promise.reject(exception);
                }
            );
        } catch (error) {
            return promise.reject(error);
        }
        return promise.promise();
    }

    /**
 * Imposta l'attributo enable dei pulsanti
 * @param value
 */
    private setButtonEnable(value: boolean): void {
        this._btnConfirm.set_enabled(value);
    }

    private setRaciRole(raciRoles: RoleModel[], fascicleRoleId: string, removedRoleIds: number[]): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (!raciRoles || !raciRoles.length) {
            return promise.resolve();
        }

        if (removedRoleIds) {
            raciRoles = raciRoles.filter(raciRole => !removedRoleIds.some(removedRoleId => raciRole.EntityShortId === removedRoleId));
        }
        for (let role of raciRoles) {
            let fascicleRole: FascicleRoleModel = fascicleRoleId !== ""
                ? <FascicleRoleModel>{
                    UniqueId: fascicleRoleId,
                    IsMaster: false,
                    Role: role
                }
                : this._fascicleModel.FascicleRoles.filter((x) => x.Role.EntityShortId == role.EntityShortId)[0];

            if (!fascicleRole) {
                return promise.resolve();
            }

            fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
            this._fascicleRoleService.updateFascicleRole(fascicleRole,
                (data: any) => {
                    promise.resolve();
                }, (exception: ExceptionDTO) => {
                    promise.reject(exception);
                });
        }
        return promise.promise();
    }

    private updateFascicleVisibilityType(fascicleVisibilityType: VisibilityType): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (this._fascicleModel.FascicleType === FascicleType.Procedure) {
            this._fascicleModel.VisibilityType = fascicleVisibilityType;
            this.service.updateFascicle(this._fascicleModel, null,
                (data: any) => {
                    promise.resolve();
                },
                (exception: ExceptionDTO) => {
                    promise.reject(exception);
                });
        }
        return promise.promise();
    }
}
export = FascAutorizza;

