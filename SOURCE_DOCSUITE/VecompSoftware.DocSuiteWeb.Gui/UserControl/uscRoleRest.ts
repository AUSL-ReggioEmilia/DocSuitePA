/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require("../App/Services/ServiceConfiguration");
import RoleService = require("../App/Services/Commons/RoleService");
import ServiceConfigurationHelper = require("../App/Helpers/ServiceConfigurationHelper");
import RoleModel = require("../App/Models/Commons/RoleModel");
import ImageHelper = require("../App/Helpers/ImageHelper");
import UscRoleRestEventType = require("../App/Models/Commons/UscRoleRestEventType");
import UscRoleRestConfiguration = require("../App/Models/Commons/UscRoleRestConfiguration");

declare var ValidatorEnable: any;
class uscRoleRest {
    public actionToolbarId: string;
    public rolesTreeId: string;
    public pnlContentId: string;
    public windowManagerId: string;
    public windowSelRoleId: string;
    public validatorAnyNodeId: string;
    public btnExpandRolesId: string;
    public contentRowId: string;
    public multipleRoles: string;
    public requiredValidationEnabled: string;
    public expanded: string;

    public uscRoleRestEvents = UscRoleRestEventType;

    private readonly _configurationRoleSessionKey: string;

    private _actionToolbar: Telerik.Web.UI.RadToolBar;
    private _rolesTree: Telerik.Web.UI.RadTreeView;
    private _windowManager: Telerik.Web.UI.RadWindowManager;
    private _windowSelRole: Telerik.Web.UI.RadWindow;
    private _btnExpandRoles: Telerik.Web.UI.RadButton;
    private _roleService: RoleService;
    private _instanceId: string;
    private _isContentExpanded: boolean;
    private _rowContent: JQuery;
    private _initialRoleCollection: RoleModel[];

    private static DISABLED_CSSCLASS: string = "node-disabled";
    private static BOLD_CSSCLASS: string = "dsw-text-bold";

    private get _multipleRolesEnabled(): boolean
    {
        return JSON.parse(this.multipleRoles.toLowerCase());
    }

    private get _requiredValidationEnabled(): boolean {
        return JSON.parse(this.requiredValidationEnabled.toLowerCase());
    }

    private get _expanded(): boolean {
        return JSON.parse(this.expanded.toLowerCase());
    }

    private _parentPageEventHandlersDictionary:
        {
            eventType: UscRoleRestEventType,
            promiseCallback: (data: any, instanceId?: string) => JQueryPromise<any>
        } = {} as {
            eventType: UscRoleRestEventType,
            promiseCallback: (data: any, instanceId?: string) => JQueryPromise<any>
        };

    private get toolbarActions(): Array<[string, () => void]> {
        let items: Array<[string, () => void]> = [
            ["add", () => this.addRoles()],
            ["delete", () => this.removeRole()]
        ];
        return items;
    }

    constructor(serviceConfigurations: ServiceConfiguration[], configuration: UscRoleRestConfiguration, uscId: string) {
        let roleServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Role");
        this._roleService = new RoleService(roleServiceConfiguration);

        this._configurationRoleSessionKey = `${uscId}_configuration`;
        sessionStorage[this._configurationRoleSessionKey] = JSON.stringify(configuration);
    }

    public initialize(): void {
        this._actionToolbar = $find(this.actionToolbarId) as Telerik.Web.UI.RadToolBar;
        this._rolesTree = $find(this.rolesTreeId) as Telerik.Web.UI.RadTreeView;
        this._windowManager = $find(this.windowManagerId) as Telerik.Web.UI.RadWindowManager;
        this._windowSelRole = $find(this.windowSelRoleId) as Telerik.Web.UI.RadWindow;
        this._rowContent = $("#".concat(this.contentRowId));
        this._rowContent.show();
        this._isContentExpanded = true;
        this._btnExpandRoles = <Telerik.Web.UI.RadButton>$find(this.btnExpandRolesId);

        this.bindControlsEvents();
        $(`#${this.pnlContentId}`).data(this);
    }

