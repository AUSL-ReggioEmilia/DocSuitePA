/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/ViewModels/Dossiers/DossierFolderInsertViewModel", "App/ViewModels/Dossiers/DossierInsertViewModel", "App/Mappers/Dossiers/DossierFolderSummaryModelMapper", "App/Models/Dossiers/DossierFolderStatus", "App/Helpers/GuidHelper", "App/Services/Commons/ContainerPropertyService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Models/Commons/ContainerPropertyType", "UserControl/uscMetadataRepositorySel", "App/Services/Commons/MetadataRepositoryService"], function (require, exports, DossierFolderInsertViewModel, DossierInsertViewModel, DossierSummaryFolderViewModelMapper, DossierFolderStatus, Guid, ContainerPropertyService, ServiceConfigurationHelper, ExceptionDTO, ContainerPropertyType, UscMetadataRepositorySel, MetadataRepositoryService) {
    var UscContainerDossierOptions = /** @class */ (function () {
        function UscContainerDossierOptions(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            this.chkMetadataReadonly_onCheckedChanged = function () {
                var isChecked = $("#".concat(_this.chkMetadataReadonlyId)).is(":checked");
                if (isChecked) {
                }
            };
            /**
             * Evento scatenato alla selezione di un metdata repository
             */
            this.uscMetadataSel_onSelectedIndexChanged = function (args, data) {
                if (data) {
                    var uscRepositorySel = $("#".concat(_this.uscMetadataSelId)).data();
                    $.when(uscRepositorySel.getSelectedMetadata())
                        .done(function (repository) {
                        try {
                            _this._rtvMetadata = $find(_this.rtvMetadataId);
                            var metadataRepository = repository;
                            var node = new Telerik.Web.UI.RadTreeNode();
                            node.set_value(data);
                            node.set_text(metadataRepository.Name);
                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/EnumDesigner_16x_green.png");
                            var isChecked = $("#".concat(_this.chkMetadataReadonlyId)).is(":checked");
                            if (isChecked) {
                                _this._rtvMetadata.get_nodes().getNode(0).get_nodes().clear();
                            }
                            _this._rtvMetadata.get_nodes().getNode(0).get_nodes().add(node);
                            $("#".concat(_this.chkMetadataReadonlyId)).prop("disabled", false);
                            if (_this._rtvMetadata.get_nodes().getNode(0).get_nodes().get_count() > 1) {
                                $("#".concat(_this.chkMetadataReadonlyId)).prop("disabled", true);
                            }
                        }
                        catch (e) {
                            console.error(e);
                            _this.showNotificationException("Errore nella gestione del metadata selezionato");
                        }
                    })
                        .fail(function () {
                        _this.showNotificationException("Errore nel caricamento del metadata selezionato");
                    });
                }
            };
            /**
             * Evento scatenato al click di un pulsante della toolbar relativa ai metadata repository selezionati
             */
            this.tlbMetadata_OnButtonClicked = function (source, args) {
                _this._rtvMetadata = $find(_this.rtvMetadataId);
                var item = args.get_item();
                if (item) {
                    var command = item.get_value();
                    switch (command) {
                        case "removeMetadata": {
                            try {
                                var selectedNode = _this._rtvMetadata.get_selectedNode();
                                if (selectedNode && selectedNode.get_value()) {
                                    _this._rtvMetadata.get_nodes().getNode(0).get_nodes().remove(selectedNode);
                                    $("#".concat(_this.chkMetadataReadonlyId)).prop("disabled", false);
                                    if (_this._rtvMetadata.get_nodes().getNode(0).get_nodes().get_count() > 1) {
                                        $("#".concat(_this.chkMetadataReadonlyId)).prop("disabled", true);
                                    }
                                }
                            }
                            catch (e) {
                                console.error(e);
                                _this.showNotificationException("Errore nella rimozione del metadata selezionato");
                            }
                            break;
                        }
                    }
                }
            };
            /**
             * Salva le informazioni del dossier per il contenitore selezionato
             */
            this.saveDossierFolders = function () {
                var instance = new DossierSummaryFolderViewModelMapper();
                var folders = _this.getStorageFolders();
                var rootFolders = folders.filter(function (x) {
                    for (var _i = 0, folders_1 = folders; _i < folders_1.length; _i++) {
                        var f = folders_1[_i];
                        if (f.UniqueId == x.ParentInsertId) {
                            return false;
                        }
                    }
                    return true;
                });
                var uscDossierFolders = $("#".concat(_this.uscDossierFoldersId)).data();
                try {
                    var action = (_this._currentContainerProperty) ? function (m, c, e) { return _this._containerPropertyService.updateContainerProperty(m, c, e); } : function (m, c, e) { return _this._containerPropertyService.insertContainerProperty(m, c, e); };
                    uscDossierFolders.showLoadingPanel();
                    var saveModel = new DossierInsertViewModel();
                    var toSaveFolders = [];
                    _this.fillFoldersRecursive(rootFolders, folders, toSaveFolders);
                    saveModel.Folders = toSaveFolders;
                    if (!_this._currentContainerProperty) {
                        _this._currentContainerProperty = {};
                        _this._currentContainerProperty.Container = {};
                        _this._currentContainerProperty.Container.EntityShortId = _this._currentContainerId;
                        _this._currentContainerProperty.Name = "DossierFoldersModel";
                        _this._currentContainerProperty.ContainerType = ContainerPropertyType.Json;
                    }
                    _this._rtvMetadata = $find(_this.rtvMetadataId);
                    var nodes = _this._rtvMetadata.get_nodes().getNode(0).get_nodes().toArray();
                    for (var _i = 0, nodes_1 = nodes; _i < nodes_1.length; _i++) {
                        var node = nodes_1[_i];
                        saveModel.MetadataRestrictions.push(node.get_value());
                    }
                    saveModel.SetMetadataReadOnly = $("#".concat(_this.chkMetadataReadonlyId)).is(":checked");
                    _this._currentContainerProperty.ValueString = JSON.stringify(saveModel);
                    action(_this._currentContainerProperty, function (data) {
                        uscDossierFolders.hideLoadingPanel();
                    }, function (exception) {
                        uscDossierFolders.hideLoadingPanel();
                        _this.showNotificationException(exception);
                    });
                }
                catch (e) {
                    uscDossierFolders.hideLoadingPanel();
                    console.error(e);
                    _this.showNotificationException("Errore nel salvataggio del modello di dossier per il contenitore selezionato.");
                }
                return false;
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        /**
         *------------------------- Methods -----------------------------
         */
        UscContainerDossierOptions.prototype.initialize = function () {
            this._rtvMetadata = $find(this.rtvMetadataId);
            this._tlbMetadata = $find(this.tlbMetadataId);
            var containerPropertyConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ContainerProperty");
            this._containerPropertyService = new ContainerPropertyService(containerPropertyConfiguration);
            var metadataRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository");
            this._metadataRepositoryService = new MetadataRepositoryService(metadataRepositoryConfiguration);
            this.bindLoaded();
        };
        UscContainerDossierOptions.prototype.bindLoaded = function () {
            $("#".concat(this.splPageContentId)).data(this);
        };
        /**
         * Carica l'alberatura di un dossier per uno specifico contenitore
         * @param idContainer
         */
        UscContainerDossierOptions.prototype.loadFolders = function (idContainer) {
            var _this = this;
            $("#".concat(this.uscMetadataSelId)).on(UscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, this.uscMetadataSel_onSelectedIndexChanged);
            $("#".concat(this.chkMetadataReadonlyId)).on("click", this.chkMetadataReadonly_onCheckedChanged);
            this._tlbMetadata = $find(this.tlbMetadataId);
            this._tlbMetadata.add_buttonClicked(this.tlbMetadata_OnButtonClicked);
            var uscRepositorySel = $("#".concat(this.uscMetadataSelId)).data();
            this._rtvMetadata = $find(this.rtvMetadataId);
            if (!jQuery.isEmptyObject(uscRepositorySel)) {
                uscRepositorySel.clearComboboxText();
                this._rtvMetadata.get_nodes().getNode(0).get_nodes().clear();
            }
            sessionStorage[UscContainerDossierOptions.DOSSIERFOLDERS_SESSIONNAME] = [];
            this._currentContainerProperty = undefined;
            this._currentContainerId = idContainer;
            $("#".concat(this.chkMetadataReadonlyId)).prop('checked', false);
            var uscDossierFolders = $("#".concat(this.uscDossierFoldersId)).data();
            if (!jQuery.isEmptyObject(uscDossierFolders)) {
                var tmpDossierId_1 = Guid.newGuid();
                uscDossierFolders.setRootNode('', tmpDossierId_1);
                uscDossierFolders.loadNodes([]);
                uscDossierFolders.setToolbarButtonsVisibility(true);
                uscDossierFolders.showLoadingPanel();
                this._containerPropertyService.getByContainer(idContainer, "DossierFoldersModel", function (data) {
                    if (!data || data.length == 0) {
                        uscDossierFolders.hideLoadingPanel();
                        return;
                    }
                    try {
                        var containerProperty = data[0];
                        _this._currentContainerProperty = containerProperty;
                        if (containerProperty.ValueString) {
                            var insertModel = JSON.parse(containerProperty.ValueString);
                            var folders_2 = [];
                            _this.loadFolderModelRecursive(insertModel.Folders, folders_2, tmpDossierId_1, tmpDossierId_1);
                            _this.loadMetadata(insertModel.MetadataRestrictions);
                            _this._rtvMetadata.get_nodes().getNode(0).expand();
                            $("#".concat(_this.chkMetadataReadonlyId)).prop('checked', insertModel.SetMetadataReadOnly);
                            sessionStorage[UscContainerDossierOptions.DOSSIERFOLDERS_SESSIONNAME] = JSON.stringify(folders_2);
                            var rootFolders = folders_2.filter(function (x) {
                                for (var _i = 0, folders_3 = folders_2; _i < folders_3.length; _i++) {
                                    var f = folders_3[_i];
                                    if (f.UniqueId == x.ParentInsertId) {
                                        return false;
                                    }
                                }
                                return true;
                            });
                            var instance = new DossierSummaryFolderViewModelMapper();
                            var toLoadFolders = [];
                            for (var _i = 0, rootFolders_1 = rootFolders; _i < rootFolders_1.length; _i++) {
                                var folder = rootFolders_1[_i];
                                toLoadFolders.push(instance.Map(folder));
                            }
                            uscDossierFolders.loadNodes(toLoadFolders);
                        }
                        uscDossierFolders.hideLoadingPanel();
                    }
                    catch (e) {
                        uscDossierFolders.hideLoadingPanel();
                        console.error(e);
                        _this.showNotificationException("Errore nella lettura dei valori per il contenitore selezionato");
                    }
                }, function (exception) {
                    uscDossierFolders.hideLoadingPanel();
                    _this.showNotificationException(exception);
                });
            }
        };
        UscContainerDossierOptions.prototype.loadMetadata = function (metadatas) {
            var _this = this;
            if (!metadatas || metadatas.length == 0) {
                return;
            }
            var metadata = metadatas.shift();
            this._metadataRepositoryService.getFullModelById(metadata.toString(), function (data) {
                if (!data) {
                    return;
                }
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(data.Name);
                node.set_value(metadata);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/EnumDesigner_16x_green.png");
                _this._rtvMetadata = $find(_this.rtvMetadataId);
                _this._rtvMetadata.get_nodes().getNode(0).get_nodes().add(node);
                $("#".concat(_this.chkMetadataReadonlyId)).prop("disabled", false);
                if (_this._rtvMetadata.get_nodes().getNode(0).get_nodes().get_count() > 1) {
                    $("#".concat(_this.chkMetadataReadonlyId)).prop("disabled", true);
                }
                _this.loadMetadata(metadatas);
            }, function (exception) {
                _this.showNotificationException(exception);
            });
        };
        UscContainerDossierOptions.prototype.getStorageFolders = function () {
            var jsFolders = sessionStorage[UscContainerDossierOptions.DOSSIERFOLDERS_SESSIONNAME];
            if (!jsFolders) {
                return [];
            }
            return JSON.parse(jsFolders);
        };
        UscContainerDossierOptions.prototype.loadFolderModelRecursive = function (models, folders, dossierId, parentId) {
            if (!models || models.length == 0) {
                return;
            }
            for (var _i = 0, models_1 = models; _i < models_1.length; _i++) {
                var model = models_1[_i];
                var folder = {};
                folder.Dossier = {};
                folder.Dossier.UniqueId = dossierId;
                folder.Name = model.Name;
                folder.ParentInsertId = parentId;
                folder.Status = DossierFolderStatus.InProgress;
                folder.UniqueId = Guid.newGuid();
                if (model.Roles && model.Roles.length > 0) {
                    folder.DossierFolderRoles = [];
                    for (var _a = 0, _b = model.Roles; _a < _b.length; _a++) {
                        var role = _b[_a];
                        var dossierRole = {};
                        dossierRole.Role = {};
                        dossierRole.Role.EntityShortId = role;
                        folder.DossierFolderRoles.push(dossierRole);
                    }
                }
                if (model.Fascicle) {
                    model.Fascicle.UniqueId = Guid.newGuid();
                    folder.Fascicle = model.Fascicle;
                    folder.Status = DossierFolderStatus.Fascicle;
                }
                if (model.Children && model.Children.length > 0) {
                    folder.Status = DossierFolderStatus.Folder;
                    this.loadFolderModelRecursive(model.Children, folders, dossierId, folder.UniqueId);
                }
                folders.push(folder);
            }
        };
        UscContainerDossierOptions.prototype.fillFoldersRecursive = function (toFillFolders, folders, models) {
            if (!folders || folders.length == 0) {
                return;
            }
            var _loop_1 = function (folder) {
                var model = this_1.createDossierInsertModel(folder);
                var folderChildren = folders.filter(function (x) { return x.ParentInsertId == folder.UniqueId; });
                var hasChildren = folderChildren && folderChildren.length > 0;
                if (hasChildren) {
                    model.Children = [];
                    this_1.fillFoldersRecursive(folderChildren, folders, model.Children);
                }
                models.push(model);
            };
            var this_1 = this;
            for (var _i = 0, toFillFolders_1 = toFillFolders; _i < toFillFolders_1.length; _i++) {
                var folder = toFillFolders_1[_i];
                _loop_1(folder);
            }
        };
        UscContainerDossierOptions.prototype.createDossierInsertModel = function (folder) {
            var model = new DossierFolderInsertViewModel();
            model.Name = folder.Name;
            model.Roles = (folder.DossierFolderRoles) ? folder.DossierFolderRoles.map(function (x) { return x.Role.EntityShortId; }) : [];
            model.Fascicle = folder.Fascicle;
            return model;
        };
        UscContainerDossierOptions.prototype.showNotificationException = function (exception) {
            var uscNotification = $("#".concat(this.uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception instanceof ExceptionDTO) {
                    uscNotification.showNotification(exception);
                }
                else {
                    uscNotification.showNotificationMessage(exception);
                }
            }
        };
        UscContainerDossierOptions.DOSSIERFOLDERS_SESSIONNAME = "dossierfoldersessionname";
        return UscContainerDossierOptions;
    }());
    return UscContainerDossierOptions;
});
//# sourceMappingURL=uscContainerDossierOptions.js.map