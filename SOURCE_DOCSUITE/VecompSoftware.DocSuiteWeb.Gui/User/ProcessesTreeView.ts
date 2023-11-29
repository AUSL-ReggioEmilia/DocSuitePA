/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/core/extensions/number.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import CategoryService = require("App/Services/Commons/CategoryService");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import DocumentUnitService = require("App/Services/DocumentUnits/DocumentUnitService");
import DocumentUnitModel = require("App/Models/DocumentUnits/DocumentUnitModel");
import ProcessService = require("App/Services/Processes/ProcessService");
import ProcessModel = require("App/Models/Processes/ProcessModel");
import DossierFolderService = require("App/Services/Dossiers/DossierFolderService");
import DossierSummaryFolderViewModel = require("App/ViewModels/Dossiers/DossierSummaryFolderViewModel");
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import DossierFolderStatus = require("App/Models/Dossiers/DossierFolderStatus");
import FascicleFolderService = require("App/Services/Fascicles/FascicleFolderService");
import FascicleSummaryFolderViewModel = require("App/ViewModels/Fascicles/FascicleSummaryFolderViewModel");
import DossierSummaryFolderViewModelMapper = require("App/Mappers/Dossiers/DossierSummaryFolderViewModelMapper");
import FascicleFolderStatus = require("App/Models/Fascicles/FascicleFolderStatus");
import FascicleFolderTypology = require("App/Models/Fascicles/FascicleFolderTypology");
import Environment = require("App/Models/Environment");
import UscErrorNotification = require("UserControl/uscErrorNotification");
import TreeNodeType = require("App/Models/Commons/TreeNodeType");
import FascicleService = require("App/Services/Fascicles/FascicleService");
import ODATAResponseModel = require("App/Models/ODATAResponseModel");
import FascProcessMoveResponseModelDTO = require("App/DTOs/FascProcessMoveResponseModelDTO");
import PaginationModel = require("App/Models/Commons/PaginationModel");
import CategoryModel = require('App/Models/Commons/CategoryModel');
import DossierFolderModel = require("App/Models/Dossiers/DossierFolderModel");

class ProcessesTreeView {
    public categoryTreeViewId: string;
    public detailsPaneId: string;
    public ajaxLoadingPanelId: string;
    public uscNotificationId: string;
    public showOnlyMyProcesses: string;
    public actionToolbarId: string;
    public currentTenantAOOId: string;
    public windowManagerId: string;
    public windowMoveFascId: string;
    public splitterMainId: string;
    public treeViewNodesPageSize: number;

    private _categoryService: CategoryService;
    private _documentUnitService: DocumentUnitService;
    private _processService: ProcessService;
    private _dossierFolderService: DossierFolderService;
    private _fascicleFolderService: FascicleFolderService;
    private _fascicleService: FascicleService;

    private _categoryTreeView: Telerik.Web.UI.RadTreeView;
    private _detailsPane: Telerik.Web.UI.RadPane;
    private _actionToolbar: Telerik.Web.UI.RadToolBar;
    private _moveFascicleToolbarBtn: Telerik.Web.UI.RadToolBarButton;
    private _editProcessToolbarBtn: Telerik.Web.UI.RadToolBarButton;
    private _windowManager: Telerik.Web.UI.RadWindowManager;
    private _windowMoveFasc: Telerik.Web.UI.RadWindow;
    private _uscNotification: UscErrorNotification;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    private _showOnlyMyProcesses(): boolean {
        return JSON.parse(this.showOnlyMyProcesses.toLowerCase());
    }

    private _nodeTypeExpandingActionsDictionary;
    private _nodeTypeClickActionsDictionary;
    private _dossierFolderStatusImageDictionary;
    private _expandedFolderImageDictionary;
    private _dossierFolderStatusTooltipDictionary;
    private _fascicleFolderStatusImageDictionary;
    private _fascicleFolderStatusTooltipDictionary;
    private _environmentImageDictionary;

    private static ROOTNODE_DESCRIPTION: string = "Classificatore";
    private static NODETYPE_ATTRNAME: string = "NodeType";
    private static FASCICLEID_ATTRNAME: string = "FascicleId";
    private static DOCUNITTYPE_ATTRNAME: string = "DocUnitType";
    private static YEAR_ATTRNAME: string = "Year";
    private static NUMBER_ATTRNAME: string = "Number";
    private static ENTITYID_ATTRNAME: string = "EntityId";
    private static UDSREPOID_ATTRNAME: string = "UdsRepoUniqueId";
    private static DOSSIERFOLDERID_ATTRNAME: string = "DossierFolderId";
    private static FASCISACTIVE_ATTRNAME: string = "FascicleIsActive";
    private static COMMANDNAME_MOVEFASCICLE: string = "MoveFascicle";
    private static COMMANDNAME_EDITPROCESS: string = "EditProcess";
    private static CATEGORYID_ATTRNAME: string = "CategoryId";
    private static ROOTNODE_CATEGORYID_ATTRNAME: string = "rootCategoryId";
    private static TOTAL_CHILDREN_COUNT_ATTRNAME: string = "totalChildrenCount";

    private static DEFAULT_DOCUMENT_IMAGEURL: string = "../App_Themes/DocSuite2008/imgset16/document.png";
    private static LOAD_MORE_NODE_IMAGEURL: string = "../App_Themes/DocSuite2008/imgset16/add.png";
    private static LOADMORE_NODE_LABEL: string = "Carica più elementi";
    private static CATEGORY_NODE_IMAGEURL: string = "../Comm/images/folderopen16.gif";
    private static PROCESS_NODE_IMAGEURL: string = "../App_Themes/DocSuite2008/imgset16/process.png";

    private get _defaultPaginationModel(): PaginationModel {
        return new PaginationModel(0, this.treeViewNodesPageSize);
    }

    private _categoryTreeViewRootNode(): Telerik.Web.UI.RadTreeNode {
        return this._categoryTreeView.get_nodes().getNode(0);
    }

    private _toolbarActions(): Array<[string, () => void]> {
        let items: Array<[string, () => void]> = [
            [ProcessesTreeView.COMMANDNAME_MOVEFASCICLE, () => this._moveFascicle()],
            [ProcessesTreeView.COMMANDNAME_EDITPROCESS, () => this._redirectToProcessEditPage()]
        ];
        return items;
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let categoryServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
        this._categoryService = new CategoryService(categoryServiceConfiguration);

        let documentUnitConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DocumentUnit");
        this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);

        let processConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Process");
        this._processService = new ProcessService(processConfiguration);