    /**
     * Registers the given callback function as handler for the given event
     * @param eventType
     * @param callback
     */
    public registerEventHandler = (eventType: UscRoleRestEventType, callback: (data: any, instanceId?: string) => JQueryPromise<any>, instanceId?: string): void => {
        this._instanceId = instanceId;
        this._parentPageEventHandlersDictionary[eventType] = callback;
    }

    /**
     * Builds the role tree view based on the given role model collection
     * @param roleCollection
     */
    public renderRolesTree = (roleCollection: RoleModel[]): void => {
        this._initialRoleCollection = roleCollection ? [...roleCollection] : [];
        this.enableValidators(roleCollection.length === 0 && this._requiredValidationEnabled ? true : false);
        this.populateRolesTreeView(roleCollection, true, this._expanded);
    }

    /**
     * Displays tree view validation error
     * */
    private enableValidators = (state: boolean) => {
        ValidatorEnable($get(this.validatorAnyNodeId), state);
    }

    /**
     * Returns the configuration object from session storage 
    */
    private getConfiguration(): UscRoleRestConfiguration {
        let configuration: UscRoleRestConfiguration = {} as UscRoleRestConfiguration;
        if (sessionStorage[this._configurationRoleSessionKey]) {
            configuration = JSON.parse(sessionStorage[this._configurationRoleSessionKey]);
        }
        return configuration;
    }

    /**
     * Populates roles tree view with given roles
     * @param roleTreeModels
     */
    private populateRolesTreeView(rolesCollection: RoleModel[], clearTree: boolean, isExpanded: boolean): void {
        let configuration: UscRoleRestConfiguration = this.getConfiguration();

        let treeViewContainsNodes: boolean = this._rolesTree.get_allNodes().length > 0;
        if (clearTree && treeViewContainsNodes) {
            this._rolesTree.get_nodes().clear();
        }

        rolesCollection.forEach(roleModel => {
            this.buildTreeViewRecursive(roleModel, configuration.isReadOnlyMode, isExpanded);
        });
    }

    /**
     * Creates and returns a RadTreeNode based on the given role model
     * @param roleModel
     * @param isReadOnlyMode
     */
    private createTreeNodeFromRoleModel(roleModel: RoleModel, isReadOnlyMode: boolean, isExpanded: boolean): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        let treeNodeDescription: string = isReadOnlyMode && roleModel.ActiveFrom ? `${roleModel.Name} - autorizzato il ${roleModel.ActiveFrom}` : `${roleModel.Name}`;
        let treeNodeImageUrl: string = roleModel.IdRoleFather === null ? ImageHelper.roleRootNodeImageUrl : ImageHelper.roleChildNodeImageUrl;

        treeNode.set_text(treeNodeDescription);
        treeNode.set_value(`${roleModel.IdRole}`);
        treeNode.set_imageUrl(treeNodeImageUrl);
        treeNode.set_contentCssClass(roleModel.IsActive ? uscRoleRest.BOLD_CSSCLASS : uscRoleRest.DISABLED_CSSCLASS);
        treeNode.set_expanded(isExpanded);

