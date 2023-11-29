/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import DossierService = require('App/Services/Dossiers/DossierService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import DossierBase = require('Dossiers/DossierBase');
import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import RadTreeNodeViewModel = require('App/ViewModels/Telerik/RadTreeNodeViewModel');
import DossierFolderStatus = require('App/Models/Dossiers/DossierFolderStatus');
import DossierFolderService = require('App/Services/Dossiers/DossierFolderService');
import DossierFolderLocalService = require('App/Services/Dossiers/DossierFolderLocalService');
import IDossierFolderService = require('App/Services/Dossiers/IDossierFolderService');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierFolderSummaryModelMapper = require('App/Mappers/Dossiers/DossierFolderSummaryModelMapper');
import AjaxModel = require('App/Models/AjaxModel');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import UpdateActionType = require("App/Models/UpdateActionType");
import RoleModel = require('App/Models/Commons/RoleModel');
import RoleService = require('App/Services/Commons/RoleService');
import ContainerPropertyService = require('App/Services/Commons/ContainerPropertyService');
import BuildActionModel = require('App/Models/Commons/BuildActionModel');
import BuildActionType = require('App/Models/Commons/BuildActionType');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleService = require('App/Services/Fascicles/FascicleService');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import IRoleService = require('App/Services/Commons/IRoleService');
import RoleLocalService = require('App/Services/Commons/RoleLocalService');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import FascicleRoleService = require('App/Services/Fascicles/FascicleRoleService');
import InsertActionType = require('App/Models/InsertActionType');
import DossierModel = require('App/Models/Dossiers/DossierModel');

class uscDossierFolders {

    pageId: string;
    ajaxManagerId: string;
    treeDossierFoldersId: string;
    pageContentId: string;
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    statusToolBarId: string;
    currentDossierId: string;
    folderToolBarId: string;
    managerCreateFolderId: string;
    managerFascicleLinkId: string;
    managerModifyFolderId: string;
    managerCreateFascicleFolderId: string;
    status: DossierFolderStatus;
    fascicleId: string;
    managerWindowsId: string;
    managerId: string;
    persistanceDisabled: boolean;
    hideFascicleAssociateButton: boolean;
    hideStatusToolbar: boolean;
    managerModifyFascicle: string;
    processEnabled: boolean;
    isWindowPopupEnable: boolean;
    btnSelectDossierFolderId: string;
    fascicleModifyButtonEnable: boolean;

    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _serviceConfigurations: ServiceConfiguration[];
    private _service: DossierService;
    private _lblRegistrationUser: JQuery;
    private _treeDossierFolders: Telerik.Web.UI.RadTreeView;
    private _dossierFolders: Array<DossierSummaryFolderViewModel> = [];
    private _uscNotification: UscErrorNotification;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _statusToolBar: Telerik.Web.UI.RadToolBar;
    private _folderToolBar: Telerik.Web.UI.RadToolBar;
    private _managerCreateFolder: Telerik.Web.UI.RadWindowManager;
    private _managerAddFascLink: Telerik.Web.UI.RadWindowManager;
    private _managerModifyFolder: Telerik.Web.UI.RadWindowManager;
    private _managerCreateFascicleFolder: Telerik.Web.UI.RadWindowManager;
    private _managerModifyFascicle: Telerik.Web.UI.RadWindowManager;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _checkedToolBarButtons: number;
    private _dossierFolderService: IDossierFolderService;
    private _currentSelectedNode: Telerik.Web.UI.RadTreeNode;
    private _btnCreateFolder: Telerik.Web.UI.RadToolBarItem;
    private _btnDeleteFolder: Telerik.Web.UI.RadToolBarItem;
    private _btnRemoveFascicle: Telerik.Web.UI.RadToolBarItem;
    private _btnModifyFascicle: Telerik.Web.UI.RadToolBarItem;
    private _btnAddFascicle: Telerik.Web.UI.RadToolBarItem;
    private _btnModifyFolder: Telerik.Web.UI.RadToolBarItem;
    private _btnCreateFascicle: Telerik.Web.UI.RadToolBarItem;
    private _btnCreateFascicleFromTemplate: Telerik.Web.UI.RadToolBarItem;
    private _roleService: IRoleService;
    private _dossierFolderRoles: DossierFolderRoleModel[];
    private _containerPropertyService: ContainerPropertyService;
    private _fascicleService: FascicleService;
    private _fascicleRoleService: FascicleRoleService;
    private _templateDossierFolder: DossierSummaryFolderViewModel;


    public static ON_END_LOAD_EVENT = "onEndLoad";
    public static LOADED_EVENT: string = "onLoaded";
    public static FASCICLE_TREE_NODE_CLICK: string = "onFascicleTreeNodeClick";
    public static ROOT_NODE_CLICK: string = "onRootTreeNodeClick";
    public static CREATE_FOLDER: string = "createFolder";
    public static CREATE_FASCICLE: string = "createFascicle";
    public static DELETE_FOLDER: string = "deleteFolder";
    public static REMOVE_FASCICLE: string = "removeFascicle";
    public static ADD_FASCICLE: string = "addFascicle";
    public static MODIFY_FOLDER: string = "modifyFolder";
    public static MODIFY_FASCICLE: string = "modifyFascicle";
    public static REFRESH_DOSSIER_FOLDERS: string = "refreshDossierFolders";
    public static CREATE_FASCICLE_FROM_TEMPLATE: string = "createFascicleFromTemplate";
    public static defaultFilterStatus: number = 0;

    /**
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
            let rootNode: Telerik.Web.UI.RadTreeNode = this._treeDossierFolders.get_nodes().getNode(0);
            switch (item.get_value()) {
                case uscDossierFolders.CREATE_FOLDER: {
                    let selectedNodeId: string = this._currentSelectedNode.get_value();
                    if (selectedNodeId != "00000000-0000-0000-0000-000000000000") {
                        this.setDossierFolder(rootNode.get_value());
                    }

                    let url: string = '../Dossiers/DossierFolderInserimento.aspx?Type=Dossier&idDossier='.concat(rootNode.get_value(), '&PersistanceDisabled=', this.persistanceDisabled.toString());
                    this.openWindow(url, "managerCreateFolder", 750, 600);
                    break;
                }
                case uscDossierFolders.CREATE_FASCICLE: {

                    let selectedNodeId: string = this._currentSelectedNode.get_value();
                    if (selectedNodeId != "00000000-0000-0000-0000-000000000000") {
                        this.setDossierFolder(rootNode.get_value());
                    }

                    let url: string = '../Dossiers/DossierFascicleFolderInserimento.aspx?Type=Fasc&idDossier='.concat(rootNode.get_value(), '&PersistanceDisabled=', this.persistanceDisabled.toString());

                    this.openWindow(url, "managerCreateFascicleFolder", 750, 650);
                    break;
                }
                case uscDossierFolders.DELETE_FOLDER: {
                    if (this._currentSelectedNode.get_attributes() && !this._currentSelectedNode.get_attributes().getAttribute("idFascicle")) {

                        this.removeDossierFolder();
                    }
                    else {
                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                        if (!jQuery.isEmptyObject(uscNotification)) {
                            uscNotification.showWarningMessage("Impossibile eliminare una cartella con associato un fascicolo.");
                        }
                    }
                    break;
                }
                case uscDossierFolders.REMOVE_FASCICLE: {
                    if (this._currentSelectedNode.get_attributes() && this._currentSelectedNode.get_attributes().getAttribute("idFascicle")) {
                        this.removeFascicleFromfolder();
                    }
                    else {
                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                        if (!jQuery.isEmptyObject(uscNotification)) {
                            uscNotification.showWarningMessage("La cartella selezionata non ha alcun fascicolo associato.");
                        }
                    }
                    break;
                }
                case uscDossierFolders.ADD_FASCICLE: {
                    if (this._currentSelectedNode.get_attributes() && !this._currentSelectedNode.get_attributes().getAttribute("idFascicle")) {
                        this.setDossierFolder(rootNode.get_value());
                        let url: string = `../Fasc/FascRicerca.aspx?Type=Fasc&Action=SearchFascicles&idDossier=${rootNode.get_value()}`;
                        this.openWindow(url, "managerFascicleLink", 750, 600, this.closeSearchFasciclesWindow);

                    }
                    else {
                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                        if (!jQuery.isEmptyObject(uscNotification)) {
                            uscNotification.showWarningMessage("Impossibile associare un fascicolo ad una cartella che già contiene un fascicolo");
                        }
                    }
                    break;
                }
                case uscDossierFolders.MODIFY_FOLDER: {
                    if (this._currentSelectedNode.get_attributes()) {

                        this.setDossierFolder(rootNode.get_value());

                        let url: string = '../Dossiers/DossierFolderModifica.aspx?Type=Dossier&idDossier='.concat(rootNode.get_value(), '&PersistanceDisabled=', this.persistanceDisabled.toString());
                        this.openWindow(url, "managerModifyFolder", 750, 600);

                    }
                    break;
                }
                case uscDossierFolders.MODIFY_FASCICLE: {
                    let selectedNodeId: string = this._currentSelectedNode.get_value();
                    if (selectedNodeId != "00000000-0000-0000-0000-000000000000") {
                        this.setDossierFolder(rootNode.get_value());
                    }
                    let selectedNodeIdFascicle: string = this._currentSelectedNode.get_attributes().getAttribute("idFascicle");
                    let url: string = `../Dossiers/DossierFascicleFolderInserimento.aspx?Type=Fasc&ActionType=Update&idDossier=${rootNode.get_value()}&idFascicle=${selectedNodeIdFascicle}&PersistanceDisabled=${this.persistanceDisabled.toString()}`;

                    this.openWindow(url, "managerModifyFascicle", 750, 650);
                    break;
                }
                case uscDossierFolders.REFRESH_DOSSIER_FOLDERS: {
                    this._treeDossierFolders.get_nodes().getNode(0).collapse();
                    this._treeDossierFolders.get_nodes().getNode(0).select();
                    $(`#${this.pageId}`).triggerHandler(uscDossierFolders.ROOT_NODE_CLICK);
                }
                case uscDossierFolders.CREATE_FASCICLE_FROM_TEMPLATE: {
                    this.createAndLinkFascicle(this._templateDossierFolder.UniqueId, this._treeDossierFolders.get_selectedNode().get_parent().get_value());
                }
            }
        }
    }

    closeSearchFasciclesWindow = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            let fascicleId: string = args.get_argument();
            this.updateDossierFolder(fascicleId, this._treeDossierFolders.get_nodes().getNode(0).get_value());
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
        sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        this._currentSelectedNode = node;

        if (this.isWindowPopupEnable) {
            this._setToolbarButtonsEnabled(false);
        }
        else {
            this._btnCreateFolder.set_enabled(true);
            this._btnRemoveFascicle.set_enabled(false);
            this._btnModifyFascicle.set_visible(false);
            this._btnDeleteFolder.set_enabled(false);
            this._btnAddFascicle.set_enabled(true);
            this._btnModifyFolder.set_enabled(true);
            this._btnCreateFascicle.set_enabled(true);
        }
        this._dossierFolderService.getChildren(node.get_value(), uscDossierFolders.defaultFilterStatus != 0 ? uscDossierFolders.defaultFilterStatus : this._checkedToolBarButtons,
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

    /**
    * Evento scatenato al click dei filtri
    */
    statusToolBar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        this._loadingPanel.show(this.pageId);

        this._checkedToolBarButtons = 0;
        for (let item of this._statusToolBar.get_items().toArray()) {
            if (((<any>item).get_checked())) {
                this._checkedToolBarButtons = this._checkedToolBarButtons + <DossierFolderStatus>DossierFolderStatus[item.get_value()];
            }
        }

        //Se l'utente seleziona dei filtri, carico sempre anche le cartelle che hanno delle sottocartelle figlie
        if (this._checkedToolBarButtons != 0) {
            this._checkedToolBarButtons += DossierFolderStatus.Folder;
        }

        let rootNode: Telerik.Web.UI.RadTreeNode = this._treeDossierFolders.get_nodes().getNode(0);
        this.loadFolders(rootNode.get_value(), this._checkedToolBarButtons);
    }

    /**
    *---------------------------- Methods ---------------------------
    */
    /**
   * Inizializzazione
   */
    initialize() {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._treeDossierFolders = <Telerik.Web.UI.RadTreeView>$find(this.treeDossierFoldersId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        //this._statusToolBar = <Telerik.Web.UI.RadToolBar>$find(this.statusToolBarId);
        this._folderToolBar = <Telerik.Web.UI.RadToolBar>$find(this.folderToolBarId);
        this._folderToolBar.add_buttonClicked(this.folderToolBar_ButtonClicked);
        //this._statusToolBar.add_buttonClicked(this.statusToolBar_ButtonClicked);
        this._managerCreateFolder = <Telerik.Web.UI.RadWindowManager>$find(this.managerCreateFolderId);
        this._managerAddFascLink = <Telerik.Web.UI.RadWindowManager>$find(this.managerFascicleLinkId);
        this._managerModifyFolder = <Telerik.Web.UI.RadWindowManager>$find(this.managerModifyFolderId);
        this._managerCreateFascicleFolder = <Telerik.Web.UI.RadWindowManager>$find(this.managerCreateFascicleFolderId);
        this._managerModifyFascicle = <Telerik.Web.UI.RadWindowManager>$find(this.managerModifyFascicle);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);
        this._loadingPanel.show(this.pageId);
        this._btnCreateFolder = this._folderToolBar.findItemByValue(uscDossierFolders.CREATE_FOLDER);
        this._btnDeleteFolder = this._folderToolBar.findItemByValue(uscDossierFolders.DELETE_FOLDER);
        this._btnRemoveFascicle = this._folderToolBar.findItemByValue(uscDossierFolders.REMOVE_FASCICLE);
        this._btnModifyFascicle = this._folderToolBar.findItemByValue(uscDossierFolders.MODIFY_FASCICLE);
        this._btnAddFascicle = this._folderToolBar.findItemByValue(uscDossierFolders.ADD_FASCICLE);
        this._btnModifyFolder = this._folderToolBar.findItemByValue(uscDossierFolders.MODIFY_FOLDER);
        this._btnCreateFascicle = this._folderToolBar.findItemByValue(uscDossierFolders.CREATE_FASCICLE);
        this._btnCreateFascicleFromTemplate = this._folderToolBar.findItemByValue(uscDossierFolders.CREATE_FASCICLE_FROM_TEMPLATE);
        this._checkedToolBarButtons = 0;
        this._managerCreateFolder.add_close(this.closeFolderInsertWindow);
        this._managerCreateFascicleFolder.add_close(this.closeFolderInsertWindow);
        this._managerModifyFolder.add_close(this.closeModifyWindow);
        this._managerModifyFascicle.add_close(this.closeModifyWindow);

        if (this.hideFascicleAssociateButton) {
            this._btnAddFascicle.set_visible(false);
        }

        if (this.hideStatusToolbar) {
            $("#".concat(this.statusToolBarId)).hide();
        }

        if (this.persistanceDisabled) {
            this._dossierFolderService = new DossierFolderLocalService();
        } else {
            let dossierFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DossierFolder");
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
        }

        let containerPropertyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ContainerProperty");
        this._containerPropertyService = new ContainerPropertyService(containerPropertyConfiguration);

        let fascicleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
        this._fascicleService = new FascicleService(fascicleConfiguration);

        let fascicleRoleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleRole");
        this._fascicleRoleService = new FascicleRoleService(fascicleRoleConfiguration);

        let roleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Role");
        if (this.persistanceDisabled) {
            this._roleService = new RoleLocalService(roleConfiguration);
        } else {
            this._roleService = new RoleService(roleConfiguration);
        }

        this._currentSelectedNode = this._treeDossierFolders.get_nodes().getNode(0);

        if (this.isWindowPopupEnable) {
            let btnSelectDossierFolder: Telerik.Web.UI.RadButton = <Telerik.Web.UI.RadButton>$find(this.btnSelectDossierFolderId);
            btnSelectDossierFolder.set_enabled(false);
        }

        this._btnCreateFascicleFromTemplate.set_enabled(false);

        this.bindLoaded();
    }

    /**
    * Carico le cartelle 
    */

    loadFolders(idDossier: string, status: number): void {
        this._dossierFolderService.getChildren(idDossier, status,
            (data: any) => {
                this.loadNodes(data);
                this._loadingPanel.hide(this.pageId);
            },
            (exception: ExceptionDTO): void => {
                this._loadingPanel.hide(this.pageId);
                let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            });
    }

    removeNode(idDossierFolder: string) {
        let nodeToRemove: Telerik.Web.UI.RadTreeNode = this._treeDossierFolders.findNodeByValue(idDossierFolder);
        if (nodeToRemove) {
            let parentNode: Telerik.Web.UI.RadTreeNode = nodeToRemove.get_parent();
            if (parentNode && parentNode.get_nodes()) {
                parentNode.get_nodes().remove(nodeToRemove);
                if (parentNode != this._treeDossierFolders.get_nodes().getNode(0) && parentNode.get_nodes().get_count() == 0) {
                    parentNode.get_attributes().setAttribute("Status", "InProgress");
                    parentNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_hidden.png");
                    parentNode.set_toolTip("Da gestire");
                }
                this._treeDossierFolders.commitChanges();
            }
        }
    }


    /**
    * Imposta gli attributi di un nodo
    * @param node
    * @param dossierFolder
    */
    private setNodeAttribute(node: Telerik.Web.UI.RadTreeNode, dossierFolder: DossierSummaryFolderViewModel): Telerik.Web.UI.RadTreeNode {
        node.get_attributes().setAttribute("Status", dossierFolder.Status);
        node.set_text(dossierFolder.Name);
        node.set_value(dossierFolder.UniqueId);
        node.get_attributes().setAttribute("idParent", null);

        if (node.get_parent()) {
            let parentNode: Telerik.Web.UI.RadTreeNode = node.get_parent();
            node.get_attributes().setAttribute("idParent", parentNode.get_value());
        }
        node.get_attributes().setAttribute("idFascicle", null);
        if (dossierFolder.idFascicle) {
            node.get_attributes().setAttribute("idFascicle", dossierFolder.idFascicle);
        }

        node.get_attributes().setAttribute("idCategory", null);
        if (dossierFolder.idCategory) {
            node.get_attributes().setAttribute("idCategory", dossierFolder.idCategory);
        }

        node.get_attributes().setAttribute("idRole", null);
        if (dossierFolder.idRole) {
            node.get_attributes().setAttribute("idRole", dossierFolder.idRole);
        }

        //qui scelgo l'immagine da visualizzare per il nodo
        switch (DossierFolderStatus[dossierFolder.Status]) {
            case DossierFolderStatus.DoAction:
            case DossierFolderStatus.InProgress: {
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_hidden.png");
                node.set_toolTip("Da gestire");
                break;
            }
            case DossierFolderStatus.Fascicle: {
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_open.png");
                node.set_toolTip("Fascicolo ");
                break;
            }
            case DossierFolderStatus.FascicleClose: {
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_close.png");
                node.set_toolTip("Fascicolo chiuso")
                break;
            }
            case DossierFolderStatus.Folder: {
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_closed.png");
                node.set_expandedImageUrl("../App_Themes/DocSuite2008/imgset16/folder_open.png")
                node.set_toolTip("Cartella con sottocartelle")
                let nodeToAdd = new Telerik.Web.UI.RadTreeNode();
                node.get_nodes().add(nodeToAdd);
                node.set_expanded(false);
                break;
            }
        }

        return node;
    }

    /*
    * Imposto il valore del nodo Root
    * @param dossierName
    */
    setRootNode(dossierTitle: string, dossierId: string): void {
        let rootNode: Telerik.Web.UI.RadTreeNode = this._treeDossierFolders.get_nodes().getNode(0);
        rootNode.set_text("Dossier: ".concat(dossierTitle));
        rootNode.set_value(dossierId);
        rootNode.set_expanded(true);
        rootNode.set_selected(true);

        this._treeDossierFolders.commitChanges();

        if (this.isWindowPopupEnable) {
            this._setToolbarButtonsEnabled(false);
        }
        else {
            this._btnCreateFolder.set_enabled(true);
            this._btnRemoveFascicle.set_enabled(false);
            this._btnModifyFascicle.set_visible(false);
            this._btnDeleteFolder.set_enabled(false);
            this._btnCreateFascicle.set_enabled(true);
            this._btnAddFascicle.set_enabled(true);
            this._btnModifyFolder.set_enabled(false);
            this.currentDossierId = dossierId;
        }

    }

    setButtonVisibility(isManager: boolean) {
        if (!isManager) {
            $(this._folderToolBar.get_element()).hide();
        }
    }

    setStatusVisibility(isManager: boolean) {
        if (!isManager) {
            $("#".concat(this.statusToolBarId)).hide();
        }
    }

    /**
    * Carica i dati dello user control
    */
    loadNodes(dossierFolders: DossierSummaryFolderViewModel[], node?: Telerik.Web.UI.RadTreeNode, appendToParentNode: boolean = false): void {
        if (dossierFolders == null) return;

        let parentSelectedNode: Telerik.Web.UI.RadTreeNode

        if (node) {
            parentSelectedNode = node;
        } else {
            parentSelectedNode = this._treeDossierFolders.get_nodes().getNode(0);
        }

        this._btnCreateFascicleFromTemplate.set_enabled(false);
        if (!appendToParentNode) {
            parentSelectedNode.get_nodes().clear();
            parentSelectedNode.select();
        }

        let newNode: Telerik.Web.UI.RadTreeNode;
        $.each(dossierFolders, (index: number, dossierFolder: DossierSummaryFolderViewModel) => {
            this._dossierFolders.push(dossierFolder);
            if (this._treeDossierFolders.findNodeByValue(dossierFolder.UniqueId) != undefined) {
                return;
            }
            if (!this.isWindowPopupEnable || (this.isWindowPopupEnable && DossierFolderStatus[dossierFolder.Status] !== DossierFolderStatus.Fascicle)) {

                newNode = new Telerik.Web.UI.RadTreeNode();

                parentSelectedNode.get_nodes().add(newNode);

                this.setNodeAttribute(newNode, dossierFolder);
            }
        });
        this._treeDossierFolders.commitChanges();
        $("#".concat(this.treeDossierFoldersId)).triggerHandler(uscDossierFolders.ON_END_LOAD_EVENT);

        this._loadingPanel.hide(this.pageId);
    }


    /**
    * Scateno l'evento di "Load Completed" del controllo
    */
    private bindLoaded(): void {
        $("#".concat(this.pageId)).data(this);
        $("#".concat(this.pageId)).triggerHandler(uscDossierFolders.LOADED_EVENT);
    }

    /**
    * Metono chiamato in chiusura della radwindow di inserimento
    * @param sender
    * @param args
    */
    closeFolderInsertWindow = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument) {
            let result: AjaxModel = <AjaxModel>args.get_argument();
            let parentNode: Telerik.Web.UI.RadTreeNode = this._treeDossierFolders.get_selectedNode();
            if (result) {
                let dossierFolderModel = <DossierSummaryFolderViewModel>{};
                dossierFolderModel = JSON.parse(result.Value[0]);

                this._btnCreateFolder.set_enabled(true);
                this._btnRemoveFascicle.set_enabled(false);
                this._btnDeleteFolder.set_enabled(false);
                this._btnModifyFascicle.set_visible(false);
                this._btnCreateFascicle.set_enabled(true);
                this._btnAddFascicle.set_enabled(false);
                this._btnModifyFolder.set_enabled(false);

                if (parentNode != this._treeDossierFolders.get_nodes().getNode(0)) {
                    this._btnModifyFolder.set_enabled(true);
                    let attributeStatus: string = parentNode.get_attributes().getAttribute("Status");
                    this.status = DossierFolderStatus[attributeStatus];

                    if (this.status == DossierFolderStatus.Folder && !parentNode.get_nodes().getNode(0).get_value()) {
                        parentNode.get_nodes().remove(parentNode.get_nodes().getNode(0))
                    }

                    parentNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_closed.png");
                    parentNode.set_expandedImageUrl("../App_Themes/DocSuite2008/imgset16/folder_open.png")
                    parentNode.set_toolTip("Cartella con sottocartelle");
                    parentNode.get_attributes().setAttribute("Status", "Folder");
                    parentNode.set_selected(true);
                }

                if (parentNode.get_expanded() == false) {
                    this._dossierFolderService.getChildren(parentNode.get_value(), this._checkedToolBarButtons,
                        (data: any) => {
                            this.loadNodes(data, parentNode);
                            parentNode.set_expanded(true);
                            this._treeDossierFolders.commitChanges();
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
                    this.setNodeAttribute(nodeToAdd, dossierFolderModel);
                    parentNode.set_expanded(true);
                    this._treeDossierFolders.commitChanges();
                }
            }
        }
    }


    /**
    * Metodo chiamato in chiusura della radwindow di modifica di cartella
    * @param sender
    * @param args
    */
    closeModifyWindow = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument) {
            let result: AjaxModel = <AjaxModel>args.get_argument();
            let updatedNode: Telerik.Web.UI.RadTreeNode = this._treeDossierFolders.get_selectedNode();
            if (result && result.Value[0]) {

                let dossierFolderModel: DossierSummaryFolderViewModel = JSON.parse(result.Value[0]);
                this.setNodeAttribute(updatedNode, dossierFolderModel)
                this._currentSelectedNode = updatedNode;
                this.setVisibilityButtonsByStatus();
                this._treeDossierFolders.commitChanges();
            }
        }
    }

    /**
    * Evento scatenato al click di un nodo
    * @param sender
    * @param eventArgs
    */
    treeView_ClientNodeClicked = (sender: Telerik.Web.UI.RadTreeView, eventArgs: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        this._currentSelectedNode = eventArgs.get_node();
        sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        if (this.isWindowPopupEnable) {
            this._setToolbarButtonsEnabled(false);
            let btnSelectDossierFolder: Telerik.Web.UI.RadButton = <Telerik.Web.UI.RadButton>$find(this.btnSelectDossierFolderId);
            btnSelectDossierFolder.set_enabled(this._currentSelectedNode.get_level() !== 0);
            btnSelectDossierFolder.remove_clicked(this.btnSelectDossierFolder_onClick);
            btnSelectDossierFolder.add_clicked(this.btnSelectDossierFolder_onClick);
        }
        else {
            this.setVisibilityButtonsByStatus();
        }

        let selectedDossierFolder: DossierSummaryFolderViewModel = this._dossierFolders.filter(x => x.UniqueId === this._currentSelectedNode.get_value())[0];
        this._btnCreateFascicleFromTemplate.set_enabled(false);
        if (this._currentSelectedNode.get_level() > 1) {
            this._enableBtnCreateFascicleFromTemplate(selectedDossierFolder);
        }
    }
    /**
     * Rimuovo un fascicolo dalla cartella
     */
    removeFascicleFromfolder() {
        this._manager.radconfirm("Sei sicuro di voler rimuovere il fascicolo dalla cartella?", (arg) => {
            if (arg) {
                let dossierFolder: DossierFolderModel = <DossierFolderModel>{};
                dossierFolder.UniqueId = this._currentSelectedNode.get_value();
                dossierFolder.Name = this._currentSelectedNode.get_text();
                dossierFolder.Status = DossierFolderStatus.InProgress;

                $.when(this.getFolderRoles(dossierFolder.UniqueId)).done(() => {
                    dossierFolder.DossierFolderRoles = this._dossierFolderRoles;
                    this._dossierFolderService.updateDossierFolder(dossierFolder, UpdateActionType.RemoveFascicleFromDossierFolder,
                        (data: any) => {
                            let mapper = new DossierFolderSummaryModelMapper();
                            this.setNodeAttribute(this._currentSelectedNode, mapper.Map(data));
                            this._btnCreateFolder.set_enabled(true);
                            this._btnRemoveFascicle.set_enabled(false);
                            this._btnModifyFascicle.set_visible(false);
                            this._btnDeleteFolder.set_enabled(true);
                            this._btnCreateFascicle.set_enabled(true);
                            this._btnAddFascicle.set_enabled(true);
                            this._btnModifyFolder.set_enabled(true);
                            $("#".concat(this.pageId)).triggerHandler(uscDossierFolders.ROOT_NODE_CLICK);
                        },
                        (exception: ExceptionDTO) => {
                            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                            if (!jQuery.isEmptyObject(uscNotification)) {
                                uscNotification.showNotification(exception);
                            }
                        }
                    );
                }).fail((exception) => {
                    this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento dei settori autorizzati alla cartella.");
                });
            }
        }, 300, 160);
    }

    /*
    * Eliminazione di una cartellina
    */
    removeDossierFolder = () => {
        this._manager.radconfirm("Sei sicuro di voler eliminare la cartella?", (arg) => {
            if (arg) {
                let dossierFolder = <DossierFolderModel>{};
                dossierFolder.UniqueId = this._currentSelectedNode.get_value();
                dossierFolder.Name = this._currentSelectedNode.get_text();
                let parentNode: Telerik.Web.UI.RadTreeNode = this._currentSelectedNode.get_parent();
                if (parentNode && parentNode.get_value() != this.currentDossierId) {
                    dossierFolder.ParentInsertId = parentNode.get_value();
                }
                let idRole: number = this._currentSelectedNode.get_attributes().getAttribute("idRole");
                if (idRole) {
                    let dossierFolderRole: DossierFolderRoleModel = <DossierFolderRoleModel>{};
                    let role: RoleModel = <RoleModel>{};
                    role.EntityShortId = idRole;
                    dossierFolderRole.Role = role;
                    dossierFolder.DossierFolderRoles = [];
                    dossierFolder.DossierFolderRoles.push(dossierFolderRole);
                }
                this._dossierFolderService.deleteDossierFolder(dossierFolder,
                    (data: any) => {
                        this.removeNode(dossierFolder.UniqueId);
                        this._btnCreateFolder.set_enabled(false);
                        this._btnRemoveFascicle.set_enabled(false);
                        this._btnModifyFascicle.set_visible(false);
                        this._btnDeleteFolder.set_enabled(false);
                        this._btnCreateFascicle.set_enabled(false);
                        this._btnAddFascicle.set_enabled(false);
                        this._btnModifyFolder.set_enabled(false);
                    }, (exception: ExceptionDTO) => {
                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                        if (!jQuery.isEmptyObject(uscNotification)) {
                            uscNotification.showNotification(exception);
                        }
                    }
                );
            }

        });
    }

    /**
    * Caricamento della cartella selezionata nella Session Storage
    * @param currentDossierId
    */
    setDossierFolder = (currentDossierId) => {
        let dossierFolder: DossierSummaryFolderViewModel = <DossierSummaryFolderViewModel>{};
        dossierFolder.UniqueId = this._currentSelectedNode.get_value();
        dossierFolder.Name = this._currentSelectedNode.get_text();

        if (this._currentSelectedNode.get_attributes().getAttribute("idCategory")) {
            dossierFolder.idCategory = this._currentSelectedNode.get_attributes().getAttribute("idCategory");
        }
        if (this._currentSelectedNode.get_attributes().getAttribute("Status")) {
            dossierFolder.Status = this._currentSelectedNode.get_attributes().getAttribute("Status");
        }
        if (this._currentSelectedNode.get_attributes().getAttribute("idRole")) {
            dossierFolder.idRole = this._currentSelectedNode.get_attributes().getAttribute("idRole");
        }
        if (this._currentSelectedNode.get_attributes().getAttribute("idParent")) {
            dossierFolder.idParent = this._currentSelectedNode.get_attributes().getAttribute("idParent");
        }
        if (this._currentSelectedNode.get_attributes().getAttribute("idFascicle")) {
            dossierFolder.idFascicle = this._currentSelectedNode.get_attributes().getAttribute("idFascicle");
        }

        sessionStorage[currentDossierId] = JSON.stringify(dossierFolder);
    }

    /**
    * Apre una nuova nuova RadWindow
    * @param url
    * @param name
    * @param width
    * @param height
    */
    openWindow(url, name, width, height, closeHandler?: (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => void): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.managerWindowsId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        if (closeHandler != null) {
            wnd.remove_close(closeHandler);
            wnd.add_close(closeHandler);
        }
        wnd.center();
        return false;
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

    private updateDossierFolder(fascicleId: string, dossierId: string): any {
        this._loadingPanel.show(this.pageId);
        this._fascicleService.getFascicle(fascicleId, (data) => {
            let fascicleModel: FascicleModel = data;
            let dossierFolder = <DossierFolderModel>{};

            let dossier = <DossierModel>{};
            dossier.UniqueId = dossierId;
            dossierFolder.Status = DossierFolderStatus.InProgress;

            dossierFolder.Dossier = dossier;

            dossierFolder.ParentInsertId = this._treeDossierFolders.get_selectedNode().get_value();

            dossierFolder.Fascicle = fascicleModel;
            dossierFolder.Status = DossierFolderStatus.Fascicle;
            dossierFolder.Category = fascicleModel.Category;

            this._dossierFolderService.insertDossierFolder(dossierFolder, InsertActionType.InsertDossierFolderAssociatedToFascicle,
                (data: any) => {
                    if (data == null) return;

                    let parentNode: Telerik.Web.UI.RadTreeNode = this._treeDossierFolders.get_selectedNode();
                    if (parentNode != this._treeDossierFolders.get_nodes().getNode(0)) {
                        this._btnModifyFolder.set_enabled(true);
                        let attributeStatus: string = parentNode.get_attributes().getAttribute("Status");
                        this.status = DossierFolderStatus[attributeStatus];

                        if (this.status == DossierFolderStatus.Folder && !parentNode.get_nodes().getNode(0).get_value()) {
                            parentNode.get_nodes().remove(parentNode.get_nodes().getNode(0))
                        }

                        parentNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_closed.png");
                        parentNode.set_expandedImageUrl("../App_Themes/DocSuite2008/imgset16/folder_open.png")
                        parentNode.set_toolTip("Cartella con sottocartelle");
                        parentNode.get_attributes().setAttribute("Status", "Folder");
                        parentNode.set_selected(true);
                    }

                    if (parentNode.get_expanded() == false) {
                        this._dossierFolderService.getChildren(parentNode.get_value(), this._checkedToolBarButtons,
                            (data: any) => {
                                this.loadNodes(data, parentNode);
                                parentNode.set_expanded(true);
                                this._treeDossierFolders.commitChanges();
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

                        let mapper = new DossierFolderSummaryModelMapper();
                        let folder: DossierSummaryFolderViewModel = mapper.Map(data);

                        this.setNodeAttribute(nodeToAdd, folder);
                        parentNode.set_expanded(true);
                        this._treeDossierFolders.commitChanges();
                    }

                    this._loadingPanel.hide(this.pageId);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.pageId);
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        });
    }

    private setVisibilityButtonsByStatus = () => {
        if (this._currentSelectedNode == this._treeDossierFolders.get_nodes().getNode(0)) {
            this._btnCreateFolder.set_enabled(true);
            this._btnRemoveFascicle.set_enabled(false);
            this._btnModifyFascicle.set_visible(false);
            this._btnDeleteFolder.set_enabled(false);
            this._btnCreateFascicle.set_enabled(true);
            this._btnAddFascicle.set_enabled(true);
            this._btnModifyFolder.set_enabled(false);
            $("#".concat(this.pageId)).triggerHandler(uscDossierFolders.ROOT_NODE_CLICK);
            return;
        }
        let attributeStatus: string = this._currentSelectedNode.get_attributes().getAttribute("Status");
        this.status = <DossierFolderStatus>DossierFolderStatus[attributeStatus];

        switch (this.status) {
            case DossierFolderStatus.InProgress: {
                this._btnCreateFolder.set_enabled(true);
                this._btnRemoveFascicle.set_enabled(false);
                this._btnModifyFascicle.set_visible(false);
                this._btnDeleteFolder.set_enabled(true);
                this._btnCreateFascicle.set_enabled(true);
                this._btnAddFascicle.set_enabled(true);
                this._btnModifyFolder.set_enabled(true);
                $("#".concat(this.pageId)).triggerHandler(uscDossierFolders.ROOT_NODE_CLICK);
                break;
            }
            case DossierFolderStatus.Folder: {
                this._btnCreateFolder.set_enabled(true);
                this._btnRemoveFascicle.set_enabled(false);
                this._btnModifyFascicle.set_visible(false);
                this._btnDeleteFolder.set_enabled(false);
                this._btnCreateFascicle.set_enabled(true);
                this._btnAddFascicle.set_enabled(true);
                this._btnModifyFolder.set_enabled(true);
                $("#".concat(this.pageId)).triggerHandler(uscDossierFolders.ROOT_NODE_CLICK);
                break;
            }
            case DossierFolderStatus.Fascicle: {
                this.fascicleId = this._currentSelectedNode.get_attributes().getAttribute("idFascicle");
                if (this.fascicleId) {
                    this._btnCreateFolder.set_enabled(false);
                    this._btnRemoveFascicle.set_enabled(true);
                    this._btnModifyFascicle.set_visible(!this.isWindowPopupEnable && this.fascicleModifyButtonEnable);
                    this._btnDeleteFolder.set_enabled(false);
                    this._btnCreateFascicle.set_enabled(false);
                    this._btnAddFascicle.set_enabled(false);
                    this._btnModifyFolder.set_enabled(false);
                    $("#".concat(this.pageId)).triggerHandler(uscDossierFolders.FASCICLE_TREE_NODE_CLICK, this.fascicleId);
                }
                break;
            }
            case DossierFolderStatus.FascicleClose: {
                this.fascicleId = this._currentSelectedNode.get_attributes().getAttribute("idFascicle");
                if (this.fascicleId) {
                    this._btnCreateFolder.set_enabled(false);
                    this._btnRemoveFascicle.set_enabled(false);
                    this._btnModifyFascicle.set_visible(false);
                    this._btnDeleteFolder.set_enabled(false);
                    this._btnCreateFascicle.set_enabled(false);
                    this._btnAddFascicle.set_enabled(false);
                    this._btnModifyFolder.set_enabled(false);
                    $("#".concat(this.pageId)).triggerHandler(uscDossierFolders.FASCICLE_TREE_NODE_CLICK, this.fascicleId);
                }
                break;
            }
            case DossierFolderStatus.DoAction: {
                this._btnCreateFolder.set_enabled(false);
                this._btnRemoveFascicle.set_enabled(false);
                this._btnModifyFascicle.set_visible(false);
                this._btnDeleteFolder.set_enabled(false);
                this._btnCreateFascicle.set_enabled(false);
                this._btnAddFascicle.set_enabled(false);
                this._btnModifyFolder.set_enabled(false);
                const message: string = "E' stata selezionata una cartellina con definizione di fascicolo associata. Procedere con la creazione del fascicolo?";
                this._manager.radconfirm(message,
                    (arg) => {
                        if (arg) {
                            this.createAndLinkFascicle(this._currentSelectedNode.get_value());
                        }
                    }, 300, 160);
                break;
            }
        }
    }

    private createAndLinkFascicle(dossierFolderId: string, dossierFolderParentId: string = null): void {
        this._loadingPanel.show(this.pageId);
        this._dossierFolderService.getFullDossierFolder(dossierFolderId,
            (dossierFolder: any) => {
                let folder: DossierFolderModel = dossierFolder as DossierFolderModel;
                if (!folder.JsonMetadata) {
                    this._loadingPanel.hide(this.pageId);
                    this.showNotificationMessage(this.uscNotificationId, "Nessun modello di fascicolo associato alla cartellina selezionata.");
                    return;
                }

                let actionModels: BuildActionModel[] = JSON.parse(folder.JsonMetadata) as BuildActionModel[];
                let filteredItems: BuildActionModel[] = actionModels.filter((item, index) => { return item.BuildType == BuildActionType.Build });

                if (filteredItems.length == 0) {
                    this._loadingPanel.hide(this.pageId);
                    this.showNotificationMessage(this.uscNotificationId, "Nessun modello di fascicolo associato alla cartellina selezionata.");
                    return;
                }

                let insertFascicleAction: BuildActionModel = filteredItems[0];
                let fascicleToInsert: FascicleModel = JSON.parse(insertFascicleAction.Model) as FascicleModel;
                if (fascicleToInsert.MetadataRepository && folder.Dossier.MetadataRepository && fascicleToInsert.MetadataRepository.UniqueId == folder.Dossier.MetadataRepository.UniqueId) {
                    fascicleToInsert.MetadataDesigner = folder.Dossier.MetadataDesigner;
                    fascicleToInsert.MetadataValues = folder.Dossier.MetadataValues;
                }

                this._fascicleService.insertFascicle(fascicleToInsert, null,
                    (fascicle: any) => {
                        folder.Fascicle = fascicle;
                        folder.Status = DossierFolderStatus.Fascicle;
                        let category = new CategoryModel();
                        category.EntityShortId = fascicle.Category.EntityShortId;
                        folder.Category = category;
                        fascicleToInsert.UniqueId = fascicle.UniqueId;
                        $.when(this.setFascicleRoles(fascicleToInsert, folder.DossierFolderRoles))
                            .done(() => {
                                if (dossierFolderParentId) {
                                    folder.ParentInsertId = dossierFolderParentId;
                                    folder.UniqueId = null;
                                    this._dossierFolderService.insertDossierFolder(folder, null,
                                        (data: any) => {
                                            let mapper: DossierFolderSummaryModelMapper = new DossierFolderSummaryModelMapper();
                                            let insertedDossierFolder: DossierSummaryFolderViewModel = mapper.Map(data);
                                            insertedDossierFolder.idFascicle = fascicleToInsert.UniqueId;
                                            let parentNode: Telerik.Web.UI.RadTreeNode = this._treeDossierFolders.findNodeByValue(dossierFolderParentId);
                                            this.loadNodes([insertedDossierFolder], parentNode, true);
                                        },
                                        (exception: ExceptionDTO) => {
                                            this._loadingPanel.hide(this.pageId);
                                            this.showNotificationException(this.uscNotificationId, exception);
                                        });
                                }
                                else {
                                    this._dossierFolderService.updateDossierFolder(folder, null,
                                        (data: any) => {
                                            let node: Telerik.Web.UI.RadTreeNode = this._treeDossierFolders.findNodeByValue(dossierFolderId);
                                            data.Fascicle = fascicle;
                                            let mapper: DossierFolderSummaryModelMapper = new DossierFolderSummaryModelMapper();
                                            let updatedDossierFolder: DossierSummaryFolderViewModel = mapper.Map(data);
                                            this._dossierFolders = this._dossierFolders.filter(x => x.UniqueId !== updatedDossierFolder.UniqueId);
                                            this._dossierFolders.push(updatedDossierFolder);
                                            this._enableBtnCreateFascicleFromTemplate(updatedDossierFolder);
                                            this.setNodeAttribute(node, updatedDossierFolder);
                                            $("#".concat(this.pageId)).triggerHandler(uscDossierFolders.FASCICLE_TREE_NODE_CLICK, fascicle.UniqueId);
                                            this._loadingPanel.hide(this.pageId);
                                        },
                                        (exception: ExceptionDTO) => {
                                            this._loadingPanel.hide(this.pageId);
                                            this.showNotificationException(this.uscNotificationId, exception);
                                        });
                                }
                            })
                            .fail(() => {
                                this._loadingPanel.hide(this.pageId);
                                this.showNotificationMessage(this.uscNotificationId, "Errore in autorizzazione del fascicolo");
                            });
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.pageId);
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageId);
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private setFascicleRoles(fascicle: FascicleModel, roles: DossierFolderRoleModel[]): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (!roles || roles.length == 0) {
            promise.resolve();
            return;
        }

        let role: DossierFolderRoleModel = roles.shift();
        let existFascicleAccountedRole: boolean = fascicle.FascicleRoles &&
            fascicle.FascicleRoles.filter(x => x.AuthorizationRoleType == AuthorizationRoleType.Accounted && x.Role.EntityShortId == role.Role.EntityShortId).length > 0;

        if (existFascicleAccountedRole) {
            if (roles.length > 0) {
                $.when(this.setFascicleRoles(fascicle, roles))
                    .done(() => promise.resolve())
                    .fail(() => promise.reject());
            } else {
                promise.resolve();
                return;
            }
        }

        let fascicleRole: FascicleRoleModel = new FascicleRoleModel();
        fascicleRole.Fascicle = fascicle;
        fascicleRole.Role = role.Role;
        fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;

        this._fascicleRoleService.insertFascicleRole(fascicleRole,
            (data: any) => {
                if (roles.length == 0) {
                    promise.resolve();
                    return;
                }

                $.when(this.setFascicleRoles(fascicle, roles))
                    .done(() => promise.resolve())
                    .fail(() => promise.reject());
            },
            (exception: ExceptionDTO) => {
                promise.reject();
            });

        return promise.promise();
    }

    private getFolderRoles = (uniqueId: string) => {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._roleService.getDossierFolderRole(uniqueId,
                (data: any) => {
                    if (data == null) {
                        promise.resolve();
                        return;
                    }
                    let dossierFolderRoles: DossierFolderRoleModel[] = new Array<DossierFolderRoleModel>();
                    for (let role of data) {
                        let dossierFolderRoleModel: DossierFolderRoleModel = <DossierFolderRoleModel>{};
                        let r: RoleModel = <RoleModel>{};
                        r.EntityShortId = role.EntityShortId;
                        r.IdRole = role.EntityShortId;
                        r.Name = role.Name;
                        r.IdTenantAOO = role.IdTenantAOO;

                        dossierFolderRoleModel.UniqueId = role.DossierFolderRoles[0].UniqueId;
                        dossierFolderRoleModel.Role = r;
                        dossierFolderRoles.push(dossierFolderRoleModel);
                    }
                    this._dossierFolderRoles = dossierFolderRoles;
                    promise.resolve();
                },
                (exception: ExceptionDTO) => {
                    promise.reject(exception);
                }
            );
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    loadFascicleDossierFolders = (dossierId: string, fascicleId: string, dossierFoldersModel: DossierSummaryFolderViewModel[]): void => {
        this._dossierFolderService.getAllParentsOfFascicle(dossierId, fascicleId, (data: any) => {
            if (!data) return;

            let dossierFolders: DossierSummaryFolderViewModel[] = data.reverse();
            this.setRootNode(dossierFolders[0].Name, dossierFolders[0].UniqueId);
            let parentNode: Telerik.Web.UI.RadTreeNode = this._treeDossierFolders.get_nodes().getNode(0);
            for (let dossierFolder of dossierFolders.slice(1)) {
                parentNode = this._createFascicleDossierFolderParent(parentNode, dossierFolder);
            }
            parentNode.select();
            this._currentSelectedNode = parentNode;
            this.setVisibilityButtonsByStatus();
            this._loadingPanel.hide(this.pageId);
        }, (exception: ExceptionDTO) => {
            this._loadingPanel.hide(this.pageId);
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }

    private _createFascicleDossierFolderParent(parentNode: Telerik.Web.UI.RadTreeNode, dossierFolder: DossierSummaryFolderViewModel): Telerik.Web.UI.RadTreeNode {
        let childNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        parentNode.get_nodes().clear();
        parentNode.get_nodes().add(childNode);
        this.setNodeAttribute(childNode, dossierFolder);
        parentNode.expand();
        return childNode;
    }

    private _setToolbarButtonsEnabled(isEnabled: boolean): void {
        this._btnCreateFolder.set_enabled(isEnabled);
        this._btnRemoveFascicle.set_enabled(isEnabled);
        this._btnDeleteFolder.set_enabled(isEnabled);
        this._btnCreateFascicle.set_enabled(isEnabled);
        this._btnAddFascicle.set_enabled(isEnabled);
        this._btnModifyFolder.set_enabled(isEnabled);
        this._btnModifyFascicle.set_visible(isEnabled);
    }

    setToolbarButtonsVisibility(isVisible: boolean): void {
        this._btnCreateFolder.set_visible(isVisible);
        this._btnRemoveFascicle.set_visible(isVisible);
        this._btnDeleteFolder.set_visible(isVisible);
        this._btnCreateFascicle.set_visible(isVisible);
        this._btnAddFascicle.set_visible(isVisible);
        this._btnModifyFolder.set_visible(isVisible);
        this._btnModifyFascicle.set_visible(false);
        this._btnCreateFascicleFromTemplate.set_visible(isVisible);
    }

    btnSelectDossierFolder_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let idDossier: string = this._treeDossierFolders.get_nodes().getNode(0).get_value();
        let idDossierFolder: string = this._currentSelectedNode.get_value();
        this.closeGridWindow(idDossier, idDossierFolder);
    }

    closeGridWindow(idDossier: string, idDossierFolder: string) {
        let wnd: Telerik.Web.UI.RadWindow = this._getRadWindow();
        wnd.close(`${idDossier}|${idDossierFolder}`);
    }

    private _getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    private _enableBtnCreateFascicleFromTemplate(dossierFolder: DossierSummaryFolderViewModel) {
        if (dossierFolder.Status === DossierFolderStatus[DossierFolderStatus.Fascicle] && dossierFolder.JsonMetadata) {
            let actionModels: BuildActionModel[] = JSON.parse(dossierFolder.JsonMetadata) as BuildActionModel[];
            if (actionModels.filter(x => x.BuildType === BuildActionType.Build).length > 0) {
                this._btnCreateFascicleFromTemplate.set_enabled(true);
                this._templateDossierFolder = dossierFolder;
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

    public getSelectedDossierFolderNode = (): Telerik.Web.UI.RadTreeNode => {
        return this._currentSelectedNode;
    }
}
export = uscDossierFolders;