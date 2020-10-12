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
import DossierService = require('App/Services/Dossiers/DossierService');
import RoleModel = require('App/Models/Commons/RoleModel');
import ProcessFascicleTemplateService = require('App/Services/Processes/ProcessFascicleTemplateService');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import CategoryTreeViewModel = require('App/ViewModels/Commons/CategoryTreeViewModel');
import uscRoleRest = require('UserControl/uscRoleRest');
import ProcessType = require('App/Models/Processes/ProcessType');
import InsertActionType = require('App/Models/InsertActionType');
import UpdateActionType = require('App/Models/UpdateActionType');
import CategoryService = require('App/Services/Commons/CategoryService');
import CategoryFascicleRightViewModel = require('App/ViewModels/Commons/CategoryFascicleRightViewModel');
import CategoryFascicleViewModel = require('App/ViewModels/Commons/CategoryFascicleViewModel');
import ExternalSourceActionEnum = require('App/Helpers/ExternalSourceActionEnum');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import CommonSelCategoryRest = require('UserControl/CommonSelCategoryRest');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

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
    rtbDossierFolderNameId: string;
    rtbFascicleTemplateNameId: string;
    rtbCloneDossierFolderNameId: string;
    rbConfirmId: string;
    rcbProcessNoteId: string;
    filterToolbarId: string;
    uscProcessRoleRestId: string;
    defaultSelectedProcessId: string;
    defaultSelectedProcessCategoryId: string;
    currentTenantAOOId: string;

    processesModel: ProcessModel[];
    processFascicleTemplatesModel: ProcessFascicleTemplateModel[];
    private tempPFTModel: ProcessFascicleTemplateModel[] = [];
    selectedDossierFolderId: string;
    selectedProcessId: string;
    selectedProcessFascicleTemplateId: string;
    processRoles: RoleModel[];
    categoryModel: CategoryModel;
    private _processCategories: CategoryModel[];

    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;

    private _categoryService: CategoryService;
    private _processService: ProcessService;
    private _dossierFolderService: DossierFolderService;
    private _dossierService: DossierService;
    private _fascicleTemplateService: ProcessFascicleTemplateService;
    protected static Process_TYPE_NAME = "Process";
    protected static DossierFolder_TYPE_NAME = "DossierFolder";
    protected static Dossier_TYPE_NAME = "Dossier";
    protected static ProcessFascicleTemplate_TYPE_NAME = "ProcessFascicleTemplate";
    protected static Category_TYPE_NAME = "Category"
    private static TOOLBAR_CREATE = "create";
    private static TOOLBAR_CREATE_PROCESS_FASCICLE_TEMPLATE = "createProcessFascicleTemplate";
    private static TOOLBAR_MODIFY = "modify";
    private static TOOLBAR_DELETE = "delete";
    private static TOOLBAR_CLONE = "clone";
    private static TOOLBAR_COPYPFT = "copyPFT";
    private static TOOLBAR_PASTEPFT = "pastePFT";
    public static NodeType_TYPE_NAME = "NodeType";
    private static ProcessType_TYPE_NAME = "ProcessType";
    private static Category_ID_TYPE = "idCategory";


    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rtvProcesses: Telerik.Web.UI.RadTreeView;
    private _uscProcessDetails: uscProcessDetails;
    private _rwInsert: Telerik.Web.UI.RadWindow;
    private _folderToolBar: Telerik.Web.UI.RadToolBar;
    private _rtbProcessName: Telerik.Web.UI.RadTextBox;
    private _uscCategoryRest: uscCategoryRest;
    private _rtbDossierFolderName: Telerik.Web.UI.RadTextBox;
    private _rtbFascicleTemplateName: Telerik.Web.UI.RadTextBox;
    private _rtbCloneDossierFolderName: Telerik.Web.UI.RadTextBox;
    private _rbConfirm: Telerik.Web.UI.RadButton;
    private _rcbDossier: Telerik.Web.UI.RadComboBox;
    private _rcbProcessNote: Telerik.Web.UI.RadTextBox;
    private _filterToolbar: Telerik.Web.UI.RadToolBar;
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
        this._initializeProcessesTree();
        this.processFascicleTemplatesModel = [];
        this.processRoles = [];
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL);
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
        let categoryServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.Category_TYPE_NAME);
        this._categoryService = new CategoryService(categoryServiceConfiguration);

    }

    initializeControls(): void {
        this._ajaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rtvProcesses = <Telerik.Web.UI.RadTreeView>$find(this.rtvProcessesId);
        this._rtvProcesses.add_nodeClicked(this.rtvProcesses_nodeClicked);
        this._rtvProcesses.get_nodes().getNode(0).get_attributes().setAttribute(TbltProcess.NodeType_TYPE_NAME, ProcessNodeType.Root);
        this._rtvProcesses.add_nodeExpanded(this.rtvProcess_onExpand);
        this._rtvProcesses.get_nodes().getNode(0).expand();
        this._rwInsert = <Telerik.Web.UI.RadWindow>$find(this.rwInsertId);
        this._folderToolBar = <Telerik.Web.UI.RadToolBar>$find(this.folderToolBarId);
        this._folderToolBar.add_buttonClicked(this.folderToolBar_onClick);
        this._rtbProcessName = <Telerik.Web.UI.RadTextBox>$find(this.rtbProcessNameId);
        this._rtbDossierFolderName = <Telerik.Web.UI.RadTextBox>$find(this.rtbDossierFolderNameId);
        this._rtbFascicleTemplateName = <Telerik.Web.UI.RadTextBox>$find(this.rtbFascicleTemplateNameId);
        this._rtbCloneDossierFolderName = <Telerik.Web.UI.RadTextBox>$find(this.rtbCloneDossierFolderNameId);
        this._rbConfirm = <Telerik.Web.UI.RadButton>$find(this.rbConfirmId);
        this._rbConfirm.add_clicked(this.rbConfirmInsert_onCLick);
        this._rcbProcessNote = <Telerik.Web.UI.RadTextBox>$find(this.rcbProcessNoteId);
        this._filterToolbar = <Telerik.Web.UI.RadToolBar>$find(this.filterToolbarId);
        //TODO: feature will be completed in > 9.01
        //this._filterToolbar.add_buttonClicked(this.filterToolbar_onClick);
        this._rtbProcessSearchName = <Telerik.Web.UI.RadTextBox>this._filterToolbar.findItemByValue("searchInput").findControl("txtSearch");
    }

    initializeUserControls(): void {
        this._uscCategoryRest = <uscCategoryRest>$(`#${this.uscCategoryRestId}`).data();
        this._uscProcessRoleRest = <uscRoleRest>$(`#${this.uscProcessRoleRestId}`).data();
        this._uscProcessRoleRest.renderRolesTree([]);
        this.registerUscRoleRestEventHandlers();

        this._uscProcessRoleRest.disableButtons();
        this._uscCategoryRest.registerAddedEventhandler(this.addCategoryEventHandler);

    }

    private addCategoryEventHandler = (data: JQueryEventObject, args: any): void => {
        let categoryId = args;
        if (categoryId) {
            this._uscProcessRoleRest.clearRoleTreeView();
            this._categoryService.getRolesByCategoryId(categoryId, (data) => {
                this.categoryModel = data;
                let categoryFascicleModel: CategoryFascicleViewModel = this.categoryModel.CategoryFascicles[0];

                let categoryFascicleRightsModel: CategoryFascicleRightViewModel[];

                if (categoryFascicleModel) {
                    categoryFascicleRightsModel = categoryFascicleModel.CategoryFascicleRights;

                    if (categoryFascicleRightsModel) {
                        let roleArray: RoleModel[] = [];
                        for (let cfrm of categoryFascicleRightsModel) {
                            roleArray.push(cfrm.Role)
                        }

                        if (roleArray) {
                            //set popup roles source
                            let needRolesFromExternalSource_eventArgs: string[] = [ExternalSourceActionEnum.Category.toString(), categoryId];
                            $(`#${this.uscProcessRoleRestId}`).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, needRolesFromExternalSource_eventArgs);

                            this._uscProcessRoleRest.renderRolesTree(roleArray);
                            this.processRoles = roleArray;
                            this._uscProcessRoleRest.enableButtons();
                        }
                        else {
                            this._uscProcessRoleRest.disableButtons();
                        }
                    }
                    else {
                        this._uscProcessRoleRest.disableButtons();
                    }
                }
                else {
                    this._uscProcessRoleRest.disableButtons();
                }
            }, (exception: ExceptionDTO) => {
                this.showNotificationException(exception);
            });
        }
    }

    private _initializeProcessesTree(): void {
        this.loadFascicolableCategories();
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

    private loadFascicolableCategories(): void {
        this._categoryService.getOnlyFascicolableCategories(this.currentTenantAOOId, (data) => {
            if (!data) {
                return;
            }
            this._processCategories = data;
            this._rtvProcesses.get_nodes().getNode(0).showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            let categoryIds: number[] = this.getCategoryIds(0, null);
            this.loadCategories(categoryIds, null);
            this._ajaxLoadingPanel.hide(this.processViewPaneId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.processViewPaneId);
            this.showNotificationException(error);
        });
    }

    private loadCategories(categoryIds: number[], parentId?: number): void {
        this._categoryService.getCategoriesByIds(categoryIds, this.currentTenantAOOId, (data) => {
            if (!data) {
                return;
            }
            let categoryChildren: CategoryModel[] = data;
            let parentNode: Telerik.Web.UI.RadTreeNode = parentId !== null
                ? this._rtvProcesses.findNodeByValue(parentId.toString())
                : this._rtvProcesses.get_nodes().getNode(0);
            for (let categoryChild of categoryChildren) {
                let categoryNode: Telerik.Web.UI.RadTreeNode = this.createTreeNodeFromCategoryModel(categoryChild, false);
                parentNode.get_nodes().add(categoryNode);
            }
            if (parentId !== null) {
                this.createProcessesNode(parentNode);
            }
            parentNode.hideLoadingStatus();
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.processViewPaneId);
            this.showNotificationException(error);
        });
    }

    private expandCategoryNode(expandedNode: Telerik.Web.UI.RadTreeNode): void {
        expandedNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        expandedNode.get_nodes().clear();
        let expandedCategoryId: number = +expandedNode.get_value();
        let categoryIds: number[] = this.getCategoryIds(expandedNode.get_level(), expandedCategoryId);
        this.loadCategories(categoryIds, expandedCategoryId);
    }

    private getCategoryIds(nodeLevel: number, parentCategoryId?: number): number[] {
        let expandedCategoryChildren: CategoryModel[] = parentCategoryId !== null
            ? this._processCategories.filter(x => +x.FullIncrementalPath.split('|')[nodeLevel] === parentCategoryId && x.EntityShortId !== parentCategoryId)
            : this._processCategories;
        let categoryIds: number[] = expandedCategoryChildren.map(x => +x.FullIncrementalPath.split('|')[nodeLevel + 1]).filter(x => !isNaN(x));
        let distinctCategoryIds: number[] = [];
        for (let categoryId of categoryIds) {
            if (distinctCategoryIds.indexOf(categoryId) === -1) {
                distinctCategoryIds.push(categoryId);
            }
        }
        return distinctCategoryIds;
    }

    private createTreeNodeFromCategoryModel(category: CategoryModel, expanded: boolean = true): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        let treeNodeDescription: string = `${category.Code}.${category.Name}`;
        let treeNodeImageUrl: string = "../Comm/images/Classificatore.gif";

        treeNode.set_text(treeNodeDescription);
        treeNode.set_value(`${category.EntityShortId}`);
        treeNode.set_imageUrl(treeNodeImageUrl);
        treeNode.set_contentCssClass((category.CategoryFascicles && category.CategoryFascicles.length > 0) ? "node-fascicle" : "node-no-fascicle");
        treeNode.get_attributes().setAttribute(TbltProcess.NodeType_TYPE_NAME, ProcessNodeType.Category);
        if (category.CategoryFascicles) {
            this.createProcessesNode(treeNode);
        }
        treeNode.set_expanded(expanded);

        return treeNode;
    }

    private createProcessesNode(parentNode: Telerik.Web.UI.RadTreeNode): void {
        let processesNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        processesNode.set_text(CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT);
        this.createNoItemsFoundNode(processesNode);
        parentNode.get_nodes().add(processesNode);
    }


    private createNoItemsFoundNode(parentNode: Telerik.Web.UI.RadTreeNode): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text(CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT);
        parentNode.get_nodes().add(emptyNode);
    }

    private loadProcesses(categoryId: number): void {
        this._processService.getProcessesByCategoryId(categoryId, (data) => {
            if (!data) return;
            this.processesModel = data;
            if (this.processesModel.length > 0) {
                this._rtvProcesses.findNodeByValue(categoryId.toString()).get_nodes().getNode(0).get_nodes().clear();
            };
            for (let process of this.processesModel) {
                var node = new Telerik.Web.UI.RadTreeNode();
                let isActive: boolean = process.EndDate === null || new Date(process.EndDate) > new Date();
                this.createNode(node, process.Name, process.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png",
                    ProcessNodeType.Process, false, null, isActive, process.Dossier.UniqueId,
                    this._rtvProcesses.findNodeByValue(categoryId.toString()).get_nodes().getNode(0));
                node.get_attributes().setAttribute(TbltProcess.ProcessType_TYPE_NAME, process.ProcessType);
                node.get_attributes().setAttribute(TbltProcess.Category_ID_TYPE, process.Category.EntityShortId);
                if (ProcessType[process.ProcessType.toString()] === ProcessType.Defined) {
                    this.createEmptyNode(node.get_nodes());
                }
                this._rtvProcesses.commitChanges();
            }
            if (this.defaultSelectedProcessId.length) {
                let treeNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.findNodeByValue(this.defaultSelectedProcessId);
                if (treeNode) {
                    treeNode.set_selected(true);
                    this.showNodeDetails(treeNode);
                }
            }

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
        if (expandedNode.get_level() === 0) {
            expandedNode.get_nodes().clear();
            this._initializeProcessesTree();
        }
        else if (expandedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.Category) {
            this.expandCategoryNode(expandedNode);
        }
        else if (expandedNode.get_text() === CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT) {
            this.loadProcesses(expandedNode.get_parent().get_value());
        }
        else if (expandedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.Process) {
            this.expandNodeLogic(expandedNode);
            this.loadData(expandedNode.get_attributes().getAttribute("idDossier"), 0, expandedNode.get_value());
        }
        else if (expandedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) !== ProcessNodeType.Category) {
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

    private showNodeDetails(selectedNode: Telerik.Web.UI.RadTreeNode): void {
        this.initializeNodeClicked(selectedNode);
        if (selectedNode.get_level() === 0
            || selectedNode.get_text() === CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT) {
            $(`#${this._uscProcessDetails.pnlDetailsId}`).hide();
            this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, false);
        }
        else if (selectedNode.get_text() === CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT) {
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CREATE).set_enabled(true);
            $(`#${this._uscProcessDetails.pnlDetailsId}`).hide();
            this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, false);
        }
        else {
            switch (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME)) {
                case ProcessNodeType.Category: {
                    this.initializeCategoryNodeDetails(selectedNode);
                    break;
                }
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

    rtvProcesses_nodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let selectedNode = args.get_node();
        this.showNodeDetails(selectedNode);
    }

    private initializeNodeClicked(selectedNode: Telerik.Web.UI.RadTreeNode): void {
        this._uscProcessDetails = <uscProcessDetails>$(`#${this.uscProcessDetailsId}`).data();
        this._uscProcessDetails.clearProcessDetails();
        this.enableFolderToolbarButtons(false);
        $(`#${this._uscProcessDetails.pnlDetailsId}`).show();
        document.getElementById(this._uscProcessDetails.uscFascicleFoldersId.replace("_pageContent", "").concat("_pnlTitle")).style.position = "absolute";
        document.getElementById(this._uscProcessDetails.uscFascicleFoldersId.replace("_pageContent", "").concat("_pnlFolderToolbar")).style.position = "absolute";
        this._uscProcessDetails.clearFascicleInputs();
        this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, true);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.InformationDetails_PanelName, true);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.CategoryInformationDetails_PanelName, false);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.WorkflowDetails_PanelName, false);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.FascicleDetails_PanelName, false);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.RoleDetails_PanelName, true);
    }

    private initializeCategoryNodeDetails(selectedNode: Telerik.Web.UI.RadTreeNode): void {
        uscProcessDetails.selectedCategoryId = +selectedNode.get_value();
        uscProcessDetails.selectedEntityType = ProcessNodeType.Category;
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.InformationDetails_PanelName, false);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.CategoryInformationDetails_PanelName, true);
        this._uscProcessDetails.setCategoryDetails();
    }

    private initializeProcessNodeDetails(selectedNode: Telerik.Web.UI.RadTreeNode): void {
        uscProcessDetails.selectedProcessId = selectedNode.get_value();
        uscProcessDetails.selectedEntityType = ProcessNodeType.Category;
        uscProcessDetails.selectedCategoryId = selectedNode.get_attributes().getAttribute(TbltProcess.Category_ID_TYPE);
        this._uscProcessDetails.setProcessDetails('', true);
        this.enableFolderToolbarButtons(true);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_DELETE).set_enabled(false);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CREATE_PROCESS_FASCICLE_TEMPLATE).set_enabled(false);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CLONE).set_enabled(false);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_COPYPFT).set_enabled(false);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_PASTEPFT).set_enabled(false);
    }

    private initializeDossierFolderNodeDetails(selectedNode: Telerik.Web.UI.RadTreeNode): void {
        this._uscProcessDetails.setPanelLoading(uscProcessDetails.WorkflowDetails_PanelName, true);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.WorkflowDetails_PanelName, true);
        let processParentNode: Telerik.Web.UI.RadTreeNode = this.getProcessNodeByChild(selectedNode);
        uscProcessDetails.selectedDossierFolderId = selectedNode.get_value();
        uscProcessDetails.selectedProcessId = processParentNode.get_value();
        uscProcessDetails.selectedEntityType = ProcessNodeType.DossierFolder;
        this.enableFolderToolbarButtons(true);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CLONE).set_enabled(true);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_COPYPFT).set_enabled(false);
        if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL) == null ||
            sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL) == "[]") {
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_PASTEPFT).set_enabled(false);
        } else {
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_PASTEPFT).set_enabled(true);
        }
        this._uscProcessDetails.setProcessDetails(selectedNode.get_text(), false);
        this._uscProcessDetails.setDossierFolderRoles();

        this._uscProcessDetails.setDossierFolderWorkflowRepositories();
    }

    private initializeProcessFascicleTemplateNodeDetails(selectedNode: Telerik.Web.UI.RadTreeNode): void {
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.FascicleDetails_PanelName, true);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.RoleDetails_PanelName, false);
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_PROCESS_FASCICLE_TEMPLATE_NAME, selectedNode.get_text())
        document.getElementById("pnlMainFascicleFolder").style.position = "initial";
        this.enableFolderToolbarButtons(false);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_DELETE).set_enabled(true);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_MODIFY).set_enabled(true);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_COPYPFT).set_enabled(true);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_PASTEPFT).set_enabled(false);
        uscProcessDetails.selectedProcessFascicleTemplateId = selectedNode.get_value();
        uscProcessDetails.selectedDossierFolderId = selectedNode.get_parent().get_value();
        uscProcessDetails.selectedProcessId = this.getProcessNodeByChild(selectedNode).get_value();
        uscProcessDetails.selectedEntityType = ProcessNodeType.ProcessFascicleTemplate;
        this._uscProcessDetails.setProcessDetails(selectedNode.get_parent().get_text(), false);
        this._uscProcessDetails.setFascicle();
    }

    private loadData(currentNodeValue: string, status: number, idProcess?: string): void {
        this._ajaxLoadingPanel.show(this.rtvProcessesId);
        this._dossierFolderService.getProcessFascicleChildren(currentNodeValue, status, (data) => {
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
            this.processFascicleTemplatesModel = [];
            for (let tpftm of data) {
                if (tpftm) {
                    this.tempPFTModel.push(tpftm);
                }
            }
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
        while (node.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) !== ProcessNodeType.Process) {
            node = node.get_parent();
        }
        return node;
    }

    rbProcessInsert_onCLick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
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
        this.processRoles = [];
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
        $("#cloneDossierFolder").hide();
    }

    enableFolderToolbarButtons(value: boolean): void {
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CREATE).set_enabled(value);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CREATE_PROCESS_FASCICLE_TEMPLATE).set_enabled(value);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_MODIFY).set_enabled(value);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_DELETE).set_enabled(value);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CLONE).set_enabled(value);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_COPYPFT).set_enabled(value);
        this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_PASTEPFT).set_enabled(value);
    }

    folderToolBar_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        this.hideInsertInputs();
        switch (args.get_item().get_value()) {
            case TbltProcess.TOOLBAR_CREATE: {
                this.clearInputs();
                let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.get_selectedNode();
                if (selectedNode.get_text() === CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT) {
                    this.selectedProcessId = "";
                    this._uscCategoryRest.disableButtons();
                    let categoryNodeText: string = selectedNode.get_parent().get_text();
                    let categoryNodeValue: number = +selectedNode.get_parent().get_value();
                    let category: CategoryTreeViewModel = <CategoryTreeViewModel>{};
                    category.Code = +categoryNodeText.split(".")[0];
                    category.Name = categoryNodeText.split(".")[1];
                    category.IdCategory = categoryNodeValue;
                    this._uscCategoryRest.updateSessionStorageSelectedCategory(category);
                    this._uscCategoryRest.populateCategotyTree(category);
                    $(`#${this.uscCategoryRestId}`).triggerHandler(uscCategoryRest.ADDED_EVENT, category.IdCategory);
                    $("#insertProcess").show();
                    this._rwInsert.set_title("Aggiungi serie");
                }
                switch (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME)) {
                    case ProcessNodeType.Process:
                    case ProcessNodeType.DossierFolder: {
                        this.selectedDossierFolderId = "";
                        $("#insertDossierFolder").show();
                        this._rwInsert.set_title("Aggiungi volume");
                        break;
                    }
                }
                this._rwInsert.show();
                break;
            }
            case TbltProcess.TOOLBAR_CREATE_PROCESS_FASCICLE_TEMPLATE: {
                this.selectedProcessFascicleTemplateId = "";
                $("#insertFascicleTemplate").show();
                this._rwInsert.set_title("Aggiungi modello di fascicolo di processo");
                this._rwInsert.show();
                break;
            }
            case TbltProcess.TOOLBAR_DELETE: {
                let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.get_selectedNode();
                switch (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME)) {
                    case ProcessNodeType.Process: {
                        let yesterdayDate = new Date();
                        yesterdayDate.setDate(new Date().getDate() - 1);
                        this._ajaxLoadingPanel.show(this.rtvProcessesId);
                        this.removeProcess(yesterdayDate);
                        break;
                    }
                    case ProcessNodeType.DossierFolder: {
                        if (uscProcessDetails.processFascicleWorkflowRepositories.length > 0) {
                            alert("Impossibile eliminare il volume. Esiste un flusso di lavoro associato.");
                            return;
                        }
                        let dossierFolder: DossierFolderModel = <DossierFolderModel>{};
                        dossierFolder.UniqueId = this._rtvProcesses.get_selectedNode().get_value();
                        if (this._rtvProcesses.get_selectedNode().get_parent().get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.Process) {
                            dossierFolder.ParentInsertId = this.getProcessNodeByChild(this._rtvProcesses.get_selectedNode()).get_attributes().getAttribute("idDossier");
                        }
                        else {
                            dossierFolder.ParentInsertId = this._rtvProcesses.get_selectedNode().get_parent().get_value();
                        }
                        this._ajaxLoadingPanel.show(this.rtvProcessesId);
                        if (window.confirm("Vuoi eliminare il volume selezionato?")) {
                            this._dossierFolderService.deleteDossierFolder(dossierFolder, (data) => {
                                this._rtvProcesses.get_selectedNode().get_parent().get_nodes().remove(this._rtvProcesses.get_selectedNode());
                                this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                            }, (error) => {
                                this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                                this.showNotificationException(error);
                            });
                        } else {
                            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                        }
                        break;
                    }
                    case ProcessNodeType.ProcessFascicleTemplate: {
                        let yesterdayDate = new Date();
                        this._ajaxLoadingPanel.show(this.rtvProcessesId);
                        this.removeFascicleTemplate(yesterdayDate);
                        break;
                    }
                }
                break;
            }
            case TbltProcess.TOOLBAR_MODIFY: {
                this.hideInsertInputs();
                let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.get_selectedNode();
                switch (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME)) {
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
                        this._rwInsert.set_title("Modifica serie");
                        break;
                    }
                    case ProcessNodeType.ProcessFascicleTemplate: {
                        this.selectedProcessFascicleTemplateId = this._rtvProcesses.get_selectedNode().get_value();
                        this._rtbFascicleTemplateName.set_value(this._rtvProcesses.get_selectedNode().get_text());
                        $("#insertFascicleTemplate").show();
                        this._rwInsert.set_title("Modifica modello di fascicolo");
                        break;
                    }
                    case ProcessNodeType.DossierFolder: {
                        this.selectedDossierFolderId = this._rtvProcesses.get_selectedNode().get_value();
                        this._rtbDossierFolderName.set_value(this._rtvProcesses.get_selectedNode().get_text());
                        $("#insertDossierFolder").show();
                        this._rwInsert.set_title("Modifica volume");
                        break;
                    }
                }
                this._rwInsert.show();
                break;
            }
            case TbltProcess.TOOLBAR_CLONE: {
                this.clearInputs();
                this.selectedDossierFolderId = "";
                this._rtbCloneDossierFolderName.set_value("");
                $("#cloneDossierFolder").show();
                this._rwInsert.set_title("Duplica volume");
                this._rwInsert.show();
                break;
            }

            case TbltProcess.TOOLBAR_COPYPFT: {
                let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.get_selectedNode();
                let fascicleTemplate: ProcessFascicleTemplateModel = <ProcessFascicleTemplateModel>{};
                fascicleTemplate.Name = selectedNode.get_text();
                fascicleTemplate.StartDate = new Date();
                fascicleTemplate.EndDate = null;
                this._uscProcessDetails.populateFascicleTemplateInfo().then((jsonModel) => {
                    fascicleTemplate.JsonModel = jsonModel;
                    fascicleTemplate.Process = <ProcessModel>{};
                    fascicleTemplate.Process.UniqueId = this.getProcessNodeByChild(selectedNode).get_value();

                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL, JSON.stringify(fascicleTemplate));
                });
                break;
            }

            case TbltProcess.TOOLBAR_PASTEPFT: {
                let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.get_selectedNode();
                let fascicleTemplate: ProcessFascicleTemplateModel = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL));
                fascicleTemplate.DossierFolder = <DossierFolderModel>{};
                fascicleTemplate.DossierFolder.UniqueId = selectedNode.get_value();

                this.insertPFT(selectedNode, fascicleTemplate);
                break;
            }
        }
    }

    rbConfirmInsert_onCLick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.get_selectedNode();

        if ($("#cloneDossierFolder").is(":visible")) {
            return this.cloneDossierFolder(selectedNode);
        }

        if ($("#insertDossierFolder").is(":visible")) {
            return this.insertOrUpdateDossierFolder(selectedNode);
        }

        if ($("#insertProcess").is(":visible")) {
            return this.insertOrUpdateProcess(selectedNode);
        }

        if ($("#insertFascicleTemplate").is(":visible")) {
            return this.insertOrUpdateFascicleTemplate(selectedNode);
        }
    }

    private insertPFT(selectedNode: Telerik.Web.UI.RadTreeNode, fascicleTemplate: ProcessFascicleTemplateModel) {
        if (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) != ProcessNodeType.DossierFolder) {
            alert("Seleziona un nodo di volume");
            return;
        }

        let imageUrl: string;
        if (fascicleTemplate.EndDate === null || new Date(fascicleTemplate.EndDate) < new Date()) {
            imageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
        }
        else {
            imageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
        }

        this.insertProcessFascicleTemplate(selectedNode, fascicleTemplate, imageUrl);
    }

    ///Insert or Update Process Fascicle Template
    private insertOrUpdateFascicleTemplate(selectedNode: Telerik.Web.UI.RadTreeNode) {
        let exists: boolean = this.selectedProcessFascicleTemplateId !== "";
        let fascicleTemplate: ProcessFascicleTemplateModel = <ProcessFascicleTemplateModel>{};
        fascicleTemplate.Name = this._rtbFascicleTemplateName.get_value();
        fascicleTemplate.StartDate = new Date();
        fascicleTemplate.EndDate = null;
        fascicleTemplate.Process = <ProcessModel>{};
        fascicleTemplate.Process.UniqueId = this.getProcessNodeByChild(selectedNode).get_value();
        fascicleTemplate.DossierFolder = <DossierFolderModel>{};
        fascicleTemplate.DossierFolder.UniqueId = selectedNode.get_value();
        let imageUrl: string;
        if (fascicleTemplate.EndDate === null || new Date(fascicleTemplate.EndDate) < new Date()) {
            imageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
        }
        else {
            imageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
        }
        if (exists) {
            this.updateProcessFascicleTemplate(selectedNode, fascicleTemplate, imageUrl);
        } else {
            this.insertProcessFascicleTemplate(selectedNode, fascicleTemplate, imageUrl);
        }
    }
    private updateProcessFascicleTemplate(selectedNode: Telerik.Web.UI.RadTreeNode, fascicleTemplate: ProcessFascicleTemplateModel, imageUrl: string): void {
        this._ajaxLoadingPanel.show(this.rtvProcessesId);
        fascicleTemplate.UniqueId = this.selectedProcessFascicleTemplateId;
        this._uscProcessDetails.populateFascicleTemplateInfo().then((jsonModel) => {
            fascicleTemplate.JsonModel = jsonModel;
            this._fascicleTemplateService.update(fascicleTemplate, (data) => {
                this.createNode(selectedNode, data.Name, data.UniqueId, imageUrl,
                    ProcessNodeType.ProcessFascicleTemplate, false, null, null, null, null);
                this._rwInsert.close();
                this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            }, (error) => {
                this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                this.showNotificationException(error);
            });
        });
    }
    private insertProcessFascicleTemplate(selectedNode: Telerik.Web.UI.RadTreeNode, fascicleTemplate: ProcessFascicleTemplateModel, imageUrl: string): void {
        this._ajaxLoadingPanel.show(this.rtvProcessesId);
        if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL) == null ||
            sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL) == "[]") {
            fascicleTemplate.JsonModel = "";
        }
        this._fascicleTemplateService.insert(fascicleTemplate, (data) => {
            let hasChildren: number = selectedNode.get_nodes().get_count();
            let isExpandable: boolean = hasChildren > 0 ? false : true;
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            this.createNode(node, data.Name, data.UniqueId, imageUrl,
                ProcessNodeType.ProcessFascicleTemplate, true, null, null, null, selectedNode, isExpandable);
            this._rwInsert.close();
            this.processFascicleTemplatesModel.push(data);
            this.tempPFTModel.push(data);
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            this.showNotificationException(error);
        });
    }
    ///end Insert or Update Process Fascicle Template

    ///Insert or Update Process 
    private insertOrUpdateProcess(selectedNode: Telerik.Web.UI.RadTreeNode) {
        let exists: boolean = this.selectedProcessId !== "";
        let process: ProcessModel = <ProcessModel>{};

        let processCategory: CategoryModel = this._uscCategoryRest.getSelectedCategory();

        if (!processCategory) return;

        process.Name = this._rtbProcessName.get_value();
        process.Dossier = <DossierModel>{};
        if (selectedNode.get_text() !== CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT) {
            process.Dossier.UniqueId = this.getProcessNodeByChild(selectedNode).get_value();
        }
        process.Category = new CategoryModel();
        process.Category.EntityShortId = processCategory.EntityShortId;
        process.StartDate = new Date();
        process.EndDate = null;
        process.Dossier = <DossierModel>{};
        process.Roles = this.processRoles;
        process.Note = this._rcbProcessNote.get_value();

        if (exists) {
            this.updateProcess(selectedNode, process);
        }
        else {
            this.insertProcess(selectedNode);
        }

    }
    private updateProcess(selectedNode: Telerik.Web.UI.RadTreeNode, process: ProcessModel): void {
        this._ajaxLoadingPanel.show(this.rtvProcessesId);
        process.UniqueId = this.selectedProcessId;
        this._processService.update(process, (data) => {
            let isActive: boolean = data.EndDate === null || new Date(data.EndDate) > new Date();
            this.createNode(selectedNode, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png",
                ProcessNodeType.Process, false, null, isActive, data.Dossier.UniqueId, null, false);

            let processModelToUpdate: ProcessModel = this.processesModel.filter(x => x.UniqueId === data.UniqueId)[0];
            let processModelToUpdateIdx = this.processesModel.indexOf(processModelToUpdate);
            this.processesModel[processModelToUpdateIdx] = data;
            this.processesModel[processModelToUpdateIdx].Roles = this.processRoles;

            this._rwInsert.close();
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);

            this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, true);
            this._uscProcessDetails = <uscProcessDetails>$(`#${this.uscProcessDetailsId}`).data();
            this._uscProcessDetails.clearProcessDetails();
            uscProcessDetails.selectedProcessId = data.UniqueId;
            this._uscProcessDetails.setProcessDetails('', true);
            this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, false);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            this.showNotificationException(error);
        });
    }
    private insertProcess(selectedNode: Telerik.Web.UI.RadTreeNode): void {
        let process: ProcessModel = <ProcessModel>{};
        process.Name = this._rtbProcessName.get_value();
        process.Category = new CategoryModel();
        process.Category.EntityShortId = this._uscCategoryRest.getSelectedCategory().EntityShortId;
        process.StartDate = new Date();
        process.Roles = this.processRoles;
        process.Note = this._rcbProcessNote.get_value();

        this._ajaxLoadingPanel.show(this.rtvProcessesId);
        this._processService.insert(process, (data) => {
            let isActive: boolean = data.EndDate === null || new Date(data.EndDate) > new Date();
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            if (selectedNode.get_expanded() && selectedNode.get_nodes().getNode(0).get_text() === CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT) {
                selectedNode.get_nodes().clear();
            }
            this.createNode(node, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png",
                ProcessNodeType.Process, true, null, isActive, data.Dossier.UniqueId, selectedNode, false);
            data.Roles = this.processRoles;
            if (!this.processesModel) {
                this.processesModel = [];
            }
            this.processesModel.push(data);
            this._rwInsert.close();
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            this.showNotificationException(error);
        });
    }
    ///end Insert or Update Process

    ///Insert or Update Dossier Folder
    private insertOrUpdateDossierFolder(selectedNode: Telerik.Web.UI.RadTreeNode) {
        if (this._rtbDossierFolderName.get_value()) {
            let exists: boolean = this.selectedDossierFolderId !== "";
            let dossierFolder: DossierFolderModel = <DossierFolderModel>{};
            dossierFolder.Name = this._rtbDossierFolderName.get_value();

            if (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) !== ProcessNodeType.Process) {
                dossierFolder.ParentInsertId = exists ? selectedNode.get_parent().get_value() : selectedNode.get_value();
            }
            else {
                dossierFolder.ParentInsertId = this.getProcessNodeByChild(selectedNode).get_attributes().getAttribute("idDossier");
            }
            dossierFolder.Dossier = <DossierModel>{};
            dossierFolder.Dossier.UniqueId = this.getProcessNodeByChild(selectedNode).get_attributes().getAttribute("idDossier");
            if (exists) {
                this.updateDossierFolder(selectedNode, dossierFolder);
            }
            else {
                this.insertDossierFolder(selectedNode, dossierFolder);
            }
        }
    }
    private updateDossierFolder(selectedNode: Telerik.Web.UI.RadTreeNode, dossierFolder: DossierFolderModel): void {
        this._ajaxLoadingPanel.show(this.rtvProcessesId);
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
    private insertDossierFolder(selectedNode: Telerik.Web.UI.RadTreeNode, dossierFolder: DossierFolderModel): void {
        this._ajaxLoadingPanel.show(this.rtvProcessesId);

        this._processService.getById(this.getProcessNodeByChild(selectedNode).get_value(), (data: ProcessModel) => {
            dossierFolder.DossierFolderRoles = data.Roles
                .map<DossierFolderRoleModel>(role => <DossierFolderRoleModel>{
                    Role: role
                });

            this._dossierFolderService.insertDossierFolder(dossierFolder, null, (data) => {
                let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                if (selectedNode.get_nodes().get_count() > 0 && selectedNode.get_nodes().getNode(0).get_text() === "") {
                    selectedNode.get_nodes().clear();
                }
                this.createNode(node, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png",
                    ProcessNodeType.DossierFolder, true, data.Status, null, null, selectedNode, false);
                this._rwInsert.close();
                this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            }, (error) => {
                this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                this.showNotificationException(error);
            });
        });
    }
    ///end Insert or Update Dossier FOlder

    ///Clone dossier folder
    private cloneDossierFolder(sourceNode: Telerik.Web.UI.RadTreeNode) {
        this._ajaxLoadingPanel.show(this.rtvProcessesId);
        let dossierFolder: DossierFolderModel = <DossierFolderModel>{};
        dossierFolder.Name = this._rtbCloneDossierFolderName.get_textBoxValue();
        if (dossierFolder.Name === undefined || dossierFolder.Name === "") {
            return;
        }
        if (this._rtvProcesses.get_selectedNode().get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.DossierFolder) {
            dossierFolder.ParentInsertId = this.getProcessNodeByChild(this._rtvProcesses.get_selectedNode()).get_attributes().getAttribute("idDossier");
        }
        else {
            dossierFolder.ParentInsertId = this._rtvProcesses.get_selectedNode().get_parent().get_value();
        }
        dossierFolder.JsonMetadata = sourceNode.get_value();
        dossierFolder.Dossier = <DossierModel>{};
        dossierFolder.Dossier.UniqueId = this.getProcessNodeByChild(sourceNode).get_attributes().getAttribute("idDossier");
        //TODO: Remove ActionTypes in DocSuite 9.0X (move webapi logic in Store Procedure) 
        this._dossierFolderService.insertDossierFolder(dossierFolder, InsertActionType.CloneProcessFolder, (data) => {
            //insert node
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            if (sourceNode.get_nodes().get_count() > 0 && sourceNode.get_nodes().getNode(0).get_text() === "") {
                sourceNode.get_nodes().clear();
            }

            //update node hierarchy
            this._dossierFolderService.updateDossierFolder(data, UpdateActionType.CloneProcessFolder, (data) => {
                this.createNode(node, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png",
                    ProcessNodeType.DossierFolder, true, data.Status, null, null, this.getProcessNodeByChild(sourceNode), false);

                //to be able to see the plus sign
                let dummyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                this.createNode(dummyNode, "", "", "../App_Themes/DocSuite2008/imgset16/folder_closed.png",
                    ProcessNodeType.DossierFolder, true, data.Status, null, null, node, false);

                this._rwInsert.close();
                this._ajaxLoadingPanel.hide(this.rtvProcessesId);

            })


        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            this.showNotificationException(error);
        });
    }
    ///end Clone dossier folder

    private createNode(node: Telerik.Web.UI.RadTreeNode, text: string, value: string, imagePath: string, nodeType: ProcessNodeType, isExpanded: boolean,
        dossierFolderStatus?: DossierFolderStatus, isActive?: boolean, idDossier?: string, parentNode?: Telerik.Web.UI.RadTreeNode, expandParent: boolean = true): void {
        node.set_value(value);
        node.set_text(text);
        node.get_attributes().setAttribute(TbltProcess.NodeType_TYPE_NAME, nodeType);
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
        }
        if (expandParent && parentNode !== null) {
            parentNode.expand();
        }
    }

    private removeFascicleTemplate(endDate: Date): void {
        let fascicleTemplate: ProcessFascicleTemplateModel = <ProcessFascicleTemplateModel>{};
        fascicleTemplate.UniqueId = this._rtvProcesses.get_selectedNode().get_value();
        for (let fascicleToFind of this.tempPFTModel) {
            if (fascicleToFind.UniqueId == fascicleTemplate.UniqueId) {
                fascicleTemplate = fascicleToFind;
                break;
            }
        }

        fascicleTemplate.EndDate = endDate;
        if (window.confirm("Vuoi eliminare modello di fascicolo selezionato?")) {
            this._fascicleTemplateService.update(fascicleTemplate, (data) => {
                let imgUrl: string = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
                this._rtvProcesses.get_selectedNode().set_imageUrl(imgUrl);
                this._rtvProcesses.get_selectedNode().get_attributes().setAttribute("IsActive", false);
                this._rtvProcesses.commitChanges();
                this._rwInsert.close();
                this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            }, (error) => {
                this._ajaxLoadingPanel.hide(this.rtvProcessesId);
                this.showNotificationException(error);
            });
        } else {
            this._ajaxLoadingPanel.hide(this.rtvProcessesId);
        }
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

    //TODO: filtering feature will be completed in 9.0X
    //filterToolbar_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
    //    switch (args.get_item().get_value()) {
    //        case "search": {
    //            this._ajaxLoadingPanel.show(this.processViewPaneId);
    //            this._uscProcessDetails = <uscProcessDetails>$(`#${this.uscProcessDetailsId}`).data();
    //            this._uscProcessDetails.clearProcessDetails();
    //            $(`#${this._uscProcessDetails.pnlDetailsId}`).hide();
    //            this._rtvProcesses.get_nodes().getNode(0).get_nodes().clear();
    //            let processSearchName: string = this._rtbProcessSearchName.get_value();
    //            let processActiveItem: any = this._filterToolbar.findItemByValue("processActive");
    //            let processDisabledItem: any = this._filterToolbar.findItemByValue("processDisabled");
    //            let callLoadProcesses: void = processActiveItem.get_checked()
    //                ? processDisabledItem.get_checked() ? this.loadProcesses(processSearchName, null) : this.loadProcesses(processSearchName, true)
    //                : processDisabledItem.get_checked() ? this.loadProcesses(processSearchName, false) : this.loadProcesses(processSearchName, null);
    //            break;
    //        }
    //    }
    //}

    populateProcessInputs(processId: string): void {
        let process: ProcessModel = <ProcessModel>{};
        for (let processToFind of this.processesModel) {
            if (processToFind.UniqueId === processId) {
                process = processToFind;
                break;
            }
        }
        this._rtbProcessName.set_value(process.Name);
        this._rcbProcessNote.set_value(process.Note);
        let category: CategoryTreeViewModel = <CategoryTreeViewModel>{};
        category.Code = process.Category.Code;
        category.Name = process.Category.Name;
        category.IdCategory = process.Category.EntityShortId;
        this._uscCategoryRest.updateSessionStorageSelectedCategory(category);
        this._uscCategoryRest.populateCategotyTree(category);
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