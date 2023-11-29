/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import FascicleSummaryFolderViewModel = require('App/ViewModels/Fascicles/FascicleSummaryFolderViewModel');
import RadTreeNodeViewModel = require('App/ViewModels/Telerik/RadTreeNodeViewModel');
import FascicleFolderStatus = require('App/Models/Fascicles/FascicleFolderStatus');
import FascicleFolderTypology = require('App/Models/Fascicles/FascicleFolderTypology');
import FascicleFolderService = require('App/Services/Fascicles/FascicleFolderService');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import AjaxModel = require('App/Models/AjaxModel');
import UpdateActionType = require("App/Models/UpdateActionType");
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleService = require('App/Services/Fascicles/FascicleService');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import FascMoveItems = require('Fasc/FascMoveItems');
import FascicleMoveItemViewModel = require('App/ViewModels/Fascicles/FascicleMoveItemViewModel');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import uscErrorNotification = require('UserControl/uscErrorNotification');
import FascicleDocumentService = require('App/Services/Fascicles/FascicleDocumentService');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import Guid = require('App/Helpers/GuidHelper');
import uscStartWorkflow = require('./uscStartWorkflow');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

class uscFascicleFolders {

    pageId: string;
    ajaxManagerId: string;
    treeFascicleFoldersId: string;
    pageContentId: string;
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    currentFascicleId: string;
    folderToolBarId: string;
    managerCreateFolderId: string;
    managerModifyFolderId: string;
    managerMoveFolderId: string;
    typology: FascicleFolderTypology;
    fascicleId: string;
    managerWindowsId: string;
    managerId: string;
    btnExpandFascicleFoldersId: string;
    pnlFascicleFolderId: string;
    isVisible: boolean;
    hideInternalInitializeLoading: boolean;
    foldersToDisabled: string[];
    viewOnlyFolders: boolean;
    doNotUpdateDatabase: string;
    fascicleFoldersModel: FascicleSummaryFolderViewModel[];
    chainId: string;
    scannerLightRestEnabled: string;

    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _serviceConfigurations: ServiceConfiguration[];
    private _service: FascicleService;
    private _lblRegistrationUser: JQuery;
    private _treeFascicleFolders: Telerik.Web.UI.RadTreeView;
    private _uscNotification: UscErrorNotification;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _folderToolBar: Telerik.Web.UI.RadToolBar;
    private _managerCreateFolder: Telerik.Web.UI.RadWindowManager;
    private _managerModifyFolder: Telerik.Web.UI.RadWindowManager;
    private _managerMoveFolder: Telerik.Web.UI.RadWindow;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _checkedToolBarButtons: number;
    private _btnCreateFolder: Telerik.Web.UI.RadToolBarItem;
    private _btnDeleteFolder: Telerik.Web.UI.RadToolBarItem;
    private _btnModifyFolder: Telerik.Web.UI.RadToolBarItem;
    private _btnRefreshFolders: Telerik.Web.UI.RadToolBarItem;
    private _btnMoveFolder: Telerik.Web.UI.RadToolBarItem;
    private _btnUploadScanner: Telerik.Web.UI.RadToolBarItem;
    private _btnUploadFile: Telerik.Web.UI.RadToolBarItem;
    private _fascicleFolderService: FascicleFolderService;
    private _fascicleService: FascicleService;
    private _btnExpandFascicleFolders: Telerik.Web.UI.RadButton;
    private _pnlFascicleFolder: JQuery;
    private _isPnlFascicleFolderOpen: boolean;

    public static ON_END_LOAD_EVENT = "onEndLoad";
    public static LOADED_EVENT: string = "onLoaded";
    public static FASCICLE_TREE_NODE_CLICK: string = "onFascicleTreeNodeClick";
    public static SUBFASCICLE_TREE_NODE_CLICK: string = "onSubFascicleTreeNodeClick";
    public static ROOT_NODE_CLICK: string = "onRootTreeNodeClick";
    public static RESIZE_EVENT: string = "onResize";
    public static REFRESH_GRID_EVENT: string = "onRefresh";
    public static REFRESH_GRID_UPLOAD_DOCUMENTS: string = "onGridUpdateDocument";

    private folder_close_path: string = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
    private folder_open_path: string = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
    private folder_internet_close_path: string = "../App_Themes/DocSuite2008/imgset16/folder_internet_closed.png";
    private folder_internet_open_path: string = "../App_Themes/DocSuite2008/imgset16/folder_internet_open.png";
    private folder_auto_close_path: string = "../App_Themes/DocSuite2008/imgset16/folder_auto_closed.png";
    private folder_auto_open_path: string = "../App_Themes/DocSuite2008/imgset16/folder_auto_open.png";

    private tooltip_folder = "Cartella con sottocartelle";
    private tooltip_internet_folder = "Cartella pubblica";
    private tooltip_auto_folder = "Cartella di sottofascicolo";

    public SESSION_FascicleHierarchy;
    public SESSION_FascicleId: string = "FascicleId_Tree";
    public TYPOLOGY_ATTRIBUTE: string = "Typology";
    public HASCHILDREN_ATTRIBUTE: string = "hasChildren";

    public static REFRESH_FolderHierarchy: string = "refreshFolder";
    public static UPLOAD_file: string = "uploadFile";
    public static SCANNER: string = "scanner";

    public static SCAN_DOCUMENT: string = "Scan_document";
    public static CHAIN_ID: string = "chainId";


