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
define(["require", "exports", "App/Models/Dossiers/DossierFolderStatus", "Dossiers/DossierBase", "App/Services/Commons/ContainerService", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscMetadataRepositorySel", "App/Services/Commons/ContainerPropertyService", "App/Services/Dossiers/DossierFolderService", "App/Models/InsertActionType", "App/Models/Commons/BuildActionType"], function (require, exports, DossierFolderStatus, DossierBase, ContainerService, ServiceConfigurationHelper, UscMetadataRepositorySel, ContainerPropertyService, DossierFolderService, InsertActionType, BuildActionType) {
    var DossierInserimento = /** @class */ (function (_super) {
        __extends(DossierInserimento, _super);
        /**
    * Costruttore
    * @param serviceConfiguration
    */
        function DossierInserimento(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME)) || this;
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
            _this.btnConfirm_OnClicked = function (sender, eventArgs) {
                if (Page_IsValid) {
                    _this._loadingPanel.show(_this.dossierPageContentId);
                    _this._btnConfirm.set_enabled(false);
                    $find(_this.ajaxManagerId).ajaxRequestWithTarget(_this.btnConfirmUniqueId, '');
                }
            };
            _this.rdlContainer_OnSelectedIndexChanged = function (sender, eventArgs) {
                if (sender.get_selectedItem()) {
                    var uscMetadataRepositorySel_1 = $("#".concat(_this.uscMetadataRepositorySelId)).data();
                    var uscDynamicMetadataClient_1 = $("#".concat(_this.uscDynamicMetadataId)).data();
                    uscMetadataRepositorySel_1.setRepositoryRestrictions([]);
                    uscMetadataRepositorySel_1.clearComboboxText();
                    uscMetadataRepositorySel_1.enableSelection();
                    uscDynamicMetadataClient_1.clearPage();
                    _this._selectedDossierFoldersModel = undefined;
                    _this._loadingPanel.show(_this.dossierPageContentId);
                    _this._containerPropertyService.getByContainer(Number(sender.get_selectedItem().get_value()), "DossierFoldersModel", function (data) {
                        if (!data || data.length == 0) {
                            _this._loadingPanel.hide(_this.dossierPageContentId);
                            return;
                        }
                        try {
                            var containerProperty = data[0];
                            if (containerProperty.ValueString) {
                                _this._selectedDossierFoldersModel = JSON.parse(containerProperty.ValueString);
                                uscMetadataRepositorySel_1.setRepositoryRestrictions(_this._selectedDossierFoldersModel.MetadataRestrictions);
                                if (_this._selectedDossierFoldersModel.SetMetadataReadOnly && _this._selectedDossierFoldersModel.MetadataRestrictions.length > 0) {
                                    var repositoryId = _this._selectedDossierFoldersModel.MetadataRestrictions[0];
                                    uscMetadataRepositorySel_1.setComboboxText(repositoryId);
                                    _this._selectedMetadataRepository = repositoryId;
                                    uscDynamicMetadataClient_1.loadMetadataRepository(repositoryId);
                                    uscMetadataRepositorySel_1.disableSelection();
                                }
                            }
                            _this._loadingPanel.hide(_this.dossierPageContentId);
                        }
                        catch (e) {
                            console.error(e);
                            _this._loadingPanel.hide(_this.dossierPageContentId);
                            _this.showNotificationMessage(_this.uscNotificationId, "Errore nella lettura degli automatismi associati al contenitore.");
                        }
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.dossierPageContentId);
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
            };
            _this.loadContainers = function () {
                _this._containerService.getDossierInsertAuthorizedContainers(function (data) {
                    var containers = data;
                    _this.addContainers(containers, _this._rdlContainer);
                    _this._btnConfirm.set_enabled(true);
                    _this._loadingPanel.hide(_this.dossierPageContentId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.dossierPageContentId);
                    _this._btnConfirm.set_enabled(false);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
       *------------------------- Methods -----------------------------
       */
        /**
        * Metodo di inizializzazione
        */
        DossierInserimento.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_OnClicked);
            this._txtNote = $find(this.txtNoteId);
            this._rdpStartDate = $find(this.rdpStartDateId);
            this._rdlContainer = $find(this.rdlContainerId);
            this._rdlContainer.add_selectedIndexChanged(this.rdlContainer_OnSelectedIndexChanged);
            this._manager = $find(this.ajaxManagerId);
            this._rowMetadataRepository = $("#".concat(this.rowMetadataId));
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            var containerConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Container");
            this._containerService = new ContainerService(containerConfiguration);
            var containerPropertyConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ContainerProperty");
            this._containerPropertyService = new ContainerPropertyService(containerPropertyConfiguration);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME);
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            this._rdpStartDate.set_selectedDate(this._rdpStartDate.get_focusedDate());
            this._btnConfirm.set_enabled(false);
            this._rowMetadataRepository.hide();
            this._loadingPanel.show(this.dossierPageContentId);
            this.checkInsertRight();
            if (this.metadataRepositoryEnabled) {
                this._rowMetadataRepository.show();
                $("#".concat(this.uscMetadataRepositorySelId)).on(UscMetadataRepositorySel.SELECTED_INDEX_EVENT, function (args, data) {
                    var uscDynamicMetadataClient = $("#".concat(_this.uscDynamicMetadataId)).data();
                    if (!jQuery.isEmptyObject(uscDynamicMetadataClient)) {
                        uscDynamicMetadataClient.clearPage();
                        _this._selectedMetadataRepository = data;
                        if (data) {
                            uscDynamicMetadataClient.loadMetadataRepository(data);
                        }
                    }
                });
            }
        };
        /**
          * Controllo Diritti di inserimento
          * @param
          */
        DossierInserimento.prototype.checkInsertRight = function () {
            var _this = this;
            this.service.hasInsertRight(function (data) {
                if (data == null)
                    return;
                if (data) {
                    _this.loadContainers();
                }
                else {
                    _this._btnConfirm.set_enabled(false);
                    _this._loadingPanel.hide(_this.dossierPageContentId);
                    $("#".concat(_this.dossierPageContentId)).hide();
                    _this.showNotificationMessage(_this.uscNotificationId, "Errore in inizializzazione pagina.<br \> Utente non autorizzato all'inserimento di un nuovo Dossier.");
                }
            }, function (exception) {
                _this._btnConfirm.set_enabled(false);
                _this._loadingPanel.hide(_this.dossierPageContentId);
                $("#".concat(_this.dossierPageContentId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
        * Callback da code-behind per l'inserimento di un Dossier
        * @param contact
        * @param category
        */
        DossierInserimento.prototype.insertCallback = function (contact, role) {
            var _this = this;
            var dossierModel = {};
            //riferimento
            this.fillContacts(contact, dossierModel);
            //settore
            this.fillRoles(role, dossierModel);
            this.fillModelFromPage(dossierModel);
            if (this.metadataRepositoryEnabled && this._selectedMetadataRepository && !this.fillMetadataModel(dossierModel)) {
                this._loadingPanel.hide(this.dossierPageContentId);
                this._btnConfirm = $find(this.btnConfirmId);
                this._btnConfirm.set_enabled(true);
                return;
            }
            this.service.insertDossier(dossierModel, function (data) {
                if (data == null)
                    return;
                if (_this._selectedDossierFoldersModel) {
                    $.when(_this.saveFolders(_this._selectedDossierFoldersModel.Folders, data.UniqueId))
                        .done(function () { return window.location.href = "DossierVisualizza.aspx?Type=Dossier&IdDossier=".concat(data.UniqueId, "&DossierTitle=", data.Year.toString(), "/", ("000000000" + data.Number.toString()).slice(-7)); })
                        .fail(function () {
                        _this.showNotificationMessage(_this.uscNotificationId, "Errore nella generazione automatica dell'alberatura del Dossier.");
                    });
                }
                else {
                    window.location.href = "DossierVisualizza.aspx?Type=Dossier&IdDossier=".concat(data.UniqueId, "&DossierTitle=", data.Year.toString(), "/", ("000000000" + data.Number.toString()).slice(-7));
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.dossierPageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        DossierInserimento.prototype.saveFolders = function (modelFolders, dossierId) {
            var _this = this;
            var promise = $.Deferred();
            var dossier = {};
            dossier.UniqueId = dossierId;
            $.when(this.persistFoldersRecursive(modelFolders, dossier, dossierId))
                .done(function () {
                if (modelFolders.length > 0) {
                    $.when(_this.saveFolders(modelFolders, dossierId))
                        .done(function () { return promise.resolve(); })
                        .fail(function () { return promise.reject(); });
                }
                else {
                    promise.resolve();
                }
            })
                .fail(function () { return promise.reject(); });
            return promise.promise();
        };
        DossierInserimento.prototype.persistFoldersRecursive = function (modelFolders, dossier, parentId) {
            var _this = this;
            var promise = $.Deferred();
            if (!modelFolders) {
                return promise.resolve();
            }
            var folder = modelFolders.shift();
            var dossierFolder = {};
            dossierFolder.Status = DossierFolderStatus.InProgress;
            dossierFolder.Name = folder.Name;
            dossierFolder.ParentInsertId = parentId;
            dossierFolder.DossierFolderRoles = this.getFolderRoles(folder.Roles);
            dossierFolder.Dossier = dossier;
            if (folder.Fascicle) {
                var toInsertFascicle = folder.Fascicle;
                toInsertFascicle.UniqueId = undefined;
                dossierFolder.Status = DossierFolderStatus.DoAction;
                var actionModel = {};
                actionModel.BuildType = BuildActionType.Build;
                actionModel.Model = JSON.stringify(toInsertFascicle);
                dossierFolder.JsonMetadata = JSON.stringify([actionModel]);
            }
            this._dossierFolderService.insertDossierFolder(dossierFolder, InsertActionType.BuildDossierFolders, function (data) {
                if (folder.Children && folder.Children.length > 0) {
                    $.when(_this.persistFoldersRecursive(folder.Children, dossier, data.UniqueId))
                        .done(function () {
                        if (folder.Children.length > 0) {
                            $.when(_this.persistFoldersRecursive(folder.Children, dossier, data.UniqueId))
                                .done(function () { return promise.resolve(); })
                                .fail(function () { return promise.reject(); });
                        }
                        else {
                            promise.resolve();
                        }
                    })
                        .fail(function () { return promise.reject(); });
                }
                else if (modelFolders.length > 0) {
                    $.when(_this.persistFoldersRecursive(modelFolders, dossier, parentId))
                        .done(function () { return promise.resolve(); })
                        .fail(function () { return promise.reject(); });
                }
                else {
                    promise.resolve();
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.dossierPageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
            return promise.promise();
        };
        DossierInserimento.prototype.getFolderRoles = function (roles) {
            var dossierFolderRoles = [];
            if (!roles) {
                return dossierFolderRoles;
            }
            for (var _i = 0, roles_1 = roles; _i < roles_1.length; _i++) {
                var idRole = roles_1[_i];
                var dossierFolderRole = {};
                var role = {};
                role.EntityShortId = idRole;
                dossierFolderRole.Role = role;
                dossierFolderRoles.push(dossierFolderRole);
            }
            return dossierFolderRoles;
        };
        /**
        * Esegue il fill dei controlli della pagina in  modello DossierModel in inserimento
        */
        DossierInserimento.prototype.fillModelFromPage = function (model) {
            var txtObject = $find(this.txtObjectId);
            model.Subject = txtObject.get_value();
            model.Note = this._txtNote.get_value();
            var selectedDate = new Date(this._rdpStartDate.get_selectedDate().getTime() - this._rdpStartDate.get_selectedDate().getTimezoneOffset() * 60000);
            model.StartDate = selectedDate;
            var containerModel = {};
            containerModel.EntityShortId = Number(this._rdlContainer.get_selectedItem().get_value());
            model.Container = containerModel;
            return model;
        };
        DossierInserimento.prototype.fillMetadataModel = function (model) {
            var uscDynamicMetadataClient = $("#".concat(this.uscDynamicMetadataId)).data();
            if (!jQuery.isEmptyObject(uscDynamicMetadataClient)) {
                model.JsonMetadata = uscDynamicMetadataClient.bindModelFormPage();
                if (!model.JsonMetadata) {
                    return false;
                }
                var currentRepository = {};
                currentRepository.UniqueId = this._selectedMetadataRepository;
                model.MetadataRepository = currentRepository;
            }
            return true;
        };
        return DossierInserimento;
    }(DossierBase));
    return DossierInserimento;
});
//# sourceMappingURL=DossierInserimento.js.map