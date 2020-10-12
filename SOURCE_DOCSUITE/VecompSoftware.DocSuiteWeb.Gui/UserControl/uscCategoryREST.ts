/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import CategoryTreeViewModel = require("App/ViewModels/Commons/CategoryTreeViewModel");
import FascicleType = require("App/Models/Fascicles/FascicleType");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import CategoryService = require("App/Services/Commons/CategoryService");
import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import UscErrorNotification = require("UserControl/uscErrorNotification");
import CategoryModel = require("App/Models/Commons/CategoryModel");
import TreeNodeModel = require("App/ViewModels/Commons/TreeNodeModel");
import CommonSelCategoryRest = require("./CommonSelCategoryRest");
import ProcessNodeType = require("App/Models/Processes/ProcessNodeType");
import CategoryFascicleViewModel = require("App/ViewModels/Commons/CategoryFascicleViewModel");

declare var ValidatorEnable: any;
interface uscCategoryRestConfiguration {
    showAuthorizedFascicolable?: boolean;
    showManagerFascicolable: string;
    showSecretaryFascicolable: string;
    showRoleFascicolable?: number;
    fascicleType?: FascicleType;
    showContainerFascicolable?: number;
}

class uscCategoryRest {
    actionToolbarId: string;
    pnlMainContentId: string;
    windowManagerId: string;
    treeCategoryId: string;
    uscNotificationId: string;
    anyNodeCheckId: string;
    idCategory?: number;
    ajaxManagerId: string;
    windowSelCategoryId: string;
    showProcesses: boolean;
    currentTenantAOOId: string;

    static LOADED_EVENT: string = "onLoaded";
    static ADDED_EVENT: string = "onAdded";
    static REMOVED_EVENT: string = "onRemoved";

    private readonly _selectedCategorySessionKey: string;
    private readonly _configurationCategorySessionKey: string;

    private _windowManager: Telerik.Web.UI.RadWindowManager;
    private _actionToolbar: Telerik.Web.UI.RadToolBar;
    private _treeCategory: Telerik.Web.UI.RadTreeView;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _windowSelCategory: Telerik.Web.UI.RadWindow;
    private _categoryService: CategoryService;

    private toolbarActions(): Array<[string, () => void]> {
        let items: Array<[string, () => void]> = [
            ["add", () => this.addCategories()],
            ["delete", () => this.removeCategories()]
        ];
        return items;
    }

    constructor(serviceConfigurations: ServiceConfiguration[], configuration: uscCategoryRestConfiguration, uscId: string) {
        let categoryServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
        this._categoryService = new CategoryService(categoryServiceConfiguration);
        this._selectedCategorySessionKey = `${uscId}_selectedCategories`;
        this._configurationCategorySessionKey = `${uscId}_configuration`;
        sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
    }

    /**
    *------------------------- Events -----------------------------
    */
    protected actionToolbar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let currentActionButtonItem: Telerik.Web.UI.RadToolBarButton = args.get_item() as Telerik.Web.UI.RadToolBarButton;
        let currentAction: () => void = this.toolbarActions().filter((item: [string, () => void]) => item[0] == currentActionButtonItem.get_commandName())
            .map((item: [string, () => void]) => item[1])[0];

