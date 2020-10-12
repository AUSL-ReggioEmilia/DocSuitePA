/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/core/extensions/number.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import CategoryService = require("App/Services/Commons/CategoryService");
import CategoryTreeViewModel = require("App/ViewModels/Commons/CategoryTreeViewModel");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import CategorySearchFilterDTO = require("App/DTOs/CategorySearchFilterDTO");
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
import DocumentUnitModelMapper = require("App/Mappers/DocumentUnits/DocumentUnitModelMapper");
import UscErrorNotification = require("UserControl/uscErrorNotification");
import TreeNodeType = require("App/Models/Commons/TreeNodeType");
import FascicleService = require("App/Services/Fascicles/FascicleService");
import ODATAResponseModel = require("App/Models/ODATAResponseModel");
import FascProcessMoveResponseModelDTO = require("App/DTOs/FascProcessMoveResponseModelDTO");

class ProcessesTreeView {
    public categoryTreeViewId: string;
    public detailsPaneId: string;
    public ajaxLoadingPanelId: string;
    public uscNotificationId: string;
    public showOnlyMyProcesses: string;
    public actionToolbarId: string;
    currentTenantAOOId: string;
    public windowManagerId: string;
    public windowMoveFascId: string;
    public splitterMainId: string;

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
    private static DEFAULT_DOCUMENT_IMAGEURL: string = "../App_Themes/DocSuite2008/imgset16/document.png";

    private _categoryTreeViewRootNode(): Telerik.Web.UI.RadTreeNode {
        return this._categoryTreeView.get_nodes().getNode(0);
    }

