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
define(["require", "exports", "Dossiers/DossierBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Dossiers/DossierFolderService", "App/Services/Commons/RoleService", "App/Models/Commons/CategoryModel", "UserControl/uscFascicleLink", "App/Models/Fascicles/FascicleModel", "App/Models/Dossiers/DossierFolderStatus", "App/Mappers/Dossiers/DossierFolderSummaryModelMapper", "App/Models/InsertActionType", "App/Models/Commons/AuthorizationRoleType", "App/Models/Dossiers/DossierRoleStatus"], function (require, exports, DossierBase, ServiceConfigurationHelper, DossierFolderService, RoleService, CategoryModel, UscFascicleLink, FascicleModel, DossierFolderStatus, DossierFolderSummaryModelMapper, InsertActionType, AuthorizationRoleType, DossierRoleStatus) {
    var DossierFolderLinkFascicle = /** @class */ (function (_super) {
        __extends(DossierFolderLinkFascicle, _super);
        /**
        * Costruttore
        * @param serviceConfiguration
        */
        function DossierFolderLinkFascicle(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME)) || this;
            /**
            * Evento scatenato al click del pulsante ConfermaInserimento
            * @method
            * @param sender
            * @param eventArgs
            * @returns
            */
            _this.btmConferma_ButtonClicked = function (sender, eventArgs) {
                _this._loadingPanel.show(_this.currentPageId);
                _this._btnConferma.set_enabled(false);
                var uscFascLink = $("#".concat(_this.uscFascLinkId)).data();
                if (!jQuery.isEmptyObject(uscFascLink)) {
                    if (!uscFascLink.currentFascicleId) {
                        _this._loadingPanel.hide(_this.currentPageId);
                        _this.showNotificationException(_this.uscNotificationId, null, "Selezionare un fascicolo");
                        _this._btnConferma.set_enabled(true);
                        return;
                    }
                }
                _this.updateDossierFolder();
            };
            /**
               * Recupero la cartella salvata nella session storage
               * @param idDossier
               */
            _this.getFolder = function (idDossier) {
                var dossierFolder = {};
                var result = sessionStorage[idDossier];
                if (result == null) {
                    return null;
                }
                var source = JSON.parse(result);
                if (source) {
                    dossierFolder.UniqueId = source.UniqueId;
                    dossierFolder.idParent = source.idParent;
                    dossierFolder.Name = source.Name;
                    dossierFolder.Status = source.Status;
                    dossierFolder.idRole = source.idRole;
                    dossierFolder.idCategory = source.idCategory;
                }
                return dossierFolder;
            };
            _this.getFolderRoles = function () {
                var promise = $.Deferred();
                try {
                    _this._roleService.getDossierFolderRole(_this._currentFolder.UniqueId, function (data) {
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
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        /**
        *------------------------- Events -----------------------------
        */
        /**
        * Inizializzazione
        */
        DossierFolderLinkFascicle.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnConferma = $find(this.btnConfermaId);
            this._btnConferma.add_clicked(this.btmConferma_ButtonClicked);
            this._lblName = $("#".concat(this.lblNameId));
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._manager = $find(this.managerId);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DossierFolder");
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            var roleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.ROLE_TYPE_NAME);
            this._roleService = new RoleService(roleConfiguration);
            $("#".concat(this.uscFascLinkId)).bind(UscFascicleLink.LOADED_EVENT, function (args) {
                _this.loadFolder(_this.currentDossierId);
            });
            this.loadFolder(this.currentDossierId);
        };
        /**
        *------------------------- Methods -----------------------------
        */
        /*
        * Carico la cartella
        */
        DossierFolderLinkFascicle.prototype.loadFolder = function (idDossierFolder) {
            var _this = this;
            this._currentFolder = this.getFolder(this.currentDossierId);
            $.when(this.getFolderRoles()).done(function () {
                _this.setData(_this._currentFolder);
            }).fail(function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento dei settori autorizzati alla cartella.");
            });
        };
        /**
        * Aggiorno la cartella
        */
        DossierFolderLinkFascicle.prototype.updateDossierFolder = function () {
            var _this = this;
            var uscFascLink = $("#".concat(this.uscFascLinkId)).data();
            var dossierFolder = {};
            var dossier = {};
            dossier.UniqueId = this.currentDossierId;
            dossierFolder.Status = DossierFolderStatus.InProgress;
            dossierFolder.Dossier = dossier;
            dossierFolder.ParentInsertId = this._currentFolder.UniqueId;
            dossierFolder.Status = DossierFolderStatus.InProgress;
            var dossierFolderRoles = new Array();
            var dossierFolderRole = {};
            dossierFolder.DossierFolderRoles = this._dossierFolderRoles;
            if (!jQuery.isEmptyObject(uscFascLink)) {
                if (uscFascLink.currentFascicleId) {
                    var fascicle = new FascicleModel;
                    fascicle.UniqueId = uscFascLink.currentFascicleId;
                    dossierFolder.Status = DossierFolderStatus.Fascicle;
                    dossierFolder.Fascicle = fascicle;
                }
                if (uscFascLink.selectedCategoryId) {
                    var category = new CategoryModel;
                    category.EntityShortId = uscFascLink.selectedCategoryId;
                    dossierFolder.Category = category;
                }
            }
            this._dossierFolderService.insertDossierFolder(dossierFolder, InsertActionType.InsertDossierFolderAssociatedToFascicle, function (data) {
                if (data == null)
                    return;
                var model = {};
                model.ActionName = "AddFascicleLink";
                model.Value = [];
                var mapper = new DossierFolderSummaryModelMapper();
                //todo: da togliere quando riusciremo a farci tornare le navigation properties con la put
                var folder = mapper.Map(data);
                folder.idFascicle = dossierFolder.Fascicle.UniqueId;
                model.Value.push(JSON.stringify(folder));
                _this._loadingPanel.hide(_this.currentPageId);
                _this.closeWindow(model);
            }, function (exception) {
                _this._loadingPanel.hide(_this.currentPageId);
                _this.showNotificationException(_this.uscNotificationId, exception);
                _this._btnConferma.set_enabled(true);
            });
        };
        DossierFolderLinkFascicle.prototype.setData = function (dossierFolder) {
            this._lblName.html(dossierFolder.Name);
            var uscFascLink = $("#".concat(this.uscFascLinkId)).data();
            if (!jQuery.isEmptyObject(uscFascLink)) {
                if (dossierFolder.idCategory) {
                    uscFascLink.onExternalCategoryChange(dossierFolder.idCategory);
                }
            }
        };
        return DossierFolderLinkFascicle;
    }(DossierBase));
    return DossierFolderLinkFascicle;
});
//# sourceMappingURL=DossierFolderLinkFascicle.js.map