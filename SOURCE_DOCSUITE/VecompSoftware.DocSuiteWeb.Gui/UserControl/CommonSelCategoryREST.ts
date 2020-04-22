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

    private _categoryService: CategoryService;
    private _treeViewCategory: Telerik.Web.UI.RadTreeView;
    private _btnSearchOnlyFascicolable: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    private static FOUND_CATEGORIES_SESSION_KEY = "FoundCategories";
    private static CACHE_CATEGORIES_SESSION_KEY = "CacheCategories";

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
        if (sessionStorage[CommonSelCategoryRest.CACHE_CATEGORIES_SESSION_KEY]) {
            categories = JSON.parse(sessionStorage[CommonSelCategoryRest.CACHE_CATEGORIES_SESSION_KEY]);
        }
        return categories;
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let categoryServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
        this._categoryService = new CategoryService(categoryServiceConfiguration);
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
        if (node.get_nodes().get_count() == 0 && !this.hasFilters()) {
            args.set_cancel(true);
            node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            node.set_selected(true);
            this.findCategories(node.get_value())
                .fail((exception: ExceptionDTO) => this.showNotificationException(exception))
                .always(() => node.hideLoadingStatus());
        }
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
            node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
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
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._treeViewCategory.get_selectedNode();
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

    btnSearchCode_OnClick = (sender: any, args: any) => {
        sender.preventDefault();
        this._loadingPanel.show(this.pnlMainContentId);
        this.findCategories()
            .done(() => {
                let foundCategories = sessionStorage[CommonSelCategoryRest.FOUND_CATEGORIES_SESSION_KEY];
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

                sessionStorage.removeItem(CommonSelCategoryRest.FOUND_CATEGORIES_SESSION_KEY);
                this.closeWindow(categories[0]);
            })
            .fail((exception: ExceptionDTO) => this.showNotificationException(exception))
            .always(() => this._loadingPanel.hide(this.pnlMainContentId));
    }

    btnSearchOnlyFascicolable_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._loadingPanel.show(this.pnlMainContentId);
        this.findCategories()
            .fail((exception: ExceptionDTO) => this.showNotificationException(exception))
            .always(() => this._loadingPanel.hide(this.pnlMainContentId));
    }

    /**
    *------------------------- Methods -----------------------------
    */
    initialize(): void {
        this._treeViewCategory = $find(this.treeViewCategoryId) as Telerik.Web.UI.RadTreeView;
        this._btnSearchOnlyFascicolable = $find(this.btnSearchOnlyFascicolableId) as Telerik.Web.UI.RadButton;
        this._btnSearchOnlyFascicolable.add_clicked(this.btnSearchOnlyFascicolable_OnClick);
        this._loadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;

        $(`#${this.btnSearchId}`).click(this.btnSearch_OnClick);
        $(`#${this.btnSearchCodeId}`).click(this.btnSearchCode_OnClick);
        $(`#${this.btnConfermaId}`).click(this.btnConfirm_OnClick);

        $(`#${this.rowOnlyFascicolableId}`).hide();
        if (this.fascicleBehavioursEnabled) {
            $(`#${this.rowOnlyFascicolableId}`).show();
            this._btnSearchOnlyFascicolable.set_checked(true);
        }

        sessionStorage.removeItem(CommonSelCategoryRest.FOUND_CATEGORIES_SESSION_KEY);
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
                this.lblDescription.text("Visualizzazione filtrata in base al settore responsabile selezionato");
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
        this._categoryService.findTreeCategories(finder,
            (data) => {
                sessionStorage[CommonSelCategoryRest.CACHE_CATEGORIES_SESSION_KEY] = JSON.stringify(data);
                promise.resolve();
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        )
        return promise.promise();
    }

    findCategories(parentId?: number): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let finder: CategorySearchFilterDTO = this.prepareFinder(parentId);
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
            finder.FascicleType = this.fascicleType.toString();
            finder.FascicleFilterEnabled = this._btnSearchOnlyFascicolable.get_checked();
        }
        return finder;
    }

    private formatFullCode(fullCode: string): string {
        if (!fullCode) {
            return '';
        }

        let splittedCode: string[] = fullCode.split('.').filter((code) => Number(code)).map((code) => Number(code).padLeft(4));
        let fullCodeFormatted: string = splittedCode.join('|');
        return fullCodeFormatted;
    }

    private populateTreeView(categories: CategoryTreeViewModel[], needClearItems: boolean = true): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (!categories || categories.length == 0) {
            if (this.hasFilters()) {
                sessionStorage.removeItem(CommonSelCategoryRest.FOUND_CATEGORIES_SESSION_KEY);
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
            sessionStorage[CommonSelCategoryRest.FOUND_CATEGORIES_SESSION_KEY] = JSON.stringify(categories);
        }

        this.createNodes(categories, nodeSource)
            .done(() => {
                if (needClearItems) {
                    this.rootNode.get_nodes().clear();
                    for (let node of nodeSource) {
                        this.rootNode.get_nodes().add(node);
                    }
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

        let currentCategory: CategoryTreeViewModel = categories.shift();
        this.createNode(currentCategory, nodeSource)
            .done((node: Telerik.Web.UI.RadTreeNode) => {
                if (this.hasFilters()) {
                    let toAppendClass: string = node.get_cssClass();
                    node.set_cssClass(`${toAppendClass} dsw-text-bold`);
                }

                if (this.fascicleBehavioursEnabled) {
                    if (currentCategory.HasFascicleDefinition) {                        
                        node.get_attributes().setAttribute("HasFascicleDefinition", true);
                        node.set_cssClass("node-tree-fascicle");
                    }
                }

                if (categories.length == 0) {
                    return promise.resolve();
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
            return promise.resolve(currentNode);
        }

        currentNode = new Telerik.Web.UI.RadTreeNode();
        currentNode.set_text(`${category.Code}.${category.Name}`);
        currentNode.set_value(category.IdCategory);
        this.setNodeAttributes(currentNode, category);
        if (category.HasChildren) {
            if (!this.hasFilters()) {
                currentNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ServerSideCallBack);
            }            
            currentNode.set_imageUrl("../Comm/images/folderopen16.gif");
        } else {
            currentNode.set_imageUrl("../Comm/images/Classificatore.gif");
        }     

        if (this.fascicleBehavioursEnabled) {
            if (!category.HasFascicleDefinition) {
                currentNode.set_cssClass("node-disabled");
            }
        }

        if (category.IdParent) {
            let parentNode: Telerik.Web.UI.RadTreeNode = this.findNodeFromSource(nodeSource, category.IdParent);
            if (parentNode) {
                parentNode.get_nodes().add(currentNode);
                parentNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                parentNode.set_expanded(true);
                return promise.resolve(currentNode);
            }

            let parentFromCache: CategoryTreeViewModel = this.cachedCategories.filter((item) => item.IdCategory == category.IdParent)[0];
            this.createNode(parentFromCache, nodeSource)
                .done((node) => {
                    node.get_nodes().add(currentNode);
                    node.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                    node.set_expanded(true);
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
        return this.txtSearch.val() || this.formatFullCode(this.txtSearchCode.val()) || this._btnSearchOnlyFascicolable.get_checked();
    }

    getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    closeWindow(selectedCategory: CategoryTreeViewModel): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(selectedCategory);
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

export = CommonSelCategoryRest;