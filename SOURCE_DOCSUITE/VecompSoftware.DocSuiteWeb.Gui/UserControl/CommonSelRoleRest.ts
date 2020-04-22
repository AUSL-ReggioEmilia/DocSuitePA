import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('../App/Helpers/ServiceConfigurationHelper');
import RoleService = require('../App/Services/Commons/RoleService');
import RoleModel = require('../App/Models/Commons/RoleModel');
import ExceptionDTO = require('../App/DTOs/ExceptionDTO');
import RoleSearchFilterDTO = require('../App/DTOs/RoleSearchFilterDTO');
import ImageHelper = require('../App/Helpers/ImageHelper');
import UscErrorNotification = require("UserControl/uscErrorNotification");
import Environment = require('../App/Models/Environment');

class CommonSelRoleRest {
    public pnlMainContentId: string;
    public rolesTreeId: string;
    public descriptionSearchBtnId: string;
    public descriptionFilterInputId: string;
    public codeSearchBtnId: string;
    public codeSearchFilterInputId: string;
    public confirmSelectionBtnId: string;
    public selectAllBtnId: string;
    public unselectAllBtnId: string;
    public uscNotificationId: string;
    public ajaxLoadingPanelId: string;
    public set multipleRolesEnabled(value: string) {
        this._multipleRolesEnabled = JSON.parse(value);
    }
    public set multiTenantEnabled(value: string) {
        this._multitenantEnabled = JSON.parse(value);
    }

    private _roleService: RoleService;
    private _rolesTree: Telerik.Web.UI.RadTreeView;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _multitenantEnabled: boolean;
    private _multipleRolesEnabled: boolean;

    private static BOLD_CSSCLASS: string = "dsw-text-bold";

    private get descriptionFilterValue(): JQuery {
        return $(`#${this.descriptionFilterInputId}`);
    }

    private get codeFilterValue(): JQuery {
        return $(`#${this.codeSearchFilterInputId}`);
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let roleServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Role");
        this._roleService = new RoleService(roleServiceConfiguration);
    }


    public initialize(): void {
        this._rolesTree = $find(this.rolesTreeId) as Telerik.Web.UI.RadTreeView;
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        this.bindControlsEvents();
        this.initializeRolesTree();
        $(`#${this.pnlMainContentId}`).data(this);
    }

    private searchRoles(finderModel: RoleSearchFilterDTO): void {
        this._loadingPanel.show(this.pnlMainContentId);
        this._roleService.findRoles(finderModel,
            (successResult: RoleModel[]) => {
                this.buildRolesTreeNodes(successResult);
                this._loadingPanel.hide(this.pnlMainContentId);
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(exception);
                this._loadingPanel.hide(this.pnlMainContentId);
            });
    }

    private initializeRolesTree(): void {
        let defaultSearchRoleFinderModel: RoleSearchFilterDTO = this.getDefaultFinderModel();
        this.searchRoles(defaultSearchRoleFinderModel);
    }

    private buildRolesTreeNodes(roleModels: RoleModel[]): void {
        if (this._rolesTree.get_nodes().get_count() > 0)
            this._rolesTree.get_nodes().clear();

        // build and add tree root node
        let rootNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        rootNode.set_text("Settori");
        rootNode.set_expanded(true);
        rootNode.set_checkable(false);
        this._rolesTree.get_nodes().add(rootNode);

        // recursively build and add other tree nodes
        this.buildNodesRecursive(roleModels, rootNode);
    }

    private addNodeAttributes(roleNode: Telerik.Web.UI.RadTreeNode, roleModel: RoleModel): void {
        roleNode.get_attributes().setAttribute("IdRole", roleModel.IdRole);
        roleNode.get_attributes().setAttribute("IdRoleFather", roleModel.IdRoleFather);
        roleNode.get_attributes().setAttribute("UniqueId", roleModel.UniqueId);
        roleNode.get_attributes().setAttribute("EntityShortId", roleModel.EntityShortId ? roleModel.EntityShortId : roleModel.IdRole);
        roleNode.get_attributes().setAttribute("Name", roleModel.Name);
        roleNode.get_attributes().setAttribute("IdRoleTenant", roleModel.IdRoleTenant);
        roleNode.get_attributes().setAttribute("TenantId", roleModel.TenantId);
        roleNode.get_attributes().setAttribute("IsActive", roleModel.IsActive);
        roleNode.get_attributes().setAttribute("ServiceCode", roleModel.ServiceCode);
        roleNode.get_attributes().setAttribute("ActiveFrom", roleModel.ActiveFrom);
        roleNode.get_attributes().setAttribute("FullIncrementalPath", roleModel.FullIncrementalPath);
    }

