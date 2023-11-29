/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/core/extensions/number.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import CategorySearchFilterDTO = require("App/DTOs/CategorySearchFilterDTO");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import CategoryService = require("App/Services/Commons/CategoryService");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import CategoryTreeViewModel = require("App/ViewModels/Commons/CategoryTreeViewModel");
import UscErrorNotification = require("UserControl/uscErrorNotification");
import FascicleType = require("App/Models/Fascicles/FascicleType");
import ProcessService = require("App/Services/Processes/ProcessService");
import ProcessModel = require("App/Models/Processes/ProcessModel");
import ProcessNodeType = require('App/Models/Processes/ProcessNodeType');
import DossierFolderService = require("App/Services/Dossiers/DossierFolderService");
import DossierSummaryFolderViewModelMapper = require("App/Mappers/Dossiers/DossierSummaryFolderViewModelMapper");
import DossierSummaryFolderViewModel = require("App/ViewModels/Dossiers/DossierSummaryFolderViewModel");
import ProcessFascicleTemplateService = require("App/Services/Processes/ProcessFascicleTemplateService");
import ProcessFascicleTemplateModel = require("App/Models/Processes/ProcessFascicleTemplateModel");
import TreeNodeModel = require("App/ViewModels/Commons/TreeNodeModel");
import TbltProcess = require("Tblt/TbltProcess");
import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");
import PageClassHelper = require("App/Helpers/PageClassHelper");

class CommonSelCategoryRest {

    treeViewCategoryId: string;
    txtSearchId: string;
    txtSearchCodeId: string;
    btnSearchOnlyFascicolableId: string;
    btnSearchId: string;
    btnSearchCodeId: string;
    btnConfermaId: string;
    ajaxLoadingPanelId: string;
    pnlMainContentId: string;
    uscNotificationId: string;
    rowOnlyFascicolableId: string;
    fascicleBehavioursEnabled: boolean;
    manager: string;
    secretary: string;
    fascicleType?: FascicleType;
    role?: number;
    container?: number;
    lblDescriptionId: string;
    pnlDescriptionId: string;
    includeParentDescendants?: boolean;
    parentId?: number;
    currentTenantAOOId: string;
    showProcesses: boolean;
    showProcessFascicleTemplate: boolean;
    processNodeSelectable: boolean;
    isProcessActive: boolean

    private _categoryService: CategoryService;
    private _processService: ProcessService;
    private _dossierFolderService: DossierFolderService;
    private _processFascicleTemplateService: ProcessFascicleTemplateService;
    private _treeViewCategory: Telerik.Web.UI.RadTreeView;
    private _btnSearchOnlyFascicolable: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    private static CATEGORYFULLCODE_MAXLENGTH = 4;
    private static NODETYPE_ATTRNAME: string = "NodeType";
    private static NODE_CHILDREN_LOADED_ATTRIBUTE: string = "ChildrenLoaded";

    public static PROCESSES_AND_FOLDERS_TEXT = "Serie e volumi";
    public static NO_ITEMS_FOUND_TEXT = "Nessun elemento trovato";

    get rootNode(): Telerik.Web.UI.RadTreeNode {
        return this._treeViewCategory.get_nodes().getNode(0);
    }

    get txtSearch(): JQuery {
        return $(`#${this.txtSearchId}`);
    }

    get txtSearchCode(): JQuery {
        return $(`#${this.txtSearchCodeId}`);
    }

    get lblDescription(): JQuery {
        return $(`#${this.lblDescriptionId}`);
    }

    get cachedCategories(): CategoryTreeViewModel[] {
        let categories: CategoryTreeViewModel[] = [];
        if (sessionStorage[SessionStorageKeysHelper.SESSION_KEY_CACHE_CATEGORIES]) {
            categories = JSON.parse(sessionStorage[SessionStorageKeysHelper.SESSION_KEY_CACHE_CATEGORIES]);
        }
        return categories;
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let categoryServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
        this._categoryService = new CategoryService(categoryServiceConfiguration);

        let processConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Process");
        this._processService = new ProcessService(processConfiguration);

        let dossierFolderConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DossierFolder");
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);

