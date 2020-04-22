var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
define(["require", "exports", "App/Helpers/EnumHelper", "App/Services/Processes/ProcessService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Fascicles/FascicleType", "App/Services/Dossiers/DossierFolderService", "App/Services/Processes/ProcessFascicleWorkflowRepositoryService", "App/Services/Workflows/WorkflowRepositoryService", "App/Services/Commons/MetadataRepositoryService", "App/Models/Fascicles/FascicleModel", "App/Models/Fascicles/VisibilityType", "UserControl/uscFascicleFolders", "App/Models/Fascicles/FascicleFolderStatus", "App/Helpers/GuidHelper", "App/Models/Fascicles/FascicleFolderTypology", "App/DTOs/ExceptionDTO", "App/Models/Commons/MetadataRepositoryModel", "App/Models/Processes/ProcessNodeType", "App/Models/Fascicles/AuthorizationRoleType", "App/Models/Fascicles/FascicleRoleModel", "App/Services/Processes/ProcessFascicleTemplateService"], function (require, exports, EnumHelper, ProcessService, ServiceConfigurationHelper, FascicleType, DossierFolderService, ProcessFascicleWorkflowRepositoryService, WorkflowRepositoryService, MetadataRepositoryService, FascicleModel, VisibilityType, UscFascicleFolders, FascicleFolderStatus, Guid, FascicleFolderTypology, ExceptionDTO, MetadataRepositoryModel, ProcessNodeType, AuthorizationRoleType, FascicleRoleModel, ProcessFascicleTemplateService) {
    var uscProcessDetails = /** @class */ (function () {
        function uscProcessDetails(serviceConfigurations) {
            var _this = this;
            this.deleteRolePromise = function (roleIdToDelete, senderId) {
                var promise = $.Deferred();
                if (!roleIdToDelete)
                    return promise.promise();
                switch (uscProcessDetails.selectedEntityType) {
                    case ProcessNodeType.Process: {
                        _this._processService.getById(uscProcessDetails.selectedProcessId, function (data) {
                            var process = data;
                            process.Roles = process.Roles
                                .filter(function (role) { return role.IdRole !== roleIdToDelete || role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1; });
                            _this._processService.update(process, function (data) {
                                promise.resolve(data);
                            }, function (error) {
                                _this._ajaxLoadingPanel.hide("ItemDetailTable");
                                _this.showNotificationException(error);
                            });
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide("ItemDetailTable");
                            _this.showNotificationException(error);
                        });
                        break;
                    }
                    case ProcessNodeType.DossierFolder: {
                        _this._dossierFolderService.getDossierFolderById(uscProcessDetails.selectedDossierFolderId, function (data) {
                            var dossierFolder = data[0];
                            dossierFolder.DossierFolderRoles = dossierFolder.DossierFolderRoles
                                .filter(function (role) { return role.Role.IdRole !== roleIdToDelete || role.Role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1; });
                            _this._dossierFolderService.updateDossierFolder(dossierFolder, null, function (data) {
                                promise.resolve(data);
                            }, function (error) {
                                _this._ajaxLoadingPanel.hide("ItemDetailTable");
                                _this.showNotificationException(error);
                            });
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide("workflowDetails");
                            _this.showNotificationException(error);
                        });
                        break;
                    }
                    case ProcessNodeType.ProcessFascicleTemplate: {
                        switch (senderId) {
                            case _this.uscResponsibleRolesId: {
                                uscProcessDetails.responsibleRoles = uscProcessDetails.responsibleRoles
                                    .filter(function (role) { return role.IdRole !== roleIdToDelete || role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1; });
                                promise.resolve(uscProcessDetails.responsibleRoles);
                                break;
                            }
                            case _this.uscAuthorizedRolesId: {
                                uscProcessDetails.authorizedRoles = uscProcessDetails.authorizedRoles
                                    .filter(function (role) { return role.IdRole !== roleIdToDelete || role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1; });
                                promise.resolve(uscProcessDetails.authorizedRoles);
                                break;
                            }
                        }
                        break;
                    }
                }
                return promise.promise();
            };
            this.updateRolesPromise = function (newAddedRoles, senderId) {
                var promise = $.Deferred();
                if (!newAddedRoles.length)
                    return promise.promise();
                switch (uscProcessDetails.selectedEntityType) {
                    case ProcessNodeType.Process: {
                        _this._processService.getById(uscProcessDetails.selectedProcessId, function (data) {
                            var process = data;
                            process.Roles = __spreadArrays(process.Roles, newAddedRoles);
                            _this._processService.update(process, function (data) {
                                promise.resolve(data);
                            }, function (error) {
                                _this._ajaxLoadingPanel.hide("ItemDetailTable");
                                _this.showNotificationException(error);
                            });
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide("ItemDetailTable");
                            _this.showNotificationException(error);
                        });
                        break;
                    }
                    case ProcessNodeType.DossierFolder: {
                        _this._dossierFolderService.getDossierFolderById(uscProcessDetails.selectedDossierFolderId, function (data) {
                            var dossierFolder = data[0];
                            for (var _i = 0, newAddedRoles_1 = newAddedRoles; _i < newAddedRoles_1.length; _i++) {
                                var newRole = newAddedRoles_1[_i];
                                if (dossierFolder.DossierFolderRoles.map(function (x) { return x.Role; }).indexOf(newRole) === -1) {
                                    var dossierFolderRole = {};
                                    dossierFolderRole.Role = newRole;
                                    dossierFolderRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                                    dossierFolderRole.IsMaster = true;
                                    dossierFolder.DossierFolderRoles.push(dossierFolderRole);
                                }
                            }
                            _this._dossierFolderService.updateDossierFolder(dossierFolder, null, function (data) {
                                promise.resolve(data);
                            }, function (error) {
                                _this._ajaxLoadingPanel.hide("ItemDetailTable");
                                _this.showNotificationException(error);
                            });
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide("workflowDetails");
                            _this.showNotificationException(error);
                        });
                        break;
                    }
                    case ProcessNodeType.ProcessFascicleTemplate: {
                        switch (senderId) {
                            case _this.uscResponsibleRolesId: {
                                uscProcessDetails.responsibleRoles = __spreadArrays(uscProcessDetails.responsibleRoles, newAddedRoles);
                                promise.resolve(uscProcessDetails.responsibleRoles);
                                break;
                            }
                            case _this.uscAuthorizedRolesId: {
                                uscProcessDetails.authorizedRoles = __spreadArrays(uscProcessDetails.authorizedRoles, newAddedRoles);
                                promise.resolve(uscProcessDetails.authorizedRoles);
                                break;
                            }
                        }
                        break;
                    }
                }
                return promise.promise();
            };
            this.deleteContactPromise = function (contactIdToDelete, senderId) {
                var promise = $.Deferred();
                if (!contactIdToDelete)
                    return promise.promise();
                uscProcessDetails.contacts = uscProcessDetails.contacts
                    .filter(function (contact) { return contact.EntityId !== contactIdToDelete || contact.FullIncrementalPath.indexOf(contactIdToDelete.toString()) === -1; });
                promise.resolve(uscProcessDetails.contacts);
                return promise.promise();
            };
            this.updateContactPromise = function (newAddedContact, senderId) {
                var promise = $.Deferred();
                if (!newAddedContact)
                    return promise.promise();
                uscProcessDetails.contacts.push(newAddedContact);
                promise.resolve(uscProcessDetails.contacts);
                return promise.promise();
            };
            this.toolbarWorkflowRepository_buttonClick = function (sender, args) {
                var toolbarButton = args.get_item();
                switch (toolbarButton.get_commandName()) {
                    case "add": {
                        if (_this._rcbWorkflowRepository.get_selectedItem().get_value() === "") {
                            alert("Selezionare flusso di lavoro");
                            return;
                        }
                        _this._ajaxLoadingPanel.show("workflowDetails");
                        var processFascicleWorkflowRepository = {};
                        processFascicleWorkflowRepository.Process = {};
                        processFascicleWorkflowRepository.Process.UniqueId = uscProcessDetails.selectedProcessId;
                        processFascicleWorkflowRepository.DossierFolder = {};
                        processFascicleWorkflowRepository.DossierFolder.UniqueId = uscProcessDetails.selectedDossierFolderId;
                        processFascicleWorkflowRepository.WorkflowRepository = {};
                        processFascicleWorkflowRepository.WorkflowRepository.UniqueId = _this._rcbWorkflowRepository.get_selectedItem().get_value();
                        var selectedWorkflowRepository_1 = {};
                        for (var _i = 0, _a = _this.workflowRepositories; _i < _a.length; _i++) {
                            var workflowRepository = _a[_i];
                            if (workflowRepository.UniqueId === _this._rcbWorkflowRepository.get_selectedItem().get_value()) {
                                selectedWorkflowRepository_1 = workflowRepository;
                                break;
                            }
                        }
                        _this._processFascicleWorkflowRepositoryService.insert(processFascicleWorkflowRepository, function (data) {
                            var node = new Telerik.Web.UI.RadTreeNode();
                            node.set_text(selectedWorkflowRepository_1.Name);
                            node.set_value(data.UniqueId);
                            _this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().add(node);
                            _this._rtvWorkflowRepository.get_nodes().getNode(0).expand();
                            _this._ajaxLoadingPanel.hide("workflowDetails");
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide("workflowDetails");
                            _this.showNotificationException(error);
                        });
                        break;
                    }
                    case "delete": {
                        if (_this._rtvWorkflowRepository.get_selectedNode() === null || _this._rtvWorkflowRepository.get_selectedNode().get_level() === 0) {
                            alert("Selezionare flusso di lavoro");
                            return;
                        }
                        _this._ajaxLoadingPanel.show("workflowDetails");
                        var processFascicleWorkflowRepository = {};
                        processFascicleWorkflowRepository.UniqueId = _this._rtvWorkflowRepository.get_selectedNode().get_value();
                        _this._processFascicleWorkflowRepositoryService.delete(processFascicleWorkflowRepository, function (data) {
                            _this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().remove(_this._rtvWorkflowRepository.get_selectedNode());
                            _this._ajaxLoadingPanel.hide("workflowDetails");
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide("workflowDetails");
                            _this.showNotificationException(error);
                        });
                        break;
                    }
                }
            };
            this.rbAddFascicle_onClick = function (sender, args) {
                _this._ajaxLoadingPanel.show("fascicleDetails");
                var fascicleModel = new FascicleModel();
                fascicleModel.FascicleObject = _this._rtbFascicleSubject.get_textBoxValue();
                fascicleModel.VisibilityType = _this._rbFascicleVisibilityType.get_checked() ? VisibilityType.Accessible : VisibilityType.Confidential;
                fascicleModel.FascicleRoles = [];
                for (var _i = 0, _a = uscProcessDetails.responsibleRoles; _i < _a.length; _i++) {
                    var responsibleRole = _a[_i];
                    var fascicleRole = new FascicleRoleModel();
                    fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
                    fascicleRole.IsMaster = true;
                    fascicleRole.Role = responsibleRole;
                    fascicleModel.FascicleRoles.push(fascicleRole);
                }
                for (var _b = 0, _c = uscProcessDetails.authorizedRoles; _b < _c.length; _b++) {
                    var authorizedRole = _c[_b];
                    var fascicleRole = new FascicleRoleModel();
                    fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                    fascicleRole.IsMaster = false;
                    fascicleRole.Role = authorizedRole;
                    fascicleModel.FascicleRoles.push(fascicleRole);
                }
                _this.fascicleFolders = [];
                _this.getFascicleFolderListFromTree(_this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0));
                fascicleModel.FascicleFolders = _this.fascicleFolders;
                fascicleModel.Contacts = uscProcessDetails.contacts;
                fascicleModel.MetadataRepository = new MetadataRepositoryModel();
                if (_this._rcbMetadataRepository.get_selectedItem()) {
                    fascicleModel.MetadataRepository.Name = _this._rcbMetadataRepository.get_selectedItem().get_text();
                    fascicleModel.MetadataRepository.UniqueId = _this._rcbMetadataRepository.get_selectedItem().get_value();
                }
                var processFascicleTemplate = {};
                processFascicleTemplate.DossierFolder = {};
                processFascicleTemplate.Process = {};
                processFascicleTemplate.UniqueId = uscProcessDetails.selectedProcessFascicleTemplateId;
                processFascicleTemplate.Process.UniqueId = uscProcessDetails.selectedProcessId;
                processFascicleTemplate.JsonModel = JSON.stringify(fascicleModel);
                sessionStorage.setItem("ProcessFascicleTemplate", JSON.stringify(processFascicleTemplate));
                _this._processFascicleTemplateService.update(processFascicleTemplate, function (data) {
                    alert("Modificato con successo");
                    _this._ajaxLoadingPanel.hide("fascicleDetails");
                }, function (error) {
                    _this._ajaxLoadingPanel.hide("fascicleDetails");
                    _this.showNotificationException(error);
                });
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        uscProcessDetails.prototype.initialize = function () {
            this.initializeServices();
            this.initializeControls();
            this.initializeUserControls();
            $("#" + this.pnlDetailsId).hide();
            this._ajaxLoadingPanel.show(this.rcbWorkflowRepositoryId);
            this._ajaxLoadingPanel.show(this.rcbMetadataRepositoryId);
            this.loadWorkflowRepositories();
            this.loadMetadataRepositories();
            this.bindLoaded();
        };
        uscProcessDetails.prototype.initializeServices = function () {
            var processConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Process");
            this._processService = new ProcessService(processConfiguration);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DossierFolder");
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            var processFascicleWorkflowRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ProcessFascicleWorkflowRepository");
            this._processFascicleWorkflowRepositoryService = new ProcessFascicleWorkflowRepositoryService(processFascicleWorkflowRepositoryConfiguration);
            var workflowRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowRepository");
            this._workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);
            var metadataRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository");
            this._metadataRepositoryService = new MetadataRepositoryService(metadataRepositoryConfiguration);
            var processFascicleTemplateConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ProcessFascicleTemplate");
            this._processFascicleTemplateService = new ProcessFascicleTemplateService(processFascicleTemplateConfiguration);
        };
        uscProcessDetails.prototype.initializeControls = function () {
            this._lblProcessName = document.getElementById(this.lblNameId);
            this._lblClasificationName = document.getElementById(this.lblClasificationNameId);
            this._lblFascicleType = document.getElementById(this.lblFascicleTypeId);
            this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId);
            this._rcbWorkflowRepository = $find(this.rcbWorkflowRepositoryId);
            this._toolbarWorkflowRepository = $find(this.toolbarWorkflowRepositoryId);
            this._toolbarWorkflowRepository.add_buttonClicked(this.toolbarWorkflowRepository_buttonClick);
            this._rtvWorkflowRepository = $find(this.rtvWorkflowRepositoryId);
            this._rcbMetadataRepository = $find(this.rcbMetadataRepositoryId);
            this._rbAddFascicle = $find(this.rbAddFascicleId);
            this._rbAddFascicle.add_clicked(this.rbAddFascicle_onClick);
            this._rtbFascicleSubject = $find(this.rtbFascicleSubjectId);
            this._rbFascicleVisibilityType = $find(this.rbFascicleVisibilityTypeId);
        };
        uscProcessDetails.prototype.initializeUserControls = function () {
            $("#".concat(this.uscFascicleFoldersId)).bind(UscFascicleFolders.LOADED_EVENT, function (args) {
            });
            this._uscFascicleFolders = $("#" + this.uscFascicleFoldersId).data();
            if (!jQuery.isEmptyObject(this._uscFascicleFolders)) {
                this._uscFascicleFolders.hideLoadingPanel();
                this._uscFascicleFolders.setRootNode("");
                this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0).get_nodes().clear();
                var fascicleFolder = {};
                fascicleFolder.Status = FascicleFolderStatus.Active;
                fascicleFolder.Name = "Fascicolo";
                var model = {};
                model.ActionName = "ManageParent";
                model.Value = [];
                fascicleFolder.UniqueId = Guid.newGuid();
                fascicleFolder.Typology = FascicleFolderTypology.Fascicle;
                model.Value.push(JSON.stringify(fascicleFolder));
                this._uscFascicleFolders.addNewFolder(model);
                this._uscFascicleFolders.setManageFascicleFolderVisibility(true);
            }
            this._uscContactRest = $("#" + this.uscContactRestId).data();
            this._uscRoleRest = $("#" + this.uscRoleRestId).data();
            this._uscResponsibleRoles = $("#" + this.uscResponsibleRolesId).data();
            this._uscAuthorizedRoles = $("#" + this.uscAuthorizedRolesId).data();
            this.registerUscRoleRestEventHandlers();
            uscProcessDetails.contacts = [];
            this._uscContactRest.renderContactsTree([]);
            this.registerUscContattiRestEventHandlers();
        };
        uscProcessDetails.prototype.registerUscRoleRestEventHandlers = function () {
            var uscRoleRestEventsDictionary = this._uscRoleRest.uscRoleRestEvents;
            this._uscRoleRest.registerEventHandler(uscRoleRestEventsDictionary.RoleDeleted, this.deleteRolePromise, this.uscRoleRestId);
            this._uscRoleRest.registerEventHandler(uscRoleRestEventsDictionary.NewRolesAdded, this.updateRolesPromise, this.uscRoleRestId);
            this._uscResponsibleRoles.registerEventHandler(uscRoleRestEventsDictionary.RoleDeleted, this.deleteRolePromise, this.uscResponsibleRolesId);
            this._uscResponsibleRoles.registerEventHandler(uscRoleRestEventsDictionary.NewRolesAdded, this.updateRolesPromise, this.uscResponsibleRolesId);
            this._uscAuthorizedRoles.registerEventHandler(uscRoleRestEventsDictionary.RoleDeleted, this.deleteRolePromise, this.uscAuthorizedRolesId);
            this._uscAuthorizedRoles.registerEventHandler(uscRoleRestEventsDictionary.NewRolesAdded, this.updateRolesPromise, this.uscAuthorizedRolesId);
        };
        uscProcessDetails.prototype.registerUscContattiRestEventHandlers = function () {
            var uscContattiSelRestEvents = this._uscContactRest.uscContattiSelRestEvents;
            this._uscContactRest.registerEventHandler(uscContattiSelRestEvents.ContactDeleted, this.deleteContactPromise);
            this._uscContactRest.registerEventHandler(uscContattiSelRestEvents.NewContactsAdded, this.updateContactPromise);
        };
        uscProcessDetails.prototype.bindLoaded = function () {
            $("#" + this.pnlDetailsId).data(this);
        };
        uscProcessDetails.prototype.loadWorkflowRepositories = function () {
            var _this = this;
            this._workflowRepositoryService.getWorkflowRepositories(function (data) {
                _this.workflowRepositories = data;
                _this._rcbWorkflowRepository.get_items().clear();
                var item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text("");
                item.set_value("");
                _this._rcbWorkflowRepository.get_items().add(item);
                for (var _i = 0, _a = _this.workflowRepositories; _i < _a.length; _i++) {
                    var workflowRepository = _a[_i];
                    var item_1 = new Telerik.Web.UI.RadComboBoxItem();
                    item_1.set_text(workflowRepository.Name);
                    item_1.set_value(workflowRepository.UniqueId);
                    _this._rcbWorkflowRepository.get_items().add(item_1);
                }
                _this._ajaxLoadingPanel.hide(_this.rcbWorkflowRepositoryId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rcbWorkflowRepositoryId);
                _this.showNotificationException(error);
            });
        };
        uscProcessDetails.prototype.loadMetadataRepositories = function () {
            var _this = this;
            this._metadataRepositoryService.findMetadataRepositories("", function (data) {
                _this.metadataRepositories = data;
                _this._rcbMetadataRepository.get_items().clear();
                var item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text("");
                item.set_value("");
                _this._rcbMetadataRepository.get_items().add(item);
                for (var _i = 0, _a = _this.metadataRepositories; _i < _a.length; _i++) {
                    var metadataRepository = _a[_i];
                    var item_2 = new Telerik.Web.UI.RadComboBoxItem();
                    item_2.set_text(metadataRepository.Name);
                    item_2.set_value(metadataRepository.UniqueId);
                    _this._rcbMetadataRepository.get_items().add(item_2);
                }
                _this._ajaxLoadingPanel.hide(_this.rcbMetadataRepositoryId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rcbMetadataRepositoryId);
                _this.showNotificationException(error);
            });
        };
        uscProcessDetails.prototype.setProcessDetails = function () {
            var _this = this;
            this._rcbWorkflowRepository.get_items().getItem(0).select();
            this._rcbMetadataRepository.get_items().getItem(0).select();
            this._uscResponsibleRoles.renderRolesTree([]);
            this._uscAuthorizedRoles.renderRolesTree([]);
            this._processService.getById(uscProcessDetails.selectedProcessId, function (data) {
                var process = data;
                _this._lblProcessName.innerText = process.Name;
                _this._lblClasificationName.innerText = process.Category.Name;
                _this._lblFascicleType.innerText = FascicleType[FascicleType[process.FascicleType]];
                if (uscProcessDetails.selectedEntityType === ProcessNodeType.Process) {
                    _this._uscRoleRest.renderRolesTree(process.Roles);
                }
                else if (uscProcessDetails.selectedEntityType === ProcessNodeType.ProcessFascicleTemplate) {
                    uscProcessDetails.responsibleRoles = [];
                    uscProcessDetails.authorizedRoles = [];
                }
                _this._ajaxLoadingPanel.hide("ItemDetailTable");
            }, function (error) {
                _this._ajaxLoadingPanel.hide("ItemDetailTable");
                _this.showNotificationException(error);
            });
        };
        uscProcessDetails.prototype.clearProcessDetails = function () {
            this._lblProcessName.innerText = "";
            this._lblClasificationName.innerText = "";
            this._lblFascicleType.innerText = "";
        };
        uscProcessDetails.prototype.setDossierFolderWorkflowRepositories = function () {
            var _this = this;
            this._processFascicleWorkflowRepositoryService.getByDossierFolderId(uscProcessDetails.selectedDossierFolderId, function (data) {
                uscProcessDetails.processFascicleWorkflowRepositories = data;
                _this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().clear();
                for (var _i = 0, _a = uscProcessDetails.processFascicleWorkflowRepositories; _i < _a.length; _i++) {
                    var processFascicleWorkflowRepository = _a[_i];
                    var node = new Telerik.Web.UI.RadTreeNode();
                    node.set_text(processFascicleWorkflowRepository.WorkflowRepository.Name);
                    node.set_value(processFascicleWorkflowRepository.UniqueId);
                    _this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().add(node);
                }
                _this._rtvWorkflowRepository.get_nodes().getNode(0).expand();
                _this._ajaxLoadingPanel.hide("workflowDetails");
            }, function (error) {
                _this._ajaxLoadingPanel.hide("workflowDetails");
                _this.showNotificationException(error);
            });
        };
        uscProcessDetails.prototype.setDossierFolderRoles = function () {
            var _this = this;
            this._dossierFolderService.getDossierFolderById(uscProcessDetails.selectedDossierFolderId, function (data) {
                var dossierFolder = data[0];
                if (dossierFolder.DossierFolderRoles && dossierFolder.DossierFolderRoles.length > 0) {
                    _this._uscRoleRest.renderRolesTree(dossierFolder.DossierFolderRoles.map(function (x) { return x.Role; }));
                }
            }, function (error) {
                _this._ajaxLoadingPanel.hide("workflowDetails");
                _this.showNotificationException(error);
            });
        };
        uscProcessDetails.prototype.setFascicle = function () {
            var _this = this;
            this._processFascicleTemplateService.getById(uscProcessDetails.selectedProcessFascicleTemplateId, function (data) {
                _this.populateDetails(data);
            }, function (error) {
                _this.showNotificationException(error);
            });
        };
        uscProcessDetails.prototype.populateDetails = function (data) {
            var processFascicleTemplate = data;
            var fascicle = JSON.parse(processFascicleTemplate.JsonModel);
            this._rtbFascicleSubject.set_value(fascicle.FascicleObject);
            this._rbFascicleVisibilityType.set_checked(fascicle.VisibilityType === VisibilityType.Accessible);
            uscProcessDetails.responsibleRoles = fascicle.FascicleRoles.filter(function (x) { return x.IsMaster === true; }).map(function (x) { return x.Role; });
            this._uscResponsibleRoles.renderRolesTree(uscProcessDetails.responsibleRoles);
            uscProcessDetails.authorizedRoles = fascicle.FascicleRoles.filter(function (x) { return x.IsMaster === false; }).map(function (x) { return x.Role; });
            this._uscAuthorizedRoles.renderRolesTree(uscProcessDetails.authorizedRoles);
            this.populateFascicleFoldersTree(fascicle.FascicleFolders);
            uscProcessDetails.contacts = fascicle.Contacts;
            this._uscContactRest.renderContactsTree(uscProcessDetails.contacts);
            var rcbItem = this._rcbMetadataRepository.findItemByValue(fascicle.MetadataRepository.UniqueId);
            if (rcbItem) {
                rcbItem.select();
            }
        };
        uscProcessDetails.prototype.getFascicleFolderListFromTree = function (fascicleFoldersNode) {
            if (fascicleFoldersNode.get_level() === 0) {
                var fascicleFolder = {};
                fascicleFolder.Name = fascicleFoldersNode.get_text();
                fascicleFolder.UniqueId = fascicleFoldersNode.get_value();
                fascicleFolder.Typology = FascicleFolderTypology.Fascicle;
                fascicleFolder.Status = FascicleFolderStatus.Active;
                this.fascicleFolders.push(fascicleFolder);
                fascicleFoldersNode = fascicleFoldersNode.get_nodes().getNode(0);
            }
            for (var index = 0; index < fascicleFoldersNode.get_nodes().get_count(); index++) {
                var child = fascicleFoldersNode.get_nodes().getNode(index);
                var fascicleFolder = {};
                fascicleFolder.Name = child.get_text();
                fascicleFolder.UniqueId = child.get_value();
                fascicleFolder.Typology = child.get_attributes().getAttribute("Typology");
                fascicleFolder.Status = FascicleFolderStatus.Active;
                fascicleFolder.ParentInsertId = fascicleFoldersNode.get_level() === 1
                    ? fascicleFoldersNode.get_parent().get_value()
                    : fascicleFoldersNode.get_value();
                this.fascicleFolders.push(fascicleFolder);
                if (child.get_attributes().getAttribute("hasChildren")) {
                    this.getFascicleFolderListFromTree(child);
                }
            }
        };
        uscProcessDetails.prototype.alreadySavedInTree = function (nodeValue, radTreeView) {
            var alreadySavedInTree = false;
            if (radTreeView.get_nodes().get_count() !== 0) {
                var allNodes = radTreeView.get_nodes().getNode(0).get_allNodes();
                for (var i = 0; i < allNodes.length; i++) {
                    var node = allNodes[i];
                    if (node.get_value() === nodeValue) {
                        alreadySavedInTree = true;
                        break;
                    }
                }
            }
            return alreadySavedInTree;
        };
        uscProcessDetails.prototype.addNodesToRadTreeView = function (nodeValue, nodeText, text, nodeImageUrl, radTreeView) {
            var rtvNode;
            if (radTreeView.get_nodes().get_count() === 0) {
                rtvNode = new Telerik.Web.UI.RadTreeNode();
                rtvNode.set_text(text);
                radTreeView.get_nodes().add(rtvNode);
            }
            rtvNode = new Telerik.Web.UI.RadTreeNode();
            rtvNode.set_text(nodeText);
            rtvNode.set_value(nodeValue);
            rtvNode.set_imageUrl(nodeImageUrl);
            radTreeView.get_nodes().getNode(0).get_nodes().add(rtvNode);
            radTreeView.get_nodes().getNode(0).expand();
        };
        uscProcessDetails.prototype.populateFascicleFoldersTree = function (fascicleFolders) {
            this._uscFascicleFolders.setRootNode("");
            this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0).get_nodes().clear();
            var fascicleFolder = {};
            fascicleFolder.Status = FascicleFolderStatus.Active;
            fascicleFolder.Name = "Fascicolo";
            var model = {};
            model.ActionName = "ManageParent";
            model.Value = [];
            fascicleFolder.UniqueId = Guid.newGuid();
            fascicleFolder.Typology = FascicleFolderTypology.Fascicle;
            model.Value.push(JSON.stringify(fascicleFolder));
            this._uscFascicleFolders.addNewFolder(model);
            for (var index = 1; index < fascicleFolders.length; index++) {
                var fascicleFolder_1 = fascicleFolders[index];
                var model_1 = {};
                model_1.ActionName = "ManageParent";
                model_1.Value = [];
                model_1.Value.push(JSON.stringify(fascicleFolder_1));
                var parentNode = this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0).get_nodes().getNode(0);
                parentNode.expand();
                this.addFascicleFolderNodeRecursive(fascicleFolder_1, parentNode);
            }
        };
        uscProcessDetails.prototype.addFascicleFolderNodeRecursive = function (fasciclefolder, node) {
            if (fasciclefolder.ParentInsertId === "" || node.get_value() === fasciclefolder.ParentInsertId) {
                var child = new Telerik.Web.UI.RadTreeNode();
                child.set_text(fasciclefolder.Name);
                child.set_value(fasciclefolder.UniqueId);
                child.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_closed.png");
                child.expand();
                node.get_nodes().add(child);
                return;
            }
            for (var index = 0; index < node.get_nodes().get_count(); index++) {
                var parent_1 = node.get_nodes().getNode(index);
                this.addFascicleFolderNodeRecursive(fasciclefolder, parent_1);
            }
        };
        uscProcessDetails.prototype.clearFascicleInputs = function () {
            this._rtbFascicleSubject.clear();
            this._rbFascicleVisibilityType.set_checked(false);
            this._uscResponsibleRoles.renderRolesTree([]);
            this._uscAuthorizedRoles.renderRolesTree([]);
            this.initializeUserControls();
            this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0).get_nodes().getNode(0).get_nodes().clear();
            this._uscContactRest.renderContactsTree([]);
            this._rcbMetadataRepository.set_selectedItem(this._rcbMetadataRepository.get_items().getItem(0));
        };
        uscProcessDetails.prototype.showNotificationException = function (exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#" + this.uscNotificationId).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(customMessage);
            }
        };
        uscProcessDetails.prototype.showNotificationMessage = function (customMessage) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        return uscProcessDetails;
    }());
    return uscProcessDetails;
});
//# sourceMappingURL=uscProcessDetails.js.map