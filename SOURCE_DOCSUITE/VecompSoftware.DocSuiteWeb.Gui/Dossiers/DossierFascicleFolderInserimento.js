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
define(["require", "exports", "Dossiers/DossierBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Dossiers/DossierFolderService", "App/Services/Commons/RoleService", "App/Models/Commons/CategoryModel", "App/Models/Dossiers/DossierFolderStatus", "App/Models/Commons/AuthorizationRoleType", "App/Models/Dossiers/DossierRoleStatus", "App/Mappers/Dossiers/DossierFolderSummaryModelMapper", "App/Models/Fascicles/FascicleType", "App/Services/Fascicles/FascicleService", "App/DTOs/ValidationExceptionDTO", "App/Models/Commons/MetadataRepositoryModel", "App/Services/Dossiers/DossierFolderLocalService", "App/Services/Fascicles/FascicleLocalService", "App/Services/Commons/RoleLocalService", "App/Helpers/PageClassHelper", "App/Helpers/GenericHelper", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, DossierBase, ServiceConfigurationHelper, DossierFolderService, RoleService, CategoryModel, DossierFolderStatus, AuthorizationRoleType, DossierRoleStatus, DossierFolderSummaryModelMapper, FascicleType, FascicleService, ValidationExceptionDTO, MetadataRepositoryModel, DossierFolderLocalService, FascicleLocalService, RoleLocalService, PageClassHelper, GenericHelper, SessionStorageKeysHelper) {
    var DossierFascicleFolderInserimento = /** @class */ (function (_super) {
        __extends(DossierFascicleFolderInserimento, _super);
        /**
    * Costruttore
    * @param serviceConfiguration
    */
        function DossierFascicleFolderInserimento(serviceConfigurations) {
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
                var externalValidation = function () { return $.Deferred().resolve(true).promise(); };
                if (!_this.processEnabled) {
                    externalValidation = function () { return _this.fascicleExternalValidation(); };
                }
                externalValidation()
                    .done(function (isValid) {
                    if (!isValid || !Page_IsValid) {
                        return;
                    }
                    _this._loadingPanel.show(_this.currentPageId);
                    _this._btnConferma.set_enabled(false);
                    if (_this.processEnabled) {
                        PageClassHelper.callUserControlFunctionSafe(_this.fascicleInsertControlId).done(function (instance) {
                            instance.fillMetadataModel().done(function (metadatas) {
                                if (!metadatas) {
                                    _this._btnConferma.set_enabled(true);
                                    return;
                                }
                                _this.insertDossierFolder(0, metadatas[0], metadatas[1]);
                            });
                        });
                    }
                    else {
                        PageClassHelper.callUserControlFunctionSafe(_this.fascicleInsertControlId).done(function (instance) {
                            var ajaxModel = {};
                            ajaxModel.Value = new Array();
                            ajaxModel.ActionName = "Insert";
                            _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                        });
                    }
                });
            };
            _this.callInsertDossierFolderService = function (dossierFolder, fascicleTitle) {
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
                    _this.exceptionWindow(fascicleTitle, exception);
                    _this._btnConferma.set_enabled(true);
                });
            };
            _this.exceptionWindow = function (fascicleTitle, exception) {
                var message;
                var ex = exception;
                if (!String.isNullOrEmpty(fascicleTitle)) {
                    message = "Attenzione: il fascicolo ".concat(fascicleTitle, " è stato creato correttamente ma sono occorsi degli errori in fase di creazione della cartella.<br /> <br />");
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
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        Object.defineProperty(DossierFascicleFolderInserimento.prototype, "currentFascicleInsertInstanceFactory", {
            get: function () {
                //TODO UscFascicleInsert prepopulate in the future
                if (this.processEnabled) {
                    return PageClassHelper.callUserControlFunctionSafe(this.fascicleInsertControlId);
                }
                else {
                    return PageClassHelper.callUserControlFunctionSafe(this.fascicleInsertControlId);
                }
            },
            enumerable: true,
            configurable: true
        });
        /**
        * Inizializzazione
        */
        DossierFascicleFolderInserimento.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnConferma = $find(this.btnConfermaId);
            this._btnConferma.add_clicked(this.btmConferma_ButtonClicked);
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
            if (this.persistanceDisabled) {
                this._fascicleService = new FascicleLocalService();
            }
            else {
                var fascicleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.FASCICLE_TYPE_NAME);
                this._fascicleService = new FascicleService(fascicleConfiguration);
            }
            this.currentFascicleInsertInstanceFactory
                .done(function (instance) {
                $("#".concat(_this.fascicleTypeRow)).show();
            });
            $.when(this.getFolderParentRoles()).done(function () {
                _this.fascicleId = GenericHelper.getUrlParams(window.location.href, "idFascicle");
                _this.actionType = GenericHelper.getUrlParams(window.location.href, "ActionType");
                if (_this.fascicleId && _this.actionType == DossierFascicleFolderInserimento.updateActionType) {
                    var sessionModel = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOSSIERFOLDERS_SESSIONNAME));
                    var fascicle = sessionModel.filter(function (x) { return x.Fascicle && x.Fascicle.UniqueId == _this.fascicleId; })[0].Fascicle;
                    _this.populateFascicleFields(fascicle);
                }
            }).fail(function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento dei settori autorizzati alla cartella madre.");
            });
            this._loadingPanel.hide(this.currentPageId);
        };
        /**
        *---------------------------- Methods ---------------------------
        */
        DossierFascicleFolderInserimento.prototype.insertDossierFolder = function (responsibleContact, metadataDesignerModel, metadataValueModels) {
            var _this = this;
            this.currentFascicleInsertInstanceFactory
                .done(function (instance) {
                var dossierFolder = {};
                var dossier = {};
                dossier.UniqueId = _this.currentDossierId;
                dossierFolder.Status = DossierFolderStatus.InProgress;
                dossierFolder.Dossier = dossier;
                var dossierFolderToUpdate = _this.getFolderParent(_this.currentDossierId);
                if (dossierFolderToUpdate) {
                    dossierFolder.ParentInsertId = dossierFolderToUpdate.UniqueId;
                }
                ;
                var deferredAction = $.Deferred().resolve(instance.getFascicle()).promise();
                if (_this.processEnabled) {
                    deferredAction = instance.getFascicle();
                }
                deferredAction.done(function (fascicle) {
                    if (!!metadataValueModels) {
                        fascicle.MetadataDesigner = metadataDesignerModel;
                        fascicle.MetadataValues = metadataValueModels;
                        if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY)) {
                            var metadataRepository = new MetadataRepositoryModel();
                            metadataRepository.UniqueId = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_METADATA_REPOSITORY);
                            fascicle.MetadataRepository = metadataRepository;
                        }
                    }
                    if (responsibleContact > 0 && fascicle.FascicleType != FascicleType.Activity) {
                        var contactModel = {};
                        contactModel.EntityId = responsibleContact;
                        fascicle.Contacts.push(contactModel);
                    }
                    //imprimo il settore della cartella madre
                    dossierFolder.DossierFolderRoles = _this._dossierParentFolderRoles;
                    var fascicleUniqueId;
                    var dossierParentId;
                    _this._fascicleService.insertFascicle(fascicle, null, function (data) {
                        var savedFascicle = data;
                        dossierFolder.Status = DossierFolderStatus.Fascicle;
                        dossierFolder.Fascicle = savedFascicle;
                        var category = new CategoryModel();
                        category.EntityShortId = savedFascicle.Category.EntityShortId;
                        dossierFolder.Category = category;
                        if (_this.actionType == DossierFascicleFolderInserimento.updateActionType) {
                            var sessionModel = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOSSIERFOLDERS_SESSIONNAME));
                            if (sessionModel.filter(function (x) { return x.Fascicle && x.Fascicle.UniqueId == _this.fascicleId; })[0] &&
                                sessionModel.filter(function (x) { return x.Fascicle && x.Fascicle.UniqueId == _this.fascicleId; })[0].Fascicle) {
                                fascicleUniqueId = sessionModel.filter(function (x) { return x.Fascicle && x.Fascicle.UniqueId == _this.fascicleId; })[0].Fascicle.UniqueId;
                                dossierParentId = sessionModel.filter(function (x) { return x.Fascicle && x.Fascicle.UniqueId == _this.fascicleId; })[0].ParentInsertId;
                                dossierFolder.ParentInsertId = dossierParentId;
                                var arr = [];
                                for (var i = 0; i <= sessionModel.length - 1; i++) {
                                    if (!sessionModel[i].Fascicle || (sessionModel[i].Fascicle && sessionModel[i].Fascicle.UniqueId != fascicleUniqueId)) {
                                        arr.push(sessionModel[i]);
                                    }
                                }
                                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_DOSSIERFOLDERS_SESSIONNAME, JSON.stringify(arr));
                            }
                        }
                        _this.callInsertDossierFolderService(dossierFolder, savedFascicle.FascicleObject);
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.currentPageId);
                        _this.showNotificationException(_this.uscNotificationId, exception);
                        _this._btnConferma.set_enabled(true);
                    });
                });
            });
        };
        DossierFascicleFolderInserimento.prototype.fascicleExternalValidation = function () {
            var _this = this;
            var promise = $.Deferred();
            PageClassHelper.callUserControlFunctionSafe(this.fascicleInsertControlId)
                .done(function (instance) {
                var isFascValid = instance.isPageValid();
                var selectedFascicleType = instance.getSelectedFascicleType();
                if (!selectedFascicleType) {
                    _this.showNotificationMessage(_this.uscNotificationId, 'Selezionare una tipologia di fascicolo');
                }
                return promise.resolve((isFascValid && !!selectedFascicleType));
            });
            return promise.promise();
        };
        DossierFascicleFolderInserimento.prototype.populateFascicleFields = function (fascicle) {
            PageClassHelper.callUserControlFunctionSafe(this.fascicleInsertControlId).done(function (instance) {
                instance.populateInputs(fascicle);
            });
        };
        DossierFascicleFolderInserimento.updateActionType = "Update";
        return DossierFascicleFolderInserimento;
    }(DossierBase));
    return DossierFascicleFolderInserimento;
});
//# sourceMappingURL=DossierFascicleFolderInserimento.js.map