        let processFascicleTemplateConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "ProcessFascicleTemplate");
        this._processFascicleTemplateService = new ProcessFascicleTemplateService(processFascicleTemplateConfiguration);
    }

    /**
    *------------------------- Events -----------------------------
    */

    /**
     * Evento scatenato al expand di un nodo
     * @method
     * @param sender
     * @param eventArgs
     * @return
     */
    treeViewCategory_ClientNodeExpanding = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let node: Telerik.Web.UI.RadTreeNode = args.get_node();
        let expandedNodeType: ProcessNodeType = node.get_attributes().getAttribute(CommonSelCategoryRest.NODETYPE_ATTRNAME);

        if (this.nodeIsLoaded(node) || expandedNodeType === ProcessNodeType.TreeRootNode) {
            //no lazy loading required
            return;
        }

        args.set_cancel(true);
        node.get_nodes().clear();
        node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        let func: JQueryPromise<void>;
        switch (expandedNodeType) {
            case ProcessNodeType.Root:
                func = this.findProcesses(node);
                break;
            case ProcessNodeType.Process:
                func = this.loadProcessDossierFolders(node);
                break;
            case ProcessNodeType.DossierFolder:
                func = this.loadDossierFoldersChildren(node);
                break;
            default:
                func = this.findCategories(node.get_value());
                break;
        }

        func.fail((exception: ExceptionDTO) => this.showNotificationException(exception))
            .always(() => node.hideLoadingStatus());
    }

    /**
     * Evento scatenato al click di un nodo
     * @method
     * @param sender
     * @param eventArgs
     * @returns
     */
    treeViewCategory_ClientNodeClicked = (sender: Telerik.Web.UI.RadTreeView, eventArgs: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let node: Telerik.Web.UI.RadTreeNode = eventArgs.get_node();
        if (node.get_value() && node.get_nodes().get_count() == 0 && !this.hasFilters()) {
            this.findCategories(node.get_value())
                .fail((exception: ExceptionDTO) => this.showNotificationException(exception))
                .always(() => node.hideLoadingStatus());
        }
    }

    btnSearch_OnClick = (sender: any, args: any) => {
        sender.preventDefault();
        this._loadingPanel.show(this.pnlMainContentId);
        this.findCategories()
            .fail((exception: ExceptionDTO) => this.showNotificationException(exception))
            .always(() => this._loadingPanel.hide(this.pnlMainContentId));
    }

    btnConfirm_OnClick = (sender: any, args: any) => {
        sender.preventDefault();
        this.confirmNodeSelection();
    }

    btnSearchCode_OnClick = (sender: any, args: any) => {
        sender.preventDefault();

        let inputCodeIsValid: boolean = this.validateSearchCode(this.txtSearchCode.val());

        if (!inputCodeIsValid) {
            this.showNotificationException(null, "Il codice inserito non è formattato correttamente");
            return;
        }

        this._loadingPanel.show(this.pnlMainContentId);
        this.findCategories()
            .done(() => {
                let foundCategories = sessionStorage[SessionStorageKeysHelper.SESSION_KEY_FOUND_CATEGORIES];
                if (!foundCategories) {
                    this.showNotificationException(null, "Il codice cercato è inesistente");
                    return;
                }

                let categories: CategoryTreeViewModel[] = JSON.parse(foundCategories);
                if (categories.length > 1) {
                    this.showNotificationException(null, "Il codice cercato non è univoco");
                    return;
                }

                if (this.fascicleBehavioursEnabled && !categories[0].HasFascicleDefinition) {
                    this.showNotificationException(null, "Non si dispongono i permessi per questa voce del piano di fascicolazione.");
                    return;
                }

                let node = this._treeViewCategory.findNodeByAttribute("FullCode", categories[0].FullCode);

                if (node === null) {
                    this.showNotificationException(null, "Il codice cercato è inesistente");
                    return;
                }

                sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_FOUND_CATEGORIES);

                this._treeViewCategory.trackChanges();
                node.set_selected(true);
                this._treeViewCategory.commitChanges();
                this.confirmNodeSelection();
            })
            .fail((exception: ExceptionDTO) => this.showNotificationException(exception))
            .always(() => this._loadingPanel.hide(this.pnlMainContentId));
    }

    btnSearchOnlyFascicolable_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._loadingPanel.show(this.pnlMainContentId);
        this.findCategories()
            .fail((exception: ExceptionDTO) => this.showNotificationException(exception))
            .always(() => this._loadingPanel.hide(this.pnlMainContentId));
    }

    treeViewCategory_nodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let buttonDisabled: boolean =
            args.get_node().get_text() === CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT
            || args.get_node().get_text() === CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT
            || args.get_node().get_level() === 0
            || (args.get_node().get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.Process && !this.processNodeSelectable);
        $(`#${this.btnConfermaId}`).prop('disabled', buttonDisabled);
    }

    /**
    *------------------------- Methods -----------------------------
    */
    initialize(): void {
        this._treeViewCategory = $find(this.treeViewCategoryId) as Telerik.Web.UI.RadTreeView;
        this._treeViewCategory.add_nodeClicked(this.treeViewCategory_nodeClicked);
        this._btnSearchOnlyFascicolable = $find(this.btnSearchOnlyFascicolableId) as Telerik.Web.UI.RadButton;
        this._btnSearchOnlyFascicolable.add_clicked(this.btnSearchOnlyFascicolable_OnClick);
        this._loadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;

        $(`#${this.btnSearchId}`).click(this.btnSearch_OnClick);
        $(`#${this.btnSearchCodeId}`).click(this.btnSearchCode_OnClick);
        $(`#${this.btnConfermaId}`).click(this.btnConfirm_OnClick);
        $(`#${this.btnConfermaId}`).prop('disabled', true);

        $(`#${this.rowOnlyFascicolableId}`).hide();
        if (this.fascicleBehavioursEnabled) {
            $(`#${this.rowOnlyFascicolableId}`).show();
            this._btnSearchOnlyFascicolable.set_checked(true);
        }

        this.rootNode.get_attributes().setAttribute(CommonSelCategoryRest.NODETYPE_ATTRNAME, ProcessNodeType.TreeRootNode);
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_FOUND_CATEGORIES);
        this._loadingPanel.show(this.pnlMainContentId);
        this.initializeDescription();
        this.initCategoriesCache()
            .done(() => {
                this.findCategories()
                    .fail((exception: ExceptionDTO) => this.showNotificationException(exception))
                    .always(() => this._loadingPanel.hide(this.pnlMainContentId));
            })
            .fail((exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pnlMainContentId);
                this.showNotificationException(exception);
            })

    }

    initializeDescription(): void {
        $(`#${this.pnlDescriptionId}`).hide();
        if (this.fascicleBehavioursEnabled) {
            $(`#${this.pnlDescriptionId}`).show();

            if (this.manager) {
                this.lblDescription.text("Visualizzazione filtrata in base al ruolo di funzione");
                return;
            }

            if (this.role) {
                this.lblDescription.text("Visualizzazione filtrata in base al Settore responsabile selezionato");
                return;
            }

            if (this.fascicleBehavioursEnabled) {
                this.lblDescription.text("Visualizzazione filtrata sulle proprie autorizzazioni dei diritti di fascicolazione");
                return;
            }
        }
    }

    initCategoriesCache(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let finder: CategorySearchFilterDTO = {} as CategorySearchFilterDTO;
        finder.FascicleFilterEnabled = false;
        finder.IdTenantAOO = this.currentTenantAOOId;
        this._categoryService.findTreeCategories(finder,
            (data) => {
                sessionStorage[SessionStorageKeysHelper.SESSION_KEY_CACHE_CATEGORIES] = JSON.stringify(data);
                promise.resolve();
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        )
        return promise.promise();
    }

    confirmNodeSelection() {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._treeViewCategory.get_selectedNode();
        if (this.showProcesses) {
            let processFascicleTemplateParents: TreeNodeModel[] = [
                {
                    text: selectedNode.get_text(),
                    value: selectedNode.get_value(),
                    icon: selectedNode.get_imageUrl(),
                    cssClass: selectedNode.get_cssClass(),
                    nodeType: selectedNode.get_attributes().getAttribute("NodeType")
                }
            ];
            this.getAllParents(selectedNode.get_parent(), processFascicleTemplateParents);
            this.closeWindow(processFascicleTemplateParents);
        }
        else {
            if (!selectedNode || !selectedNode.get_value()) {
                this.showNotificationException(null, "Selezionare almeno un classificatore");
                return;
            }

            let category: CategoryTreeViewModel = this.getCategoryFromNode(selectedNode);
            if (this.fascicleBehavioursEnabled && !category.HasFascicleDefinition) {
                this.showNotificationException(null, "Non si dispongono i permessi per questa voce del piano di fascicolazione.");
                return;
            }

            this.closeWindow(category);
        }
    }

    findCategories(parentId?: number): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let finder: CategorySearchFilterDTO = this.prepareFinder(parentId);
        finder.IdTenantAOO = this.currentTenantAOOId;
        this._categoryService.findTreeCategories(finder,
            (data: any) => {
                this.populateTreeView(data, (!parentId || parentId == 0))
                    .done(() => promise.resolve())
                    .fail((exception: ExceptionDTO) => promise.reject(exception));
            },
            (exception: ExceptionDTO) => promise.reject(exception),
        );
        return promise.promise();
    }

    private prepareFinder(parentId?: number): CategorySearchFilterDTO {
        let finder: CategorySearchFilterDTO = {} as CategorySearchFilterDTO;
        parentId = parentId ? parentId : this.parentId;
        finder.LoadRoot = !this.hasFilters() && (!parentId || parentId == 0);
        finder.ParentId = parentId;
        finder.Name = this.txtSearch.val();
        finder.FullCode = this.formatFullCode(this.txtSearchCode.val());
        finder.ParentAllDescendants = this.includeParentDescendants;
        if (this.fascicleBehavioursEnabled) {
            finder.HasFascicleInsertRights = this._btnSearchOnlyFascicolable.get_checked() && this.fascicleType == FascicleType.Procedure;
            finder.IdRole = this.role;
            finder.IdContainer = this.container;
            if (this.fascicleType == FascicleType.Procedure) {
                finder.Manager = this.manager;
                finder.Secretary = this.secretary;
            }
            finder.FascicleType = this.fascicleType ? this.fascicleType.toString() : null;
            finder.FascicleFilterEnabled = this._btnSearchOnlyFascicolable.get_checked();
        }
        return finder;
    }

    private formatFullCode(fullCode: string): string {

        let inputCodeIsValid: boolean = this.validateSearchCode(this.txtSearchCode.val());

        if (!inputCodeIsValid) {
            return '';
        }

        let codeStringFragments: string[] = fullCode.split('.');

        let fullCodeFormatted: string = codeStringFragments.map(stringFragment => {
            let fragmentLength = stringFragment.length;
            return fragmentLength == CommonSelCategoryRest.CATEGORYFULLCODE_MAXLENGTH ? Number(stringFragment) : Number(stringFragment).padLeft(CommonSelCategoryRest.CATEGORYFULLCODE_MAXLENGTH);
        }).join('|');

        return fullCodeFormatted;
    }

    private populateTreeView(categories: CategoryTreeViewModel[], needClearItems: boolean = true): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (!categories || categories.length == 0) {
            if (this.hasFilters()) {
                sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_FOUND_CATEGORIES);
            }

            if (needClearItems) {
                this.rootNode.get_nodes().clear();
            }

            return promise.resolve();
        }

        let nodeSource: Telerik.Web.UI.RadTreeNode[] = [];
        if (!needClearItems) {
            nodeSource = this._treeViewCategory.get_allNodes();
        }

        if (this.hasFilters()) {
            sessionStorage[SessionStorageKeysHelper.SESSION_KEY_FOUND_CATEGORIES] = JSON.stringify(categories);
        }

        let hasFascicleRights: boolean = this._btnSearchOnlyFascicolable.get_checked();
        this.createNodes(categories, nodeSource)
            .done(() => {
                if (needClearItems) {
                    this.rootNode.get_nodes().clear();
                    nodeSource.forEach(n => this.rootNode.get_nodes().add(n));
                }

                if (hasFascicleRights && this.showProcesses) {
                    this.rootNode.get_allNodes().forEach((nn) => {
                        this.createProcessesNode(nn);
                        nn.get_attributes().setAttribute(CommonSelCategoryRest.NODE_CHILDREN_LOADED_ATTRIBUTE, true);
                    });
                }
                this.rootNode.expand();
                this._treeViewCategory.commitChanges();
                promise.resolve();
            })
            .fail((exception: ExceptionDTO) => promise.reject(exception));
        return promise.promise();
    }

    private createNodes(categories: CategoryTreeViewModel[], nodeSource: Telerik.Web.UI.RadTreeNode[]): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (!categories || categories.length == 0) {
            return promise.resolve();
        }

        let hasFilters: boolean = this.hasFilters();
        let currentCategory: CategoryTreeViewModel = categories.shift();
        this.createNode(currentCategory, nodeSource)
            .done((node: Telerik.Web.UI.RadTreeNode) => {
                if (hasFilters) {
                    let toAppendClass: string = node.get_cssClass();
                    node.set_cssClass(`${toAppendClass} dsw-text-bold`);
                }

                if (this.fascicleBehavioursEnabled) {
                    if (currentCategory.HasFascicleDefinition) {
                        node.set_cssClass("node-tree-fascicle");
                    } else {
                        node.set_cssClass("node-disabled");
                    }
                }

                this.createNodes(categories, nodeSource)
                    .done(() => promise.resolve())
                    .fail((exception: ExceptionDTO) => promise.reject(exception));
            })
        return promise.promise();
    }

    private createNode(category: CategoryTreeViewModel, nodeSource: Telerik.Web.UI.RadTreeNode[]): JQueryPromise<Telerik.Web.UI.RadTreeNode> {
        let promise: JQueryDeferred<Telerik.Web.UI.RadTreeNode> = $.Deferred<Telerik.Web.UI.RadTreeNode>();
        let currentNode: Telerik.Web.UI.RadTreeNode = this.findNodeFromSource(nodeSource, category.IdCategory);
        if (currentNode) {
            //node already exists, return it.
            return promise.resolve(currentNode);
        }

        currentNode = new Telerik.Web.UI.RadTreeNode();
        currentNode.set_text(`${category.Code}.${category.Name}`);
        currentNode.set_value(category.IdCategory);
        currentNode.set_imageUrl("../Comm/images/Classificatore.gif");
        this.setNodeAttributes(currentNode, category);
        if (category.HasChildren) {
            if (!this._btnSearchOnlyFascicolable.get_checked()) {
                //set webservice expand mode for node expansion logic
                currentNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.WebService);
            }
            currentNode.set_imageUrl("../Comm/images/folderopen16.gif");
        }

        if (category.IdParent) {
            let parentNode: Telerik.Web.UI.RadTreeNode = this.findNodeFromSource(nodeSource, category.IdParent);
            if (parentNode) {
                parentNode.get_nodes().add(currentNode);
                parentNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                parentNode.set_expanded(true);
                parentNode.get_attributes().setAttribute(CommonSelCategoryRest.NODE_CHILDREN_LOADED_ATTRIBUTE, true);
                return promise.resolve(currentNode);
            }

            let parentFromCache: CategoryTreeViewModel = this.cachedCategories.filter((item) => item.IdCategory == category.IdParent)[0];
            if (!parentFromCache) {
                nodeSource.push(currentNode);
                return promise.resolve(currentNode);
            }

            this.createNode(parentFromCache, nodeSource)
                .done((node) => {
                    node.get_nodes().add(currentNode);
                    node.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                    node.set_expanded(true);
                    node.get_attributes().setAttribute(CommonSelCategoryRest.NODE_CHILDREN_LOADED_ATTRIBUTE, true);
                    promise.resolve(currentNode);
                })
                .fail((exception: ExceptionDTO) => promise.reject(exception));
        } else {
            nodeSource.push(currentNode);
            return promise.resolve(currentNode);
        }
        return promise.promise();
    }

    private setNodeAttributes(node: Telerik.Web.UI.RadTreeNode, category: CategoryTreeViewModel): void {
        node.get_attributes().setAttribute("UniqueId", category.UniqueId);
        node.get_attributes().setAttribute("HasFascicleDefinition", category.HasFascicleDefinition);
        node.get_attributes().setAttribute("IdCategory", category.IdCategory);
        node.get_attributes().setAttribute("Name", category.Name);
        node.get_attributes().setAttribute("FullCode", category.FullCode);
        node.get_attributes().setAttribute("Code", category.Code);
        node.get_attributes().setAttribute("FullIncrementalPath", category.FullIncrementalPath);
        node.get_attributes().setAttribute("HasChildren", category.HasChildren);
        node.get_attributes().setAttribute("IdParent", category.IdParent);
        node.get_attributes().setAttribute("NodeType", ProcessNodeType.Category);
    }

    private getCategoryFromNode(node: Telerik.Web.UI.RadTreeNode): CategoryTreeViewModel {
        let category: CategoryTreeViewModel = {} as CategoryTreeViewModel;
        category.UniqueId = node.get_attributes().getAttribute("UniqueId");
        category.HasFascicleDefinition = node.get_attributes().getAttribute("HasFascicleDefinition");
        category.IdCategory = node.get_attributes().getAttribute("IdCategory");
        category.Name = node.get_attributes().getAttribute("Name");
        category.FullCode = node.get_attributes().getAttribute("FullCode");
        category.Code = node.get_attributes().getAttribute("Code");
        category.FullIncrementalPath = node.get_attributes().getAttribute("FullIncrementalPath");
        category.HasChildren = node.get_attributes().getAttribute("HasChildren");
        category.IdParent = node.get_attributes().getAttribute("IdParent");
        return category;
    }

    private findNodeFromSource(nodeSource: Telerik.Web.UI.RadTreeNode[], idCategory: number): Telerik.Web.UI.RadTreeNode {
        let foundNode: Telerik.Web.UI.RadTreeNode = null;
        for (let sourceNode of nodeSource) {
            if (sourceNode.get_value() == idCategory) {
                return sourceNode;
            }

            if (sourceNode.get_allNodes()) {
                foundNode = this.findNodeFromSource(sourceNode.get_allNodes(), idCategory);
            }

            if (foundNode) {
                return foundNode;
            }
        }
    }

    private hasFilters(): boolean {
        return this.txtSearch.val() || this.validateSearchCode(this.txtSearchCode.val()) || this._btnSearchOnlyFascicolable.get_checked();
    }

    getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    closeWindow(dataToReturn: any): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(dataToReturn);
    }

    protected showNotificationException(exception: ExceptionDTO, customMessage?: string) {
        PageClassHelper.callUserControlFunctionSafe<UscErrorNotification>(this.uscNotificationId)
            .done(control => {
                if (exception) {
                    control.showNotification(exception);
                    return;
                }
                control.showWarningMessage(customMessage);
            })
    }

    private createProcessesNode(parentNode: Telerik.Web.UI.RadTreeNode): void {
        let processesNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        processesNode.set_text(CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT);
        processesNode.set_cssClass("dsw-text-bold");
        processesNode.get_attributes().setAttribute("NodeType", ProcessNodeType.Root);
        this.createEmptyNode(processesNode);
        parentNode.get_nodes().add(processesNode);
        parentNode.expand();
    }

    private createEmptyNode(parentNode: Telerik.Web.UI.RadTreeNode): void {
        let emptyNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        emptyNode.set_text(CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT);
        parentNode.get_nodes().add(emptyNode);
    }

    findProcesses(node: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        this._processService.getAvailableProcesses(null, true, Number(node.get_parent().get_value()), null, this.isProcessActive,
            (categoryProcesses: ProcessModel[]) => {
                if (!categoryProcesses.length) {
                    this.createEmptyNode(node);
                    node.set_expanded(true);
                    node.get_attributes().setAttribute(CommonSelCategoryRest.NODE_CHILDREN_LOADED_ATTRIBUTE, true);
                    promise.resolve();
                    return;
                }
                categoryProcesses.map((process: ProcessModel) => {
                    let currentProcessTreeNode: Telerik.Web.UI.RadTreeNode
                        = this.createTreeNode(ProcessNodeType.Process, process.Name, process.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png");
                    this.createEmptyNode(currentProcessTreeNode);
                    return currentProcessTreeNode;
                }).forEach((child: Telerik.Web.UI.RadTreeNode) => {
                    node.get_nodes().add(child);
                    node.set_expanded(true);
                    node.get_attributes().setAttribute(CommonSelCategoryRest.NODE_CHILDREN_LOADED_ATTRIBUTE, true);
                });
                promise.resolve();
            }, (exception: ExceptionDTO) => {
                promise.reject(exception);
            });
        return promise.promise();
    }

    private loadProcessDossierFolders(parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();
        this._dossierFolderService.getChildrenByParentProcess(parentNode.get_value(), false, false, (processDossierFolders: any[]) => {
            if (this.showProcessFascicleTemplate) {
                this.loadProcessFascicleTemplate(parentNode);
            }
            if (!processDossierFolders || processDossierFolders.length == 0) {
                this.createEmptyNode(parentNode);
                parentNode.set_expanded(true);
                parentNode.get_attributes().setAttribute(CommonSelCategoryRest.NODE_CHILDREN_LOADED_ATTRIBUTE, true);
                defferedRequest.resolve();
                return;
            }
            let dossierSummaryFolderViewModelMapper: DossierSummaryFolderViewModelMapper = new DossierSummaryFolderViewModelMapper();
            let processDossierFoldersViewModels: DossierSummaryFolderViewModel[] = dossierSummaryFolderViewModelMapper.MapCollection(processDossierFolders);
            this.addDossierFolderNodes(processDossierFoldersViewModels, parentNode);
            parentNode.set_expanded(true);
            parentNode.get_attributes().setAttribute(CommonSelCategoryRest.NODE_CHILDREN_LOADED_ATTRIBUTE, true);
            defferedRequest.resolve();
        }, (exception: ExceptionDTO) => defferedRequest.reject(exception));

        return defferedRequest.promise();
    }

    private addDossierFolderNodes(dossierFolders: DossierSummaryFolderViewModel[], parentNode: Telerik.Web.UI.RadTreeNode): void {
        dossierFolders.forEach((dossierFolder: DossierSummaryFolderViewModel) => {
            let dossierFolderImageUrl: string = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
            let dossierFolderExpandedImageUrl: string = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
            let dossierFolderNodeValue: string = dossierFolder.idFascicle ? dossierFolder.idFascicle : dossierFolder.UniqueId;

            let currentDossierFolderTreeNode: Telerik.Web.UI.RadTreeNode
                = this.createTreeNode(ProcessNodeType.DossierFolder, dossierFolder.Name, dossierFolderNodeValue, dossierFolderImageUrl, parentNode, null, dossierFolderExpandedImageUrl);

            this.createEmptyNode(currentDossierFolderTreeNode);
        });
    }

    private loadDossierFoldersChildren(parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> {
        let defferedRequest: JQueryDeferred<void> = $.Deferred<void>();
        this._dossierFolderService.getProcessFascicleChildren(parentNode.get_value(), 0, (dossierFolders: DossierSummaryFolderViewModel[]) => {
            if (this.showProcessFascicleTemplate) {
                this.loadProcessFascicleTemplate(parentNode);
            }
            if (!dossierFolders || dossierFolders.length == 0) {
                this.createEmptyNode(parentNode);
                parentNode.set_expanded(true);
                parentNode.get_attributes().setAttribute(CommonSelCategoryRest.NODE_CHILDREN_LOADED_ATTRIBUTE, true);
                defferedRequest.resolve();
                return;
            }
            this.addDossierFolderNodes(dossierFolders, parentNode);
            parentNode.set_expanded(true);
            parentNode.get_attributes().setAttribute(CommonSelCategoryRest.NODE_CHILDREN_LOADED_ATTRIBUTE, true);
            defferedRequest.resolve();
        }, (exception: ExceptionDTO) => defferedRequest.reject(exception));

        return defferedRequest.promise();
    }

    private loadProcessFascicleTemplate(parentNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._processFascicleTemplateService.getActiveFascicleTemplatesByDossierFolderId(parentNode.get_value(),
            (data: any) => {
                if (!data.length) {
                    promise.resolve();
                    return;
                }
                if (parentNode.get_nodes().get_count() > 0 && parentNode.get_nodes().getNode(0).get_text() == CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT) {
                    parentNode.get_nodes().clear();
                }
                data.map((processFascicleTemplate: ProcessFascicleTemplateModel) => {
                    let currentProcessTreeNode: Telerik.Web.UI.RadTreeNode
                        = this.createTreeNode(ProcessNodeType.ProcessFascicleTemplate, processFascicleTemplate.Name, processFascicleTemplate.UniqueId, "../App_Themes/DocSuite2008/imgset16/fascicle_close.png");
                    return currentProcessTreeNode;
                }).forEach((child: Telerik.Web.UI.RadTreeNode) => {
                    parentNode.get_nodes().add(child);
                    parentNode.set_expanded(true);
                    parentNode.get_attributes().setAttribute(CommonSelCategoryRest.NODE_CHILDREN_LOADED_ATTRIBUTE, true);
                });
                promise.resolve();
            });
        return promise.promise();
    }

    private createTreeNode(nodeType: ProcessNodeType, nodeDescription: string, nodeValue: number | string, imageUrl: string, parentNode?: Telerik.Web.UI.RadTreeNode, tooltipText?: string, expandedImageUrl?: string): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        treeNode.set_text(nodeDescription);
        treeNode.set_value(nodeValue);
        treeNode.get_attributes().setAttribute("NodeType", nodeType);

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

    private getAllParents(node: Telerik.Web.UI.RadTreeNode, treeNodeModel: TreeNodeModel[]): void {
        if (!(node instanceof Telerik.Web.UI.RadTreeView)) {
            treeNodeModel.push({
                text: node.get_text(),
                value: node.get_value(),
                cssClass: node.get_cssClass(),
                icon: node.get_imageUrl(),
                nodeType: node.get_attributes().getAttribute("NodeType")
            });
            this.getAllParents(node.get_parent(), treeNodeModel);
        }
    }

    private validateSearchCode(fullCode: string): boolean {
        if (!fullCode) {
            return false;
        }

        let codeStringFragments: string[] = fullCode.split('.');
        let invalidLengthFragments = codeStringFragments.some(stringFragment => stringFragment.length > CommonSelCategoryRest.CATEGORYFULLCODE_MAXLENGTH);
        let fragmentsNotNumeric = codeStringFragments.some(stringFragment => isNaN(Number(stringFragment)));

        if (invalidLengthFragments || fragmentsNotNumeric) {
            return false;
        }

        return true;
    }

    private nodeIsLoaded(node: Telerik.Web.UI.RadTreeNode): boolean {
        return node.get_attributes().getAttribute("ChildrenLoaded") == true;
    }
}

export = CommonSelCategoryRest;