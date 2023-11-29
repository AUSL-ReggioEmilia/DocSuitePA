/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import RoleService = require("App/Services/Commons/RoleService");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import RoleModel = require("App/Models/Commons/RoleModel");
import ImageHelper = require("App/Helpers/ImageHelper");
import UscRoleRestEventType = require("App/Models/Commons/UscRoleRestEventType");
import UscRoleRestConfiguration = require("App/Models/Commons/UscRoleRestConfiguration");
import VisibilityType = require("App/Models/Fascicles/VisibilityType");
import DSWEnvironmentType = require("App/Models/Workflows/WorkflowDSWEnvironmentType");
import Environment = require("App/Models/Environment");
import RoleSearchFilterDTO = require("App/DTOs/RoleSearchFilterDTO");
import RoleTypologyType = require("App/Models/Commons/RoleTypologyType");

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
    public get onlyMyRoles(): boolean { return sessionStorage.getItem(`${this.pnlContentId}_onlyMyRoles`) === "true" };
    public set onlyMyRoles(value: boolean) { this.setOnlyMyRole(value); };
    public requiredValidationEnabled: string;
    public expanded: string;
    public allDataButtonEnabled: string;
    public removeAllDataButtonEnabled: string;
    public raciButtonEnabled: boolean;
    public fascicleVisibilityTypeButtonEnabled: boolean;
    public entityId: string;
    public entityType: string;
    public dswEnvironmentType: string;
    public idTenantAOO: string;
    private _persistToolbarVisibilityOnClick: boolean;
    private lblCaptionId: string;

    public uscRoleRestEvents = UscRoleRestEventType;

    private _configurationRoleSessionKey: string;
    private readonly _roleValidationSessionKey: string;
    private readonly _uscId: string;

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
    private _raciRoleCollection: RoleModel[] = <RoleModel[]>[];
    private _removedRaciRoleCollection: RoleModel[] = <RoleModel[]>[];
    private _lblCaption: JQuery;

    private static DISABLED_CSSCLASS: string = "node-disabled";
    private static BOLD_CSSCLASS: string = "dsw-text-bold";
    private static RACI_ROLE_ICON: string = "../App_Themes/DocSuite2008/imgset16/coedit.png";
    private static LOADED_EVENT: string = "onLoaded";
    private static ADD_TOOLBAR_ACTION_KEYNAME = "add";
    private static DELETE_TOOLBAR_ACTION_KEYNAME = "delete";
    private static ADD_ALL_TOOLBAR_ACTION_KEYNAME = "addAll";
    private static DELETE_ALL_TOOLBAR_ACTION_KEYNAME = "deleteAll";
    private static SET_RACI_ROLE_TOOLBAR_ACTION_KEYNAME = "setRaciRole";
    private static SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME = "setFascicleVisibilityType";
    public static NEED_ROLES_FROM_EXTERNAL_SOURCE: string = "onNeedRolesFromExternalSource";
    public static TENANT_CHANGE_EVENT: string = "onTenantChange";

    private _multipleRolesEnabled(): boolean {
        return JSON.parse(this.multipleRoles.toLowerCase());
    }

    private _requiredValidationEnabled(): boolean {
        return JSON.parse(this.requiredValidationEnabled.toLowerCase());
    }

    private _expanded(): boolean {
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

    private toolbarActions(): Array<[string, () => void]> {
        let items: Array<[string, () => void]> = [
            [uscRoleRest.ADD_TOOLBAR_ACTION_KEYNAME, () => this.addRoles()],
            [uscRoleRest.DELETE_TOOLBAR_ACTION_KEYNAME, () => this.removeRole()],
            [uscRoleRest.ADD_ALL_TOOLBAR_ACTION_KEYNAME, () => this.addAllRoles()],
            [uscRoleRest.DELETE_ALL_TOOLBAR_ACTION_KEYNAME, () => this.removeAllRoles()],
            [uscRoleRest.SET_RACI_ROLE_TOOLBAR_ACTION_KEYNAME, () => this.setRaciRole()],
            [uscRoleRest.SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME, () => this.setFascicleVisibilityType()]
        ];
        return items;
    }

    public static LOCAL_STORAGE_ROLE_REST: string = "uscRoleRest_setInitialRoleCollection";

    constructor(serviceConfigurations: ServiceConfiguration[], configuration: UscRoleRestConfiguration, uscId: string) {
        let roleServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Role");
        this._roleService = new RoleService(roleServiceConfiguration);

        this._uscId = uscId;
        this.setConfiguration(configuration);

        this._roleValidationSessionKey = `${uscId}_validationState`;
        sessionStorage.removeItem(this._roleValidationSessionKey);
    }

    public initialize(): void {
        this._actionToolbar = $find(this.actionToolbarId) as Telerik.Web.UI.RadToolBar;
        this._rolesTree = $find(this.rolesTreeId) as Telerik.Web.UI.RadTreeView;
        this._rolesTree.add_nodeClicked(this.rolesTree_onNodeClick);
        this._windowManager = $find(this.windowManagerId) as Telerik.Web.UI.RadWindowManager;
        this._windowSelRole = $find(this.windowSelRoleId) as Telerik.Web.UI.RadWindow;
        this._rowContent = $("#".concat(this.contentRowId));
        this._rowContent.show();
        this._isContentExpanded = true;
        this._btnExpandRoles = <Telerik.Web.UI.RadButton>$find(this.btnExpandRolesId);
        this._lblCaption = <JQuery>$(`#${this.lblCaptionId}`);

        this.bindControlsEvents();
        $(`#${this.pnlContentId}`).data(this);
        $(`#${this.pnlContentId}`).triggerHandler(uscRoleRest.LOADED_EVENT);

        let configuration: UscRoleRestConfiguration = this.getConfiguration();

        if (!configuration.isReadOnlyMode && this._actionToolbar) {
            this._actionToolbar.findItemByValue(uscRoleRest.ADD_ALL_TOOLBAR_ACTION_KEYNAME)
                .set_visible(this.allDataButtonEnabled.toLowerCase() === "true" ? true : false);
            this._actionToolbar.findItemByValue(uscRoleRest.DELETE_ALL_TOOLBAR_ACTION_KEYNAME)
                .set_visible(this.removeAllDataButtonEnabled.toLowerCase() === "true" ? true : false);
            this._actionToolbar.findItemByValue(uscRoleRest.SET_RACI_ROLE_TOOLBAR_ACTION_KEYNAME)
                .set_visible(this.raciButtonEnabled);
        }

        if (this._actionToolbar) {
            let btnFascicleVisibilityType = <Telerik.Web.UI.RadToolBarButton>this._actionToolbar
                .findItemByValue(uscRoleRest.SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME);
            if (btnFascicleVisibilityType) {
                btnFascicleVisibilityType.set_visible(this.fascicleVisibilityTypeButtonEnabled);
            }
        }

        $(`#${this.pnlContentId}`).on(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, (event, entityType: string, entityId: string) => {
            if (entityType && entityId) {
                this.addExternalSource(entityType, entityId);
            }
        });
        $(`#${this.pnlContentId}`).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE);
        $(`#${this.pnlContentId}`).on(uscRoleRest.TENANT_CHANGE_EVENT, (args, idTenantAOO) => {
            this.enableButtons();
            this.idTenantAOO = idTenantAOO;
        });
    }

    public countRoles(idTenantAOO: string): JQueryPromise<number> {
        let promise: JQueryDeferred<number> = $.Deferred<number>();
        this._roleService.countRoles(idTenantAOO, promise.resolve, promise.reject);
        return promise.promise();
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
        this.enableValidators(roleCollection.length === 0 && this._requiredValidationEnabled() ? true : false);
        this.populateRolesTreeView(roleCollection, true, this._expanded());
    }

    /**
     * Displays tree view validation error
     * */
    public enableValidators = (state: boolean) => {
        let behaviourValidationConfiguration: string = sessionStorage.getItem(this._roleValidationSessionKey);
        let behaviourValidationConfigurationValue: boolean = state;
        if (behaviourValidationConfiguration) {
            behaviourValidationConfigurationValue = behaviourValidationConfiguration.toLowerCase() == "true";
        }
        ValidatorEnable($get(this.validatorAnyNodeId), behaviourValidationConfigurationValue);
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
     * Set the configuration object in session storage
    */
    setConfiguration(configuration: UscRoleRestConfiguration): void {
        this._configurationRoleSessionKey = `${this._uscId}_configuration`;
        sessionStorage[this._configurationRoleSessionKey] = JSON.stringify(configuration);
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

    public clearRoleTreeView = (clearRolesModel: boolean): void => {
        this._rolesTree.get_nodes().clear();
        if (clearRolesModel) {
            this._initialRoleCollection = new Array<RoleModel>();
            localStorage.removeItem(uscRoleRest.LOCAL_STORAGE_ROLE_REST);
        }
    }

    /**
     * Creates and returns a RadTreeNode based on the given role model
     * @param roleModel
     * @param isReadOnlyMode
     */
    private createTreeNodeFromRoleModel(roleModel: RoleModel, isReadOnlyMode: boolean, isExpanded: boolean): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        let treeNodeDescription: string = `${roleModel.Name}`;
        let treeNodeImageUrl: string = this._raciRoleCollection && this._raciRoleCollection.some(x => x.IdRole === roleModel.IdRole)
            ? uscRoleRest.RACI_ROLE_ICON
            : roleModel.IdRoleFather === null || roleModel.IdRoleFather === undefined
                ? ImageHelper.roleRootNodeImageUrl
                : ImageHelper.roleChildNodeImageUrl;

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
        let currentAction: () => void = this.toolbarActions().filter((item: [string, () => void]) => item[0] == currentActionButtonItem.get_commandName())
            .map((item: [string, () => void]) => item[1])[0];
        currentAction();
    }

    /**
     * Registers event handlers of UI controls
     * */
    private bindControlsEvents = (): void => {
        if (this._actionToolbar)
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

        parentUpdateCallback.then((existedRole: RoleModel, keepChanges: boolean = false) => {
            if (this._requiredValidationEnabled())
                this.enableValidators(false);
            if (keepChanges && existedRole) {
                return;
            }
            if (existedRole && this._multipleRolesEnabled()) {
                newRoles = newRoles.filter(x => x.IdRole !== existedRole.IdRole);
            }

            this._initialRoleCollection = this._multipleRolesEnabled()
                ? [...this._initialRoleCollection, ...newRoles]
                : newRoles.length > 0
                    ? [newRoles[0]]
                    : [];
            this.populateRolesTreeView(newRoles, !this._multipleRolesEnabled(), true);
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

                if (this._requiredValidationEnabled() && this._rolesTree.get_allNodes().length === 0) {
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

        if (selectedNodeToDelete.get_allNodes().length) {
            selectedNodeToDelete.set_contentCssClass(uscRoleRest.DISABLED_CSSCLASS);
            this._actionToolbar.findItemByValue(uscRoleRest.DELETE_TOOLBAR_ACTION_KEYNAME)?.set_enabled(false);
        } else {
            parentNodeChildren.remove(selectedNodeToDelete);
        }

        let parentHasChildrenLeft: boolean = parentNodeChildren.get_count() > 0;
        let parentIsDisabled: boolean = selectedNodeParent.get_contentCssClass() === uscRoleRest.DISABLED_CSSCLASS;

        if (!parentHasChildrenLeft && parentIsDisabled) {
            this.removeNodeFromTree(selectedNodeParent, selectedNodeParent.get_parent());
        }
    }

    /**
     * Opens roles selection window
     * */
    private addRoles() {
        let url: string = `../UserControl/CommonSelRoleRest.aspx?Type=Comm&MultipleRoles=${this.multipleRoles}&OnlyMyRoles=${this.onlyMyRoles}`;
        if (this.entityType) {
            url = `${url}&EntityType=${this.entityType}`;
        }
        if (this.entityId) {
            url = `${url}&EntityId=${this.entityId}`;
        }
        if (this.dswEnvironmentType == DSWEnvironmentType[DSWEnvironmentType.Document]) {
            url = `${url}&DSWEnvironment=${this.dswEnvironmentType}`;
        }

        if (this.idTenantAOO) {
            url = `${url}&IdTenantAOO=${this.idTenantAOO}`;
        }
        if (localStorage.getItem(uscRoleRest.LOCAL_STORAGE_ROLE_REST)) {
            localStorage.removeItem(uscRoleRest.LOCAL_STORAGE_ROLE_REST);
        }
        localStorage.setItem(uscRoleRest.LOCAL_STORAGE_ROLE_REST, JSON.stringify(this._initialRoleCollection));
        this._windowManager.open(url, "windowSelRole", undefined);
    }

    /*
     * Expands or hides the tree view
     * */
    private btnExpandRoles_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
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

    public persistToolbarVisibilityOnClick = (persistVisibility: boolean): void => {
        this._persistToolbarVisibilityOnClick = persistVisibility;
    }

    /**
     * Add all roles from the tree view
     * */
    private addAllRoles = () => {
        this._windowManager.radconfirm("Sei sicuro di voler aggiungere tutti i settori?", (arg) => {
            if (arg) {
                this._parentPageEventHandlersDictionary[this.uscRoleRestEvents.AllRolesAdded]();
            }
        }, 400, 300);
    }

    /**
     * Removes all roles from the tree view
     * */
    private removeAllRoles = () => {
        this._windowManager.radconfirm("Sei sicuro di voler eliminare tutti i settori?", (arg) => {
            if (arg) {
                let deleteCallback: JQueryPromise<any> = this._parentPageEventHandlersDictionary[this.uscRoleRestEvents.AllRolesDeleted]();

                deleteCallback.then(() => this._rolesTree.get_nodes().clear());
            }
        }, 400, 300);
    }

    /**
     * Set the selected role from tree view as RACI role
     * */
    private setRaciRole = () => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rolesTree.get_selectedNode();
        if (!selectedNode) return;
        if (selectedNode.get_imageUrl() === uscRoleRest.RACI_ROLE_ICON) {
            if (selectedNode.get_level() === 0) {
                selectedNode.set_imageUrl(ImageHelper.roleRootNodeImageUrl);
            }
            else {
                selectedNode.set_imageUrl(ImageHelper.roleChildNodeImageUrl);
            }
            this._raciRoleCollection = this._raciRoleCollection.filter(x => x.IdRole !== +selectedNode.get_value());
            this._removedRaciRoleCollection.push(this._initialRoleCollection.filter(x => x.IdRole === +selectedNode.get_value())[0]);
        }
        else {
            selectedNode.set_imageUrl(uscRoleRest.RACI_ROLE_ICON);
            let selectedRole: RoleModel = this._initialRoleCollection.filter(x => x.IdRole === +selectedNode.get_value())[0];
            this._raciRoleCollection.push(selectedRole);
            this._removedRaciRoleCollection = this._removedRaciRoleCollection.filter(x => x.IdRole !== selectedRole.IdRole);
        }
    }

    private setFascicleVisibilityType = () => {
        let btnFascicleVisibilityType = <Telerik.Web.UI.RadToolBarButton>this._actionToolbar
            .findItemByValue(uscRoleRest.SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME);
        if (btnFascicleVisibilityType && btnFascicleVisibilityType.get_visible()) {
            let checked: boolean = <any>btnFascicleVisibilityType.get_checked();
            let visibilityType: VisibilityType = checked ? VisibilityType.Accessible : VisibilityType.Confidential;
            this._parentPageEventHandlersDictionary[this.uscRoleRestEvents.SetFascicleVisibilityType](visibilityType);
        }
    }

    disableButtons(): void {
        if (this._actionToolbar) {
            this._actionToolbar.get_items().forEach(function (item: Telerik.Web.UI.RadToolBarItem) {
                item.set_enabled(false);
            });
        }
    }

    enableButtons(): void {
        if (this._actionToolbar) {
            this._actionToolbar.get_items().forEach(function (item: Telerik.Web.UI.RadToolBarItem) {
                item.set_enabled(true);
            });
        }
    }

    existsRole(roles: RoleModel[]): RoleModel {
        for (let role of roles) {
            if (this.exists(role.IdRole)) {
                return role;
            }
        }
        return null;
    }

    private exists(id: number): boolean {
        return this._rolesTree.get_allNodes()
            .filter(x => x.get_contentCssClass() !== uscRoleRest.DISABLED_CSSCLASS && +x.get_value() === id).length > 0;
    }

    addExternalSource(entityType: string, entityId: string) {
        this.entityType = entityType;
        this.entityId = entityId;
    }

    forceBehaviourValidationState(state: boolean): void {
        sessionStorage[this._roleValidationSessionKey] = state;
    }

    rolesTree_onNodeClick = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        if (args.get_node().get_contentCssClass() === uscRoleRest.DISABLED_CSSCLASS) {
            this._actionToolbar.findItemByValue(uscRoleRest.SET_RACI_ROLE_TOOLBAR_ACTION_KEYNAME).disable();
            this._actionToolbar.findItemByValue(uscRoleRest.DELETE_TOOLBAR_ACTION_KEYNAME).disable();
        }
        else {
            this._actionToolbar.findItemByValue(uscRoleRest.SET_RACI_ROLE_TOOLBAR_ACTION_KEYNAME).enable();

            if (!this._persistToolbarVisibilityOnClick) {
                this._actionToolbar.findItemByValue(uscRoleRest.DELETE_TOOLBAR_ACTION_KEYNAME).enable();
            }
        }
    }

    setRaciRoles = (raciRoles: RoleModel[]) => {
        this._raciRoleCollection = raciRoles;
    }

    getRaciRoles = (): RoleModel[] => {
        return this._raciRoleCollection;
    }

    setFascicleVisibilityTypeButtonCheck = (fascicleVisibilityType: string) => {
        let btnFascicleVisibilityType = <Telerik.Web.UI.RadToolBarButton>this._actionToolbar
            .findItemByValue(uscRoleRest.SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME);
        if (btnFascicleVisibilityType && btnFascicleVisibilityType.get_visible()) {
            btnFascicleVisibilityType.set_checked(fascicleVisibilityType === VisibilityType[VisibilityType.Accessible]);
        }
    }

    setVisibilityOnFascicleVisibilityTypeButton = (fascicleVisibilityTypeButtonVisibility: boolean) => {
        let btnFascicleVisibilityType = <Telerik.Web.UI.RadToolBarButton>this._actionToolbar
            .findItemByValue(uscRoleRest.SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME);
        if (btnFascicleVisibilityType) {
            btnFascicleVisibilityType.set_visible(fascicleVisibilityTypeButtonVisibility);
        }
    }

    public setToolbarRoleVisibility(isVisible: boolean): void {
        if (this._actionToolbar) {
            if (isVisible) {
                $(`#${this.actionToolbarId}`).show();
            }
            else {
                $(`#${this.actionToolbarId}`).hide();
            }
        }
    }

    getRemovedRaciRoles = (): RoleModel[] => {
        return this._removedRaciRoleCollection;
    }

    public setCaption(caption: string): void {
        this._lblCaption.text(caption);
    }

    public setOnlyMyRole(value: boolean): void {
        sessionStorage.setItem(`${this.pnlContentId}_onlyMyRoles`, value.toString())
    }

    public setAddActionTooltip(tooltip: string): void {
        this._actionToolbar.findItemByValue(uscRoleRest.ADD_TOOLBAR_ACTION_KEYNAME)
            .set_toolTip(tooltip);
    }
}

export = uscRoleRest;