    private _fascicleDocumentService: FascicleDocumentService;
    /*
    * Costruttore
    * @param webApiConfiguration
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }



    /**
    *---------------------------- Events ---------------------------
    */

    /**
    * Evento scatenato al click della toolbar delle cartelle
    */

    folderToolBar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {

        let item: Telerik.Web.UI.RadToolBarItem = args.get_item();
        if (item) {
            let rootNode: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.get_nodes().getNode(0);
            let currentSelectedNode: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.get_selectedNode();
            switch (item.get_value()) {
                case "createFolder": {
                    let selectedNodeId: string = currentSelectedNode.get_value();
                    if (!selectedNodeId) {
                        selectedNodeId = "";
                    }
                    if (selectedNodeId != "00000000-0000-0000-0000-000000000000") {
                        this.setFascicleFolder(selectedNodeId);
                    }

                    let url: string = `../Fasc/FascicleFolderInserimento.aspx?Type=Fasc&idFascicleFolder=${selectedNodeId}&SessionUniqueKey=${this.pageId}_${selectedNodeId}&DoNotUpdateDatabase=${this.doNotUpdateDatabase}`;
                    this.openWindow(url, "managerCreateFolder", 480, 300);
                    break;
                }
                case "modifyFolder": {
                    let selectedNodeId: string = currentSelectedNode.get_value();
                    if (currentSelectedNode.get_attributes()) {
                        this.setFascicleFolder(selectedNodeId);
                        let url: string = `../Fasc/FascicleFolderModifica.aspx?Type=Fasc&idFascicleFolder=${selectedNodeId}&SessionUniqueKey=${this.pageId}_${selectedNodeId}&DoNotUpdateDatabase=${this.doNotUpdateDatabase}`;
                        this.openWindow(url, "managerModifyFolder", 480, 300);
                    }
                    break;
                }
                case "moveFolder": {
                    let selectedNodeId: string = currentSelectedNode.get_value();
                    if (currentSelectedNode.get_attributes()) {

                        this.setFascicleFolder(selectedNodeId);
                        let idFascicle: string = this._treeFascicleFolders.get_nodes().getNode(0).get_value();
                        let dto: FascicleMoveItemViewModel = {} as FascicleMoveItemViewModel;
                        dto.name = currentSelectedNode.get_text();
                        dto.uniqueId = selectedNodeId;

                        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_FASC_MOVE_ITEMS, JSON.stringify([dto]));

                        let url: string = `FascMoveItems.aspx?Type=Fasc&idFascicle=${idFascicle}&ItemsType=FolderType&IdFascicleFolder=${selectedNodeId}`;
                        this.openWindow(url, "managerMoveFolder", 480, 300);
                    }
                    break;
                }
                case "deleteFolder": {

                    let hasChildren: boolean = currentSelectedNode.get_attributes().getAttribute(this.HASCHILDREN_ATTRIBUTE);
                    let hasDocuments: boolean = currentSelectedNode.get_attributes().getAttribute("hasDocuments");
                    let level: number = currentSelectedNode.get_level() + 1;
                    let hasAttribute: boolean = false;
                    if (currentSelectedNode.get_attributes().getAttribute("idCategory")) {
                        hasAttribute = true;
                    }

                    if (currentSelectedNode.get_attributes() && !hasAttribute && !hasChildren && !hasDocuments && level > 2) {
                        this.removeFascicleFolder();
                    }
                    else {
                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                        if (!jQuery.isEmptyObject(uscNotification)) {
                            let errorMessage: string = `Impossibile eliminare la cartella ${currentSelectedNode.get_text()} per le seguenti motivazioni: `;

                            if (hasChildren) {
                                errorMessage = errorMessage.concat("La cartella contiene sotto strutture che necessariamente vanno rimosse, ");
                            }
                            if (hasDocuments) {
                                errorMessage = errorMessage.concat("La cartella ha documenti associati che necessariamente vanno rimossi, ");
                            }
                            if (hasAttribute) {
                                errorMessage = errorMessage.concat("La cartella 'sotto fascicolo' è stata configurata in automatico dal sistema durante la creazione del fascicolo, ");
                            }
                            if (level < 3) {
                                errorMessage = errorMessage.concat("La cartella 'principale' è stata configurata in automatico dal sistema durante la creazione del fascicolo, ");
                            }
                            uscNotification.showWarningMessage(errorMessage);
                        }
                    }
                    break;
                }

                case uscFascicleFolders.REFRESH_FolderHierarchy: {
                    sessionStorage.removeItem(this.SESSION_FascicleHierarchy);
                    this.setRootNode(sessionStorage.getItem(this.SESSION_FascicleId));
                    this.loadFolders(sessionStorage.getItem(this.SESSION_FascicleId))
                        .done(() => this.selectFascicleNode());
                    break;
                }

                case uscFascicleFolders.UPLOAD_file: {
                    let url: string = `../UserControl/CommonUploadDocument.aspx`;
                    this.openWindow(url, "managerUploadFile", 480, 300, this.closeDocumentWnd);
                    break;
                }
                case uscFascicleFolders.SCANNER: {
                    sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_COMPONENT_SCANNER);
                    let url: string = "";
                    if (this.scannerLightRestEnabled.toLocaleLowerCase() == "true") {
                        url = `../UserControl/ScannerRest.aspx?multipleEnabled=False`;
                    }
                    this.openWindow(url, "managerScannerDocument", 800, 500, this.closeScannerWnd);
                    break;
                }
            }
        }
    }

    /**
    * Evento scatenato all'espansione di un nodo
    * @param sender
    * @param args
    */
    treeView_ClientNodeExpanding = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        if (!args.get_node()) {
            return;
        }
        let node: Telerik.Web.UI.RadTreeNode = args.get_node();

        node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);

        if (this.doNotUpdateDatabase === "False") {
            this._fascicleFolderService.getChildren(node.get_value(),
                (data: any) => {
                    this.loadNodes(data, node);
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

    /**
    *---------------------------- Methods ---------------------------
    */
    /**
   * Inizializzazione
   */
    initialize() {
        if (!this.isVisible) {
            return;
        }
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._treeFascicleFolders = <Telerik.Web.UI.RadTreeView>$find(this.treeFascicleFoldersId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._folderToolBar = <Telerik.Web.UI.RadToolBar>$find(this.folderToolBarId);
        this._folderToolBar.add_buttonClicked(this.folderToolBar_ButtonClicked);
        this._managerCreateFolder = <Telerik.Web.UI.RadWindowManager>$find(this.managerCreateFolderId);
        this._managerModifyFolder = <Telerik.Web.UI.RadWindowManager>$find(this.managerModifyFolderId);
        this._managerMoveFolder = <Telerik.Web.UI.RadWindow>$find(this.managerMoveFolderId);
        this._managerMoveFolder.add_close(this.closeMoveWindow);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);
        this._btnCreateFolder = this._folderToolBar.findItemByValue("createFolder");
        this._btnDeleteFolder = this._folderToolBar.findItemByValue("deleteFolder");
        this._btnModifyFolder = this._folderToolBar.findItemByValue("modifyFolder");
        this._btnMoveFolder = this._folderToolBar.findItemByValue("moveFolder");
        this._btnUploadScanner = this._folderToolBar.findItemByValue("scanner");
        this._btnUploadFile = this._folderToolBar.findItemByValue("uploadFile");
        this._btnRefreshFolders = this._folderToolBar.findItemByValue(uscFascicleFolders.REFRESH_FolderHierarchy);
        this._managerModifyFolder.add_close(this.closeModifyWindow);
        this._checkedToolBarButtons = 0;
        this._managerCreateFolder.add_close(this.closeFolderInsertWindow);
        this._pnlFascicleFolder = $("#".concat(this.pnlFascicleFolderId));
        this._btnExpandFascicleFolders = <Telerik.Web.UI.RadButton>$find(this.btnExpandFascicleFoldersId);
        this._btnExpandFascicleFolders.addCssClass("dsw-arrow-down");
        this._pnlFascicleFolder.show();
        this._btnExpandFascicleFolders.add_clicking(this.btnExpandFascicleFolders_OnClick);
        this._isPnlFascicleFolderOpen = true;

        if (this.viewOnlyFolders) {
            this._pnlFascicleFolder.css("margin-top", "0");
        }

        if (this.doNotUpdateDatabase === "True") {
            this._btnMoveFolder.set_visible(false);
            this._btnMoveFolder.set_enabled(false);
            this._btnRefreshFolders.set_visible(false);
            this._btnRefreshFolders.set_enabled(false);
        }

        let fascicleDocumentConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleDocument");
        this._fascicleDocumentService = new FascicleDocumentService(fascicleDocumentConfiguration);

        this.SESSION_FascicleHierarchy = `FascicleHierarchy_${this.pageId}`;

        let fascicleFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleFolder");
        this._fascicleFolderService = new FascicleFolderService(fascicleFolderConfiguration);

        let fascicleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
        this._fascicleService = new FascicleService(fascicleConfiguration);

        this.clearSessionStorageCache();

        this.bindLoaded();
    }

    private clearSessionStorageCache(): void {
        if (sessionStorage.getItem(this.SESSION_FascicleHierarchy)) {
            sessionStorage.removeItem(this.SESSION_FascicleHierarchy);
        }
    }

    /**
* Evento al click del pulsante per la espandere o comprimere la gliglia delle UD presenti nel fascicolo
*/
    btnExpandFascicleFolders_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this.changeVisibilityFascicleFolders(this._isPnlFascicleFolderOpen);
        $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.RESIZE_EVENT);
    }

    changeVisibilityFascicleFolders(param: boolean): void {
        if (!param) {
            this._isPnlFascicleFolderOpen = false;
        }
        else {
            this._isPnlFascicleFolderOpen = true;
        }
    }

    /**
    * Carico le cartelle 
    */

    loadFolders(idFascicle: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._fascicleFolderService.getChildren(idFascicle,
            (data: any) => {
                this.loadNodes(data);
                this._loadingPanel.hide(this.pageId);
                promise.resolve();
            },
            (exception: ExceptionDTO): void => {
                this._loadingPanel.hide(this.pageId);
                let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
                promise.reject(exception);
            });
        return promise.promise();
    }

    fileManagementButtonsVisibility(right: boolean): void {
        if (right && this.scannerLightRestEnabled.toLocaleLowerCase() == "true") {
            this._folderToolBar.findItemByValue(uscFascicleFolders.SCANNER).set_visible(right);
        } else {
            this._folderToolBar.findItemByValue(uscFascicleFolders.SCANNER).set_visible(false);
        }
        this._folderToolBar.findItemByValue(uscFascicleFolders.UPLOAD_file).set_visible(right);
    }

    setManageFascicleFolderVisibility(right: boolean): void {
        this._treeFascicleFolders.get_attributes().setAttribute("treeViewReadOnly", right.toString());
        this._folderToolBar.set_enabled(right);
    }

    setCloseAttributeFascicleFolder(): void {
        this._treeFascicleFolders.get_attributes().setAttribute("isClosed", "true");
        this._folderToolBar.set_enabled(false);
    }

    /**
    * Imposta gli attributi di un nodo
    * @param node
    * @param dossierFolder
    */
    private setNodeAttribute(node: Telerik.Web.UI.RadTreeNode, fascicleFolder: FascicleSummaryFolderViewModel): Telerik.Web.UI.RadTreeNode {
        node.get_attributes().setAttribute(this.TYPOLOGY_ATTRIBUTE, fascicleFolder.Typology);
        node.set_text(fascicleFolder.Name);
        node.set_value(fascicleFolder.UniqueId);
        node.get_attributes().setAttribute("idParent", null);

        if (node.get_parent()) {
            let parentNode: Telerik.Web.UI.RadTreeNode = node.get_parent();
            node.get_attributes().setAttribute("idParent", parentNode.get_value());
        }
        node.get_attributes().setAttribute("idFascicle", null);
        if (fascicleFolder.idFascicle) {
            node.get_attributes().setAttribute("idFascicle", fascicleFolder.idFascicle);
        }

        node.get_attributes().setAttribute("idCategory", null);
        if (fascicleFolder.idCategory) {
            node.get_attributes().setAttribute("idCategory", fascicleFolder.idCategory);
        }

        node.get_attributes().setAttribute("hasDocuments", null);
        node.get_attributes().setAttribute("hasDocuments", fascicleFolder.hasDocuments);

        node.get_attributes().setAttribute(this.HASCHILDREN_ATTRIBUTE, null);
        node.get_attributes().setAttribute(this.HASCHILDREN_ATTRIBUTE, fascicleFolder.hasChildren);

        node.get_attributes().setAttribute("fascicleFolderLevel", null);
        node.get_attributes().setAttribute("fascicleFolderLevel", fascicleFolder.FascicleFolderLevel);

        if (!fascicleFolder.FascicleDocuments) {
            node.get_attributes().setAttribute(uscFascicleFolders.CHAIN_ID, Guid.empty);
        }
        else {
            let inserts: FascicleDocumentModel = $.grep(fascicleFolder.FascicleDocuments, (x) => x.ChainType.toString() == ChainType[ChainType.Miscellanea])[0];
            if (inserts != undefined) {
                node.get_attributes().setAttribute(uscFascicleFolders.CHAIN_ID, inserts.IdArchiveChain);
            }
        }

        node.set_imageUrl(this.folder_close_path);

        //qui scelgo l'immagine da visualizzare per il nodo
        if (FascicleFolderTypology[fascicleFolder.Typology] == FascicleFolderTypology.Fascicle) {
            node.set_toolTip("Fascicolo ");
            node.get_imageElement().title = "Fascicolo";
        }

        if (fascicleFolder.hasChildren) {
            node.set_imageUrl(this.folder_close_path);
            node.set_expandedImageUrl(this.folder_open_path);
            node.set_toolTip(this.tooltip_folder);
            node.get_imageElement().title = this.tooltip_folder;
            let nodeToAdd = new Telerik.Web.UI.RadTreeNode();
            node.get_nodes().add(nodeToAdd);
            node.set_expanded(false);
        }

        if (FascicleFolderStatus[fascicleFolder.Status] == FascicleFolderStatus.Internet) {
            node.set_imageUrl(this.folder_internet_close_path);
            node.set_expandedImageUrl(this.folder_internet_open_path);
            node.set_toolTip(this.tooltip_internet_folder);
            node.get_imageElement().title = this.tooltip_internet_folder;
        }

        if ((fascicleFolder.idCategory != null) && (FascicleFolderTypology[fascicleFolder.Typology] == FascicleFolderTypology.SubFascicle)) {
            node.set_imageUrl(this.folder_auto_close_path);
            node.set_expandedImageUrl(this.folder_auto_open_path);
            node.set_toolTip(this.tooltip_auto_folder);
            node.get_imageElement().title = this.tooltip_auto_folder;
        }

        return node;
    }

    selectFascicleNode(triggerHandler: boolean = true): void {
        let fascicleNode: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.get_nodes().getNode(0).get_nodes().getNode(0);
        this.setSelectedNode(fascicleNode, triggerHandler);
    }

    setSelectedNode(node: Telerik.Web.UI.RadTreeNode, triggerHandler: boolean = true) {
        let idFascicle: string = this._treeFascicleFolders.get_nodes().getNode(0).get_value();
        this.currentFascicleId = node.get_value();
        node.set_selected(true);
        this.setFascicleFolder(idFascicle);
        this.setVisibilityButtonsByStatus(triggerHandler);
        this.createModelInSession();
    }

    private createTreeFromSession() {
        this.fascicleFoldersModel = JSON.parse(sessionStorage.getItem(this.SESSION_FascicleHierarchy));
        this.fascicleFoldersModel = this.fascicleFoldersModel.reverse();
        this._treeFascicleFolders.get_nodes().getNode(0).expand();
    }

    private createModelInSession(): void {
        let fascicleFolderParent: FascicleSummaryFolderViewModel[] = [];
        this.getParentsById(this._treeFascicleFolders.get_selectedNode(), fascicleFolderParent);
        sessionStorage.setItem(this.SESSION_FascicleHierarchy, JSON.stringify(fascicleFolderParent));
    }

    private getParentsById(node: Telerik.Web.UI.RadTreeNode, parents: FascicleSummaryFolderViewModel[]) {
        if (node.get_level() > 0) {
            let fascFolder: FascicleSummaryFolderViewModel = <FascicleSummaryFolderViewModel>{
                Name: node.get_text(),
                UniqueId: node.get_value(),
                Typology: node.get_attributes().getAttribute(this.TYPOLOGY_ATTRIBUTE),
                hasChildren: node.get_attributes().getAttribute(this.HASCHILDREN_ATTRIBUTE)
            };
            parents.push(fascFolder);

            this.getParentsById(node.get_parent(), parents);
        }
    }

    /*
    * Imposto il valore del nodo Root 
    */
    setRootNode(fascicleId: string): void
    setRootNode(fascicleId: string, nodeText: string, setAsSelected?: boolean): void
    setRootNode(fascicleId: string, nodeText?: any, setAsSelected: boolean = true): void {
        let rootNode: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.get_nodes().getNode(0);
        if (nodeText) {
            rootNode.set_text(nodeText);
        } else {
            rootNode.set_text("Tutti i documenti");
        }
        rootNode.set_value(fascicleId);
        rootNode.set_expanded(true);
        rootNode.set_selected(setAsSelected);

        this._treeFascicleFolders.commitChanges();

        this.hiddenInputs();
        if (fascicleId !== "") {
            this.currentFascicleId = fascicleId;
            this.setFascicleFolder(fascicleId);
        }
    }

    setButtonVisibility(isManager: boolean) {
        if (!isManager) {
            $(this._folderToolBar.get_element()).hide();
        }
    }

    /**
    * Carica i dati dello user control
    */
    loadNodes(fascicleFolders: FascicleSummaryFolderViewModel[], node?: Telerik.Web.UI.RadTreeNode): void {
        if (fascicleFolders == null) return;

        let parentSelectedNode: Telerik.Web.UI.RadTreeNode

        if (node) {
            parentSelectedNode = node;
        } else {
            parentSelectedNode = this._treeFascicleFolders.get_nodes().getNode(0);
        }

        parentSelectedNode.get_nodes().clear();

        let idFascicle: string = parentSelectedNode.get_value();
        let lastSelectedFolder: FascicleSummaryFolderViewModel = this.getSelectedFascicleFolder(idFascicle);
        let newNode: Telerik.Web.UI.RadTreeNode;
        $.each(fascicleFolders, (index: number, fascicleFolder: FascicleSummaryFolderViewModel) => {
            if (this._treeFascicleFolders.findNodeByValue(fascicleFolder.UniqueId) != undefined) {
                return;
            }

            newNode = new Telerik.Web.UI.RadTreeNode();
            parentSelectedNode.get_nodes().add(newNode);
            this.setNodeAttribute(newNode, fascicleFolder);

            if (this.foldersToDisabled && this.foldersToDisabled.some((s) => s == fascicleFolder.UniqueId)) {
                newNode.set_cssClass(`${newNode.get_cssClass()} rtDisabled`);
            }

            if (lastSelectedFolder && lastSelectedFolder.UniqueId === fascicleFolder.UniqueId) {
                newNode.set_selected(true);
            }
        });
        this._treeFascicleFolders.commitChanges();
        $("#".concat(this.treeFascicleFoldersId)).triggerHandler(uscFascicleFolders.ON_END_LOAD_EVENT);

        this._loadingPanel.hide(this.pageId);
        parentSelectedNode.set_expanded(true);
        parentSelectedNode.hideLoadingStatus();
    }

    /**
    * Metodo chiamato in chiusura della radwindow di modifica di cartella
    * @param sender
    * @param args
    */
    closeModifyWindow = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument) {
            let result: AjaxModel = <AjaxModel>args.get_argument();
            let updatedNode: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.get_selectedNode();
            if (result && result.Value[0]) {

                let fascicleFolderModel: FascicleSummaryFolderViewModel = JSON.parse(result.Value[0]);
                this.setNodeAttribute(updatedNode, fascicleFolderModel)
                this.setVisibilityButtonsByStatus();
                this._treeFascicleFolders.commitChanges();
            }
        }
    }


    /**
    * Scateno l'evento di "Load Completed" del controllo
    */
    private bindLoaded(): void {
        $("#".concat(this.pageId)).data(this);
        $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.LOADED_EVENT);
    }

    /**
    * Metono chiamato in chiusura della radwindow di inserimento
    * @param sender
    * @param args
    */
    closeFolderInsertWindow = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            let result: AjaxModel = <AjaxModel>args.get_argument();
            this.addNewFolder(result);
        }
    }

    addNewFolder(ajaxModel: AjaxModel): void {
        let parentNode: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.get_selectedNode();
        if (ajaxModel) {
            let fascicleFolderModel = <FascicleSummaryFolderViewModel>{};
            fascicleFolderModel = JSON.parse(ajaxModel.Value[0]);

            if (this.doNotUpdateDatabase === "True") {
                this._btnCreateFolder.set_enabled(parentNode.get_level() > 0);
            }
            else {
                this._btnCreateFolder.set_enabled(true);
            }
            this._btnDeleteFolder.set_enabled(false);
            this._btnModifyFolder.set_enabled(false);
            this._btnMoveFolder.set_enabled(false);
            this._btnUploadScanner.set_enabled(false);
            this._btnUploadFile.set_enabled(false);

            if (parentNode != this._treeFascicleFolders.get_nodes().getNode(0)) {
                this._btnModifyFolder.set_enabled(false);
                let attributeStatus: string = parentNode.get_attributes().getAttribute(this.TYPOLOGY_ATTRIBUTE);
                this.typology = FascicleFolderTypology[attributeStatus];

                let attributeChildren: boolean = parentNode.get_attributes().getAttribute(this.HASCHILDREN_ATTRIBUTE);

                if (!attributeChildren) {
                    parentNode.get_attributes().setAttribute(this.HASCHILDREN_ATTRIBUTE, true);
                }
                parentNode.set_imageUrl(this.folder_close_path);
                parentNode.set_expandedImageUrl(this.folder_open_path)
                parentNode.set_toolTip(this.tooltip_folder);
                parentNode.get_imageElement().title = this.tooltip_folder;
                parentNode.set_selected(true);
            }

            if (parentNode.get_expanded() == false && this.doNotUpdateDatabase === "False") {
                this._fascicleFolderService.getChildren(parentNode.get_value(),
                    (data: any) => {
                        this.loadNodes(data, parentNode);
                        parentNode.set_expanded(true);
                        this._treeFascicleFolders.commitChanges();
                    },
                    (exception: ExceptionDTO) => {
                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                        if (!jQuery.isEmptyObject(uscNotification)) {
                            uscNotification.showNotification(exception);
                        }
                    }
                );
            } else {
                let nodeToAdd = new Telerik.Web.UI.RadTreeNode();
                parentNode.get_nodes().add(nodeToAdd);
                this.setNodeAttribute(nodeToAdd, fascicleFolderModel);
                parentNode.set_expanded(true);
                this._treeFascicleFolders.commitChanges();
            }
        }
    }

    closeMoveWindow = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            let fascicleId: string = this._treeFascicleFolders.get_nodes().getNode(0).get_value();
            this.setRootNode(fascicleId);
            this.loadFolders(fascicleId);
            this.setVisibilityButtonsByStatus();
        }
    }

    treeView_ClientNodeClicking = (sender: Telerik.Web.UI.RadTreeView, eventArgs: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let isSelectable: boolean = eventArgs.get_node().get_attributes().getAttribute("selectable");

        if (isSelectable !== undefined && isSelectable === false) {
            eventArgs.set_cancel(true);
            return;
        }
    }

    /**
    * Evento scatenato al click di un nodo
    * @param sender
    * @param eventArgs
    */
    treeView_ClientNodeClicked = (sender: Telerik.Web.UI.RadTreeView, eventArgs: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        if (sessionStorage.getItem(this.SESSION_FascicleHierarchy) == null ||
            sessionStorage.getItem(this.SESSION_FascicleHierarchy) == "[]") {
            this.setSelectedNode(eventArgs.get_node());
        } else {
            this.createModelInSession();
            let idFascicle: string = this._treeFascicleFolders.get_nodes().getNode(0).get_value();
            this.setFascicleFolder(idFascicle);
            this.setVisibilityButtonsByStatus();
        }
    }

    /*
    * Eliminazione di una cartellina
    */
    removeFascicleFolder = () => {
        this._manager.radconfirm("Sei sicuro di voler eliminare la cartella?", (arg) => {
            if (arg) {
                let fascicleFolder = <FascicleFolderModel>{};
                let currentSelectedNode: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.get_selectedNode();
                fascicleFolder.UniqueId = currentSelectedNode.get_value();
                fascicleFolder.Name = currentSelectedNode.get_text();
                let parentNode: Telerik.Web.UI.RadTreeNode = currentSelectedNode.get_parent();
                if (parentNode && parentNode.get_value() != this.currentFascicleId) {
                    fascicleFolder.ParentInsertId = parentNode.get_value();
                }

                if (this.doNotUpdateDatabase === "False") {
                    this._fascicleFolderService.deleteFascicleFolder(fascicleFolder,
                        (data: any) => {
                            this.removeNode(fascicleFolder.UniqueId);
                            this.hiddenInputs();
                        }, (exception: ExceptionDTO) => {
                            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                            if (!jQuery.isEmptyObject(uscNotification)) {
                                uscNotification.showNotification(exception);
                            }
                        }
                    );
                }
                else {
                    this.removeNode(fascicleFolder.UniqueId);
                    this.hiddenInputs();
                }
                this.setSelectedNode(parentNode);
            }
        }, 400, 300);
    }
    private hiddenInputs() {
        this._btnCreateFolder.set_enabled(false);
        this._btnDeleteFolder.set_enabled(false);
        this._btnModifyFolder.set_enabled(false);
        this._btnMoveFolder.set_enabled(false);
        this._btnUploadScanner.set_enabled(false);
        this._btnUploadFile.set_enabled(false);
    }

    removeNode(idFascicleFolder: string) {
        let nodeToRemove: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.findNodeByValue(idFascicleFolder);
        if (nodeToRemove) {
            let parentNode: Telerik.Web.UI.RadTreeNode = nodeToRemove.get_parent();
            if (parentNode && parentNode.get_nodes()) {
                parentNode.get_nodes().remove(nodeToRemove);
                if (parentNode != this._treeFascicleFolders.get_nodes().getNode(0) && parentNode.get_nodes().get_count() == 0) {
                    parentNode.get_attributes().setAttribute(this.HASCHILDREN_ATTRIBUTE, false);
                    parentNode.set_imageUrl(this.folder_open_path);
                }
                this._treeFascicleFolders.commitChanges();
            }
        }
    }
    /**
    * Caricamento della cartella selezionata nella Session Storage
    * @param currentFascicleId
    */
    private setFascicleFolder(currentFascicleId) {
        let currentSelectedNode: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.get_selectedNode();
        if (!currentSelectedNode || !currentSelectedNode.get_value()) {
            sessionStorage.removeItem(`${this.pageId}_${currentFascicleId}`);
            return;
        }

        let fascicleFolder: FascicleSummaryFolderViewModel = <FascicleSummaryFolderViewModel>{};
        fascicleFolder.UniqueId = currentSelectedNode.get_value();
        fascicleFolder.Name = currentSelectedNode.get_text();

        if (currentSelectedNode.get_attributes().getAttribute("idCategory")) {
            fascicleFolder.idCategory = currentSelectedNode.get_attributes().getAttribute("idCategory");
        }
        if (currentSelectedNode.get_attributes().getAttribute(this.TYPOLOGY_ATTRIBUTE)) {
            fascicleFolder.Typology = currentSelectedNode.get_attributes().getAttribute(this.TYPOLOGY_ATTRIBUTE);
        }

        if (currentSelectedNode.get_attributes().getAttribute("idParent")) {
            fascicleFolder.idParent = currentSelectedNode.get_attributes().getAttribute("idParent");
        }
        if (currentSelectedNode.get_attributes().getAttribute("idFascicle")) {
            fascicleFolder.idFascicle = currentSelectedNode.get_attributes().getAttribute("idFascicle");
        }
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_FOLDER_ID, JSON.stringify(fascicleFolder));
        sessionStorage[`${this.pageId}_${currentFascicleId}`] = JSON.stringify(fascicleFolder);
    }

    getSelectedFascicleFolder(currentFascicleId): FascicleSummaryFolderViewModel {
        if (sessionStorage.getItem(this.SESSION_FascicleHierarchy) == null ||
            sessionStorage.getItem(this.SESSION_FascicleHierarchy) == "[]") {
            let selectedFolder: string = sessionStorage.getItem(`${this.pageId}_${currentFascicleId}`);
            if (!selectedFolder) {
                return undefined;
            }
            return JSON.parse(selectedFolder) as FascicleSummaryFolderViewModel;

        } else {
            let selectedFolderFromSession: FascicleSummaryFolderViewModel[] = JSON.parse(sessionStorage.getItem(this.SESSION_FascicleHierarchy));
            return selectedFolderFromSession[0];
        }
    }

    /**
    * Apre una nuova nuova RadWindow
    * @param url
    * @param name
    * @param width
    * @param height
    */
    openWindow(url, name, width, height, oncloseCallback = null): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.managerWindowsId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        if (oncloseCallback) {
            wnd.add_close(oncloseCallback);
        }
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }

    closeDocumentWnd = (sender, args) => {
        sender.remove_close(this.closeDocumentWnd);
        $(`#${this.pageId}`).triggerHandler(uscFascicleFolders.REFRESH_GRID_UPLOAD_DOCUMENTS);
        this.chainId = this._treeFascicleFolders.get_selectedNode().get_attributes().getAttribute(uscFascicleFolders.CHAIN_ID);
        let model: AjaxModel = <AjaxModel>{};
        model.ActionName = "Upload_document";
        if (args.get_argument() !== null) {
            var argument = args.get_argument();
            model.Value = [argument, this.chainId];
            this._ajaxManager.ajaxRequest(JSON.stringify(model));
        }
    }

    closeScannerWnd = (sender, args) => {
        sender.remove_close(this.closeScannerWnd);
        $(`#${this.pageId}`).triggerHandler(uscFascicleFolders.REFRESH_GRID_UPLOAD_DOCUMENTS);
        this.chainId = this._treeFascicleFolders.get_selectedNode().get_attributes().getAttribute(uscFascicleFolders.CHAIN_ID);
        var documents = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_COMPONENT_SCANNER);
        let model: AjaxModel = <AjaxModel>{};
        model.ActionName = uscFascicleFolders.SCAN_DOCUMENT;
        if (documents) {
            model.Value = [documents, this.chainId];
            this._ajaxManager.ajaxRequest(JSON.stringify(model));
        }
    }

    bindMiscellanea(chainId: string): void {
        if (this.chainId == Guid.empty || this.chainId == undefined) {
            let fascicleDocumentModel: FascicleDocumentModel = <FascicleDocumentModel>{};
            fascicleDocumentModel.ChainType = ChainType.Miscellanea;
            fascicleDocumentModel.IdArchiveChain = chainId;
            fascicleDocumentModel.Fascicle = <FascicleModel>{ UniqueId: this._treeFascicleFolders.get_nodes().getNode(0).get_value() };
            fascicleDocumentModel.FascicleFolder = <FascicleFolderModel>{};
            fascicleDocumentModel.FascicleFolder.UniqueId = this._treeFascicleFolders.get_selectedNode().get_value();

            this._fascicleDocumentService.insertFascicleDocument(fascicleDocumentModel,
                (data: any) => {
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.pageContentId);
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        }
    }


    /**
     * Metodo che nasconde il loading 
     */
    hideLoadingPanel = () => {
        this._loadingPanel.hide(this.pageId);
    }

    showLoadingPanel = () => {
        this._loadingPanel.show(this.pageId);
    }

    setVisibilityButtonsByStatus(triggerHandler: boolean = true) {
        let isClosed: boolean = false;
        let currentSelectedNode: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.get_selectedNode();
        if (this._treeFascicleFolders.get_attributes().getAttribute("isClosed")) {
            isClosed = (this._treeFascicleFolders.get_attributes().getAttribute("isClosed").toLowerCase() === "true");
        }

        if (isClosed || currentSelectedNode == this._treeFascicleFolders.get_nodes().getNode(0)) {
            this.hiddenInputs();
            if (triggerHandler) {
                $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.ROOT_NODE_CLICK);
            }
            return;
        }
        let attributeStatus: string = currentSelectedNode.get_attributes().getAttribute(this.TYPOLOGY_ATTRIBUTE);
        this.typology = <FascicleFolderTypology>FascicleFolderTypology[attributeStatus];

        let hasChildren: boolean = currentSelectedNode.get_attributes().getAttribute(this.HASCHILDREN_ATTRIBUTE);
        let hasDocuments: boolean = currentSelectedNode.get_attributes().getAttribute("hasDocuments");
        let hasCategory: string = currentSelectedNode.get_attributes().getAttribute("idCategory");
        let level: number = currentSelectedNode.get_attributes().getAttribute("fascicleFolderLevel");

        if (this.typology && isNaN(this.typology)) {
            this.typology = FascicleFolderTypology[this.typology.toString()];
        }

        let manageable: boolean = false;
        if (this._treeFascicleFolders.get_attributes().getAttribute("treeViewReadOnly")) {
            manageable = (this._treeFascicleFolders.get_attributes().getAttribute("treeViewReadOnly").toLowerCase() === "true");
        }

        switch (this.typology) {
            case FascicleFolderTypology.Fascicle: {
                this._btnCreateFolder.set_enabled(manageable);
                this._btnDeleteFolder.set_enabled(!(hasCategory || hasDocuments || hasChildren || level < 3) && manageable);
                this._btnModifyFolder.set_enabled(false);
                this._btnMoveFolder.set_enabled(false);
                this._btnUploadScanner.set_enabled(manageable);
                this._btnUploadFile.set_enabled(manageable);
                if (triggerHandler) {
                    $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.FASCICLE_TREE_NODE_CLICK);
                }
                break;
            }
            case FascicleFolderTypology.SubFascicle: {
                this._btnCreateFolder.set_enabled(manageable);
                this._btnDeleteFolder.set_enabled(!(hasCategory || hasDocuments || hasChildren || level < 3) && manageable);
                this._btnModifyFolder.set_enabled(!(hasCategory || level < 3) && manageable);
                this._btnMoveFolder.set_enabled(manageable);
                this._btnUploadScanner.set_enabled(manageable);
                this._btnUploadFile.set_enabled(manageable);
                if (triggerHandler) {
                    $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.SUBFASCICLE_TREE_NODE_CLICK);
                }
                break;
            }
        }
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    getFascicleFolderTree(): Telerik.Web.UI.RadTreeView {
        return this._treeFascicleFolders;
    }

    rebuildTreeFromSession(fascicleId: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        sessionStorage.setItem(this.SESSION_FascicleId, fascicleId);
        this.createTreeFromSession();

        for (let fascicleFolder of this.fascicleFoldersModel) {
            $(document).queue((next) => {
                this.populateTree(fascicleFolder)
                    .done(() => {
                        if (fascicleFolder.UniqueId == this.fascicleFoldersModel[this.fascicleFoldersModel.length - 1].UniqueId) {
                            let toSelectNode: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.findNodeByValue(this.fascicleFoldersModel[this.fascicleFoldersModel.length - 1].UniqueId);
                            if (toSelectNode) {
                                this.setSelectedNode(toSelectNode, false);
                            } else {
                                this.selectFascicleNode(false);
                            }
                        }
                        next();
                    })
                    .fail((exception: ExceptionDTO) => {
                        $(document).clearQueue();
                        promise.reject(exception);
                        return;
                    });
            });
        }

        $(document).queue((next) => {
            this._loadingPanel.hide(this.pageId);
            promise.resolve();
            next();
        });
        return promise.promise();
    }

    private populateTree(fascicleFolder: FascicleSummaryFolderViewModel): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._fascicleFolderService.getChildren(fascicleFolder.UniqueId,
            (data: any) => {
                if (!data || data.length == 0) {
                    promise.resolve();
                    return;
                }
                let fascicleFolders: FascicleSummaryFolderViewModel[] = data as FascicleSummaryFolderViewModel[];
                let parentNode: Telerik.Web.UI.RadTreeNode = this._treeFascicleFolders.findNodeByValue(fascicleFolder.UniqueId);
                parentNode.get_nodes().clear();
                this.buildFascicleFolder(fascicleFolders, parentNode);
                parentNode.set_expanded(true);
                promise.resolve();
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    private buildFascicleFolder(data: FascicleSummaryFolderViewModel[], parentNode: Telerik.Web.UI.RadTreeNode) {
        this._treeFascicleFolders.trackChanges();
        for (let i = 0; i < data.length; i++) {
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode;
            node.set_text(data[i].Name);
            node.set_value(data[i].UniqueId);
            node.set_imageUrl(this.folder_close_path);
            parentNode.get_nodes().add(node);

            this.setNodeAttribute(node, data[i]);

            if (data[i].hasChildren && this.fascicleFoldersModel.some(f => f.Name == data[i].Name)) {
                node.set_expanded(true);
                if (node.get_nodes().getNode(0).get_text() == "") {
                    node.get_nodes().remove(node.get_nodes().getNode(0));
                }
            } else if (!data[i].hasChildren) {
                node.set_expanded(false);
            } else {
                node.set_expanded(false);
            }
        }
        this._treeFascicleFolders.commitChanges();
    }
}
export = uscFascicleFolders;