        return treeNode;
    }

    /**
     * Sets tree node style as bolded if included in initial role database collection
     * @param treeNode
     */
    private setNodeAsBoldIfFromInitialCollection(treeNode: Telerik.Web.UI.RadTreeNode): void {
        let roleIsFromDatabase: boolean = this._initialRoleCollection.some(roleModel => roleModel.IdRole === +treeNode.get_value());
        treeNode.set_contentCssClass(roleIsFromDatabase ? uscRoleRest.BOLD_CSSCLASS : uscRoleRest.DISABLED_CSSCLASS);
    }

    /**
     * Creates the tree view, fetching missing role parents
     * @param roleModel
     * @param isReadOnlyMode
     */
    private buildTreeViewRecursive(roleModel: RoleModel, isReadOnlyMode: boolean, isExpanded: boolean): JQueryPromise<Telerik.Web.UI.RadTreeNode> {
        let promise: JQueryDeferred<Telerik.Web.UI.RadTreeNode> = $.Deferred<Telerik.Web.UI.RadTreeNode>();

        // Checks if current role already exists in the tree view
        let currentNodeFromTree: Telerik.Web.UI.RadTreeNode = this._rolesTree.get_allNodes().filter(node => +node.get_value() === roleModel.IdRole)[0];

        if (currentNodeFromTree) {
            currentNodeFromTree.set_expanded(isExpanded);
            this.setNodeAsBoldIfFromInitialCollection(currentNodeFromTree);
            return promise.resolve(currentNodeFromTree);
        }

        let currentTreeNode: Telerik.Web.UI.RadTreeNode = this.createTreeNodeFromRoleModel(roleModel, isReadOnlyMode, isExpanded);

        if (roleModel.IdRoleFather) {
            this._roleService.findRole(roleModel.IdRoleFather, (parentRole: RoleModel) => {
                this.buildTreeViewRecursive(parentRole, isReadOnlyMode, isExpanded)
                    .then((parentNode: Telerik.Web.UI.RadTreeNode) => {
                        let parentNodeFromTree: Telerik.Web.UI.RadTreeNode
                            = parentNode.get_allNodes().filter(node => +node.get_value() === +currentTreeNode.get_value())[0];

                        this.setNodeAsBoldIfFromInitialCollection(parentNode);

                        if (parentNodeFromTree) {
                            return promise.resolve(parentNodeFromTree);
                        } else {
                            parentNode.get_nodes().add(currentTreeNode);
                            parentNode.set_expanded(isExpanded);
                            return promise.resolve(currentTreeNode);
                        }
                    });
            });
        } else {
            this._rolesTree.get_nodes().add(currentTreeNode);
            promise.resolve(currentTreeNode);
        }

        return promise.promise();
    }

    /**
     *  Triggers the event listener of the clicked toolbar button
     */
    protected actionToolbar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let currentActionButtonItem: Telerik.Web.UI.RadToolBarButton = args.get_item() as Telerik.Web.UI.RadToolBarButton;
        let currentAction: () => void = this.toolbarActions.filter((item: [string, () => void]) => item[0] == currentActionButtonItem.get_commandName())
            .map((item: [string, () => void]) => item[1])[0];
        currentAction();
    }

    /**
     * Registers event handlers of UI controls
     * */
    private bindControlsEvents = (): void => {
        if(this._actionToolbar)
            this._actionToolbar.add_buttonClicked(this.actionToolbar_ButtonClicked);

        if (this._btnExpandRoles) {
            this._btnExpandRoles.addCssClass("dsw-arrow-down");
            this._btnExpandRoles.add_clicking(this.btnExpandRoles_OnClick);
        }  

        this._windowSelRole.add_close(this.bindNewRolesToTree);
    }

    /**
     * Adds new roles to role tree view
     * */
    private bindNewRolesToTree = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs): void => {
        let allSelectedRoles: RoleModel[] = args.get_argument() as RoleModel[];

        if (!allSelectedRoles || !allSelectedRoles.length)
            return;

        let currentTreeNodes: Telerik.Web.UI.RadTreeNode[] = this._rolesTree.get_allNodes();
        let newRoles: RoleModel[] = allSelectedRoles.filter(role => !currentTreeNodes.some(node => +node.get_value() === role.IdRole && node.get_contentCssClass() === uscRoleRest.BOLD_CSSCLASS));

        if (!newRoles.length)
            return;

        let parentUpdateCallback: JQueryPromise<any> = this._parentPageEventHandlersDictionary[this.uscRoleRestEvents.NewRolesAdded](newRoles, this._instanceId);

        parentUpdateCallback.then((data: any) => {
            if (this._requiredValidationEnabled)
                this.enableValidators(false);

            this._initialRoleCollection = this._multipleRolesEnabled
                    ? [...this._initialRoleCollection, ...newRoles]
                    : [newRoles[0]];
            this.populateRolesTreeView(newRoles, !this._multipleRolesEnabled, true);
        });
    }

    /**
     * Removes selected node from the tree view
     * */
    private removeRole = () => {
        let selectedNodeToDelete: Telerik.Web.UI.RadTreeNode = this._rolesTree.get_selectedNode();

        if (!selectedNodeToDelete)
            return;

        let parentDeleteCallback: JQueryPromise<any> = this._parentPageEventHandlersDictionary[this.uscRoleRestEvents.RoleDeleted](+selectedNodeToDelete.get_value(), this._instanceId);
        parentDeleteCallback.then(
            (data: any) => {
                this._initialRoleCollection = this._initialRoleCollection.filter(roleModel => roleModel.IdRole !== +selectedNodeToDelete.get_value());
                this._rolesTree.trackChanges();
                this.removeNodeFromTree(selectedNodeToDelete, selectedNodeToDelete.get_parent());
                this._rolesTree.commitChanges();

                if (this._requiredValidationEnabled && this._rolesTree.get_allNodes().length === 0) {
                    this.enableValidators(true);
                }
            });
    }

    /**
     * Removes the selected tree node and it's disabled parents from the tree view
     * @param selectedNodeToDelete
     * @param nodeToDeleteParent
     */
    private removeNodeFromTree(selectedNodeToDelete: Telerik.Web.UI.RadTreeNode, selectedNodeParent: Telerik.Web.UI.RadTreeNode): void {
        let nodeToDeleteIsRootNode: boolean = this._rolesTree.get_nodes().toArray().some(rootNode => rootNode.get_value() === selectedNodeToDelete.get_value());
        if (nodeToDeleteIsRootNode) {
            this._rolesTree.get_nodes().remove(selectedNodeToDelete);
            return;
        }

        let parentNodeChildren: Telerik.Web.UI.RadTreeNodeCollection = selectedNodeParent.get_nodes();
        parentNodeChildren.remove(selectedNodeToDelete);

        let parentHasChildrenLeft: boolean = parentNodeChildren.get_count() > 0;
        let parentIsDisabled: boolean = selectedNodeParent.get_contentCssClass() === uscRoleRest.DISABLED_CSSCLASS;

        if (!parentHasChildrenLeft && parentIsDisabled) {
            this.removeNodeFromTree(selectedNodeParent, selectedNodeParent.get_parent());
        }
    }

    /**
     * Opens roles selection window
     * */
    private addRoles = () => {
        let url: string = `../UserControl/CommonSelRoleRest.aspx?Type=Comm&MultipleRoles=${this.multipleRoles}`;
        this._windowManager.open(url, "windowSelRole", undefined);
    }

    /*
     * Expands or hides the tree view
     * */
    private btnExpandRoles_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (this._isContentExpanded) {
            this._rowContent.hide();
            this._isContentExpanded = false;
            this._btnExpandRoles.removeCssClass("dsw-arrow-down");
            this._btnExpandRoles.addCssClass("dsw-arrow-up");
        }
        else {
            this._rowContent.show();
            this._isContentExpanded = true;
            this._btnExpandRoles.removeCssClass("dsw-arrow-up");
            this._btnExpandRoles.addCssClass("dsw-arrow-down");
        }
    }

    public setToolbarVisibility(isVisible: boolean): void {
        this._actionToolbar.get_items().forEach(function (item: Telerik.Web.UI.RadToolBarItem) {
            item.set_enabled(isVisible);
        });
    }
}

export = uscRoleRest;