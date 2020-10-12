import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import FascicleService = require("App/Services/Fascicles/FascicleService");
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ProcessService = require("App/Services/Processes/ProcessService");
import ProcessModel = require("App/Models/Processes/ProcessModel");
import TreeNodeType = require("App/Models/Commons/TreeNodeType");
import DossierFolderService = require("App/Services/Dossiers/DossierFolderService");
import DossierSummaryFolderViewModelMapper = require("App/Mappers/Dossiers/DossierSummaryFolderViewModelMapper");
import DossierSummaryFolderViewModel = require("App/ViewModels/Dossiers/DossierSummaryFolderViewModel");
import DossierFolderStatus = require("App/Models/Dossiers/DossierFolderStatus");
import FascicleFolderTypology = require("App/Models/Fascicles/FascicleFolderTypology");
import DossierFolderModel = require("App/Models/Dossiers/DossierFolderModel");
import UpdateActionType = require("App/Models/UpdateActionType");
import DossierModel = require("App/Models/Dossiers/DossierModel");
import CategoryModel = require("App/Models/Commons/CategoryModel");
import FascBase = require("Fasc/FascBase");
import FascProcessMoveResponseModelDTO = require("App/DTOs/FascProcessMoveResponseModelDTO");

class FascProcessMove extends FascBase {
    public sourceFascicleId: string;
    public lblFascNameId: string;
    public processesTreeViewId: string;
    public uscNotificationId: string;
    public moveFascicleConfirmBtnId: string;
    public pnlMainContentId: string;
    public ajaxLoadingPanelId: string;
    public fascicleParentFolderId: string;
    public radWindowManagerId: string;

    private _processesTreeView: Telerik.Web.UI.RadTreeView;
    private _moveFascicleConfirmBtn: Telerik.Web.UI.RadButton;
    private _dossierFolderService: DossierFolderService;
    private _processService: ProcessService;
    private _fascicleService: FascicleService;

    private _dossierFolderStatusImageDictionary;
    private _dossierFolderStatusTooltipDictionary;
    private _expandedFolderImageDictionary;
    private _nodeTypeExpandingActionsDictionary;
    private _sourceDossierFolderModel: DossierFolderModel;
    private _sourceFascicleModel: FascicleModel;

    private static ROOTNODE_IMGURL: string = "../Comm/images/folderopen16.gif";
    private static PROCESS_IMGURL: string = "../App_Themes/DocSuite2008/imgset16/process.png";
    private static EXPANDEDFOLDER_IMGURL: string = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
    private static NODETYPE_ATTRNAME: string = "NodeType";

    private _fascicleNameLabel: HTMLLabelElement;
    private _moveFascicleResponseModel: FascProcessMoveResponseModelDTO = {} as FascProcessMoveResponseModelDTO;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _windowManager: Telerik.Web.UI.RadWindowManager;

    private _treeRootNode(): Telerik.Web.UI.RadTreeNode {
        return this._processesTreeView.get_nodes().getNode(0);
    }