    private toolbarActions(): Array<[string, () => void]> {
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
        let expandedNodeAction: (parentNode: Telerik.Web.UI.RadTreeNode) => JQueryPromise<void> = this._nodeTypeExpandingActionsDictionary[nodeType];

        if (expandedNodeAction) {
            expandedNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            expandedNodeAction(expandedNode)
                .done(() => {
                    expandedNode.hideLoadingStatus();
                    expandedNode.set_expanded(true);
                })
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

    /// region [ LOAD TREE NODE ITEMS METHODS]
    private _initializeCategoryTreeView(): void {

        if (this._categoryTreeView.get_nodes().get_count() > 0)
            this._categoryTreeView.get_nodes().clear();

        let rootNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        rootNode.set_text(ProcessesTreeView.ROOTNODE_DESCRIPTION);
        rootNode.get_attributes().setAttribute(ProcessesTreeView.NODETYPE_ATTRNAME, TreeNodeType.Root);
        this._categoryTreeView.get_nodes().add(rootNode);

        rootNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        this._loadCategories(TreeNodeType.Category, this._categoryTreeViewRootNode())
            .done(() => {
                rootNode.hideLoadingStatus();
                rootNode.set_expanded(true);
            });
    }

    private _loadCategories(nodeType: TreeNodeType, parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<Telerik.Web.UI.RadTreeNode[]> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();
        let categoryFinderModel: CategorySearchFilterDTO = {} as CategorySearchFilterDTO;
        categoryFinderModel.IdTenantAOO = this.currentTenantAOOId;
        if (!parentNode.get_value()) {
            categoryFinderModel.LoadRoot = true;
        }
        else {
            categoryFinderModel.ParentId = parentNode.get_value();
        }

        this._categoryService.findTreeCategories(categoryFinderModel,
            (categories: CategoryTreeViewModel[]) => {
                let parentNodeIsTreeRootNode: boolean = parentNode === this._categoryTreeViewRootNode();
                let categoryTreeNodeCollection: Telerik.Web.UI.RadTreeNode[] = categories.map((categoryModel: CategoryTreeViewModel) => {
                    let categoryNodeDescription: string = `${categoryModel.Code}.${categoryModel.Name}`;
                    let categoryNodeImageUrl: string = "../Comm/images/folderopen16.gif";

                    let currentCategoryTreeNode: Telerik.Web.UI.RadTreeNode
                        = this._createTreeNode(nodeType, categoryNodeDescription, categoryModel.IdCategory, categoryNodeImageUrl, parentNodeIsTreeRootNode ? parentNode : undefined);

                    this._appendEmptyNode(currentCategoryTreeNode);
                    return currentCategoryTreeNode;
                });

                defferedRequest.resolve(categoryTreeNodeCollection);
            }, (exception: ExceptionDTO) => defferedRequest.reject(exception));

        return defferedRequest.promise();
    }
    private _loadCategoryProcesses(nodeType: TreeNodeType, parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<Telerik.Web.UI.RadTreeNode[]> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();
        let categoryId: number = parentNode.get_value();
        this._processService.getAvailableProcesses(null, this._showOnlyMyProcesses(), parentNode.get_value(), null, (categoryProcesses: ProcessModel[]) => {
            let processTreeNodeCollection: Telerik.Web.UI.RadTreeNode[] = categoryProcesses.map((process: ProcessModel) => {
                let currentProcessTreeNode: Telerik.Web.UI.RadTreeNode
                    = this._createTreeNode(nodeType, process.Name, process.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png");
                currentProcessTreeNode.get_attributes().setAttribute(ProcessesTreeView.CATEGORYID_ATTRNAME, categoryId);
                this._appendEmptyNode(currentProcessTreeNode);
                return currentProcessTreeNode;
            });
            defferedRequest.resolve(processTreeNodeCollection);
        }, (exception: ExceptionDTO) => {
            defferedRequest.reject(exception);
        });

        return defferedRequest.promise();
    }
    private _loadCategoryFascicles(nodeType: TreeNodeType, parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<Telerik.Web.UI.RadTreeNode[]> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();
        this._fascicleService.getFascicleByCategory(parentNode.get_value(), '', false, (odataResponseModel: ODATAResponseModel<FascicleModel>) => {
            let categoryFascicles: FascicleModel[] = odataResponseModel.value;
            let fascicleTreeNodeCollection: Telerik.Web.UI.RadTreeNode[] = categoryFascicles.map((fasc: FascicleModel) => {
                let fascicleImageUrl: string = this._dossierFolderStatusImageDictionary[DossierFolderStatus.Fascicle];
                let fascicleTooltip: string = this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.Fascicle];
                let fascicleExpandedImageUrl: string = this._expandedFolderImageDictionary[DossierFolderStatus.Fascicle];
                let nodeDescription: string = `${fasc.Title}-${fasc.FascicleObject}`;

                let currentFascTreeNode: Telerik.Web.UI.RadTreeNode
                    = this._createTreeNode(nodeType, nodeDescription, fasc.UniqueId, fascicleImageUrl, undefined, fascicleTooltip, fascicleExpandedImageUrl);
                currentFascTreeNode.get_attributes().setAttribute(ProcessesTreeView.FASCISACTIVE_ATTRNAME, !fasc.EndDate);
                this._appendEmptyNode(currentFascTreeNode);
                return currentFascTreeNode;
            });
            defferedRequest.resolve(fascicleTreeNodeCollection);
        }, (exception: ExceptionDTO) => defferedRequest.reject(exception));

        return defferedRequest.promise();
    }
    private _loadProcessDossierFolders(processId: string, parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();
        this._dossierFolderService.getProcessFolders(null, processId, false, false, (processDossierFolders: any[]) => {
            if (!processDossierFolders.length) {
                this._appendEmptyNode(parentNode);
                defferedRequest.resolve();
                return;
            }

            let dossierSummaryFolderViewModelMapper: DossierSummaryFolderViewModelMapper = new DossierSummaryFolderViewModelMapper();
            let processDossierFoldersViewModels: DossierSummaryFolderViewModel[] = dossierSummaryFolderViewModelMapper.MapCollection(processDossierFolders);
            this._addDossierFolderNodesRecursive(processDossierFoldersViewModels, parentNode);
            defferedRequest.resolve();
        }, (exception: ExceptionDTO) => defferedRequest.reject(exception));

        return defferedRequest.promise();
    }
    private _loadDossierFoldersChildren(parentId: string, parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();
        this._dossierFolderService.getChildren(parentId, 0, (dossierFolders: DossierSummaryFolderViewModel[]) => {
            if (!dossierFolders.length) {
                this._appendEmptyNode(parentNode);
                defferedRequest.resolve();
                return;
            }

            dossierFolders.forEach((dossierFolder: DossierSummaryFolderViewModel) => {
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
            defferedRequest.resolve();
        }, (exception: ExceptionDTO) => defferedRequest.reject(exception));

        return defferedRequest.promise();
    }
    private _loadFascicleFolders(parentFascicleId: string, nodeType: TreeNodeType): JQueryPromise<Telerik.Web.UI.RadTreeNode[]> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();
        this._fascicleFolderService.getChildren(parentFascicleId, (fascicleFolders: FascicleSummaryFolderViewModel[]) => {
            if (!fascicleFolders.length) {
                defferedRequest.resolve([]);
                return;
            }

            let fascFoldersTreeNodeCollection: Telerik.Web.UI.RadTreeNode[] = fascicleFolders.map((fascicleFolder: FascicleSummaryFolderViewModel) => {
                let fascicleFolderImageUrl: string = this._dossierFolderStatusImageDictionary[DossierFolderStatus.Folder];
                let fascicleFolderTooltip: string = this._fascicleFolderStatusTooltipDictionary[FascicleFolderStatus[fascicleFolder.Status]];
                let fascicleFolderExpandedImageUrl: string = this._expandedFolderImageDictionary[DossierFolderStatus.Folder];

                let currentFascicleFolderTreeNode: Telerik.Web.UI.RadTreeNode
                    = this._createTreeNode(nodeType, fascicleFolder.Name, fascicleFolder.UniqueId, fascicleFolderImageUrl, undefined, fascicleFolderTooltip, fascicleFolderExpandedImageUrl);
                currentFascicleFolderTreeNode.get_attributes().setAttribute(ProcessesTreeView.FASCICLEID_ATTRNAME, fascicleFolder.idFascicle);
                this._appendEmptyNode(currentFascicleFolderTreeNode);

                return currentFascicleFolderTreeNode;
            });
            defferedRequest.resolve(fascFoldersTreeNodeCollection);
        }, (exception: ExceptionDTO) => defferedRequest.reject(exception));

        return defferedRequest.promise();
    }
    private _loadFascicleFolderDocumentUnits(fascicleFolderFascicleId: string, fascicleFolderId: string, nodeType: TreeNodeType): JQueryPromise<Telerik.Web.UI.RadTreeNode[]> {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();
        let fascicleModel: FascicleModel = new FascicleModel();
        fascicleModel.UniqueId = fascicleFolderFascicleId;

        this._documentUnitService.getFascicleDocumentUnits(fascicleModel, null, this.currentTenantAOOId, fascicleFolderId,
            (fascicleFolderDocUnits: DocumentUnitModel[]) => {
                
                let docUnitMapper: DocumentUnitModelMapper = new DocumentUnitModelMapper();
                fascicleFolderDocUnits = docUnitMapper.MapCollection(fascicleFolderDocUnits);
                let fascFolderDocUnitTreeNodeCollection: Telerik.Web.UI.RadTreeNode[]
                    = fascicleFolderDocUnits.map((fascicleFolderDocUnit: DocumentUnitModel) => this._createDocumentUnitNode(fascicleFolderDocUnit, nodeType));

                defferedRequest.resolve(fascFolderDocUnitTreeNodeCollection);
            }, (exception: ExceptionDTO) => defferedRequest.reject(exception));

        return defferedRequest.promise();
    }
    /// endregion [ LOAD TREE NODE ITEMS METHODS]

    /// region [ ** HELPER METHODS ** ]
    private _registerNodeTypesExpandingActions(): void {
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.Root] = (expandedNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
            let categoryLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            this._loadCategories(TreeNodeType.Category, expandedNode)
                .done(() => categoryLoadingDefferedRequest.resolve())
                .fail((exception: ExceptionDTO) => categoryLoadingDefferedRequest.reject(exception));

            return categoryLoadingDefferedRequest.promise();
        }
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.Category] = (expandedNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
            let categoryChildrenLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            let childrenLoadingPromises: JQueryPromise<Telerik.Web.UI.RadTreeNode[]>[] =
                [
                    this._loadCategories(TreeNodeType.Category, expandedNode),
                    this._loadCategoryProcesses(TreeNodeType.Process, expandedNode),
                    this._loadCategoryFascicles(TreeNodeType.Fascicle, expandedNode)
                ];

            $.when(...childrenLoadingPromises)
                .done((...categoryChildrenNodeGroups) => {
                    let [subCategories, processes, fascicles] = categoryChildrenNodeGroups;
                    
                    if (subCategories.length === 0 && processes.length === 0 && fascicles.length === 0) {
                        this._appendEmptyNode(expandedNode);
                        categoryChildrenLoadingDefferedRequest.resolve();
                        return;
                    }

                    let currentParentNodeCollection: Telerik.Web.UI.RadTreeNodeCollection = expandedNode.get_nodes();
                    categoryChildrenNodeGroups.forEach((childrenGroup: Telerik.Web.UI.RadTreeNode[]) => {
                        childrenGroup.forEach((node: Telerik.Web.UI.RadTreeNode) => currentParentNodeCollection.add(node));
                    });
                    categoryChildrenLoadingDefferedRequest.resolve();
                })
                .fail((exception: ExceptionDTO) => categoryChildrenLoadingDefferedRequest.reject(exception));

            return categoryChildrenLoadingDefferedRequest.promise();
        }
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.Process] = (parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
            let processDossierFoldersLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            this._loadProcessDossierFolders(parentNode.get_value(), parentNode)
                .done(() => processDossierFoldersLoadingDefferedRequest.resolve())
                .fail((exception: ExceptionDTO) => processDossierFoldersLoadingDefferedRequest.reject(exception));

            return processDossierFoldersLoadingDefferedRequest.promise();
        };
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.DossierFolder] = (parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
            let dossierFolderChildrenLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            this._loadDossierFoldersChildren(parentNode.get_value(), parentNode)
                .done(() => dossierFolderChildrenLoadingDefferedRequest.resolve())
                .fail((exception: ExceptionDTO) => dossierFolderChildrenLoadingDefferedRequest.reject(exception));

            return dossierFolderChildrenLoadingDefferedRequest.promise();
        };
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.Fascicle] = (parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
            let fascicleFoldersLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            let fascicleId: string = parentNode.get_value();
            this._loadFascicleFolders(fascicleId, TreeNodeType.FascicleFolder)
                .done((fascFolders: Telerik.Web.UI.RadTreeNode[]) => {
                    if (fascFolders.length === 0) {
                        this._appendEmptyNode(parentNode);
                        fascicleFoldersLoadingDefferedRequest.resolve();
                        return;
                    }

                    fascFolders.forEach((fascFolderNode: Telerik.Web.UI.RadTreeNode) => {
                        parentNode.get_nodes().add(fascFolderNode);
                    });
                    fascicleFoldersLoadingDefferedRequest.resolve();
                })
                .fail((exception: ExceptionDTO) => fascicleFoldersLoadingDefferedRequest.reject(exception));

            return fascicleFoldersLoadingDefferedRequest.promise();
        };
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.FascicleFolder] = (parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
            let fascFolderChildrenLoadingDefferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            let parentFascicleFolderId: string = parentNode.get_value();
            let parentFascileFolderFascicleId: string = parentNode.get_attributes().getAttribute(ProcessesTreeView.FASCICLEID_ATTRNAME);

            let childrenLoadingPromises: JQueryPromise<Telerik.Web.UI.RadTreeNode[]>[] =
                [
                    this._loadFascicleFolders(parentFascicleFolderId, TreeNodeType.FascicleFolder),
                    this._loadFascicleFolderDocumentUnits(parentFascileFolderFascicleId, parentFascicleFolderId, TreeNodeType.DocumentUnit)
                ];

            $.when(...childrenLoadingPromises)
                .done((...fascFolderChildrenNodeGroups) => {
                    let [subFolders, docUnits] = fascFolderChildrenNodeGroups;

                    if (subFolders.length === 0 && docUnits.length === 0) {
                        this._appendEmptyNode(parentNode);
                        fascFolderChildrenLoadingDefferedRequest.resolve();
                        return;
                    }

                    let currentParentNodeCollection: Telerik.Web.UI.RadTreeNodeCollection = parentNode.get_nodes();
                    fascFolderChildrenNodeGroups.forEach((childrenGroup: Telerik.Web.UI.RadTreeNode[]) => {
                        childrenGroup.forEach((node: Telerik.Web.UI.RadTreeNode) => currentParentNodeCollection.add(node));
                    });
                    fascFolderChildrenLoadingDefferedRequest.resolve();
                })
                .fail((exception: ExceptionDTO) => fascFolderChildrenLoadingDefferedRequest.reject(exception));

            return fascFolderChildrenLoadingDefferedRequest.promise();
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
        this._environmentImageDictionary[Environment.Protocol] = "../Comm/Images/DocSuite/Protocollo16.gif";
        this._environmentImageDictionary[Environment.Resolution] = "../Comm/Images/DocSuite/Atti16.gif";
        this._environmentImageDictionary[Environment.DocumentSeries] = "../App_Themes/DocSuite2008/imgset16/document_copies.png";
        this._environmentImageDictionary[Environment.UDS] = "../App_Themes/DocSuite2008/imgset16/document_copies.png";
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
        let currentAction: () => void = this.toolbarActions().filter((item: [string, () => void]) => item[0] == currentActionButtonItem.get_commandName())
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
            this._loadDossierFoldersChildren(newDossierFolderParent.get_value(), newDossierFolderParent)
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

    private _addDossierFolderNodesRecursive(dossierFolders: DossierSummaryFolderViewModel[], parentNode: Telerik.Web.UI.RadTreeNode) {
        dossierFolders.forEach((dossierFolder: DossierSummaryFolderViewModel) => {
            let dossierFolderImageUrl: string = this._dossierFolderStatusImageDictionary[DossierFolderStatus[dossierFolder.Status]];
            let dossierFolderExpandedImageUrl: string = this._expandedFolderImageDictionary[DossierFolderStatus.Folder];
            let dossierFolderTooltip: string = this._dossierFolderStatusTooltipDictionary[DossierFolderStatus[dossierFolder.Status]];
            let dossierFolderNodeType: TreeNodeType = dossierFolder.idFascicle ? TreeNodeType.Fascicle : TreeNodeType.DossierFolder;
            let dossierFolderNodeValue: string = dossierFolder.idFascicle ? dossierFolder.idFascicle : dossierFolder.UniqueId;

            let currentDossierFolderTreeNode: Telerik.Web.UI.RadTreeNode
                = this._createTreeNode(dossierFolderNodeType, dossierFolder.Name, dossierFolderNodeValue, dossierFolderImageUrl, parentNode, dossierFolderTooltip, dossierFolderExpandedImageUrl);

            if (dossierFolder.DossierFolders.length > 0) {
                this._addDossierFolderNodesRecursive(dossierFolder.DossierFolders, currentDossierFolderTreeNode);
            }
            else {
                this._appendEmptyNode(currentDossierFolderTreeNode);
            }
        });
    }

    private _appendEmptyNode(treeNode: Telerik.Web.UI.RadTreeNode): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text("Nessun elemento trovato");
        treeNode.get_nodes().add(emptyNode);
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