        currentAction();
    }

    windowSelCategory_onClose = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        let returnedData: any = args.get_argument();
        if (this.showProcesses) {
            let processFascicleTemplateParents: TreeNodeModel[] = returnedData;
            if (processFascicleTemplateParents) {
                this._treeCategory.get_nodes().clear();
                this.createProcessFascicleTemplateNode(processFascicleTemplateParents.reverse()).done((node) => {
                })
                    .fail((exception: ExceptionDTO) => this.showNotificationException(exception));
            }
        }
        else {
            let category: CategoryTreeViewModel = returnedData;
            if (category) {
                this._treeCategory.trackChanges();
                this._treeCategory.get_nodes().clear();
                sessionStorage.removeItem(this._selectedCategorySessionKey);
                this.createNode(category).
                    done((node) => {
                        node.get_attributes().setAttribute("IsSelected", true);
                        node.set_selected(true);
                        this._treeCategory.commitChanges();
                        this.addToSelectedSource(category);
                        $(`#${this.pnlMainContentId}`).triggerHandler(uscCategoryRest.ADDED_EVENT, category.IdCategory);
                        this._ajaxManager.ajaxRequest("Add");
                    })
                    .fail((exception: ExceptionDTO) => this.showNotificationException(exception));
            }
        }
    }

    /**
    *------------------------- Methods -----------------------------
    */

    initialize(): void {
        this._windowManager = $find(this.windowManagerId) as Telerik.Web.UI.RadWindowManager;
        this._ajaxManager = $find(this.ajaxManagerId) as Telerik.Web.UI.RadAjaxManager;
        this._actionToolbar = $find(this.actionToolbarId) as Telerik.Web.UI.RadToolBar;
        this._treeCategory = $find(this.treeCategoryId) as Telerik.Web.UI.RadTreeView;
        this._windowSelCategory = $find(this.windowSelCategoryId) as Telerik.Web.UI.RadWindow;
        this._windowSelCategory.add_close(this.windowSelCategory_onClose);
        this._actionToolbar.add_buttonClicked(this.actionToolbar_ButtonClicked);

        sessionStorage.removeItem(this._selectedCategorySessionKey);
        this.initializeSources()
            .done(() => this.bindLoaded())
            .fail((exception: ExceptionDTO) => this.showNotificationException(exception));
    }

    public registerAddedEventhandler(handler: (data: JQueryEventObject, args: any[]) => void) {
        $(`#${this.pnlMainContentId}`).on(uscCategoryRest.ADDED_EVENT, handler);
    }

    private initializeSources(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let configuration: uscCategoryRestConfiguration = this.getConfiguration();
        if (!this.idCategory) {
            return promise.resolve();
        }

        this._categoryService.findTreeCategory(this.idCategory, configuration.fascicleType,
            (data) => {
                if (!data) {
                    return promise.resolve();
                }

                this._treeCategory.trackChanges();
                this.createNode(data)
                    .done((node) => {
                        node.get_attributes().setAttribute("IsSelected", true);
                        this._treeCategory.commitChanges();
                        this.addToSelectedSource(data);
                        $(`#${this.pnlMainContentId}`).triggerHandler(uscCategoryRest.ADDED_EVENT, data.IdCategory);
                        setTimeout(() => this._ajaxManager.ajaxRequest("Add"), 800);
                        promise.resolve();
                    })
                    .fail((exception: ExceptionDTO) => promise.reject(exception));
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        )
        return promise.promise();
    }

    private bindLoaded(): void {
        $(`#${this.pnlMainContentId}`).data(this);
        $(`#${this.pnlMainContentId}`).triggerHandler(uscCategoryRest.LOADED_EVENT);
    }

    private addCategories(): void {
        let url: string = "../UserControl/CommonSelCategoryRest.aspx?Type=Comm";
        let configuration: uscCategoryRestConfiguration = this.getConfiguration();
        if (configuration.showAuthorizedFascicolable) {
            url = url.concat("&FascicleBehavioursEnabled=true");
        }
        if (configuration.showManagerFascicolable) {
            url = url.concat(`&Manager=${configuration.showManagerFascicolable}`);
        }
        if (configuration.showSecretaryFascicolable) {
            url = url.concat(`&Secretary=${configuration.showSecretaryFascicolable}`);
        }
        if (configuration.fascicleType) {
            url = url.concat(`&FascicleType=${configuration.fascicleType}`);
        }
        if (configuration.showRoleFascicolable) {
            url = url.concat(`&Role=${configuration.showRoleFascicolable}`);
        }
        if (configuration.showContainerFascicolable) {
            url = url.concat(`&Container=${configuration.showContainerFascicolable}`);
        }
        url = url.concat(`&ShowProcesses=${this.showProcesses}`);
        this._windowManager.open(url, "windowSelCategory", undefined);
    }

    private removeCategories(): void {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._treeCategory.get_selectedNode();
        if (!selectedNode) {
            this.showNotificationException(null, "Selezionare un classificatore");
            return;
        }

        let idCategory: number = selectedNode.get_value();
        this._treeCategory.trackChanges();
        this._treeCategory.get_nodes().clear();
        this._treeCategory.commitChanges();
        sessionStorage[this._selectedCategorySessionKey] = JSON.stringify([]);
        $(`#${this.pnlMainContentId}`).triggerHandler(uscCategoryRest.REMOVED_EVENT, idCategory);
        this._ajaxManager.ajaxRequest("Remove");
    }

    private createNode(category: CategoryTreeViewModel): JQueryPromise<Telerik.Web.UI.RadTreeNode> {
        let promise: JQueryDeferred<Telerik.Web.UI.RadTreeNode> = $.Deferred<Telerik.Web.UI.RadTreeNode>();
        let currentNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        currentNode.set_text(`${category.Code}.${category.Name}`);
        currentNode.set_value(category.IdCategory);

        if (category.HasChildren) {
            currentNode.set_imageUrl("../Comm/images/folderopen16.gif");
        } else {
            currentNode.set_imageUrl("../Comm/images/Classificatore.gif");
        }

        if (category.IdParent) {
            let configuration: uscCategoryRestConfiguration = this.getConfiguration();
            this._categoryService.findTreeCategory(category.IdParent, configuration.fascicleType,
                (data: any) => {
                    if (data.Code == 0) {
                        this._treeCategory.get_nodes().add(currentNode);
                        return promise.resolve(currentNode);
                    }
                    this.createNode(data)
                        .done((node) => {
                            node.get_nodes().add(currentNode);
                            node.set_expanded(true);
                            promise.resolve(currentNode);
                        })
                        .fail((exception: ExceptionDTO) => promise.reject(exception));
                },
                (exception: ExceptionDTO) => promise.reject(exception)
            )
        } else {
            this._treeCategory.get_nodes().add(currentNode);
            return promise.resolve(currentNode);
        }
        return promise.promise();
    }

    private createProcessFascicleTemplateNode(processFascicleTemplateParents: TreeNodeModel[]): JQueryPromise<Telerik.Web.UI.RadTreeNode> {
        let promise: JQueryDeferred<Telerik.Web.UI.RadTreeNode> = $.Deferred<Telerik.Web.UI.RadTreeNode>();
        let nodeCollection: Telerik.Web.UI.RadTreeNodeCollection = this._treeCategory.get_nodes();
        let leafNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        for (let treeNodeData of processFascicleTemplateParents) {
            let currentNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            currentNode.set_text(`${treeNodeData.text}`);
            currentNode.set_value(treeNodeData.value);
            currentNode.set_imageUrl(treeNodeData.icon);
            currentNode.set_cssClass(treeNodeData.cssClass);
            currentNode.get_attributes().setAttribute("NodeType", treeNodeData.nodeType);
            nodeCollection.add(currentNode);
            currentNode.expand();
            currentNode.select();
            nodeCollection = nodeCollection.getNode(0).get_nodes();
            leafNode = currentNode;
        }
        leafNode.get_attributes().setAttribute("IsSelected", true);
        $(`#${this.pnlMainContentId}`).triggerHandler(uscCategoryRest.ADDED_EVENT, processFascicleTemplateParents[processFascicleTemplateParents.length - 1].value);
        return promise.resolve(leafNode);
    }

    private addToSelectedSource(category: CategoryTreeViewModel): void {
        let currentSource: CategoryTreeViewModel[] = this.getSelectedCategories();
        currentSource.push(category);
        sessionStorage[this._selectedCategorySessionKey] = JSON.stringify(currentSource);
    }

    private getSelectedCategories(): CategoryTreeViewModel[] {
        let source: CategoryTreeViewModel[] = [];
        if (sessionStorage[this._selectedCategorySessionKey]) {
            source = JSON.parse(sessionStorage[this._selectedCategorySessionKey]);
        }
        return source;
    }

    getSelectedCategory(): CategoryModel {
        let source: CategoryTreeViewModel[] = this.getSelectedCategories();
        if (source.length == 0) {
            return null;
        }
        let model: CategoryModel = new CategoryModel();
        model.EntityShortId = source[0].IdCategory;
        model.Name = source[0].Name;
        return model;
    }

    getProcessId(): string {
        return this._treeCategory.findNodeByText(CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT).get_nodes().getNode(0).get_value();
    }

    getSelectedNode(): Telerik.Web.UI.RadTreeNode {
        return this._treeCategory.get_selectedNode();
    }

    getProcessFascicleTemplateFolderId(): string {
        return this.getSelectedNode().get_attributes().getAttribute("NodeType") === ProcessNodeType.ProcessFascicleTemplate
            ? this._treeCategory.get_selectedNode().get_parent().get_value()
            : this._treeCategory.get_selectedNode().get_value();
    }

    private getConfiguration(): uscCategoryRestConfiguration {
        let configuration: uscCategoryRestConfiguration = {} as uscCategoryRestConfiguration;
        if (sessionStorage[this._configurationCategorySessionKey]) {
            configuration = JSON.parse(sessionStorage[this._configurationCategorySessionKey]);
        }
        return configuration;
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

    setShowAuthorizedParam(value?: boolean): void {
        let configuration: uscCategoryRestConfiguration = this.getConfiguration();
        configuration.showAuthorizedFascicolable = value;
        sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
    }

    setShowManagerParam(value: string): void {
        let configuration: uscCategoryRestConfiguration = this.getConfiguration();
        configuration.showManagerFascicolable = value;
        sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
    }

    setShowSecretaryParam(value: string): void {
        let configuration: uscCategoryRestConfiguration = this.getConfiguration();
        configuration.showSecretaryFascicolable = value;
        sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
    }

    setShowRoleParam(value?: number): void {
        let configuration: uscCategoryRestConfiguration = this.getConfiguration();
        configuration.showRoleFascicolable = value;
        sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
    }

    setShowContainerParam(value?: number): void {
        let configuration: uscCategoryRestConfiguration = this.getConfiguration();
        configuration.showContainerFascicolable = value;
        sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
    }

    setFascicleTypeParam(value?: FascicleType): void {
        let configuration: uscCategoryRestConfiguration = this.getConfiguration();
        configuration.fascicleType = value;
        sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
    }

    addDefaultCategory(idCategory: number, onlyFascicolable: boolean = false): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._treeCategory.get_nodes().clear();
        this._treeCategory.trackChanges();

        let finderAction: Function = (id, callback, error) => this._categoryService.findTreeCategory(id, null, callback, error);
        if (onlyFascicolable) {
            finderAction = (id, callback, error) => this._categoryService.findFascicolableCategory(id, callback, error);
        }

        finderAction(idCategory,
            (data: any) => {
                if (!data || data.Code == 0) {
                    return promise.resolve();
                }

                this.createNode(data as CategoryTreeViewModel)
                    .done((node) => {
                        node.select();
                        node.get_attributes().setAttribute("IsSelected", true);
                        node.get_attributes().setAttribute("NodeType", ProcessNodeType.Category);
                        this.addToSelectedSource(data);
                        this._treeCategory.commitChanges();                        
                        $(`#${this.pnlMainContentId}`).triggerHandler(uscCategoryRest.ADDED_EVENT, data.IdCategory);
                        setTimeout(() => this._ajaxManager.ajaxRequest("Add"), 800);
                        promise.resolve();
                    })
                    .fail((exception: ExceptionDTO) => {
                        this._treeCategory.commitChanges();
                        promise.reject(exception);
                    });
            },
            (exception: ExceptionDTO) => {
                this._treeCategory.commitChanges();
                promise.reject(exception);
            }
        );        
        return promise.promise();
    }

    disableButtons(): void {
        this._actionToolbar.get_items().forEach(function (item: Telerik.Web.UI.RadToolBarItem) {
            item.set_enabled(false);
        });
    }

    setToolbarVisibilityButtons(): void {
        $(`#${this.actionToolbarId}`).hide();
    }

    enableButtons(): void {
        this._actionToolbar.get_items().forEach(function (item: Telerik.Web.UI.RadToolBarItem) {
            item.set_enabled(true);
        });
    }

    public updateSessionStorageSelectedCategory(category: CategoryTreeViewModel) {
        sessionStorage.setItem(this._selectedCategorySessionKey, JSON.stringify([category]));
    }

    public populateCategotyTree(category: CategoryTreeViewModel) {
        this._treeCategory.get_nodes().clear();
        let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        node.set_text(`${category.Code}.${category.Name}`);
        node.set_value(category.IdCategory);
        this._treeCategory.get_nodes().add(node);
    }

    clearTree() {
        this._treeCategory.get_nodes().clear();
    }

    getCategoryFascicles(categoryId: number): JQueryPromise<CategoryFascicleViewModel[]> {
        let promise: JQueryDeferred<CategoryFascicleViewModel[]> = $.Deferred<CategoryFascicleViewModel[]>();
        this._categoryService.getCategoriesByIds([categoryId], this.currentTenantAOOId, (data) => {
            if (!data) return;
            let category: CategoryModel = data[0];
            promise.resolve(category.CategoryFascicles);
        }, this.showNotificationException);
        return promise.promise();
    }
}

export = uscCategoryRest;