    private _sourceFascicleParentIsFolder(): boolean {
        return !!this.fascicleParentFolderId;
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME));

        let fascicleServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME);
        this._fascicleService = new FascicleService(fascicleServiceConfiguration);

        let processConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Process");
        this._processService = new ProcessService(processConfiguration);

        let dossierFolderConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DossierFolder");
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
    }

    public initialize(): void {
        this._initializeControls();
        this._initializeDictionaries();

        if (this._sourceFascicleParentIsFolder()) {
            this._initializeSourceDossierFolder();
        } else {
            this._initializeSourceFascicle();
        }
    }

    private _initializeSourceFascicle(): void {
        this._fascicleService.getFascicle(this.sourceFascicleId, (fascicleModel: FascicleModel) => {
            this._sourceFascicleModel = fascicleModel;

            let fascicleLabelText: string = `${fascicleModel.Title}-${fascicleModel.FascicleObject}`;
            let rootNodeText: string = `${fascicleModel.Category.Code}.${fascicleModel.Category.Name}`;

            this._initializeFascNameLabelAndTreeView(fascicleLabelText, rootNodeText, fascicleModel.Category.EntityShortId);

            this._loadCategoryProcesses(fascicleModel.Category.EntityShortId);
        }, (exception: ExceptionDTO) => this.showNotificationException(this.uscNotificationId, exception));
    }

    private _initializeSourceDossierFolder(): void {
        this._dossierFolderService.getFullDossierFolder(this.sourceFascicleId, (dossierFolder: any) => {
            this._sourceDossierFolderModel = dossierFolder;

            let fascicleLabelText: string = `${dossierFolder.Fascicle.Title}-${dossierFolder.Fascicle.FascicleObject}`;
            let rootNodeText: string = `${dossierFolder.Category.Code}.${dossierFolder.Category.Name}`;

            this._initializeFascNameLabelAndTreeView(fascicleLabelText, rootNodeText, dossierFolder.Category.EntityShortId);

            this._loadCategoryProcesses(dossierFolder.Category.EntityShortId);
        });
    }

    private _loadCategoryProcesses(categoryId: number): JQueryPromise<void> {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        this._processService.getAvailableProcesses(null, true, categoryId, null, (categoryProcesses: ProcessModel[]) => {
            categoryProcesses.forEach((categoryProcess: ProcessModel) => {
                let currentProcessTreeNode: Telerik.Web.UI.RadTreeNode
                    = this._createTreeNode(TreeNodeType.Process, categoryProcess.Name, categoryProcess.UniqueId, FascProcessMove.PROCESS_IMGURL, this._treeRootNode());
                this._appendEmptyNode(currentProcessTreeNode);
            });
            this._treeRootNode().hideLoadingStatus();
            defferedRequest.resolve();
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
            this._renderDossierFolders(processDossierFoldersViewModels, parentNode);
            defferedRequest.resolve();
        }, (exception: ExceptionDTO) => defferedRequest.reject(exception));

        return defferedRequest.promise();
    }

    private _loadDossierFoldersChildren(parentId: string, parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();
        this._dossierFolderService.getChildren(parentId, 0, (dossierFolders: DossierSummaryFolderViewModel[]) => {
            let childrenFolders: DossierSummaryFolderViewModel[] = dossierFolders.filter(dossierFolder => !dossierFolder.idFascicle);

            if (!childrenFolders.length) {
                this._appendEmptyNode(parentNode);
                defferedRequest.resolve();
                return;
            }

            this._renderDossierFolders(dossierFolders, parentNode);
            defferedRequest.resolve();
        }, (exception: ExceptionDTO) => defferedRequest.reject(exception));

        return defferedRequest.promise();
    }

    private _moveFascicleToDossierFolder = (): void => {
        let selectedDossierFolderNode: Telerik.Web.UI.RadTreeNode = this._processesTreeView.get_selectedNode();

        if (selectedDossierFolderNode.get_value() === this.fascicleParentFolderId) {
            this.showWarningMessage(this.uscNotificationId, "Selezionare un volume di destinazione differente dal volume di origine");
            return;
        }

        this._windowManager.radconfirm(`Sei sicuro di voler spostare il fascicolo selezionato nel volume ${selectedDossierFolderNode.get_text()}?`, (arg) => {

            if (!arg) {
                return;
            }

            this._loadingPanel.show(this.pnlMainContentId);
            this._dossierFolderService.getDossierFolderById(selectedDossierFolderNode.get_value(), (res: DossierFolderModel[]) => {
                let targetDossierFolder: DossierFolderModel = res[0];

                let dossierFolderToInsert: DossierFolderModel = this.fascicleParentFolderId
                    ? this._createSourceDossierFolderCopy(targetDossierFolder)
                    : this._createNewDossierFolder(targetDossierFolder);

                this._moveFascicleResponseModel.newDossierFolderParentId = targetDossierFolder.UniqueId;
                this._moveFascicleResponseModel.categoryId = this.fascicleParentFolderId
                    ? this._sourceDossierFolderModel.Category.EntityShortId
                    : this._sourceFascicleModel.Category.EntityShortId;

                this._dossierFolderService.insertDossierFolder(dossierFolderToInsert, null, (data: any) => {

                    if (!this.fascicleParentFolderId) {
                        this.closeWindow(this._moveFascicleResponseModel);
                    } else {
                        let sourceDossierFolder: DossierFolderModel = {} as DossierFolderModel;
                        sourceDossierFolder.Name = this._sourceDossierFolderModel.Name;
                        sourceDossierFolder.UniqueId = this.sourceFascicleId;

                        this._deleteDossierFolderFascicle(sourceDossierFolder);
                    }
                }, (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.pnlMainContentId);
                    this.showNotificationException(this.uscNotificationId, exception);
                });

            }, (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pnlMainContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            });
        });
    }

    private _deleteDossierFolderFascicle(dossierFolder: DossierFolderModel): void {
        this._dossierFolderService.updateDossierFolder(dossierFolder, UpdateActionType.RemoveFascicleFromDossierFolder, (data: any) => {
            dossierFolder.ParentInsertId = this.fascicleParentFolderId;
            this._deleteDossierFolder(dossierFolder);
        }, (exception: ExceptionDTO) => {
            this._loadingPanel.hide(this.pnlMainContentId);
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }

    private _deleteDossierFolder(dossierFolder: DossierFolderModel): void {
        this._dossierFolderService.deleteDossierFolder(dossierFolder, (data: any) => {
            this._loadingPanel.hide(this.pnlMainContentId);
            this.closeWindow(this._moveFascicleResponseModel);
        }, (exception: ExceptionDTO) => {
            this._loadingPanel.hide(this.pnlMainContentId);
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }

    private _initializeControls(): void {
        this._windowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._processesTreeView = <Telerik.Web.UI.RadTreeView>$find(this.processesTreeViewId);
        this._processesTreeView.add_nodeExpanding(this._treeView_LoadNodeChildrenOnExpand);
        this._processesTreeView.add_nodeCollapsed(this._treeView_ClearSelectionOnCollapse);
        this._processesTreeView.add_nodeClicked(this._treeView_SetConfirmBtnStateOnNodeClick);
        this._fascicleNameLabel = <HTMLLabelElement>document.getElementById(this.lblFascNameId);
        this._moveFascicleConfirmBtn = <Telerik.Web.UI.RadButton>$find(this.moveFascicleConfirmBtnId);
        this._moveFascicleConfirmBtn.add_clicked(this._moveFascicleToDossierFolder);
        this._moveFascicleConfirmBtn.set_enabled(false);
    }

    private _initializeFascNameLabelAndTreeView(fascicleLabelValue: string, categoryName: string, categoryId: number): void {
        this._fascicleNameLabel.innerText = fascicleLabelValue;
        this._addTreeRootNode(categoryName, categoryId);
    }

    private _initializeDictionaries(): void {
        /// Nodes expanding event handlers registration
        this._nodeTypeExpandingActionsDictionary = {};
        this._registerNodeTypesExpandingActions();

        /// Dossier folders images and tooltips
        this._dossierFolderStatusImageDictionary = {};
        this._dossierFolderStatusTooltipDictionary = {};
        this._registerDossierFoldersStatusImages();
        this._registerDossierFoldersStatusTooltips();

        /// Expanded folders images
        this._expandedFolderImageDictionary = {};
        this._registerExpandedFoldersImages();
    }

    private _registerNodeTypesExpandingActions(): void {
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.Root] = (expandedNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
            let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            this._loadCategoryProcesses(expandedNode.get_value())
                .done(() => defferedRequest.resolve())
                .fail((exception: ExceptionDTO) => defferedRequest.reject(exception));

            return defferedRequest.promise();
        }
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.Process] = (parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
            let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            this._loadProcessDossierFolders(parentNode.get_value(), parentNode)
                .done(() => defferedRequest.resolve())
                .fail((exception: ExceptionDTO) => defferedRequest.reject(exception));

            return defferedRequest.promise();
        };
        this._nodeTypeExpandingActionsDictionary[TreeNodeType.DossierFolder] = (parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
            let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

            this._loadDossierFoldersChildren(parentNode.get_value(), parentNode)
                .done(() => defferedRequest.resolve())
                .fail((exception: ExceptionDTO) => defferedRequest.reject(exception));

            return defferedRequest.promise();
        };
    }

    private _treeView_ClearSelectionOnCollapse = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs): void => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._processesTreeView.get_selectedNode();

        if (!selectedNode) {
            return;
        }

        selectedNode.set_selected(false);
        this._moveFascicleConfirmBtn.set_enabled(false);
    }

    private _treeView_SetConfirmBtnStateOnNodeClick = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs): void => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = args.get_node();
        let nodeType: TreeNodeType = selectedNode.get_attributes().getAttribute(FascProcessMove.NODETYPE_ATTRNAME);
        let confirmBtnEnableState: boolean = (nodeType === TreeNodeType.DossierFolder) && (selectedNode.get_value() !== this.fascicleParentFolderId);

        this._moveFascicleConfirmBtn.set_enabled(confirmBtnEnableState);
    }

    private _treeView_LoadNodeChildrenOnExpand = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs): void => {
        let expandedNode: Telerik.Web.UI.RadTreeNode = args.get_node();
        expandedNode.get_nodes().clear();

        let nodeType: TreeNodeType = expandedNode.get_attributes().getAttribute(FascProcessMove.NODETYPE_ATTRNAME);
        let expandedNodeAction: (parentNode: Telerik.Web.UI.RadTreeNode) => JQueryPromise<void> = this._nodeTypeExpandingActionsDictionary[nodeType];

        if (expandedNodeAction) {
            expandedNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            expandedNodeAction(expandedNode)
                .done(() => {
                    expandedNode.hideLoadingStatus();
                    expandedNode.set_expanded(true);
                })
                .fail((exception: ExceptionDTO) => {
                    this.showNotificationException(this.uscNotificationId, exception);
                    expandedNode.hideLoadingStatus();
                });
        }
    }

    private _appendEmptyNode(treeNode: Telerik.Web.UI.RadTreeNode): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text("Nessun elemento trovato");
        treeNode.get_nodes().add(emptyNode);
    }

    private _createSourceDossierFolderCopy = (targetDossierFolder: DossierFolderModel): DossierFolderModel => {
        let dossierFolderCopy: DossierFolderModel = <DossierFolderModel>{};
        dossierFolderCopy.ParentInsertId = targetDossierFolder.UniqueId;

        dossierFolderCopy.Category = <CategoryModel>{};
        dossierFolderCopy.Category.EntityShortId = this._sourceDossierFolderModel.Category.EntityShortId;

        dossierFolderCopy.Dossier = <DossierModel>{};
        dossierFolderCopy.Dossier.UniqueId = targetDossierFolder.Dossier.UniqueId;

        dossierFolderCopy.Fascicle = <FascicleModel>{};
        dossierFolderCopy.Fascicle.UniqueId = this._sourceDossierFolderModel.Fascicle.UniqueId;

        dossierFolderCopy.JsonMetadata = this._sourceDossierFolderModel.JsonMetadata;
        dossierFolderCopy.DossierFolders = this._sourceDossierFolderModel.DossierFolders;
        dossierFolderCopy.FascicleTemplates = this._sourceDossierFolderModel.FascicleTemplates;
        dossierFolderCopy.DossierFolderRoles = this._sourceDossierFolderModel.DossierFolderRoles;

        return dossierFolderCopy;
    }

    private _createNewDossierFolder = (targetDossierFolder: DossierFolderModel): DossierFolderModel => {
        let newDossierFolder: DossierFolderModel = <DossierFolderModel>{};
        newDossierFolder.ParentInsertId = targetDossierFolder.UniqueId;

        newDossierFolder.Category = <CategoryModel>{};
        newDossierFolder.Category.EntityShortId = this._sourceFascicleModel.Category.EntityShortId;

        newDossierFolder.Dossier = <DossierModel>{};
        newDossierFolder.Dossier.UniqueId = targetDossierFolder.Dossier.UniqueId;

        newDossierFolder.Fascicle = <FascicleModel>{};
        newDossierFolder.Fascicle.UniqueId = this._sourceFascicleModel.UniqueId;

        return newDossierFolder;
    }

    private _addTreeRootNode(rootNodeText: string, rootNodeValue: number): void {
        let rootNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        rootNode.set_text(rootNodeText);
        rootNode.set_value(rootNodeValue);
        rootNode.set_imageUrl(FascProcessMove.ROOTNODE_IMGURL);
        rootNode.get_attributes().setAttribute(FascProcessMove.NODETYPE_ATTRNAME, TreeNodeType.Root);
        rootNode.set_expanded(true);
        this._processesTreeView.get_nodes().add(rootNode);
    }

    private _renderDossierFolders(dossierFolders: DossierSummaryFolderViewModel[], parentNode: Telerik.Web.UI.RadTreeNode): void {
        dossierFolders.forEach((dossierFolder: DossierSummaryFolderViewModel) => {
            let dossierFolderIsFascicle: boolean = !!dossierFolder.idFascicle;

            if (!dossierFolderIsFascicle) {
                let nodeValue: string = dossierFolderIsFascicle ? dossierFolder.idFascicle : dossierFolder.UniqueId;

                let dossierFolderImageUrl: string = this._dossierFolderStatusImageDictionary[DossierFolderStatus[dossierFolder.Status]];
                let dossierFolderTooltip: string = this._dossierFolderStatusTooltipDictionary[DossierFolderStatus[dossierFolder.Status]];
                let dossierFolderExpandedImageUrl: string = FascProcessMove.EXPANDEDFOLDER_IMGURL;

                let currentDossierFolderTreeNode: Telerik.Web.UI.RadTreeNode
                    = this._createTreeNode(TreeNodeType.DossierFolder, dossierFolder.Name, nodeValue, dossierFolderImageUrl, parentNode, dossierFolderTooltip, dossierFolderExpandedImageUrl);

                if (dossierFolder.UniqueId === this.fascicleParentFolderId) {
                    currentDossierFolderTreeNode.disable();
                }

                this._appendEmptyNode(currentDossierFolderTreeNode);
            }
        });
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
    private _registerExpandedFoldersImages(): void {
        this._expandedFolderImageDictionary[DossierFolderStatus.Folder] = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
        this._expandedFolderImageDictionary[FascicleFolderTypology.SubFascicle] = "../App_Themes/DocSuite2008/imgset16/folder_internet_open.png";
    }

    private closeWindow(fascMoveResponseModel: FascProcessMoveResponseModelDTO): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(fascMoveResponseModel);
    }

    private getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
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
        treeNode.get_attributes().setAttribute(FascProcessMove.NODETYPE_ATTRNAME, nodeType);

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
}

export = FascProcessMove;