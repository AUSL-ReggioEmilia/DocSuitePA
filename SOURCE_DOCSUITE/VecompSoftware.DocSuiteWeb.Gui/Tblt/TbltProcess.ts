import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import EnumHelper = require("App/Helpers/EnumHelper");
import ProcessService = require('App/Services/Processes/ProcessService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ProcessModel = require('App/Models/Processes/ProcessModel');
import ProcessNodeType = require('App/Models/Processes/ProcessNodeType');
import DossierFolderService = require('App/Services/Dossiers/DossierFolderService');
import uscProcessDetails = require('UserControl/uscProcessDetails');
import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import ProcessFascicleTemplateModel = require('App/Models/Processes/ProcessFascicleTemplateModel');
import DossierFolderStatus = require('App/Models/Dossiers/DossierFolderStatus');
import uscCategoryRest = require('UserControl/uscCategoryRest');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierModel = require('App/Models/Dossiers/DossierModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import DossierService = require('App/Services/Dossiers/DossierService');
import RoleModel = require('App/Models/Commons/RoleModel');
import ProcessFascicleTemplateService = require('App/Services/Processes/ProcessFascicleTemplateService');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import CategoryTreeViewModel = require('App/ViewModels/Commons/CategoryTreeViewModel');
import uscRoleRest = require('UserControl/uscRoleRest');
import ProcessType = require('App/Models/Processes/ProcessType');

class TbltProcess {
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    processViewPaneId: string;
    processDetailsPaneId: string;
    rtvProcessesId: string;
    uscProcessDetailsId: string;
    rwInsertId: string;
    folderToolBarId: string;
    rtbProcessNameId: string;
    uscCategoryRestId: string;
    rcbFascicleTypeId: string;
    rtbDossierFolderNameId: string;
    rtbFascicleTemplateNameId: string;
    rbConfirmId: string;
    rcbProcessNoteId: string;
    filterToolbarId: string;
    uscProcessRoleRestId: string;

    processesModel: ProcessModel[];
    processFascicleTemplatesModel: ProcessFascicleTemplateModel[];
    selectedDossierFolderId: string;
    selectedProcessId: string;
    selectedProcessFascicleTemplateId: string;
    processRoles: RoleModel[];

    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;

    private _processService: ProcessService;
    private _dossierFolderService: DossierFolderService;
    private _dossierService: DossierService;
    private _fascicleTemplateService: ProcessFascicleTemplateService;
    protected static Process_TYPE_NAME = "Process";
    protected static DossierFolder_TYPE_NAME = "DossierFolder";
    protected static Dossier_TYPE_NAME = "Dossier";
    protected static ProcessFascicleTemplate_TYPE_NAME = "ProcessFascicleTemplate";

    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rtvProcesses: Telerik.Web.UI.RadTreeView;
    private _uscProcessDetails: uscProcessDetails;
    private _rwInsert: Telerik.Web.UI.RadWindow;
    private _folderToolBar: Telerik.Web.UI.RadToolBar;
    private _rtbProcessName: Telerik.Web.UI.RadTextBox;
    private _uscCategoryRest: uscCategoryRest;
    private _rcbFascicleType: Telerik.Web.UI.RadComboBox;
    private _rtbDossierFolderName: Telerik.Web.UI.RadTextBox;
    private _rtbFascicleTemplateName: Telerik.Web.UI.RadTextBox;
    private _rbConfirm: Telerik.Web.UI.RadButton;
    private _rcbDossier: Telerik.Web.UI.RadComboBox;
    private _rcbProcessNote: Telerik.Web.UI.RadTextBox;
    private _filterToolbar: Telerik.Web.UI.RadToolBar;
    private _rcbProcessStatus: Telerik.Web.UI.RadComboBox;
    private _rtbProcessSearchName: Telerik.Web.UI.RadTextBox;
    private _uscProcessRoleRest: uscRoleRest;


    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
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
    }

    initializeServices(): void {
        let processConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.Process_TYPE_NAME);
        this._processService = new ProcessService(processConfiguration);
        let dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.DossierFolder_TYPE_NAME);
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
        let dossierConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.Dossier_TYPE_NAME);
        this._dossierService = new DossierService(dossierConfiguration);
        let fascicleTemplateConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.ProcessFascicleTemplate_TYPE_NAME);
        this._fascicleTemplateService = new ProcessFascicleTemplateService(fascicleTemplateConfiguration);
    }

    initializeControls(): void {
        this._ajaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rtvProcesses = <Telerik.Web.UI.RadTreeView>$find(this.rtvProcessesId);
        this._rtvProcesses.add_nodeClicked(this.rtvProcesses_nodeClicked);
        this._rtvProcesses.get_nodes().getNode(0).get_attributes().setAttribute("NodeType", ProcessNodeType.Root);
        this._rtvProcesses.add_nodeExpanded(this.rtvProcess_onExpand);
        this._rwInsert = <Telerik.Web.UI.RadWindow>$find(this.rwInsertId);
        this._folderToolBar = <Telerik.Web.UI.RadToolBar>$find(this.folderToolBarId);
        this._folderToolBar.add_buttonClicked(this.folderToolBar_onClick);
        this._rtbProcessName = <Telerik.Web.UI.RadTextBox>$find(this.rtbProcessNameId);
        this._rcbFascicleType = <Telerik.Web.UI.RadComboBox>$find(this.rcbFascicleTypeId);
        this._rtbDossierFolderName = <Telerik.Web.UI.RadTextBox>$find(this.rtbDossierFolderNameId);
        this._rtbFascicleTemplateName = <Telerik.Web.UI.RadTextBox>$find(this.rtbFascicleTemplateNameId);
        this._rbConfirm = <Telerik.Web.UI.RadButton>$find(this.rbConfirmId);
        this._rbConfirm.add_clicked(this.rbConfirmInsert_onCLick);
        this._rcbProcessNote = <Telerik.Web.UI.RadTextBox>$find(this.rcbProcessNoteId);
        this._filterToolbar = <Telerik.Web.UI.RadToolBar>$find(this.filterToolbarId);
        this._filterToolbar.add_buttonClicked(this.filterToolbar_onClick);
        this._rcbProcessStatus = <Telerik.Web.UI.RadComboBox>this._filterToolbar.findItemByValue("statusInput").findControl("rcbProcessStatus");
        this._rtbProcessSearchName = <Telerik.Web.UI.RadTextBox>this._filterToolbar.findItemByValue("searchInput").findControl("txtSearch");
    }

    initializeUserControls(): void {
        this._uscCategoryRest = <uscCategoryRest>$(`#${this.uscCategoryRestId}`).data();
        this._uscProcessRoleRest = <uscRoleRest>$(`#${this.uscProcessRoleRestId}`).data();
        this._uscProcessRoleRest.renderRolesTree([]);
        this.registerUscRoleRestEventHandlers();
    }

    private registerUscRoleRestEventHandlers(): void {
        let uscRoleRestEventsDictionary = this._uscProcessRoleRest.uscRoleRestEvents;
        this._uscProcessRoleRest.registerEventHandler(uscRoleRestEventsDictionary.RoleDeleted, this.deleteRolePromise, this.uscProcessRoleRestId);
        this._uscProcessRoleRest.registerEventHandler(uscRoleRestEventsDictionary.NewRolesAdded, this.updateRolesPromise, this.uscProcessRoleRestId);
    }

    private deleteRolePromise = (roleIdToDelete: number, senderId?: string): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();
        if (!roleIdToDelete)
            return promise.promise();
        this.processRoles = this.processRoles
            .filter(role => role.IdRole !== roleIdToDelete || role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);
        promise.resolve(this.processRoles);
        return promise.promise();
    }

    private updateRolesPromise = (newAddedRoles: RoleModel[], senderId?: string): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();
        if (!newAddedRoles.length)
            return promise.promise();
        this.processRoles = [...this.processRoles, ...newAddedRoles];
        promise.resolve(this.processRoles);
        return promise.promise();
    }

    private loadFascicleTypes(): void {
        let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
        emptyItem.set_text("");
        this._rcbProcessStatus.get_items().add(emptyItem);
        this.setFascicleTypeItem(this._rcbFascicleType, [FascicleType.Procedure, FascicleType.Activity]);
        this.setFascicleTypeItem(this._rcbProcessStatus, [FascicleType.Procedure, FascicleType.Activity]);
    }

    setFascicleTypeItem(comboBox: Telerik.Web.UI.RadComboBox, fascicleTypes: FascicleType[]): void {
        for (let itemType of fascicleTypes) {
            let item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(this._enumHelper.getFascicleTypeDescription(itemType));
            item.set_value(FascicleType[itemType]);
            comboBox.get_items().add(item);
        }
    }

    private loadProcesses(searchName: string, fascicleType: string, isActive?: boolean): void {
        let type: FascicleType = FascicleType[fascicleType];
        this._processService.getAll(searchName, type, isActive, (data) => {
            if (!data) return;
            this.processesModel = data;

            for (let process of this.processesModel) {
                var node = new Telerik.Web.UI.RadTreeNode();
                let isActive: boolean = process.EndDate === null || new Date(process.EndDate) > new Date();
                this.createNode(node, process.Name, process.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png",
                    ProcessNodeType.Process, false, null, isActive, process.Dossier.UniqueId, this._rtvProcesses.get_nodes().getNode(0));
                node.get_attributes().setAttribute("ProcessType", process.ProcessType);
                if (process.ProcessType === ProcessType[ProcessType.Defined]) {
                    this.createEmptyNode(node.get_nodes());
                }
                this._rtvProcesses.commitChanges();
            }
            this._rtvProcesses.get_nodes().getNode(0).expand();
            this._ajaxLoadingPanel.hide(this.processViewPaneId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.processViewPaneId);
            this.showNotificationException(error);
        });
    }

    private createEmptyNode(nodes: Telerik.Web.UI.RadTreeNodeCollection): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text("");
        nodes.add(emptyNode);
    }

    rtvProcess_onExpand = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let expandedNode: Telerik.Web.UI.RadTreeNode = args.get_node();
        if (expandedNode.get_level() === 1) {
            this.expandNodeLogic(expandedNode);
            this.loadData(expandedNode.get_attributes().getAttribute("idDossier"), 0, expandedNode.get_value());
        }
        else {
            if (expandedNode.get_nodes().getNode(0).get_text() === "") {
                expandedNode.get_nodes().clear();
                this.loadData(expandedNode.get_value(), 0);
            }
            else {
                for (let index = 0; index < expandedNode.get_nodes().get_count(); index++) {
                    this.expandNodeLogic(expandedNode.get_nodes().getNode(index));
                }
            }
        }
    }

    private expandNodeLogic(expandedNodeChild: Telerik.Web.UI.RadTreeNode): void {
        expandedNodeChild.collapse();
        expandedNodeChild.get_nodes().clear();
        let dossierFolderStatus: string = expandedNodeChild.get_attributes().getAttribute("DossierFolderStatus");
        if (dossierFolderStatus === DossierFolderStatus[DossierFolderStatus.Folder]) {
            this.createEmptyNode(expandedNodeChild.get_nodes());
        }
    }

    rtvProcesses_nodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let selectedNode = args.get_node();
        this.initializeNodeClicked(selectedNode);
        if (selectedNode.get_level() === 0) {
            this._folderToolBar.findItemByValue("create").set_enabled(true);
            $(`#${this._uscProcessDetails.pnlDetailsId}`).hide();
            this._ajaxLoadingPanel.hide("ItemDetailTable");
        }
        else {
            switch (selectedNode.get_attributes().getAttribute("NodeType")) {
                case ProcessNodeType.Process: {
                    this.initializeProcessNodeDetails(selectedNode);
                    break;
                }
                case ProcessNodeType.DossierFolder: {
                    this.initializeDossierFolderNodeDetails(selectedNode);
                    break;
                }
                case ProcessNodeType.ProcessFascicleTemplate: {
                    this.initializeProcessFascicleTemplateNodeDetails(selectedNode);
                    break;
                }
            }
        }
    }

    private initializeNodeClicked(selectedNode: Telerik.Web.UI.RadTreeNode): void {
        this._uscProcessDetails = <uscProcessDetails>$(`#${this.uscProcessDetailsId}`).data();
        this._uscProcessDetails.clearProcessDetails();
        this.enableFolderToolbarButtons(false);
        $(`#${this._uscProcessDetails.pnlDetailsId}`).show();
        document.getElementById(this._uscProcessDetails.uscFascicleFoldersId.replace("_pageContent", "").concat("_pnlTitle")).style.position = "absolute";
        document.getElementById(this._uscProcessDetails.uscFascicleFoldersId.replace("_pageContent", "").concat("_pnlFolderToolbar")).style.position = "absolute";
        this._uscProcessDetails.clearFascicleInputs();
        this._ajaxLoadingPanel.show("ItemDetailTable");
        $("#workflowDetails").hide();
        $("#fascicleDetails").hide();
        $("#roleDetails").show();
    }

    private initializeProcessNodeDetails(selectedNode: Telerik.Web.UI.RadTreeNode): void {
        uscProcessDetails.selectedProcessId = selectedNode.get_value();
        uscProcessDetails.selectedEntityType = ProcessNodeType.Process;
        this._uscProcessDetails.setProcessDetails();
        this.enableFolderToolbarButtons(true);
        this._folderToolBar.findItemByValue("createProcessFascicleTemplate").set_enabled(false);
    }

    private initializeDossierFolderNodeDetails(selectedNode: Telerik.Web.UI.RadTreeNode): void {
        this._ajaxLoadingPanel.show("workflowDetails");
        $("#workflowDetails").show();
        let processParentNode: Telerik.Web.UI.RadTreeNode = this.getProcessNodeByChild(selectedNode);
        uscProcessDetails.selectedDossierFolderId = selectedNode.get_value();
        uscProcessDetails.selectedProcessId = processParentNode.get_value();
        uscProcessDetails.selectedEntityType = ProcessNodeType.DossierFolder;
        this.enableFolderToolbarButtons(true);
        this._folderToolBar.findItemByValue("modify").set_enabled(false);
        this._uscProcessDetails.setProcessDetails();
        this._uscProcessDetails.setDossierFolderRoles();
        this._uscProcessDetails.setDossierFolderWorkflowRepositories();
    }

    private initializeProcessFascicleTemplateNodeDetails(selectedNode: Telerik.Web.UI.RadTreeNode): void {
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
    }

    private loadData(currentNodeValue: string, status: number, idProcess?: string): void {
        this._ajaxLoadingPanel.show(this.rtvProcessesId);
        this._dossierFolderService.getChildren(currentNodeValue, status, (data) => {
            if (!data) return;
            let dossierFolders: DossierSummaryFolderViewModel[] = data;

            for (let child of dossierFolders) {
                var node = new Telerik.Web.UI.RadTreeNode();
                this.createNode(node, child.Name, child.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png",
                    ProcessNodeType.DossierFolder, true, DossierFolderStatus[child.Status], null, null,
                    this._rtvProcesses.findNodeByValue(idProcess ? idProcess : currentNodeValue));
                if (child.Status === DossierFolderStatus[DossierFolderStatus.Folder]) {
                    this.createEmptyNode(node.get_nodes());
                }
            }
            this.loadFascicleTemplates(currentNodeValue);
            this._rtvProcesses.commitChanges();
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            this.showNotificationException(error);
        });
    }

    private loadFascicleTemplates(dossierFolderId: string): void {
        this._dossierFolderService.getFascicleTemplatesByDossierFolderId(dossierFolderId, (data) => {
            if (!data) return;
            this.processFascicleTemplatesModel = this.processFascicleTemplatesModel.concat(data);
            for (let fascicleTemplate of this.processFascicleTemplatesModel) {
                var node = new Telerik.Web.UI.RadTreeNode();
                let isActive: boolean = fascicleTemplate.EndDate === null || new Date(fascicleTemplate.EndDate) > new Date();
                let imageUrl: string = isActive
                    ? "../App_Themes/DocSuite2008/imgset16/fascicle_close.png"
                    : "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
                this.createNode(node, fascicleTemplate.Name, fascicleTemplate.UniqueId, imageUrl,
                    ProcessNodeType.ProcessFascicleTemplate, false, null, isActive, null,
                    this._rtvProcesses.findNodeByValue(dossierFolderId));
            }
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            this.showNotificationException(error);
        });
    }

    getProcessNodeByChild(node: Telerik.Web.UI.RadTreeNode): Telerik.Web.UI.RadTreeNode {
        while (node.get_level() > 1) {
            node = node.get_parent();
        }
        return node;
    }

    rbProcessInsert_onCLick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this.hideInsertInputs();
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.get_selectedNode();
        this.clearInputs();
        if (selectedNode.get_level() === 0) {
            this.selectedProcessId = "";
            $("#insertProcess").show();
        }
        else {
            this.selectedProcessFascicleTemplateId = "";
            $("#insertFascicleTemplate").show();
        }
        this.processRoles = [];
        this._rwInsert.show();
    }

    clearInputs(): void {
        //process inputs
        this._rtbProcessName.clear();
        this._rcbProcessNote.clear();
        this._uscCategoryRest.clearTree();
        this._uscProcessRoleRest.renderRolesTree([]);
        //dossierFolder inputs
        this._rtbDossierFolderName.clear();
        //fascicleTemplate inputs
        this._rtbFascicleTemplateName.clear();
    }

    hideInsertInputs(): void {
        $("#insertProcess").hide();
        $("#insertDossierFolder").hide();
        $("#insertFascicleTemplate").hide();
    }

    enableFolderToolbarButtons(value: boolean): void {
        this._folderToolBar.findItemByValue("create").set_enabled(value);
        this._folderToolBar.findItemByValue("createProcessFascicleTemplate").set_enabled(value);
        this._folderToolBar.findItemByValue("modify").set_enabled(value);
        this._folderToolBar.findItemByValue("delete").set_enabled(value);
    }

    folderToolBar_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        this.hideInsertInputs();
        switch (args.get_item().get_value()) {
            case "create": {
                this.clearInputs();
                let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.get_selectedNode();
                if (selectedNode.get_level() === 0) {
                    this.selectedProcessId = "";
                    this._uscCategoryRest.enableButtons();
                    $("#insertProcess").show();
                    this._rwInsert.set_title("Aggiungi procedimento");
                }
                switch (selectedNode.get_attributes().getAttribute("NodeType")) {
                    case ProcessNodeType.Process:
                    case ProcessNodeType.DossierFolder: {
                        this.selectedDossierFolderId = "";
                        $("#insertDossierFolder").show();
                        this._rwInsert.set_title("Aggiungi cartella di dossier");
                        break;
                    }
                }
                this._rwInsert.show();
                break;
            }
            case "createProcessFascicleTemplate": {
                this.selectedProcessFascicleTemplateId = "";
                $("#insertFascicleTemplate").show();
                this._rwInsert.set_title("Aggiungi modello di fascicolo di processo");
                this._rwInsert.show();
                break;
            }
            case "delete": {
                let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.get_selectedNode();
                switch (selectedNode.get_attributes().getAttribute("NodeType")) {
                    case ProcessNodeType.Process: {
                        let yesterdayDate = new Date();
                        yesterdayDate.setDate(new Date().getDate() - 1);
                        this._ajaxLoadingPanel.show(this.rtvProcessesId);
                        this.removeProcess(yesterdayDate);
                        break;
                    }
                    case ProcessNodeType.DossierFolder: {
                        if (uscProcessDetails.processFascicleWorkflowRepositories.length > 0) {
                            alert("Impossibile eliminare la cartella. Esiste un flusso di lavoro associato.");
                            return;
                        }
                        let dossierFolder: DossierFolderModel = <DossierFolderModel>{};
                        dossierFolder.UniqueId = this._rtvProcesses.get_selectedNode().get_value();
                        if (this._rtvProcesses.get_selectedNode().get_level() === 2) {
                            dossierFolder.ParentInsertId = this.getProcessNodeByChild(this._rtvProcesses.get_selectedNode()).get_attributes().getAttribute("idDossier");
                        }
                        else {
                            dossierFolder.ParentInsertId = this._rtvProcesses.get_selectedNode().get_parent().get_value();
                        }
                        this._ajaxLoadingPanel.show(this.rtvProcessesId);
                        this._dossierFolderService.deleteDossierFolder(dossierFolder, (data) => {
                            this._rtvProcesses.get_selectedNode().get_parent().get_nodes().remove(this._rtvProcesses.get_selectedNode());
                            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                        }, (error) => {
                            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                            this.showNotificationException(error);
                        });
                        break;
                    }
                    case ProcessNodeType.ProcessFascicleTemplate: {
                        let yesterdayDate = new Date();
                        yesterdayDate.setDate(new Date().getDate() - 1);
                        this._ajaxLoadingPanel.show(this.rtvProcessesId);
                        this.removeFascicleTemaple(yesterdayDate);
                        break;
                    }
                }
                break;
            }
            case "modify": {
                this.hideInsertInputs();
                let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.get_selectedNode();
                switch (selectedNode.get_attributes().getAttribute("NodeType")) {
                    case ProcessNodeType.Process: {
                        this.selectedProcessId = this._rtvProcesses.get_selectedNode().get_value();
                        this.populateProcessInputs(this.selectedProcessId);
                        for (let processToFind of this.processesModel) {
                            if (processToFind.UniqueId === this.selectedProcessId) {
                                this.processRoles = processToFind.Roles;
                                this._uscProcessRoleRest.renderRolesTree(processToFind.Roles);
                                break;
                            }
                        }
                        this._uscCategoryRest.disableButtons();
                        $("#insertProcess").show();
                        this._rwInsert.set_title("Modifica procedimento");
                        break;
                    }
                    case ProcessNodeType.ProcessFascicleTemplate: {
                        this.selectedProcessFascicleTemplateId = this._rtvProcesses.get_selectedNode().get_value();
                        this.populateProcessFascicleTemplateInputs(this.selectedProcessFascicleTemplateId);
                        $("#insertFascicleTemplate").show();
                        this._rwInsert.set_title("Modifica modello di fascicolo di processo");
                        break;
                    }
                }
                this._rwInsert.show();
                break;
            }
        }
    }

    rbConfirmInsert_onCLick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._ajaxLoadingPanel.show(this.rtvProcessesId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.get_selectedNode();

        if ($("#insertDossierFolder").is(":visible")) {
            if (this._rtbDossierFolderName.get_textBoxValue()) {
                let exists: boolean = this.selectedDossierFolderId !== "";
                let dossierFolder: DossierFolderModel = <DossierFolderModel>{};
                dossierFolder.Name = this._rtbDossierFolderName.get_textBoxValue();

                if (selectedNode.get_attributes().getAttribute("NodeType") !== ProcessNodeType.Process) {
                    dossierFolder.ParentInsertId = exists ? selectedNode.get_parent().get_value() : selectedNode.get_value();
                }
                else {
                    dossierFolder.ParentInsertId = this.getProcessNodeByChild(selectedNode).get_attributes().getAttribute("idDossier");
                }
                dossierFolder.Dossier = <DossierModel>{};
                dossierFolder.Dossier.UniqueId = this.getProcessNodeByChild(selectedNode).get_attributes().getAttribute("idDossier");
                if (exists) {
                    dossierFolder.UniqueId = this.selectedDossierFolderId;
                    this._dossierFolderService.updateDossierFolder(dossierFolder, null, (data) => {
                        this.createNode(selectedNode, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png",
                            ProcessNodeType.DossierFolder, false, data.Status, null, null, null);
                        this._rwInsert.close();
                        this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                    }, (error) => {
                        this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                        this.showNotificationException(error);
                    });
                }
                else {
                    this._dossierFolderService.insertDossierFolder(dossierFolder, null, (data) => {
                        let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                        if (selectedNode.get_nodes().get_count() > 0 && selectedNode.get_nodes().getNode(0).get_text() === "") {
                            selectedNode.get_nodes().clear();
                        }
                        this.createNode(node, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png",
                            ProcessNodeType.DossierFolder, true, data.Status, null, null, selectedNode);
                        this._rwInsert.close();
                        this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                    }, (error) => {
                        this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                        this.showNotificationException(error);
                    });
                }
            }
        }

        if ($("#insertProcess").is(":visible")) {
            let exists: boolean = this.selectedProcessId !== "";
            let process: ProcessModel = <ProcessModel>{};
            process.Name = this._rtbProcessName.get_textBoxValue();
            process.Dossier = <DossierModel>{};
            process.Dossier.UniqueId = this.getProcessNodeByChild(selectedNode).get_value();
            process.Category = new CategoryModel();
            process.Category.EntityShortId = this._uscCategoryRest.getSelectedCategory().EntityShortId;
            process.FascicleType = this._enumHelper.getFascicleType(this._rcbFascicleType.get_selectedItem().get_text());
            process.StartDate = new Date();
            process.EndDate = null;
            process.Dossier = <DossierModel>{};
            process.Roles = this.processRoles;
            process.Note = this._rcbProcessNote.get_value();

            if (exists) {
                process.UniqueId = this.selectedProcessId;
                this._processService.update(process, (data) => {
                    let isActive: boolean = data.EndDate === null || new Date(data.EndDate) > new Date();
                    this.createNode(selectedNode, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png",
                        ProcessNodeType.Process, false, null, isActive, data.Dossier.UniqueId, null);
                    this._rwInsert.close();
                    this._ajaxLoadingPanel.hide(this.rtvProcessesId);

                    this._ajaxLoadingPanel.show("ItemDetailTable");
                    this._uscProcessDetails = <uscProcessDetails>$(`#${this.uscProcessDetailsId}`).data();
                    this._uscProcessDetails.clearProcessDetails();
                    uscProcessDetails.selectedProcessId = data.UniqueId;
                    this._uscProcessDetails.setProcessDetails();
                    this._ajaxLoadingPanel.hide("ItemDetailTable");
                }, (error) => {
                    this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                    this.showNotificationException(error);
                });
            }
            else {
                let process: ProcessModel = <ProcessModel>{};
                process.Name = this._rtbProcessName.get_value();
                process.Category = new CategoryModel();
                process.Category.EntityShortId = this._uscCategoryRest.getSelectedCategory().EntityShortId;
                process.FascicleType = this._enumHelper.getFascicleType(this._rcbFascicleType.get_selectedItem().get_text());
                process.StartDate = new Date();
                process.Dossier = <DossierModel>{};
                process.Roles = this.processRoles;
                process.Note = this._rcbProcessNote.get_value();
                this._processService.insert(process, (data) => {
                    let isActive: boolean = data.EndDate === null || new Date(data.EndDate) > new Date();
                    let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                    this.createNode(node, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png",
                        ProcessNodeType.Process, true, null, isActive, data.Dossier.UniqueId, selectedNode);
                    this.processesModel.push(data);
                    this._rwInsert.close();
                    this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                }, (error) => {
                    this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                    this.showNotificationException(error);
                });
            }

        }

        if ($("#insertFascicleTemplate").is(":visible")) {
            let exists: boolean = this.selectedProcessFascicleTemplateId !== "";
            let fascicleTemplate: ProcessFascicleTemplateModel = <ProcessFascicleTemplateModel>{};
            fascicleTemplate.Name = this._rtbFascicleTemplateName.get_value();
            fascicleTemplate.StartDate = new Date();
            fascicleTemplate.EndDate = null;
            fascicleTemplate.Process = <ProcessModel>{};
            fascicleTemplate.Process.UniqueId = this.getProcessNodeByChild(selectedNode).get_value();
            fascicleTemplate.DossierFolder = <DossierFolderModel>{};
            fascicleTemplate.DossierFolder.UniqueId = selectedNode.get_value();
            fascicleTemplate.JsonModel = "";
            let imageUrl: string;
            if (fascicleTemplate.EndDate === null || new Date(fascicleTemplate.EndDate) < new Date()) {
                imageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
            }
            else {
                imageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
            }
            if (exists) {
                fascicleTemplate.UniqueId = this.selectedProcessFascicleTemplateId;
                this._fascicleTemplateService.update(fascicleTemplate, (data) => {
                    this.createNode(selectedNode, data.Name, data.UniqueId, imageUrl,
                        ProcessNodeType.ProcessFascicleTemplate, false, null, null, null, null);
                    this._rwInsert.close();
                    this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                }, (error) => {
                    this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                    this.showNotificationException(error);
                });
            } else {
                this._fascicleTemplateService.insert(fascicleTemplate, (data) => {
                    let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                    this.createNode(node, data.Name, data.UniqueId, imageUrl,
                        ProcessNodeType.ProcessFascicleTemplate, true, null, null, null, selectedNode);
                    this._rwInsert.close();
                    this.processFascicleTemplatesModel.push(data);
                    this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                }, (error) => {
                    this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                    this.showNotificationException(error);
                });
            }
        }
    }

    private createNode(node: Telerik.Web.UI.RadTreeNode, text: string, value: string, imagePath: string, nodeType: ProcessNodeType, isExpanded: boolean,
        dossierFolderStatus?: DossierFolderStatus, isActive?: boolean, idDossier?: string, parentNode?: Telerik.Web.UI.RadTreeNode): void {
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
    }

    private removeFascicleTemaple(endDate: Date): void {
        let fascicleTemplate: ProcessFascicleTemplateModel = <ProcessFascicleTemplateModel>{};
        fascicleTemplate.UniqueId = this._rtvProcesses.get_selectedNode().get_value();
        for (let fascicleToFind of this.processFascicleTemplatesModel) {
            if (fascicleToFind.UniqueId == fascicleTemplate.UniqueId) {
                fascicleTemplate = fascicleToFind;
                break;
            }
        }

        fascicleTemplate.EndDate = endDate;
        this._fascicleTemplateService.delete(fascicleTemplate, (data) => {
            let imgUrl: string = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
            this._rtvProcesses.get_selectedNode().set_imageUrl(imgUrl);
            this._rtvProcesses.get_selectedNode().get_attributes().setAttribute("IsActive", false);
            this._rwInsert.close();
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            this.showNotificationException(error);
        });
    }

    private removeProcess(endDate: Date): void {
        let process: ProcessModel = <ProcessModel>{};
        process.UniqueId = this._rtvProcesses.get_selectedNode().get_value();
        for (let processToFind of this.processesModel) {
            if (processToFind.UniqueId == process.UniqueId) {
                process = processToFind;
                break;
            }
        }
        process.Roles = this.processRoles;
        process.EndDate = endDate;
        this._processService.delete(process, (data) => {
            let processActiveItem: any = this._filterToolbar.findItemByValue("processActive");
            let processDisabledItem: any = this._filterToolbar.findItemByValue("processDisabled");
            let nodeRemoveConditions: boolean = processActiveItem.get_checked() && !processDisabledItem.get_checked();
            if (nodeRemoveConditions || !nodeRemoveConditions) {
                this._rtvProcesses.get_selectedNode().get_parent().get_nodes().remove(this._rtvProcesses.get_selectedNode());
            }
            this._rwInsert.close();
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            this.showNotificationException(error);
        });
    }

    filterToolbar_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        switch (args.get_item().get_value()) {
            case "search": {
                this._ajaxLoadingPanel.show(this.processViewPaneId);
                this._uscProcessDetails = <uscProcessDetails>$(`#${this.uscProcessDetailsId}`).data();
                this._uscProcessDetails.clearProcessDetails();
                $(`#${this._uscProcessDetails.pnlDetailsId}`).hide();
                this._rtvProcesses.get_nodes().getNode(0).get_nodes().clear();
                let processSearchName: string = this._rtbProcessSearchName.get_textBoxValue();
                let fascicleType: string = "";
                if (this._rcbFascicleType.get_selectedItem()) {
                    fascicleType = FascicleType[this._enumHelper.getFascicleType(this._rcbFascicleType.get_selectedItem().get_text())];
                }
                let processActiveItem: any = this._filterToolbar.findItemByValue("processActive");
                let processDisabledItem: any = this._filterToolbar.findItemByValue("processDisabled");
                let callLoadProcesses: void = processActiveItem.get_checked()
                    ? processDisabledItem.get_checked() ? this.loadProcesses(processSearchName, fascicleType, null) : this.loadProcesses(processSearchName, fascicleType, true)
                    : processDisabledItem.get_checked() ? this.loadProcesses(processSearchName, fascicleType, false) : this.loadProcesses(processSearchName, fascicleType, null);
                break;
            }
        }
    }

    populateProcessInputs(processId: string): void {
        let process: ProcessModel = <ProcessModel>{};
        for (let processToFind of this.processesModel) {
            if (processToFind.UniqueId === processId) {
                process = processToFind;
                break;
            }
        }
        this._rtbProcessName.set_textBoxValue(process.Name);
        let fascicleTypeItem: Telerik.Web.UI.RadComboBoxItem = this._rcbFascicleType.findItemByValue(FascicleType[process.FascicleType]);
        if (fascicleTypeItem) {
            fascicleTypeItem.select();
        }
        else {
            fascicleTypeItem = this._rcbFascicleType.get_items().getItem(0);
            fascicleTypeItem.select();
        }
        this._rcbProcessNote.set_textBoxValue(process.Note);
        let category: CategoryTreeViewModel = <CategoryTreeViewModel>{};
        category.Code = process.Category.Code;
        category.Name = process.Category.Name;
        category.IdCategory = process.Category.EntityShortId;
        this._uscCategoryRest.populateCategotyTree(category);
    }

    populateProcessFascicleTemplateInputs(processFascicleTemplateId: string): void {
        let fascicleTemplate: ProcessFascicleTemplateModel = <ProcessFascicleTemplateModel>{};
        for (let fascicleTemplateToFind of this.processFascicleTemplatesModel) {
            if (fascicleTemplateToFind.UniqueId === processFascicleTemplateId) {
                fascicleTemplate = fascicleTemplateToFind;
                break;
            }
        }
        this._rtbFascicleTemplateName.set_textBoxValue(fascicleTemplate.Name);
    }

    private showNotificationException(exception: ExceptionDTO, customMessage?: string): void {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(customMessage)
        }
    }

    private showNotificationMessage(customMessage: string): void {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }
}

export = TbltProcess;