var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
define(["require", "exports", "App/Helpers/EnumHelper", "App/Services/Processes/ProcessService", "App/Helpers/ServiceConfigurationHelper", "App/Services/Dossiers/DossierFolderService", "App/Services/Processes/ProcessFascicleWorkflowRepositoryService", "App/Services/Workflows/WorkflowRepositoryService", "App/Services/Commons/MetadataRepositoryService", "App/Models/Fascicles/FascicleModel", "App/Models/Fascicles/VisibilityType", "UserControl/uscFascicleFolders", "App/Models/Fascicles/FascicleFolderStatus", "App/Helpers/GuidHelper", "App/Models/Fascicles/FascicleFolderTypology", "App/DTOs/ExceptionDTO", "App/Models/Commons/MetadataRepositoryModel", "./uscRoleRest", "App/Models/Processes/ProcessNodeType", "App/Models/Commons/AuthorizationRoleType", "App/Models/Fascicles/FascicleRoleModel", "App/Services/Processes/ProcessFascicleTemplateService", "App/Helpers/ExternalSourceActionEnum", "App/Models/Fascicles/FascicleType", "App/Services/Commons/CategoryService", "App/Helpers/PageClassHelper", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, EnumHelper, ProcessService, ServiceConfigurationHelper, DossierFolderService, ProcessFascicleWorkflowRepositoryService, WorkflowRepositoryService, MetadataRepositoryService, FascicleModel, VisibilityType, UscFascicleFolders, FascicleFolderStatus, Guid, FascicleFolderTypology, ExceptionDTO, MetadataRepositoryModel, uscRoleRest, ProcessNodeType, AuthorizationRoleType, FascicleRoleModel, ProcessFascicleTemplateService, ExternalSourceActionEnum, FascicleType, CategoryService, PageClassHelper, SessionStorageKeysHelper) {
    var uscProcessDetails = /** @class */ (function () {
        function uscProcessDetails(serviceConfigurations) {
            var _this = this;
            this.TYPOLOGY_ATTRIBUTE = "Typology";
            this.deleteRolePromise = function (roleIdToDelete, senderId) {
                var promise = $.Deferred();
                if (!roleIdToDelete)
                    return promise.promise();
                switch (uscProcessDetails.selectedEntityType) {
                    case ProcessNodeType.Category:
                    case ProcessNodeType.Process: {
                        _this._processService.getById(uscProcessDetails.selectedProcessId, function (data) {
                            var process = data;
                            process.Roles = process.Roles
                                .filter(function (role) { return role.IdRole !== roleIdToDelete || role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1; });
                            _this._processService.update(process, function (data) {
                                promise.resolve(data);
                            }, function (error) {
                                _this._ajaxLoadingPanel.hide(_this._pnlInformations.get_element().id);
                                _this.showNotificationException(error);
                            });
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide(_this._pnlInformations.get_element().id);
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
                                _this._ajaxLoadingPanel.hide(_this._pnlInformations.get_element().id);
                                _this.showNotificationException(error);
                            });
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide(_this._pnlWorkflowDetails.get_element().id);
                            _this.showNotificationException(error);
                        });
                        break;
                    }
                    case ProcessNodeType.ProcessFascicleTemplate: {
                        switch (senderId) {
                            case _this.uscResponsibleRolesId: {
                                promise.resolve(uscProcessDetails.responsibleRole ? [uscProcessDetails.responsibleRole] : []);
                                uscProcessDetails.responsibleRole = null;
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
                    case ProcessNodeType.Category:
                    case ProcessNodeType.Process: {
                        _this._processService.getById(uscProcessDetails.selectedProcessId, function (data) {
                            var process = data;
                            process.Roles = __spreadArrays(process.Roles, newAddedRoles);
                            _this._processService.update(process, function (data) {
                                promise.resolve(data);
                            }, function (error) {
                                _this._ajaxLoadingPanel.hide(_this._pnlInformations.get_element().id);
                                _this.showNotificationException(error);
                            });
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide(_this._pnlInformations.get_element().id);
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
                                _this._ajaxLoadingPanel.hide(_this._pnlInformations.get_element().id);
                                _this.showNotificationException(error);
                            });
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide(_this._pnlWorkflowDetails.get_element().id);
                            _this.showNotificationException(error);
                        });
                        break;
                    }
                    case ProcessNodeType.ProcessFascicleTemplate: {
                        switch (senderId) {
                            case _this.uscResponsibleRolesId: {
                                var _loop_1 = function (authorizedRole) {
                                    if (newAddedRoles.filter(function (x) { return x.IdRole === authorizedRole.IdRole; }).length > 0) {
                                        var existedRole = newAddedRoles.filter(function (x) { return x.IdRole === authorizedRole.IdRole; })[0];
                                        alert("Non \u00E8 possibile selezionare il settore " + existedRole.Name + " in quanto gi\u00E0 presente come settore autorizzato del modello di fascicolo");
                                        newAddedRoles = newAddedRoles.filter(function (x) { return x.IdRole !== authorizedRole.IdRole; });
                                        promise.resolve(existedRole, true);
                                    }
                                };
                                for (var _i = 0, _a = uscProcessDetails.authorizedRoles; _i < _a.length; _i++) {
                                    var authorizedRole = _a[_i];
                                    _loop_1(authorizedRole);
                                }
                                if (newAddedRoles.length > 0) {
                                    uscProcessDetails.responsibleRole = newAddedRoles[0];
                                    promise.resolve([uscProcessDetails.responsibleRole]);
                                }
                                else {
                                    promise.resolve([]);
                                }
                                break;
                            }
                            case _this.uscAuthorizedRolesId: {
                                if (uscProcessDetails.responsibleRole && (newAddedRoles.filter(function (x) { return x.IdRole === uscProcessDetails.responsibleRole.IdRole; }).length > 0)) {
                                    var existedRole = newAddedRoles.filter(function (x) { return x.IdRole === uscProcessDetails.responsibleRole.IdRole; })[0];
                                    alert("Non \u00E8 possibile selezionare il settore " + existedRole.Name + " in quanto gi\u00E0 presente come settore responsabile del modello di fascicolo");
                                    newAddedRoles = newAddedRoles.filter(function (x) { return x.IdRole !== uscProcessDetails.responsibleRole.IdRole; });
                                    promise.resolve(existedRole);
                                }
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
                        _this._ajaxLoadingPanel.show(_this._pnlWorkflowDetails.get_element().id);
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
                        var exist = false;
                        for (var i = 0; i < _this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().get_count(); i++) {
                            if (_this._rcbWorkflowRepository.get_selectedItem().get_text() === _this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().getItem(i).get_text())
                                exist = true;
                        }
                        if (!exist) {
                            _this._processFascicleWorkflowRepositoryService.insert(processFascicleWorkflowRepository, function (data) {
                                var node = new Telerik.Web.UI.RadTreeNode();
                                node.set_text(selectedWorkflowRepository_1.Name);
                                node.set_value(data.UniqueId);
                                _this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().add(node);
                                _this._rtvWorkflowRepository.get_nodes().getNode(0).expand();
                                _this._ajaxLoadingPanel.hide(_this._pnlWorkflowDetails.get_element().id);
                            }, function (error) {
                                _this._ajaxLoadingPanel.hide(_this._pnlWorkflowDetails.get_element().id);
                                _this.showNotificationException(error);
                            });
                        }
                        else {
                            _this._ajaxLoadingPanel.hide(_this._pnlWorkflowDetails.get_element().id);
                            alert("Un flusso del lavoro con il nome scelto è già esistente.");
                        }
                        break;
                    }
                    case "delete": {
                        if (_this._rtvWorkflowRepository.get_selectedNode() === null || _this._rtvWorkflowRepository.get_selectedNode().get_level() === 0) {
                            alert("Selezionare flusso di lavoro");
                            return;
                        }
                        _this._ajaxLoadingPanel.show(_this._pnlWorkflowDetails.get_element().id);
                        var processFascicleWorkflowRepository = {};
                        processFascicleWorkflowRepository.UniqueId = _this._rtvWorkflowRepository.get_selectedNode().get_value();
                        _this._processFascicleWorkflowRepositoryService.delete(processFascicleWorkflowRepository, function (data) {
                            _this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().remove(_this._rtvWorkflowRepository.get_selectedNode());
                            _this._ajaxLoadingPanel.hide(_this._pnlWorkflowDetails.get_element().id);
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide(_this._pnlWorkflowDetails.get_element().id);
                            _this.showNotificationException(error);
                        });
                        break;
                    }
                }
            };
            this.rbAddFascicle_onClick = function (sender, args) {
                var processFascicleTemplate = {};
                processFascicleTemplate.DossierFolder = {};
                processFascicleTemplate.Process = {};
                processFascicleTemplate.UniqueId = uscProcessDetails.selectedProcessFascicleTemplateId;
                processFascicleTemplate.Process.UniqueId = uscProcessDetails.selectedProcessId;
                _this.populateFascicleTemplateInfo().then(function (jsonModel) {
                    processFascicleTemplate.JsonModel = jsonModel;
                    processFascicleTemplate.Name = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_PROCESS_FASCICLE_TEMPLATE_NAME);
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_PROCESS_FASCICLE_TEMPLATE_NAME, JSON.stringify(processFascicleTemplate));
                    _this._processFascicleTemplateService.update(processFascicleTemplate, function (data) {
                        alert("Modificato con successo");
                        _this._ajaxLoadingPanel.hide(_this._pnlFascicleDetails.get_element().id);
                    }, function (error) {
                        _this._ajaxLoadingPanel.hide(_this._pnlFascicleDetails.get_element().id);
                        _this.showNotificationException(error);
                    });
                });
            };
            this.rcbFascicleType_selectedIndexChanged = function (sender, args) {
                var fascicleIsProcedureOrDefault = ["", FascicleType[FascicleType.Procedure]].indexOf(args.get_item().get_value()) > -1;
                $("#uscContactRestFieldset").toggle(fascicleIsProcedureOrDefault);
                $("#responsibleRoleFieldset").toggle(fascicleIsProcedureOrDefault);
                _this._rbFascicleVisibilityType.set_visible(fascicleIsProcedureOrDefault);
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
            this.loadCustomActions({
                AutoClose: false,
                AutoCloseAndClone: false
            });
            this.bindLoaded();
            this.loadFascicleTypes();
        };
        uscProcessDetails.prototype.initializeServices = function () {
            var categoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Category");
            this._categoryService = new CategoryService(categoryConfiguration);
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
            this._lblFolderName = document.getElementById(this.lblFolderNameId);
            this._divFolderName = document.getElementById(this.divFolderNameId);
            this._lblActivationDate = document.getElementById(this.lblActivationDateId);
            this._lblNote = document.getElementById(this.lblNoteId);
            this._lblClasificationName = document.getElementById(this.lblClasificationNameId);
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
            this._rcbFascicleType = $find(this.rcbFascicleTypeId);
            this._rcbFascicleType.add_selectedIndexChanged(this.rcbFascicleType_selectedIndexChanged);
            this._rpbDetails = $find(this.rpbDetailsId);
            this._pnlInformations = this._rpbDetails.findItemByValue("pnlInformations");
            this._pnlCategoryInformations = this._rpbDetails.findItemByValue("pnlCategoryInformations");
            this._pnlRoleDetails = this._rpbDetails.findItemByValue("pnlRoleDetails");
            this._pnlWorkflowDetails = this._rpbDetails.findItemByValue("pnlWorkflowDetails");
            this._pnlFascicleDetails = this._rpbDetails.findItemByValue("pnlFascicleDetails");
            this._lblCategoryCode = document.getElementById(this.lblCategoryCodeId);
            this._lblCategoryName = document.getElementById(this.lblCategoryNameId);
            this._lblStartDate = document.getElementById(this.lblStartDateId);
            this._lblEndDate = document.getElementById(this.lblEndDateId);
            this._lblMetadata = document.getElementById(this.lblMetadataId);
            this._lblMassimarioName = document.getElementById(this.lblMassimarioNameId);
            this._lblRegistrationDate = document.getElementById(this.lblRegistrationDateId);
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
        uscProcessDetails.prototype.setCategoryDetails = function () {
            var _this = this;
            this._uscRoleRest.disableButtons();
            this._categoryService.getRolesByCategoryId(uscProcessDetails.selectedCategoryId, function (data) {
                var category = data;
                _this._lblCategoryCode.innerText = category.getFullCodeDotted();
                _this._lblCategoryName.innerText = category.Name + " (" + category.EntityShortId + ")";
                _this._lblStartDate.innerText = category.StartDate ? moment(new Date(category.StartDate)).format("DD/MM/YYYY") : "";
                _this._lblEndDate.innerText = category.EndDate ? moment(new Date(category.EndDate)).format("DD/MM/YYYY") : "";
                if (_this._lblMetadata) {
                    _this._lblMetadata.innerText = category.MetadataRepository ? category.MetadataRepository.Name : "";
                }
                _this._lblMassimarioName.innerText = category.MassimarioScarto
                    ? category.MassimarioScarto.FullCode.replace("|", ".") + "." + category.MassimarioScarto.Name + "(" + category.MassimarioScarto.ConservationPeriod + " Anni)"
                    : "";
                _this._lblRegistrationDate.innerText = moment(new Date(category.RegistrationDate)).format("DD/MM/YYYY");
                if (category.CategoryFascicles.length > 0) {
                    var categoryFascicleRightsModel = category.CategoryFascicles.map(function (x) { return x.CategoryFascicleRights; });
                    var roleArray = [];
                    for (var _i = 0, categoryFascicleRightsModel_1 = categoryFascicleRightsModel; _i < categoryFascicleRightsModel_1.length; _i++) {
                        var cfrm = categoryFascicleRightsModel_1[_i];
                        roleArray = cfrm.map(function (x) { return x.Role; });
                    }
                    _this._uscRoleRest.renderRolesTree(roleArray);
                }
                else {
                    _this._uscRoleRest.renderRolesTree([]);
                }
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this._pnlInformations.get_element().id);
                _this.showNotificationException(error);
            });
        };
        uscProcessDetails.prototype.setProcessDetails = function (dossierFolderName, populateRoles) {
            var _this = this;
            this._uscRoleRest.enableButtons();
            var workflowRepositories = this._rcbWorkflowRepository.get_items();
            var metadataRepositories = this._rcbMetadataRepository.get_items();
            if (workflowRepositories.get_count() > 0) {
                workflowRepositories.getItem(0).select();
            }
            if (metadataRepositories.get_count() > 0) {
                metadataRepositories.getItem(0).select();
            }
            this._uscResponsibleRoles.renderRolesTree([]);
            this._uscAuthorizedRoles.renderRolesTree([]);
            this._uscAuthorizedRoles.disableRaciRoleButton();
            this._uscFascicleFolders.fileManagementButtonsVisibility(false);
            if (uscProcessDetails.selectedEntityType === ProcessNodeType.ProcessFascicleTemplate) {
                uscProcessDetails.responsibleRole = null;
                uscProcessDetails.authorizedRoles = [];
                uscProcessDetails.raciRoles = [];
            }
            this._processService.getById(uscProcessDetails.selectedProcessId, function (data) {
                var process = data;
                _this._divFolderName.style.display = "none";
                _this._lblProcessName.innerText = process.Name;
                if (process.StartDate) {
                    _this._lblActivationDate.innerText = moment(process.StartDate).format("DD/MM/YYYY");
                }
                if (dossierFolderName && dossierFolderName !== '') {
                    _this._divFolderName.style.display = "";
                    _this._lblFolderName.innerText = dossierFolderName;
                }
                _this._lblClasificationName.innerText = process.Category.getFullCodeDotted() + " - " + process.Category.Name;
                _this._lblNote.innerText = process.Note;
                if (populateRoles) {
                    //set popup roles source
                    if (uscProcessDetails.selectedEntityType === ProcessNodeType.Process) {
                        _this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.Process.toString(), uscProcessDetails.selectedProcessId];
                    }
                    else if (uscProcessDetails.selectedEntityType === ProcessNodeType.Category) {
                        _this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.Category.toString(), uscProcessDetails.selectedCategoryId.toString()];
                    }
                    $("#" + _this.uscRoleRestId).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, _this.needRolesFromExternalSource_eventArgs);
                    _this._uscRoleRest.renderRolesTree(process.Roles);
                }
                _this._ajaxLoadingPanel.hide(_this._pnlInformations.get_element().id);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this._pnlInformations.get_element().id);
                _this.showNotificationException(error);
            });
        };
        uscProcessDetails.prototype.clearProcessDetails = function () {
            this._lblProcessName.innerText = "";
            this._lblClasificationName.innerText = "";
        };
        uscProcessDetails.prototype.setDossierFolderWorkflowRepositories = function () {
            var _this = this;
            this._uscRoleRest.enableButtons();
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
                _this._ajaxLoadingPanel.hide(_this._pnlWorkflowDetails.get_element().id);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this._pnlWorkflowDetails.get_element().id);
                _this.showNotificationException(error);
            });
        };
        uscProcessDetails.prototype.setDossierFolderRoles = function () {
            var _this = this;
            this._uscRoleRest.enableButtons();
            this._dossierFolderService.getDossierFolderById(uscProcessDetails.selectedDossierFolderId, function (data) {
                var dossierFolder = data[0];
                //set popup roles source
                _this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.Process.toString(), uscProcessDetails.selectedProcessId];
                $("#" + _this.uscRoleRestId).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, _this.needRolesFromExternalSource_eventArgs);
                if (dossierFolder.DossierFolderRoles && dossierFolder.DossierFolderRoles.length > 0) {
                    _this._uscRoleRest.renderRolesTree(dossierFolder.DossierFolderRoles.map(function (x) { return x.Role; }));
                }
                else {
                    _this._uscRoleRest.clearRoleTreeView();
                }
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this._pnlWorkflowDetails.get_element().id);
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
        uscProcessDetails.prototype.populateFascicleTemplateInfo = function () {
            var promise = $.Deferred();
            var fascicleModel = new FascicleModel();
            fascicleModel.FascicleObject = this._rtbFascicleSubject.get_value();
            if (this._rcbFascicleType.get_selectedItem()) {
                fascicleModel.FascicleType = FascicleType[this._rcbFascicleType.get_selectedItem().get_value()];
            }
            var rbFascicleVisibilityType_isVisible = this._rbFascicleVisibilityType.get_visible();
            fascicleModel.VisibilityType = (rbFascicleVisibilityType_isVisible && this._rbFascicleVisibilityType.get_checked())
                ? VisibilityType.Accessible
                : VisibilityType.Confidential;
            uscProcessDetails.raciRoles = this._uscAuthorizedRoles.getRaciRoles();
            fascicleModel.FascicleRoles = [];
            if (uscProcessDetails.responsibleRole) {
                var fascicleRole = new FascicleRoleModel();
                fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
                fascicleRole.IsMaster = true;
                fascicleRole.Role = uscProcessDetails.responsibleRole;
                fascicleModel.FascicleRoles.push(fascicleRole);
            }
            var _loop_2 = function (authorizedRole) {
                var fascicleRole = new FascicleRoleModel();
                fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                fascicleRole.IsMaster = false;
                fascicleRole.Role = authorizedRole;
                fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                if (uscProcessDetails.raciRoles && uscProcessDetails.raciRoles.some(function (x) { return x.IdRole === authorizedRole.IdRole; })) {
                    fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
                }
                fascicleModel.FascicleRoles.push(fascicleRole);
            };
            for (var _i = 0, _a = uscProcessDetails.authorizedRoles; _i < _a.length; _i++) {
                var authorizedRole = _a[_i];
                _loop_2(authorizedRole);
            }
            this.fascicleFolders = [];
            this.getFascicleFolderListFromTree(this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0));
            fascicleModel.FascicleFolders = this.fascicleFolders;
            fascicleModel.Contacts = uscProcessDetails.contacts;
            fascicleModel.MetadataRepository = new MetadataRepositoryModel();
            if (this._rcbMetadataRepository.get_selectedItem() && this._rcbMetadataRepository.get_selectedItem().get_value()) {
                fascicleModel.MetadataRepository.Name = this._rcbMetadataRepository.get_selectedItem().get_text();
                fascicleModel.MetadataRepository.UniqueId = this._rcbMetadataRepository.get_selectedItem().get_value();
            }
            PageClassHelper.callUserControlFunctionSafe(this.uscCustomActionsRestId)
                .done(function (instance) {
                var customActions = instance.getCustomActions();
                fascicleModel.CustomActions = JSON.stringify(customActions);
                promise.resolve(JSON.stringify(fascicleModel));
            });
            return promise.promise();
        };
        uscProcessDetails.prototype.populateDetails = function (data) {
            var processFascicleTemplate = data;
            if (processFascicleTemplate.JsonModel === "") {
                return;
            }
            var fascicle = JSON.parse(processFascicleTemplate.JsonModel);
            this._rtbFascicleSubject.set_value(fascicle.FascicleObject);
            if (fascicle.FascicleType) {
                var fascicleTypeItem = this._rcbFascicleType.findItemByValue(FascicleType[fascicle.FascicleType]);
                fascicleTypeItem.select();
            }
            this._rbFascicleVisibilityType.set_checked(fascicle.VisibilityType === VisibilityType.Accessible);
            if (fascicle.FascicleRoles.filter(function (x) { return x.IsMaster === true; }).map(function (x) { return x.Role; }).length > 0) {
                uscProcessDetails.responsibleRole = fascicle.FascicleRoles.filter(function (x) { return x.IsMaster === true; }).map(function (x) { return x.Role; })[0];
                this._uscResponsibleRoles.renderRolesTree([uscProcessDetails.responsibleRole]);
            }
            uscProcessDetails.authorizedRoles = fascicle.FascicleRoles.filter(function (x) { return x.IsMaster === false; }).map(function (x) { return x.Role; });
            uscProcessDetails.raciRoles = fascicle.FascicleRoles
                .filter(function (x) { return x.IsMaster === false && x.AuthorizationRoleType === AuthorizationRoleType.Responsible; }).map(function (x) { return x.Role; });
            //set popup roles source
            this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.Process.toString(), uscProcessDetails.selectedProcessId];
            $("#" + this.uscResponsibleRolesId).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, this.needRolesFromExternalSource_eventArgs);
            if (uscProcessDetails.raciRoles) {
                this._uscAuthorizedRoles.setRaciRoles(uscProcessDetails.raciRoles);
            }
            this._uscAuthorizedRoles.renderRolesTree(uscProcessDetails.authorizedRoles);
            this.populateFascicleFoldersTree(fascicle.FascicleFolders);
            uscProcessDetails.contacts = fascicle.Contacts;
            this._uscContactRest.renderContactsTree(uscProcessDetails.contacts);
            if (fascicle.MetadataRepository) {
                var rcbItem = this._rcbMetadataRepository.findItemByValue(fascicle.MetadataRepository.UniqueId);
                if (rcbItem) {
                    rcbItem.select();
                }
            }
            if (fascicle.CustomActions) {
                this.loadCustomActions(JSON.parse(fascicle.CustomActions));
            }
        };
        uscProcessDetails.prototype.getFascicleFolderListFromTree = function (fascicleFoldersNode) {
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
                this.getFascicleFolderListFromTree(child);
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
                child.get_attributes().setAttribute(this.TYPOLOGY_ATTRIBUTE, fasciclefolder.Typology);
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
            this._rcbFascicleType.clearSelection();
            this._rbFascicleVisibilityType.set_checked(false);
            this._uscResponsibleRoles.renderRolesTree([]);
            this._uscAuthorizedRoles.renderRolesTree([]);
            this.initializeUserControls();
            this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0).get_nodes().getNode(0).get_nodes().clear();
            this._uscContactRest.renderContactsTree([]);
            this._rcbMetadataRepository.set_selectedItem(this._rcbMetadataRepository.get_items().getItem(0));
        };
        uscProcessDetails.prototype.loadFascicleTypes = function () {
            var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
            emptyItem.set_text("");
            this._rcbFascicleType.get_items().add(emptyItem);
            this.setFascicleTypeItem(this._rcbFascicleType, [FascicleType.Procedure, FascicleType.Activity]);
        };
        uscProcessDetails.prototype.setFascicleTypeItem = function (comboBox, fascicleTypes) {
            for (var _i = 0, fascicleTypes_1 = fascicleTypes; _i < fascicleTypes_1.length; _i++) {
                var itemType = fascicleTypes_1[_i];
                var item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(this._enumHelper.getFascicleTypeDescription(itemType));
                item.set_value(FascicleType[itemType]);
                comboBox.get_items().add(item);
            }
        };
        uscProcessDetails.prototype.setPanelVisibility = function (panelName, isVisible) {
            switch (panelName) {
                case uscProcessDetails.InformationDetails_PanelName: {
                    this._pnlInformations.set_visible(isVisible);
                    break;
                }
                case uscProcessDetails.CategoryInformationDetails_PanelName: {
                    this._pnlCategoryInformations.set_visible(isVisible);
                    break;
                }
                case uscProcessDetails.RoleDetails_PanelName: {
                    this._pnlRoleDetails.set_visible(isVisible);
                    break;
                }
                case uscProcessDetails.WorkflowDetails_PanelName: {
                    this._pnlWorkflowDetails.set_visible(isVisible);
                    break;
                }
                case uscProcessDetails.FascicleDetails_PanelName: {
                    this._pnlFascicleDetails.set_visible(isVisible);
                    break;
                }
            }
        };
        uscProcessDetails.prototype.setPanelLoading = function (panelName, isVisible) {
            switch (panelName) {
                case uscProcessDetails.InformationDetails_PanelName: {
                    this.setLoading(this._pnlInformations.get_element().id, isVisible);
                    break;
                }
                case uscProcessDetails.CategoryInformationDetails_PanelName: {
                    this.setLoading(this._pnlCategoryInformations.get_element().id, isVisible);
                    break;
                }
                case uscProcessDetails.RoleDetails_PanelName: {
                    this.setLoading(this._pnlRoleDetails.get_element().id, isVisible);
                    break;
                }
                case uscProcessDetails.WorkflowDetails_PanelName: {
                    this.setLoading(this._pnlWorkflowDetails.get_element().id, isVisible);
                    break;
                }
                case uscProcessDetails.FascicleDetails_PanelName: {
                    this.setLoading(this._pnlFascicleDetails.get_element().id, isVisible);
                    break;
                }
            }
        };
        uscProcessDetails.prototype.setLoading = function (elementId, isVisible) {
            if (isVisible) {
                this._ajaxLoadingPanel.show(elementId);
            }
            else {
                this._ajaxLoadingPanel.hide(elementId);
            }
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
        uscProcessDetails.prototype.loadCustomActions = function (customActions) {
            PageClassHelper.callUserControlFunctionSafe(this.uscCustomActionsRestId)
                .done(function (instance) {
                instance.loadItems(customActions);
            });
        };
        uscProcessDetails.raciRoles = [];
        uscProcessDetails.InformationDetails_PanelName = "informationDetails";
        uscProcessDetails.CategoryInformationDetails_PanelName = "categoryInformationDetails";
        uscProcessDetails.RoleDetails_PanelName = "roleDetails";
        uscProcessDetails.WorkflowDetails_PanelName = "workflowDetails";
        uscProcessDetails.FascicleDetails_PanelName = "fascicleDetails";
        return uscProcessDetails;
    }());
    return uscProcessDetails;
});
//# sourceMappingURL=uscProcessDetails.js.map