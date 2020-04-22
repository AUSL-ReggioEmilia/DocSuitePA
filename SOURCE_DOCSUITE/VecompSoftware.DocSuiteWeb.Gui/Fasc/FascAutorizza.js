/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Models/Commons/AuthorizationRoleType", "App/Models/Fascicles/FascicleType", "App/Services/Fascicles/FascicleRoleService", "Fasc/FascBase", "UserControl/uscFascicolo", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, AuthorizationRoleType, FascicleType, FascicleRoleService, FascicleBase, UscFascicolo, ServiceConfigurationHelper) {
    var FascAutorizza = /** @class */ (function (_super) {
        __extends(FascAutorizza, _super);
        /**
         * Costruttore
         */
        function FascAutorizza(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
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
            _this.btnConfirm_OnClick = function (sender, args) {
                if (!Page_IsValid) {
                    args.set_cancel(true);
                    return;
                }
                _this._btnConfirm.set_enabled(false);
                _this._loadingPanel.show(_this.fasciclePageContentId);
                if (Page_IsValid) {
                    $find(_this.ajaxManagerId).ajaxRequest("Authorized");
                    args.set_cancel(true);
                    return;
                }
                _this._loadingPanel.hide(_this.fasciclePageContentId);
                _this._btnConfirm.set_enabled(true);
                args.set_cancel(true);
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
    *------------------------- Methods-----------------------------
    */
        /**
         * Initialize
         */
        FascAutorizza.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._manager = $find(this.radWindowManagerId);
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicking(this.btnConfirm_OnClick);
            var fascicleRoleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLEROLE_TYPE_NAME);
            this._fascicleRoleService = new FascicleRoleService(fascicleRoleConfiguration);
            this.setButtonEnable(false);
            this._loadingPanel.show(this.pageContentId);
            this.service.getFascicle(this.currentFascicleId, function (data) {
                if (data == null) {
                    $("#".concat(_this.pageContentId)).hide();
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationMessage(_this.uscNotificationId, "Fascicolo non trovato con i parametri passati");
                    return;
                }
                _this._fascicleModel = data;
                _this.checkFascicleRight(data.UniqueId)
                    .done(function (result) {
                    if (!result) {
                        $("#".concat(_this.pageContentId)).hide();
                        _this.showNotificationMessage(_this.uscNotificationId, "Fascicolo n. " + _this._fascicleModel.Title + ", <br />Impossibile visualizzare il fascicolo. Non si dispone dei diritti necessari.");
                        return;
                    }
                    _this.setButtonEnable(true);
                    _this.loadFascicoloSummary();
                })
                    .fail(function (exception) {
                    $("#".concat(_this.pageContentId)).hide();
                    _this.showNotificationException(_this.uscNotificationId, exception);
                })
                    .always(function () { return _this._loadingPanel.hide(_this.pageContentId); });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                $("#".concat(_this.pageContentId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascAutorizza.prototype.checkFascicleRight = function (idFascicle) {
            var promise = $.Deferred();
            this.service.hasManageableRight(idFascicle, function (data) { return promise.resolve(!!data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        /**
         * Inizializza lo user control del sommario di fascicolo
         */
        FascAutorizza.prototype.loadFascicoloSummary = function () {
            var uscFascicolo = $("#".concat(this.uscFascicoloId)).data();
            if (!jQuery.isEmptyObject(uscFascicolo)) {
                $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.DATA_LOADED_EVENT, function (args) {
                });
                uscFascicolo.loadData(this._fascicleModel);
            }
        };
        FascAutorizza.prototype.insertCallback = function (rolesAdded, rolesRemoved) {
            var _this = this;
            if (this._fascicleModel.FascicleType != FascicleType.Procedure) {
                this.manageRoles(rolesAdded, rolesRemoved);
                return;
            }
            var uscFascicolo = $("#".concat(this.uscFascicoloId)).data();
            if (!jQuery.isEmptyObject(uscFascicolo)) {
                this._fascicleModel.VisibilityType = uscFascicolo.getSelectedAccountedVisibilityType();
                this.service.updateFascicle(this._fascicleModel, null, function (data) {
                    _this.manageRoles(rolesAdded, rolesRemoved);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
        };
        FascAutorizza.prototype.manageRoles = function (rolesAdded, rolesRemoved) {
            var _this = this;
            if (!rolesAdded && !rolesRemoved) {
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(this._fascicleModel.UniqueId);
            }
            $.when(this.insertFascicleRoles(rolesAdded), this.removeFascicleRoles(rolesRemoved))
                .done(function () { return window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(_this._fascicleModel.UniqueId); })
                .fail(function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception, "E' avvenuto un errore durante la fase di salvataggio delle autorizzazioni.");
            });
        };
        FascAutorizza.prototype.insertFascicleRoles = function (roles) {
            var promise = $.Deferred();
            if (!roles) {
                return promise.resolve();
            }
            try {
                var role = void 0;
                var fascicleRole = void 0;
                var roleAddedIds = JSON.parse(roles);
                var authorizationType = void 0;
                if (this._fascicleModel.FascicleType == FascicleType.Procedure) {
                    authorizationType = AuthorizationRoleType.Accounted;
                }
                var ajaxPromises = [];
                for (var _i = 0, roleAddedIds_1 = roleAddedIds; _i < roleAddedIds_1.length; _i++) {
                    var roleId = roleAddedIds_1[_i];
                    role = {};
                    role.EntityShortId = roleId;
                    fascicleRole = {};
                    fascicleRole.AuthorizationRoleType = authorizationType;
                    fascicleRole.Role = role;
                    fascicleRole.Fascicle = this._fascicleModel;
                    ajaxPromises.push(this.insertFascicleRole(fascicleRole));
                }
                $.when.apply(null, ajaxPromises)
                    .done(function () { return promise.resolve(); })
                    .fail(function (exception) { return promise.reject(exception); });
            }
            catch (error) {
                return promise.reject(error);
            }
            return promise.promise();
        };
        FascAutorizza.prototype.insertFascicleRole = function (fascicleRole) {
            var promise = $.Deferred();
            try {
                this._fascicleRoleService.insertFascicleRole(fascicleRole, function (data) {
                    promise.resolve();
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                return promise.reject(error);
            }
            return promise.promise();
        };
        FascAutorizza.prototype.removeFascicleRoles = function (roles) {
            var promise = $.Deferred();
            if (!roles) {
                return promise.resolve();
            }
            try {
                var role = void 0;
                var fascicleRole = void 0;
                var roleRemovedIds = JSON.parse(roles);
                var ajaxPromises = [];
                var _loop_1 = function (roleId) {
                    role = {};
                    fascicleRole = this_1._fascicleModel.FascicleRoles.filter(function (x) { return x.Role.EntityShortId == roleId; })[0];
                    ajaxPromises.push(this_1.removeFascicleRole(fascicleRole));
                };
                var this_1 = this;
                for (var _i = 0, roleRemovedIds_1 = roleRemovedIds; _i < roleRemovedIds_1.length; _i++) {
                    var roleId = roleRemovedIds_1[_i];
                    _loop_1(roleId);
                }
                $.when.apply(null, ajaxPromises)
                    .done(function () { return promise.resolve(); })
                    .fail(function (exception) { return promise.reject(exception); });
            }
            catch (error) {
                return promise.reject(error);
            }
            return promise.promise();
        };
        FascAutorizza.prototype.removeFascicleRole = function (fascicleRole) {
            var promise = $.Deferred();
            try {
                this._fascicleRoleService.deleteFascicleRole(fascicleRole, function (data) {
                    promise.resolve();
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                return promise.reject(error);
            }
            return promise.promise();
        };
        /**
     * Imposta l'attributo enable dei pulsanti
     * @param value
     */
        FascAutorizza.prototype.setButtonEnable = function (value) {
            this._btnConfirm.set_enabled(value);
        };
        return FascAutorizza;
    }(FascicleBase));
    return FascAutorizza;
});
//# sourceMappingURL=FascAutorizza.js.map