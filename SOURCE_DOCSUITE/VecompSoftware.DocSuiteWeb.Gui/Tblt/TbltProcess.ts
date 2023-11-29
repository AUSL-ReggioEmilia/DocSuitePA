import ServiceConfiguration = require('App/Services/ServiceConfiguration');
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
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import CommonSelCategoryRest = require('UserControl/CommonSelCategoryRest');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');
import PaginationModel = require('App/Models/Commons/PaginationModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');

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
    treeViewNodesPageSize: number;

    processesModel: ProcessModel[];
    processFascicleTemplatesModel: ProcessFascicleTemplateModel[];
    private tempPFTModel: ProcessFascicleTemplateModel[] = [];
    selectedDossierFolderId: string;
    selectedProcessId: string;
    selectedProcessFascicleTemplateId: string;
    processRoles: RoleModel[];
    categoryModel: CategoryModel;

    private _serviceConfigurations: ServiceConfiguration[];
    private _categoryService: CategoryService;
    private _processService: ProcessService;
    private _dossierFolderService: DossierFolderService;
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
    private static Category_ID_TYPE = "idCategory";
    private static ATTRNAME_IDDOSSIER: string = "idDossier";
    private static ATTRNAME_ISACTIVE: string = "IsActive";
    private static ATTRNAME_DOSSIERFOLDERSTATUS: string = "DossierFolderStatus";
    private static NOELEMENTS_NODE_LABEL: string = "Nessun elemento trovato";
    private static LOADMORE_NODE_LABEL: string = "Carica più elementi";
    private static LOAD_MORE_NODE_IMAGEURL: string = "../App_Themes/DocSuite2008/imgset16/add.png";
    private static TOTAL_CHILDREN_COUNT_ATTRNAME: string = "totalChildrenCount";
    private static CSSCLASS_FASCICLE_NODE: string = "node-fascicle";
    private static CSSCLASS_NOTFASCICLE_NODE: string = "node-no-fascicle";
    private static FOLDER_CLOSED_IMGURL: string = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
    private static CATEGORY_NODE_IMGURL: string = "../Comm/images/Classificatore.gif";
    private static FASCICLECLOSED_IMGURL: string = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
    private static FASCICLEOPENED_IMGURL: string = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
    private static PROCESS_NODE_IMGURL: string = "../App_Themes/DocSuite2008/imgset16/process.png";
    private static BOLD_CSSCLASS: string = "dsw-text-bold";
    private static PROCESS_ACTIVE_FILTERBTNVAL: string = "processActive";
    private static SEARCH_BTNVAL: string = "search";

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
    private _nodeExpandingActionHandlerMap: Map<ProcessNodeType, (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel) => JQueryPromise<void>>;
    private _createdCategoriesCache: Telerik.Web.UI.RadTreeNode[] = [];
    private _currentActiveRequestsCount: number = 0;

    private get _defaultPaginationModel(): PaginationModel {
        return new PaginationModel(0, this.treeViewNodesPageSize);
    }

    private get filterInputValue(): string {
        return this._rtbProcessSearchName.get_value();
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize(): void {
        this.initializeServices();
        this.initializeControls();
        this.initializeUserControls();
        this._initializeNodesExpandingActionHandlersMap();
        this._ajaxLoadingPanel.show(this.processViewPaneId);
        this.enableFolderToolbarButtons(false);
        this._initializeTreeView();
        this.processFascicleTemplatesModel = [];
        this.processRoles = [];
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL);
    }

    initializeServices(): void {
        let processConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.Process_TYPE_NAME);
        this._processService = new ProcessService(processConfiguration);
        let dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.DossierFolder_TYPE_NAME);
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
        let fascicleTemplateConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.ProcessFascicleTemplate_TYPE_NAME);
        this._fascicleTemplateService = new ProcessFascicleTemplateService(fascicleTemplateConfiguration);
        let categoryServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.Category_TYPE_NAME);
        this._categoryService = new CategoryService(categoryServiceConfiguration);

    }

    initializeControls(): void {
        this._ajaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rtvProcesses = <Telerik.Web.UI.RadTreeView>$find(this.rtvProcessesId);
        this._rtvProcesses.add_nodeClicked(this.rtvProcesses_nodeClicked);
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
        this._filterToolbar.add_buttonClicked(this.filterToolbar_onClick);
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

    private _initializeNodesExpandingActionHandlersMap(): void {
        this._nodeExpandingActionHandlerMap = new Map<ProcessNodeType, (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel) => JQueryPromise<void>>();
        this._nodeExpandingActionHandlerMap.set(ProcessNodeType.Root, this._rootNodeExpandedActionHandler);
        this._nodeExpandingActionHandlerMap.set(ProcessNodeType.LoadMore, this._loadMoreNodeExpandedActionHandler);
        this._nodeExpandingActionHandlerMap.set(ProcessNodeType.Category, this._categoryNodeExpandedActionHandler);
        this._nodeExpandingActionHandlerMap.set(ProcessNodeType.Process, this._processNodeExpandedActionHandler);
        this._nodeExpandingActionHandlerMap.set(ProcessNodeType.LoadProcessesAndFolders, this._processesAndFoldersNodeExpandedActionHandler);
        this._nodeExpandingActionHandlerMap.set(ProcessNodeType.DossierFolder, this._dossierFolderNodeExpandedActionHandler);
    }

    private addCategoryEventHandler = (data: JQueryEventObject, args: any): void => {
        let categoryId = args;
        if (categoryId) {
            this._uscProcessRoleRest.clearRoleTreeView(false);
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

    private _treeViewRootNode: Telerik.Web.UI.RadTreeNode;
    private get TreeviewRootNode(): Telerik.Web.UI.RadTreeNode {
        if (!this._treeViewRootNode) {
            this._treeViewRootNode = this._rtvProcesses.get_nodes().getNode(0);
        }

        return this._treeViewRootNode;
    }

    private _initializeTreeView(): void {

        if (this.TreeviewRootNode.get_nodes().get_count() > 0) {
            this.TreeviewRootNode.get_nodes().clear();
            this.TreeviewRootNode.set_selected(true);
            this.showNodeDetails(this.TreeviewRootNode);
        }

        this._setNodeAttribute(this.TreeviewRootNode, TbltProcess.NodeType_TYPE_NAME, ProcessNodeType.Root);

        this.TreeviewRootNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        this._categoryService.getTenantAOORootCategory(this.currentTenantAOOId, (tenantAOOCategory: CategoryModel) => {

            if (!tenantAOOCategory) {
                this.TreeviewRootNode.hideLoadingStatus();
                return;
            }

            this.TreeviewRootNode.set_value(tenantAOOCategory.EntityShortId);

            this._loadCategories(this.TreeviewRootNode, this._defaultPaginationModel)
                .done(() => {
                    this.TreeviewRootNode.set_expanded(true);

                    const expandedNodeParentTotalChildrenCount: number = this._getNodeAttribute<number>(this.TreeviewRootNode, TbltProcess.TOTAL_CHILDREN_COUNT_ATTRNAME);
                    this._appendLoadMoreNode(this.TreeviewRootNode, expandedNodeParentTotalChildrenCount);
                })
                .always(() => {
                    this.TreeviewRootNode.hideLoadingStatus();
                    this._ajaxLoadingPanel.hide(this.processViewPaneId);
                })
                .fail(this.showNotificationException);
        });
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

    private _findProcessesByCategoryId(categoryId: number, paginationModel?: PaginationModel): JQueryPromise<ODATAResponseModel<ProcessModel>> {
        let defferedRequest: JQueryDeferred<ODATAResponseModel<ProcessModel>> = $.Deferred<ODATAResponseModel<ProcessModel>>();

        this._processService.getProcessesByCategoryId(categoryId, defferedRequest.resolve, defferedRequest.reject, paginationModel);

        return defferedRequest.promise();
    }

    private _findProcessesByName(name: string, loadOnlyActiveProcesses: boolean, tenantAOOId?: string): JQueryPromise<ProcessModel[]> {
        let defferedRequest: JQueryDeferred<ProcessModel[]> = $.Deferred<ProcessModel[]>();

        this._processService.findProcessesByName(name, loadOnlyActiveProcesses, defferedRequest.resolve, defferedRequest.reject, tenantAOOId, ["Category($expand=Parent, CategoryFascicles)", "Dossier"]);

        return defferedRequest.promise();
    }

    private _findProcessDossierFolders(processId: string, paginationModel?: PaginationModel): JQueryPromise<DossierFolderModel[] | ODATAResponseModel<DossierFolderModel>> {
        let defferedRequest: JQueryDeferred<DossierFolderModel[] | ODATAResponseModel<DossierFolderModel>> = $.Deferred<DossierFolderModel[] | ODATAResponseModel<DossierFolderModel>>();

        this._dossierFolderService.getDossierFoldersByProcessId(processId, defferedRequest.resolve, defferedRequest.reject, paginationModel, true);

        return defferedRequest.promise();
    }

    private _findDossierFolderFascicleTemplates(dossierFolderId: string, paginationModel?: PaginationModel): JQueryPromise<ProcessFascicleTemplateModel[] | ODATAResponseModel<ProcessFascicleTemplateModel>> {
        let defferedRequest: JQueryDeferred<ProcessFascicleTemplateModel[] | ODATAResponseModel<ProcessFascicleTemplateModel>> = $.Deferred<ProcessFascicleTemplateModel[] | ODATAResponseModel<ProcessFascicleTemplateModel>>();

        this._fascicleTemplateService.getFascicleTemplateByDossierFolderId(dossierFolderId, defferedRequest.resolve, defferedRequest.reject, paginationModel);

        return defferedRequest.promise();
    }

    private _findDossierFolderChildren(dossierFolderId: string, paginationModel?: PaginationModel): JQueryPromise<DossierSummaryFolderViewModel[]> {
        let defferedRequest: JQueryDeferred<DossierSummaryFolderViewModel[]> = $.Deferred<DossierSummaryFolderViewModel[]>();

        this._dossierFolderService.getDossierFolderChildren(dossierFolderId, paginationModel, defferedRequest.resolve, defferedRequest.reject, true);

        return defferedRequest.promise();
    } 

    private _findCategoryChildren(categoryId: number, paginationModel?: PaginationModel, tenantAOOId?: string): JQueryPromise<ODATAResponseModel<CategoryModel>> {
        let defferedRequest: JQueryDeferred<ODATAResponseModel<CategoryModel>> = $.Deferred<ODATAResponseModel<CategoryModel>>();

        this._categoryService.getSubCategories(categoryId, defferedRequest.resolve, defferedRequest.reject, paginationModel, tenantAOOId, ["CategoryFascicles"]);

        return defferedRequest.promise();
    }

    private _findCategoryById(categoryId: number, tenantAOOId: string): JQueryPromise<CategoryModel> {
        let defferedRequest: JQueryDeferred<CategoryModel> = $.Deferred<CategoryModel>();

        this._categoryService.getById(categoryId, defferedRequest.resolve, defferedRequest.reject, tenantAOOId, ["Parent", "CategoryFascicles"]);

        return defferedRequest.promise();
    }

    private _getDossierFolderFascTemplatesCount = (dossierFolderId: string): JQueryPromise<number> => {
        let defferedRequest: JQueryDeferred<number> = $.Deferred<number>();

        this._fascicleTemplateService.countFascicleTemplatesByDossierFolderId(dossierFolderId, defferedRequest.resolve, defferedRequest.reject);

        return defferedRequest.promise();
    }

    private _getDossierFolderChildrenCount = (dossierFolderId: string): JQueryPromise<number> => {
        let defferedRequest: JQueryDeferred<number> = $.Deferred<number>();

        this._dossierFolderService.countDossierFolderChildren(dossierFolderId, defferedRequest.resolve, defferedRequest.reject, true);

        return defferedRequest.promise();
    }

    private _loadCategories(expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel = null): JQueryPromise<Telerik.Web.UI.RadTreeNode[]> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();

        this._findCategoryChildren(expandedNode.get_value(), paginationModel, this.currentTenantAOOId)
            .then((odataResult: ODATAResponseModel<CategoryModel>) => this._createCategoryNodesFromModels(odataResult.value, expandedNode, odataResult.count))
            .done(defferedRequest.resolve)
            .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }

    private _appendLoadMoreNode(expandedNode: Telerik.Web.UI.RadTreeNode, totalChildrenCount: number) {
        let expandedNodeType: ProcessNodeType = expandedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME);

        if (expandedNodeType === ProcessNodeType.LoadMore) {
            const expandedNodeParentChildrenCollection: Telerik.Web.UI.RadTreeNodeCollection = expandedNode.get_parent().get_nodes();
            const allChildrenLoaded: boolean = (expandedNodeParentChildrenCollection.get_count() - 1) === totalChildrenCount;

            expandedNodeParentChildrenCollection.remove(expandedNode);
            if (!allChildrenLoaded) {
                expandedNodeParentChildrenCollection.add(this._createLoadMoreNode());
            }
        } else {
            const expandedNodeChildrenCollection: Telerik.Web.UI.RadTreeNodeCollection = expandedNode.get_nodes();
                // If node is of type Category, exclude "Serie e volumi" node from counting (-1)
            const expandedNodeChildrenCount: number = expandedNodeChildrenCollection.get_count();
            const loadedAllChildren: boolean = expandedNodeType === ProcessNodeType.Category
                ? (expandedNodeChildrenCount - 1) === totalChildrenCount
                : expandedNodeChildrenCount === totalChildrenCount;

            if (loadedAllChildren) {
                return;
            }

            expandedNodeChildrenCollection.add(this._createLoadMoreNode());
        }

    }

    private _appendLoadProcessesNode(parentNode: Telerik.Web.UI.RadTreeNode, processNodes?: Telerik.Web.UI.RadTreeNode[]): void {
        let existingProcessNode: Telerik.Web.UI.RadTreeNode = parentNode.get_nodes().toArray()
            .filter(child => this._getNodeAttribute<ProcessNodeType>(child, TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.LoadProcessesAndFolders)[0];

        if (existingProcessNode) {
            parentNode.get_nodes().remove(existingProcessNode);
        }

        let processesNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        processesNode.set_text(CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT);
        this._setNodeAttribute(processesNode, TbltProcess.NodeType_TYPE_NAME, ProcessNodeType.LoadProcessesAndFolders);

        if (processNodes && processNodes.length) {
            this._appendNodesToParent(processNodes, processesNode);
            processesNode.set_expanded(true);
        } else {
            this.appendEmptyNode(processesNode);
        }

        parentNode.get_nodes().add(processesNode)
    }

    private appendEmptyNode(parentNode: Telerik.Web.UI.RadTreeNode): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text(CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT);
        parentNode.get_nodes().add(emptyNode);
    }

    private _dossierFolderNodeExpandedActionHandler = (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        const totalChildrenCount: number | undefined = expandedNode.get_attributes().getAttribute(TbltProcess.TOTAL_CHILDREN_COUNT_ATTRNAME),
            childrenAlreadyCounted: boolean = totalChildrenCount !== undefined,
            dossierFolderId: string = expandedNode.get_value();

        if (childrenAlreadyCounted) {
            this._findDossierFolderChildren(dossierFolderId, paginationModel)
                .then(this._createDossierFolderNodesFromViewModels)
                .then((dossierFolderNodes: Telerik.Web.UI.RadTreeNode[]) => this._appendDossierFolderChildren(dossierFolderNodes, expandedNode))
                .done(defferedRequest.resolve)
                .fail(defferedRequest.reject);
        } else {
            $.when(this._getDossierFolderChildrenCount(dossierFolderId), this._getDossierFolderFascTemplatesCount(dossierFolderId))
                .done((dossierFoldersCount, fascTemplatesCount) => {
                    const totalChildrenCount: number = dossierFoldersCount + fascTemplatesCount;

                    if (totalChildrenCount) {
                        this._appendChildrenCountAttribute(expandedNode, totalChildrenCount);
                    }

                    this._findDossierFolderChildren(dossierFolderId, paginationModel)
                        .then(this._createDossierFolderNodesFromViewModels)
                        .then((dossierFolderNodes: Telerik.Web.UI.RadTreeNode[]) => this._appendDossierFolderChildren(dossierFolderNodes, expandedNode))
                        .done(defferedRequest.resolve)
                        .fail(defferedRequest.reject);
                }).fail(defferedRequest.reject);
        }

        return defferedRequest.promise();
    }
    private _processNodeExpandedActionHandler = (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        const processId: string = expandedNode.get_value();
        this._findProcessDossierFolders(processId, paginationModel)
            .then((odataResponse: ODATAResponseModel<DossierFolderModel>) => this._createDossierFolderNodesFromModels(odataResponse.value, expandedNode, odataResponse.count))
            .then((dossierFolderNodes: Telerik.Web.UI.RadTreeNode[]) => this._appendNodesToParent(dossierFolderNodes, expandedNode))
            .done(defferedRequest.resolve)
            .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _processesAndFoldersNodeExpandedActionHandler = (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        const categoryId: number = +expandedNode.get_parent().get_value();
        this._findProcessesByCategoryId(categoryId, paginationModel)
            .then((odataResult: ODATAResponseModel<ProcessModel>) => {
                this.processesModel = odataResult.value;

                const processNodes: Telerik.Web.UI.RadTreeNode[] = this._createProcessNodesFromModels(odataResult.value, expandedNode, odataResult.count);

                if (this.defaultSelectedProcessId.length) {
                    let nodeToSelect: Telerik.Web.UI.RadTreeNode = this._rtvProcesses.findNodeByValue(this.defaultSelectedProcessId);
                    if (nodeToSelect) {
                        nodeToSelect.set_selected(true);
                        this.showNodeDetails(nodeToSelect);
                    }
                }

                return processNodes;
            })
            .then((processNodes: Telerik.Web.UI.RadTreeNode[]) => this._appendNodesToParent(processNodes, expandedNode))
            .done(defferedRequest.resolve)
            .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _categoryNodeExpandedActionHandler = (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        const categoryId: number = expandedNode.get_value();
        this._findCategoryChildren(categoryId, paginationModel, this.currentTenantAOOId)
            .then((odataResult: ODATAResponseModel<CategoryModel>) => this._createCategoryNodesFromModels(odataResult.value, expandedNode, odataResult.count))
            .then((subCategories: Telerik.Web.UI.RadTreeNode[]) => this._appendNodesToParent(subCategories, expandedNode))
            .done(defferedRequest.resolve)
            .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _loadMoreNodeExpandedActionHandler = (expandedNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        const expandedNodeParent: Telerik.Web.UI.RadTreeNode = expandedNode.get_parent();
        const parentNodeChildrenCount: number = expandedNodeParent.get_nodes().get_count();
        const parentNodeType: ProcessNodeType = expandedNodeParent.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME);

        if (!this._nodeExpandingActionHandlerMap.has(parentNodeType)) {
            defferedRequest.reject();
            return;
        }

        const expandedNodeActionHandler: (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel) => JQueryPromise<void>
            = this._nodeExpandingActionHandlerMap.get(parentNodeType);

        // exclude "Load more" node from children collection count
        const recordsToSkip: number = parentNodeChildrenCount - 1;
        const paginationModel: PaginationModel = new PaginationModel(recordsToSkip, this.treeViewNodesPageSize);

        expandedNodeActionHandler(expandedNodeParent, paginationModel)
            .done(() => {
                if (parentNodeType === ProcessNodeType.Category) {
                    this._appendLoadProcessesNode(expandedNodeParent);
                }

                defferedRequest.resolve();
            })
            .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _rootNodeExpandedActionHandler = (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        this._loadCategories(expandedNode, paginationModel)
            .done(() => defferedRequest.resolve())
            .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }

    rtvProcess_onExpand = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let expandedNode: Telerik.Web.UI.RadTreeNode = args.get_node(),
            expandedNodeType: ProcessNodeType | undefined = expandedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME);

        if (this.filterInputValue && expandedNodeType === ProcessNodeType.Root || this.filterInputValue && expandedNode.get_allNodes().length > 1) {
            return;
        }

        if (expandedNodeType === undefined) {
            console.error(`Invalid expanded node type ${expandedNodeType}`);
            return;
        }

        if (!this._nodeExpandingActionHandlerMap.has(expandedNodeType)) {
            console.error(`Expanding action handler not registered for node type ${expandedNodeType}`);
            return;
        }

        let expandedNodeActionHandler: (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel) => JQueryPromise<void>
            = this._nodeExpandingActionHandlerMap.get(expandedNodeType);

        expandedNode.get_nodes().clear();
        expandedNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        expandedNodeActionHandler(expandedNode, this._defaultPaginationModel)
            .done(() => {
                expandedNode.set_expanded(true);

                if (expandedNodeType === ProcessNodeType.Category) {
                    this._appendLoadProcessesNode(expandedNode);
                }

                const expandedNodeParentTotalChildrenCount: number | undefined =
                    expandedNodeType === ProcessNodeType.LoadMore
                        ? expandedNode.get_parent().get_attributes().getAttribute(TbltProcess.TOTAL_CHILDREN_COUNT_ATTRNAME)
                        : expandedNode.get_attributes().getAttribute(TbltProcess.TOTAL_CHILDREN_COUNT_ATTRNAME);

                if (expandedNodeParentTotalChildrenCount) {
                    this._appendLoadMoreNode(expandedNode, expandedNodeParentTotalChildrenCount);
                }
            })
            .always(() => expandedNode.hideLoadingStatus())
            .fail(this.showNotificationException);
    }

    private showNodeDetails(selectedNode: Telerik.Web.UI.RadTreeNode): void {
        const selectedNodeType: ProcessNodeType = this._getNodeAttribute<ProcessNodeType>(selectedNode, TbltProcess.NodeType_TYPE_NAME);

        this.initializeNodeClicked(selectedNode);
        if (selectedNode.get_level() === 0 || selectedNode.get_text() === CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT || selectedNodeType === ProcessNodeType.LoadMore) {
            $(`#${this._uscProcessDetails.pnlDetailsId}`).hide();
            this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, false);
        }
        else if (selectedNode.get_text() === CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT) {
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CREATE).set_enabled(true);
            $(`#${this._uscProcessDetails.pnlDetailsId}`).hide();
            this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, false);
        }
        else {
            switch (selectedNodeType) {
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

    private _createDossierFolderNodesFromViewModels = (dossierFolders: DossierSummaryFolderViewModel[]): Telerik.Web.UI.RadTreeNode[] => {
        const dossierFolderNodeImageUrl: string = TbltProcess.FOLDER_CLOSED_IMGURL;

        const dossierFolderNodes: Telerik.Web.UI.RadTreeNode[] = dossierFolders.map((dossierFolder: DossierSummaryFolderViewModel) => {
            const dossierFolderNode: Telerik.Web.UI.RadTreeNode
                = this._createTreeNode(ProcessNodeType.DossierFolder, dossierFolder.Name, dossierFolder.UniqueId, dossierFolderNodeImageUrl);

            if (dossierFolder.Status.toString() === DossierFolderStatus[DossierFolderStatus.Folder].toString()) {
                this._appendEmptyNode(dossierFolderNode);
            }

            return dossierFolderNode;
        });

        return dossierFolderNodes;
    }

    private _createDossierFolderNodesFromModels = (dossierFolders: DossierFolderModel[], parentNode: Telerik.Web.UI.RadTreeNode, totalCount?: number): Telerik.Web.UI.RadTreeNode[] => {
        const dossierFolderNodeImageUrl: string = TbltProcess.FOLDER_CLOSED_IMGURL;

        const dossierFolderNodes: Telerik.Web.UI.RadTreeNode[] = dossierFolders.map((dossierFolder: DossierFolderModel) => {
            const dossierFolderNode: Telerik.Web.UI.RadTreeNode
                = this._createTreeNode(ProcessNodeType.DossierFolder, dossierFolder.Name, dossierFolder.UniqueId, dossierFolderNodeImageUrl);

            if (dossierFolder.Status.toString() === DossierFolderStatus[DossierFolderStatus.Folder].toString()) {
                this._appendEmptyNode(dossierFolderNode);
            }

            return dossierFolderNode;
        });

        if (totalCount) {
            this._appendChildrenCountAttribute(parentNode, totalCount);
        }

        return dossierFolderNodes;
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
                this._rtbFascicleTemplateName.clear();
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
                            dossierFolder.ParentInsertId = this.getProcessNodeByChild(this._rtvProcesses.get_selectedNode()).get_attributes().getAttribute(TbltProcess.ATTRNAME_IDDOSSIER);
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
                        this._uscProcessRoleRest.enableButtons();
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
                this._updateTreeNode(selectedNode, data.Name, data.UniqueId, imageUrl,
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
            this._updateTreeNode(node, data.Name, data.UniqueId, imageUrl,
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
            this._updateTreeNode(selectedNode, data.Name, data.UniqueId, TbltProcess.PROCESS_NODE_IMGURL,
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
            this._updateTreeNode(node, data.Name, data.UniqueId, TbltProcess.PROCESS_NODE_IMGURL,
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
        if (!this._rtbDossierFolderName.get_value()) {
            return;
        }

        const selectedNodeType: ProcessNodeType = selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME),
              exists: boolean = !!this.selectedDossierFolderId;

        const dossierFolderParentInsertId: string = selectedNodeType === ProcessNodeType.Process
            ? this.getProcessNodeByChild(selectedNode).get_attributes().getAttribute(TbltProcess.ATTRNAME_IDDOSSIER)
            : (exists ? selectedNode.get_parent().get_value() : selectedNode.get_value());

        if (exists) {
            this._dossierFolderService.getDossierFolderWithRoles(this.selectedDossierFolderId,
                (dossierFolderModel: DossierFolderModel) => {
                    dossierFolderModel.Name = this._rtbDossierFolderName.get_value();
                    dossierFolderModel.ParentInsertId = dossierFolderParentInsertId;

                    this.updateDossierFolder(selectedNode, dossierFolderModel);
                }, (error) => {
                    this.showNotificationException(error);
                });
        } else {
            let dossierFolder: DossierFolderModel = <DossierFolderModel>{};
            dossierFolder.Name = this._rtbDossierFolderName.get_value();
            dossierFolder.ParentInsertId = dossierFolderParentInsertId;
            dossierFolder.Dossier = <DossierModel>{};
            dossierFolder.Dossier.UniqueId = this.getProcessNodeByChild(selectedNode).get_attributes().getAttribute(TbltProcess.ATTRNAME_IDDOSSIER);

            this.insertDossierFolder(selectedNode, dossierFolder);
        }
    }
    private updateDossierFolder(selectedNode: Telerik.Web.UI.RadTreeNode, dossierFolder: DossierFolderModel): void {
        this._ajaxLoadingPanel.show(this.rtvProcessesId);
        dossierFolder.UniqueId = this.selectedDossierFolderId;
        this._dossierFolderService.updateDossierFolder(dossierFolder, null, (data) => {
            this._updateTreeNode(selectedNode, data.Name, data.UniqueId, TbltProcess.FOLDER_CLOSED_IMGURL,
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
                this._updateTreeNode(node, data.Name, data.UniqueId, TbltProcess.FOLDER_CLOSED_IMGURL,
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
            dossierFolder.ParentInsertId = this.getProcessNodeByChild(this._rtvProcesses.get_selectedNode()).get_attributes().getAttribute(TbltProcess.ATTRNAME_IDDOSSIER);
        }
        else {
            dossierFolder.ParentInsertId = this._rtvProcesses.get_selectedNode().get_parent().get_value();
        }
        dossierFolder.JsonMetadata = sourceNode.get_value();
        dossierFolder.Dossier = <DossierModel>{};
        dossierFolder.Dossier.UniqueId = this.getProcessNodeByChild(sourceNode).get_attributes().getAttribute(TbltProcess.ATTRNAME_IDDOSSIER);
        //TODO: Remove ActionTypes in DocSuite 9.0X (move webapi logic in Store Procedure) 
        this._dossierFolderService.insertDossierFolder(dossierFolder, InsertActionType.CloneProcessFolder, (data) => {
            //insert node
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            if (sourceNode.get_nodes().get_count() > 0 && sourceNode.get_nodes().getNode(0).get_text() === "") {
                sourceNode.get_nodes().clear();
            }

            //update node hierarchy
            this._dossierFolderService.updateDossierFolder(data, UpdateActionType.CloneProcessFolder, (data) => {
                this._updateTreeNode(node, data.Name, data.UniqueId, TbltProcess.FOLDER_CLOSED_IMGURL,
                    ProcessNodeType.DossierFolder, true, data.Status, null, null, this.getProcessNodeByChild(sourceNode), false);

                //to be able to see the plus sign
                let dummyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                this._updateTreeNode(dummyNode, "", "", TbltProcess.FOLDER_CLOSED_IMGURL,
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

    private _updateTreeNode(node: Telerik.Web.UI.RadTreeNode, text: string, value: string, imagePath: string, nodeType: ProcessNodeType, isExpanded: boolean,
        dossierFolderStatus?: DossierFolderStatus, isActive?: boolean, idDossier?: string, parentNode?: Telerik.Web.UI.RadTreeNode, expandParent: boolean = true): void {
        node.set_value(value);
        node.set_text(text);
        node.get_attributes().setAttribute(TbltProcess.NodeType_TYPE_NAME, nodeType);
        node.set_imageUrl(imagePath);
        switch (nodeType) {
            case ProcessNodeType.Process: {
                node.get_attributes().setAttribute(TbltProcess.ATTRNAME_ISACTIVE, isActive);
                node.get_attributes().setAttribute(TbltProcess.ATTRNAME_IDDOSSIER, idDossier);
                break;
            }
            case ProcessNodeType.DossierFolder: {
                node.get_attributes().setAttribute(TbltProcess.ATTRNAME_DOSSIERFOLDERSTATUS, DossierFolderStatus[dossierFolderStatus]);
                break;
            }
            case ProcessNodeType.ProcessFascicleTemplate: {
                node.get_attributes().setAttribute(TbltProcess.ATTRNAME_ISACTIVE, isActive);
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
        fascicleTemplate.Name = this._rtvProcesses.get_selectedNode().get_text();
        fascicleTemplate.JsonModel = uscProcessDetails.pftJsonModel;
        fascicleTemplate.EndDate = endDate;
        if (window.confirm("Vuoi eliminare modello di fascicolo selezionato?")) {
            this._fascicleTemplateService.update(fascicleTemplate, (data) => {
                let imgUrl: string = TbltProcess.FASCICLEOPENED_IMGURL;
                this._rtvProcesses.get_selectedNode().set_imageUrl(imgUrl);
                this._rtvProcesses.get_selectedNode().get_attributes().setAttribute(TbltProcess.ATTRNAME_ISACTIVE, false);
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
            let processActiveItem: any = this._filterToolbar.findItemByValue(TbltProcess.PROCESS_ACTIVE_FILTERBTNVAL);
            let nodeRemoveConditions: boolean = processActiveItem.get_checked();
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
        const toolbarBtnValue: string = args.get_item().get_value();
        switch (toolbarBtnValue) {
            case TbltProcess.SEARCH_BTNVAL: {
                this._createdCategoriesCache = [];
                this._currentActiveRequestsCount = 0;

                if (!this.filterInputValue) {
                    this._initializeTreeView();
                    return;
                }

                this._uscProcessDetails = <uscProcessDetails>$(`#${this.uscProcessDetailsId}`).data();
                this._uscProcessDetails.clearProcessDetails();
                $(`#${this._uscProcessDetails.pnlDetailsId}`).hide();
                this.TreeviewRootNode.set_expanded(false);
                this.TreeviewRootNode.get_nodes().clear();

                this._filterToolbar.findItemByValue(TbltProcess.SEARCH_BTNVAL).set_enabled(false);
                let processActiveItem: any = this._filterToolbar.findItemByValue(TbltProcess.PROCESS_ACTIVE_FILTERBTNVAL);
                this.TreeviewRootNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                this._findProcessesByName(this.filterInputValue, processActiveItem.get_checked(), this.currentTenantAOOId)
                    .then((processes: ProcessModel[]) => {

                        if (!processes || !processes.length) {
                            this.TreeviewRootNode.hideLoadingStatus();
                            this._appendEmptyNode(this.TreeviewRootNode);
                            this.TreeviewRootNode.set_expanded(true);
                            this._filterToolbar.findItemByValue(TbltProcess.SEARCH_BTNVAL).set_enabled(true);
                            return;
                        }

                        processes.forEach(processModel => {
                            this._onRequestStarted();
                            let processNode: Telerik.Web.UI.RadTreeNode = this._createProcessNode(processModel),
                                existingParentCategoryNode: Telerik.Web.UI.RadTreeNode = this._createdCategoriesCache.filter(node => +node.get_value() === processModel.Category.EntityShortId)[0];

                            processNode.set_contentCssClass(TbltProcess.BOLD_CSSCLASS);
                            if (existingParentCategoryNode) {
                                let categorySerieEVolumiNode: Telerik.Web.UI.RadTreeNode = existingParentCategoryNode.get_allNodes()
                                    .filter(node => this._getNodeAttribute<ProcessNodeType>(node, TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.LoadProcessesAndFolders)[0];
                                categorySerieEVolumiNode.get_nodes().add(processNode);
                                existingParentCategoryNode.set_expanded(true);
                                categorySerieEVolumiNode.set_expanded(true);
                                this._onRequestCompleted();
                                return;
                            }

                            let parentCategoryNode: Telerik.Web.UI.RadTreeNode = this._createCategoryNode(processModel.Category, true, null, [processNode]);
                            parentCategoryNode.set_expanded(true);

                            this._createdCategoriesCache.push(parentCategoryNode);

                            this._appendProcessCategoryParents(parentCategoryNode, processModel.Category.Parent?.EntityShortId);
                        });
                    });
                break;
            }
        }
    }

    private _appendProcessCategoryParents(currentCategoryNode: Telerik.Web.UI.RadTreeNode, parentId?: number): JQueryPromise<Telerik.Web.UI.RadTreeNode> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode> = $.Deferred<Telerik.Web.UI.RadTreeNode>();

        // Checks if current category already exists in the tree view
        let currentNodeFromTree: Telerik.Web.UI.RadTreeNode = this.TreeviewRootNode.get_allNodes().filter(node => +node.get_value() === currentCategoryNode.get_value())[0];

        if (currentNodeFromTree) {
            currentNodeFromTree.set_expanded(true);
            return defferedRequest.resolve(currentNodeFromTree);
        }

        currentCategoryNode.set_expanded(true);
        if (parentId && parentId !== +this.TreeviewRootNode.get_value()) {
            this._findCategoryById(parentId, this.currentTenantAOOId)
                .then((parentCategory: CategoryModel) => {
                    let currentParentNode: Telerik.Web.UI.RadTreeNode = this._createCategoryNode(parentCategory, false);
                    this._appendProcessCategoryParents(currentParentNode, parentCategory.Parent?.EntityShortId)
                        .then((parentNode: Telerik.Web.UI.RadTreeNode) => {
                            let parentNodeFromTree: Telerik.Web.UI.RadTreeNode
                                = parentNode.get_allNodes().filter(node => +node.get_value() === +currentCategoryNode.get_value())[0];

                            if (parentNodeFromTree) {
                                return defferedRequest.resolve(parentNodeFromTree);
                            } else {
                                parentNode.get_nodes().add(currentCategoryNode);
                                parentNode.set_expanded(true);
                                return defferedRequest.resolve(currentCategoryNode);
                            }
                        });
                }).always(this._onRequestCompleted);
        } else {
            currentCategoryNode.set_expanded(true);
            this.TreeviewRootNode.get_nodes().add(currentCategoryNode);
            defferedRequest.resolve(currentCategoryNode);
            this._onRequestCompleted();
        }

        return defferedRequest.promise();
    }

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

    private _appendChildrenCountAttribute(node: Telerik.Web.UI.RadTreeNode, totalChildrenCount: number): void {
        const currentChildrenAttributeValue: number | undefined = node.get_attributes().getAttribute(TbltProcess.TOTAL_CHILDREN_COUNT_ATTRNAME);

        if (currentChildrenAttributeValue === undefined) {
            node.get_attributes().setAttribute(TbltProcess.TOTAL_CHILDREN_COUNT_ATTRNAME, totalChildrenCount);
        }
    }

    private _appendEmptyNode(parentNode: Telerik.Web.UI.RadTreeNode): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text(TbltProcess.NOELEMENTS_NODE_LABEL);
        parentNode.get_nodes().add(emptyNode);
    }

    private _createLoadMoreNode(): Telerik.Web.UI.RadTreeNode {
        let loadMoreNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(ProcessNodeType.LoadMore, TbltProcess.LOADMORE_NODE_LABEL, null, TbltProcess.LOAD_MORE_NODE_IMAGEURL);
        this.appendEmptyNode(loadMoreNode);

        return loadMoreNode;
    }

    private _createTreeNode(nodeType: ProcessNodeType, nodeDescription: string, nodeValue: number | string, imageUrl: string, parentNode?: Telerik.Web.UI.RadTreeNode, tooltipText?: string, expandedImageUrl?: string): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        treeNode.set_text(nodeDescription);
        treeNode.set_value(nodeValue);

        this._setNodeAttribute(treeNode, TbltProcess.NodeType_TYPE_NAME, nodeType);
        if (imageUrl) {
            treeNode.set_imageUrl(imageUrl);
        }

        if (tooltipText) {
            treeNode.set_toolTip(tooltipText);
        }

        if (expandedImageUrl) {
            treeNode.set_expandedImageUrl(expandedImageUrl);
        }

        if (parentNode) {
            parentNode.get_nodes().add(treeNode);
        }

        return treeNode;
    }

    private _getNodeAttribute<TAttributeValue>(treeNode: Telerik.Web.UI.RadTreeNode, attributeName: string): TAttributeValue | undefined {
        return treeNode.get_attributes().getAttribute(attributeName);
    }

    private _setNodeAttribute<TAttributeValue>(treeNode: Telerik.Web.UI.RadTreeNode, attributeName: string, attributeValue: TAttributeValue): void {
        const currentNodeTypeAttributeValue: TAttributeValue = this._getNodeAttribute<TAttributeValue>(treeNode, attributeName);

        if (currentNodeTypeAttributeValue !== undefined) {
            treeNode.get_attributes().removeAttribute(TbltProcess.NodeType_TYPE_NAME);
        }

        treeNode.get_attributes().setAttribute(attributeName, attributeValue);
    }

    private _appendNodesToParent = (childNodes: Telerik.Web.UI.RadTreeNode[], parentNode: Telerik.Web.UI.RadTreeNode): void => {
        let parentNodeCollection: Telerik.Web.UI.RadTreeNodeCollection = parentNode.get_nodes();
        childNodes.forEach(childNode => parentNodeCollection.add(childNode));
    }

    private _createCategoryNode(categoryModel: CategoryModel, appendProcessesNode?: boolean, parentNode?: Telerik.Web.UI.RadTreeNode, childProcessNodes?: Telerik.Web.UI.RadTreeNode[]): Telerik.Web.UI.RadTreeNode {
        const parentNodeIsRootNode: boolean = !!parentNode && parentNode.get_value() === this.TreeviewRootNode.get_value();
        let treeNodeDescription: string = `${categoryModel.Code}.${categoryModel.Name}`;
        let treeNodeImageUrl: string = TbltProcess.CATEGORY_NODE_IMGURL;

        let treeNode: Telerik.Web.UI.RadTreeNode
            = this._createTreeNode(ProcessNodeType.Category, treeNodeDescription, categoryModel.EntityShortId, treeNodeImageUrl, parentNodeIsRootNode ? parentNode : undefined);

        treeNode.set_contentCssClass((categoryModel.CategoryFascicles && categoryModel.CategoryFascicles.length > 0) ? TbltProcess.CSSCLASS_FASCICLE_NODE : TbltProcess.CSSCLASS_NOTFASCICLE_NODE);

        if (!!appendProcessesNode) {
            this._appendLoadProcessesNode(treeNode, childProcessNodes);
        }

        return treeNode;
    }

    private _createCategoryNodesFromModels = (categories: CategoryModel[], parentNode: Telerik.Web.UI.RadTreeNode, totalCount?: number): Telerik.Web.UI.RadTreeNode[] => {
        let categoryNodes: Telerik.Web.UI.RadTreeNode[] = categories.map(category => this._createCategoryNode(category, true, parentNode));

        if (totalCount && totalCount > 0) {
            this._appendChildrenCountAttribute(parentNode, totalCount);
        }

        return categoryNodes;
    }

    private _createProcessNode = (processModel: ProcessModel): Telerik.Web.UI.RadTreeNode => {
        const isProcessActive: boolean = processModel.EndDate === null || new Date(processModel.EndDate) > new Date();
        const processNode: Telerik.Web.UI.RadTreeNode
            = this._createTreeNode(ProcessNodeType.Process, processModel.Name, processModel.UniqueId, TbltProcess.PROCESS_NODE_IMGURL);

        this._setNodeAttribute(processNode, TbltProcess.ATTRNAME_ISACTIVE, isProcessActive);
        this._setNodeAttribute(processNode, TbltProcess.ATTRNAME_IDDOSSIER, processModel.Dossier.UniqueId);
        this._setNodeAttribute(processNode, TbltProcess.Category_ID_TYPE, processModel.Category.EntityShortId);

        this._appendEmptyNode(processNode);

        return processNode;
    }

    private _createProcessNodesFromModels = (processes: ProcessModel[], parentNode: Telerik.Web.UI.RadTreeNode, totalCount?: number): Telerik.Web.UI.RadTreeNode[] => {

        const processNodes: Telerik.Web.UI.RadTreeNode[] = processes.map(this._createProcessNode);

        if (totalCount && totalCount > 0) {
            this._appendChildrenCountAttribute(parentNode, totalCount);
        }

        return processNodes;
    }

    private _createFascicleTemplateNodesFromModels = (fascTemplates: ProcessFascicleTemplateModel[]): Telerik.Web.UI.RadTreeNode[] => {
        const fascicleTemplateNodes: Telerik.Web.UI.RadTreeNode[] = fascTemplates.map((fascTemplate: ProcessFascicleTemplateModel) => {
            const isActive: boolean = fascTemplate.EndDate === null || new Date(fascTemplate.EndDate) > new Date();
            const fascTemplateNodeImageUrl: string = isActive
                ? TbltProcess.FASCICLECLOSED_IMGURL
                : TbltProcess.FASCICLEOPENED_IMGURL;

            const fascTemplateNode: Telerik.Web.UI.RadTreeNode
                = this._createTreeNode(ProcessNodeType.ProcessFascicleTemplate, fascTemplate.Name, fascTemplate.UniqueId, fascTemplateNodeImageUrl);

            this._setNodeAttribute(fascTemplateNode, TbltProcess.ATTRNAME_ISACTIVE, isActive)

            return fascTemplateNode;
        });

        return fascicleTemplateNodes;
    }

    private _appendNodesToCollection(children: Telerik.Web.UI.RadTreeNode[], nodesCollection: Telerik.Web.UI.RadTreeNodeCollection): void {
        children.forEach((childNode: Telerik.Web.UI.RadTreeNode) => nodesCollection.add(childNode));
    }

    private _appendDossierFolderChildren(dossierFolders: Telerik.Web.UI.RadTreeNode[], parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        const fascicleTemplatesToLoad: number = this.treeViewNodesPageSize - dossierFolders.length;
        const currentParentNodeCollection: Telerik.Web.UI.RadTreeNodeCollection = parentNode.get_nodes();
        const dossierFolderId: string = parentNode.get_value();

        if (!fascicleTemplatesToLoad) {
            this._appendNodesToCollection(dossierFolders, currentParentNodeCollection);
            defferedRequest.resolve();
            return;
        }

        const alreadyLoadedFascTemplatesCount: number = currentParentNodeCollection.toArray()
            .filter(node => this._getNodeAttribute<ProcessNodeType>(node, TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.ProcessFascicleTemplate)?.length;
        const fascTemplatesPaginationModel: PaginationModel = new PaginationModel(alreadyLoadedFascTemplatesCount, fascicleTemplatesToLoad);
        this._findDossierFolderFascicleTemplates(dossierFolderId, fascTemplatesPaginationModel)
            .then((odataResult: ODATAResponseModel<ProcessFascicleTemplateModel>) => this._createFascicleTemplateNodesFromModels(odataResult.value))
            .then((fascTemplateNodes: Telerik.Web.UI.RadTreeNode[]) => {
                this._appendNodesToCollection(dossierFolders, currentParentNodeCollection);
                this._appendNodesToCollection(fascTemplateNodes, currentParentNodeCollection);
            })
            .done(defferedRequest.resolve)
            .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }

    private _onRequestStarted = (): void => {
        this._currentActiveRequestsCount = this._currentActiveRequestsCount + 1;
    }

    private _onRequestCompleted = (): void => {

        if (this._currentActiveRequestsCount > 0) {
            this._currentActiveRequestsCount = this._currentActiveRequestsCount - 1;
        }

        if (this._currentActiveRequestsCount < 1) {

            if (!this.TreeviewRootNode.get_allNodes().length) {
                this._appendEmptyNode(this.TreeviewRootNode);
            }

            this.TreeviewRootNode.hideLoadingStatus();
            this.TreeviewRootNode.set_expanded(true);
            this._filterToolbar.findItemByValue(TbltProcess.SEARCH_BTNVAL).set_enabled(true);
        }
    }
}

export = TbltProcess;