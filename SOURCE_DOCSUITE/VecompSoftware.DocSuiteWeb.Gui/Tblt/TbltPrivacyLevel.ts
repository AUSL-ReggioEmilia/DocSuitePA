/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import PrivacyLevelService = require('App/Services/Commons/PrivacyLevelService');
import PrivacyLevelModel = require('App/Models/Commons/PrivacyLevelModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import AjaxModel = require('App/Models/AjaxModel');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import EnumHelper = require('App/Helpers/EnumHelper');

class TbltPrivacyLevel {

    pnlDetailsId: string;
    pnlInformationsId: string;
    toolBarSearchId: string;
    rtvLevelsId: string;
    uscNotificationId: string;
    btnAddId: string;
    btnEditId: string;
    btnDeleteId: string;
    windowAddUDSTypologyId: string;
    ajaxLoadingPanelId: string;
    paneSelectionId: string;
    txtDescriptionId: string;
    lblActiveFromId: string;
    managerId: string;
    lblIsActiveId: string;
    ajaxManagerId: string;
    txtLevelId: string;
    colorBoxId: string;
    privacyLabel: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _privacyLevelService: PrivacyLevelService;
    private _toolBarSearch: Telerik.Web.UI.RadToolBar;
    private _rtvLevels: Telerik.Web.UI.RadTreeView;
    private _btnAdd: Telerik.Web.UI.RadButton;
    private _btnEdit: Telerik.Web.UI.RadButton;
    private _btnDelete: Telerik.Web.UI.RadButton;
    private _windowAddPrivacyLevel: Telerik.Web.UI.RadWindow;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _currentSelectedNode: Telerik.Web.UI.RadTreeNode;
    private _currentPrivacyLevel: PrivacyLevelModel;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;

    /**
   * Costruttore
   * @param serviceConfigurations
   */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "PrivacyLevel");
        this._privacyLevelService = new PrivacyLevelService(serviceConfiguration);
    }

    /**
    * Inizializzazione classe
    */
    initialize = () => {
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);  
        this._rtvLevels = <Telerik.Web.UI.RadTreeView>$find(this.rtvLevelsId);
        this._toolBarSearch = <Telerik.Web.UI.RadToolBar>$find(this.toolBarSearchId);
        this._btnAdd = <Telerik.Web.UI.RadButton>$find(this.btnAddId);
        this._btnEdit = <Telerik.Web.UI.RadButton>$find(this.btnEditId);
        //this._btnDelete = <Telerik.Web.UI.RadButton>$find(this.btnDeleteId);
        this._windowAddPrivacyLevel = <Telerik.Web.UI.RadWindow>$find(this.windowAddUDSTypologyId);
        this._windowAddPrivacyLevel.add_close(this.closeInsertWindow);
        this._toolBarSearch.add_buttonClicked(this.toolBarSearch_ButtonClicked);
        this._btnAdd.add_clicked(this.btnAdd_OnClick);
        this._btnEdit.add_clicked(this.btnEdit_OnClick);
        //this._btnDelete.add_clicked(this.btnDelete_OnClick);
        this._btnEdit.set_enabled(false);
        //this._btnDelete.set_enabled(false);
        this._rtvLevels.add_nodeClicked(this.treeView_ClientNodeClicked);
        $('#'.concat(this.pnlDetailsId)).hide();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.paneSelectionId);
        this.loadLevels();

    }


    /**
    *------------------------- Events -----------------------------
    */

    private toolBarSearch_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, eventArgs: Telerik.Web.UI.RadToolBarEventArgs) => {
        //this._btnDelete.set_enabled(false);
        this._btnEdit.set_enabled(false);
        this._btnAdd.set_enabled(true);
        this.loadLevels();
    }

    private btnAdd_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        let url: string = "../Tblt/TbltPrivacyLevelGes.aspx?Type=Comm&Action=Add";
        this.openWindow(url, 500, 400, "Livelli di ".concat(this.privacyLabel, " - Aggiungi"));
    }

    private btnEdit_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {

        let selectedNode: Telerik.Web.UI.RadTreeNode = this._currentSelectedNode;
        if (selectedNode == undefined || selectedNode.get_value() == null || selectedNode.get_value() == "") {
            this.showWarningMessage(this.uscNotificationId, 'Nessun livello selezionato.');
            return;
        }

        this.setPrivacyLevelNode();
        let url: string = "../Tblt/TbltPrivacyLevelGes.aspx?Type=Comm&Action=Edit&IdPrivacyLevel=".concat(selectedNode.get_value());
        this.openWindow(url, 500, 400, "Livelli di ".concat(this.privacyLabel, " - Modifica"));
    }

    private btnDelete_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._currentSelectedNode;
        if (selectedNode == undefined || selectedNode.get_value() == null || selectedNode.get_value() == "") {
            this.showWarningMessage(this.uscNotificationId, 'Nessun livello selezionato.');
            return;
        }

        this._manager.radconfirm("Sei sicuro di voler eliminare il livello di ".concat(this.privacyLabel, "?"), (arg) => {
            if (arg) {
                let privacyLevel = new PrivacyLevelModel();
                privacyLevel.UniqueId = this._currentSelectedNode.get_value();
                privacyLevel.IsActive = false;
                this._privacyLevelService.updatePrivacyLevel(privacyLevel,
                    (data: any) => {
                        this._btnAdd.set_enabled(false);
                        this._btnEdit.set_enabled(false);
                        //this._btnDelete.set_enabled(false);
                        this.removeNode(privacyLevel.UniqueId);
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

    private treeView_ClientNodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        this._currentSelectedNode = args.get_node();
        sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);

        $('#'.concat(this.pnlDetailsId)).show();
        if (!this._currentSelectedNode || !this._currentSelectedNode.get_value()) {
            $('#'.concat(this.pnlDetailsId)).hide();
            this._btnAdd.set_enabled(true);
            this._btnEdit.set_enabled(false);
            //this._btnDelete.set_enabled(false);

            return;
        }

        //this._btnDelete.set_enabled(true);
        this._btnEdit.set_enabled(true);
        this._btnAdd.set_enabled(false);
        if (this._currentSelectedNode == this._rtvLevels.get_nodes().getNode(0)) {
            this._btnAdd.set_enabled(true);
            this._btnEdit.set_enabled(false);
            //this._btnDelete.set_enabled(false);
            $('#'.concat(this.pnlDetailsId)).hide();
            return;
        }

        this._loadingPanel.show(this.paneSelectionId);
        this._loadingPanel.show(this.pnlDetailsId);
        this.loadPrivacyLevel();        
    }
    /**
     *------------------------- Methods -----------------------------
     */

    private loadLevels() {
        let txtDescription: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>this._toolBarSearch.findItemByValue('searchDescription').findControl('txtDescription');
        let description: string = txtDescription ? txtDescription.get_value() : '';

        this._privacyLevelService.findPrivacyLevels(description,
            (data: any) => {
                if (data) {
                    this.setNodes(data);
                    this._loadingPanel.hide(this.paneSelectionId);
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.paneSelectionId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
    * Crea e imposta i nodi nella RadTreeView di visualizzazione    
    */
    private setNodes = (levels: PrivacyLevelModel[]) => {
        let rootNode: Telerik.Web.UI.RadTreeNode = this._rtvLevels.get_nodes().getNode(0);
        rootNode.get_nodes().clear();

        $.each(levels, (index: number, item: PrivacyLevelModel) => {
            //Verifico se il nodo già esiste nella treeview
            if (this._rtvLevels.findNodeByValue(item.UniqueId)) {
                return;
            }
            let newNode: Telerik.Web.UI.RadTreeNode = this.decorateNode(item);
            rootNode.get_nodes().add(newNode);
            this.setNodeColour(newNode, item.Colour);
        });
        rootNode.set_expanded(true);
        this._rtvLevels.commitChanges();
    }

    private decorateNode(level: PrivacyLevelModel): Telerik.Web.UI.RadTreeNode {
        let newNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        newNode = this.setNodeAttribute(newNode, level);
        newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/lock.png");
        newNode.set_selectedCssClass("selectedLevel");
        return newNode;
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

    protected showWarningMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showWarningMessage(customMessage)
        }
    }

    private openWindow(url: string, width: number, height: number, title?: string) {
        if (title) {
            this._windowAddPrivacyLevel.set_title(title);
        }
        this._windowAddPrivacyLevel.setSize(width, height);
        this._windowAddPrivacyLevel.setUrl(url);
        this._windowAddPrivacyLevel.set_modal(true);
        this._windowAddPrivacyLevel.show();
    }

    private closeInsertWindow = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        let result: AjaxModel = <AjaxModel>args.get_argument();
        if (result) {
            switch (result.ActionName) {
                case "Add":
                    let newLevel: PrivacyLevelModel = JSON.parse(result.Value[0]);
                    let newNode = this.decorateNode(newLevel);
                    let rootNode: Telerik.Web.UI.RadTreeNode = this._rtvLevels.get_nodes().getNode(0);
                    rootNode.get_nodes().add(newNode);
                    this.setNodeColour(newNode, newLevel.Colour);
                    this.refreshLevels();
                    break;
                case "Edit":
                    let updatedNode: Telerik.Web.UI.RadTreeNode = this._rtvLevels.get_selectedNode();
                    if (result && result.Value[0]) {
                        let level: PrivacyLevelModel = JSON.parse(result.Value[0]);
                        this.setNodeAttribute(updatedNode, level);
                        this.setNodeColour(updatedNode, level.Colour);
                        this._rtvLevels.commitChanges();
                        this.loadPrivacyLevel();
                        this.refreshLevels();
                    }
                    break;
            }
        }
    }

    private refreshLevels() {
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.ActionName = 'Refresh';
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    private setNodeAttribute = (node: Telerik.Web.UI.RadTreeNode, level: PrivacyLevelModel) => {
        node.set_text(level.Description);
        node.set_value(level.UniqueId);
        node.get_attributes().setAttribute("Level", level.Level);
        node.get_attributes().setAttribute("IsActive", level.IsActive);
        node.get_attributes().setAttribute("Colour", level.Colour);
        node.set_cssClass(level.IsActive ? "" : "node-disabled");
        return node;
    }

    private setNodeColour(node: Telerik.Web.UI.RadTreeNode, colour: string) {
        if (colour && colour.toLowerCase() != '#ffffff') {
            node.get_textElement().style.color = colour;
        }        
    }

    /**
    * Caricamento del nodo selezionato nella Session Storage    
    */
    private setPrivacyLevelNode = () => {
        let privacyLevelNode: PrivacyLevelModel = new PrivacyLevelModel();
        privacyLevelNode.UniqueId = this._currentSelectedNode.get_value();
        privacyLevelNode.Description = this._currentSelectedNode.get_text();
        privacyLevelNode.Level = Number(this._currentSelectedNode.get_attributes().getAttribute("Level"));
        privacyLevelNode.IsActive = this._currentSelectedNode.get_attributes().getAttribute("IsActive");
        privacyLevelNode.Colour = this._currentSelectedNode.get_attributes().getAttribute("Colour");
        sessionStorage[privacyLevelNode.UniqueId] = JSON.stringify(privacyLevelNode);
    }

    /**
    * Carica la tipologia selezionata
    */
    private loadPrivacyLevel = () => {
        this._privacyLevelService.getById(this._currentSelectedNode.get_value(),
            (data: any) => {
                if (data) {
                    this._currentPrivacyLevel = data;
                    this.loadDetails();
                    this._btnEdit.set_enabled(true);
                    //this._btnDelete.set_enabled(true);                    
                    this._loadingPanel.hide(this.pnlDetailsId);
                    this._loadingPanel.hide(this.pnlInformationsId);
                    this._loadingPanel.hide(this.paneSelectionId);
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.paneSelectionId);
                this._loadingPanel.hide(this.pnlInformationsId);
                this._loadingPanel.hide(this.pnlDetailsId);
                this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento del livello.");
            }
        );
    }

    /**
    * Carica i dettagli della tipologia selezionata nella sezione dettagli
    */
    private loadDetails = () => {
        $('#'.concat(this.txtDescriptionId)).text(this._currentPrivacyLevel.Description);
        $('#'.concat(this.txtLevelId)).text(this._currentPrivacyLevel.Level);
        $('#'.concat(this.lblActiveFromId)).text(moment(this._currentPrivacyLevel.RegistrationDate).format("DD/MM/YYYY"));
        $('#'.concat(this.lblIsActiveId)).text(this._currentPrivacyLevel.IsActive ? 'Attivo' : 'Non Attivo');
        let colour: string = this._currentPrivacyLevel.Colour;
        if (!colour) {
            colour = '#ffffff';
        }
        $('#'.concat(this.colorBoxId)).css('background-color', colour);
    }

    private removeNode = (idPrivacyLevel: string) => {
        let nodeToRemove: Telerik.Web.UI.RadTreeNode = this._rtvLevels.findNodeByValue(idPrivacyLevel);
        if (nodeToRemove) {
            this._rtvLevels.get_nodes().remove(nodeToRemove);
            this._rtvLevels.commitChanges();
            $('#'.concat(this.pnlDetailsId)).hide();
        }
    }
}
export = TbltPrivacyLevel;