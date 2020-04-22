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
define(["require", "exports", "Dossiers/DossierBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Dossiers/DossierFolderService", "App/Services/Commons/RoleService", "App/Models/Dossiers/DossierFolderStatus", "App/Models/Commons/AuthorizationRoleType", "App/Models/Dossiers/DossierRoleStatus", "App/Mappers/Dossiers/DossierFolderSummaryModelMapper", "App/DTOs/ValidationExceptionDTO", "App/Services/Dossiers/DossierFolderLocalService", "App/Services/Commons/RoleLocalService"], function (require, exports, DossierBase, ServiceConfigurationHelper, DossierFolderService, RoleService, DossierFolderStatus, AuthorizationRoleType, DossierRoleStatus, DossierFolderSummaryModelMapper, ValidationExceptionDTO, DossierFolderLocalService, RoleLocalService) {
    var DossierFolderInserimento = /** @class */ (function (_super) {
        __extends(DossierFolderInserimento, _super);
        /**
    * Costruttore
    * @param serviceConfiguration
    */
        function DossierFolderInserimento(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME)) || this;
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
            _this.btmConferma_ButtonClicked = function (sender, eventArgs) {
                if (!Page_IsValid) {
                    return;
                }
                _this._loadingPanel.show(_this.currentPageId);
                _this._btnConferma.set_enabled(false);
                _this.insertDossierFolder();
            };
            _this.callInsertDossierFolderService = function (dossierFolder) {
                _this._dossierFolderService.insertDossierFolder(dossierFolder, null, function (data) {
                    if (data == null)
                        return;
                    var model = {};
                    model.ActionName = "ManageParent";
                    model.Value = [];
                    var mapper = new DossierFolderSummaryModelMapper();
                    var modelDossierFolderSummary = mapper.Map(data);
                    if (dossierFolder.Fascicle != null && dossierFolder.Fascicle.UniqueId != null) {
                        modelDossierFolderSummary.idFascicle = dossierFolder.Fascicle.UniqueId;
                    }
                    model.Value.push(JSON.stringify(modelDossierFolderSummary));
                    _this._loadingPanel.hide(_this.currentPageId);
                    _this.closeWindow(model);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.currentPageId);
                    _this.exceptionWindow(exception);
                    _this._btnConferma.set_enabled(true);
                });
            };
            _this.exceptionWindow = function (exception) {
                var message = "";
                var ex = exception;
                if (exception && exception instanceof ValidationExceptionDTO && exception.validationMessages.length > 0) {
                    message = message.concat("Gli errori sono i seguenti: <br />");
                    exception.validationMessages.forEach(function (item) {
                        message = message.concat(item.message, "<br />");
                    });
                }
                _this.showNotificationException(_this.uscNotificationId, ex, message);
            };
            /**
            * Recupero la cartella salvata nella session storage
            * @param idDossier
            */
            _this.getFolderParent = function (idDossier) {
                var dossierFolder = {};
                var dossierFolderRole = {};
                dossierFolder.DossierFolderRoles = new Array();
                var result = sessionStorage[idDossier];
                if (result == null) {
                    return null;
                }
                var source = JSON.parse(result);
                if (source) {
                    dossierFolder.UniqueId = source.UniqueId;
                }
                return dossierFolder;
            };
            _this.getFolderParentRoles = function () {
                var promise = $.Deferred();
                try {
                    var parentFolder = _this.getFolderParent(_this.currentDossierId);
                    _this._roleService.getDossierFolderRole(parentFolder.UniqueId, function (data) {
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
                            dossierFolderRoleModel.Role = r;
                            dossierFolderRoleModel.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                            dossierFolderRoleModel.Status = DossierRoleStatus.Active;
                            dossierFolderRoles.push(dossierFolderRoleModel);
                        }
                        _this._dossierParentFolderRoles = dossierFolderRoles;
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
                        for (var _i = 0, source_1 = source; _i < source_1.length; _i++) {
                            var s = source_1[_i];
                            var dossierFolderRole = {};
                            dossierFolderRole.Role = s;
                            dossierFolderRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                            dossierFolderRole.Status = DossierRoleStatus.Active;
                            dossierFolderRoles.push(dossierFolderRole);
                        }
                    }
                }
                return dossierFolderRoles;
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        * Inizializzazione
        */
        DossierFolderInserimento.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnConferma = $find(this.btnConfermaId);
            this._btnConferma.add_clicked(this.btmConferma_ButtonClicked);
            this._txtName = $find(this.txtNameId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._manager = $find(this.managerId);
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
            $.when(this.getFolderParentRoles()).done(function () {
                var ajaxModel = {};
                ajaxModel.Value = new Array();
                var roles = new Array();
                if (_this._dossierParentFolderRoles != null) {
                    for (var _i = 0, _a = _this._dossierParentFolderRoles; _i < _a.length; _i++) {
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
                ajaxModel.ActionName = "DossierFolderInserimentoLoadExternalData";
                $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
            }).fail(function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento dei settori autorizzati alla cartella madre.");
            });
            $("#".concat(this.dossierNameRowId)).show();
            $("#".concat(this.dossierRolesRowId)).show();
            this._loadingPanel.hide(this.currentPageId);
        };
        /**
        *---------------------------- Methods ---------------------------
        */
        DossierFolderInserimento.prototype.insertDossierFolder = function () {
            var dossierFolder = {};
            var dossier = {};
            dossier.UniqueId = this.currentDossierId;
            dossierFolder.Status = DossierFolderStatus.InProgress;
            dossierFolder.Name = this._txtName.get_textBoxValue();
            dossierFolder.Dossier = dossier;
            var dossierFolderToUpdate = this.getFolderParent(this.currentDossierId);
            if (dossierFolderToUpdate) {
                dossierFolder.ParentInsertId = dossierFolderToUpdate.UniqueId;
            }
            ;
            dossierFolder.DossierFolderRoles = this.getUscRoles(this.uscFolderAccountedId);
            this.callInsertDossierFolderService(dossierFolder);
        };
        return DossierFolderInserimento;
    }(DossierBase));
    return DossierFolderInserimento;
});
//# sourceMappingURL=DossierFolderInserimento.js.map