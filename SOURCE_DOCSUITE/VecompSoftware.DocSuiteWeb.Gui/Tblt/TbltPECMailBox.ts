import TbltPECMailBoxBase = require('Tblt/TbltPECMailBoxBase');
import PECMailBoxService = require('App/Services/PECMails/PECMailBoxService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import PECMailBoxViewModel = require('App/ViewModels/PECMails/PECMailBoxViewModel');
import PECMailBoxRulesetModel = require('App/Models/PECMails/PECMailBoxRulesetModel');
import PECMailBoxModel = require("App/Models/PECMails/PECMailBoxModel");
import uscPECMailBoxSettings = require('UserControl/uscPECMailBoxSettings');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import PaginationModel = require('App/Models/Commons/PaginationModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import TreeNodeType = require('App/Models/Commons/TreeNodeType');

class TbltPECMailBox extends TbltPECMailBoxBase {
    protected PECMailBox_TYPE_NAME = "PECMailBox";
    private _serviceConfigurations: ServiceConfiguration[];

    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    splitterMainId: string;
    ToolBarSearchId: string;
    rtvPECMailBoxesId: string;
    rtvResult: PECMailBoxViewModel[];
    rpbDetailsId: string;
    lblPECMailBoxIdId: string;
    lblMailBoxRecipientId: string;
    lblIncomingServerId: string;
    lblOutgoingServerId: string;
    lblRulesetNameId: string;
    lblRulesetConditionId: string;
    lblRulesetTypeId: string;
    windowSetRuleId: string;
    windowInsertId: string;
    txtRulesetNameId: string;
    txtSpecifySenderId: string;
    treeViewNodesPageSize: number;
    rlbSpecifyPECMailBoxId: string;
    btnPECMAilBoxSaveId: string;
    uscPECMailBoxSettingsId: string;
    uscPECMailBoxSettingsInsertId: string;
    folderToolBarId: string;
    lblUsernameId: string;
    lblServerTypeId: string;
    lblOUTPortId: string;
    lblProfileId: string;
    lblJeepServINId: string;
    lblJeepServOUTId: string;
    lblElectronicTypeId: string;
    lblINPortId: string;
    lblLocationId: string;
    lblIsInteropId: string;
    lblIsProtocolId: string;
    lblIsPublicProtocolId: string;
    lblINSSLId: string;
    lblOUTSSLId: string;
    lblIsManagedId: string;
    lblIsNotManagedId: string;
    lblIsHandleEnabledId: string;
    viewLoginError: string;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _toolbarSearch: Telerik.Web.UI.RadToolBar;
    private _rtvPECMailBoxes: Telerik.Web.UI.RadTreeView;
    private _toolbarItemSearchName: Telerik.Web.UI.RadToolBarItem;
    private _toolbarButtonMailboxError: Telerik.Web.UI.RadToolBarButton;
    private _toolbarButtonNotHandled: Telerik.Web.UI.RadToolBarButton;
    private _txtSearchName: Telerik.Web.UI.RadTextBox;
    private _toolbarItemButtonSearch: Telerik.Web.UI.RadToolBarItem;
    private _rpbDetails: Telerik.Web.UI.RadPanelBar;
    private _lblMailBoxRecipient: HTMLLabelElement;
    private _lblIncomingServer: HTMLLabelElement;
    private _lblOutgoingServer: HTMLLabelElement;
    private _lblUsername: HTMLLabelElement;
    private _lblServerType: HTMLLabelElement;
    private _lblOUTPort: HTMLLabelElement;
    private _lblINPort: HTMLLabelElement;
    private _lblProfile: HTMLLabelElement;
    private _lblJeepServIN: HTMLLabelElement;
    private _lblJeepServOUT: HTMLLabelElement;
    private _lblElectronicType: HTMLLabelElement;
    private _lblLocation: HTMLLabelElement;
    private _lblIsInteropId: HTMLLabelElement;
    private _lblIsProtocolId: HTMLLabelElement;
    private _lblIsPublicProtocolId: HTMLLabelElement;
    private _lblINSSLId: HTMLLabelElement;
    private _lblOUTSSLId: HTMLLabelElement;
    private _lblIsManagedId: HTMLLabelElement;
    private _lblIsNotManagedId: HTMLLabelElement;
    private _lblIsHandleEnabledId: HTMLLabelElement;
    private _lblRulesetName: HTMLLabelElement;
    private _lblRulesetCondition: HTMLLabelElement;
    private _lblRulesetType: HTMLLabelElement;
    private _windowSetRule: Telerik.Web.UI.RadWindow;
    private _windowInsert: Telerik.Web.UI.RadWindow;
    private _txtRulesetName: Telerik.Web.UI.RadTextBox;
    private _txtSpecifySender: Telerik.Web.UI.RadTextBox;
    private _rlbSpecifyPECMailBox: Telerik.Web.UI.RadDropDownList;
    private _btnPECMailBoxSave: Telerik.Web.UI.RadButton;
    private _folderToolBar: Telerik.Web.UI.RadToolBar;
    private _uscPECMailBoxSettings: uscPECMailBoxSettings;
    private _uscPECMailBoxSettingsInsert: uscPECMailBoxSettings;
    private _treeViewRootNode: Telerik.Web.UI.RadTreeNode;
    private _nodeExpandingActionHandlerMap: Map<TreeNodeType, (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel) => JQueryPromise<void>>;
    private _appliedSearchText: string;

    private get _defaultPaginationModel(): PaginationModel {
        return new PaginationModel(0, this.treeViewNodesPageSize);
    }

    private get filterInputValue(): string {
        return this._txtSearchName.get_value();
    }

    private get filterOnError(): boolean {
        if (this._toolbarButtonMailboxError.get_checked() === true) {
            return true;
        }
        else {
            return null;
        }
    }

    private get filterIncludeNotHandled(): boolean {
        if (this._toolbarButtonNotHandled.get_checked() === true) {
            return true;
        }
        else {
            return false;
        }
    }

    private get TreeviewRootNode(): Telerik.Web.UI.RadTreeNode {
        if (!this._treeViewRootNode) {
            this._treeViewRootNode = this._rtvPECMailBoxes.get_nodes().getNode(0);
        }

        return this._treeViewRootNode;
    }

    private static TOOLBAR_CREATE = "create";
    private static TOOLBAR_MODIFY = "modify";
    private static TOTAL_CHILDREN_COUNT_ATTRNAME = "totalChildrenCount";
    private static LOADMORE_NODE_LABEL: string = "Carica più elementi";
    private static LOAD_MORE_NODE_IMAGEURL: string = "../App_Themes/DocSuite2008/imgset16/add.png";
    private static NO_ITEMS_FOUND_TEXT = "Nessun elemento trovato";
    private static NODETYPE_TYPE_NAME = "NodeType";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.PECMailBox_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    initialize() {
        super.initialize();
        this.initializeControls();
        this.initializeUserControls();
        this._initializeNodesExpandingActionHandlersMap();
        this.initializeViewLoginError();
    }

    initializeControls(): void {
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._toolbarSearch = <Telerik.Web.UI.RadToolBar>$find(this.ToolBarSearchId);
        this._toolbarSearch.add_buttonClicking(this.toolbarSearch_onClick);
        this._rtvPECMailBoxes = <Telerik.Web.UI.RadTreeView>$find(this.rtvPECMailBoxesId);
        this._rtvPECMailBoxes.add_nodeClicked(this.rtvPECMailBoxes_onClick);
        this._rtvPECMailBoxes.add_nodeExpanded(this.rtvPECMailBoxes_onExpand);
        this._toolbarItemSearchName = this._toolbarSearch.findItemByValue("searchName");
        this._toolbarButtonMailboxError = this._toolbarSearch.findItemByValue("mailboxError") as Telerik.Web.UI.RadToolBarButton;
        this._toolbarButtonNotHandled = this._toolbarSearch.findItemByValue("notHandled") as Telerik.Web.UI.RadToolBarButton;
        this._txtSearchName = <Telerik.Web.UI.RadTextBox>this._toolbarItemSearchName.findControl("txtName");
        this._toolbarItemButtonSearch = <Telerik.Web.UI.RadToolBarItem>this._toolbarSearch.findItemByValue("searchCommand");
        this._rpbDetails = <Telerik.Web.UI.RadPanelBar>$find(this.rpbDetailsId);
        this._rpbDetails.findItemByValue("rpiDetails").set_visible(false);
        this._lblMailBoxRecipient = <HTMLLabelElement>document.getElementById(this.lblMailBoxRecipientId);
        this._lblIncomingServer = <HTMLLabelElement>document.getElementById(this.lblIncomingServerId);
        this._lblOutgoingServer = <HTMLLabelElement>document.getElementById(this.lblOutgoingServerId);
        this._lblUsername = <HTMLLabelElement>document.getElementById(this.lblUsernameId);
        this._lblServerType = <HTMLLabelElement>document.getElementById(this.lblServerTypeId);
        this._lblOUTPort = <HTMLLabelElement>document.getElementById(this.lblOUTPortId);
        this._lblINPort = <HTMLLabelElement>document.getElementById(this.lblINPortId);
        this._lblProfile = <HTMLLabelElement>document.getElementById(this.lblProfileId);
        this._lblJeepServIN = <HTMLLabelElement>document.getElementById(this.lblJeepServINId);
        this._lblJeepServOUT = <HTMLLabelElement>document.getElementById(this.lblJeepServOUTId);
        this._lblElectronicType = <HTMLLabelElement>document.getElementById(this.lblElectronicTypeId);
        this._lblLocation = <HTMLLabelElement>document.getElementById(this.lblLocationId);
        this._lblIsInteropId = <HTMLLabelElement>document.getElementById(this.lblIsInteropId);
        this._lblIsProtocolId = <HTMLLabelElement>document.getElementById(this.lblIsProtocolId);
        this._lblIsPublicProtocolId = <HTMLLabelElement>document.getElementById(this.lblIsPublicProtocolId);
        this._lblINSSLId = <HTMLLabelElement>document.getElementById(this.lblINSSLId);
        this._lblOUTSSLId = <HTMLLabelElement>document.getElementById(this.lblOUTSSLId);
        this._lblIsManagedId = <HTMLLabelElement>document.getElementById(this.lblIsManagedId);
        this._lblIsNotManagedId = <HTMLLabelElement>document.getElementById(this.lblIsNotManagedId);
        this._lblIsHandleEnabledId = <HTMLLabelElement>document.getElementById(this.lblIsHandleEnabledId);
        this._lblRulesetName = <HTMLLabelElement>document.getElementById(this.lblRulesetNameId);
        this._lblRulesetCondition = <HTMLLabelElement>document.getElementById(this.lblRulesetConditionId);
        this._lblRulesetType = <HTMLLabelElement>document.getElementById(this.lblRulesetTypeId);
        this._windowSetRule = <Telerik.Web.UI.RadWindow>$find(this.windowSetRuleId);
        this._windowInsert = <Telerik.Web.UI.RadWindow>$find(this.windowInsertId);
        this._windowInsert.add_close(this.windowInsert_close);
        this._txtRulesetName = <Telerik.Web.UI.RadTextBox>$find(this.txtRulesetNameId);
        this._txtSpecifySender = <Telerik.Web.UI.RadTextBox>$find(this.txtSpecifySenderId);
        this._rlbSpecifyPECMailBox = <Telerik.Web.UI.RadDropDownList>$find(this.rlbSpecifyPECMailBoxId);
        this._btnPECMailBoxSave = <Telerik.Web.UI.RadButton>$find(this.btnPECMAilBoxSaveId);
        this._btnPECMailBoxSave.add_clicked(this.btnPECMailBoxSave_onClick);
        this._folderToolBar = <Telerik.Web.UI.RadToolBar>$find(this.folderToolBarId);
        this._folderToolBar.add_buttonClicked(this.folderToolBar_onClick);
        this._folderToolBar.findItemByValue(TbltPECMailBox.TOOLBAR_MODIFY).set_enabled(false);
    }

    initializeUserControls(): void {
        this._uscPECMailBoxSettings = <uscPECMailBoxSettings>$(`#${this.uscPECMailBoxSettingsId}`).data();
        this._uscPECMailBoxSettingsInsert = <uscPECMailBoxSettings>$(`#${this.uscPECMailBoxSettingsInsertId}`).data();
        $(`#${this._uscPECMailBoxSettings.pnlDetailsId}`).hide();
        $(`#${this._uscPECMailBoxSettingsInsert.pnlDetailsId}`).hide();
    }

    initializeViewLoginError(): void {
        if (this.viewLoginError === "True") {
            this._toolbarButtonMailboxError.set_checked(true);
            this.doSearch();
        }
    }

    private _initializeNodesExpandingActionHandlersMap(): void {
        this._nodeExpandingActionHandlerMap = new Map<TreeNodeType, (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel) => JQueryPromise<void>>();
        this._nodeExpandingActionHandlerMap.set(TreeNodeType.Root, this._rootNodeExpandedActionHandler);
        this._nodeExpandingActionHandlerMap.set(TreeNodeType.LoadMore, this._loadMoreNodeExpandedActionHandler);
    }

    windowInsert_close = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        this.loadResults(this._txtSearchName.get_textBoxValue());
        this._rpbDetails.findItemByValue("rpiDetails").set_visible(false);
        $(`#${this._uscPECMailBoxSettings.pnlDetailsId}`).hide();
    }

    toolbarSearch_onClick = (sender: any, args: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
        let item: Telerik.Web.UI.RadToolBarItem = args.get_item();
        if (item.get_value() === "searchMailbox") {
            this.doSearch();
        }
    }

    doSearch(): void {
        this._loadingPanel.show(this.splitterMainId);
        this._setNodeAttribute(this.TreeviewRootNode, TbltPECMailBox.TOTAL_CHILDREN_COUNT_ATTRNAME, null);
        this._appliedSearchText = this._txtSearchName.get_textBoxValue();
        this.loadResults(this._txtSearchName.get_textBoxValue());
        this._rpbDetails.findItemByValue("rpiDetails").set_visible(false);
        $(`#${this._uscPECMailBoxSettings.pnlDetailsId}`).hide();
    }

    rtvPECMailBoxes_onExpand = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let expandedNode: Telerik.Web.UI.RadTreeNode = args.get_node(),
            expandedNodeType: TreeNodeType = expandedNode.get_attributes().getAttribute(TbltPECMailBox.NODETYPE_TYPE_NAME);

        if (this.filterInputValue && expandedNodeType === TreeNodeType.Root || this.filterInputValue && expandedNode.get_allNodes().length > 1) {
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

                const expandedNodeParentTotalChildrenCount: number | undefined =
                    expandedNodeType === TreeNodeType.LoadMore
                        ? expandedNode.get_parent().get_attributes().getAttribute(TbltPECMailBox.TOTAL_CHILDREN_COUNT_ATTRNAME)
                        : expandedNode.get_attributes().getAttribute(TbltPECMailBox.TOTAL_CHILDREN_COUNT_ATTRNAME);

                if (expandedNodeParentTotalChildrenCount) {
                    this._appendLoadMoreNode(expandedNode, expandedNodeParentTotalChildrenCount);
                }
            })
            .always(() => expandedNode.hideLoadingStatus())
            .fail(this.showNotificationException);
    }

    private _loadMoreNodeExpandedActionHandler = (expandedNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> => {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();

        const expandedNodeParent: Telerik.Web.UI.RadTreeNode = expandedNode.get_parent();
        const parentNodeChildrenCount: number = expandedNodeParent.get_nodes().get_count();
        let parentNodeType: TreeNodeType = expandedNodeParent.get_attributes().getAttribute(TbltPECMailBox.NODETYPE_TYPE_NAME);

        if (!this._nodeExpandingActionHandlerMap.has(parentNodeType)) {
            defferedRequest.reject();
            return;
        }

        const expandedNodeActionHandler: (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel) => JQueryPromise<void>
            = this._nodeExpandingActionHandlerMap.get(parentNodeType);

        const recordsToSkip: number = parentNodeChildrenCount - 1;
        const paginationModel: PaginationModel = new PaginationModel(recordsToSkip, this.treeViewNodesPageSize);

        expandedNodeActionHandler(expandedNodeParent, paginationModel)
            .done(() => defferedRequest.resolve())
            .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }


    private _rootNodeExpandedActionHandler = (expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel): JQueryPromise<void> => {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();
        this._loadPECMail(expandedNode, paginationModel, this._appliedSearchText)
        .done(() => defferedRequest.resolve())
        .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }

    private _findPECMailChildren(searchFilter: string, paginationModel?: PaginationModel): JQueryPromise<ODATAResponseModel<PECMailBoxViewModel>> {
        let defferedRequest: JQueryDeferred<ODATAResponseModel<PECMailBoxViewModel>> = $.Deferred<ODATAResponseModel<PECMailBoxViewModel>>();
        this.pecMailBoxService.getPECMailBoxes(searchFilter, this.filterOnError, this.filterIncludeNotHandled, defferedRequest.resolve, defferedRequest.reject, paginationModel);

        return defferedRequest.promise();
    }

    private _createPECMailNode(pecMailModel: PECMailBoxViewModel, parentNode?: Telerik.Web.UI.RadTreeNode): Telerik.Web.UI.RadTreeNode {
        const parentNodeIsRootNode: boolean = !!parentNode && parentNode.get_value() === this.TreeviewRootNode.get_value();
        let treeNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(TreeNodeType.PECMailBox, pecMailModel.MailBoxRecipient, pecMailModel.EntityShortId, null, parentNodeIsRootNode ? parentNode : undefined);

        return treeNode;
    }

    private _appendChildrenCountAttribute(node: Telerik.Web.UI.RadTreeNode, totalChildrenCount: number): void {
        const currentChildrenAttributeValue: number | undefined = node.get_attributes().getAttribute(TbltPECMailBox.TOTAL_CHILDREN_COUNT_ATTRNAME);

        if (!currentChildrenAttributeValue) {
            node.get_attributes().setAttribute(TbltPECMailBox.TOTAL_CHILDREN_COUNT_ATTRNAME, totalChildrenCount);
        }
    }

    private _createPECMailNodesFromModels = (pecMails: PECMailBoxViewModel[], parentNode: Telerik.Web.UI.RadTreeNode, totalCount?: number): Telerik.Web.UI.RadTreeNode[] => {
        let pecMailsNodes: Telerik.Web.UI.RadTreeNode[] = pecMails.map(pecMail => this._createPECMailNode(pecMail, parentNode));

        if (totalCount && totalCount > 0) {
            this._appendChildrenCountAttribute(parentNode, totalCount);
        }

        return pecMailsNodes;
    }

    private loadResults(searchFilter: string) {
        this.TreeviewRootNode.get_nodes().clear();
        this._setNodeAttribute(this.TreeviewRootNode, TbltPECMailBox.NODETYPE_TYPE_NAME, TreeNodeType.Root);
        this._loadPECMail(this.TreeviewRootNode, this._defaultPaginationModel, searchFilter)
            .done(() => {
                this.TreeviewRootNode.set_expanded(true);
                const expandedNodeParentTotalChildrenCount: number = this._getNodeAttribute<number>(this.TreeviewRootNode, TbltPECMailBox.TOTAL_CHILDREN_COUNT_ATTRNAME);
                this._appendLoadMoreNode(this.TreeviewRootNode, expandedNodeParentTotalChildrenCount);
                this._loadingPanel.hide(this.splitterMainId);
            });
    }

    private _loadPECMail(expandedNode: Telerik.Web.UI.RadTreeNode, paginationModel: PaginationModel = null, searchFilter?: string) {
        let defferedRequest: JQueryDeferred<Telerik.Web.UI.RadTreeNode[]> = $.Deferred<Telerik.Web.UI.RadTreeNode[]>();

        this._findPECMailChildren(searchFilter, paginationModel)
            .then((odataResult: ODATAResponseModel<PECMailBoxViewModel>) => this._createPECMailNodesFromModels(odataResult.value, expandedNode, odataResult.count))
            .done(defferedRequest.resolve)
            .fail(defferedRequest.reject);

        return defferedRequest.promise();
    }

    private _appendLoadMoreNode(expandedNode: Telerik.Web.UI.RadTreeNode, totalChildrenCount: number) {
        let expandedNodeType: TreeNodeType = expandedNode.get_attributes().getAttribute(TbltPECMailBox.NODETYPE_TYPE_NAME);
        if (expandedNodeType === TreeNodeType.LoadMore) {
            const expandedNodeParentChildrenCollection: Telerik.Web.UI.RadTreeNodeCollection = expandedNode.get_parent().get_nodes();
            const allChildrenLoaded: boolean = (expandedNodeParentChildrenCollection.get_count() - 1) === totalChildrenCount;

            expandedNodeParentChildrenCollection.remove(expandedNode);
            if (!allChildrenLoaded) {
                expandedNodeParentChildrenCollection.add(this._createLoadMoreNode());
            }
        }
        else {
            const expandedNodeChildrenCollection: Telerik.Web.UI.RadTreeNodeCollection = expandedNode.get_nodes();
            const expandedNodeChildrenCount: number = expandedNodeChildrenCollection.get_count();

            if (expandedNodeChildrenCount === totalChildrenCount) {
                return;
            }

            expandedNodeChildrenCollection.add(this._createLoadMoreNode());
        }
    }

    rtvPECMailBoxes_onClick = (sender: any, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        $("#insertPEC").hide();
        $(`#${this._uscPECMailBoxSettings.pnlDetailsId}`).hide();
        this._rtvPECMailBoxes.get_nodes().getNode(0).set_selected(false);
        let selectedNode: Telerik.Web.UI.RadTreeNode = args.get_node();
        uscPECMailBoxSettings.selectedPecId = selectedNode.get_value()
        if (selectedNode.get_level() == 0) {
            this._folderToolBar.findItemByValue(TbltPECMailBox.TOOLBAR_MODIFY).set_enabled(false);
            this._folderToolBar.findItemByValue(TbltPECMailBox.TOOLBAR_CREATE).set_enabled(true);
            this._rpbDetails.findItemByValue("rpiDetails").set_visible(false);
        } else {
            this._folderToolBar.findItemByValue(TbltPECMailBox.TOOLBAR_MODIFY).set_enabled(true);
            this._folderToolBar.findItemByValue(TbltPECMailBox.TOOLBAR_CREATE).set_enabled(false);
            this._rpbDetails.findItemByValue("rpiDetails").set_visible(true);
            this.loadPECMailBoxDetails(this._rtvPECMailBoxes.get_selectedNode().get_value());
        }
    }

    private appendEmptyNode(parentNode: Telerik.Web.UI.RadTreeNode): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text(TbltPECMailBox.NO_ITEMS_FOUND_TEXT);
        parentNode.get_nodes().add(emptyNode);
    }

    private _createLoadMoreNode(): Telerik.Web.UI.RadTreeNode {
        let loadMoreNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(TreeNodeType.LoadMore, TbltPECMailBox.LOADMORE_NODE_LABEL, null, TbltPECMailBox.LOAD_MORE_NODE_IMAGEURL);
        this.appendEmptyNode(loadMoreNode);

        return loadMoreNode;
    }

    private _createTreeNode(nodeType: TreeNodeType, nodeDescription: string, nodeValue: number | string, imageUrl: string, parentNode?: Telerik.Web.UI.RadTreeNode, tooltipText?: string, expandedImageUrl?: string): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        treeNode.set_text(nodeDescription);
        treeNode.set_value(nodeValue);

        this._setNodeAttribute(treeNode, TbltPECMailBox.NODETYPE_TYPE_NAME, nodeType);
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
            treeNode.get_attributes().removeAttribute(TbltPECMailBox.NODETYPE_TYPE_NAME);
        }

        treeNode.get_attributes().setAttribute(attributeName, attributeValue);
    }

    loadPECMailBoxDetails(pecMailBoxId: number) {
        this.pecMailBoxService.getPECMailBoxById(pecMailBoxId,
            (data) => {
                if (!data) return;
                let pecMailBox: PECMailBoxViewModel = data[0];

                PageClassHelper.callUserControlFunctionSafe<uscPECMailBoxSettings>(this.uscPECMailBoxSettingsInsertId)
                    .done((instance) => {
                        instance.loadDropdowns().done(() => {
                            if (pecMailBox.IdConfiguration) {
                                this._lblProfile.innerText = instance.pecMailBoxConfigurations.filter(x => x.EntityId === pecMailBox.IdConfiguration)[0].Name;
                            }
                            if (pecMailBox.Location) {
                                this._lblLocation.innerText = instance.locations.filter(x => x.EntityShortId === pecMailBox.Location.EntityShortId)[0].Name;
                            }
                            if (pecMailBox.IdJeepServiceIncomingHost) {
                                this._lblJeepServIN.innerText = instance.jeepServiceHosts.filter(x => x.UniqueId === pecMailBox.IdJeepServiceIncomingHost)[0].Hostname;
                            }
                            if (pecMailBox.IdJeepServiceOutgoingHost) {
                                this._lblJeepServOUT.innerText = instance.jeepServiceHosts.filter(x => x.UniqueId === pecMailBox.IdJeepServiceOutgoingHost)[0].Hostname;
                            }
                        });
                    });
                this._lblMailBoxRecipient.innerText = pecMailBox.MailBoxRecipient;
                this._lblIncomingServer.innerText = pecMailBox.IncomingServer;
                this._lblOutgoingServer.innerText = pecMailBox.OutgoingServer;
                this._lblUsername.innerText = pecMailBox.Username;
                this._lblServerType.innerText = (1 + pecMailBox.IncomingServerProtocol).toString();
                this._lblOUTPort.innerText = pecMailBox.OutgoingServerPort.toString();
                this._lblINPort.innerText = pecMailBox.IncomingServerPort.toString();
                this._lblElectronicType.innerText = pecMailBox.InvoiceType;
                this._lblIsInteropId.innerText = pecMailBox.IsForInterop ? "Si" : "No";
                this._lblIsProtocolId.innerText = pecMailBox.IsProtocolBox ? "Si" : "No";
                this._lblIsPublicProtocolId.innerText = pecMailBox.IsProtocolBoxExplicit ? "Si" : "No";
                this._lblINSSLId.innerText = pecMailBox.IncomingServerUseSsl ? "Si" : "No";
                this._lblOUTSSLId.innerText = pecMailBox.OutgoingServerUseSsl ? "Si" : "No";
                this._lblIsManagedId.innerText = pecMailBox.Managed ? "Si" : "No";
                this._lblIsNotManagedId.innerText = pecMailBox.Unmanaged ? "Si" : "No";
                this._lblIsHandleEnabledId.innerText = pecMailBox.IsHandleEnabled ? "Si" : "No";
                if (pecMailBox.RulesetDefinition !== null) {
                    let ruleset: PECMailBoxRulesetModel = JSON.parse(pecMailBox.RulesetDefinition);
                    this._lblRulesetName.innerText = ruleset.Name === undefined ? "" : ruleset.Name;
                    this._lblRulesetCondition.innerText = ruleset.Condition === undefined ? "" : ruleset.Condition;
                    this._lblRulesetType.innerText = ruleset.Rule === undefined ? "" : ruleset.Rule;
                } else {
                    this._lblRulesetName.innerText = "";
                    this._lblRulesetCondition.innerText = "";
                    this._lblRulesetType.innerText = "";
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvPECMailBoxesId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    btnPECMailBoxSave_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        let thisObj = this;
        let pecMailBox: PECMailBoxModel = <PECMailBoxModel>this.rtvResult.filter(function (x) {
            return x.EntityShortId === +uscPECMailBoxSettings.selectedPecId
        })[0];
        let ruleset: PECMailBoxRulesetModel = pecMailBox.RulesetDefinition === null ? new PECMailBoxRulesetModel() : JSON.parse(pecMailBox.RulesetDefinition);
        ruleset.Name = this._txtRulesetName.get_value();
        pecMailBox.MailBoxRecipient = this._txtSpecifySender.get_value();
        let selectedPECMailBoxId: number = Number(this._rlbSpecifyPECMailBox.get_selectedItem().get_value());
        let selectedPECMailBox: PECMailBoxModel = <PECMailBoxModel>this.rtvResult.filter(function (x) {
            return x.EntityShortId === selectedPECMailBoxId
        })[0];
        ruleset.Reference = selectedPECMailBox;
        pecMailBox.RulesetDefinition = JSON.stringify(ruleset);
        this.updateCallback(pecMailBox);
    }

    updateCallback(pecMailBoxModel: PECMailBoxModel): void {
        this.pecMailBoxService.updatePECMailBox(pecMailBoxModel,
            (data: any) => {
                if (!data) return;
                this._loadingPanel.show(this.splitterMainId);
                this.loadResults(this._txtSearchName.get_textBoxValue());
                this._windowSetRule.close();
                this.loadPECMailBoxDetails(pecMailBoxModel.EntityShortId);
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    folderToolBar_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        switch (args.get_item().get_value()) {
            case TbltPECMailBox.TOOLBAR_CREATE: {
                PageClassHelper.callUserControlFunctionSafe<uscPECMailBoxSettings>(this.uscPECMailBoxSettingsInsertId)
                    .done((instance) => {
                        instance.emptyUserInputs();
                        instance.setPanelText('');
                    });
                $(`#${this._uscPECMailBoxSettingsInsert.pnlDetailsId}`).show();
                $("#insertPEC").show();
                this._windowInsert.show();
                break;
            }
            case TbltPECMailBox.TOOLBAR_MODIFY: {
                if (this._rtvPECMailBoxes.get_selectedNode().get_level() > 0 && this._rtvPECMailBoxes.get_selectedNode() !== null) {
                    //this.loadPECMailBoxDetails(this._rtvPECMailBoxes.get_selectedNode().get_value());
                    PageClassHelper.callUserControlFunctionSafe<uscPECMailBoxSettings>(this.uscPECMailBoxSettingsId)
                        .done((instance) => {
                            instance.loadPECMailBoxDetails(this._rtvPECMailBoxes.get_selectedNode().get_value());
                        });
                    $(`#${this._uscPECMailBoxSettings.pnlDetailsId}`).show();
                }
                else {
                    alert("Nessun elemento selezionato per la modifica.");
                }
                break;
            }
        }
    }
}

export = TbltPECMailBox;
