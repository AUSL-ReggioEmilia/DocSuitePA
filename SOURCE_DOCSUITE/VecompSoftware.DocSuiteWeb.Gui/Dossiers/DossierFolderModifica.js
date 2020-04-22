/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
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
define(["require", "exports", "Dossiers/DossierBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Dossiers/DossierFolderService", "App/Services/Commons/RoleService", "App/Mappers/Dossiers/DossierFolderSummaryModelMapper", "App/Models/Commons/CategoryModel", "App/Models/Commons/AuthorizationRoleType", "App/Models/Dossiers/DossierRoleStatus", "App/Models/Fascicles/FascicleModel", "App/Models/UpdateActionType", "App/DTOs/ValidationExceptionDTO", "App/Services/Dossiers/DossierFolderLocalService", "App/Services/Commons/RoleLocalService"], function (require, exports, DossierBase, ServiceConfigurationHelper, DossierFolderService, RoleService, DossierFolderSummaryModelMapper, CategoryModel, AuthorizationRoleType, DossierRoleStatus, FascicleModel, UpdateActionType, ValidationExceptionDTO, DossierFolderLocalService, RoleLocalService) {
    var DossierFolderModifica = /** @class */ (function (_super) {
        __extends(DossierFolderModifica, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function DossierFolderModifica(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME)) || this;
            /**
            * ---------------------------- Events ---------------------------------
            */
            /**
            * Evento al click del bottone conferma
            * @param sender
            * @param eventArgs
            * @returns
            */
            _this.btnConferma_ButtonClicked = function (sender, eventArgs) {
                _this._loadingPanel.show(_this.currentPageId);
                _this._btnConferma.set_enabled(false);
                _this.modifyDossierFolder();
            };
            /*
            * ---------------------------- Methods ----------------------------
            */
            /**
            * Recupero i dati della cartella dalla SessionStorage
            * @param idDossier
            */
            _this.getFolderFromSessionStorage = function (idDossier) {
                var dossierFolder = {};
                var result = sessionStorage[idDossier];
                if (result == null) {
                    return null;
                }
                var source = JSON.parse(result);
                if (source) {
                    dossierFolder.UniqueId = source.UniqueId;
                    dossierFolder.Name = source.Name;
                    dossierFolder.Status = source.Status;
                    dossierFolder.idCategory = source.idCategory;
                    dossierFolder.idRole = source.idRole;
                    dossierFolder.idFascicle = source.idFascicle;
                }
                return dossierFolder;
            };
            /**
            * Evento al cambio di classificatore
            */
            _this.onCategoryChanged = function (idCategory) {
                _this.selectedCategoryId = idCategory;
                $("#".concat(_this.currentPageId)).data(_this);
            };
            /**
        * Recupero il settore salvato nella session storage dallo usc dei settori
        * @param uscId
        */
            _this.getUscRoles = function (uscId) {
                var dossierFolderRoles = new Array();
                var uscRoles = $("#".concat(uscId)).data();
                if (!jQuery.isEmptyObject(uscRoles)) {
                    var source = JSON.parse(uscRoles.getRoles());
                    if (source) {
                        var _loop_1 = function (s) {
                            var dossierFolderRole = {};
                            dossierFolderRole.Role = s;
                            dossierFolderRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                            dossierFolderRole.Status = DossierRoleStatus.Active;
                            //devo mettere lo uniqueid ai settori che erano già presenti nel dossier
                            var roleFolder = _this._dossierFolderRoles.filter(function (item) {
                                if (item.Role.IdRole == s.EntityShortId && item.Role.TenantId == s.TenantId) {
                                    return item;
                                }
                            });
                            if (roleFolder && roleFolder[0]) {
                                dossierFolderRole.UniqueId = roleFolder[0].UniqueId;
                            }
                            dossierFolderRoles.push(dossierFolderRole);
                        };
                        for (var _i = 0, source_1 = source; _i < source_1.length; _i++) {
                            var s = source_1[_i];
                            _loop_1(s);
                        }
                    }
                }
                return dossierFolderRoles;
            };
            _this.getFolderRoles = function () {
                var promise = $.Deferred();
                try {
                    var currentFolder = _this.getFolderFromSessionStorage(_this.currentDossierId);
                    _this._roleService.getDossierFolderRole(currentFolder.UniqueId, function (data) {
                        if (data == null) {
                            promise.resolve();
                            return;
                        }
                        var dossierFolderRoles = new Array();
                        for (var _i = 0, data_1 = data; _i < data_1.length; _i++) {
                            var role = data_1[_i];
                            var dossierFolderRoleModel = {};
                            var r = {};
                            r.EntityShortId = role.EntityShortId;
                            r.IdRole = role.EntityShortId;
                            r.Name = role.Name;
                            r.TenantId = role.TenantId;
                            r.IdRoleTenant = role.IdRoleTenant;
                            dossierFolderRoleModel.UniqueId = role.DossierFolderRoles[0].UniqueId;
                            dossierFolderRoleModel.Role = r;
                            dossierFolderRoleModel.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                            dossierFolderRoleModel.Status = DossierRoleStatus.Active;
                            dossierFolderRoles.push(dossierFolderRoleModel);
                        }
                        _this._dossierFolderRoles = dossierFolderRoles;
                        promise.resolve();
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
            _this.propagateAuthorizations = function (data, dossierFolder) {
                var updateActionType = null;
                var uscRoles = $("#".concat(_this.uscRoleId)).data();
                if (!jQuery.isEmptyObject(uscRoles)) {
                    var checkedChoice = uscRoles.getPropagateAuthorizationsChecked();
                    if (checkedChoice === 1) {
                        updateActionType = UpdateActionType.DossierFolderAuthorizationsPropagation;
                    }
                }
                if (!updateActionType) {
                    _this.closeDossierFolderModifica(data, dossierFolder);
                    return;
                }
                _this._dossierFolderService.updateDossierFolder(data, updateActionType, function (data) {
                    _this.closeDossierFolderModifica(data, dossierFolder);
                }, function (exception) {
                    _this.exceptionWindow(data.Name, exception);
                    _this._loadingPanel.hide(_this.currentPageId);
                    _this._btnConferma.set_enabled(true);
                });
            };
            _this.closeDossierFolderModifica = function (data, dossierFolder) {
                var model = {};
                model.ActionName = "ModifyFolder";
                model.Value = [];
                var mapper = new DossierFolderSummaryModelMapper();
                var resultModel = mapper.Map(data);
                if (dossierFolder.Fascicle) {
                    resultModel.idFascicle = dossierFolder.Fascicle.UniqueId;
                }
                model.Value.push(JSON.stringify(resultModel));
                _this._loadingPanel.hide(_this.currentPageId);
                _this.closeWindow(model);
            };
            _this.exceptionWindow = function (dossierFolderName, exception) {
                var message;
                var ex = exception;
                if (dossierFolderName && dossierFolderName != "") {
                    message = "Attenzione: la cartella ".concat(dossierFolderName, " è stata modificata correttamente, ma sono occorsi degli errori in fase di propagazione delle autorizzazioni.<br /> <br />");
                    if (exception && exception instanceof ValidationExceptionDTO && exception.validationMessages.length > 0) {
                        message = message.concat("Gli errori sono i seguenti: <br />");
                        exception.validationMessages.forEach(function (item) {
                            message = message.concat(item.message, "<br />");
                        });
                    }
                    ex = null;
                }
                _this.showNotificationException(_this.uscNotificationId, ex, message);
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
         * Inizializzazione
         */
        DossierFolderModifica.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._btnConferma = $find(this.btnConfermaId);
            this._btnConferma.add_clicked(this.btnConferma_ButtonClicked);
            this._txtName = $find(this.txtNameId);
            this._loadingPanel = $find(this.loadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            if (this.persistanceDisabled) {
                this._dossierFolderService = new DossierFolderLocalService();
            }
            else {
                var dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME);
                this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            }
            var roleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.ROLE_TYPE_NAME);
            if (this.persistanceDisabled) {
                this._roleService = new RoleLocalService(roleConfiguration);
            }
            else {
                this._roleService = new RoleService(roleConfiguration);
            }
            this._dossierFolderRoles = [];
            this._loadingPanel.show(this.currentPageId);
            this._currentFolder = this.getFolderFromSessionStorage(this.currentDossierId);
            this.setData(this._currentFolder);
            this.bindLoaded();
        };
        DossierFolderModifica.prototype.modifyDossierFolder = function () {
            var _this = this;
            var dossierFolder = {};
            dossierFolder.Name = this._txtName.get_textBoxValue();
            dossierFolder.UniqueId = this._currentFolder.UniqueId;
            if (this._currentFolder.idFascicle) {
                var fasc = new FascicleModel();
                fasc.UniqueId = this._currentFolder.idFascicle;
                dossierFolder.Fascicle = fasc;
            }
            if (this.selectedCategoryId) {
                var category = new CategoryModel;
                category.EntityShortId = this.selectedCategoryId;
                dossierFolder.Category = category;
            }
            dossierFolder.DossierFolderRoles = this.getUscRoles(this.uscRoleId);
            this._dossierFolderService.updateDossierFolder(dossierFolder, null, function (data) {
                if (data == null)
                    return;
                _this.propagateAuthorizations(data, dossierFolder);
            }, function (exception) {
                _this._loadingPanel.hide(_this.currentPageId);
                _this.showNotificationException(_this.uscNotificationId, exception);
                _this._btnConferma.set_enabled(true);
            });
        };
        /**
         *  Imposto i dati della cartella nella pagina di modifica
         * @param dossierFolder
         */
        DossierFolderModifica.prototype.setData = function (dossierFolder) {
            var _this = this;
            this._txtName.set_value(dossierFolder.Name);
            $.when(this.getFolderRoles()).done(function () {
                var ajaxModel = {};
                ajaxModel.Value = new Array();
                var roles = new Array();
                if (_this._dossierFolderRoles != null) {
                    for (var _i = 0, _a = _this._dossierFolderRoles; _i < _a.length; _i++) {
                        var role = _a[_i];
                        var roleModel = {};
                        roleModel.EntityShortId = role.Role.EntityShortId;
                        roleModel.IdRole = role.Role.EntityShortId;
                        roleModel.IdRoleTenant = role.Role.IdRoleTenant;
                        roleModel.TenantId = role.Role.TenantId;
                        roleModel.Name = role.Role.Name;
                        roles.push(roleModel);
                    }
                }
                ajaxModel.Value.push(JSON.stringify(roles));
                ajaxModel.Value.push(JSON.stringify(dossierFolder.idCategory));
                ajaxModel.ActionName = "LoadExternalData";
                _this._ajaxManager = $find(_this.ajaxManagerId);
                _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
            }).fail(function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento dei settori autorizzati alla cartella.");
            });
        };
        /**
         * Metodo chiamato al completamento del caricamento degli usercontrol
         */
        DossierFolderModifica.prototype.loadExternalDataCallback = function () {
            if (this._currentFolder.idFascicle) {
                $('#'.concat(this.fascRowId)).show();
                $('#'.concat(this.lblFascNameId)).text(this._currentFolder.Name);
                $('#'.concat(this.nameRowId)).hide();
            }
            this._loadingPanel.hide(this.currentPageId);
        };
        /**
        * salvo lo stato corrente della pagina
        */
        DossierFolderModifica.prototype.bindLoaded = function () {
            $("#".concat(this.currentPageId)).data(this);
        };
        return DossierFolderModifica;
    }(DossierBase));
    return DossierFolderModifica;
});
//# sourceMappingURL=DossierFolderModifica.js.map