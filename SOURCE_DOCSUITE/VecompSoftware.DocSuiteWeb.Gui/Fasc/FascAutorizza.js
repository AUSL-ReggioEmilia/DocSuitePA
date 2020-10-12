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
define(["require", "exports", "App/Models/Commons/AuthorizationRoleType", "App/Models/Fascicles/FascicleType", "App/Services/Fascicles/FascicleRoleService", "Fasc/FascBase", "UserControl/uscFascicolo", "App/Helpers/ServiceConfigurationHelper", "App/Helpers/PageClassHelper"], function (require, exports, AuthorizationRoleType, FascicleType, FascicleRoleService, FascicleBase, UscFascicolo, ServiceConfigurationHelper, PageClassHelper) {
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
                if (Page_IsValid) {
                    PageClassHelper.callUserControlFunctionSafe(_this.uscFascicoloId)
                        .done(function (instance) {
                        instance.getRaciRoles().done(function (raciRoles) {
                            _this._loadingPanel.show(_this.pageContentId);
                            $.when(_this.insertFascicleRoles(instance.getAddedRolesIds(), raciRoles, instance.getRemovedRolesIds()), _this.removeFascicleRoles(instance.getRemovedRolesIds()), _this.setRaciRole(raciRoles, "", instance.getRemovedRolesIds()), _this.updateFascicleVisibilityType(instance.getSelectedVisibilityType()))
                                .done(function () {
                                _this._loadingPanel.hide(_this.pageContentId);
                                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=" + _this._fascicleModel.UniqueId;
                            })
                                .fail(function (exception) {
                                _this._loadingPanel.hide(_this.pageContentId);
                                _this.showNotificationException(_this.uscNotificationId, exception, "E' avvenuto un errore durante la fase di salvataggio delle autorizzazioni.");
                            });
                        });
                    });
                    args.set_cancel(true);
                    return;
                }
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
                PageClassHelper.callUserControlFunctionSafe(_this.uscFascicoloId)
                    .done(function (instance) {
                    UscFascicolo.masterRole = _this._fascicleModel.FascicleRoles.filter(function (x) { return x.IsMaster === true; }).map(function (x) { return x.Role; })[0];
                    instance.setFascicleVisibilityTypeButtonCheck(_this._fascicleModel.VisibilityType);
                });
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
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscFascicoloId)
                .done(function (instance) {
                instance.loadDataWithoutFolders(_this._fascicleModel);
            });
        };
        FascAutorizza.prototype.insertFascicleRoles = function (roleAddedIds, raciRole, removedRoleIds) {
            var promise = $.Deferred();
            if (!roleAddedIds || !roleAddedIds.length) {
                return promise.resolve();
            }
            try {
                var role = void 0;
                var fascicleRole = void 0;
                var authorizationType = void 0;
                if (this._fascicleModel.FascicleType == FascicleType.Procedure) {
                    authorizationType = AuthorizationRoleType.Accounted;
                }
                var ajaxPromises = [];
                if (removedRoleIds) {
                    roleAddedIds = roleAddedIds.filter(function (roleAddedId) { return !removedRoleIds.some(function (removedRoleId) { return roleAddedId === removedRoleId; }); });
                }
                for (var _i = 0, roleAddedIds_1 = roleAddedIds; _i < roleAddedIds_1.length; _i++) {
                    var roleId = roleAddedIds_1[_i];
                    role = {};
                    role.EntityShortId = roleId;
                    fascicleRole = {};
                    fascicleRole.AuthorizationRoleType = authorizationType;
                    fascicleRole.Role = role;
                    fascicleRole.Fascicle = this._fascicleModel;
                    ajaxPromises.push(this.insertFascicleRole(fascicleRole, raciRole));
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
        FascAutorizza.prototype.insertFascicleRole = function (fascicleRole, raciRole) {
            var _this = this;
            var promise = $.Deferred();
            try {
                this._fascicleRoleService.insertFascicleRole(fascicleRole, function (data) {
                    if (raciRole && raciRole.some(function (x) { return x.EntityShortId === fascicleRole.Role.EntityShortId; })) {
                        _this.setRaciRole(raciRole, data.UniqueId, null).then(function () {
                            promise.resolve();
                        }, function (exception) {
                            promise.reject(exception);
                        });
                    }
                    else {
                        promise.resolve();
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                return promise.reject(error);
            }
            return promise.promise();
        };
        FascAutorizza.prototype.removeFascicleRoles = function (roleRemovedIds) {
            var promise = $.Deferred();
            if (!roleRemovedIds || !roleRemovedIds.length) {
                return promise.resolve();
            }
            try {
                var role = void 0;
                var fascicleRole = void 0;
                var ajaxPromises = [];
                var _loop_1 = function (roleId) {
                    role = {};
                    fascicleRole = this_1._fascicleModel.FascicleRoles.filter(function (x) { return x.Role.EntityShortId == roleId; })[0];
                    if (fascicleRole) {
                        ajaxPromises.push(this_1.removeFascicleRole(fascicleRole));
                    }
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
        FascAutorizza.prototype.setRaciRole = function (raciRoles, fascicleRoleId, removedRoleIds) {
            var promise = $.Deferred();
            if (!raciRoles || !raciRoles.length) {
                return promise.resolve();
            }
            if (removedRoleIds) {
                raciRoles = raciRoles.filter(function (raciRole) { return !removedRoleIds.some(function (removedRoleId) { return raciRole.EntityShortId === removedRoleId; }); });
            }
            var _loop_2 = function (role) {
                var fascicleRole = fascicleRoleId !== ""
                    ? {
                        UniqueId: fascicleRoleId,
                        IsMaster: false,
                        Role: role
                    }
                    : this_2._fascicleModel.FascicleRoles.filter(function (x) { return x.Role.EntityShortId == role.EntityShortId; })[0];
                if (!fascicleRole) {
                    return { value: promise.resolve() };
                }
                fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
                this_2._fascicleRoleService.updateFascicleRole(fascicleRole, function (data) {
                    promise.resolve();
                }, function (exception) {
                    promise.reject(exception);
                });
            };
            var this_2 = this;
            for (var _i = 0, raciRoles_1 = raciRoles; _i < raciRoles_1.length; _i++) {
                var role = raciRoles_1[_i];
                var state_1 = _loop_2(role);
                if (typeof state_1 === "object")
                    return state_1.value;
            }
            return promise.promise();
        };
        FascAutorizza.prototype.updateFascicleVisibilityType = function (fascicleVisibilityType) {
            var promise = $.Deferred();
            if (this._fascicleModel.FascicleType === FascicleType.Procedure) {
                this._fascicleModel.VisibilityType = fascicleVisibilityType;
                this.service.updateFascicle(this._fascicleModel, null, function (data) {
                    promise.resolve();
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            return promise.promise();
        };
        return FascAutorizza;
    }(FascicleBase));
    return FascAutorizza;
});
//# sourceMappingURL=FascAutorizza.js.map