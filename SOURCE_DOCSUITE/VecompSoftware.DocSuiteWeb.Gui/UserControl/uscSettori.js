/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
define(["require", "exports", "App/Models/Fascicles/VisibilityType"], function (require, exports, VisibilityType) {
    var uscSettori = /** @class */ (function () {
        /**
        * Costruttore
             * @param serviceConfiguration
             * @param workflowStartConfiguration
        */
        function uscSettori(serviceConfigurations) {
            var _this = this;
            this._btnExpandRoles = $find(this.btnExpandRolesId);
            /**
            *------------------------- Events -----------------------------
            */
            /**
        * Evento al click del pulsante per espandere o collassare il pannello dei metadati dinamici
        */
            this.btnExpandRoles_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (_this._isContentExpanded) {
                    _this._rowContent.hide();
                    _this._isContentExpanded = false;
                    _this._btnExpandRoles.removeCssClass("dsw-arrow-down");
                    _this._btnExpandRoles.addCssClass("dsw-arrow-up");
                }
                else {
                    _this._rowContent.show();
                    _this._isContentExpanded = true;
                    _this._btnExpandRoles.removeCssClass("dsw-arrow-up");
                    _this._btnExpandRoles.addCssClass("dsw-arrow-down");
                }
            };
            this.setRoles = function (isAjaxModelResult, ajaxResult) {
                var inputString;
                var inputs;
                var tenantId = _this.currentTenantId;
                if (isAjaxModelResult === true) {
                    inputString = ajaxResult.Value;
                }
                else {
                    inputString = ajaxResult;
                }
                inputs = inputString.toString().split("|");
                //parte che guarda tenantId e UserName        
                var tenants = inputs.filter(function (i) { return i.search("tenantId= ") != -1; });
                if (tenants.length == 1) {
                    tenantId = tenants[0].trim().split("tenantId= ")[0];
                }
                var rolesToAdd = inputs[1].split(",");
                //chiamo il roleservice?
                _this._selectedRoles = new Array();
                if (_this.multiSelect.toLowerCase() === "true") {
                    var sessionValue = _this.getRoles();
                    if (sessionValue != null) {
                        var source = JSON.parse(sessionValue);
                        _this._selectedRoles = _this.parseRolesFromJson(source);
                    }
                }
                for (var _i = 0, rolesToAdd_1 = rolesToAdd; _i < rolesToAdd_1.length; _i++) {
                    var r = rolesToAdd_1[_i];
                    var roleAdded = {};
                    roleAdded.EntityShortId = parseInt(r);
                    roleAdded.TenantId = tenantId;
                    _this._selectedRoles.push(roleAdded);
                }
                sessionStorage[_this._sessionStorageKey] = JSON.stringify(_this._selectedRoles);
            };
            this.getRoles = function () {
                var result = sessionStorage[_this._sessionStorageKey];
                if (result == null) {
                    return null;
                }
                return result;
            };
            this.parseRolesFromJson = function (source) {
                var result = new Array();
                for (var _i = 0, source_1 = source; _i < source_1.length; _i++) {
                    var s = source_1[_i];
                    var role = {};
                    role.EntityShortId = s.EntityShortId;
                    role.TenantId = s.TenantId;
                    result.push(role);
                }
                return result;
            };
            this.deleteCallback = function (roleId, roleTenantId) {
                var source = _this.getRoles();
                if (source != null) {
                    _this._selectedRoles = _this.parseRolesFromJson(JSON.parse(source));
                    var updatedRoles = _this._selectedRoles.filter(function (d) { return !(d.EntityShortId === parseInt(roleId) && d.TenantId == roleTenantId); });
                    sessionStorage[_this._sessionStorageKey] = JSON.stringify(updatedRoles);
                }
            };
            this.clearSessionStorage = function () {
                if (sessionStorage[_this._sessionStorageKey] != null) {
                    sessionStorage.removeItem(_this._sessionStorageKey);
                }
            };
            this.getFascicleVisibilityType = function () {
                _this._btnFascicleVisibilityType = _this._toolBar.findItemByValue("checkFascicleVisibilityType");
                if (_this._btnFascicleVisibilityType) {
                    var checked = _this._btnFascicleVisibilityType.get_checked();
                    if (checked) {
                        return VisibilityType.Accessible;
                    }
                    return VisibilityType.Confidential;
                }
            };
            this.enableValidators = function (state) {
                ValidatorEnable($get(_this.validatorAnyNodeId), state);
            };
            this.getPropagateAuthorizationsChecked = function () {
                _this._btnPropagateAuthorizations = _this._toolBar.findItemByValue("checkPropagateAuthorizations");
                var checked = _this._btnPropagateAuthorizations.get_checked();
                if (checked) {
                    return 1;
                }
                return 0;
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
        * ------------------------- Methods -----------------------------
        */
        /**
        * Initialize
        */
        uscSettori.prototype.initialize = function () {
            this._selectedRoles = new Array();
            this._toolBar = $find(this.toolBarId);
            this._rowContent = $("#".concat(this.contentRowId));
            this._btnExpandRoles = $find(this.btnExpandRolesId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._rowContent.show();
            this._isContentExpanded = true;
            if (this._btnExpandRoles) {
                this._btnExpandRoles.addCssClass("dsw-arrow-down");
                this._btnExpandRoles.add_clicking(this.btnExpandRoles_OnClick);
            }
            this._sessionStorageKey = this.contentId.concat(uscSettori.SESSION_NAME_SELECTED_ROLES);
            this.bindLoaded();
        };
        uscSettori.prototype.getRolesByTenantId = function (tenantId) {
            var ajaxModel = {};
            ajaxModel.Value = new Array();
            ajaxModel.Value.push(tenantId);
            ajaxModel.ActionName = "GetByTenantId";
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        };
        /**
        * Scateno l'evento di "Load Completed" del controllo
        */
        uscSettori.prototype.bindLoaded = function () {
            $("#".concat(this.contentId)).data(this);
            $("#".concat(this.contentId)).triggerHandler(uscSettori.LOADED_EVENT);
        };
        uscSettori.SESSION_NAME_SELECTED_ROLES = "SelectedRoles";
        uscSettori.LOADED_EVENT = "onLoaded";
        return uscSettori;
    }());
    return uscSettori;
});
//# sourceMappingURL=uscSettori.js.map