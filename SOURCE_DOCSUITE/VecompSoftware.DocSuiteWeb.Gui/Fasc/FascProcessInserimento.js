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
define(["require", "exports", "./FascBase", "../App/Helpers/ServiceConfigurationHelper", "../App/Services/Dossiers/DossierFolderService", "../App/Services/Processes/ProcessService", "../App/Services/Processes/ProcessFascicleTemplateService", "../App/Models/Fascicles/FascicleModel", "../App/Models/Commons/CategoryModel", "UserControl/uscMetadataRepositorySel", "../App/Models/Fascicles/FascicleRoleModel", "../App/Services/Fascicles/FascicleService", "../App/Models/Fascicles/FascicleType", "../App/Models/Commons/MetadataRepositoryModel"], function (require, exports, FascBase, ServiceConfigurationHelper, DossierFolderService, ProcessService, ProcessFascicleTemplateService, FascicleModel, CategoryModel, UscMetadataRepositorySel, FascicleRoleModel, FascicleService, FascicleType, MetadataRepositoryModel) {
    var FascProcessInserimento = /** @class */ (function (_super) {
        __extends(FascProcessInserimento, _super);
        function FascProcessInserimento(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME)) || this;
            _this._ddlProcess_OnClientSelectedIndexChanged = function (sender, args) {
                _this._loadingPanel.show(_this.pnlFascProcessInsertId);
                _this._rtvProcessFolders.get_nodes().clear();
                _this._currentProcess = _this._processes.filter(function (x) { return x.UniqueId === args.get_item().get_value(); })[0];
                _this._dossierFolderService.getProcessFolders(null, _this._ddlProcess.get_value(), false, false, function (dossierFolders) {
                    _this._dossierFolders = dossierFolders;
                    var node = null;
                    if (dossierFolders.length < 2) {
                        _this._rowDossierFolders.hide();
                    }
                    else {
                        _this._rowDossierFolders.show();
                    }
                    _this._rowDossierFolders.show();
                    for (var _i = 0, dossierFolders_1 = dossierFolders; _i < dossierFolders_1.length; _i++) {
                        var dossierFolder = dossierFolders_1[_i];
                        _this._rtvProcessFolders.trackChanges();
                        node = new Telerik.Web.UI.RadTreeNode();
                        node.set_text(dossierFolder.Name);
                        node.set_value(dossierFolder.UniqueId);
                        node.set_imageUrl('../App_Themes/DocSuite2008/imgset16/fascicle_open.png');
                        _this._rtvProcessFolders.get_nodes().add(node);
                        _this.fillDossiersRecursive(dossierFolder.DossierFolders, node);
                    }
                    var firstNode = _this._rtvProcessFolders.get_nodes().getNode(0);
                    firstNode.set_selected(true);
                    _this.loadDossier(firstNode.get_value());
                    _this.loadProcessTemplates(firstNode.get_value());
                    _this.loadCategory(_this._currentProcess.Category);
                    _this._rtvProcessFolders.commitChanges();
                    _this.clearFascicleFields();
                    _this._loadingPanel.hide(_this.pnlFascProcessInsertId);
                }, function (exception) {
                });
            };
            _this._rtvProcessFolders_OnNodeClicked = function (sender, args) {
                if (!args.get_node()) {
                    return;
                }
                _this.loadDossier(args.get_node().get_value());
                _this.loadProcessTemplates(args.get_node().get_value());
            };
            _this._ddlTemplate_OnClientSelectedIndexChanged = function (sender, args) {
                if (!args.get_item()) {
                    return;
                }
                _this._currentFascicleTemplate = _this._processFascicleTemplates.filter(function (x) { return x.UniqueId === args.get_item().get_value(); })[0];
                _this.loadFascicleFields(_this._currentFascicleTemplate.JsonModel);
            };
            _this._btnInsert_OnClick = function (sender, args) {
                if (!Page_IsValid) {
                    args.set_cancel(true);
                    return;
                }
                _this._btnInsert.set_enabled(false);
                _this._fascicleModel.Category = _this.getCategory();
                _this._fascicleModel.FascicleObject = _this._txtObject.get_textBoxValue();
                _this._fascicleModel.Note = _this._txtNote.get_textBoxValue();
                _this._fascicleModel.StartDate = _this._radStartDate.get_selectedDate();
                _this._fascicleModel.Conservation = parseInt(_this._txtConservation.get_textBoxValue());
                _this._fascicleModel.FascicleType = _this._currentProcess.FascicleType;
                var dossierFolders = [];
                dossierFolders.push(_this._currentDossierFolder);
                _this._fascicleModel.DossierFolders = dossierFolders;
                _this._fascicleModel.FascicleTemplate = _this._currentFascicleTemplate;
                _this._loadingPanel.show(_this.pnlFascProcessInsertId);
                var ajaxModel = {};
                ajaxModel.Value = new Array();
                ajaxModel.ActionName = "Insert";
                _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                args.set_cancel(true);
            };
            _this.deleteContactFromModelPromise = function (contactIdToDelete) {
                var promise = $.Deferred();
                if (!contactIdToDelete)
                    return promise.promise();
                var contactParentId = _this._fascicleModel.Contacts.filter(function (x) { return x.EntityId === contactIdToDelete; })[0].IncrementalFather;
                _this._fascicleModel.Contacts = _this._fascicleModel.Contacts.filter(function (x) { return x.EntityId !== contactIdToDelete; });
                return promise.resolve(contactParentId);
            };
            _this.addContactToModelPromise = function (newAddedContact) {
                var promise = $.Deferred();
                if (!newAddedContact)
                    return promise.promise();
                _this._fascicleModel.Contacts.push(newAddedContact);
                return promise.resolve(newAddedContact);
            };
            _this.deleteRoleFromModelPromise = function (roleIdToDelete) {
                var promise = $.Deferred();
                if (!roleIdToDelete)
                    return promise.promise();
                _this._fascicleModel.FascicleRoles = _this._fascicleModel.FascicleRoles.filter(function (x) { return x.Role.IdRole !== roleIdToDelete && x.Role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1; });
                return promise.resolve();
            };
            _this.addRoleToModelPromise = function (newAddedRoles) {
                return _this.addRole(newAddedRoles, false);
            };
            _this.addRoleMasterToModelPromise = function (newAddedRoles) {
                return _this.addRole(newAddedRoles, true);
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        FascProcessInserimento.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascProcessInserimento.DOSSIER_FOLDER_TYPE_NAME);
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            var processConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascProcessInserimento.PROCESS_TYPE_NAME);
            this._processService = new ProcessService(processConfiguration);
            var processFascicleTemplateConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascProcessInserimento.PROCESS_FASCICLE_TEMPLATE_TYPE_NAME);
            this._processFascicleTemplateService = new ProcessFascicleTemplateService(processFascicleTemplateConfiguration);
            var fascicleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLE_TYPE_NAME);
            this._fascicleService = new FascicleService(fascicleConfiguration);
            this._ddlProcess = $find(this.ddlProcessId);
            this._ddlProcess.add_selectedIndexChanged(this._ddlProcess_OnClientSelectedIndexChanged);
            this._rtvProcessFolders = $find(this.rtvProcessFoldersId);
            this._rtvProcessFolders.add_nodeClicked(this._rtvProcessFolders_OnNodeClicked);
            this._ddlTemplate = $find(this.ddlTemplateId);
            this._ddlTemplate.add_selectedIndexChanged(this._ddlTemplate_OnClientSelectedIndexChanged);
            this._rowTemplate = $('#'.concat(this.rowTemplateId));
            this._rowDossierFolders = $('#'.concat(this.rowDossierFoldersId));
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._uscCategoryRest = $("#".concat(this.uscCategoryId)).data();
            this._uscCategoryRest.setFascicleTypeParam(FascicleType.Procedure);
            this._uscMetadataRepository = $("#".concat(this.uscMetadataRepositorySelId)).data();
            this._uscDynamicMetadata = $("#".concat(this.uscDynamicMetadataId)).data();
            this._uscContact = $("#".concat(this.uscContactId)).data();
            this._uscRoleMaster = $("#".concat(this.uscRoleMasterId)).data();
            this._uscRole = $("#".concat(this.uscRoleId)).data();
            this._txtObject = $find(this.txtObjectId);
            this._txtConservation = $find(this.txtConservationId);
            this._txtNote = $find(this.txtNoteId);
            this._radStartDate = $find(this.radStartDateId);
            this._btnInsert = $find(this.btnInsertId);
            this._btnInsert.add_clicking(this._btnInsert_OnClick);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._fascicleModel = new FascicleModel();
            this._fascicleModel.FascicleRoles = [];
            this._fascicleModel.Contacts = [];
            this._rowDossierFolders.hide();
            this._rowTemplate.hide();
            this._uscRoleMaster.renderRolesTree([]);
            this._uscRole.renderRolesTree([]);
            this._uscContact.renderContactsTree([]);
            this.loadProcesses();
            this.registerUscRoleRestEventHandlers();
            this.registerUscContactRestEventHandlers();
            this.setMetadataRepositorySelectedIndexEvent();
        };
        FascProcessInserimento.prototype.insertCallback = function (metadataModel) {
            var _this = this;
            this._fascicleModel.MetadataValues = metadataModel;
            if (sessionStorage.getItem("MetadataRepository")) {
                var metadataRepository = new MetadataRepositoryModel();
                metadataRepository.UniqueId = sessionStorage.getItem("MetadataRepository");
                this._fascicleModel.MetadataRepository = metadataRepository;
            }
            this._fascicleService.insertFascicle(this._fascicleModel, function (data) {
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=" + data.UniqueId;
            }, function (exception) {
                _this._loadingPanel.hide(_this.pnlFascProcessInsertId);
                _this._btnInsert.set_enabled(true);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascProcessInserimento.prototype.loadDossier = function (uniqueId) {
            this._currentDossierFolder = this._dossierFolders.filter(function (x) { return x.UniqueId == uniqueId; })[0];
            this.clearFascicleFields();
            this._ddlTemplate.set_enabled(true);
            this._ddlTemplate.get_items().clear();
            this._ddlTemplate.clearSelection();
            this._ddlTemplate.enable();
        };
        FascProcessInserimento.prototype.loadProcesses = function () {
            var _this = this;
            this._loadingPanel.show(this.pnlFascProcessInsertId);
            this._processService.getAvailableProcesses(null, false, null, null, function (processes) {
                _this._processes = processes;
                var today = new Date();
                var cmbItem = null;
                processes = processes.filter(function (x) { return new Date(x.StartDate) < today && new Date(x.EndDate) > today; });
                for (var _i = 0, processes_1 = processes; _i < processes_1.length; _i++) {
                    var process = processes_1[_i];
                    cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(process.Name);
                    cmbItem.set_value(process.UniqueId);
                    _this._ddlProcess.get_items().add(cmbItem);
                    if (processes.length == 1) {
                        var selectedItem = _this._ddlProcess.findItemByValue(process.UniqueId);
                        selectedItem.select();
                        _this._ddlProcess.disable();
                    }
                    ;
                }
                _this._loadingPanel.hide(_this.pnlFascProcessInsertId);
            }, function (exception) {
            });
        };
        FascProcessInserimento.prototype.fillDossiersRecursive = function (dossierFolders, currentNode) {
            var dossierChildNode = null;
            for (var _i = 0, dossierFolders_2 = dossierFolders; _i < dossierFolders_2.length; _i++) {
                var dossierFolder = dossierFolders_2[_i];
                dossierChildNode = new Telerik.Web.UI.RadTreeNode();
                dossierChildNode.set_text(dossierFolder.Name);
                dossierChildNode.set_value(dossierFolder.UniqueId);
                dossierChildNode.set_imageUrl('../App_Themes/DocSuite2008/imgset16/fascicle_open.png');
                currentNode.get_nodes().add(dossierChildNode);
                if (dossierFolder.DossierFolders.length >= 1) {
                    this.fillDossiersRecursive(dossierFolder.DossierFolders, dossierChildNode);
                }
            }
        };
        FascProcessInserimento.prototype.loadProcessTemplates = function (dossierFolderId) {
            var _this = this;
            this._processFascicleTemplateService.getFascicleTemplateByDossierFolderId(dossierFolderId, function (processFascicleTemplates) {
                _this._processFascicleTemplates = processFascicleTemplates;
                var cmbItem = null;
                if (processFascicleTemplates.length === 0) {
                    _this._rowTemplate.hide();
                }
                else {
                    _this._rowTemplate.show();
                }
                for (var _i = 0, processFascicleTemplates_1 = processFascicleTemplates; _i < processFascicleTemplates_1.length; _i++) {
                    var template = processFascicleTemplates_1[_i];
                    cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(template.Name);
                    cmbItem.set_value(template.UniqueId);
                    _this._ddlTemplate.get_items().add(cmbItem);
                    if (processFascicleTemplates.length == 1) {
                        var selectedItem = _this._ddlTemplate.findItemByValue(template.UniqueId);
                        selectedItem.select();
                        _this._ddlTemplate.disable();
                    }
                    ;
                }
            }, function (exception) {
            });
        };
        FascProcessInserimento.prototype.loadFascicleFields = function (jsonModel) {
            var fascicleModel = new FascicleModel();
            try {
                fascicleModel = JSON.parse(jsonModel);
                this._txtObject.set_value(fascicleModel.FascicleObject);
                this.loadContacts(fascicleModel.Contacts);
                if (fascicleModel.MetadataRepository) {
                    this.loadMetadataRepository(fascicleModel.MetadataRepository.UniqueId);
                }
                else {
                    this._uscMetadataRepository.clearComboboxText();
                }
                if (fascicleModel.FascicleRoles.filter(function (x) { return x.IsMaster; })[0]) {
                    this._uscRoleMaster.setToolbarVisibility(false);
                }
                if (fascicleModel.FascicleRoles.filter(function (x) { return !x.IsMaster; })[0]) {
                    this._uscRole.setToolbarVisibility(false);
                }
                if (fascicleModel.Contacts.length > 0) {
                    this._uscContact.setToolbarVisibility(false);
                }
                this.loadRoles(fascicleModel.FascicleRoles);
                this._fascicleModel = fascicleModel;
            }
            catch (e) {
                this.clearFascicleFields();
                return;
            }
        };
        FascProcessInserimento.prototype.loadMetadataRepository = function (id) {
            this._uscMetadataRepository.setComboboxText(id);
        };
        FascProcessInserimento.prototype.loadContacts = function (contactsModel) {
            this._uscContact.renderContactsTree(contactsModel);
        };
        FascProcessInserimento.prototype.loadCategory = function (categoryModel) {
            if (!categoryModel) {
                return;
            }
            this._uscCategoryRest.addDefaultNode(categoryModel);
            this._uscCategoryRest.disableButtons();
        };
        FascProcessInserimento.prototype.loadRoles = function (fascicleRoles) {
            var rolesModel = [];
            var masterRolesModel = [];
            for (var _i = 0, fascicleRoles_1 = fascicleRoles; _i < fascicleRoles_1.length; _i++) {
                var fascicleRole = fascicleRoles_1[_i];
                if (fascicleRole.IsMaster) {
                    masterRolesModel.push(fascicleRole.Role);
                }
                else {
                    rolesModel.push(fascicleRole.Role);
                }
            }
            this._uscRoleMaster.renderRolesTree(masterRolesModel);
            this._uscRole.renderRolesTree(rolesModel);
        };
        FascProcessInserimento.prototype.clearFascicleFields = function () {
            this._txtObject.clear();
            var fascicleRoles = [];
            var contacts = [];
            this.loadRoles(fascicleRoles); //pass empty array so that usc sets no roles
            this._uscRole.setToolbarVisibility(true);
            this._uscRoleMaster.setToolbarVisibility(true);
            this.loadContacts(contacts); //pass empty array so that usc sets no contacts
            this._uscContact.setToolbarVisibility(true);
            this._uscMetadataRepository.clearComboboxText();
            this._uscDynamicMetadata.loadDynamicMetadata("");
        };
        FascProcessInserimento.prototype.registerUscRoleRestEventHandlers = function () {
            var uscRoleRestEvents = this._uscRole.uscRoleRestEvents;
            this._uscRole.registerEventHandler(uscRoleRestEvents.RoleDeleted, this.deleteRoleFromModelPromise);
            this._uscRole.registerEventHandler(uscRoleRestEvents.NewRolesAdded, this.addRoleToModelPromise);
            this._uscRoleMaster.registerEventHandler(uscRoleRestEvents.RoleDeleted, this.deleteRoleFromModelPromise);
            this._uscRoleMaster.registerEventHandler(uscRoleRestEvents.NewRolesAdded, this.addRoleMasterToModelPromise);
        };
        FascProcessInserimento.prototype.registerUscContactRestEventHandlers = function () {
            var uscContactRestEvents = this._uscContact.uscContattiSelRestEvents;
            this._uscContact.registerEventHandler(uscContactRestEvents.ContactDeleted, this.deleteContactFromModelPromise);
            this._uscContact.registerEventHandler(uscContactRestEvents.NewContactsAdded, this.addContactToModelPromise);
        };
        FascProcessInserimento.prototype.addRole = function (newAddedRoles, isMaster) {
            var promise = $.Deferred();
            if (!newAddedRoles.length)
                return promise.promise();
            for (var _i = 0, newAddedRoles_1 = newAddedRoles; _i < newAddedRoles_1.length; _i++) {
                var newAddedRole = newAddedRoles_1[_i];
                var fascicleRole = new FascicleRoleModel();
                fascicleRole.IsMaster = isMaster;
                fascicleRole.Role = newAddedRole;
                this._fascicleModel.FascicleRoles.push(fascicleRole);
            }
            return promise.resolve();
        };
        FascProcessInserimento.prototype.getCategory = function () {
            var category = new CategoryModel;
            category.Code = this._currentProcess.Category.Code;
            category.Name = this._currentProcess.Category.Name;
            category.EntityShortId = this._currentProcess.Category.IdCategory;
            return category;
        };
        FascProcessInserimento.prototype.setMetadataRepositorySelectedIndexEvent = function () {
            var _this = this;
            $("#".concat(this.uscMetadataRepositorySelId)).off(UscMetadataRepositorySel.SELECTED_INDEX_EVENT);
            $("#".concat(this.uscMetadataRepositorySelId)).on(UscMetadataRepositorySel.SELECTED_INDEX_EVENT, function (args, data) {
                if (!jQuery.isEmptyObject(_this._uscDynamicMetadata)) {
                    setTimeout(function () {
                        _this._uscDynamicMetadata.loadDynamicMetadata(data);
                    }, 500);
                }
            });
        };
        FascProcessInserimento.DOSSIER_FOLDER_TYPE_NAME = "DossierFolder";
        FascProcessInserimento.PROCESS_TYPE_NAME = "Process";
        FascProcessInserimento.PROCESS_FASCICLE_TEMPLATE_TYPE_NAME = "ProcessFascicleTemplate";
        return FascProcessInserimento;
    }(FascBase));
    return FascProcessInserimento;
});
//# sourceMappingURL=FascProcessInserimento.js.map