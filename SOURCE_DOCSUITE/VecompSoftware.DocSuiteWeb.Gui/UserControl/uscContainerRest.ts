import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import TenantViewModel = require("App/ViewModels/Tenants/TenantViewModel");
import UpdateActionType = require("App/Models/UpdateActionType");
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import PaginationModel = require('App/Models/Commons/PaginationModel');
import ContainerService = require('App/Services/Commons/ContainerService');
import TenantService = require('App/Services/Tenants/TenantService');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UscContainerRestEventType = require('App/Models/Commons/UscContainerRestEventType');
import TreeNodeType = require('App/Models/Commons/TreeNodeType');

class uscContainerRest {
    splitterMainId: string;
    uscNotificationId: string;

    rtvTenantsId: string;
    rtvContainersId: string;
    tbContainersControlId: string;
    rwContainerId: string;
    cmbContainerId: string;
    btnContainerSelectorOkId: string;
    btnContainerSelectorCancelId: string;
    txtSearchContainerNameId: string

    managerId: string;
    maxNumberElements: string;
    containers: ContainerModel[];
    selectedContainer: ContainerModel;
    ajaxLoadingPanelId: string;
    treeViewNodesPageSize: number;
    _totalContainerCount: number;

    private _instanceId: string;
    public pnlContentId: string;

    private _rtvContainers: Telerik.Web.UI.RadTreeView;
    private _toolbarContainer: Telerik.Web.UI.RadToolBar;
    private _rwContainer: Telerik.Web.UI.RadWindow;
    private _cmbContainer: Telerik.Web.UI.RadComboBox;
    private _btnContainerOk: Telerik.Web.UI.RadButton;
    private _btnContainerCancel: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _rtbContainersSearchName: Telerik.Web.UI.RadTextBox;

    public uscContainerRestEvents = UscContainerRestEventType;
    protected _tenantService: TenantService;
    protected _containerService: ContainerService;
    private _serviceConfiguration: ServiceConfiguration[];
    public static _currentSelectedTenant: TenantViewModel;
    private _uscNotification: UscErrorNotification;

    protected static TENANT_TYPE_NAME: string = "Tenant";
    protected static CONTAINER_TYPE_NAME: string = "Container";
    private static LOAD_MORE_NODE_IMAGEURL: string = "../App_Themes/DocSuite2008/imgset16/add.png";
    private static CONTAINER_NODE_IMAGE_URL: string = "../App_Themes/DocSuite2008/imgset16/box_open.png";
    private static LOADMORE_NODE_LABEL: string = "Carica più elementi";
    private static NODETYPE_ATTRNAME: string = "NodeType";
    private static ADD_COMMANDNAME: string = "ADDNEW";
    private static REMOVE_COMMANDNAME: string = "REMOVE";
    private static ADDALL_COMMANDNAME: string = "ADDALL";
    private static REMOVEALL_COMMANDNAME: string = "REMOVEALL";
    private static SEARCH_COMMANDNAME: string = "SEARCH";

    private _treeViewRootNode(): Telerik.Web.UI.RadTreeNode {
        return this._rtvContainers.get_nodes().getNode(0);
    }

