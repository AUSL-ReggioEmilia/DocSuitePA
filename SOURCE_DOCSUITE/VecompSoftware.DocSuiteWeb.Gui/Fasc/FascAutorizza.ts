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
    btnConfirm_OnClick = (sender: any, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        if (!Page_IsValid) {
            args.set_cancel(true);
            return;
        }
        this._btnConfirm.set_enabled(false);
        this._loadingPanel.show(this.fasciclePageContentId);
        if (Page_IsValid) {
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest("Authorized");
            args.set_cancel(true);
            return;
        }
        this._loadingPanel.hide(this.fasciclePageContentId);
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
        let uscFascicolo: UscFascicolo = <UscFascicolo>$("#".concat(this.uscFascicoloId)).data();
        if (!jQuery.isEmptyObject(uscFascicolo)) {
            $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.DATA_LOADED_EVENT, (args) => {
            });
            uscFascicolo.loadData(this._fascicleModel);
        }
    }

    insertCallback(rolesAdded: string, rolesRemoved: string): void {
        if (this._fascicleModel.FascicleType != FascicleType.Procedure) {
            this.manageRoles(rolesAdded, rolesRemoved);
            return;
        }

        let uscFascicolo: UscFascicolo = <UscFascicolo>$("#".concat(this.uscFascicoloId)).data();
        if (!jQuery.isEmptyObject(uscFascicolo)) {
            this._fascicleModel.VisibilityType = uscFascicolo.getSelectedAccountedVisibilityType();
            this.service.updateFascicle(this._fascicleModel,null,
                (data: any) => {
                    this.manageRoles(rolesAdded, rolesRemoved);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.pageContentId);
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        }
    }

    private manageRoles(rolesAdded: string, rolesRemoved: string): void {
        if (!rolesAdded && !rolesRemoved) {
            window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(this._fascicleModel.UniqueId);
        }

        $.when(this.insertFascicleRoles(rolesAdded), this.removeFascicleRoles(rolesRemoved))
            .done(() => window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(this._fascicleModel.UniqueId))
            .fail((exception) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception, "E' avvenuto un errore durante la fase di salvataggio delle autorizzazioni.");
            });
    }

    private insertFascicleRoles(roles: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        if (!roles) {
            return promise.resolve();
        }

        try {
            let role: RoleModel;
            let fascicleRole: FascicleRoleModel;
            let roleAddedIds: number[] = JSON.parse(roles);
            let authorizationType: AuthorizationRoleType;
            if (this._fascicleModel.FascicleType == FascicleType.Procedure) {
                authorizationType = AuthorizationRoleType.Accounted;
            }

            let ajaxPromises: JQueryPromise<void>[] = [];
            for (let roleId of roleAddedIds) {
                role = <RoleModel>{};
                role.EntityShortId = roleId;
                fascicleRole = <FascicleRoleModel>{};
                fascicleRole.AuthorizationRoleType = authorizationType;
                fascicleRole.Role = role;
                fascicleRole.Fascicle = this._fascicleModel;
                ajaxPromises.push(this.insertFascicleRole(fascicleRole));
            }

            $.when.apply(null, ajaxPromises)
                .done(() => promise.resolve())
                .fail((exception) => promise.reject(exception));
        } catch (error) {
            return promise.reject(error);
        }        

        return promise.promise();
    }

    private insertFascicleRole(fascicleRole: FascicleRoleModel): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        try {
            this._fascicleRoleService.insertFascicleRole(fascicleRole,
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

    private removeFascicleRoles(roles: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        if (!roles) {
            return promise.resolve();
        }

        try {
            let role: RoleModel;
            let fascicleRole: FascicleRoleModel;
            let roleRemovedIds: number[] = JSON.parse(roles);

            let ajaxPromises: JQueryPromise<void>[] = [];
            for (let roleId of roleRemovedIds) {
                role = <RoleModel>{};
                fascicleRole = this._fascicleModel.FascicleRoles.filter((x) => x.Role.EntityShortId == roleId)[0];
                ajaxPromises.push(this.removeFascicleRole(fascicleRole));
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

}
export = FascAutorizza;