    private getRoleModelFromNode(roleNode: Telerik.Web.UI.RadTreeNode): RoleModel {
        let roleModel: RoleModel =
        {
            IdRole: roleNode.get_attributes().getAttribute("IdRole"),
            IdRoleFather: roleNode.get_attributes().getAttribute("IdRoleFather"),
            UniqueId: roleNode.get_attributes().getAttribute("UniqueId"),
            EntityShortId: roleNode.get_attributes().getAttribute("EntityShortId"),
            Name: roleNode.get_attributes().getAttribute("Name"),
            IdRoleTenant: roleNode.get_attributes().getAttribute("IdRoleTenant"),
            TenantId: roleNode.get_attributes().getAttribute("TenantId"),
            IsActive: roleNode.get_attributes().getAttribute("IsActive"),
            ServiceCode: roleNode.get_attributes().getAttribute("ServiceCode"),
            ActiveFrom: roleNode.get_attributes().getAttribute("ActiveFrom"),
            FullIncrementalPath: roleNode.get_attributes().getAttribute("FullIncrementalPath"),
            Children: []
        };

        return roleModel;
    }

    private buildNodesRecursive(roleModels: RoleModel[], parentNode: Telerik.Web.UI.RadTreeNode): void {
        roleModels.forEach(roleModel => {
            let currentNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            this.addNodeAttributes(currentNode, roleModel);

            let currentNodeImageUrl: string = roleModel.IdRoleFather === null ? ImageHelper.roleRootNodeImageUrl : ImageHelper.roleChildNodeImageUrl;

            currentNode.set_text(roleModel.ServiceCode ? `${roleModel.Name} (${roleModel.ServiceCode})` : `${roleModel.Name}`);
            currentNode.set_value(`${roleModel.IdRole}`);
            currentNode.set_imageUrl(currentNodeImageUrl);

            parentNode.get_nodes().add(currentNode);

            if (this.hasFilters()) {
                parentNode.set_expanded(true);

                let descriptionSearchAndContains: boolean = this.descriptionFilterValue.val() !== "" && this.lowerCaseContains(roleModel.Name, this.descriptionFilterValue.val());

                if (descriptionSearchAndContains || (this.codeFilterValue.val() !== "" && roleModel.ServiceCode === this.codeFilterValue.val())) {
                    let currentNodeClass: string = currentNode.get_cssClass();
                    currentNode.set_cssClass(`${currentNodeClass} ${CommonSelRoleRest.BOLD_CSSCLASS}`);
                }

                if (roleModel.Children.length)
                    currentNode.set_expanded(true);
            }

            if (roleModel.Children.length)
                this.buildNodesRecursive(roleModel.Children, currentNode);
        });
    }

    private btnSearch_OnClick = (sender: any, args: any) => {
        sender.preventDefault();
        this.searchRolesByDescription(this.descriptionFilterValue.val());
    }

    private searchRolesByDescription(descriptionFilterValue: string): void {
        let descriptionSearchRoleFinderModel: RoleSearchFilterDTO = this.getDefaultFinderModel();
        descriptionSearchRoleFinderModel.Name = descriptionFilterValue;

        this.searchRoles(descriptionSearchRoleFinderModel);
    }

    private btnSearchCode_OnClick = (sender: any, args: any) => {
        sender.preventDefault();
        this.searchRolesByCode(this.codeFilterValue.val());
    }

