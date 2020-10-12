/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
define(["require", "exports", "App/Helpers/PageClassHelper", "App/Models/Fascicles/FascicleType", "UserControl/uscCategoryRest", "./uscRoleRest", "App/Models/Commons/UscRoleRestEventType", "./uscMetadataRepositorySel", "App/Models/Fascicles/FascicleRoleModel", "App/Helpers/EnumHelper", "App/Models/Processes/ProcessNodeType", "App/Helpers/ExternalSourceActionEnum", "App/Services/Dossiers/DossierFolderService", "App/Services/Processes/ProcessService", "App/Services/Processes/ProcessFascicleTemplateService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Fascicles/FascicleModel", "App/DTOs/ExceptionDTO", "App/Models/Fascicles/VisibilityType", "App/Models/Commons/CategoryModel", "App/Models/Commons/AuthorizationRoleType"], function (require, exports, PageClassHelper, FascicleType, uscCategoryRest, uscRoleRest, UscRoleRestEventType, uscMetadataRepositorySel, FascicleRoleModel, EnumHelper, ProcessNodeType, ExternalSourceActionEnum, DossierFolderService, ProcessService, ProcessFascicleTemplateService, ServiceConfigurationHelper, FascicleModel, ExceptionDTO, VisibilityType, CategoryModel, AuthorizationRoleType) {
    var uscFascicleProcessInsert = /** @class */ (function () {
        /**
        * Costruttore
        */
        function uscFascicleProcessInsert(serviceConfigurations) {
            var _this = this;
            this.customActionFromSession = false;
            this._enumHelper = new EnumHelper();
            /**
            *------------------------- Events -----------------------------
            */
            this.rcbFascicleType_selectedIndexChanged = function (sender, args) {
                var elements = [_this._pnlContact(), _this._pnlRoleMaster(), _this._pnlConservation()];
                var procedureSelected = args.get_item().get_value() === FascicleType[FascicleType.Procedure];
                _this.setVisibilityForManyElements(elements, procedureSelected);
                PageClassHelper.callUserControlFunctionSafe(_this.uscContactId)
                    .done(function (instance) {
                    instance.forceBehaviourValidationState(procedureSelected);
                    instance.enableValidators(procedureSelected);
                });
                PageClassHelper.callUserControlFunctionSafe(_this.uscRoleMasterId)
                    .done(function (instance) {
                    instance.forceBehaviourValidationState(procedureSelected);
                    instance.enableValidators(procedureSelected);
                });
                PageClassHelper.callUserControlFunctionSafe(_this.uscRoleId)
                    .done(function (instance) {
                    instance.forceBehaviourValidationState(!procedureSelected);
                    instance.enableValidators(!procedureSelected);
                    instance.disableRaciRoleButton();
                });
            };
            this.uscCategory_CategoryAdded = function (eventObject) {
                PageClassHelper.callUserControlFunctionSafe(_this.uscCategoryId)
                    .done(function (instance) {
                    switch (instance.getSelectedNode().get_attributes().getAttribute("NodeType")) {
                        case ProcessNodeType.ProcessFascicleTemplate: {
                            _this.clearFascicleFields();
                            _this._processFascicleTemplateService.getById(instance.getSelectedNode().get_value(), function (data) {
                                _this.setFascicleTemplateToSession(data);
                                _this.loadFascicleFields(data);
                            });
                            _this._processService.getById(instance.getProcessId(), function (data) {
                                _this.setProcessToSession(data);
                            });
                            _this._dossierFolderService.getDossierFolderById(instance.getProcessFascicleTemplateFolderId(), function (data) {
                                _this.setDossierFolderToSession(data[0]);
                            });
                            break;
                        }
                        case ProcessNodeType.DossierFolder: {
                            _this._processService.getById(instance.getProcessId(), function (data) {
                                _this.setProcessToSession(data);
                            });
                            _this._dossierFolderService.getDossierFolderById(instance.getProcessFascicleTemplateFolderId(), function (data) {
                                _this.setDossierFolderToSession(data[0]);
                                //set popup roles source
                                _this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.DossierFolder.toString(), instance.getProcessFascicleTemplateFolderId()];
                                $("#" + _this.uscRoleMasterId).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, _this.needRolesFromExternalSource_eventArgs);
                                _this._rcbFascicleType.findItemByValue(FascicleType[FascicleType.Procedure]).select();
                                _this._rcbFascicleType.enable();
                            });
                            break;
                        }
                        case ProcessNodeType.Category: {
                            PageClassHelper.callUserControlFunctionSafe(_this.uscCategoryId)
                                .done(function (instance) {
                                var selectedCategoryId = instance.getSelectedNode().get_value();
                                var selectedCategoryText = instance.getSelectedNode().get_text();
                                //set popup roles source
                                _this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.Category.toString(), selectedCategoryId.toString()];
                                $("#" + _this.uscRoleMasterId).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, _this.needRolesFromExternalSource_eventArgs);
                                var category = new CategoryModel();
                                category.EntityShortId = selectedCategoryId;
                                category.Code = +(selectedCategoryText.split(".")[0]);
                                category.Name = selectedCategoryText.split(".")[1];
                                _this.setCategoryToSession(category);
                                if (_this.customActionFromSession) {
                                    instance.getCategoryFascicles(selectedCategoryId).then(function (data) {
                                        var customActionsJson = data[0].CustomActions;
                                        if (customActionsJson) {
                                            PageClassHelper.callUserControlFunctionSafe(_this.uscCustomActionsRestId)
                                                .done(function (instance) {
                                                instance.loadItems(JSON.parse(customActionsJson));
                                            });
                                        }
                                    });
                                }
                            });
                        }
                    }
                });
            };
            this.getFascicle = function () {
                var promise = $.Deferred();
                var fascicle = new FascicleModel();
                fascicle.FascicleType = _this.getSelectedFascicleType();
                fascicle.Category = _this.getCategory();
                fascicle.FascicleObject = _this._txtObject.get_textBoxValue();
                fascicle.Note = _this._txtNote.get_textBoxValue();
                fascicle.Conservation = _this._txtConservation.get_textBoxValue() ? parseInt(_this._txtConservation.get_textBoxValue()) : null;
                fascicle.Contacts = _this.getFascicleContactsToAdd();
                _this.getFascicleRoles().then(function (fascicleRoles) {
                    fascicle.FascicleRoles = fascicleRoles;
                    var dossierFolders = [];
                    var dossier = {};
                    if (_this.getCurrentDossierFolder()) {
                        var fascicleDossierFolder = _this.getCurrentDossierFolder();
                        dossier.UniqueId = fascicleDossierFolder.UniqueId;
                        dossier.Status = fascicleDossierFolder.Status;
                        dossierFolders.push(dossier);
                    }
                    fascicle.DossierFolders = dossierFolders;
                    fascicle.FascicleTemplate = _this.getCurrentFascicleTemplate();
                    if (!fascicle.VisibilityType) {
                        fascicle.VisibilityType = VisibilityType.Confidential;
                    }
                    PageClassHelper.callUserControlFunctionSafe(_this.uscCustomActionsRestId)
                        .done(function (instance) {
                        var customActions = instance.getCustomActions();
                        fascicle.CustomActions = JSON.stringify(customActions);
                        promise.resolve(fascicle);
                    });
                });
                return promise.promise();
            };
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, uscFascicleProcessInsert.DOSSIER_FOLDER_TYPE_NAME);
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            var processConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, uscFascicleProcessInsert.PROCESS_TYPE_NAME);
            this._processService = new ProcessService(processConfiguration);
            var processFascicleTemplateConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, uscFascicleProcessInsert.PROCESS_FASCICLE_TEMPLATE_TYPE_NAME);
            this._processFascicleTemplateService = new ProcessFascicleTemplateService(processFascicleTemplateConfiguration);
        }
        uscFascicleProcessInsert.prototype._pnlContact = function () {
            return $("#" + this.pnlContactId);
        };
        uscFascicleProcessInsert.prototype._pnlRoleMaster = function () {
            return $("#" + this.pnlRoleMasterId);
        };
        uscFascicleProcessInsert.prototype._pnlConservation = function () {
            return $("#" + this.pnlConservationId);
        };
        /**
        *------------------------- Methods -----------------------------
        */
        uscFascicleProcessInsert.prototype.initialize = function () {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscCategoryId)
                .done(function (instance) { return instance.setFascicleTypeParam(FascicleType.Procedure); });
            this._txtObject = $find(this.txtObjectId);
            this._txtConservation = $find(this.txtConservationId);
            this._txtNote = $find(this.txtNoteId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._rcbFascicleType = $find(this.rcbFascicleTypeId);
            this._rcbFascicleType.add_selectedIndexChanged(this.rcbFascicleType_selectedIndexChanged);
            this.clearFascicleFields();
            this.registerUscRoleRestEventHandlers();
            this.registerUscContactRestEventHandlers();
            this.setMetadataRepositorySelectedIndexEvent();
            this.loadFascicleTypes();
            $("#" + this.uscCategoryId).bind(uscCategoryRest.ADDED_EVENT, this.uscCategory_CategoryAdded);
            /*event for filing out the fields with the chosen Seti contact*/
            $("#".concat(this.uscMetadataRepositorySelId)).on(uscMetadataRepositorySel.SELECTED_SETI_CONTACT_EVENT, function (sender, args) {
                PageClassHelper.callUserControlFunctionSafe(_this.uscDynamicMetadataRestId)
                    .done(function (instance) { return instance.populateMetadataRepository(args); });
            });
            PageClassHelper.callUserControlFunctionSafe(this.uscCustomActionsRestId)
                .done(function (instance) {
                instance.loadItems({
                    AutoClose: false,
                    AutoCloseAndClone: false
                });
            });
            this.bindLoaded();
        };
        uscFascicleProcessInsert.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(uscFascicleProcessInsert.LOADED_EVENT);
        };
        uscFascicleProcessInsert.prototype.setVisibilityForManyElements = function (elements, isVisible) {
            for (var _i = 0, elements_1 = elements; _i < elements_1.length; _i++) {
                var element = elements_1[_i];
                isVisible ? element.show() : element.hide();
            }
        };
        uscFascicleProcessInsert.prototype.registerUscRoleRestEventHandlers = function () {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleId)
                .done(function (instance) {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, function (roleId) {
                    _this.deleteRoleFromModel(roleId);
                    return $.Deferred().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, function (newAddedRoles) {
                    var existedRole;
                    _this.addRoleToModel(_this.uscRoleMasterId, newAddedRoles, function (role) {
                        existedRole = role;
                    });
                    return $.Deferred().resolve(existedRole);
                });
            });
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleMasterId)
                .done(function (instance) {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, function (roleId) {
                    _this.deleteRoleFromModel(roleId);
                    return $.Deferred().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, function (newAddedRoles) {
                    var existedRole;
                    _this.addRoleToModel(_this.uscRoleId, newAddedRoles, function (role) {
                        existedRole = role;
                    });
                    if (!existedRole) {
                        _this.selectedResponsibleRole = newAddedRoles[0];
                    }
                    return $.Deferred().resolve(existedRole, true);
                });
            });
        };
        uscFascicleProcessInsert.prototype.registerUscContactRestEventHandlers = function () {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscContactId)
                .done(function (instance) {
                instance.registerEventHandler(instance.uscContattiSelRestEvents.ContactDeleted, function (contactIdToDelete) {
                    _this.deleteContactFromModel(contactIdToDelete);
                    return $.Deferred().resolve();
                });
                instance.registerEventHandler(instance.uscContattiSelRestEvents.NewContactsAdded, function (newAddedContact) {
                    _this.addContactToModel(newAddedContact);
                    return $.Deferred().resolve();
                });
            });
        };
        uscFascicleProcessInsert.prototype.setMetadataRepositorySelectedIndexEvent = function () {
            var _this = this;
            $("#" + this.uscMetadataRepositorySelId).off(uscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT);
            $("#" + this.uscMetadataRepositorySelId).on(uscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, function (args, data) {
                PageClassHelper.callUserControlFunctionSafe(_this.uscDynamicMetadataRestId)
                    .done(function (instance) {
                    if (data) {
                        instance.loadMetadataRepository(data);
                    }
                    else {
                        // when no selection (or selected index is an invalid value like the first empty string in a drop down)
                        instance.clearPage();
                    }
                });
            });
        };
        uscFascicleProcessInsert.prototype.deleteRoleFromModel = function (roleIdToDelete) {
            if (!roleIdToDelete)
                return;
            var fascicleRoles = [];
            if (this.getFascicleRolesToAdd()) {
                fascicleRoles = this.getFascicleRolesToAdd();
            }
            fascicleRoles = fascicleRoles.filter(function (x) { return x.Role.IdRole !== roleIdToDelete && x.Role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1; });
            this.setFascicleRolesToSession(fascicleRoles);
        };
        uscFascicleProcessInsert.prototype.setFascicleRolesToSession = function (fascicleRoles) {
            if (!fascicleRoles) {
                sessionStorage.removeItem(this.clientId + "_FascicleRolesToAdd");
            }
            sessionStorage[this.clientId + "_FascicleRolesToAdd"] = JSON.stringify(fascicleRoles);
        };
        uscFascicleProcessInsert.prototype.setFascicleContactsToSession = function (fascicleContacts) {
            if (!fascicleContacts) {
                sessionStorage.removeItem(this.clientId + "_FascicleContactsToAdd");
            }
            sessionStorage[this.clientId + "_FascicleContactsToAdd"] = JSON.stringify(fascicleContacts);
        };
        uscFascicleProcessInsert.prototype.setFascicleTemplateToSession = function (fascicleTemplate) {
            if (!fascicleTemplate) {
                sessionStorage.removeItem(this.clientId + "_CurrentFascicleTemplate");
            }
            sessionStorage[this.clientId + "_CurrentFascicleTemplate"] = JSON.stringify(fascicleTemplate);
        };
        uscFascicleProcessInsert.prototype.setProcessToSession = function (process) {
            if (!process) {
                sessionStorage.removeItem(this.clientId + "_CurrentProcess");
            }
            sessionStorage[this.clientId + "_CurrentProcess"] = JSON.stringify(process);
        };
        uscFascicleProcessInsert.prototype.setDossierFolderToSession = function (dossierFolder) {
            if (!dossierFolder) {
                sessionStorage.removeItem(this.clientId + "_CurrentDossierFolder");
            }
            sessionStorage[this.clientId + "_CurrentDossierFolder"] = JSON.stringify(dossierFolder);
        };
        uscFascicleProcessInsert.prototype.setCategoryToSession = function (category) {
            if (!category) {
                sessionStorage.removeItem(this.clientId + "_CurrentCategory");
            }
            sessionStorage[this.clientId + "_CurrentCategory"] = JSON.stringify(category);
        };
        uscFascicleProcessInsert.prototype.addRoleToModel = function (toCheckControlId, newAddedRoles, existedRoleCallback) {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(toCheckControlId)
                .done(function (instance) {
                var existedRole = instance.existsRole(newAddedRoles);
                if (existedRole) {
                    alert("Non \u00E8 possibile selezionare il settore " + existedRole.Name + " in quanto gi\u00E0 presente come settore " + (toCheckControlId == _this.uscRoleMasterId ? "responsabile" : "autorizzato") + " del fascicolo");
                    existedRoleCallback(existedRole);
                    newAddedRoles = newAddedRoles.filter(function (x) { return x.IdRole !== existedRole.IdRole; });
                }
                if (toCheckControlId === _this.uscRoleMasterId) {
                    return _this.addRole(newAddedRoles, false);
                }
            });
        };
        uscFascicleProcessInsert.prototype.addRole = function (newAddedRoles, isMaster) {
            if (!newAddedRoles.length)
                return;
            var fascicleRoles = [];
            if (this.getFascicleRolesToAdd()) {
                fascicleRoles = this.getFascicleRolesToAdd();
            }
            for (var _i = 0, newAddedRoles_1 = newAddedRoles; _i < newAddedRoles_1.length; _i++) {
                var newAddedRole = newAddedRoles_1[_i];
                var fascicleRole = new FascicleRoleModel();
                fascicleRole.IsMaster = isMaster;
                fascicleRole.AuthorizationRoleType = isMaster
                    ? AuthorizationRoleType.Responsible
                    : AuthorizationRoleType.Accounted;
                fascicleRole.Role = newAddedRole;
                fascicleRoles.push(fascicleRole);
            }
            this.setFascicleRolesToSession(fascicleRoles);
        };
        uscFascicleProcessInsert.prototype.deleteContactFromModel = function (contactIdToDelete) {
            if (!contactIdToDelete)
                return;
            var fascicleContacts = [];
            if (this.getFascicleContactsToAdd()) {
                fascicleContacts = this.getFascicleContactsToAdd();
            }
            fascicleContacts = fascicleContacts.filter(function (x) { return x.EntityId !== contactIdToDelete; });
            this.setFascicleContactsToSession(fascicleContacts);
        };
        uscFascicleProcessInsert.prototype.addContactToModel = function (newAddedContact) {
            if (!newAddedContact)
                return;
            var fascicleContacts = [];
            if (this.getFascicleContactsToAdd()) {
                fascicleContacts = this.getFascicleContactsToAdd();
            }
            fascicleContacts.push(newAddedContact);
            this.setFascicleContactsToSession(fascicleContacts);
        };
        uscFascicleProcessInsert.prototype.loadFascicleTypes = function () {
            var fascicleTypes = [FascicleType.Procedure];
            if (this.activityFascicleEnabled) {
                fascicleTypes.push(FascicleType.Activity);
            }
            this.setFascicleTypeItem(this._rcbFascicleType, fascicleTypes);
            var item = this._rcbFascicleType.findItemByValue(FascicleType[FascicleType.Procedure]);
            item.select();
            if (this._rcbFascicleType.get_items().get_count() == 1) {
                this._rcbFascicleType.disable();
            }
        };
        uscFascicleProcessInsert.prototype.setFascicleTypeItem = function (comboBox, fascicleTypes) {
            for (var _i = 0, fascicleTypes_1 = fascicleTypes; _i < fascicleTypes_1.length; _i++) {
                var itemType = fascicleTypes_1[_i];
                var item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(this._enumHelper.getFascicleTypeDescription(itemType));
                item.set_value(FascicleType[itemType]);
                comboBox.get_items().add(item);
            }
        };
        uscFascicleProcessInsert.prototype.clearFascicleFields = function () {
            this._txtObject.clear();
            this.setFascicleRolesToSession([]);
            this.setFascicleContactsToSession([]);
            this.setFascicleTemplateToSession(null);
            this.setProcessToSession(null);
            this.setDossierFolderToSession(null);
            this.setCategoryToSession(null);
            this.loadRoles([]);
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleId).done(function (instance) { return instance.setToolbarVisibility(true); });
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleMasterId).done(function (instance) { return instance.setToolbarVisibility(true); });
            this.loadContacts([]);
            PageClassHelper.callUserControlFunctionSafe(this.uscContactId).done(function (instance) { return instance.setToolbarVisibility(true); });
            PageClassHelper.callUserControlFunctionSafe(this.uscMetadataRepositorySelId).done(function (instance) { return instance.clearComboboxText(); });
            PageClassHelper.callUserControlFunctionSafe(this.uscDynamicMetadataRestId).done(function (instance) { return instance.clearPage(); });
        };
        uscFascicleProcessInsert.prototype.loadRoles = function (items) {
            var rolesModel = [];
            var masterRolesModel = [];
            var raciRoles = [];
            var fascicleRoles = [];
            if (this.getFascicleRolesToAdd()) {
                fascicleRoles = this.getFascicleRolesToAdd();
            }
            for (var _i = 0, items_1 = items; _i < items_1.length; _i++) {
                var fascicleRole = items_1[_i];
                if (fascicleRole.IsMaster) {
                    masterRolesModel.push(fascicleRole.Role);
                }
                else {
                    if (fascicleRole.AuthorizationRoleType === AuthorizationRoleType.Responsible) {
                        raciRoles.push(fascicleRole.Role);
                    }
                    rolesModel.push(fascicleRole.Role);
                }
                fascicleRoles.push(fascicleRole);
            }
            this.setFascicleRolesToSession(fascicleRoles);
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleId).done(function (instance) {
                if (raciRoles) {
                    instance.setRaciRoles(raciRoles);
                }
                instance.renderRolesTree(rolesModel);
            });
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleMasterId).done(function (instance) { return instance.renderRolesTree(masterRolesModel); });
        };
        uscFascicleProcessInsert.prototype.loadContacts = function (contacts) {
            var fascicleContacts = [];
            if (this.getFascicleContactsToAdd()) {
                fascicleContacts = this.getFascicleContactsToAdd();
            }
            for (var _i = 0, contacts_1 = contacts; _i < contacts_1.length; _i++) {
                var contact = contacts_1[_i];
                fascicleContacts.push(contact);
            }
            PageClassHelper.callUserControlFunctionSafe(this.uscContactId).done(function (instance) { return instance.renderContactsTree(contacts); });
        };
        uscFascicleProcessInsert.prototype.loadFascicleFields = function (fascicleTemplate) {
            if (fascicleTemplate.JsonModel) {
                try {
                    var fascicleTemplateModel = JSON.parse(this.getCurrentFascicleTemplate().JsonModel);
                    if (fascicleTemplateModel.FascicleType) {
                        this._rcbFascicleType.findItemByValue(FascicleType[fascicleTemplateModel.FascicleType]).select();
                        this._rcbFascicleType.disable();
                    }
                    if (fascicleTemplateModel.FascicleType === FascicleType.Activity) {
                        this.loadActivityFascicleFields(fascicleTemplateModel);
                    }
                    else {
                        this.loadProcedureFascicleFields(fascicleTemplateModel);
                    }
                    $("#" + this.pnlFascProcessInsertId).show();
                }
                catch (e) {
                    this.clearFascicleFields();
                    return;
                }
            }
            else {
                this.loadProcedureFascicleFields(new FascicleModel());
                $("#" + this.pnlFascProcessInsertId).show();
            }
        };
        uscFascicleProcessInsert.prototype.loadActivityFascicleFields = function (fascicleTemplateModel, generateMetadataInputs) {
            if (generateMetadataInputs === void 0) { generateMetadataInputs = true; }
            var elements = [this._pnlContact(), this._pnlRoleMaster(), this._pnlConservation()];
            this.setVisibilityForManyElements(elements, false);
            this._txtObject.set_value(fascicleTemplateModel.FascicleObject);
            if (fascicleTemplateModel.MetadataRepository && fascicleTemplateModel.MetadataRepository.UniqueId) {
                this.loadMetadataRepository(fascicleTemplateModel.MetadataRepository.UniqueId, generateMetadataInputs);
            }
            else {
                PageClassHelper.callUserControlFunctionSafe(this.uscMetadataRepositorySelId).done(function (instance) { return instance.clearComboboxText(); });
            }
            this.loadRoles(fascicleTemplateModel.FascicleRoles);
            if (fascicleTemplateModel.CustomActions) {
                PageClassHelper.callUserControlFunctionSafe(this.uscCustomActionsRestId)
                    .done(function (instance) {
                    instance.loadItems(JSON.parse(fascicleTemplateModel.CustomActions));
                });
            }
        };
        uscFascicleProcessInsert.prototype.loadProcedureFascicleFields = function (fascicleTemplateModel, generateMetadataInputs) {
            var _this = this;
            if (generateMetadataInputs === void 0) { generateMetadataInputs = true; }
            var elements = [this._pnlContact(), this._pnlRoleMaster(), this._pnlConservation()];
            this.setVisibilityForManyElements(elements, true);
            this._txtObject.set_value(fascicleTemplateModel.FascicleObject);
            this.loadContacts(fascicleTemplateModel.Contacts);
            if (fascicleTemplateModel.MetadataRepository && fascicleTemplateModel.MetadataRepository.UniqueId) {
                this.loadMetadataRepository(fascicleTemplateModel.MetadataRepository.UniqueId, generateMetadataInputs);
            }
            else {
                PageClassHelper.callUserControlFunctionSafe(this.uscMetadataRepositorySelId).done(function (instance) { return instance.clearComboboxText(); });
            }
            if (fascicleTemplateModel.FascicleRoles.filter(function (x) { return x.IsMaster; })[0]) {
                PageClassHelper.callUserControlFunctionSafe(this.uscRoleMasterId).done(function (instance) { return instance.setToolbarVisibility(false); });
            }
            else {
                PageClassHelper.callUserControlFunctionSafe(this.uscCategoryId).done(function (instance) {
                    //set popup roles source
                    _this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.DossierFolder.toString(), instance.getProcessFascicleTemplateFolderId()];
                    $("#" + _this.uscRoleMasterId).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, _this.needRolesFromExternalSource_eventArgs);
                });
            }
            if (fascicleTemplateModel.Contacts.length > 0) {
                this.setFascicleContactsToSession(fascicleTemplateModel.Contacts);
                PageClassHelper.callUserControlFunctionSafe(this.uscContactId).done(function (instance) { return instance.setToolbarVisibility(false); });
            }
            this.loadRoles(fascicleTemplateModel.FascicleRoles);
            if (fascicleTemplateModel.CustomActions) {
                PageClassHelper.callUserControlFunctionSafe(this.uscCustomActionsRestId)
                    .done(function (instance) {
                    instance.loadItems(JSON.parse(fascicleTemplateModel.CustomActions));
                });
            }
        };
        uscFascicleProcessInsert.prototype.loadMetadataRepository = function (id, generateMetadataInputs) {
            PageClassHelper.callUserControlFunctionSafe(this.uscMetadataRepositorySelId).done(function (instance) { return instance.setComboboxText(id, generateMetadataInputs); });
        };
        uscFascicleProcessInsert.prototype.loadDefaultCategory = function (idCategory) {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscCategoryId).done(function (instance) {
                instance.addDefaultCategory(idCategory, true)
                    .done(function () {
                    if (!instance.getSelectedCategory()) {
                        _this.printCategoryNotFascicolable();
                    }
                })
                    .fail(function (exception) { return _this.showNotificationException(exception); });
            });
        };
        uscFascicleProcessInsert.prototype.printCategoryNotFascicolable = function () {
            this.showWarningMessage("Attenzione! Il classificatore selezionato non ha nessuna tipologia di fascicolazione associata");
        };
        uscFascicleProcessInsert.prototype.showNotificationException = function (exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                PageClassHelper.callUserControlFunctionSafe(this.uscNotificationId).done(function (instance) { return instance.showNotification(exception); });
            }
            else {
                this.showNotificationMessage(customMessage);
            }
        };
        uscFascicleProcessInsert.prototype.showNotificationMessage = function (customMessage) {
            PageClassHelper.callUserControlFunctionSafe(this.uscNotificationId).done(function (instance) { return instance.showNotificationMessage(customMessage); });
        };
        uscFascicleProcessInsert.prototype.showWarningMessage = function (customMessage) {
            PageClassHelper.callUserControlFunctionSafe(this.uscNotificationId).done(function (instance) { return instance.showWarningMessage(customMessage); });
        };
        uscFascicleProcessInsert.prototype.getCategory = function () {
            var category = new CategoryModel();
            var currentProcess = this.getCurrentProcess();
            category.Code = currentProcess ? currentProcess.Category.Code : this.getCurrentCategory().Code;
            category.Name = currentProcess ? currentProcess.Category.Name : this.getCurrentCategory().Name;
            category.EntityShortId = currentProcess ? currentProcess.Category.EntityShortId : this.getCurrentCategory().EntityShortId;
            category.IdCategory = category.EntityShortId;
            return category;
        };
        uscFascicleProcessInsert.prototype.disableFascicleTypeSelection = function () {
            this._rcbFascicleType.disable();
        };
        uscFascicleProcessInsert.prototype.getFascicleRolesToAdd = function () {
            var itemsFromSession = sessionStorage.getItem(this.clientId + "_FascicleRolesToAdd");
            if (itemsFromSession) {
                return JSON.parse(itemsFromSession);
            }
            return null;
        };
        uscFascicleProcessInsert.prototype.getFascicleContactsToAdd = function () {
            var itemsFromSession = sessionStorage.getItem(this.clientId + "_FascicleContactsToAdd");
            if (itemsFromSession) {
                return JSON.parse(itemsFromSession);
            }
            return null;
        };
        uscFascicleProcessInsert.prototype.getCurrentFascicleTemplate = function () {
            var itemFromSession = sessionStorage.getItem(this.clientId + "_CurrentFascicleTemplate");
            if (itemFromSession) {
                return JSON.parse(itemFromSession);
            }
            return null;
        };
        uscFascicleProcessInsert.prototype.getCurrentProcess = function () {
            var itemFromSession = sessionStorage.getItem(this.clientId + "_CurrentProcess");
            if (itemFromSession) {
                return JSON.parse(itemFromSession);
            }
            return null;
        };
        uscFascicleProcessInsert.prototype.getCurrentDossierFolder = function () {
            var itemFromSession = sessionStorage.getItem(this.clientId + "_CurrentDossierFolder");
            if (itemFromSession) {
                return JSON.parse(itemFromSession);
            }
            return null;
        };
        uscFascicleProcessInsert.prototype.getCurrentCategory = function () {
            var itemFromSession = sessionStorage.getItem(this.clientId + "_CurrentCategory");
            if (itemFromSession) {
                return JSON.parse(itemFromSession);
            }
            return null;
        };
        uscFascicleProcessInsert.prototype.getSelectedFascicleType = function () {
            return FascicleType[this._rcbFascicleType.get_selectedItem().get_value()];
        };
        uscFascicleProcessInsert.prototype.fillMetadataModel = function () {
            var promise = $.Deferred();
            PageClassHelper.callUserControlFunctionSafe(this.uscDynamicMetadataRestId)
                .done(function (instance) {
                var metadataRepository = instance.getMetadataRepository();
                var setiIntegrationEnabledField = false;
                if (metadataRepository && metadataRepository.JsonMetadata) {
                    var metadataJson = JSON.parse(metadataRepository.JsonMetadata);
                    setiIntegrationEnabledField = metadataJson.SETIFieldEnabled;
                }
                promise.resolve(instance.bindModelFormPage(setiIntegrationEnabledField));
            });
            return promise.promise();
        };
        uscFascicleProcessInsert.prototype.getCustomActions = function () {
            var promise = $.Deferred();
            PageClassHelper.callUserControlFunctionSafe(this.uscCustomActionsRestId)
                .done(function (instance) {
                promise.resolve(instance.getCustomActions());
            });
            return promise.promise();
        };
        uscFascicleProcessInsert.prototype.getFascicleRaciRoles = function () {
            var promise = $.Deferred();
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleId)
                .done(function (instance) {
                return promise.resolve(instance.getRaciRoles());
            });
            return promise.promise();
        };
        uscFascicleProcessInsert.prototype.getFascicleRoles = function () {
            var _this = this;
            var promise = $.Deferred();
            var fascicleRoles = this.getFascicleRolesToAdd();
            this.getFascicleRaciRoles().then(function (raciRoles) {
                var _loop_1 = function (fascicleRole) {
                    if (!fascicleRole.IsMaster && fascicleRole.AuthorizationRoleType === AuthorizationRoleType.Responsible) {
                        fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                    }
                    if (raciRoles.some(function (x) { return x.EntityShortId === fascicleRole.Role.EntityShortId; })) {
                        fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
                    }
                };
                for (var _i = 0, fascicleRoles_1 = fascicleRoles; _i < fascicleRoles_1.length; _i++) {
                    var fascicleRole = fascicleRoles_1[_i];
                    _loop_1(fascicleRole);
                }
                if (_this.selectedResponsibleRole) {
                    fascicleRoles.push({
                        Role: _this.selectedResponsibleRole,
                        IsMaster: true
                    });
                }
                return promise.resolve(fascicleRoles);
            });
            return promise.promise();
        };
        uscFascicleProcessInsert.prototype.populateInputs = function (fascicleTemplateModel) {
            var _this = this;
            this.customActionFromSession = true;
            PageClassHelper.callUserControlFunctionSafe(this.uscCategoryId).done(function (instance) {
                instance.addDefaultCategory(fascicleTemplateModel.Category.EntityShortId, false)
                    .done(function () {
                    if (fascicleTemplateModel.FascicleType === FascicleType.Activity) {
                        _this.loadActivityFascicleFields(fascicleTemplateModel, false);
                    }
                    else {
                        _this.loadProcedureFascicleFields(fascicleTemplateModel, false);
                    }
                })
                    .fail(function (exception) { return _this.showNotificationException(exception); });
            });
            if (fascicleTemplateModel.FascicleType) {
                this._rcbFascicleType.findItemByValue(FascicleType[fascicleTemplateModel.FascicleType]).select();
            }
            this._txtNote.set_value(fascicleTemplateModel.Note);
            if (fascicleTemplateModel.Conservation) {
                this._txtConservation.set_value(fascicleTemplateModel.Conservation.toString());
            }
            if (fascicleTemplateModel.MetadataDesigner && fascicleTemplateModel.MetadataValues) {
                PageClassHelper.callUserControlFunctionSafe(this.uscDynamicMetadataRestId)
                    .done(function (instance) {
                    instance.loadPageItems(fascicleTemplateModel.MetadataDesigner, fascicleTemplateModel.MetadataValues);
                });
            }
        };
        uscFascicleProcessInsert.DOSSIER_FOLDER_TYPE_NAME = "DossierFolder";
        uscFascicleProcessInsert.PROCESS_TYPE_NAME = "Process";
        uscFascicleProcessInsert.PROCESS_FASCICLE_TEMPLATE_TYPE_NAME = "ProcessFascicleTemplate";
        uscFascicleProcessInsert.LOADED_EVENT = "onLoaded";
        return uscFascicleProcessInsert;
    }());
    return uscFascicleProcessInsert;
});
//# sourceMappingURL=uscFascicleProcessInsert.js.map