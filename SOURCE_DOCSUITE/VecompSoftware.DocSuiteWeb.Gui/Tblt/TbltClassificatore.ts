/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import WindowHelper = require('App/Helpers/WindowHelper');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import UscFasciclePlan = require('UserControl/UscFasciclePlan');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import CategoryFascicleService = require('App/Services/Commons/CategoryFascicleService');
import CategoryFascicleModel = require('App/Models/Commons/CategoryFascicleModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import AjaxModel = require('App/Models/AjaxModel');



class TbltClassificatore {

    isCategoryManager: boolean = false;
    btnCreateFascicleId: string;
    ajaxManagerId: string;
    treeViewCategoryId: string;
    splPageID: string;
    radWindowManagerId: string;
    ajaxLoadingPanelId: string;
    signalRServerAddress: string;
    uscNotificationId: string;
    uscFasciclePlanId: string;
    pnlDetailsId: string;
    pnlFasciclePlanId: string;
    detailsVisible: boolean;
    metadataDetailsId: string;
    lblMetadataId: string;
    metadataRepositoryEnabled: boolean;
    fascicleContainerEnabled: boolean;
    pnlSettoriId: string;
    actionsToolbarId: string;

    private _categoryFascicleService: CategoryFascicleService;
    private _pnlFasciclePlan: JQuery;
    private _metadataRepositoryService: MetadataRepositoryService;
    private _serviceConfigurations: ServiceConfiguration[];
    private _lblMetadata: JQuery;
    private _actionToolbar: Telerik.Web.UI.RadToolBar;

    private static ADD_COMMANDNAME = "AddCategory";
    private static DELETE_COMMANDNAME = "DeleteCategory";
    private static EDIT_COMMANDNAME = "EditCategory";
    private static RECOVER_COMMANDNAME = "RecoverCategory";
    private static LOG_COMMANDNAME = "LogCategory";
    private static ADDMASSIMARIO_COMMANDNAME = "AddMassimario";
    private static ADDMETADATA_COMMANDNAME = "AddMetadata";
    private static RUNFASCICLEPLAN_COMMANDNAME = "RunFasciclePlan";
    private static CLOSEFASCICLEPLAN_COMMANDNAME = "CloseFasciclePlan";

    private get toolbarActions(): Array<[string, () => void]> {
        let items: Array<[string, () => void]> = [
            [TbltClassificatore.ADD_COMMANDNAME, () => this.openEditWindow('rwEdit', 'Add')],
            [TbltClassificatore.DELETE_COMMANDNAME, () => this.openEditWindow('rwEdit', 'Delete')],
            [TbltClassificatore.EDIT_COMMANDNAME, () => this.openEditWindow('rwEdit', 'Rename')],
            [TbltClassificatore.RECOVER_COMMANDNAME, () => this.openEditWindow('rwEdit', 'Recovery')],
            [TbltClassificatore.LOG_COMMANDNAME, () => this.openLogWindow('rwLog')],
            [TbltClassificatore.ADDMASSIMARIO_COMMANDNAME, () => this.openAddMassimarioScartoWindow('rwAddMassimario')],
            [TbltClassificatore.ADDMETADATA_COMMANDNAME, () => this.openAddMetadataRepository()],
            [TbltClassificatore.RUNFASCICLEPLAN_COMMANDNAME, () => this.runFasciclePlan()],
            [TbltClassificatore.CLOSEFASCICLEPLAN_COMMANDNAME, () => this.closeFasciclePlan()]
        ];
        return items;
    }

    /**
     * Costruttore
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     * Initialize
     */
    initialize(): void {
        let wndManager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        wndManager.getWindowByName('rwEdit').add_close(this.onCloseFunctionCallback);
        wndManager.getWindowByName('rwLog').add_close(this.onCloseFunctionCallback);
        wndManager.getWindowByName('rwAddMassimario').add_close(this.onCloseMetadataCallback);
        wndManager.getWindowByName('rwMetadata').add_close(this.onCloseFunctionCallback);
        let categoryFascicleService: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "CategoryFascicle");
        this._categoryFascicleService = new CategoryFascicleService(categoryFascicleService);
        this._pnlFasciclePlan = $("#".concat(this.pnlFasciclePlanId));
        this._pnlFasciclePlan.hide()
        this.detailsVisible = false;
        if (this.metadataRepositoryEnabled) {
            this._metadataRepositoryService = new MetadataRepositoryService(ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository"));
        }

        this._actionToolbar = $find(this.actionsToolbarId) as Telerik.Web.UI.RadToolBar;
        this._actionToolbar.add_buttonClicked(this.actionToolbar_ButtonClicked);
    }

    /**
     *------------------------- Events -----------------------------
     */
    treeView_ClientNodeExpanding = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let node: Telerik.Web.UI.RadTreeNode = args.get_node();
        if (node.get_nodes().get_count() > 0) {
            let oldExpandMode: Telerik.Web.UI.TreeNodeExpandMode = node.get_expandMode();
            node.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
            node.expand();
            node.set_expandMode(oldExpandMode);
            args.set_cancel(true);
        }
    }

    treeView_ClientNodeExpanded = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let node: Telerik.Web.UI.RadTreeNode = args.get_node();
        if (node.get_nodes().get_count() == 0) {
            var params: string = "ExpandNode|" + node.get_value() + "|" + node.toJsonString();
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(params);
        }
    }

    /**
     * Evento scatenato al click di un nodo
     * @method
     * @param sender
     * @param eventArgs
     * @returns
     */
    treeView_ClientNodeClicked = (sender: Telerik.Web.UI.RadTreeView, eventArgs: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let node: Telerik.Web.UI.RadTreeNode = eventArgs.get_node();
        sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        this.loadDetail(node);
    }

    loadDetail(node: Telerik.Web.UI.RadTreeNode) {
        this._pnlFasciclePlan = $("#".concat(this.pnlFasciclePlanId));
        node.expand();
        this._pnlFasciclePlan.show();
        this.detailsVisible = true;

        let btnCreateFascicle: HTMLButtonElement = <HTMLButtonElement>$get(this.btnCreateFascicleId);
        if (btnCreateFascicle) {
            btnCreateFascicle.innerText = "Piano di fascicolazione";
        }
        let strNodeType: string = <string>node.get_attributes().getAttribute("NodeType");
        if (strNodeType != "Root") {
            let strActive: boolean = node.get_attributes().getAttribute("Active") == 'True';
            let isLeaf: boolean = (node.get_attributes().getAttribute("HasChildren") == "False");
            let hasProcedureFascicle: boolean = (node.get_attributes().getAttribute("HasProcedureFascicle").toUpperCase() === 'true'.toUpperCase());
            let isRecoverable: boolean = (node.get_attributes().getAttribute("IsRecoverable") == "True");
            let isSubFascicle: boolean = (node.get_attributes().getAttribute("IsSubFascicle") == "True");
            this.setEnabledButtons(strNodeType, strActive, isLeaf, hasProcedureFascicle, isRecoverable, isSubFascicle);
            let uscFasciclePlan: UscFasciclePlan = <UscFasciclePlan>$("#".concat(this.uscFasciclePlanId)).data();
            if (!jQuery.isEmptyObject(uscFasciclePlan)) {
                uscFasciclePlan.loadNodes(node.get_value());
            }
        } else {
            this.setEnabledButtons(strNodeType, true, false, false, false, false);
        }
    }

    loadDetailSelectedNode() {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        this.loadDetail(selectedNode);
    }

    /**
     * Evento che dispone la visibilità degli elementi del context menu
     * @method
     * @param sender
     * @param args
     * @returns
     */
    onContextMenuShowing = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeViewContextMenuEventArgs) => {
        let treeNode: Telerik.Web.UI.RadTreeNode = args.get_node();
        let menu: Telerik.Web.UI.RadMenu = args.get_menu();
        treeNode.set_selected(true);
        let strNodeType: string = <string>treeNode.get_attributes().getAttribute("NodeType");
        let active: boolean = treeNode.get_attributes().getAttribute("Active") == "True";
        let isLeaf: boolean = (treeNode.get_attributes().getAttribute("HasChildren") == "False");
        let isRecoverable: boolean = (treeNode.get_attributes().getAttribute("IsRecoverable") == "True");
        switch (strNodeType) {
            case "Category":
            case "SubCategory":
                menu.findItemByValue('Add').enable();
                menu.findItemByValue('Rename').enable();
                if (active) {
                    menu.findItemByValue('Recovery').disable();
                    if (isLeaf) {
                        menu.findItemByValue('Delete').enable();
                    } else {
                        menu.findItemByValue('Delete').disable();
                    }
                } else {
                    menu.findItemByValue('Add').disable();
                    menu.findItemByValue('Rename').disable();
                    (isRecoverable) ? menu.findItemByValue('Recovery').enable() : menu.findItemByValue('Recovery').disable();
                    menu.findItemByValue('Delete').disable();
                }
                menu.findItemByValue('Log').enable();
                break;
            default:
                menu.findItemByValue('Add').enable();
                menu.findItemByValue('Rename').disable();
                menu.findItemByValue('Recovery').disable();
                menu.findItemByValue('Delete').disable();
                menu.findItemByValue('Log').disable();
                break;
        }
        this.alignButtons(menu);
    }

    /**
     * Evento scatenato al click di un elemento del context menu
     * @method
     * @param sender
     * @param args
     * @returns
     */
    onContextMenuItemClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeViewContextMenuEventArgs) => {
        let menuItem = args.get_menuItem();
        switch (menuItem.get_value()) {
            case "Add":
            case "Rename":
            case "Delete":
            case "Recovery":
                this.openEditWindow('rwEdit', menuItem.get_value());
                break;
            case "Log":
                this.openLogWindow('rwLog');
                break;
        }
    }

    /**
     * Evento scatenato dopo la chiusura della finestra di editazione di un nodo
     * @method
     * @param sender
     * @param args
     * @returns
     */
    onCloseFunctionCallback = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            this.updateCategories(args.get_argument());
        }
    }

    /**
     * Evento scatenato dopo la chiusura della finestra di associazione metadati
     * @method
     * @param sender
     * @param args
     * @returns
     */
    onCloseMetadataCallback = (sender: any, args: Telerik.Web.UI.WindowCloseCancelEventArgs) => {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
        let params: string = "ReloadNodes|".concat(treeView.get_selectedNode().get_value());        
        if (<boolean>args.get_argument()) {
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(params);
        }
    }

    /**
     * Callback per modifica nodo di classificatore
     * @method
     * @param existGroups
     * @param source
     * @returns
     */
    renameCallback = (existGroups: boolean, source: any) => {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
        let node: Telerik.Web.UI.RadTreeNode = treeView.findNodeByValue(source.value);
        (<any>node)._loadFromDictionary(source);
        node.get_attributes().setAttribute("Code", source.attributes.Code);
        let parentNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode().get_parent();
        this.orderPositionNode(node, parentNode);
    }

    /**
     *------------------------- Methods -----------------------------
     */

    /**
     * Allinea i pulsanti con il contextMenu
     * @method
     * @param menu
     * @returns
     */
    alignButtons(menu: Telerik.Web.UI.RadMenu): void {
        let btnAggiungi: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.ADD_COMMANDNAME);
        let btnModifica: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.EDIT_COMMANDNAME);
        let btnElimina: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.DELETE_COMMANDNAME);
        let btnRecovery: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.RECOVER_COMMANDNAME);
        let btnLog: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.LOG_COMMANDNAME);
        let btnMassimarioScarto: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.ADDMASSIMARIO_COMMANDNAME);
        if (btnAggiungi) btnAggiungi.set_enabled(menu.findItemByValue('Add').get_enabled());
        if (btnModifica) btnAggiungi.set_enabled(menu.findItemByValue('Rename').get_enabled());
        if (btnLog) btnLog.set_enabled(menu.findItemByValue('Log').get_enabled());
        if (btnRecovery) btnRecovery.set_enabled(menu.findItemByValue('Recovery').get_enabled());
        if (btnElimina) btnElimina.set_enabled(menu.findItemByValue('Delete').get_enabled());
        if (btnMassimarioScarto) btnMassimarioScarto.set_enabled(menu.findItemByValue('Rename').get_enabled());
    }

    /**
     * Setta la visibilità dei pulsanti
     * @method
     * @param strNodeType
     * @param active
     * @param isLeaf
     * @param hasProcedureFascicle
     * @param mustHaveFascicle
     * @returns
     */
    setEnabledButtons(strNodeType: string, active: boolean, isLeaf: boolean, hasProcedureFascicle: boolean, isRecoverable: boolean, isSubFascicle: boolean): void {
        let btnAggiungi: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.ADD_COMMANDNAME);
        let btnModifica: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.EDIT_COMMANDNAME);
        let btnElimina: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.DELETE_COMMANDNAME);
        let btnRecovery: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.RECOVER_COMMANDNAME);
        let btnLog: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.LOG_COMMANDNAME);
        let btnMassimarioScarto: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.ADDMASSIMARIO_COMMANDNAME);
        let btnMetadata: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.ADDMETADATA_COMMANDNAME);
        let btnRunFasciclePlan: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.RUNFASCICLEPLAN_COMMANDNAME);
        let btnCloseFasciclePlan: Telerik.Web.UI.RadToolBarButton = this._actionToolbar.findButtonByCommandName(TbltClassificatore.CLOSEFASCICLEPLAN_COMMANDNAME);

        switch (strNodeType) {
            case "Category":
            case "SubCategory":
                if (btnAggiungi) btnAggiungi.set_enabled(active);
                if (btnModifica) btnModifica.set_enabled(active);
                if (btnMassimarioScarto) btnMassimarioScarto.set_enabled(active);
                if (btnLog) btnLog.set_enabled(true);
                if (active) {
                    if (btnRecovery) btnRecovery.set_enabled(false);
                    if (btnElimina) btnElimina.set_enabled(isLeaf);
                } else {
                    if (btnRecovery) btnRecovery.set_enabled(isRecoverable);
                    if (btnElimina) btnElimina.set_enabled(false);
                }
                if (btnMetadata) btnMetadata.set_enabled(active);
                if (btnRunFasciclePlan) btnRunFasciclePlan.set_enabled(!hasProcedureFascicle || isSubFascicle);
                if (btnCloseFasciclePlan) btnCloseFasciclePlan.set_enabled(hasProcedureFascicle && !isSubFascicle);
                break;
            case "Group":
                if (btnAggiungi) btnAggiungi.set_enabled(false);
                if (btnModifica) btnModifica.set_enabled(false);
                if (btnMassimarioScarto) btnMassimarioScarto.set_enabled(false);
                if (btnLog) btnLog.set_enabled(false);
                if (btnRecovery) btnRecovery.set_enabled(false);
                if (btnElimina) btnElimina.set_enabled(false);

                if (btnMetadata) btnMetadata.set_enabled(false);
                break;
            default:
                if (btnAggiungi) btnAggiungi.set_enabled(true);
                if (btnModifica) btnModifica.set_enabled(false);
                if (btnMassimarioScarto) btnMassimarioScarto.set_enabled(false);
                if (btnLog) btnLog.set_enabled(false);
                if (btnRecovery) btnRecovery.set_enabled(false);
                if (btnElimina) btnElimina.set_enabled(false);
                if (btnMetadata) btnMetadata.set_enabled(false);
                if (btnRunFasciclePlan) btnRunFasciclePlan.set_enabled(!hasProcedureFascicle);
                if (btnCloseFasciclePlan) btnCloseFasciclePlan.set_enabled(hasProcedureFascicle);
                break;
        }
    }

    /**
     * Inserisce un nuovo nodo nella treeview e ne ordina la visualizzazione
     * @method
     * @param node
     * @param parentNode
     * @returns
     */
    private orderPositionNode(node: Telerik.Web.UI.RadTreeNode, parentNode: Telerik.Web.UI.RadTreeNode): void {
        let position: number = 0;
        $.each(parentNode.get_allNodes(), function (index: number, pNode: Telerik.Web.UI.RadTreeNode) {
            if ((<number>(node.get_attributes().getAttribute("Code"))) > (<number>pNode.get_attributes().getAttribute("Code"))) {
                position = pNode.get_index() + 1;
            }
        });
        parentNode.get_nodes().insert(position, node);
    }

    /**
     * Metodo di gestione nodi di classificatore
     * @method
     * @param category
     * @returns
     */
    updateCategories(operator: string): void {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();

        if (operator == 'Delete') {
            selectedNode = selectedNode.get_parent();
        }

        let params: string = "ReloadNodes";
        if (selectedNode && selectedNode.get_value()) {
            params = params.concat('|', selectedNode.get_value());
        }
        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(params);
    }

    /**
     * Aggiorno la visibilità dei pulsanti post reload dei nodi
     */
    updateCategoriesCallback(): void {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        this._pnlFasciclePlan = $("#".concat(this.pnlFasciclePlanId));
        let uscFasciclePlan: UscFasciclePlan = <UscFasciclePlan>$("#".concat(this.uscFasciclePlanId)).data();
        uscFasciclePlan.loadNodes(selectedNode.get_value());
        this.detailsVisible = true;

        if (selectedNode && selectedNode.get_attributes().getAttribute("NodeType") != 'Root') {
            let strNodeType: string = <string>selectedNode.get_attributes().getAttribute("NodeType");
            let active: boolean = selectedNode.get_attributes().getAttribute("Active") == 'True';
            let isLeaf: boolean = (selectedNode.get_attributes().getAttribute("HasChildren") == "False");
            let isRecoverable: boolean = (selectedNode.get_attributes().getAttribute("IsRecoverable") == "True");
            let hasProcedureFascicle: boolean = (selectedNode.get_attributes().getAttribute("HasProcedureFascicle").toUpperCase() === 'true'.toUpperCase());
            let isSubFascicle: boolean = (selectedNode.get_attributes().getAttribute("IsSubFascicle") == "True");
            this.setEnabledButtons(strNodeType, active, isLeaf, hasProcedureFascicle, isRecoverable, isSubFascicle);
        }
        else {
            this.setEnabledButtons('Root', true, false, false, false, false);
        }
    }

    /**
     * Apre la finestra di editazione di un nodo (aggiunta,eliminazione,modifica)
     * @method
     * @param name
     * @param operation
     * @returns
     */
    openEditWindow(name: string, operation: string): boolean {
        let parameters: string = "Action=".concat(operation);

        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        let active: boolean = selectedNode.get_attributes().getAttribute("Active") == '1';

        switch (operation) {
            case "Add":
            case "Rename":
            case "Delete":
                parameters = parameters.concat("&CategoryID=", selectedNode.get_value());
                break;
            case "Recovery":
                if (active) {
                    this.showWarningMessage(this.uscNotificationId, "Classificatore già attivo!");
                    return false;
                }
                parameters = parameters.concat("&CategoryID=", selectedNode.get_value());
                break;
        }
        let url: string = "../Tblt/TbltClassificatoreGesNew.aspx?".concat(parameters);
        return this.openWindow(url, name, WindowHelper.WIDTH_EDIT_WINDOW, WindowHelper.HEIGHT_EDIT_WINDOW);
    }

    /**
     * Apre la finestra con i Log per il nodo selezione
     * @method
     * @param name
     * @returns
     */
    openLogWindow(name: string): boolean {
        let url: string = "../Tblt/TbltLog.aspx?Type=Comm&TableName=Category";
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();

        if (selectedNode) {
            var attributes = selectedNode.get_attributes();
            url += "&entityUniqueId=".concat(attributes.getAttribute("UniqueId"));
        }
        return this.openWindow(url, name, WindowHelper.WIDTH_LOG_WINDOW, WindowHelper.HEIGHT_LOG_WINDOW);
    }

    /**
     * Apre la finestra per la creazione di un fascicolo
     * @method
     * @param name
     * @returns
     */
    loadEnvironmentNodesImage(): boolean {
        this._pnlFasciclePlan = $("#".concat(this.pnlFasciclePlanId));
        let btnCreateFascicle: HTMLButtonElement = <HTMLButtonElement>$get(this.btnCreateFascicleId);
        if (this.detailsVisible) {
            this.detailsVisible = false;
            let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
            let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
            this._pnlFasciclePlan.show();
            let uscFasciclePlan: UscFasciclePlan = <UscFasciclePlan>$("#".concat(this.uscFasciclePlanId)).data();
            if (!jQuery.isEmptyObject(uscFasciclePlan)) {
                //   uscFasciclePlan.setNodesAttributes(selectedNode.get_value());
            }
            if (btnCreateFascicle) {
                btnCreateFascicle.innerText = "Dettagli";
            }
        } else {
            this.detailsVisible = true;
            this._pnlFasciclePlan.hide();
            if (btnCreateFascicle) {
                btnCreateFascicle.innerText = "Piano di fascicolazione";
            }
        }
        return true;
    }

    /**
     * Apre la finestra per l'associazione di un massimario di scarto
     * @method
     * @param name
     * @returns
     */
    openAddMassimarioScartoWindow(name: string): boolean {
        let url: string = "../Tblt/TbltMassimariScarto.aspx?Type=Comm";
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        url = url.concat("&CategoryID=", selectedNode.get_value());
        return this.openWindow(url, name, WindowHelper.WIDTH_LOG_WINDOW, WindowHelper.HEIGHT_EDIT_WINDOW);
    }

    /**
     * Apre la finestra per l'associazione di metadati
     * @method
     * @param name
     * @returns
     */
    openAddMetadataRepository(): boolean {
        let url: string = "../Tblt/TbltCategoryMetadata.aspx?Type=Comm";
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        url = url.concat("&CategoryID=", selectedNode.get_value());
        if (selectedNode.get_attributes().getAttribute('MetadataRepository')) {
            url = url.concat("&IdMetadata=", selectedNode.get_attributes().getAttribute('MetadataRepository'));
        }
        return this.openWindow(url, 'rwMetadata', WindowHelper.WIDTH_LOG_WINDOW, WindowHelper.HEIGHT_EDIT_WINDOW);
    }

    closeFasciclePlan(): boolean {
        let wndManager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        wndManager.radconfirm("Sicuro di voler procedere?", (arg) => {
            if (arg) {
                let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
                let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
                this._categoryFascicleService.getAvailableProcedureCategoryFascicleByCategory(selectedNode.get_value(),
                    (data: CategoryFascicleModel[]) => {
                        if (data) {
                            if (data && data.length > 0) {
                                let categoryFascicle: CategoryFascicleModel = data[0];
                                this._categoryFascicleService.deleteCategoryFascicle(categoryFascicle,
                                    (data: any) => {
                                        let params: string = `ResetPeriodicCategoryFascicles|${selectedNode.get_value()}`;
                                        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(params);                  
                                    },
                                    (exception: ExceptionDTO) => {
                                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                                        if (!jQuery.isEmptyObject(uscNotification)) {
                                            uscNotification.showNotification(exception);
                                        }
                                    });
                            }
                        }
                    },
                    (exception: ExceptionDTO) => {
                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                        if (!jQuery.isEmptyObject(uscNotification)) {
                            uscNotification.showNotification(exception);
                        }
                    });
            }
        });
        return true;
    }

    runFasciclePlan(): boolean {
        let wndManager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        wndManager.radconfirm("Sei sicuro di avviare un piano di fascicolazione per tutte le tipologie documentarie?", (arg) => {
            let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
            if (arg) {
                let rootNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
                let item = <CategoryFascicleModel>{};
                let ajaxModel: AjaxModel = <AjaxModel>{};
                ajaxModel.Value = new Array<string>();
                item.FascicleType = FascicleType['Procedure'];
                item.DSWEnvironment = 0;
                let category: CategoryModel = new CategoryModel;
                category.EntityShortId = rootNode.get_value();
                category.Id = rootNode.get_value();
                item.Category = category;
                ajaxModel.Value.push(JSON.stringify(item));
                ajaxModel.ActionName = "ProcedureExternalDataCallback";
                (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
                treeView.commitChanges();
            }
        });
        return true
    }

    /**               
     * Apre una nuova radwindow con dati personalizzati
     * @method
     * @param url
     * @param name
     * @param width
     * @param height
     * @returns
     */
    openWindow(url: string, name: string, width: number, height: number): boolean {
        let wnd: Telerik.Web.UI.RadWindow = (<Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId)).open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }

    showLoadingPanelByElement(elementID: string): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.show(elementID);
    }

    closeLoadingPanelByElement(elementID: string): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.hide(elementID);
    }

    updateVisibility(visibility: boolean) {
        this.detailsVisible = visibility;
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.treeViewCategoryId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        let strNodeType: string = <string>selectedNode.get_attributes().getAttribute("NodeType");
        let btnCreateFascicle: HTMLButtonElement = <HTMLButtonElement>$get(this.btnCreateFascicleId);
        if (btnCreateFascicle) {
            btnCreateFascicle.innerText = "Piano di fascicolazione";
            btnCreateFascicle.disabled = true;
        }
        this.setEnabledButtons(strNodeType, true, false, false, false, false);
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }

    protected showWarningMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showWarningMessage(customMessage)
        }
    }

    loadMetadataName(id: string) {
        if (this.metadataRepositoryEnabled) {
            this._lblMetadata = $("#".concat(this.lblMetadataId));
            this._metadataRepositoryService.getNameById(id,
                (data: any) => {
                    if (data) {
                        this._lblMetadata.html(data);
                    }
                },
                (exception: ExceptionDTO) => {
                    let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                    if (!jQuery.isEmptyObject(uscNotification)) {
                        uscNotification.showNotification(exception);
                    }
                }
            );
        }
    }

    setVisibilityPanel(number: string) {
        var numberOfSettori = parseInt(number, 10);

        var _pnlDetailsId = $("#".concat(this.pnlDetailsId));

        if (numberOfSettori == 0) {

            _pnlDetailsId.hide();
        }
    }

    sendAjaxRequest(action: string) {
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value.push(JSON.stringify("ReloadRoleUsers"));
        ajaxModel.ActionName = "ReloadRoleUsers";
        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
    }

    protected actionToolbar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let currentActionButtonItem: Telerik.Web.UI.RadToolBarButton = args.get_item() as Telerik.Web.UI.RadToolBarButton;
        let currentAction: () => void = this.toolbarActions.filter((item: [string, () => void]) => item[0] == currentActionButtonItem.get_commandName())
            .map((item: [string, () => void]) => item[1])[0];
        currentAction();
    }

    private showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception && exception instanceof ExceptionDTO) {
                uscNotification.showNotification(exception);
            }
            else {
                uscNotification.showNotificationMessage(customMessage);
            }
        }
    }
}

export = TbltClassificatore;