    private searchRolesByCode(codeFilterValue: string): void {
        let codeSearchRoleFinderModel: RoleSearchFilterDTO = this.getDefaultFinderModel();
        codeSearchRoleFinderModel.ServiceCode = codeFilterValue;

        this._loadingPanel.show(this.pnlMainContentId);
        this._roleService.findRoles(codeSearchRoleFinderModel,
            (successResult: RoleModel[]) => {
                this.buildRolesTreeNodes(successResult);
                let filteredRole = this._rolesTree.get_allNodes().filter(node => node.get_cssClass().indexOf("dsw-text-bold") !== -1)[0];
                this.closeWindow([this.getRoleModelFromNode(filteredRole)]);
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(exception);
                this._loadingPanel.hide(this.pnlMainContentId);
            });
    }

    private btnConfirm_OnClick = (sender: any, args: any) => {
        sender.preventDefault();
        let selectedTreeNodes: Telerik.Web.UI.RadTreeNode[] = this._multipleRolesEnabled ? this._rolesTree.get_checkedNodes() : [this._rolesTree.get_selectedNode()];
        let selectedRoles: RoleModel[] = selectedTreeNodes.map(roleNode => this.getRoleModelFromNode(roleNode));

        this.closeWindow(selectedRoles);
    }

    private closeWindow(selectedRoles: RoleModel[]): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(selectedRoles);
    }

    private getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    private filterRoles(roleCollection: RoleModel[], name: string): RoleModel[] {
        let filteredRoles = roleCollection.filter(role => {
            if (role.Children.length) role.Children = this.filterRoles(role.Children, name);
            let predicate = this.lowerCaseContains(role.Name, name);
            return predicate;
        });

        return filteredRoles;
    }

    private hasFilters(): boolean {
        return this.descriptionFilterValue.val().length !== 0 || this.codeFilterValue.val().length !== 0;
    }

    private lowerCaseContains(str1: string, str2: string): boolean {
        return str1.toLowerCase().indexOf(str2.toLowerCase()) !== -1;
    }

    private checkAllNodes = (): void => {
        let rootNode: Telerik.Web.UI.RadTreeNode = this._rolesTree.get_nodes().getNode(0);
        let treeNodes: Telerik.Web.UI.RadTreeNode[] = rootNode.get_allNodes();

        treeNodes.forEach(treeNode => {
            let currentTreeNodeClass: string = treeNode.get_cssClass();

            treeNode.set_checked(true);
            treeNode.set_cssClass(`${currentTreeNodeClass} ${CommonSelRoleRest.BOLD_CSSCLASS}`);
            treeNode.set_expanded(true);
        });
    }

    private uncheckAllNodes = (): void => {
        let rootNode: Telerik.Web.UI.RadTreeNode = this._rolesTree.get_nodes().getNode(0);
        let treeNodes: Telerik.Web.UI.RadTreeNode[] = rootNode.get_allNodes();

        treeNodes.forEach(treeNode => {
            let currentTreeNodeClass: string = treeNode.get_cssClass();

            treeNode.set_checked(false);
            treeNode.set_cssClass(currentTreeNodeClass.replace("dsw-text-bold", ""));
        });
    }

    private bindControlsEvents(): void {
        $(`#${this.descriptionSearchBtnId}`).click(this.btnSearch_OnClick);
        $(`#${this.codeSearchBtnId}`).click(this.btnSearchCode_OnClick);
        $(`#${this.confirmSelectionBtnId}`).click(this.btnConfirm_OnClick);
        $(`#${this.selectAllBtnId}`).click(this.checkAllNodes);
        $(`#${this.unselectAllBtnId}`).click(this.uncheckAllNodes);
    }

    private getDefaultFinderModel(): RoleSearchFilterDTO {
        return {
            Name: null,
            ParentId: null,
            ServiceCode: null,
            TenantId: null,
            Environment: Environment.Any,
            LoadOnlyRoot: false,
            LoadOnlyMy: this._multitenantEnabled,
            LoadAlsoParent: true
        };
    }

    protected showNotificationException(exception: ExceptionDTO, customMessage?: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception) {
                uscNotification.showNotification(exception);
                return;
            }
            uscNotification.showWarningMessage(customMessage);
        }
    }
}

export = CommonSelRoleRest;