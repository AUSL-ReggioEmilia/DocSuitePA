var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
define(["require", "exports", "App/Helpers/EnumHelper", "App/Services/Processes/ProcessService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Processes/ProcessNodeType", "App/Services/Dossiers/DossierFolderService", "UserControl/uscProcessDetails", "App/Models/Dossiers/DossierFolderStatus", "App/Models/Commons/CategoryModel", "App/Models/Fascicles/FascicleType", "App/Services/Dossiers/DossierService", "App/Services/Processes/ProcessFascicleTemplateService", "App/DTOs/ExceptionDTO", "App/Models/Processes/ProcessType"], function (require, exports, EnumHelper, ProcessService, ServiceConfigurationHelper, ProcessNodeType, DossierFolderService, uscProcessDetails, DossierFolderStatus, CategoryModel, FascicleType, DossierService, ProcessFascicleTemplateService, ExceptionDTO, ProcessType) {
    var TbltProcess = /** @class */ (function () {
        function TbltProcess(serviceConfigurations) {
            var _this = this;
            this.deleteRolePromise = function (roleIdToDelete, senderId) {
                var promise = $.Deferred();
                if (!roleIdToDelete)
                    return promise.promise();
                _this.processRoles = _this.processRoles
                    .filter(function (role) { return role.IdRole !== roleIdToDelete || role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1; });
                promise.resolve(_this.processRoles);
                return promise.promise();
            };
            this.updateRolesPromise = function (newAddedRoles, senderId) {
                var promise = $.Deferred();
                if (!newAddedRoles.length)
                    return promise.promise();
                _this.processRoles = __spreadArrays(_this.processRoles, newAddedRoles);
                promise.resolve(_this.processRoles);
                return promise.promise();
            };
            this.rtvProcess_onExpand = function (sender, args) {
                var expandedNode = args.get_node();
                if (expandedNode.get_level() === 1) {
                    _this.expandNodeLogic(expandedNode);
                    _this.loadData(expandedNode.get_attributes().getAttribute("idDossier"), 0, expandedNode.get_value());
                }
                else {
                    if (expandedNode.get_nodes().getNode(0).get_text() === "") {
                        expandedNode.get_nodes().clear();
                        _this.loadData(expandedNode.get_value(), 0);
                    }
                    else {
                        for (var index = 0; index < expandedNode.get_nodes().get_count(); index++) {
                            _this.expandNodeLogic(expandedNode.get_nodes().getNode(index));
                        }
                    }
                }
            };
            this.rtvProcesses_nodeClicked = function (sender, args) {
                var selectedNode = args.get_node();
                _this.initializeNodeClicked(selectedNode);
                if (selectedNode.get_level() === 0) {
                    _this._folderToolBar.findItemByValue("create").set_enabled(true);
                    $("#" + _this._uscProcessDetails.pnlDetailsId).hide();
                    _this._ajaxLoadingPanel.hide("ItemDetailTable");
                }
                else {
                    switch (selectedNode.get_attributes().getAttribute("NodeType")) {
                        case ProcessNodeType.Process: {
                            _this.initializeProcessNodeDetails(selectedNode);
                            break;
                        }
                        case ProcessNodeType.DossierFolder: {
                            _this.initializeDossierFolderNodeDetails(selectedNode);
                            break;
                        }
                        case ProcessNodeType.ProcessFascicleTemplate: {
                            _this.initializeProcessFascicleTemplateNodeDetails(selectedNode);
                            break;
                        }
                    }
                }
            };
            this.rbProcessInsert_onCLick = function (sender, args) {
                _this.hideInsertInputs();
                var selectedNode = _this._rtvProcesses.get_selectedNode();
                _this.clearInputs();
                if (selectedNode.get_level() === 0) {
                    _this.selectedProcessId = "";
                    $("#insertProcess").show();
                }
                else {
                    _this.selectedProcessFascicleTemplateId = "";
                    $("#insertFascicleTemplate").show();
                }
                _this.processRoles = [];
                _this._rwInsert.show();
            };
            this.folderToolBar_onClick = function (sender, args) {
                _this.hideInsertInputs();
                switch (args.get_item().get_value()) {
                    case "create": {
                        _this.clearInputs();
                        var selectedNode = _this._rtvProcesses.get_selectedNode();
                        if (selectedNode.get_level() === 0) {
                            _this.selectedProcessId = "";
                            _this._uscCategoryRest.enableButtons();
                            $("#insertProcess").show();
                            _this._rwInsert.set_title("Aggiungi procedimento");
                        }
                        switch (selectedNode.get_attributes().getAttribute("NodeType")) {
                            case ProcessNodeType.Process:
                            case ProcessNodeType.DossierFolder: {
                                _this.selectedDossierFolderId = "";
                                $("#insertDossierFolder").show();
                                _this._rwInsert.set_title("Aggiungi cartella di dossier");
                                break;
                            }
                        }
                        _this._rwInsert.show();
                        break;
                    }
                    case "createProcessFascicleTemplate": {
                        _this.selectedProcessFascicleTemplateId = "";
                        $("#insertFascicleTemplate").show();
                        _this._rwInsert.set_title("Aggiungi modello di fascicolo di processo");
                        _this._rwInsert.show();
                        break;
                    }
                    case "delete": {
                        var selectedNode = _this._rtvProcesses.get_selectedNode();
                        switch (selectedNode.get_attributes().getAttribute("NodeType")) {
                            case ProcessNodeType.Process: {
                                var yesterdayDate = new Date();
                                yesterdayDate.setDate(new Date().getDate() - 1);
                                _this._ajaxLoadingPanel.show(_this.rtvProcessesId);
                                _this.removeProcess(yesterdayDate);
                                break;
                            }
                            case ProcessNodeType.DossierFolder: {
                                if (uscProcessDetails.processFascicleWorkflowRepositories.length > 0) {
                                    alert("Impossibile eliminare la cartella. Esiste un flusso di lavoro associato.");
                                    return;
                                }
                                var dossierFolder = {};
                                dossierFolder.UniqueId = _this._rtvProcesses.get_selectedNode().get_value();
                                if (_this._rtvProcesses.get_selectedNode().get_level() === 2) {
                                    dossierFolder.ParentInsertId = _this.getProcessNodeByChild(_this._rtvProcesses.get_selectedNode()).get_attributes().getAttribute("idDossier");
                                }
                                else {
                                    dossierFolder.ParentInsertId = _this._rtvProcesses.get_selectedNode().get_parent().get_value();
                                }
                                _this._ajaxLoadingPanel.show(_this.rtvProcessesId);
                                _this._dossierFolderService.deleteDossierFolder(dossierFolder, function (data) {
                                    _this._rtvProcesses.get_selectedNode().get_parent().get_nodes().remove(_this._rtvProcesses.get_selectedNode());
                                    _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                                }, function (error) {
                                    _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                                    _this.showNotificationException(error);
                                });
                                break;
                            }
                            case ProcessNodeType.ProcessFascicleTemplate: {
                                var yesterdayDate = new Date();
                                yesterdayDate.setDate(new Date().getDate() - 1);
                                _this._ajaxLoadingPanel.show(_this.rtvProcessesId);
                                _this.removeFascicleTemaple(yesterdayDate);
                                break;
                            }
                        }
                        break;
                    }
                    case "modify": {
                        _this.hideInsertInputs();
                        var selectedNode = _this._rtvProcesses.get_selectedNode();
                        switch (selectedNode.get_attributes().getAttribute("NodeType")) {
                            case ProcessNodeType.Process: {
                                _this.selectedProcessId = _this._rtvProcesses.get_selectedNode().get_value();
                                _this.populateProcessInputs(_this.selectedProcessId);
                                for (var _i = 0, _a = _this.processesModel; _i < _a.length; _i++) {
                                    var processToFind = _a[_i];
                                    if (processToFind.UniqueId === _this.selectedProcessId) {
                                        _this.processRoles = processToFind.Roles;
                                        _this._uscProcessRoleRest.renderRolesTree(processToFind.Roles);
                                        break;
                                    }
                                }
                                _this._uscCategoryRest.disableButtons();
                                $("#insertProcess").show();
                                _this._rwInsert.set_title("Modifica procedimento");
                                break;
                            }
                            case ProcessNodeType.ProcessFascicleTemplate: {
                                _this.selectedProcessFascicleTemplateId = _this._rtvProcesses.get_selectedNode().get_value();
                                _this.populateProcessFascicleTemplateInputs(_this.selectedProcessFascicleTemplateId);
                                $("#insertFascicleTemplate").show();
                                _this._rwInsert.set_title("Modifica modello di fascicolo di processo");
                                break;
                            }
                        }
                        _this._rwInsert.show();
                        break;
                    }
                }
            };
            this.rbConfirmInsert_onCLick = function (sender, args) {
                _this._ajaxLoadingPanel.show(_this.rtvProcessesId);
                var selectedNode = _this._rtvProcesses.get_selectedNode();
                if ($("#insertDossierFolder").is(":visible")) {
                    if (_this._rtbDossierFolderName.get_textBoxValue()) {
                        var exists = _this.selectedDossierFolderId !== "";
                        var dossierFolder = {};
                        dossierFolder.Name = _this._rtbDossierFolderName.get_textBoxValue();
                        if (selectedNode.get_attributes().getAttribute("NodeType") !== ProcessNodeType.Process) {
                            dossierFolder.ParentInsertId = exists ? selectedNode.get_parent().get_value() : selectedNode.get_value();
                        }
                        else {
                            dossierFolder.ParentInsertId = _this.getProcessNodeByChild(selectedNode).get_attributes().getAttribute("idDossier");
                        }
                        dossierFolder.Dossier = {};
                        dossierFolder.Dossier.UniqueId = _this.getProcessNodeByChild(selectedNode).get_attributes().getAttribute("idDossier");
                        if (exists) {
                            dossierFolder.UniqueId = _this.selectedDossierFolderId;
                            _this._dossierFolderService.updateDossierFolder(dossierFolder, null, function (data) {
                                _this.createNode(selectedNode, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png", ProcessNodeType.DossierFolder, false, data.Status, null, null, null);
                                _this._rwInsert.close();
                                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                            }, function (error) {
                                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                                _this.showNotificationException(error);
                            });
                        }
                        else {
                            _this._dossierFolderService.insertDossierFolder(dossierFolder, null, function (data) {
                                var node = new Telerik.Web.UI.RadTreeNode();
                                if (selectedNode.get_nodes().get_count() > 0 && selectedNode.get_nodes().getNode(0).get_text() === "") {
                                    selectedNode.get_nodes().clear();
                                }
                                _this.createNode(node, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png", ProcessNodeType.DossierFolder, true, data.Status, null, null, selectedNode);
                                _this._rwInsert.close();
                                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                            }, function (error) {
                                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                                _this.showNotificationException(error);
                            });
                        }
                    }
                }
                if ($("#insertProcess").is(":visible")) {
                    var exists = _this.selectedProcessId !== "";
                    var process = {};
                    process.Name = _this._rtbProcessName.get_textBoxValue();
                    process.Dossier = {};
                    process.Dossier.UniqueId = _this.getProcessNodeByChild(selectedNode).get_value();
                    process.Category = new CategoryModel();
                    process.Category.EntityShortId = _this._uscCategoryRest.getSelectedCategory().EntityShortId;
                    process.FascicleType = _this._enumHelper.getFascicleType(_this._rcbFascicleType.get_selectedItem().get_text());
                    process.StartDate = new Date();
                    process.EndDate = null;
                    process.Dossier = {};
                    process.Roles = _this.processRoles;
                    process.Note = _this._rcbProcessNote.get_value();
                    if (exists) {
                        process.UniqueId = _this.selectedProcessId;
                        _this._processService.update(process, function (data) {
                            var isActive = data.EndDate === null || new Date(data.EndDate) > new Date();
                            _this.createNode(selectedNode, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png", ProcessNodeType.Process, false, null, isActive, data.Dossier.UniqueId, null);
                            _this._rwInsert.close();
                            _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                            _this._ajaxLoadingPanel.show("ItemDetailTable");
                            _this._uscProcessDetails = $("#" + _this.uscProcessDetailsId).data();
                            _this._uscProcessDetails.clearProcessDetails();
                            uscProcessDetails.selectedProcessId = data.UniqueId;
                            _this._uscProcessDetails.setProcessDetails();
                            _this._ajaxLoadingPanel.hide("ItemDetailTable");
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                            _this.showNotificationException(error);
                        });
                    }
                    else {
                        var process_1 = {};
                        process_1.Name = _this._rtbProcessName.get_value();
                        process_1.Category = new CategoryModel();
                        process_1.Category.EntityShortId = _this._uscCategoryRest.getSelectedCategory().EntityShortId;
                        process_1.FascicleType = _this._enumHelper.getFascicleType(_this._rcbFascicleType.get_selectedItem().get_text());
                        process_1.StartDate = new Date();
                        process_1.Dossier = {};
                        process_1.Roles = _this.processRoles;
                        process_1.Note = _this._rcbProcessNote.get_value();
                        _this._processService.insert(process_1, function (data) {
                            var isActive = data.EndDate === null || new Date(data.EndDate) > new Date();
                            var node = new Telerik.Web.UI.RadTreeNode();
                            _this.createNode(node, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png", ProcessNodeType.Process, true, null, isActive, data.Dossier.UniqueId, selectedNode);
                            _this.processesModel.push(data);
                            _this._rwInsert.close();
                            _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                            _this.showNotificationException(error);
                        });
                    }
                }
                if ($("#insertFascicleTemplate").is(":visible")) {
                    var exists = _this.selectedProcessFascicleTemplateId !== "";
                    var fascicleTemplate = {};
                    fascicleTemplate.Name = _this._rtbFascicleTemplateName.get_value();
                    fascicleTemplate.StartDate = new Date();
                    fascicleTemplate.EndDate = null;
                    fascicleTemplate.Process = {};
                    fascicleTemplate.Process.UniqueId = _this.getProcessNodeByChild(selectedNode).get_value();
                    fascicleTemplate.DossierFolder = {};
                    fascicleTemplate.DossierFolder.UniqueId = selectedNode.get_value();
                    fascicleTemplate.JsonModel = "";
                    var imageUrl_1;
                    if (fascicleTemplate.EndDate === null || new Date(fascicleTemplate.EndDate) < new Date()) {
                        imageUrl_1 = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
                    }
                    else {
                        imageUrl_1 = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
                    }
                    if (exists) {
                        fascicleTemplate.UniqueId = _this.selectedProcessFascicleTemplateId;
                        _this._fascicleTemplateService.update(fascicleTemplate, function (data) {
                            _this.createNode(selectedNode, data.Name, data.UniqueId, imageUrl_1, ProcessNodeType.ProcessFascicleTemplate, false, null, null, null, null);
                            _this._rwInsert.close();
                            _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                            _this.showNotificationException(error);
                        });
                    }
                    else {
                        _this._fascicleTemplateService.insert(fascicleTemplate, function (data) {
                            var node = new Telerik.Web.UI.RadTreeNode();
                            _this.createNode(node, data.Name, data.UniqueId, imageUrl_1, ProcessNodeType.ProcessFascicleTemplate, true, null, null, null, selectedNode);
                            _this._rwInsert.close();
                            _this.processFascicleTemplatesModel.push(data);
                            _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                        }, function (error) {
                            _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                            _this.showNotificationException(error);
                        });
                    }
                }
            };
            this.filterToolbar_onClick = function (sender, args) {
                switch (args.get_item().get_value()) {
                    case "search": {
                        _this._ajaxLoadingPanel.show(_this.processViewPaneId);
                        _this._uscProcessDetails = $("#" + _this.uscProcessDetailsId).data();
                        _this._uscProcessDetails.clearProcessDetails();
                        $("#" + _this._uscProcessDetails.pnlDetailsId).hide();
                        _this._rtvProcesses.get_nodes().getNode(0).get_nodes().clear();
                        var processSearchName = _this._rtbProcessSearchName.get_textBoxValue();
                        var fascicleType = "";
                        if (_this._rcbFascicleType.get_selectedItem()) {
                            fascicleType = FascicleType[_this._enumHelper.getFascicleType(_this._rcbFascicleType.get_selectedItem().get_text())];
                        }
                        var processActiveItem = _this._filterToolbar.findItemByValue("processActive");
                        var processDisabledItem = _this._filterToolbar.findItemByValue("processDisabled");
                        var callLoadProcesses = processActiveItem.get_checked()
                            ? processDisabledItem.get_checked() ? _this.loadProcesses(processSearchName, fascicleType, null) : _this.loadProcesses(processSearchName, fascicleType, true)
                            : processDisabledItem.get_checked() ? _this.loadProcesses(processSearchName, fascicleType, false) : _this.loadProcesses(processSearchName, fascicleType, null);
                        break;
                    }
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        TbltProcess.prototype.initialize = function () {
            this.initializeServices();
            this.initializeControls();
            this.initializeUserControls();
            this._ajaxLoadingPanel.show(this.processViewPaneId);
            this.enableFolderToolbarButtons(false);
            this._folderToolBar.findItemByValue("create").set_enabled(true);
            this.loadFascicleTypes();
            this.loadProcesses("", "", true);
            this.processFascicleTemplatesModel = [];
            this.processRoles = [];
        };
        TbltProcess.prototype.initializeServices = function () {
            var processConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.Process_TYPE_NAME);
            this._processService = new ProcessService(processConfiguration);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.DossierFolder_TYPE_NAME);
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            var dossierConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.Dossier_TYPE_NAME);
            this._dossierService = new DossierService(dossierConfiguration);
            var fascicleTemplateConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.ProcessFascicleTemplate_TYPE_NAME);
            this._fascicleTemplateService = new ProcessFascicleTemplateService(fascicleTemplateConfiguration);
        };
        TbltProcess.prototype.initializeControls = function () {
            this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId);
            this._rtvProcesses = $find(this.rtvProcessesId);
            this._rtvProcesses.add_nodeClicked(this.rtvProcesses_nodeClicked);
            this._rtvProcesses.get_nodes().getNode(0).get_attributes().setAttribute("NodeType", ProcessNodeType.Root);
            this._rtvProcesses.add_nodeExpanded(this.rtvProcess_onExpand);
            this._rwInsert = $find(this.rwInsertId);
            this._folderToolBar = $find(this.folderToolBarId);
            this._folderToolBar.add_buttonClicked(this.folderToolBar_onClick);
            this._rtbProcessName = $find(this.rtbProcessNameId);
            this._rcbFascicleType = $find(this.rcbFascicleTypeId);
            this._rtbDossierFolderName = $find(this.rtbDossierFolderNameId);
            this._rtbFascicleTemplateName = $find(this.rtbFascicleTemplateNameId);
            this._rbConfirm = $find(this.rbConfirmId);
            this._rbConfirm.add_clicked(this.rbConfirmInsert_onCLick);
            this._rcbProcessNote = $find(this.rcbProcessNoteId);
            this._filterToolbar = $find(this.filterToolbarId);
            this._filterToolbar.add_buttonClicked(this.filterToolbar_onClick);
            this._rcbProcessStatus = this._filterToolbar.findItemByValue("statusInput").findControl("rcbProcessStatus");
            this._rtbProcessSearchName = this._filterToolbar.findItemByValue("searchInput").findControl("txtSearch");
        };
        TbltProcess.prototype.initializeUserControls = function () {
            this._uscCategoryRest = $("#" + this.uscCategoryRestId).data();
            this._uscProcessRoleRest = $("#" + this.uscProcessRoleRestId).data();
            this._uscProcessRoleRest.renderRolesTree([]);
            this.registerUscRoleRestEventHandlers();
        };
        TbltProcess.prototype.registerUscRoleRestEventHandlers = function () {
            var uscRoleRestEventsDictionary = this._uscProcessRoleRest.uscRoleRestEvents;
            this._uscProcessRoleRest.registerEventHandler(uscRoleRestEventsDictionary.RoleDeleted, this.deleteRolePromise, this.uscProcessRoleRestId);
            this._uscProcessRoleRest.registerEventHandler(uscRoleRestEventsDictionary.NewRolesAdded, this.updateRolesPromise, this.uscProcessRoleRestId);
        };
        TbltProcess.prototype.loadFascicleTypes = function () {
            var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
            emptyItem.set_text("");
            this._rcbProcessStatus.get_items().add(emptyItem);
            this.setFascicleTypeItem(this._rcbFascicleType, [FascicleType.Procedure, FascicleType.Activity]);
            this.setFascicleTypeItem(this._rcbProcessStatus, [FascicleType.Procedure, FascicleType.Activity]);
        };
        TbltProcess.prototype.setFascicleTypeItem = function (comboBox, fascicleTypes) {
            for (var _i = 0, fascicleTypes_1 = fascicleTypes; _i < fascicleTypes_1.length; _i++) {
                var itemType = fascicleTypes_1[_i];
                var item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(this._enumHelper.getFascicleTypeDescription(itemType));
                item.set_value(FascicleType[itemType]);
                comboBox.get_items().add(item);
            }
        };
        TbltProcess.prototype.loadProcesses = function (searchName, fascicleType, isActive) {
            var _this = this;
            var type = FascicleType[fascicleType];
            this._processService.getAll(searchName, type, isActive, function (data) {
                if (!data)
                    return;
                _this.processesModel = data;
                for (var _i = 0, _a = _this.processesModel; _i < _a.length; _i++) {
                    var process = _a[_i];
                    var node = new Telerik.Web.UI.RadTreeNode();
                    var isActive_1 = process.EndDate === null || new Date(process.EndDate) > new Date();
                    _this.createNode(node, process.Name, process.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png", ProcessNodeType.Process, false, null, isActive_1, process.Dossier.UniqueId, _this._rtvProcesses.get_nodes().getNode(0));
                    node.get_attributes().setAttribute("ProcessType", process.ProcessType);
                    if (process.ProcessType === ProcessType[ProcessType.Defined]) {
                        _this.createEmptyNode(node.get_nodes());
                    }
                    _this._rtvProcesses.commitChanges();
                }
                _this._rtvProcesses.get_nodes().getNode(0).expand();
                _this._ajaxLoadingPanel.hide(_this.processViewPaneId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.processViewPaneId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.createEmptyNode = function (nodes) {
            var emptyNode = new Telerik.Web.UI.RadTreeNode();
            emptyNode.set_text("");
            nodes.add(emptyNode);
        };
        TbltProcess.prototype.expandNodeLogic = function (expandedNodeChild) {
            expandedNodeChild.collapse();
            expandedNodeChild.get_nodes().clear();
            var dossierFolderStatus = expandedNodeChild.get_attributes().getAttribute("DossierFolderStatus");
            if (dossierFolderStatus === DossierFolderStatus[DossierFolderStatus.Folder]) {
                this.createEmptyNode(expandedNodeChild.get_nodes());
            }
        };
        TbltProcess.prototype.initializeNodeClicked = function (selectedNode) {
            this._uscProcessDetails = $("#" + this.uscProcessDetailsId).data();
            this._uscProcessDetails.clearProcessDetails();
            this.enableFolderToolbarButtons(false);
            $("#" + this._uscProcessDetails.pnlDetailsId).show();
            document.getElementById(this._uscProcessDetails.uscFascicleFoldersId.replace("_pageContent", "").concat("_pnlTitle")).style.position = "absolute";
            document.getElementById(this._uscProcessDetails.uscFascicleFoldersId.replace("_pageContent", "").concat("_pnlFolderToolbar")).style.position = "absolute";
            this._uscProcessDetails.clearFascicleInputs();
            this._ajaxLoadingPanel.show("ItemDetailTable");
            $("#workflowDetails").hide();
            $("#fascicleDetails").hide();
            $("#roleDetails").show();
        };
        TbltProcess.prototype.initializeProcessNodeDetails = function (selectedNode) {
            uscProcessDetails.selectedProcessId = selectedNode.get_value();
            uscProcessDetails.selectedEntityType = ProcessNodeType.Process;
            this._uscProcessDetails.setProcessDetails();
            this.enableFolderToolbarButtons(true);
            this._folderToolBar.findItemByValue("createProcessFascicleTemplate").set_enabled(false);
        };
        TbltProcess.prototype.initializeDossierFolderNodeDetails = function (selectedNode) {
            this._ajaxLoadingPanel.show("workflowDetails");
            $("#workflowDetails").show();
            var processParentNode = this.getProcessNodeByChild(selectedNode);
            uscProcessDetails.selectedDossierFolderId = selectedNode.get_value();
            uscProcessDetails.selectedProcessId = processParentNode.get_value();
            uscProcessDetails.selectedEntityType = ProcessNodeType.DossierFolder;
            this.enableFolderToolbarButtons(true);
            this._folderToolBar.findItemByValue("modify").set_enabled(false);
            this._uscProcessDetails.setProcessDetails();
            this._uscProcessDetails.setDossierFolderRoles();
            this._uscProcessDetails.setDossierFolderWorkflowRepositories();
        };
        TbltProcess.prototype.initializeProcessFascicleTemplateNodeDetails = function (selectedNode) {
            $("#fascicleDetails").show();
            $("#roleDetails").hide();
            document.getElementById("pnlMainFascicleFolder").style.position = "initial";
            this.enableFolderToolbarButtons(false);
            this._folderToolBar.findItemByValue("delete").set_enabled(true);
            this._folderToolBar.findItemByValue("modify").set_enabled(true);
            uscProcessDetails.selectedProcessFascicleTemplateId = selectedNode.get_value();
            uscProcessDetails.selectedDossierFolderId = selectedNode.get_parent().get_value();
            uscProcessDetails.selectedProcessId = this.getProcessNodeByChild(selectedNode).get_value();
            uscProcessDetails.selectedEntityType = ProcessNodeType.ProcessFascicleTemplate;
            this._uscProcessDetails.setProcessDetails();
            this._uscProcessDetails.setFascicle();
        };
        TbltProcess.prototype.loadData = function (currentNodeValue, status, idProcess) {
            var _this = this;
            this._ajaxLoadingPanel.show(this.rtvProcessesId);
            this._dossierFolderService.getChildren(currentNodeValue, status, function (data) {
                if (!data)
                    return;
                var dossierFolders = data;
                for (var _i = 0, dossierFolders_1 = dossierFolders; _i < dossierFolders_1.length; _i++) {
                    var child = dossierFolders_1[_i];
                    var node = new Telerik.Web.UI.RadTreeNode();
                    _this.createNode(node, child.Name, child.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png", ProcessNodeType.DossierFolder, true, DossierFolderStatus[child.Status], null, null, _this._rtvProcesses.findNodeByValue(idProcess ? idProcess : currentNodeValue));
                    if (child.Status === DossierFolderStatus[DossierFolderStatus.Folder]) {
                        _this.createEmptyNode(node.get_nodes());
                    }
                }
                _this.loadFascicleTemplates(currentNodeValue);
                _this._rtvProcesses.commitChanges();
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.loadFascicleTemplates = function (dossierFolderId) {
            var _this = this;
            this._dossierFolderService.getFascicleTemplatesByDossierFolderId(dossierFolderId, function (data) {
                if (!data)
                    return;
                _this.processFascicleTemplatesModel = _this.processFascicleTemplatesModel.concat(data);
                for (var _i = 0, _a = _this.processFascicleTemplatesModel; _i < _a.length; _i++) {
                    var fascicleTemplate = _a[_i];
                    var node = new Telerik.Web.UI.RadTreeNode();
                    var isActive = fascicleTemplate.EndDate === null || new Date(fascicleTemplate.EndDate) > new Date();
                    var imageUrl = isActive
                        ? "../App_Themes/DocSuite2008/imgset16/fascicle_close.png"
                        : "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
                    _this.createNode(node, fascicleTemplate.Name, fascicleTemplate.UniqueId, imageUrl, ProcessNodeType.ProcessFascicleTemplate, false, null, isActive, null, _this._rtvProcesses.findNodeByValue(dossierFolderId));
                }
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.getProcessNodeByChild = function (node) {
            while (node.get_level() > 1) {
                node = node.get_parent();
            }
            return node;
        };
        TbltProcess.prototype.clearInputs = function () {
            //process inputs
            this._rtbProcessName.clear();
            this._rcbProcessNote.clear();
            this._uscCategoryRest.clearTree();
            this._uscProcessRoleRest.renderRolesTree([]);
            //dossierFolder inputs
            this._rtbDossierFolderName.clear();
            //fascicleTemplate inputs
            this._rtbFascicleTemplateName.clear();
        };
        TbltProcess.prototype.hideInsertInputs = function () {
            $("#insertProcess").hide();
            $("#insertDossierFolder").hide();
            $("#insertFascicleTemplate").hide();
        };
        TbltProcess.prototype.enableFolderToolbarButtons = function (value) {
            this._folderToolBar.findItemByValue("create").set_enabled(value);
            this._folderToolBar.findItemByValue("createProcessFascicleTemplate").set_enabled(value);
            this._folderToolBar.findItemByValue("modify").set_enabled(value);
            this._folderToolBar.findItemByValue("delete").set_enabled(value);
        };
        TbltProcess.prototype.createNode = function (node, text, value, imagePath, nodeType, isExpanded, dossierFolderStatus, isActive, idDossier, parentNode) {
            node.set_value(value);
            node.set_text(text);
            node.get_attributes().setAttribute("NodeType", nodeType);
            node.set_imageUrl(imagePath);
            switch (nodeType) {
                case ProcessNodeType.Process: {
                    node.get_attributes().setAttribute("IsActive", isActive);
                    node.get_attributes().setAttribute("idDossier", idDossier);
                    break;
                }
                case ProcessNodeType.DossierFolder: {
                    node.get_attributes().setAttribute("DossierFolderStatus", DossierFolderStatus[dossierFolderStatus]);
                    break;
                }
                case ProcessNodeType.ProcessFascicleTemplate: {
                    node.get_attributes().setAttribute("IsActive", isActive);
                    break;
                }
            }
            if (parentNode !== null) {
                parentNode.get_nodes().add(node);
                parentNode.expand();
            }
        };
        TbltProcess.prototype.removeFascicleTemaple = function (endDate) {
            var _this = this;
            var fascicleTemplate = {};
            fascicleTemplate.UniqueId = this._rtvProcesses.get_selectedNode().get_value();
            for (var _i = 0, _a = this.processFascicleTemplatesModel; _i < _a.length; _i++) {
                var fascicleToFind = _a[_i];
                if (fascicleToFind.UniqueId == fascicleTemplate.UniqueId) {
                    fascicleTemplate = fascicleToFind;
                    break;
                }
            }
            fascicleTemplate.EndDate = endDate;
            this._fascicleTemplateService.delete(fascicleTemplate, function (data) {
                var imgUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
                _this._rtvProcesses.get_selectedNode().set_imageUrl(imgUrl);
                _this._rtvProcesses.get_selectedNode().get_attributes().setAttribute("IsActive", false);
                _this._rwInsert.close();
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.removeProcess = function (endDate) {
            var _this = this;
            var process = {};
            process.UniqueId = this._rtvProcesses.get_selectedNode().get_value();
            for (var _i = 0, _a = this.processesModel; _i < _a.length; _i++) {
                var processToFind = _a[_i];
                if (processToFind.UniqueId == process.UniqueId) {
                    process = processToFind;
                    break;
                }
            }
            process.Roles = this.processRoles;
            process.EndDate = endDate;
            this._processService.delete(process, function (data) {
                var processActiveItem = _this._filterToolbar.findItemByValue("processActive");
                var processDisabledItem = _this._filterToolbar.findItemByValue("processDisabled");
                var nodeRemoveConditions = processActiveItem.get_checked() && !processDisabledItem.get_checked();
                if (nodeRemoveConditions || !nodeRemoveConditions) {
                    _this._rtvProcesses.get_selectedNode().get_parent().get_nodes().remove(_this._rtvProcesses.get_selectedNode());
                }
                _this._rwInsert.close();
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.populateProcessInputs = function (processId) {
            var process = {};
            for (var _i = 0, _a = this.processesModel; _i < _a.length; _i++) {
                var processToFind = _a[_i];
                if (processToFind.UniqueId === processId) {
                    process = processToFind;
                    break;
                }
            }
            this._rtbProcessName.set_textBoxValue(process.Name);
            var fascicleTypeItem = this._rcbFascicleType.findItemByValue(FascicleType[process.FascicleType]);
            if (fascicleTypeItem) {
                fascicleTypeItem.select();
            }
            else {
                fascicleTypeItem = this._rcbFascicleType.get_items().getItem(0);
                fascicleTypeItem.select();
            }
            this._rcbProcessNote.set_textBoxValue(process.Note);
            var category = {};
            category.Code = process.Category.Code;
            category.Name = process.Category.Name;
            category.IdCategory = process.Category.EntityShortId;
            this._uscCategoryRest.populateCategotyTree(category);
        };
        TbltProcess.prototype.populateProcessFascicleTemplateInputs = function (processFascicleTemplateId) {
            var fascicleTemplate = {};
            for (var _i = 0, _a = this.processFascicleTemplatesModel; _i < _a.length; _i++) {
                var fascicleTemplateToFind = _a[_i];
                if (fascicleTemplateToFind.UniqueId === processFascicleTemplateId) {
                    fascicleTemplate = fascicleTemplateToFind;
                    break;
                }
            }
            this._rtbFascicleTemplateName.set_textBoxValue(fascicleTemplate.Name);
        };
        TbltProcess.prototype.showNotificationException = function (exception, customMessage) {
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
        TbltProcess.prototype.showNotificationMessage = function (customMessage) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TbltProcess.Process_TYPE_NAME = "Process";
        TbltProcess.DossierFolder_TYPE_NAME = "DossierFolder";
        TbltProcess.Dossier_TYPE_NAME = "Dossier";
        TbltProcess.ProcessFascicleTemplate_TYPE_NAME = "ProcessFascicleTemplate";
        return TbltProcess;
    }());
    return TbltProcess;
});
//# sourceMappingURL=TbltProcess.js.map