import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import RoleService = require('App/Services/Commons/RoleService');
import RoleModel = require('App/Models/Commons/RoleModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import RoleSearchFilterDTO = require('App/DTOs/RoleSearchFilterDTO');
import ImageHelper = require('App/Helpers/ImageHelper');
import UscErrorNotification = require("UserControl/uscErrorNotification");
import Environment = require('App/Models/Environment');
import ProcessService = require('App/Services/Processes/ProcessService');
import DossierFolderService = require('App/Services/Dossiers/DossierFolderService');
import ProcessModel = require('App/Models/Processes/ProcessModel');
import ExternalSourceActionEnum = require('App/Helpers/ExternalSourceActionEnum');
import CategoryFascicleViewModel = require('App/ViewModels/Commons/CategoryFascicleViewModel');
import CategoryService = require('App/Services/Commons/CategoryService');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import uscRoleRest = require('UserControl/uscRoleRest');
import GenericHelper = require('App/Helpers/GenericHelper');
import DSWEnvironmentType = require('App/Models/Workflows/WorkflowDSWEnvironmentType');
import RoleTypologyType = require('App/Models/Commons/RoleTypologyType');

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
    public onlyMyRoles: boolean;
    public idTenantAOO: string;
    private _initialRoleCollection: RoleModel[] = [];
    public entityType: string;
    public entityId: string;
    private allProcessRoles: RoleModel[];
    private allDossierFolderRoles: RoleModel[];
    private allCategoryRoles: RoleModel[];
    categoryModel: CategoryModel;
    private _savedRoles: RoleModel[];

    private _getRolesFromExternalSourceActionsDictionary:
        {
            externalSourceActionType: ExternalSourceActionEnum,
            promiseCallback: (data: any, instanceId?: string) => JQueryPromise<any>
        } = {} as {
            externalSourceActionType: ExternalSourceActionEnum,
            promiseCallback: (data: any, instanceId?: string) => JQueryPromise<any>
        };

    public set multipleRolesEnabled(value: string) {
        this._multipleRolesEnabled = JSON.parse(value);
    }

    private _categoryService: CategoryService;
    private _roleService: RoleService;
    private _processService: ProcessService;
    private _dossierFolderService: DossierFolderService;
    private _rolesTree: Telerik.Web.UI.RadTreeView;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _multipleRolesEnabled: boolean;
    private _btnConfirm: Telerik.Web.UI.RadButton;

    private static BOLD_CSSCLASS: string = "dsw-text-bold";
    private static BLUE_NODE_CSSCLASS: string = "node-tree-fascicle";

    private descriptionFilterValue(): JQuery {
        return $(`#${this.descriptionFilterInputId}`);
    }

    private codeFilterValue(): JQuery {
        return $(`#${this.codeSearchFilterInputId}`);
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let roleServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Role");
        this._roleService = new RoleService(roleServiceConfiguration);
        let processConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Process");
        this._processService = new ProcessService(processConfiguration);
        let dossierFolderConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DossierFolder");
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
        let categoryServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
        this._categoryService = new CategoryService(categoryServiceConfiguration);
    }


    public initialize(): void {
        this._rolesTree = $find(this.rolesTreeId) as Telerik.Web.UI.RadTreeView;
        this._rolesTree.add_nodeClicking(this.rolesTree_onNodeClicking);
        this._rolesTree.add_nodeClicked(this.rolesTree_onNodeClicked);
        this._rolesTree.add_nodeChecked(this.rolesTree_onNodeChecked);

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.confirmSelectionBtnId);
        this._btnConfirm.set_enabled(false);

        this.registerGetRolesFromExternalSourceActions();

        this.bindControlsEvents();
        this._savedRoles = [];
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
        let dswEnvironment: string = GenericHelper.getUrlParams(window.location.href, "DSWEnvironment");
        let defaultSearchRoleFinderModel: RoleSearchFilterDTO = this.getDefaultFinderModel();
        if (dswEnvironment && dswEnvironment == DSWEnvironmentType[DSWEnvironmentType.Document]) {
            defaultSearchRoleFinderModel = this.getDossierDefaultFinderModel();
        } else {
            defaultSearchRoleFinderModel = this.getDefaultFinderModel();
        }
        if (localStorage.getItem(uscRoleRest.LOCAL_STORAGE_ROLE_REST) && localStorage.getItem(uscRoleRest.LOCAL_STORAGE_ROLE_REST) != "undefined") {
            this._savedRoles = JSON.parse(localStorage.getItem(uscRoleRest.LOCAL_STORAGE_ROLE_REST));
        }
        if (this.entityType) {
            let externalSourceAction = this._getRolesFromExternalSourceActionsDictionary[this.entityType];
            if (externalSourceAction) {
                externalSourceAction(this.entityId);
            }
        }
        else {
            this.searchRoles(defaultSearchRoleFinderModel);
        }
    }

    private buildRolesTreeNodes(roleModels: RoleModel[]): void {
        let rootNode: Telerik.Web.UI.RadTreeNode = this.createRootNode();
        // recursively build and add other tree nodes
        this.buildNodesRecursive(roleModels, rootNode);
    }

    private addNodeAttributes(roleNode: Telerik.Web.UI.RadTreeNode, roleModel: RoleModel): void {
        roleNode.get_attributes().setAttribute("IdRole", roleModel.IdRole);
        roleNode.get_attributes().setAttribute("IdRoleFather", roleModel.IdRoleFather);
        roleNode.get_attributes().setAttribute("UniqueId", roleModel.UniqueId);
        roleNode.get_attributes().setAttribute("EntityShortId", roleModel.EntityShortId ? roleModel.EntityShortId : roleModel.IdRole);
        roleNode.get_attributes().setAttribute("Name", roleModel.Name);
        roleNode.get_attributes().setAttribute("IdTenantAOO", roleModel.IdTenantAOO);
        roleNode.get_attributes().setAttribute("IsActive", roleModel.IsActive);
        roleNode.get_attributes().setAttribute("ServiceCode", roleModel.ServiceCode);
        roleNode.get_attributes().setAttribute("FullIncrementalPath", roleModel.FullIncrementalPath);
        roleNode.get_attributes().setAttribute("RoleTypology", roleModel.RoleTypology);
        let isSelectable: boolean = true;
        if (roleModel.IsRealResult != undefined && roleModel.IsRealResult != null) {
            roleNode.get_attributes().setAttribute("IsRealResult", roleModel.IsRealResult);
            isSelectable = roleModel.IsRealResult;
        }
        roleNode.get_attributes().setAttribute("IsSelectable", isSelectable);
    }

    private getRoleModelFromNode(roleNode: Telerik.Web.UI.RadTreeNode): RoleModel {
        let roleModel: RoleModel =
        {
            IdRole: roleNode.get_attributes().getAttribute("IdRole"),
            IdRoleFather: roleNode.get_attributes().getAttribute("IdRoleFather"),
            UniqueId: roleNode.get_attributes().getAttribute("UniqueId"),
            EntityShortId: roleNode.get_attributes().getAttribute("EntityShortId"),
            Name: roleNode.get_attributes().getAttribute("Name"),
            IdTenantAOO: roleNode.get_attributes().getAttribute("IdTenantAOO"),
            IsActive: roleNode.get_attributes().getAttribute("IsActive"),
            ServiceCode: roleNode.get_attributes().getAttribute("ServiceCode"),
            FullIncrementalPath: roleNode.get_attributes().getAttribute("FullIncrementalPath"),
            RoleTypology: roleNode.get_attributes().getAttribute("RoleTypology"),
            Children: [],
            CategoryFascicleRights: [],
            IsRealResult: roleNode.get_attributes().getAttribute("IsRealResult") ? true : false
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

                let descriptionSearchAndContains: boolean = this.descriptionFilterValue().val() !== "" && this.lowerCaseContains(roleModel.Name, this.descriptionFilterValue().val());
                let codeFilterValueToLower: string = this.codeFilterValue().val().toLowerCase();

                if (descriptionSearchAndContains || (codeFilterValueToLower !== "" && roleModel.ServiceCode && roleModel.ServiceCode.toLowerCase() === codeFilterValueToLower)) {
                    let currentNodeClass: string = currentNode.get_cssClass();
                    currentNode.set_cssClass(`${currentNodeClass} ${CommonSelRoleRest.BOLD_CSSCLASS}`);
                }

                if (roleModel.Children.length)
                    currentNode.set_expanded(true);
            }

            if (roleModel.IsRealResult != undefined && roleModel.IsRealResult != null) {
                let currentNodeClass: string = currentNode.get_cssClass();
                if (roleModel.IsRealResult) {
                    currentNode.set_cssClass(`${currentNodeClass} ${CommonSelRoleRest.BOLD_CSSCLASS}`);
                }
                else {
                    currentNode.set_cssClass(`${currentNodeClass} node-disabled`);
                    currentNode.get_attributes().setAttribute("isReadOnly", true);
                    currentNode.set_checkable(false);
                }
            }

            if (roleModel.Children.length)
                this.buildNodesRecursive(roleModel.Children, currentNode);
        });
    }

    private btnSearch_OnClick = (sender: any, args: any) => {
        sender.preventDefault();
        if (this.entityType) {
            this.searchExternalRolesRecursive(this.descriptionFilterValue().val());
        }
        else {
            this.searchRolesByDescription(this.descriptionFilterValue().val());
        }
    }

    private searchRolesByDescription(descriptionFilterValue: string): void {
        let descriptionSearchRoleFinderModel: RoleSearchFilterDTO = this.getDefaultFinderModel();
        descriptionSearchRoleFinderModel.Name = descriptionFilterValue;

        this.searchRoles(descriptionSearchRoleFinderModel);
    }

    private btnSearchCode_OnClick = (sender: any, args: any) => {
        sender.preventDefault();
        this.searchRolesByCode(this.codeFilterValue().val());
    }

    private searchRolesByCode(codeFilterValue: string): void {
        let codeSearchRoleFinderModel: RoleSearchFilterDTO = this.getDefaultFinderModel();
        codeSearchRoleFinderModel.ServiceCode = codeFilterValue;

        if (this.entityType) {
            let filteredRolesByCode: RoleModel[] = [];
            switch (+this.entityType) {
                case ExternalSourceActionEnum.Process: {
                    filteredRolesByCode = this.allProcessRoles.filter(x => x.ServiceCode.toLowerCase() === codeSearchRoleFinderModel.ServiceCode.toLowerCase());
                    break;
                }
                case ExternalSourceActionEnum.DossierFolder: {
                    filteredRolesByCode = this.allDossierFolderRoles.filter(x => x.ServiceCode.toLowerCase() === codeSearchRoleFinderModel.ServiceCode.toLowerCase());
                    break;
                }
                case ExternalSourceActionEnum.Category: {
                    filteredRolesByCode = this.allCategoryRoles.filter(x => x.ServiceCode.toLowerCase() === codeSearchRoleFinderModel.ServiceCode.toLowerCase());
                    break;
                }
            }

            this.addSelectedRoleByCode(filteredRolesByCode);
        }
        else {
            this._loadingPanel.show(this.pnlMainContentId);
            this._roleService.findRoles(codeSearchRoleFinderModel,
                (successResult: RoleModel[]) => {
                    this._loadingPanel.hide(this.pnlMainContentId);
                    this.addSelectedRoleByCode(successResult);
                },
                (exception: ExceptionDTO) => {
                    this.showNotificationException(exception);
                    this._loadingPanel.hide(this.pnlMainContentId);
                });
        }
    }

    addSelectedRoleByCode(roles: RoleModel[]) {

        if (roles.length > 1) {
            this.showNotificationException(null, "Il codice cercato non è univoco");
            return;
        }

        if (roles.length === 0) {
            this.showNotificationException(null, "Il codice cercato è inesistente");
            return;
        }

        this.buildRolesTreeNodes(roles);
        let filteredRole = this._rolesTree.get_allNodes().filter(node => node.get_cssClass().indexOf("dsw-text-bold") !== -1)[0];
        this.closeWindow([this.getRoleModelFromNode(filteredRole)]);
    }

    private btnConfirm_OnClick = (sender: any, args: any) => {
        let selectedTreeNodes: Telerik.Web.UI.RadTreeNode[] = this._multipleRolesEnabled
            ? this._rolesTree.get_checkedNodes()
            : this._rolesTree.get_selectedNode() ? [this._rolesTree.get_selectedNode()] : [];

        if (!selectedTreeNodes.length) {
            this.showNotificationException(null, "Nessun settore selezionato");
            return;
        }

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
        return this.descriptionFilterValue().val().length !== 0 || this.codeFilterValue().val().length !== 0;
    }

    private lowerCaseContains(str1: string, str2: string): boolean {
        return str1.toLowerCase().indexOf(str2.toLowerCase()) !== -1;
    }

    private checkAllNodes = (): void => {
        let rootNode: Telerik.Web.UI.RadTreeNode = this._rolesTree.get_nodes().getNode(0);
        let treeNodes: Telerik.Web.UI.RadTreeNode[] = rootNode.get_allNodes();

        treeNodes.forEach(treeNode => {
            const treeNodeIsNotCheckable: boolean = treeNode.get_attributes().getAttribute("isReadOnly") || !treeNode.get_checkable();
            if (treeNodeIsNotCheckable) {
                return;
            }

            let currentTreeNodeClass: string = treeNode.get_cssClass();

            treeNode.set_checked(true);
            treeNode.set_cssClass(`${currentTreeNodeClass} ${CommonSelRoleRest.BOLD_CSSCLASS}`);

            this._expandAllParentNodes(treeNode);
        });

        this._btnConfirm.set_enabled(true);
    }

    private _expandAllParentNodes(currentNode: Telerik.Web.UI.RadTreeNode): void {
        if (currentNode.get_expanded()) {
            return;
        }

        currentNode.set_expanded(true);
        let currentParentNode: Telerik.Web.UI.RadTreeNode = currentNode.get_parent();

        if (!currentParentNode) {
            return;
        }

        this._expandAllParentNodes(currentParentNode);
    }

    private uncheckAllNodes = (): void => {
        let rootNode: Telerik.Web.UI.RadTreeNode = this._rolesTree.get_nodes().getNode(0);
        let treeNodes: Telerik.Web.UI.RadTreeNode[] = rootNode.get_allNodes();

        treeNodes.forEach(treeNode => {
            let currentTreeNodeClass: string = treeNode.get_cssClass();

            treeNode.set_checked(false);
            treeNode.set_cssClass(currentTreeNodeClass.replace("dsw-text-bold", ""));
        });

        this._btnConfirm.set_enabled(false);
    }

    private bindControlsEvents(): void {
        $(`#${this.descriptionSearchBtnId}`).click(this.btnSearch_OnClick);
        $(`#${this.codeSearchBtnId}`).click(this.btnSearchCode_OnClick);
        this._btnConfirm.add_clicked(this.btnConfirm_OnClick);

        $(`#${this.selectAllBtnId}`).click(this.checkAllNodes);
        $(`#${this.unselectAllBtnId}`).click(this.uncheckAllNodes);
    }

    private getDefaultFinderModel(): RoleSearchFilterDTO {
        return {
            Name: null,
            ParentId: null,
            ServiceCode: null,
            IdTenantAOO: this.idTenantAOO ? this.idTenantAOO : null,
            Environment: Environment.Any,
            LoadOnlyRoot: false,
            LoadOnlyMy: this.onlyMyRoles,
            LoadAlsoParent: true,
            RoleTypology: null
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

    private showNotificationMessage(customMessage: string): void {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    private registerGetRolesFromExternalSourceActions(): void {
        this._getRolesFromExternalSourceActionsDictionary[ExternalSourceActionEnum.Process] = (processId: string): void => {
            if (processId) {
                this._processService.getById(processId, (data) => {
                    let process: ProcessModel = data;
                    this.allProcessRoles = process.Roles;
                    this.renderRolesTree(process.Roles);
                }, (exception: ExceptionDTO) => {
                    this.showNotificationException(exception);
                });
            }
        }

        this._getRolesFromExternalSourceActionsDictionary[ExternalSourceActionEnum.Category] = (categoryId: number): void => {
            if (categoryId) {
                let searchFilter: RoleSearchFilterDTO = this.getDefaultFinderModel();
                searchFilter.IdCategory = categoryId;
                this._roleService.findRoles(searchFilter, (data: RoleModel[]) => {
                    this.allCategoryRoles = data;
                    this.buildRolesTreeNodes(data);
                }, (exception: ExceptionDTO) => {
                    this.showNotificationException(exception);
                });
            }
        }

        this._getRolesFromExternalSourceActionsDictionary[ExternalSourceActionEnum.DossierFolder] = (dossierFolderId: string): void => {
            let searchFilter: RoleSearchFilterDTO = this.getDefaultFinderModel();
            searchFilter.IdDossierFolder = dossierFolderId;
            this._roleService.findRoles(searchFilter, (data: RoleModel[]) => {
                this.allDossierFolderRoles = data;
                this.buildRolesTreeNodes(data);
            }, (exception: ExceptionDTO) => {
                this.showNotificationException(exception);
            });
        }
    }

    public renderRolesTree = (roleCollection: RoleModel[], isExpanded: boolean = false): void => {
        this._initialRoleCollection = roleCollection ? [...roleCollection] : [];
        this.populateRolesTreeView(roleCollection, true, isExpanded);
    }

    private populateRolesTreeView(rolesCollection: RoleModel[], clearTree: boolean, isExpanded: boolean): void {
        let rootNode: Telerik.Web.UI.RadTreeNode = this.createRootNode();

        let treeViewContainsNodes: boolean = this._rolesTree.get_allNodes().length > 0;
        if (clearTree && treeViewContainsNodes) {
            this._rolesTree.get_nodes().getNode(0).get_nodes().clear();
        }

        rolesCollection.forEach(roleModel => {
            this.buildTreeViewRecursive(roleModel, isExpanded);
        });
    }

    private buildTreeViewRecursive(roleModel: RoleModel, isExpanded: boolean): JQueryPromise<Telerik.Web.UI.RadTreeNode> {
        let promise: JQueryDeferred<Telerik.Web.UI.RadTreeNode> = $.Deferred<Telerik.Web.UI.RadTreeNode>();

        let currentNodeFromTree: Telerik.Web.UI.RadTreeNode = this._rolesTree.get_allNodes().filter(node => +node.get_value() === roleModel.IdRole)[0];

        if (currentNodeFromTree) {
            currentNodeFromTree.set_expanded(isExpanded);
            this.setNodeAsBoldIfFromInitialCollection(currentNodeFromTree);
            return promise.resolve(currentNodeFromTree);
        }

        let currentTreeNode: Telerik.Web.UI.RadTreeNode = this.createTreeNodeFromRoleModel(roleModel, isExpanded);
        this.addNodeAttributes(currentTreeNode, roleModel);

        if (roleModel.IdRoleFather) {
            this._roleService.findRole(roleModel.IdRoleFather, (parentRole: RoleModel) => {
                this.buildTreeViewRecursive(parentRole, isExpanded)
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
            this._rolesTree.get_nodes().getNode(0).get_nodes().add(currentTreeNode);
            promise.resolve(currentTreeNode);
        }

        return promise.promise();
    }

    private setNodeAsBoldIfFromInitialCollection(treeNode: Telerik.Web.UI.RadTreeNode): void {
        if (this._savedRoles.some(x => x.IdRole === +treeNode.get_value())) {
            return;
        }

        let roleIsFromDatabase: boolean = this._initialRoleCollection.some(roleModel => roleModel.IdRole === +treeNode.get_value());
        treeNode.set_contentCssClass(roleIsFromDatabase ? "dsw-text-bold" : "node-disabled");
        treeNode.get_attributes().setAttribute("isReadOnly", !roleIsFromDatabase);
        if (!roleIsFromDatabase) {
            treeNode.set_checkable(false);
        }
    }

    private createTreeNodeFromRoleModel(roleModel: RoleModel, isExpanded: boolean): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        let treeNodeDescription: string = `${roleModel.Name}`;
        let treeNodeImageUrl: string = roleModel.IdRoleFather === null ? ImageHelper.roleRootNodeImageUrl : ImageHelper.roleChildNodeImageUrl;

        treeNode.set_text(treeNodeDescription);
        treeNode.set_value(`${roleModel.IdRole}`);
        treeNode.set_imageUrl(treeNodeImageUrl);
        treeNode.set_contentCssClass(roleModel.IsActive ? "dsw-text-bold" : "node-disabled");
        treeNode.set_expanded(isExpanded);
        treeNode.get_attributes().setAttribute("isReadOnly", !roleModel.IsActive);
        if (!roleModel.IsActive) {
            treeNode.set_checkable(false);
        }

        if (this._savedRoles.some(x => x.IdRole === roleModel.IdRole)) {
            treeNode.set_contentCssClass(CommonSelRoleRest.BLUE_NODE_CSSCLASS);
            treeNode.get_attributes().setAttribute("isReadOnly", true);
            treeNode.set_checkable(false);
        }

        return treeNode;
    }

    private getDossierDefaultFinderModel(): RoleSearchFilterDTO {
        return {
            Name: null,
            ParentId: null,
            ServiceCode: null,
            IdTenantAOO: this.idTenantAOO ? this.idTenantAOO : null,
            Environment: Environment.Document,
            LoadOnlyRoot: false,
            LoadOnlyMy: this.onlyMyRoles,
            LoadAlsoParent: true,
            RoleTypology: null
        };
    }

    createRootNode(): Telerik.Web.UI.RadTreeNode {
        if (this._rolesTree.get_nodes().get_count() > 0)
            this._rolesTree.get_nodes().clear();

        // build and add tree root node
        let rootNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        rootNode.set_text("Settori");
        rootNode.set_expanded(true);
        rootNode.set_checkable(false);
        this._rolesTree.get_nodes().add(rootNode);

        return rootNode;
    }

    searchExternalRolesRecursive(searchText: string): void {
        let filteredRoles: RoleModel[] = [];
        switch (this.entityType) {
            case ExternalSourceActionEnum.Process.toString(): {
                filteredRoles = this.allProcessRoles.filter(x => x.Name.toLowerCase().indexOf(searchText.toLowerCase()) !== -1);
                break;
            }
            case ExternalSourceActionEnum.DossierFolder.toString(): {
                filteredRoles = this.allDossierFolderRoles.filter(x => x.Name.toLowerCase().indexOf(searchText.toLowerCase()) !== -1);
                break;
            }
            case ExternalSourceActionEnum.Category.toString(): {
                filteredRoles = this.allCategoryRoles.filter(x => x.Name.toLowerCase().indexOf(searchText.toLowerCase()) !== -1);
                break;
            }
        }
        this.renderRolesTree(filteredRoles, true);
    }

    rolesTree_onNodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let clickedNode: Telerik.Web.UI.RadTreeNode = args.get_node();
        this._btnConfirm.set_enabled(clickedNode.get_value() && clickedNode.get_attributes().getAttribute("IsSelectable"));
    }

    rolesTree_onNodeChecked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let checkedNodes: Telerik.Web.UI.RadTreeNode[] = this._rolesTree.get_checkedNodes();
        this._btnConfirm.set_enabled(checkedNodes.length > 0);
    }

    rolesTree_onNodeClicking = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        const clickedNode: Telerik.Web.UI.RadTreeNode = args.get_node();

        if (clickedNode.get_attributes().getAttribute("isReadOnly")) {
            args.set_cancel(true);
        }
        else {
            clickedNode.check();
        }
    }
}

export = CommonSelRoleRest;