        let dossierFolderConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DossierFolder");
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);

        let fascicleFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "FascicleFolder");
        this._fascicleFolderService = new FascicleFolderService(fascicleFolderConfiguration);

        let fascicleServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Fascicle");
        this._fascicleService = new FascicleService(fascicleServiceConfiguration);
    }

    /// region [ PUBLIC METHODS ]
    public initialize(): void {
        this._initializeDictionaries();
        this._initializeControls();
        this._initializeCategoryTreeView();
    }

    public treeView_LoadNodeChildrenOnExpand = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs): void => {
        let expandedNode: Telerik.Web.UI.RadTreeNode = args.get_node();
        expandedNode.get_nodes().clear();

        let nodeType: TreeNodeType = expandedNode.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME);
        let expandedNodeAction: (parentNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel) => JQueryPromise<void> = this._nodeTypeExpandingActionsDictionary[nodeType];

        if (expandedNodeAction) {
            expandedNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            expandedNodeAction(expandedNode, this._defaultPaginationModel)
                .done(() => {
                    expandedNode.set_expanded(true);

                    if (expandedNode.get_nodes().get_count() === 1 && !expandedNode.get_nodes().getNode(0).get_value()) {
                        return;
                    }

                    const expandedNodeParentTotalChildrenCount: number | undefined =
                        nodeType === TreeNodeType.LoadMore
                            ? expandedNode.get_parent().get_attributes().getAttribute(ProcessesTreeView.TOTAL_CHILDREN_COUNT_ATTRNAME)
                            : expandedNode.get_attributes().getAttribute(ProcessesTreeView.TOTAL_CHILDREN_COUNT_ATTRNAME);

                    expandedNodeParentTotalChildrenCount
                        ? this._appendLoadMoreNode(expandedNode, expandedNodeParentTotalChildrenCount)
                        : this._appendEmptyNode(expandedNode);
                })
                .always(() => expandedNode.hideLoadingStatus())
                .fail((exception: ExceptionDTO) => {
                    this._showNotificationException(exception);
                });
        }
    }

    public treeView_LoadNodeTypeDetailsOnClick = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let node: Telerik.Web.UI.RadTreeNode = args.get_node();
        let nodeType: TreeNodeType = node.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME);

        this._moveFascicleToolbarBtn.set_enabled(nodeType === TreeNodeType.Fascicle);
        this._editProcessToolbarBtn.set_enabled(nodeType === TreeNodeType.Process);

        let nodeTypeClickEventHandler = this._nodeTypeClickActionsDictionary[nodeType];

        if (nodeTypeClickEventHandler) {
            nodeTypeClickEventHandler(node);
        }
    }
    /// endregion [ PUBLIC METHODS ]

    /// region [ FINDER METHODS ]
    private _findCategoryProcesses(categoryId: number, loadOnlyMy: boolean, paginationModel?: PaginationModel): JQueryPromise<ProcessModel[]> {
        let defferedRequest: JQueryDeferred<ProcessModel[]> = $.Deferred<ProcessModel[]>();

        this._processService.getCategoryProcesses(categoryId, loadOnlyMy, paginationModel, defferedRequest.resolve, defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _findAuthorizedCategoryFascicles(categoryId: number, paginationModel?: PaginationModel): JQueryPromise<FascicleModel[]> {
        let defferedRequest: JQueryDeferred<FascicleModel[]> = $.Deferred<FascicleModel[]>();

        this._fascicleService.getAuthorizedCategoryFascicles(categoryId, paginationModel, defferedRequest.resolve, defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _findProcessDossierFolders(processId: string, paginationModel?: PaginationModel): JQueryPromise<DossierFolderModel[] | ODATAResponseModel<DossierFolderModel>> {
        let defferedRequest: JQueryDeferred<DossierFolderModel[] | ODATAResponseModel<DossierFolderModel>> = $.Deferred<DossierFolderModel[] | ODATAResponseModel<DossierFolderModel>>();

        this._dossierFolderService.getDossierFoldersByProcessId(processId, defferedRequest.resolve, defferedRequest.reject, paginationModel);

        return defferedRequest.promise();
    }
    private _findDossierFolderChildren(dossierFolderId: string, paginationModel?: PaginationModel): JQueryPromise<DossierSummaryFolderViewModel[]> {
        let defferedRequest: JQueryDeferred<DossierSummaryFolderViewModel[]> = $.Deferred<DossierSummaryFolderViewModel[]>();

        this._dossierFolderService.getDossierFolderChildren(dossierFolderId, paginationModel, defferedRequest.resolve, defferedRequest.reject);

        return defferedRequest.promise();
    } 
    private _findFascicleFolderChildren(fascicleFolderId: string, paginationModel?: PaginationModel): JQueryPromise<FascicleSummaryFolderViewModel[]> {
        let defferedRequest: JQueryDeferred<FascicleSummaryFolderViewModel[]> = $.Deferred<FascicleSummaryFolderViewModel[]>();

        this._fascicleFolderService.getFascicleFolderChildren(fascicleFolderId, paginationModel, defferedRequest.resolve, defferedRequest.reject);

        return defferedRequest.promise();
    } 
    private _findCategoryChildren(categoryId: number, paginationModel?: PaginationModel, tenantAOOId?: string): JQueryPromise<ODATAResponseModel<CategoryModel>> {
        let defferedRequest: JQueryDeferred<ODATAResponseModel<CategoryModel>> = $.Deferred<ODATAResponseModel<CategoryModel>>();

        this._categoryService.getSubCategories(categoryId, defferedRequest.resolve, defferedRequest.reject, paginationModel, tenantAOOId);

        return defferedRequest.promise();
    }
    private _getCategoryChildrenCount = (categoryId: number): JQueryPromise<number> => {
        let defferedRequest: JQueryDeferred<number> = $.Deferred<number>();

        this._categoryService.countSubCategories(categoryId, defferedRequest.resolve, defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _getCategoryProcessesCount = (categoryId: number): JQueryPromise<number> => {
        let defferedRequest: JQueryDeferred<number> = $.Deferred<number>();

        this._processService.countCategoryProcesses(categoryId, this._showOnlyMyProcesses(), defferedRequest.resolve, defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _getCategoryFasciclesCount = (categoryId: number): JQueryPromise<number> => {
        let defferedRequest: JQueryDeferred<number> = $.Deferred<number>();

        this._fascicleService.countAuthorizedCategoryFascicles(categoryId, defferedRequest.resolve, defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _getTenantFascileDocumentUnitsCount = (tenantAOOId: string, fascicleId: string, fascicleFolderId: string): JQueryPromise<number> => {
        let defferedRequest: JQueryDeferred<number> = $.Deferred<number>();

        this._documentUnitService.countTenantFascicleDocumentUnits(tenantAOOId, fascicleId, fascicleFolderId, defferedRequest.resolve, defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _getFascicleFolderChildrenCount = (fascicleFolderId: string): JQueryPromise<number> => {
        let defferedRequest: JQueryDeferred<number> = $.Deferred<number>();

        this._fascicleFolderService.countFascicleFolderChildren(fascicleFolderId, defferedRequest.resolve, defferedRequest.reject);

        return defferedRequest.promise();
    }
    /// endregion [ FINDER METHODS ]

    /// region [ LOAD TREE NODE ITEMS METHODS]
    private _initializeCategoryTreeView(): void {

        if (this._categoryTreeView.get_nodes().get_count() > 0)
            this._categoryTreeView.get_nodes().clear();

        let rootNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        rootNode.set_text(ProcessesTreeView.ROOTNODE_DESCRIPTION);
        rootNode.get_attributes().setAttribute(ProcessesTreeView.NODETYPE_ATTRNAME, TreeNodeType.Root);
        this._categoryTreeView.get_nodes().add(rootNode);

        rootNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        this._categoryService.getTenantAOORootCategory(this.currentTenantAOOId, (tenantAOOCategory: CategoryModel) => {

            if (!tenantAOOCategory) {
                rootNode.hideLoadingStatus();
                return;
            }

            rootNode.set_value(tenantAOOCategory.EntityShortId);

            this._loadCategories(rootNode, this._defaultPaginationModel)
                .done(() => {
                    rootNode.hideLoadingStatus();
                    rootNode.set_expanded(true);

                    rootNode.get_attributes().setAttribute(ProcessesTreeView.ROOTNODE_CATEGORYID_ATTRNAME, tenantAOOCategory.EntityShortId);
                    const expandedNodeParentTotalChildrenCount: number = rootNode.get_attributes().getAttribute(ProcessesTreeView.TOTAL_CHILDREN_COUNT_ATTRNAME);
                    this._appendLoadMoreNode(rootNode, expandedNodeParentTotalChildrenCount);
                });
        });
    }

    private _loadCategories(expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel = null): JQueryPromise<Telerik.Web.UI.RadTreeNode[]> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();

        this._findCategoryChildren(expandedNode.get_value(), paginationModel, this.currentTenantAOOId)
            .then((odataResult: ODATAResponseModel<CategoryModel>) => this._createCategoryNodesFromModels(odataResult, expandedNode))
            .done(defferedRequest.resolve)
            .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _loadCategoryProcesses(expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel = null): JQueryPromise<Telerik.Web.UI.RadTreeNode[]> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();
        const totalChildrenCount: number | undefined = expandedNode.get_attributes().getAttribute(ProcessesTreeView.TOTAL_CHILDREN_COUNT_ATTRNAME),
              childrenAlreadyCounted: boolean = totalChildrenCount !== undefined,
              categoryId: number = expandedNode.get_value();

        if (childrenAlreadyCounted) {
            this._findCategoryProcesses(categoryId, this._showOnlyMyProcesses(), paginationModel)
                .then((processes: ProcessModel[]) => this._createProcessNodesFromModels(processes, categoryId))
                .then(defferedRequest.resolve)
                .fail(defferedRequest.reject);
        } else {
            this._processService.countCategoryProcesses(categoryId, this._showOnlyMyProcesses(), (totalChildren: number) => {

                if (!totalChildren) {
                    defferedRequest.resolve([]);
                    return;
                }

                this._appendChildrenCountAttribute(expandedNode, totalChildren);

                this._findCategoryProcesses(categoryId, this._showOnlyMyProcesses(), paginationModel)
                    .then((processes: ProcessModel[]) => this._createProcessNodesFromModels(processes, categoryId))
                    .then(defferedRequest.resolve)
                    .fail(defferedRequest.reject);
            });
        }

        return defferedRequest.promise();
    }
    private _loadCategoryFascicles(expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel = null): JQueryPromise<Telerik.Web.UI.RadTreeNode[]> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();
        const totalChildrenCount: number | undefined = expandedNode.get_attributes().getAttribute(ProcessesTreeView.TOTAL_CHILDREN_COUNT_ATTRNAME),
            childrenAlreadyCounted: boolean = totalChildrenCount !== undefined,
            categoryId: number = expandedNode.get_value();

        if (childrenAlreadyCounted) {
            this._findAuthorizedCategoryFascicles(categoryId, paginationModel)
                .then(this._createFascicleNodesFromModels)
                .then(defferedRequest.resolve)
                .fail(defferedRequest.reject);
        } else {
            this._fascicleService.countAuthorizedCategoryFascicles(categoryId, (totalChildren: number) => {

                if (!totalChildren) {
                    defferedRequest.resolve([]);
                    return;
                }

                this._appendChildrenCountAttribute(expandedNode, totalChildren);

                this._findAuthorizedCategoryFascicles(categoryId, paginationModel)
                    .then(this._createFascicleNodesFromModels)
                    .then(defferedRequest.resolve)
                    .fail(defferedRequest.reject);
            });
        }

        return defferedRequest.promise();
    }
    private _loadProcessDossierFolders(processId: string, expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel = null): JQueryPromise<void> {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        this._findProcessDossierFolders(processId, paginationModel)
            .then((odataResponse: ODATAResponseModel<DossierFolderModel>) => {
                if (!odataResponse.value.length) {

                    if (!expandedNode.get_nodes().get_count()) {
                        this._appendEmptyNode(expandedNode);
                    }

                    return defferedRequest.resolve();
                }

                this._appendDossierFolderNodesToParent(odataResponse.value, expandedNode);

                if (odataResponse.count) {
                    this._appendChildrenCountAttribute(expandedNode, odataResponse.count);
                }
            })
            .done(defferedRequest.resolve)
            .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }
    private _loadDossierFoldersChildren(expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();
        const totalChildrenCount: number = expandedNode.get_attributes().getAttribute(ProcessesTreeView.TOTAL_CHILDREN_COUNT_ATTRNAME),
            childrenAlreadyCounted: boolean = !!totalChildrenCount,
            dossierFolderId: string = expandedNode.get_value();

        if (childrenAlreadyCounted) {
            this._findDossierFolderChildren(dossierFolderId, paginationModel)
                .then((dossierFolderViewModels: DossierSummaryFolderViewModel[]) => {
                    if (!dossierFolderViewModels.length) {

                        if (!expandedNode.get_nodes().get_count()) {
                            this._appendEmptyNode(expandedNode);
                        }

                        defferedRequest.resolve();
                        return;
                    }

                    this._appendDossierFolderViewModelNodesToParent(dossierFolderViewModels, expandedNode);
                })
                .done(defferedRequest.resolve)
                .fail(defferedRequest.reject);
        } else {
            this._dossierFolderService.countDossierFolderChildren(dossierFolderId, (totalChildren: number) => {

                if (!totalChildren) {
                    this._appendEmptyNode(expandedNode);
                    defferedRequest.resolve();
                    return;
                }

                this._appendChildrenCountAttribute(expandedNode, totalChildren);

                this._findDossierFolderChildren(dossierFolderId, paginationModel)
                    .then((dossierFolderViewModels: DossierSummaryFolderViewModel[]) => {
                        if (!dossierFolderViewModels.length) {

                            if (!expandedNode.get_nodes().get_count()) {
                                this._appendEmptyNode(expandedNode);
                            }

                            defferedRequest.resolve();
                            return;
                        }

                        this._appendDossierFolderViewModelNodesToParent(dossierFolderViewModels, expandedNode);
                    })
                    .done(defferedRequest.resolve)
                    .fail(defferedRequest.reject);
            });
        }

        return defferedRequest.promise();
    }
    private _loadFascicleFolders(expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<Telerik.Web.UI.RadTreeNode[]> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();
        const totalChildrenCount: number = expandedNode.get_attributes().getAttribute(ProcessesTreeView.TOTAL_CHILDREN_COUNT_ATTRNAME),
            childrenAlreadyCounted: boolean = !!totalChildrenCount,
            fascicleFolderId: string = expandedNode.get_value();

        if (childrenAlreadyCounted) {
            this._findFascicleFolderChildren(fascicleFolderId, paginationModel)
                .then(this._createFascicleFolderNodesFromModels)
                .done(defferedRequest.resolve)
                .fail(defferedRequest.reject);
        } else {
            this._fascicleFolderService.countFascicleFolderChildren(fascicleFolderId, (totalChildren: number) => {

                if (!totalChildren) {
                    defferedRequest.resolve([]);
                    return;
                }

                this._appendChildrenCountAttribute(expandedNode, totalChildren);

                this._findFascicleFolderChildren(fascicleFolderId, paginationModel)
                    .then(this._createFascicleFolderNodesFromModels)
                    .done(defferedRequest.resolve)
                    .fail(defferedRequest.reject);
            }, defferedRequest.reject);
        }

        return defferedRequest.promise();
    }
    private _loadFascicleFolderDocumentUnits(expandedNode: Telerik.Web.UI.RadTreeNode, fascicleFolderId: string, nodeType: TreeNodeType, paginationModel: PaginationModel): JQueryPromise<Telerik.Web.UI.RadTreeNode[]> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();
        let fascicleId: string = expandedNode.get_attributes().getAttribute(ProcessesTreeView.FASCICLEID_ATTRNAME);

        this._documentUnitService.getTenantFascicleDocumentUnits(this.currentTenantAOOId, fascicleId, fascicleFolderId,
            (fascicleFolderDocUnits: ODATAResponseModel<DocumentUnitModel>) => {
                let fascFolderDocUnitTreeNodeCollection: Telerik.Web.UI.RadTreeNode[]
                    = fascicleFolderDocUnits.value.map((fascicleFolderDocUnit: DocumentUnitModel) => this._createDocumentUnitNode(fascicleFolderDocUnit, nodeType));

                if (fascicleFolderDocUnits.count) {
                    this._appendChildrenCountAttribute(expandedNode, fascicleFolderDocUnits.count);
                }
                defferedRequest.resolve(fascFolderDocUnitTreeNodeCollection);
            }, (exception: ExceptionDTO) => defferedRequest.reject(exception), paginationModel);

        return defferedRequest.promise();
    }
    /// endregion [ LOAD TREE NODE ITEMS METHODS]

    /// region [ ** HELPER METHODS ** ]
    private _registerNodeTypesExpandingActions(): void {
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.Root] = (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
            let categoryLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            this._loadCategories(expandedNode, paginationModel)
                .done(() => categoryLoadingDefferedRequest.resolve())
                .fail((exception: ExceptionDTO) => categoryLoadingDefferedRequest.reject(exception));

            return categoryLoadingDefferedRequest.promise();
        }
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.Category] = (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
            let categoryChildrenLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            const totalChildrenCount: number | undefined = expandedNode.get_attributes().getAttribute(ProcessesTreeView.TOTAL_CHILDREN_COUNT_ATTRNAME),
                childrenAlreadyCounted: boolean = totalChildrenCount !== undefined,
                categoryId: number = expandedNode.get_value();

            if (childrenAlreadyCounted) {
                this._findCategoryChildren(categoryId, paginationModel, this.currentTenantAOOId)
                    .then((odataResult: ODATAResponseModel<CategoryModel>) => this._createCategoryNodesFromModels(odataResult, expandedNode))
                    .then((subCategories: Telerik.Web.UI.RadTreeNode[]) => this._appendCategoryNodeChildren(subCategories, expandedNode))
                    .done(categoryChildrenLoadingDefferedRequest.resolve)
                    .fail(categoryChildrenLoadingDefferedRequest.reject);
            } else {
                let countingRequestPromises: JQueryPromise<number>[] =
                    [
                        this._getCategoryChildrenCount(categoryId),
                        this._getCategoryProcessesCount(categoryId),
                        this._getCategoryFasciclesCount(categoryId)
                    ];

                $.when(...countingRequestPromises)
                    .done((subCategoriesCount, processesCount, fasciclesCount) => {
                        const categoryTotalChildrenCount: number = subCategoriesCount + processesCount + fasciclesCount;

                        if (categoryTotalChildrenCount) {
                            this._appendChildrenCountAttribute(expandedNode, categoryTotalChildrenCount);
                        }

                        this._findCategoryChildren(categoryId, paginationModel, this.currentTenantAOOId)
                            .then((odataResult: ODATAResponseModel<CategoryModel>) => this._createCategoryNodesFromModels(odataResult, expandedNode))
                            .then((subCategories: Telerik.Web.UI.RadTreeNode[]) => this._appendCategoryNodeChildren(subCategories, expandedNode))
                            .done(categoryChildrenLoadingDefferedRequest.resolve)
                            .fail(categoryChildrenLoadingDefferedRequest.reject);

                    }).fail(categoryChildrenLoadingDefferedRequest.reject);
            }

            return categoryChildrenLoadingDefferedRequest.promise();
        }
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.Process] = (parentNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
            let processDossierFoldersLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            this._loadProcessDossierFolders(parentNode.get_value(), parentNode, paginationModel)
                .done(() => processDossierFoldersLoadingDefferedRequest.resolve())
                .fail((exception: ExceptionDTO) => processDossierFoldersLoadingDefferedRequest.reject(exception));

            return processDossierFoldersLoadingDefferedRequest.promise();
        };
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.DossierFolder] = (parentNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
            let dossierFolderChildrenLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            this._loadDossierFoldersChildren(parentNode, paginationModel)
                .done(() => dossierFolderChildrenLoadingDefferedRequest.resolve())
                .fail((exception: ExceptionDTO) => dossierFolderChildrenLoadingDefferedRequest.reject(exception));

            return dossierFolderChildrenLoadingDefferedRequest.promise();
        };
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.Fascicle] = (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
            let fascicleFoldersLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            this._loadFascicleFolders(expandedNode, paginationModel)
                .done((fascFolders: Telerik.Web.UI.RadTreeNode[]) => {
                    if (fascFolders.length === 0) {
                        if (!expandedNode.get_nodes().get_count()) {
                            this._appendEmptyNode(expandedNode);
                        }
                        fascicleFoldersLoadingDefferedRequest.resolve();
                        return;
                    }

                    fascFolders.forEach((fascFolderNode: Telerik.Web.UI.RadTreeNode) => {
                        expandedNode.get_nodes().add(fascFolderNode);
                    });
                    fascicleFoldersLoadingDefferedRequest.resolve();
                })
                .fail((exception: ExceptionDTO) => fascicleFoldersLoadingDefferedRequest.reject(exception));

            return fascicleFoldersLoadingDefferedRequest.promise();
        };
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.FascicleFolder] = (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
            let fascFolderChildrenLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            const totalChildrenCount: number | undefined = expandedNode.get_attributes().getAttribute(ProcessesTreeView.TOTAL_CHILDREN_COUNT_ATTRNAME),
                childrenAlreadyCounted: boolean = totalChildrenCount !== undefined,
                parentFascicleFolderId: string = expandedNode.get_value();

            if (childrenAlreadyCounted) {
                this._findFascicleFolderChildren(parentFascicleFolderId, paginationModel)
                    .then(this._createFascicleFolderNodesFromModels)
                    .then((fascicleFolders: Telerik.Web.UI.RadTreeNode[]) => this._appendFascicleFolderNodeChildren(fascicleFolders, expandedNode))
                    .done(fascFolderChildrenLoadingDefferedRequest.resolve)
                    .fail(fascFolderChildrenLoadingDefferedRequest.reject);
            } else {
                let fascicleId: string = expandedNode.get_attributes().getAttribute(ProcessesTreeView.FASCICLEID_ATTRNAME);
                let countingRequestPromises: JQueryPromise<number>[] =
                    [
                        this._getFascicleFolderChildrenCount(parentFascicleFolderId),
                        this._getTenantFascileDocumentUnitsCount(this.currentTenantAOOId, fascicleId, parentFascicleFolderId)
                    ];

                $.when(...countingRequestPromises)
                    .done((fascicleFoldersCount, fascicleDocumentUnitsCount) => {
                        const fascicleFolderTotalChildrenCount: number = fascicleFoldersCount + fascicleDocumentUnitsCount;

                        if (fascicleFolderTotalChildrenCount) {
                            this._appendChildrenCountAttribute(expandedNode, fascicleFolderTotalChildrenCount);
                        }

                        this._findFascicleFolderChildren(parentFascicleFolderId, paginationModel)
                            .then(this._createFascicleFolderNodesFromModels)
                            .then((fascicleFolders: Telerik.Web.UI.RadTreeNode[]) => this._appendFascicleFolderNodeChildren(fascicleFolders, expandedNode))
                            .done(fascFolderChildrenLoadingDefferedRequest.resolve)
                            .fail(fascFolderChildrenLoadingDefferedRequest.reject);

                    }).fail(fascFolderChildrenLoadingDefferedRequest.reject);
            }

            return fascFolderChildrenLoadingDefferedRequest.promise();
        }
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.LoadMore] = (expandedNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
            let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            const expandedNodeParent: Telerik.Web.UI.RadTreeNode = expandedNode.get_parent();
            const parentNodeChildrenCount: number = expandedNodeParent.get_nodes().get_count();
            const parentNodeType: TreeNodeType = expandedNodeParent.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME);

            const expandedNodeActionHandler: (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel) => JQueryPromise<void>
                = this._nodeTypeExpandingActionsDictionary[parentNodeType];

            if (!expandedNodeActionHandler) {
                return;
            }

            // exclude "Load more" node from children collection count
            const recordsToSkip: number = parentNodeChildrenCount - 1;
            const paginationModel: PaginationModel = new PaginationModel(recordsToSkip, this.treeViewNodesPageSize);

            expandedNodeActionHandler(expandedNodeParent, paginationModel)
                .done(() => {
                    defferedRequest.resolve();
                }).fail((exception: ExceptionDTO) => defferedRequest.reject(exception));

            return defferedRequest.promise();
        }
    }

    private _registerNodeTypesClickActions(): void {
        this._nodeTypeClickActionsDictionary[TreeNodeType.Fascicle] = (clickedFascicleNode: Telerik.Web.UI.RadTreeNode): void => {
            let fascicleId: string = clickedFascicleNode.get_value();

            if (fascicleId) {
                let url = `../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${fascicleId}`;
                this._detailsPane.set_contentUrl(url);
            }
        }
        this._nodeTypeClickActionsDictionary[TreeNodeType.DocumentUnit] = (clickedDocUnitNode: Telerik.Web.UI.RadTreeNode): void => {
            let detailsPageUrl: string = this._createEnvironmentDetailsPageUrl(clickedDocUnitNode);

            if (detailsPageUrl) {
                this._detailsPane.set_contentUrl(detailsPageUrl);
            }
        }
        this._nodeTypeClickActionsDictionary[TreeNodeType.Process] = (clickedProcessNode: Telerik.Web.UI.RadTreeNode): void => {
            let processId: string = clickedProcessNode.get_value();

            if (processId) {
                let url = `../Processes/ProcessVisualizza.aspx?Type=Comm&IdProcess=${processId}&ReadOnlyRoles=${true}`;
                this._detailsPane.set_contentUrl(url);
            }
        }
    }

    private _initializeDictionaries(): void {
        /// Nodes expanding event handlers registration
        this._nodeTypeExpandingActionsDictionary = {};
        this._registerNodeTypesExpandingActions();

        /// Nodes click event handlers registration
        this._nodeTypeClickActionsDictionary = {};
        this._registerNodeTypesClickActions();

        /// Dossier folders images and tooltips
        this._dossierFolderStatusImageDictionary = {};
        this._dossierFolderStatusTooltipDictionary = {};
        this._registerDossierFoldersStatusImages();
        this._registerDossierFoldersStatusTooltips();

        /// Fascicle folders images and tooltips
        this._fascicleFolderStatusImageDictionary = {};
        this._fascicleFolderStatusTooltipDictionary = {};
        this._registerFascicleFoldersStatusImages();
        this._registerFascicleFoldersStatusTooltips();

        /// Expanded folders images
        this._expandedFolderImageDictionary = {};
        this._registerExpandedFoldersImages();

        /// Document units environment images
        this._environmentImageDictionary = {};
        this._registerEnvironmentImages();
    }

    private _registerDossierFoldersStatusImages(): void {
        this._dossierFolderStatusImageDictionary[DossierFolderStatus.DoAction] = "../App_Themes/DocSuite2008/imgset16/folder_hidden.png";
        this._dossierFolderStatusImageDictionary[DossierFolderStatus.InProgress] = "../App_Themes/DocSuite2008/imgset16/folder_hidden.png";
        this._dossierFolderStatusImageDictionary[DossierFolderStatus.Fascicle] = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
        this._dossierFolderStatusImageDictionary[DossierFolderStatus.FascicleClose] = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
        this._dossierFolderStatusImageDictionary[DossierFolderStatus.Folder] = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
    }
    private _registerDossierFoldersStatusTooltips(): void {
        this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.DoAction] = "Da gestire";
        this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.InProgress] = "Da gestire";
        this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.Fascicle] = "Fascicolo";
        this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.FascicleClose] = "Fascicolo chiuso";
        this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.Folder] = "Cartella con sottocartelle";
    }
    private _registerFascicleFoldersStatusImages(): void {
        this._fascicleFolderStatusImageDictionary[FascicleFolderStatus.Active] = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
        this._fascicleFolderStatusImageDictionary[FascicleFolderStatus.Internet] = "../App_Themes/DocSuite2008/imgset16/folder_internet_closed.png";
    }
    private _registerFascicleFoldersStatusTooltips(): void {
        this._fascicleFolderStatusTooltipDictionary[FascicleFolderStatus.Active] = "Cartella attiva";
        this._fascicleFolderStatusTooltipDictionary[FascicleFolderStatus.Internet] = "Cartella pubblica via internet";
    }
    private _registerExpandedFoldersImages(): void {
        this._expandedFolderImageDictionary[DossierFolderStatus.Folder] = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
        this._expandedFolderImageDictionary[FascicleFolderTypology.SubFascicle] = "../App_Themes/DocSuite2008/imgset16/folder_internet_open.png";
    }
    private _registerEnvironmentImages(): void {
        this._environmentImageDictionary[Environment.Protocol] = "../Comm/Images/DocSuite/Protocollo16.png";
        this._environmentImageDictionary[Environment.Resolution] = "../Comm/Images/DocSuite/Atti16.png";
        this._environmentImageDictionary[Environment.DocumentSeries] = "../App_Themes/DocSuite2008/imgset16/document_copies.png";
        this._environmentImageDictionary[Environment.UDS] = "../App_Themes/DocSuite2008/imgset16/document_copies.png";
    }
    private _createProcessNodesFromModels = (processModels: ProcessModel[], parentCategoryId: number): Telerik.Web.UI.RadTreeNode[] => {
        return processModels.map((process: ProcessModel) => {
            let currentProcessTreeNode: Telerik.Web.UI.RadTreeNode
                = this._createTreeNode(TreeNodeType.Process, process.Name, process.UniqueId, ProcessesTreeView.PROCESS_NODE_IMAGEURL);
            currentProcessTreeNode.get_attributes().setAttribute(ProcessesTreeView.CATEGORYID_ATTRNAME, parentCategoryId);

            this._appendEmptyNode(currentProcessTreeNode);

            return currentProcessTreeNode;
        });
    }
    private _createFascicleNodesFromModels = (fascicleModels: FascicleModel[]): Telerik.Web.UI.RadTreeNode[] => {
        return fascicleModels.map((fasc: FascicleModel) => {
            let fascicleImageUrl: string = this._dossierFolderStatusImageDictionary[DossierFolderStatus.Fascicle];
            let fascicleTooltip: string = this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.Fascicle];
            let fascicleExpandedImageUrl: string = this._expandedFolderImageDictionary[DossierFolderStatus.Fascicle];
            let nodeDescription: string = `${fasc.Title}-${fasc.FascicleObject}`;

            let currentFascTreeNode: Telerik.Web.UI.RadTreeNode
                = this._createTreeNode(TreeNodeType.Fascicle, nodeDescription, fasc.UniqueId, fascicleImageUrl, undefined, fascicleTooltip, fascicleExpandedImageUrl);
            currentFascTreeNode.get_attributes().setAttribute(ProcessesTreeView.FASCISACTIVE_ATTRNAME, !fasc.EndDate);
            this._appendEmptyNode(currentFascTreeNode);
            return currentFascTreeNode;
        });
    }
    private _createFascicleFolderNodesFromModels = (fascicleFolders: FascicleSummaryFolderViewModel[]): Telerik.Web.UI.RadTreeNode[] => {
        return fascicleFolders.map((fascicleFolder: FascicleSummaryFolderViewModel) => {
            let fascicleFolderImageUrl: string = this._dossierFolderStatusImageDictionary[DossierFolderStatus.Folder];
            let fascicleFolderTooltip: string = this._fascicleFolderStatusTooltipDictionary[FascicleFolderStatus[fascicleFolder.Status]];
            let fascicleFolderExpandedImageUrl: string = this._expandedFolderImageDictionary[DossierFolderStatus.Folder];

            let currentFascicleFolderTreeNode: Telerik.Web.UI.RadTreeNode
                = this._createTreeNode(TreeNodeType.FascicleFolder, fascicleFolder.Name, fascicleFolder.UniqueId, fascicleFolderImageUrl, undefined, fascicleFolderTooltip, fascicleFolderExpandedImageUrl);
            currentFascicleFolderTreeNode.get_attributes().setAttribute(ProcessesTreeView.FASCICLEID_ATTRNAME, fascicleFolder.idFascicle);
            this._appendEmptyNode(currentFascicleFolderTreeNode);

            return currentFascicleFolderTreeNode;
        });
    }
    private _createCategoryNodesFromModels = (categoryModelsResult: ODATAResponseModel<CategoryModel>, expandedNode: Telerik.Web.UI.RadTreeNode): Telerik.Web.UI.RadTreeNode[] => {
        let parentNodeIsTreeRootNode: boolean = expandedNode === this._categoryTreeViewRootNode();
        let categoryTreeNodeCollection: Telerik.Web.UI.RadTreeNode[] = categoryModelsResult.value.map((categoryModel: CategoryModel) => {
            let categoryNodeDescription: string = `${categoryModel.Code}.${categoryModel.Name}`;
            let categoryNodeImageUrl: string = ProcessesTreeView.CATEGORY_NODE_IMAGEURL;

            let currentCategoryTreeNode: Telerik.Web.UI.RadTreeNode
                = this._createTreeNode(TreeNodeType.Category, categoryNodeDescription, categoryModel.EntityShortId, categoryNodeImageUrl, parentNodeIsTreeRootNode ? expandedNode : undefined);

            this._appendEmptyNode(currentCategoryTreeNode);
            return currentCategoryTreeNode;
        });

        if (categoryModelsResult.count) {
            this._appendChildrenCountAttribute(expandedNode, categoryModelsResult.count);
        }

        return categoryTreeNodeCollection;
    }

    /**
     * Creates a RadTreeNode object with the given details. 
     * If the parent node is passed, the new created node is added to the parent node collection.
     * @param nodeType
     * @param nodeDescription
     * @param nodeValue
     * @param imageUrl
     * @param parentNode
     * @param tooltipText
     * @param expandedImageUrl
     */
    private _createTreeNode(nodeType: TreeNodeType, nodeDescription: string, nodeValue: number | string, imageUrl: string, parentNode?: Telerik.Web.UI.RadTreeNode, tooltipText?: string, expandedImageUrl?: string): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        treeNode.set_text(nodeDescription);
        treeNode.set_value(nodeValue);
        treeNode.get_attributes().setAttribute(ProcessesTreeView.NODETYPE_ATTRNAME, nodeType);

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

    private _initializeControls(): void {
        this._uscNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
        this._actionToolbar = <Telerik.Web.UI.RadToolBar>$find(this.actionToolbarId);
        this._categoryTreeView = <Telerik.Web.UI.RadTreeView>$find(this.categoryTreeViewId);
        this._detailsPane = <Telerik.Web.UI.RadPane>$find(this.detailsPaneId);
        this._windowManager = <Telerik.Web.UI.RadWindowManager>$find(this.windowManagerId);
        this._windowMoveFasc = <Telerik.Web.UI.RadWindow>$find(this.windowMoveFascId);
        this._windowMoveFasc.add_close(this._updateProcessesTreeView);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        if (this._actionToolbar) {
            this._actionToolbar.add_buttonClicked(this.actionToolbar_ButtonClicked);

            this._moveFascicleToolbarBtn = this._actionToolbar.findButtonByCommandName(ProcessesTreeView.COMMANDNAME_MOVEFASCICLE);
            this._moveFascicleToolbarBtn.set_enabled(false);

            this._editProcessToolbarBtn = this._actionToolbar.findButtonByCommandName(ProcessesTreeView.COMMANDNAME_EDITPROCESS);
            this._editProcessToolbarBtn.set_enabled(false);
        }
    }
    protected actionToolbar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let currentActionButtonItem: Telerik.Web.UI.RadToolBarButton = args.get_item() as Telerik.Web.UI.RadToolBarButton;
        let currentAction: () => void = this._toolbarActions().filter((item: [string, () => void]) => item[0] == currentActionButtonItem.get_commandName())
            .map((item: [string, () => void]) => item[1])[0];
        currentAction();
    }

    private _updateProcessesTreeView = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs): void => {
        let fascMoveResponseModel: FascProcessMoveResponseModelDTO = args.get_argument() as FascProcessMoveResponseModelDTO;

        if (!fascMoveResponseModel) {
            return;
        }

        // Get selected fascicle category in order to optimize the search in the tree, of the new dossier folder parent
        let categoryParent: Telerik.Web.UI.RadTreeNode = this._categoryTreeView.findNodeByValue(fascMoveResponseModel.categoryId.toString());
        let newDossierFolderParentId: string = fascMoveResponseModel.newDossierFolderParentId;
        let newDossierFolderParent: Telerik.Web.UI.RadTreeNode = categoryParent.get_allNodes().filter(node => node.get_value() === newDossierFolderParentId)[0];

        // Remove selected fascicle from the initial parent node collection
        let currentSelectedFascicle: Telerik.Web.UI.RadTreeNode = this._categoryTreeView.get_selectedNode();
        let currentSelectedFascicleParent: Telerik.Web.UI.RadTreeNode = currentSelectedFascicle.get_parent();
        currentSelectedFascicle.get_parent().get_nodes().remove(currentSelectedFascicle);

        // If source dossierfolder has no more children, collapse and change node icon
        if (currentSelectedFascicleParent.get_nodes().get_count() === 0) {
            currentSelectedFascicleParent.set_expanded(false);
            currentSelectedFascicleParent.set_imageUrl(this._dossierFolderStatusImageDictionary[DossierFolderStatus.InProgress]);
        }

        // If the new dossierfolder parent is visible in the tree, expand it and update node image
        if (newDossierFolderParent) {
            newDossierFolderParent.set_imageUrl(this._expandedFolderImageDictionary[DossierFolderStatus.Folder]);

            newDossierFolderParent.get_nodes().clear();
            newDossierFolderParent.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            this._loadDossierFoldersChildren(newDossierFolderParent, this._defaultPaginationModel)
                .done(() => {
                    newDossierFolderParent.hideLoadingStatus();
                    newDossierFolderParent.set_expanded(true);
                })
                .fail((exception: ExceptionDTO) => {
                    this._showNotificationException(exception);
                });
        }
    }

    private _moveFascicle(): void {
        let selectedFascicleNode: Telerik.Web.UI.RadTreeNode = this._categoryTreeView.get_selectedNode();
        let selectedFascicleId: string = selectedFascicleNode.get_value();

        if (!selectedFascicleId) {
            alert("Fascicolo non valido");
            return;
        }

        let parentNode: Telerik.Web.UI.RadTreeNode = selectedFascicleNode.get_parent();
        let parentNodeType: TreeNodeType = parentNode.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME);
        let fascicleToMoveId: string = parentNodeType === TreeNodeType.Category
            ? selectedFascicleId
            : selectedFascicleNode.get_attributes().getAttribute(ProcessesTreeView.DOSSIERFOLDERID_ATTRNAME);

        if (parentNodeType === TreeNodeType.Category) {
            let parentCategoryHasProcesses: boolean = parentNode.get_allNodes().some(node => {
                let nodeType: TreeNodeType = node.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME);
                return nodeType === TreeNodeType.Process;
            });

            if (!parentCategoryHasProcesses) {
                this._showWarningMessage("La categoria selezionata non contiene nessun processo, quindi non e' possibile spostare il fascicolo.");
                return;
            }

            let fascicleIsActive: boolean = selectedFascicleNode.get_attributes().getAttribute(ProcessesTreeView.FASCISACTIVE_ATTRNAME);

            if (!fascicleIsActive) {
                this._showWarningMessage("Il fascicolo selezionato non è attivo");
                return;
            }

            this._openMoveFascicleWindow(fascicleToMoveId);
        } else {
            this._dossierFolderService.checkIfDossierFolderFascicleIsActive(fascicleToMoveId,
            (fascicleIsActive: boolean) =>
            {
                if (!fascicleIsActive) {
                    this._showWarningMessage("Il fascicolo selezionato non è attivo");
                    return;
                }

                this._openMoveFascicleWindow(fascicleToMoveId, parentNode.get_value());
            }, (exception: ExceptionDTO) => this._showNotificationException(exception));
        }
    }

    private _openMoveFascicleWindow(fascicleToMoveId: string, parentFolderId?: string): void {
        let url: string = `../Fasc/FascProcessMove.aspx?Type=Comm&FascicleId=${fascicleToMoveId}`;

        if (parentFolderId) {
            url += `&ParentFolderId=${parentFolderId}`;
        }

        this._windowManager.open(url, "windowMoveFasc", undefined);
    }

    private _redirectToProcessEditPage = (): void => {
        let selectedProcess: Telerik.Web.UI.RadTreeNode = this._categoryTreeView.get_selectedNode();
        let processId: string = selectedProcess.get_value();
        let processCategoryId: number = selectedProcess.get_attributes().getAttribute(ProcessesTreeView.CATEGORYID_ATTRNAME);

        this._loadingPanel.show(this.splitterMainId);
        let editPageUrl: string = `../Tblt/TbltProcess.aspx?Type=Comm&IdProcess=${processId}&IdCategory=${processCategoryId}`;
        window.open(editPageUrl, "main");
    }

    private _appendChildrenCountAttribute(node: Telerik.Web.UI.RadTreeNode, totalChildrenCount: number): void {
        const currentChildrenAttributeValue: number | undefined = node.get_attributes().getAttribute(ProcessesTreeView.TOTAL_CHILDREN_COUNT_ATTRNAME);

        if (currentChildrenAttributeValue === undefined) {
            node.get_attributes().setAttribute(ProcessesTreeView.TOTAL_CHILDREN_COUNT_ATTRNAME, totalChildrenCount);
        }
    }

    private _appendEmptyNode(parentNode: Telerik.Web.UI.RadTreeNode): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text("Nessun elemento trovato");
        parentNode.get_nodes().add(emptyNode);
    }

    private _appendLoadMoreNode(expandedNode: Telerik.Web.UI.RadTreeNode, totalChildrenCount: number) {
        let expandedNodeType: TreeNodeType = expandedNode.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME);

        if (expandedNodeType === TreeNodeType.LoadMore) {
            const expandedNodeParentChildrenCollection: Telerik.Web.UI.RadTreeNodeCollection = expandedNode.get_parent().get_nodes();
            const allChildrenLoaded: boolean = (expandedNodeParentChildrenCollection.get_count() - 1) === totalChildrenCount;

            expandedNodeParentChildrenCollection.remove(expandedNode);
            if (!allChildrenLoaded) {
                expandedNodeParentChildrenCollection.add(this._createLoadMoreNode());
            }
        } else {
            const expandedNodeChildrenCollection: Telerik.Web.UI.RadTreeNodeCollection = expandedNode.get_nodes(),
                loadedAllChildren: boolean = expandedNodeChildrenCollection.get_count() === totalChildrenCount;

            if (loadedAllChildren) {
                return;
            }

            expandedNodeChildrenCollection.add(this._createLoadMoreNode());
        }

    }

    private _appendDossierFolderViewModelNodesToParent(dossierFolderModels: DossierSummaryFolderViewModel[], parentNode: Telerik.Web.UI.RadTreeNode): void {
        dossierFolderModels.forEach((dossierFolder: DossierSummaryFolderViewModel) => {
            let dossierFolderIsFascicle: boolean = !!dossierFolder.idFascicle;

            let nodeType: TreeNodeType = dossierFolderIsFascicle ? TreeNodeType.Fascicle : TreeNodeType.DossierFolder;
            let nodeValue: string = dossierFolderIsFascicle ? dossierFolder.idFascicle : dossierFolder.UniqueId;

            let dossierFolderImageUrl: string = this._dossierFolderStatusImageDictionary[DossierFolderStatus[dossierFolder.Status]];
            let dossierFolderTooltip: string = this._dossierFolderStatusTooltipDictionary[DossierFolderStatus[dossierFolder.Status]];
            let dossierFolderExpandedImageUrl: string = this._expandedFolderImageDictionary[DossierFolderStatus[dossierFolder.Status]];

            let currentDossierFolderTreeNode: Telerik.Web.UI.RadTreeNode
                = this._createTreeNode(nodeType, dossierFolder.Name, nodeValue, dossierFolderImageUrl, parentNode, dossierFolderTooltip, dossierFolderExpandedImageUrl);

            if (dossierFolderIsFascicle) {
                currentDossierFolderTreeNode.get_attributes().setAttribute(ProcessesTreeView.DOSSIERFOLDERID_ATTRNAME, dossierFolder.UniqueId);
            }

            this._appendEmptyNode(currentDossierFolderTreeNode);
        });
    }

    private _appendDossierFolderNodesToParent(dossierFolderModels: DossierFolderModel[], parentNode: Telerik.Web.UI.RadTreeNode): void {
        dossierFolderModels.forEach((dossierFolder: DossierFolderModel) => {
            let dossierFolderIsFascicle: boolean = !!dossierFolder.Fascicle;

            let nodeType: TreeNodeType = dossierFolderIsFascicle ? TreeNodeType.Fascicle : TreeNodeType.DossierFolder;
            let nodeValue: string = dossierFolderIsFascicle ? dossierFolder.Fascicle.UniqueId : dossierFolder.UniqueId;

            let dossierFolderImageUrl: string = this._dossierFolderStatusImageDictionary[DossierFolderStatus[dossierFolder.Status]];
            let dossierFolderTooltip: string = this._dossierFolderStatusTooltipDictionary[DossierFolderStatus[dossierFolder.Status]];
            let dossierFolderExpandedImageUrl: string = this._expandedFolderImageDictionary[DossierFolderStatus[dossierFolder.Status]];

            let currentDossierFolderTreeNode: Telerik.Web.UI.RadTreeNode
                = this._createTreeNode(nodeType, dossierFolder.Name, nodeValue, dossierFolderImageUrl, parentNode, dossierFolderTooltip, dossierFolderExpandedImageUrl);

            if (dossierFolderIsFascicle) {
                currentDossierFolderTreeNode.get_attributes().setAttribute(ProcessesTreeView.DOSSIERFOLDERID_ATTRNAME, dossierFolder.UniqueId);
            }

            this._appendEmptyNode(currentDossierFolderTreeNode);
        });
    }

    private _appendCategoryNodeChildren(subCategoryNodes: Telerik.Web.UI.RadTreeNode[], parentCategoryNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        const categoryProcessesToLoad: number = this.treeViewNodesPageSize - subCategoryNodes.length;
        const currentParentNodeCollection: Telerik.Web.UI.RadTreeNodeCollection = parentCategoryNode.get_nodes();

        if (!categoryProcessesToLoad) {
            this._appendNodesToCollection(subCategoryNodes, currentParentNodeCollection);
            defferedRequest.resolve();
            return;
        }

        const alreadyLoadedCategoryProcessesCount: number = currentParentNodeCollection.toArray()
            .filter(node => node.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME) === TreeNodeType.Process)?.length;
        const categoryProcessesPaginationModel: PaginationModel = new PaginationModel(alreadyLoadedCategoryProcessesCount, categoryProcessesToLoad);
        this._loadCategoryProcesses(parentCategoryNode, categoryProcessesPaginationModel)
            .then((categoryProcesses: Telerik.Web.UI.RadTreeNode[]) => {
                const categoryFasciclesToLoad: number = categoryProcessesToLoad - categoryProcesses.length;

                if (!categoryFasciclesToLoad) {
                    this._appendNodesToCollection(subCategoryNodes, currentParentNodeCollection);
                    this._appendNodesToCollection(categoryProcesses, currentParentNodeCollection);
                    defferedRequest.resolve();
                    return;
                }

                const alreadyLoadedCategoryFasciclesCount: number = currentParentNodeCollection.toArray()
                    .filter(node => node.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME) === TreeNodeType.Fascicle)?.length;
                const categoryFasciclesPaginationModel: PaginationModel = new PaginationModel(alreadyLoadedCategoryFasciclesCount, categoryFasciclesToLoad);
                this._loadCategoryFascicles(parentCategoryNode, categoryFasciclesPaginationModel)
                    .then((categoryFascicles: Telerik.Web.UI.RadTreeNode[]) => {
                        this._appendNodesToCollection(subCategoryNodes, currentParentNodeCollection);
                        this._appendNodesToCollection(categoryProcesses, currentParentNodeCollection);
                        this._appendNodesToCollection(categoryFascicles, currentParentNodeCollection);
                        defferedRequest.resolve();
                    });
            });

        return defferedRequest.promise();
    }

    private _appendFascicleFolderNodeChildren(fascicleFolders: Telerik.Web.UI.RadTreeNode[], parentFascicleFolder: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        const fascicleFolderDocumentUnitsToLoad: number = this.treeViewNodesPageSize - fascicleFolders.length;
        const currentParentNodeCollection: Telerik.Web.UI.RadTreeNodeCollection = parentFascicleFolder.get_nodes();

        if (!fascicleFolderDocumentUnitsToLoad) {
            this._appendNodesToCollection(fascicleFolders, currentParentNodeCollection);
            defferedRequest.resolve();
            return;
        }

        const alreadyLoadedFascicleFoldersCount: number = currentParentNodeCollection.toArray()
            .filter(node => node.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME) === TreeNodeType.DocumentUnit)?.length;
        const fascicleFolderDocUnitsPaginationModel: PaginationModel = new PaginationModel(alreadyLoadedFascicleFoldersCount, fascicleFolderDocumentUnitsToLoad);
        this._loadFascicleFolderDocumentUnits(parentFascicleFolder, parentFascicleFolder.get_value(), TreeNodeType.DocumentUnit, fascicleFolderDocUnitsPaginationModel)
            .then((folderDocUnits: Telerik.Web.UI.RadTreeNode[]) => {
                this._appendNodesToCollection(fascicleFolders, currentParentNodeCollection);
                this._appendNodesToCollection(folderDocUnits, currentParentNodeCollection);

                defferedRequest.resolve();
            });

        return defferedRequest.promise();
    }

    private _createLoadMoreNode(): Telerik.Web.UI.RadTreeNode {
        let loadMoreNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(TreeNodeType.LoadMore, ProcessesTreeView.LOADMORE_NODE_LABEL, null, ProcessesTreeView.LOAD_MORE_NODE_IMAGEURL);
        this._appendEmptyNode(loadMoreNode);

        return loadMoreNode;
    }

    private _addDocumentUnitNodeAttributes(docUnitNode: Telerik.Web.UI.RadTreeNode, docUnitModel: DocumentUnitModel): void {
        docUnitNode.get_attributes().setAttribute(ProcessesTreeView.DOCUNITTYPE_ATTRNAME, docUnitModel.Environment);
        docUnitNode.get_attributes().setAttribute(ProcessesTreeView.YEAR_ATTRNAME, docUnitModel.Year);
        docUnitNode.get_attributes().setAttribute(ProcessesTreeView.NUMBER_ATTRNAME, docUnitModel.Number);
        docUnitNode.get_attributes().setAttribute(ProcessesTreeView.ENTITYID_ATTRNAME, docUnitModel.EntityId);
        if (docUnitModel.UDSRepository) {
            docUnitNode.get_attributes().setAttribute(ProcessesTreeView.UDSREPOID_ATTRNAME, docUnitModel.UDSRepository.UniqueId);
        }
    }

    private _appendNodesToCollection(children: Telerik.Web.UI.RadTreeNode[], nodesCollection: Telerik.Web.UI.RadTreeNodeCollection): void {
        children.forEach((childNode: Telerik.Web.UI.RadTreeNode) => nodesCollection.add(childNode));
    }

    private _createDocumentUnitNode(documentUnitModel: DocumentUnitModel, nodeType: TreeNodeType): Telerik.Web.UI.RadTreeNode {
        let docUnitEnv: Environment = documentUnitModel.Environment < 100 ? <Environment>documentUnitModel.Environment : Environment.UDS;

        let docUnitImageUrl: string = this._environmentImageDictionary[docUnitEnv] || ProcessesTreeView.DEFAULT_DOCUMENT_IMAGEURL;
        let docUnitDescription: string = `${documentUnitModel.Title} - ${documentUnitModel.Subject}`;
        let docUnitTooltip: string = documentUnitModel.DocumentUnitName;

        let docUnitTreeNode: Telerik.Web.UI.RadTreeNode
            = this._createTreeNode(nodeType, docUnitDescription, documentUnitModel.UniqueId, docUnitImageUrl, undefined, docUnitTooltip);
        this._addDocumentUnitNodeAttributes(docUnitTreeNode, documentUnitModel);

        return docUnitTreeNode;
    }

    private _showNotificationException(exception: ExceptionDTO, customMessage?: string) {
        if (!jQuery.isEmptyObject(this._uscNotification)) {
            if (exception) {
                this._uscNotification.showNotification(exception);
                return;
            }
            this._uscNotification.showWarningMessage(customMessage);
        }
    }

    private _showWarningMessage(customMessage: string) {
        if (!jQuery.isEmptyObject(this._uscNotification)) {
            this._uscNotification.showWarningMessage(customMessage);
        }
    }

    private _createEnvironmentDetailsPageUrl(clickedDocUnitNode: Telerik.Web.UI.RadTreeNode): string {
        let docUnitEnvironmentNumber: number = clickedDocUnitNode.get_attributes().getAttribute(ProcessesTreeView.DOCUNITTYPE_ATTRNAME);
        let docUnitEnv: Environment = docUnitEnvironmentNumber < 100 ? <Environment>docUnitEnvironmentNumber : Environment.UDS;

        let docUnitEntityId: number = clickedDocUnitNode.get_attributes().getAttribute(ProcessesTreeView.ENTITYID_ATTRNAME);
        let docUnitUniqueId: string = clickedDocUnitNode.get_value();
        let detailsPageUrl: string;

        switch (docUnitEnv) {
            case Environment.Protocol: {
                detailsPageUrl = `../Prot/ProtVisualizza.aspx?Type=Prot&UniqueId=${docUnitUniqueId}`;
                break;
            }
            case Environment.UDS: {
                let idUdsRepository: string = clickedDocUnitNode.get_attributes().getAttribute(ProcessesTreeView.UDSREPOID_ATTRNAME);
                detailsPageUrl = `../UDS/UDSView.aspx?Type=UDS&IdUDS=${docUnitUniqueId}&IdUDSRepository=${idUdsRepository}`;
                break;
            }
            case Environment.Resolution: {
                detailsPageUrl = `../Resl/ReslVisualizza.aspx?Type=Resl&IdResolution=${docUnitEntityId}`;
                break;
            }
            case Environment.DocumentSeries: {
                detailsPageUrl = `../Series/Item.aspx?IdDocumentSeriesItem=${docUnitEntityId}&Action=View&Type=Series`;
                break;
            }
        }

        return detailsPageUrl;
    }
    /// endregion [ HELPER METHODS ]
}

export = ProcessesTreeView;