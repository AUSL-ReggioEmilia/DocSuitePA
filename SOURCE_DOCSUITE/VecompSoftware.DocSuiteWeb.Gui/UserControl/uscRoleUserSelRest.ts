import UscErrorNotification = require("UserControl/uscErrorNotification");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import RoleUserService = require('App/Services/RoleUsers/RoleUserService');
import RoleUserModel = require('App/Models/RoleUsers/RoleUserModel');
import RoleUserType = require('App/Models/RoleUsers/RoleUserType');
import Environment = require('App/Models/Environment');
import EnumHelper = require('App/Helpers/EnumHelper');

class uscRoleUserSelRest {
    pnlRoleUserSelRestId: string;
    ajaxLoadingPanelId: string;
    rddtRoleUserId: string;
    ddlEnvironmentId: string;
    rowEnvironmentId: string;
    uscNotificationId: string;
    fascicleEnabled: boolean;
    protocolEnabled: boolean;
    collaborationEnabled: boolean;
    collaborationRightsEnabled: boolean;
    roleUserTypeLabels: { [key: string]: string };
    environments: Environment[]

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rddtRoleUser: Telerik.Web.UI.RadDropDownTree;
    private _ddlEnvironment: Telerik.Web.UI.RadDropDownList;
    private _rowEnvironment: any;
    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    private _roleUserService: RoleUserService;
    private _selectedRole: string;
    private _roleUserType: string;
    private _multipleSelectionEnabled: boolean;
    private _autoExpandTreeEnabled: boolean;
    private static ROLEUSERS_TYPE_NAME = "RoleUser";
    private static USER_ATTRIBUTE = "IsUser";
    private static DROP_DOWN_LIST_HEADER_SELECTOR = ".rddtHeader";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        let roleUserConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, uscRoleUserSelRest.ROLEUSERS_TYPE_NAME);
        this._roleUserService = new RoleUserService(roleUserConfiguration);
        this._rddtRoleUser = <Telerik.Web.UI.RadDropDownTree>$find(this.rddtRoleUserId);

        if (!this.collaborationRightsEnabled) {
            this._rowEnvironment = $(`#${this.rowEnvironmentId}`);
            this._rowEnvironment.hide();
        }
        else {
            this._ddlEnvironment = <Telerik.Web.UI.RadDropDownList>$find(this.ddlEnvironmentId);
            this._ddlEnvironment.add_selectedIndexChanged(this.ddlEnvironmentOnSelectedIndexChanged)
            this.populateDropDownList();
        }
        this._rddtRoleUser.add_entryAdding(this.onEntryAdding);
        this._rddtRoleUser.add_entryAdded(this.onEntryAdded);
        this._rddtRoleUser.add_dropDownOpening(this.onDropDownOpening);
        $(`#${this.rddtRoleUserId}`).data(this);
    }

    onEntryAdding = (sender, eventArgs): void => {
        if (!this.getMultipleSelectionEnabled()) {
            sender.get_entries().clear();
        }
    };

    onEntryAdded = (sender, eventArgs): void => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = eventArgs.get_node();
        let isUser: string = selectedNode.get_attributes().getAttribute(uscRoleUserSelRest.USER_ATTRIBUTE);
        if (isUser) {
            selectedNode.set_selected(true);
            if (!this.getMultipleSelectionEnabled()) {
                sender.closeDropDown();
            }
        }
    };

    onDropDownOpening = (sender, eventArgs): void => {
        if (this.getAutoExpandTreeEnabled()) {
            let nodes: Telerik.Web.UI.RadTreeNode[] = this._rddtRoleUser.get_embeddedTree().get_allNodes();
            for (let i = 0; i < nodes.length; i++) {
                if (nodes[i].get_nodes() != null) {
                    nodes[i].expand();
                }
            }
        }
        if (this.getMultipleSelectionEnabled()) {
            $(`${uscRoleUserSelRest.DROP_DOWN_LIST_HEADER_SELECTOR}`).show();
        }
        else {
            $(`${uscRoleUserSelRest.DROP_DOWN_LIST_HEADER_SELECTOR}`).hide();
        }
    };

    confirmRoleUser = (sender, args): void => {
        this._rddtRoleUser.closeDropDown();
    }

    ddlEnvironmentOnSelectedIndexChanged = (sender: Telerik.Web.UI.RadDropDownList, eventArgs: Telerik.Web.UI.DropDownListIndexChangedEventArgs) => {
        if (sender.get_selectedItem()) {
            this.populateDropdownTree();
        }
    }

    public populateDropdownTree = (roleUniqueId?: string): void => {
        if (roleUniqueId) {
            this._selectedRole = roleUniqueId;
        }
        if (!this._selectedRole) {
            return;
        }
        this._loadingPanel.show(this.pnlRoleUserSelRestId);
        this._roleUserService.getRoleUserByRoleUniqueId(this._selectedRole, this.getSelectedEnvironment(), this.getRoleUserType(), (roleUsers: RoleUserModel[]) => {
            let elements: Telerik.Web.UI.RadTreeView = this._rddtRoleUser.get_embeddedTree();
            elements.trackChanges();
            elements.get_nodes().clear();
            this._rddtRoleUser.get_entries().clear();

            if (!this.collaborationRightsEnabled) {
                roleUsers = this.removeDuplicateUserRoles(roleUsers);
            }

            let fascicleTree: RoleUserType[] = [RoleUserType.RP, RoleUserType.SP];
            let protocolTree: RoleUserType[] = [RoleUserType.MP];
            let collaborationTree: RoleUserType[] = [RoleUserType.D, RoleUserType.V, RoleUserType.S];

            let showFascile: boolean = !this.getRoleUserType() ? true : fascicleTree.indexOf(RoleUserType[this.getRoleUserType()]) > -1
            let showProtocol: boolean = !this.getRoleUserType() ? true : protocolTree.indexOf(RoleUserType[this.getRoleUserType()]) > -1
            let showCollaboration: boolean = !this.getRoleUserType() ? true : collaborationTree.indexOf(RoleUserType[this.getRoleUserType()]) > -1
            if (this.fascicleEnabled && showFascile) {
                elements.get_nodes().add(this.CreateDefaultRoot("Fascicoli", "Fascicoli", fascicleTree));
            }
            if (this.protocolEnabled && showProtocol) {
                elements.get_nodes().add(this.CreateDefaultRoot("Protocollo", "Protocollo", protocolTree));
            }
            if (this.collaborationEnabled && showCollaboration) {
                elements.get_nodes().add(this.CreateDefaultRoot("Collaborazione", "Collaborazione", collaborationTree));
            }

            let node: Telerik.Web.UI.RadTreeNode
            for (let roleUser of roleUsers) {
                node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(roleUser.Account);
                node.set_value(roleUser.UniqueId);
                node.get_attributes().setAttribute(uscRoleUserSelRest.USER_ATTRIBUTE, JSON.stringify(roleUser));
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/user.png");
                node.set_checkable(true);
                let roleUserRoot: Telerik.Web.UI.RadTreeNode = elements.findNodeByValue(RoleUserType[roleUser.Type]);
                if (roleUserRoot) {
                    roleUserRoot.get_nodes().add(node);
                }
            }
            elements.commitChanges();
            this._loadingPanel.hide(this.pnlRoleUserSelRestId);
        }, (exception: ExceptionDTO) => {
            this.showNotificationException(this.uscNotificationId, exception);
            this._loadingPanel.hide(this.pnlRoleUserSelRestId);
        });
    }

    public populateDropDownList = (): void => {
        let item: Telerik.Web.UI.DropDownListItem;
        for (let env of this.environments) {
            item = new Telerik.Web.UI.DropDownListItem();
            item.set_text(Environment.toPublicDescription(env));
            item.set_value(Environment[env]);
            this._ddlEnvironment.get_items().add(item);
        }
        this._ddlEnvironment.getItem(0).set_selected(true);
    }

    public roleUserSelected(): boolean {
        return this._rddtRoleUser.get_selectedValue() != "";
    }

    public clearTree(): void {
        let elements: Telerik.Web.UI.RadTreeView = this._rddtRoleUser.get_embeddedTree();
        elements.trackChanges();
        elements.get_nodes().clear();
        elements.commitChanges();
    }

    public getSelectedRole(): string {
        return this._selectedRole;
    }

    public getSelectedEnvironment(): string {
        if (!this.collaborationRightsEnabled) {
            return null;
        }
        if (this._ddlEnvironment && this._ddlEnvironment.get_selectedItem() != null) {
            return this._ddlEnvironment.get_selectedItem().get_value();
        }
        return null;
    }

    public getRoleUserType = (): string => {
        return this._roleUserType;
    }

    public setRoleUserType = (roleUserType: string): void => {
        this._roleUserType = roleUserType;
    }

    public getMultipleSelectionEnabled = (): boolean => {
        if (this._multipleSelectionEnabled != undefined && this._multipleSelectionEnabled != null && this._multipleSelectionEnabled) {
            return true;
        }
        return false;
    }

    public getAutoExpandTreeEnabled = (): boolean => {
        if (this._autoExpandTreeEnabled != undefined && this._autoExpandTreeEnabled != null && this._autoExpandTreeEnabled) {
            return true;
        }
        return false;
    }

    public setMultipleSelectionEnabled = (enabled: boolean): void => {
        this._multipleSelectionEnabled = enabled;
    }

    public setAutoExpandTreeEnabled = (enabled: boolean): void => {
        this._autoExpandTreeEnabled = enabled;
    }

    public getSelectedRoleUsers = (): RoleUserModel[] => {
        let selectedNodes: Telerik.Web.UI.RadTreeNode[] = this._rddtRoleUser.get_embeddedTree().get_checkedNodes();
        let ret: RoleUserModel[] = [];
        if (!selectedNodes || selectedNodes.length == 0) {
            return ret;
        }

        for (let node of selectedNodes) {
            if (!node.get_attributes().getAttribute(uscRoleUserSelRest.USER_ATTRIBUTE)) {
                continue;
            }
            ret.push(JSON.parse(node.get_attributes().getAttribute(uscRoleUserSelRest.USER_ATTRIBUTE)));
        }
        return ret;
    }

    private removeDuplicateUserRoles(roleUsers: RoleUserModel[]): RoleUserModel[] {
        let ret: RoleUserModel[] = [];
        for (let roleUser of roleUsers) {
            let roleUserIndex: number = ret.findIndex(x => x.Account === roleUser.Account && x.Type === roleUser.Type);
            if (roleUserIndex < 0) {
                ret.push(roleUser);
            }
            else if (roleUser.EntityId > ret[roleUserIndex].EntityId) {
                ret.splice(roleUserIndex, 1);
                ret.push(roleUser);
            }
        }
        return ret;
    }

    private CreateDefaultRoot(text: string, value: string, roleUserType: RoleUserType[]): Telerik.Web.UI.RadTreeNode {
        let root: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        root.set_text(text);
        root.set_value(value);
        root.set_checkable(false);

        let node: Telerik.Web.UI.RadTreeNode;
        for (let i = 0; i < roleUserType.length; i++) {
            if (this.getRoleUserType() && roleUserType[i] !== RoleUserType[this.getRoleUserType()]) {
                continue;
            }
            node = new Telerik.Web.UI.RadTreeNode();
            let description: string = this.roleUserTypeLabels[RoleUserType[roleUserType[i]]];
            if (!description || description.length == 0) {
                description = this._enumHelper.getRoleUserTypeDescription(RoleUserType[roleUserType[i]]);
            }
            node.set_text(description);
            node.set_value(roleUserType[i]);
            node.set_imageUrl("../Comm/images/Interop/Ruolo.gif");
            node.set_checkable(false);
            root.get_nodes().add(node);
        }
        return root;
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }
}

export = uscRoleUserSelRest;