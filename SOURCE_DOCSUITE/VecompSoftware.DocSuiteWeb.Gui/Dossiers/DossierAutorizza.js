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
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Models/Commons/UscRoleRestEventType", "App/Models/Commons/AuthorizationRoleType", "./DossierBase", "App/Helpers/PageClassHelper", "App/Services/Dossiers/DossierRoleService", "App/Models/Dossiers/DossierRoleStatus"], function (require, exports, ServiceConfigurationHelper, UscRoleRestEventType, AuthorizationRoleType, DossierBase, PageClassHelper, DossierRoleService, DossierRoleStatus) {
    var DossierAutorizza = /** @class */ (function (_super) {
        __extends(DossierAutorizza, _super);
        function DossierAutorizza(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME)) || this;
            _this.dossierRolesList = [];
            _this.dossierRolesToDeleteList = [];
            _this.btnConfirm_clicked = function (sender, args) {
                _this._loadingPanel.show(_this.dossierPageContentId);
                var roleToExclude = _this.findCommonRoles(_this.dossierRolesList, _this.dossierRolesToDeleteList);
                var _loop_1 = function (roleIdToDelete) {
                    _this.dossierRolesList = _this.dossierRolesList.filter(function (x) { return x.Role.IdRole !== roleIdToDelete.Role.IdRole; });
                    _this.dossierRolesToDeleteList = _this.dossierRolesToDeleteList.filter(function (x) { return x.Role.IdRole !== roleIdToDelete.Role.IdRole; });
                };
                for (var _i = 0, roleToExclude_1 = roleToExclude; _i < roleToExclude_1.length; _i++) {
                    var roleIdToDelete = roleToExclude_1[_i];
                    _loop_1(roleIdToDelete);
                }
                $.when(_this.insertDossierRoles(), _this.removeDossierRoles())
                    .done(function () {
                    _this._loadingPanel.show(_this.dossierPageContentId);
                    window.location.href = "DossierVisualizza.aspx?Type=Dossier&IdDossier=".concat(_this.currentDossierId);
                })
                    .fail(function (exception) {
                    _this._loadingPanel.hide(_this.dossierPageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception, "E' avvenuto un errore durante la fase di salvataggio delle autorizzazioni.");
                });
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        DossierAutorizza.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._lblStartDate = $("#".concat(this.lblStartDateId));
            this._lblYear = $("#".concat(this.lblYearId));
            this._lblNumber = $("#".concat(this.lblNumberId));
            this._btnConfirm = $find(this.btnConfirmId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._loadingPanel.show(this.dossierPageContentId);
            this._uscRoleRest = $("#" + this.uscRoleRestId).data();
            this.registerUscRoleRestEventHandlers();
            var dossierRoleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DossierRole");
            this._dossierRoleService = new DossierRoleService(dossierRoleConfiguration);
            this.service.isManageableDossier(this.currentDossierId, function (data) {
                if (data == null)
                    return;
                if (data) {
                    $.when(_this.loadData(), _this.loadRoles()).done(function () {
                        _this._btnConfirm.set_enabled(true);
                        _this._btnConfirm.add_clicked(_this.btnConfirm_clicked);
                        _this._loadingPanel.hide(_this.dossierPageContentId);
                    }).fail(function (exception) {
                        _this._btnConfirm.set_enabled(false);
                        _this._loadingPanel.hide(_this.dossierPageContentId);
                        _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento del Dossier.");
                    });
                }
                else {
                    _this._btnConfirm.set_enabled(false);
                    _this._loadingPanel.hide(_this.dossierPageContentId);
                    $("#".concat(_this.dossierPageContentId)).hide();
                    _this.showNotificationMessage(_this.uscNotificationId, "Errore in inizializzazione pagina.<br \> Utente non autorizzato alla modifica del Dossier.");
                }
            }, function (exception) {
                _this._btnConfirm.set_enabled(false);
                _this._loadingPanel.hide(_this.dossierPageContentId);
                $("#".concat(_this.dossierPageContentId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        DossierAutorizza.prototype.registerUscRoleRestEventHandlers = function () {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleRestId)
                .done(function (instance) {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, function (roleId) {
                    var dossierRoleId = _this._DossierRoles.filter(function (x) { return x.EntityShortId == roleId; }).map(function (y) { return y.UniqueId; })[0];
                    _this.dossierRolesToDeleteList.push({ UniqueId: dossierRoleId, Role: { IdRole: roleId } });
                    return $.Deferred().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, function (newAddedRoles) {
                    var existedRole;
                    var _loop_2 = function (dossierRole) {
                        existedRole = newAddedRoles.filter(function (x) { return x.EntityShortId === dossierRole.EntityShortId; })[0];
                        if (existedRole) {
                            alert("Non \u00E8 possibile selezionare il settore " + existedRole.Name + " in quanto gi\u00E0 presente come settore autorizzato del fascicolo");
                            return { value: void 0 };
                        }
                    };
                    for (var _i = 0, _a = _this._DossierRoles; _i < _a.length; _i++) {
                        var dossierRole = _a[_i];
                        var state_1 = _loop_2(dossierRole);
                        if (typeof state_1 === "object")
                            return state_1.value;
                    }
                    var dossierRoles = newAddedRoles.map(function (x) { return ({
                        Role: x,
                        IsMaster: false,
                        AuthorizationRoleType: AuthorizationRoleType.Accounted
                    }); });
                    for (var _b = 0, dossierRoles_1 = dossierRoles; _b < dossierRoles_1.length; _b++) {
                        var dossierRole = dossierRoles_1[_b];
                        _this.dossierRolesList.push(dossierRole);
                    }
                    return $.Deferred().resolve(existedRole);
                });
            });
        };
        DossierAutorizza.prototype.loadData = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this.service.getDossier(this.currentDossierId, function (data) {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        _this._lblYear.html(data.Year.toString());
                        _this._lblNumber.html(data.Number);
                        _this._lblStartDate.html(data.FormattedStartDate);
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        DossierAutorizza.prototype.loadRoles = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this.service.getDossierRoles(this.currentDossierId, function (data) {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        //ritorna solo quello attivo
                        _this._DossierRoles = data;
                        var roles = [];
                        for (var _i = 0, _a = _this._DossierRoles; _i < _a.length; _i++) {
                            var role = _a[_i];
                            if (role.IsMaster == true) {
                                continue;
                            }
                            var newRole = {
                                UniqueId: role.UniqueId,
                                Name: role.Name,
                                IdRole: role.EntityShortId,
                                IsActive: 1
                            };
                            roles.push(newRole);
                        }
                        _this._uscRoleRest.renderRolesTree(roles);
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        DossierAutorizza.prototype.removeDossierRoles = function () {
            var promise = $.Deferred();
            try {
                var ajaxPromises = [];
                var dossierModel = {};
                dossierModel.DossierRoles = this.dossierRolesToDeleteList;
                for (var _i = 0, _a = dossierModel.DossierRoles; _i < _a.length; _i++) {
                    var dossierRole = _a[_i];
                    dossierRole.Dossier = {};
                    dossierRole.Dossier.UniqueId = this.currentDossierId;
                    ajaxPromises.push(this.removeDossierRole(dossierRole));
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
        DossierAutorizza.prototype.removeDossierRole = function (dossierRole) {
            var promise = $.Deferred();
            try {
                this._dossierRoleService.deleteDossierRole(dossierRole, function (data) {
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
        DossierAutorizza.prototype.insertDossierRoles = function () {
            var promise = $.Deferred();
            try {
                var ajaxPromises = [];
                var dossierModel = {};
                dossierModel.DossierRoles = this.dossierRolesList;
                for (var _i = 0, _a = dossierModel.DossierRoles; _i < _a.length; _i++) {
                    var dossierRole = _a[_i];
                    dossierRole.Dossier = {};
                    dossierRole.Dossier.UniqueId = this.currentDossierId;
                    dossierRole.Status = DossierRoleStatus[DossierRoleStatus.Active];
                    ajaxPromises.push(this.insertDossierRole(dossierRole));
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
        DossierAutorizza.prototype.insertDossierRole = function (dossierRole) {
            var promise = $.Deferred();
            try {
                this._dossierRoleService.insertDossierRole(dossierRole, function (data) {
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
        DossierAutorizza.prototype.findCommonRoles = function (insertedRoles, deletedRoles) {
            var sameDossierRolesModel = [];
            for (var _i = 0, insertedRoles_1 = insertedRoles; _i < insertedRoles_1.length; _i++) {
                var insertedRole = insertedRoles_1[_i];
                for (var _a = 0, deletedRoles_1 = deletedRoles; _a < deletedRoles_1.length; _a++) {
                    var deletedRole = deletedRoles_1[_a];
                    if (insertedRole.Role.IdRole == deletedRole.Role.IdRole) {
                        var dossierRoleModel = {
                            Role: {
                                Name: insertedRole.Role.Name,
                                IdRole: insertedRole.Role.IdRole
                            }
                        };
                        sameDossierRolesModel.push(dossierRoleModel);
                    }
                }
            }
            return sameDossierRolesModel;
        };
        return DossierAutorizza;
    }(DossierBase));
    return DossierAutorizza;
});
//# sourceMappingURL=DossierAutorizza.js.map