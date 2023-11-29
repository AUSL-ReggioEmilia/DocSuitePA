/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import RoleModel = require('App/Models/Commons/RoleModel');
import VisibilityType = require('App/Models/Fascicles/VisibilityType');
import AjaxModel = require('App/Models/AjaxModel');

declare var ValidatorEnable: any;
class uscSettori {
    contentId: string;
    uscNotificationId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    dswEnvironment: string;
    multiSelect: string;
    toolBarId: string;
    validatorAnyNodeId: string;
    btnPropagateAuthorizationsId: string;
    btnExpandRolesId: string;
    contentRowId: string;


    private static SESSION_NAME_SELECTED_ROLES: string = "SelectedRoles";
    public static LOADED_EVENT: string = "onLoaded";

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfiguration: ServiceConfiguration;
    private _selectedRoles: Array<RoleModel>;
    private _sessionStorageKey: string;
    private _toolBar: Telerik.Web.UI.RadToolBar;
    private _btnFascicleVisibilityType: Telerik.Web.UI.RadToolBarButton;
    private _btnPropagateAuthorizations: Telerik.Web.UI.RadToolBarButton;
    private _btnExpandRoles: Telerik.Web.UI.RadButton;
    private _isContentExpanded: boolean;
    private _rowContent: JQuery;

    /**
    * Costruttore
         * @param serviceConfiguration
         * @param workflowStartConfiguration
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    /**
    *------------------------- Events -----------------------------
    */

    /**
* Evento al click del pulsante per espandere o collassare il pannello dei metadati dinamici
*/
    btnExpandRoles_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (this._isContentExpanded) {
            this._rowContent.hide();
            this._isContentExpanded = false;
            this._btnExpandRoles.removeCssClass("dsw-arrow-down");
            this._btnExpandRoles.addCssClass("dsw-arrow-up");
        }
        else {
            this._rowContent.show();
            this._isContentExpanded = true;
            this._btnExpandRoles.removeCssClass("dsw-arrow-up");
            this._btnExpandRoles.addCssClass("dsw-arrow-down");
        }

    }

    /**
    * ------------------------- Methods -----------------------------
    */

    /**
    * Initialize
    */
    initialize() {
        this._selectedRoles = new Array<RoleModel>();
        this._toolBar = <Telerik.Web.UI.RadToolBar>$find(this.toolBarId);
        this._rowContent = $("#".concat(this.contentRowId));
        this._btnExpandRoles = <Telerik.Web.UI.RadButton>$find(this.btnExpandRolesId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._rowContent.show();
        this._isContentExpanded = true;
        if (this._btnExpandRoles) {
            this._btnExpandRoles.addCssClass("dsw-arrow-down");
            this._btnExpandRoles.add_clicking(this.btnExpandRoles_OnClick);
        }

        this._sessionStorageKey = this.contentId.concat(uscSettori.SESSION_NAME_SELECTED_ROLES);
        this.bindLoaded();
    }

    setRoles = (isAjaxModelResult: boolean, ajaxResult: any) => {
        let inputString: any;
        let inputs: string[];
        if (isAjaxModelResult === true) {
            inputString = ajaxResult.Value;
        }
        else {
            inputString = ajaxResult;
        }
        inputs = inputString.toString().split("|");
        let rolesToAdd: string[] = inputs[1].split(",");
        this._selectedRoles = new Array<RoleModel>();
        if (this.multiSelect.toLowerCase() === "true") {
            let sessionValue = this.getRoles();
            if (sessionValue != null) {
                let source: any = JSON.parse(sessionValue);
                this._selectedRoles = this.parseRolesFromJson(source);
            }
        }

        for (let r of rolesToAdd) {
            let roleAdded: RoleModel = <RoleModel>{};
            roleAdded.EntityShortId = parseInt(r);
            this._selectedRoles.push(roleAdded);
        }

        sessionStorage[this._sessionStorageKey] = JSON.stringify(this._selectedRoles);
    }

    getRoles = () => {
        let result: any = sessionStorage[this._sessionStorageKey];
        if (result == null) {
            return null;
        }
        return result;
    }


    parseRolesFromJson = (source: any) => {
        let result: Array<RoleModel> = new Array<RoleModel>();
        for (let s of source) {
            let role: RoleModel = <RoleModel>{};
            role.EntityShortId = s.EntityShortId;
            role.IdTenantAOO = s.IdTenantAOO;
            result.push(role);
        }
        return result
    }

    deleteCallback = (roleId: string) => {
        let source: any = this.getRoles();
        if (source != null) {
            this._selectedRoles = this.parseRolesFromJson(JSON.parse(source));

            let updatedRoles: RoleModel[] = this._selectedRoles.filter(d => !(d.EntityShortId === parseInt(roleId)));
            sessionStorage[this._sessionStorageKey] = JSON.stringify(updatedRoles);
        }
    }

    clearSessionStorage = () => {
        if (sessionStorage[this._sessionStorageKey] != null) {
            sessionStorage.removeItem(this._sessionStorageKey);
        }
    }

    getFascicleVisibilityType = () => {
        this._btnFascicleVisibilityType = <Telerik.Web.UI.RadToolBarButton>this._toolBar.findItemByValue("checkFascicleVisibilityType");
        if (this._btnFascicleVisibilityType) {
            let checked: boolean = <any>this._btnFascicleVisibilityType.get_checked();
            if (checked) {
                return VisibilityType.Accessible;
            }
            return VisibilityType.Confidential;
        }
    }

    /**
    * Scateno l'evento di "Load Completed" del controllo
    */
    private bindLoaded(): void {
        $("#".concat(this.contentId)).data(this);
        $("#".concat(this.contentId)).triggerHandler(uscSettori.LOADED_EVENT);

    }

    enableValidators = (state: boolean) => {
        ValidatorEnable($get(this.validatorAnyNodeId), state);
    }


    getPropagateAuthorizationsChecked = () => {
        this._btnPropagateAuthorizations = <Telerik.Web.UI.RadToolBarButton>this._toolBar.findItemByValue("checkPropagateAuthorizations");
        let checked: boolean = <any>this._btnPropagateAuthorizations.get_checked();
        if (checked) {
            return 1;
        }
        return 0;
    }
}
export = uscSettori;