    constructor(serviceConfigurations: ServiceConfiguration[], uscId: string) {
        this._serviceConfiguration = serviceConfigurations;
        let tenantConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfiguration, uscContainerRest.TENANT_TYPE_NAME);
        this._tenantService = new TenantService(tenantConfiguration);
        let containerConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfiguration, uscContainerRest.CONTAINER_TYPE_NAME);
        this._containerService = new ContainerService(containerConfiguration);

        uscContainerRest._currentSelectedTenant = new TenantViewModel();
    }

    initialize(): void {
        this._rtvContainers = <Telerik.Web.UI.RadTreeView>$find(this.rtvContainersId);
        this._toolbarContainer = <Telerik.Web.UI.RadToolBar>$find(this.tbContainersControlId);
        this._toolbarContainer.add_buttonClicking(this.toolbarContainer_onClick);
        this._rwContainer = <Telerik.Web.UI.RadWindow>$find(this.rwContainerId);
        this._rwContainer.add_show(this._rwContainer_OnShow);
        this._cmbContainer = <Telerik.Web.UI.RadComboBox>$find(this.cmbContainerId);
        this._cmbContainer.add_selectedIndexChanged(this.cmbContainers_onClick);
        this._cmbContainer.add_itemsRequested(this._cmbContainer_OnClientItemsRequested);
        this._btnContainerOk = <Telerik.Web.UI.RadButton>$find(this.btnContainerSelectorOkId);
        this._btnContainerOk.add_clicking(this.btnContainerOk_onClick);
        this._btnContainerCancel = <Telerik.Web.UI.RadButton>$find(this.btnContainerSelectorCancelId);
        this._btnContainerCancel.add_clicking(this.btnContainerCancel_onClick);
        this._uscNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
        this._rtbContainersSearchName = <Telerik.Web.UI.RadTextBox>this._toolbarContainer.findItemByValue("searchInput").findControl("txtSearch");

        this._loadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
        this._manager = $find(this.managerId) as Telerik.Web.UI.RadWindowManager;
        this._rtvContainers.add_nodeExpanded(this.treeView_LoadMoreOnExpand);
        this._rtvContainers.add_nodeClicked(this.treeView_LoadMoreOnClick);

        $(`#${this.pnlContentId}`).data(this);
    }

   

    private toolbarActions(): Array<[string, (sender, args) => void]> {
        let items: Array<[string, (args) => void]> = [
            [uscContainerRest.ADD_COMMANDNAME, (args) => this._btnAddContainer_OnClick(args)],
            [uscContainerRest.REMOVE_COMMANDNAME, (args) => this._btnRemoveContainer_OnClick(args)],
            [uscContainerRest.ADDALL_COMMANDNAME, (args) =>
                this.addOrRemoveAllTenantContainers("Sei sicuro di voler aggiungere tutti i contenitori?", UpdateActionType.TenantContainerAddAll)],
            [uscContainerRest.REMOVEALL_COMMANDNAME, (args) =>
                this.addOrRemoveAllTenantContainers("Sei sicuro di voler eliminare tutti i contenitori?", UpdateActionType.TenantContainerRemoveAll)],
            [uscContainerRest.SEARCH_COMMANDNAME, (args) => this._btnSearch_OnClick()]
        ];
        return items;
    }

    private _btnAddContainer_OnClick(args: Telerik.Web.UI.RadToolBarCancelEventArgs) {
        this._rwContainer.show();
        this._containerService.getContainers(null,
            (data: any) => {
                this.containers = <ContainerModel[]>data;
                this.addContainers(this.containers, this._cmbContainer);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.tbContainersControlId);
                this.showNotificationException(this.uscNotificationId, exception);
            });
        args.set_cancel(true);
    }

    private _btnRemoveContainer_OnClick(args: Telerik.Web.UI.RadToolBarCancelEventArgs) {
        if (this._rtvContainers.get_selectedNode() !== null) {
            this._manager.radconfirm("Sei sicuro di voler eliminare il contenitore selezionato?", (arg) => {
                if (arg) {
                    this._loadingPanel.show(this.tbContainersControlId);

                    let tenantToUpdate: TenantViewModel = this.constructTenant();
                    let containerId = this._rtvContainers.get_selectedNode().get_value();
                    let containerToDelete: ContainerModel = {} as ContainerModel;
                    containerToDelete.EntityShortId = containerId;

                    tenantToUpdate.Containers = [containerToDelete];
                    this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantContainerRemove,
                        (data) => {
                            this._treeViewRootNode().get_nodes().removeAt(this._rtvContainers.get_selectedNode().get_index());
                            if (this._treeViewRootNode().get_nodes().getItem(0) === undefined)
                                this._rtvContainers.get_nodes().clear();
                            this._loadingPanel.hide(this.tbContainersControlId);

                        },
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.tbContainersControlId);
                            $("#".concat(this.rtvTenantsId)).hide();
                            this.showNotificationException(this.uscNotificationId, exception);
                        });
                }
            }, 400, 300);
        } else {
            alert("Selezionare un Contentitore");
        }
        args.set_cancel(true);
    }

    private _btnSearch_OnClick() {
        this._treeViewRootNode().get_nodes().clear();
        this._populateTree(this._treeViewRootNode(), this._defaultPaginationModel(), this._rtbContainersSearchName.get_value());
    }

    toolbarContainer_onClick = (sender: any, args: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
        var btn = args.get_item() as Telerik.Web.UI.RadToolBarButton;
        let currentAction: (args) => void = this.toolbarActions().filter((item: [string, (args) => void]) => item[0] == btn.get_commandName())
            .map((item: [string, (args) => void]) => item[1])[0];
        currentAction(args);
    }

    _rwContainer_OnShow = (sender: Telerik.Web.UI.RadWindow, args: Sys.EventArgs) => {
        this._cmbContainer.clearSelection();
        this.selectedContainer = null;
    }

    cmbContainers_onClick = (sender: any, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this.selectedContainer = this.containers.filter(function (x) {
            return x.EntityShortId.toString() === args.get_item().get_value()
        })[0];
    }

    _cmbContainer_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox,
        args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
        let containerNumberOfItems: number = sender.get_items().get_count();
        this._containerService.getAllContainers(args.get_text(), this.maxNumberElements, containerNumberOfItems,
            (data: ODATAResponseModel<ContainerModel>) => {
                try {
                    this.refreshContainers(data.value);
                    let scrollToPosition: boolean = args.get_domEvent() == undefined;
                    if (scrollToPosition) {
                        if (sender.get_items().get_count() > 0) {
                            let scrollContainer: JQuery = $(sender.get_dropDownElement()).find('div.rcbScroll');
                            scrollContainer.scrollTop($(sender.get_items().getItem(containerNumberOfItems + 1).get_element()).position().top);
                        }
                    }
                    sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
                    sender.get_attributes().setAttribute('updating', 'false');
                    if (sender.get_items().get_count() > 0) {
                        containerNumberOfItems = sender.get_items().get_count() - 1;
                    }
                    this._cmbContainer.get_moreResultsBoxMessageElement().innerText = `Visualizzati ${containerNumberOfItems.toString()} di ${data.count.toString()}`;
                }
                catch (error) {
                }
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    btnContainerOk_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        if (this._cmbContainer && this.selectedContainer) {
            this._rwContainer.close();
            this._loadingPanel.show(this.tbContainersControlId);
            let nodeImageUrl = uscContainerRest.CONTAINER_NODE_IMAGE_URL;
            let nodeValue = this.selectedContainer.EntityShortId.toString();
            let nodeText = this.selectedContainer.Name;
            let alreadySavedInTree: boolean = this.alreadySavedInTree(nodeValue, this._rtvContainers);
            if (!alreadySavedInTree) {
                let tenantToUpdate: TenantViewModel = this.constructTenant();
                tenantToUpdate.Containers = [this.selectedContainer];
                this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantContainerAdd,
                    (data) => {
                        let searchText = this._rtbContainersSearchName.get_value();
                        if (!searchText || nodeText.indexOf(searchText) >= 0)
                            this.addNodesToRadTreeView(nodeValue, nodeText, "Contenitori", nodeImageUrl, this._rtvContainers);
                        this._loadingPanel.hide(this.tbContainersControlId);

                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.tbContainersControlId);
                        $("#".concat(this.rtvTenantsId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            }
        }
    }

    btnContainerCancel_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._rwContainer.close();
    }

    refreshContainers = (data: ContainerModel[]) => {
        if (data.length > 0) {
            this._cmbContainer.beginUpdate();
            if (this._cmbContainer.get_items().get_count() === 0) {
                let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._cmbContainer.get_items().insert(0, emptyItem);
            }

            $.each(data, (index, container) => {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(container.Name);
                item.set_value(container.EntityShortId.toString());
                this._cmbContainer.get_items().add(item);
                this.containers.push(container);
            });
            this._cmbContainer.showDropDown();
            this._cmbContainer.endUpdate();
        }
        else {
            if (this._cmbContainer.get_items().get_count() === 0) {
            }

        }
    }

    protected addContainers(containers: ContainerModel[], cmbContainer: Telerik.Web.UI.RadComboBox) {
        this.containers = containers;
        cmbContainer.get_items().clear();
        let item: Telerik.Web.UI.RadComboBoxItem;
        item = new Telerik.Web.UI.RadComboBoxItem();
        item.set_text("");
        item.set_value("");
        cmbContainer.get_items().add(item);
        for (let container of containers) {
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(container.Name);
            item.set_value(container.EntityShortId.toString());
            cmbContainer.get_items().add(item);
        }
    }

    private _appendEmptyNode(parentNode: Telerik.Web.UI.RadTreeNode): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text("Nessun elemento trovato");
        parentNode.get_nodes().add(emptyNode);
    }

    private _appendLoadMoreNode(expandedNode: Telerik.Web.UI.RadTreeNode, totalChildrenCount: number) {
        let expandedNodeType: TreeNodeType = expandedNode.get_attributes().getAttribute(uscContainerRest.NODETYPE_ATTRNAME);

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

    private _createLoadMoreNode(): Telerik.Web.UI.RadTreeNode {
        let loadMoreNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(TreeNodeType.LoadMore, uscContainerRest.LOADMORE_NODE_LABEL, null, uscContainerRest.LOAD_MORE_NODE_IMAGEURL);
        this._appendEmptyNode(loadMoreNode);

        return loadMoreNode;
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
        treeNode.get_attributes().setAttribute(uscContainerRest.NODETYPE_ATTRNAME, nodeType);

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

    public initializeContainersTreeView(tenant: TenantViewModel): void {
        uscContainerRest._currentSelectedTenant = tenant;
        this._rtvContainers.get_nodes().clear();
        this.createRootNode(this._rtvContainers, null, "Contenitori");
        this._populateTree(this._treeViewRootNode(), this._defaultPaginationModel());
    }

    private createRootNode(radTreeView: Telerik.Web.UI.RadTreeView, rtvNode: Telerik.Web.UI.RadTreeNode, text: string): Telerik.Web.UI.RadTreeNode {
        rtvNode = new Telerik.Web.UI.RadTreeNode();
        rtvNode.set_text(text);
        rtvNode.get_attributes().setAttribute(uscContainerRest.NODETYPE_ATTRNAME, TreeNodeType.Root);
        rtvNode.set_expanded(true);

        radTreeView.get_nodes().add(rtvNode);

        return rtvNode;
    }

    private _findTenantContainers(paginationModel?: PaginationModel, searchText?: string): JQueryPromise<ODATAResponseModel<ContainerModel>> {
        let defferedRequest: JQueryDeferred<ODATAResponseModel<ContainerModel>> = $.Deferred<ODATAResponseModel<ContainerModel>>();

        this._containerService.getTenantContainers(uscContainerRest._currentSelectedTenant.UniqueId, defferedRequest.resolve, defferedRequest.reject, paginationModel, searchText);

        return defferedRequest.promise();
    }

    private _populateTree(expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel, searchText?: string) {
        expandedNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);

        this._findTenantContainers(paginationModel, searchText)
            .then((odataResponse) => {
                this._createTreeNodeFromContainerModels(odataResponse.value);
                this._appendLoadMoreNode(expandedNode, odataResponse.count);
            })
            .fail((exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception)
            })
            .always(() => {
                expandedNode.hideLoadingStatus();
            });
    }

    private _createTreeNodeFromContainerModels(containers: ContainerModel[]): Telerik.Web.UI.RadTreeNode[] {
        return containers.map((container) => this._createTreeNode(TreeNodeType.Container, container.Name, container.EntityShortId, uscContainerRest.CONTAINER_NODE_IMAGE_URL, this._treeViewRootNode()));
    }

    private _defaultPaginationModel(): PaginationModel {
        return new PaginationModel(0, this.treeViewNodesPageSize);
    }

    public treeView_LoadMoreOnExpand = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs): void => {
        let expandedNode: Telerik.Web.UI.RadTreeNode = args.get_node();
        expandedNode.get_nodes().clear();

        let nodeType: TreeNodeType = expandedNode.get_attributes().getAttribute(uscContainerRest.NODETYPE_ATTRNAME);

        if (nodeType === TreeNodeType.Root) {
            this._populateTree(expandedNode, this._defaultPaginationModel(), this._rtbContainersSearchName.get_value());
        }
        if (nodeType === TreeNodeType.LoadMore) {
            const expandedNodeParent: Telerik.Web.UI.RadTreeNode = expandedNode.get_parent();
            const parentNodeChildrenCount: number = expandedNodeParent.get_nodes().get_count();

            const recordsToSkip: number = parentNodeChildrenCount - 1;
            const paginationModel: PaginationModel = new PaginationModel(recordsToSkip, this.treeViewNodesPageSize);

            this._populateTree(expandedNode, paginationModel, this._rtbContainersSearchName.get_value());
        }
    }

    public treeView_LoadMoreOnClick = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs): void => {
        let nodeType: TreeNodeType = args.get_node().get_attributes().getAttribute(uscContainerRest.NODETYPE_ATTRNAME)
        let btn: Telerik.Web.UI.RadToolBarButton = this._toolbarContainer.findButtonByCommandName(uscContainerRest.REMOVE_COMMANDNAME);

        if (nodeType === TreeNodeType.LoadMore) {
            btn.disable();
        } else {
            btn.enable();
        }
    }

    addOrRemoveAllTenantContainers(message: string, actionType: UpdateActionType): void {
        this._manager.radconfirm(message, (arg) => {
            if (arg) {
                this._loadingPanel.show(this.tbContainersControlId);
                this._tenantService.updateTenant(uscContainerRest._currentSelectedTenant, actionType, (data) => {
                    this.initializeContainersTreeView(uscContainerRest._currentSelectedTenant);
                    this._loadingPanel.hide(this.tbContainersControlId);
                }, (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.tbContainersControlId);
                    this.showNotificationException(this.uscNotificationId, exception);
                });
            }

            document.getElementsByTagName("body")[0].setAttribute("class", "comm chrome");

        }, 400, 300);
    }

    protected addNodesToRadTreeView(nodeValue: string, nodeText: string, text: string, nodeImageUrl: string, radTreeView: Telerik.Web.UI.RadTreeView) {
        let rtvNode: Telerik.Web.UI.RadTreeNode;

        if (radTreeView.get_nodes().get_count() === 0) {
            rtvNode = this.createRootNode(radTreeView, rtvNode, text);
        }

        rtvNode = new Telerik.Web.UI.RadTreeNode();
        rtvNode.set_text(nodeText);
        rtvNode.set_value(nodeValue);
        rtvNode.set_imageUrl(nodeImageUrl);
        radTreeView.get_nodes().getNode(0).get_nodes().insert(-1, rtvNode);
        radTreeView.get_nodes().getNode(0).expand();
    }

    private alreadySavedInTree(nodeValue: string, radTreeView: Telerik.Web.UI.RadTreeView): boolean {
        let alreadySavedInTree: boolean = false;
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
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    constructTenant(): TenantViewModel {
        let newTenant: TenantViewModel = new TenantViewModel();
        newTenant.UniqueId = uscContainerRest._currentSelectedTenant.UniqueId;
        newTenant.CompanyName = uscContainerRest._currentSelectedTenant.CompanyName;
        newTenant.TenantName = uscContainerRest._currentSelectedTenant.TenantName;
        newTenant.Note = uscContainerRest._currentSelectedTenant.Note;
        newTenant.StartDate = uscContainerRest._currentSelectedTenant.StartDate;
        newTenant.EndDate = uscContainerRest._currentSelectedTenant.EndDate;
        newTenant.TenantAOO = uscContainerRest._currentSelectedTenant.TenantAOO;
        return newTenant;
    }
}

export = uscContainerRest;