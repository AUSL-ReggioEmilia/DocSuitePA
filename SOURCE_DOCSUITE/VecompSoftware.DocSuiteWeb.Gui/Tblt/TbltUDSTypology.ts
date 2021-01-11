/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UDSTypologyService = require('App/Services/UDS/UDSTypologyService');
import UDSRepositoryService = require('App/Services/UDS/UDSRepositoryService');
import UDSTypologyModel = require('App/Models/UDS/UDSTypologyModel');
import UDSTypologyStatus = require('App/Models/UDS/UDSTypologyStatus');
import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import AjaxModel = require('App/Models/AjaxModel');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import EnumHelper = require('App/Helpers/EnumHelper');

class TbltUDSTypology {

    pnlDetailsId: string;
    pnlInformationsId: string;
    pnlButtonsId: string;
    toolBarSearchId: string;
    rtvTypologyId: string;
    uscNotificationId: string;
    windowAddUDSTypologyId: string;
    ajaxLoadingPanelId: string;
    paneSelectionId: string;
    lblStatusId: string;
    lblActiveFromId: string;
    grdUDSRepositoriesId: string;
    pnlUDSRepositoriesId: string;
    btnAggiungiId: string;
    btnRimuoviId: string;
    folderToolBarId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _udsTypologyService: UDSTypologyService;
    private _toolBarSearch: Telerik.Web.UI.RadToolBar;
    private _toolBarStatus: Telerik.Web.UI.RadToolBar;
    private _rtvTypology: Telerik.Web.UI.RadTreeView;
    private _btnAggiungi: Telerik.Web.UI.RadButton;
    private _btnRimuovi: Telerik.Web.UI.RadButton;
    private _windowAddUDSTypology: Telerik.Web.UI.RadWindow;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _currentSelectedNode: Telerik.Web.UI.RadTreeNode;
    private _udsRepositoryService: UDSRepositoryService;
    private _enumHelper: EnumHelper;
    private _grdUDSRepositories: Telerik.Web.UI.RadGrid;
    private _currentUDSTypologyUDSRepositories: UDSRepositoryModel[];
    private _currentUDSTypology: UDSTypologyModel;
    private _folderToolBar: Telerik.Web.UI.RadToolBar;

    protected static UDSREPOSITORY_TYPE_NAME = "UDSRepository";
    private static CREATE_OPTION = "create";
    private static MODIFY_OPTION = "modify"

    /**
   * Costruttore
   * @param serviceConfigurations
   */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "UDSTypology");
        this._udsTypologyService = new UDSTypologyService(serviceConfiguration);

        let udsRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, TbltUDSTypology.UDSREPOSITORY_TYPE_NAME);
        this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
        this._enumHelper = new EnumHelper();
    }

    /**
    * Inizializzazione classe
    */
    initialize =() => {
        this._rtvTypology = <Telerik.Web.UI.RadTreeView>$find(this.rtvTypologyId);
        this._toolBarSearch = <Telerik.Web.UI.RadToolBar>$find(this.toolBarSearchId);
        this._folderToolBar = <Telerik.Web.UI.RadToolBar>$find(this.folderToolBarId);
        this._folderToolBar.add_buttonClicked(this.folderToolBar_onClick);
        this._btnAggiungi = <Telerik.Web.UI.RadButton>$find(this.btnAggiungiId);
        this._btnRimuovi = <Telerik.Web.UI.RadButton>$find(this.btnRimuoviId);
        this._windowAddUDSTypology = <Telerik.Web.UI.RadWindow>$find(this.windowAddUDSTypologyId);
        this._windowAddUDSTypology.add_close(this.closeInsertWindow);
        this._toolBarSearch.add_buttonClicked(this.toolBarSearch_ButtonClicked);
        this._folderToolBar.findItemByValue(TbltUDSTypology.MODIFY_OPTION).set_enabled(false);
        this._btnAggiungi.add_clicked(this.btnAggiungi_OnClick);
        this._btnRimuovi.add_clicked(this.btnRimuovi_OnClick);
        this._rtvTypology.add_nodeClicked(this.treeView_ClientNodeClicked);
        this._grdUDSRepositories = <Telerik.Web.UI.RadGrid>$find(this.grdUDSRepositoriesId);
        $('#'.concat(this.pnlDetailsId)).hide();
        $('#'.concat(this.pnlButtonsId)).hide();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.paneSelectionId);
        this.loadTypologies();

    }


    /**
    *------------------------- Events -----------------------------
    */

    private toolBarSearch_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, eventArgs: Telerik.Web.UI.RadToolBarEventArgs) => {
        this.loadTypologies();
    }
    
    folderToolBar_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        switch (args.get_item().get_value()) {
            case TbltUDSTypology.CREATE_OPTION: {
                let url: string = "../Tblt/TbltUDSTypologyGes.aspx?Type=Comm&Action=Add";
                this.openWindow(url, 450, 250, "Tipologie - Aggiungi");
                break;
            }
            case "delete": {
                
                break;
            }
            case TbltUDSTypology.MODIFY_OPTION: {
                let selectedNode: Telerik.Web.UI.RadTreeNode = this._currentSelectedNode;
                if (selectedNode == undefined || selectedNode.get_value() == null || selectedNode.get_value() == "") {
                    this.showWarningMessage(this.uscNotificationId, 'Nessuna tipologia selezionata');
                    return;
                }

                this.setUDSTypologyNode();
                let url: string = "../Tblt/TbltUDSTypologyGes.aspx?Type=Comm&Action=Edit&IdUDSTypology=".concat(selectedNode.get_value());
                this.openWindow(url, 450, 250, "Tipologie - Modifica");
                break;
            }
        }
    }

    private btnAggiungi_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._currentSelectedNode;
        if (selectedNode == undefined || selectedNode.get_value() == null || selectedNode.get_value() == "") {
            this.showWarningMessage(this.uscNotificationId, 'Nessuna tipologia selezionata');
            return;
        }
        this.setUDSTypologyNode();
        let url: string = "../Tblt/TbltUDSRepositoriesTypologyGes.aspx?Type=Comm&IdUDSTypology=".concat(selectedNode.get_value());
        this.openWindow(url, 600, 450, "Tipologie - Aggiungi Archivio");
    }

    private btnRimuovi_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._currentSelectedNode;
        if (selectedNode == undefined || selectedNode.get_value() == null || selectedNode.get_value() == "") {
            this.showWarningMessage(this.uscNotificationId, 'Nessuna tipologia selezionata');
            return;
        }
        this.setUDSTypologyNode();

        if (!this._grdUDSRepositories.get_selectedItems() || this._grdUDSRepositories.get_selectedItems().length < 1) {
            this.showNotificationException(this.uscNotificationId, null, "Selezionare un archivio da rimuovere");
            return;
        }

        this._loadingPanel.show(this.pnlDetailsId);

        let currentUDSTypologyModel: UDSTypologyModel = <UDSTypologyModel>JSON.parse(sessionStorage[selectedNode.get_value()]);
        let udsRepositories = this._grdUDSRepositories.get_selectedItems();

        let notDeletedUDSReposiotries: UDSRepositoryModel[] = new Array<UDSRepositoryModel>();

        for (let item of currentUDSTypologyModel.UDSRepositories) {
            let selectedItems = udsRepositories.filter((rep) => { if (rep._dataItem.UniqueId == item.UniqueId) return rep._dataItem; });
            if ( !selectedItems || selectedItems.length<1) {
                notDeletedUDSReposiotries.push(item);
            }
        }

        currentUDSTypologyModel.UDSRepositories = notDeletedUDSReposiotries;

        this._udsTypologyService.updateUDSTypology(currentUDSTypologyModel,
            (data: UDSTypologyModel) => {
                if (data) {
                    this.loadDetailsTypology();
                    this._loadingPanel.hide(this.pnlDetailsId);
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pnlDetailsId);
                this.showNotificationException(this.uscNotificationId, exception);
        });
    }

    private treeView_ClientNodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        this._currentSelectedNode = args.get_node();
        sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);

        $('#'.concat(this.pnlDetailsId)).show();
        if (!this._currentSelectedNode || !this._currentSelectedNode.get_value()) {
            $('#'.concat(this.pnlDetailsId)).hide();
            $('#'.concat(this.pnlButtonsId)).hide();
            this._folderToolBar.findItemByValue(TbltUDSTypology.CREATE_OPTION).set_enabled(true);
            this._folderToolBar.findItemByValue(TbltUDSTypology.MODIFY_OPTION).set_enabled(false);
            return;
        }

        this._folderToolBar.findItemByValue(TbltUDSTypology.CREATE_OPTION).set_enabled(false);
        this._folderToolBar.findItemByValue(TbltUDSTypology.MODIFY_OPTION).set_enabled(true);
        if (this._currentSelectedNode == this._rtvTypology.get_nodes().getNode(0)) {
            this._folderToolBar.findItemByValue(TbltUDSTypology.CREATE_OPTION).set_enabled(true);
            this._folderToolBar.findItemByValue(TbltUDSTypology.MODIFY_OPTION).set_enabled(false);
            this._btnAggiungi.set_enabled(false);
            this._btnRimuovi.set_enabled(false);
            $('#'.concat(this.pnlDetailsId)).hide();
            $('#'.concat(this.pnlButtonsId)).hide();
            return;
        }

        this._loadingPanel.show(this.paneSelectionId);
        this._loadingPanel.show(this.pnlDetailsId);

        $.when(this.loadUDSTypology(), this.loadUDSRepositories()).done(() => {
            let inactive: boolean = (parseInt(UDSTypologyStatus[this._currentUDSTypology.Status]) == UDSTypologyStatus.Inactive);
            this._folderToolBar.findItemByValue(TbltUDSTypology.MODIFY_OPTION).set_enabled(!inactive);
            this._btnAggiungi.set_enabled(!inactive);
            this._btnRimuovi.set_enabled(!inactive);

            this.loadDetails();
            this.fillDetailsTable(this._currentUDSTypologyUDSRepositories);            
            $('#'.concat(this.pnlButtonsId)).show();
            this._loadingPanel.hide(this.paneSelectionId);
            this._loadingPanel.hide(this.pnlDetailsId);
        }).fail((exception) => {
            $('#'.concat(this.pnlButtonsId)).show();
            this._loadingPanel.hide(this.paneSelectionId);
            this._loadingPanel.hide(this.pnlDetailsId);
            this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento della Tipologia.");
        });
    }
    /**
     *------------------------- Methods -----------------------------
     */

    private loadTypologies() {
        let txtDescription: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>this._toolBarSearch.findItemByValue('searchDescription').findControl('txtDescription');
        let checkedButtons: Telerik.Web.UI.RadToolBarItem[] = this._toolBarSearch.get_items().toArray().filter(i => (<any>i).get_checked());
        let description: string = txtDescription ? txtDescription.get_value() : '';
        let status: number = null;

        if (checkedButtons && checkedButtons.length == 1) {
            status = UDSTypologyStatus[checkedButtons[0].get_value()];
        }
        this._udsTypologyService.getUDSTypologyByName(description, status,
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
    private setNodes = (typologies: UDSTypologyModel[]) => {
        let rootNode: Telerik.Web.UI.RadTreeNode = this._rtvTypology.get_nodes().getNode(0);
        rootNode.get_nodes().clear();

        $.each(typologies, (index: number, item: UDSTypologyModel) => {
            //Verifico se il nodo già esiste nella treeview
            if (this._rtvTypology.findNodeByValue(item.UniqueId) != undefined) {
                return;
            }
            let newNode: Telerik.Web.UI.RadTreeNode = this.decorateNode(item);
            rootNode.get_nodes().add(newNode);
        });
        rootNode.set_expanded(true);
        this._rtvTypology.commitChanges();
    }

    private decorateNode(typology: UDSTypologyModel): Telerik.Web.UI.RadTreeNode {
        let newNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        newNode = this.setNodeAttribute(newNode, typology);
        newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/document_copies.png");
        if (parseInt(UDSTypologyStatus[typology.Status]) == UDSTypologyStatus.Inactive) {
            newNode.set_cssClass('node-disabled');
        }
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
        if (title){
            this._windowAddUDSTypology.set_title(title);
        }
        this._windowAddUDSTypology.setSize(width, height);
        this._windowAddUDSTypology.setUrl(url);
        this._windowAddUDSTypology.set_modal(true);
        this._windowAddUDSTypology.show();
    }

    private closeInsertWindow = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        let result: AjaxModel = <AjaxModel>args.get_argument();
        if (result) {
            switch (result.ActionName) {
                case "Add":
                    let checkedButtons: Telerik.Web.UI.RadToolBarItem[] = this._toolBarStatus.get_items().toArray().filter(i => (<any>i).get_checked());
                    if (!checkedButtons || checkedButtons.filter(c => c.get_value() == UDSTypologyStatus[UDSTypologyStatus.Active]).length > 0) {
                        let newTypology: UDSTypologyModel = JSON.parse(result.Value[0]);
                        let newNode = this.decorateNode(newTypology);
                        let rootNode: Telerik.Web.UI.RadTreeNode = this._rtvTypology.get_nodes().getNode(0);
                        rootNode.get_nodes().add(newNode);
                    }
                    break;
                case "Edit":
                    let updatedNode: Telerik.Web.UI.RadTreeNode = this._rtvTypology.get_selectedNode();
                    if (result && result.Value[0]) {
                        let typology: UDSTypologyModel = JSON.parse(result.Value[0]);
                        this.setNodeAttribute(updatedNode, typology);
                        this._rtvTypology.commitChanges();
                    }
                    break;
                case "AddUDSRepositories":
                    this.loadDetailsTypology(); 
                    break;
            }
        }
    }

    private setNodeAttribute = (node: Telerik.Web.UI.RadTreeNode, typology: UDSTypologyModel) => {
        node.get_attributes().setAttribute("Status", typology.Status);
        node.set_text(typology.Name);
        node.set_value(typology.UniqueId);
        return node;
    }

    /**
    * Caricamento del nodo selezionato nella Session Storage    
    */
    private setUDSTypologyNode = () => {
        let udsTypologyNode: UDSTypologyModel = new UDSTypologyModel();
        udsTypologyNode.UniqueId = this._currentSelectedNode.get_value();
        udsTypologyNode.Name = this._currentSelectedNode.get_text();
        udsTypologyNode.Status = this._currentSelectedNode.get_attributes().getAttribute("Status");
        udsTypologyNode.UDSRepositories = this._currentUDSTypologyUDSRepositories;
        sessionStorage[udsTypologyNode.UniqueId] = JSON.stringify(udsTypologyNode);
    }

    /**
    * Carica la tipologia selezionata
    */
    private loadUDSTypology = () => {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._udsTypologyService.getUDSTypologyById(this._currentSelectedNode.get_value(),
                (data: any) => {
                    if (data == null){
                        promise.resolve();
                        return;
                    }
                    this._currentUDSTypology = data;
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

     /**
     * Carica i dettagli della tipologia selezionata nella sezione dettagli
     */
    private loadDetails = () => {
        $('#'.concat(this.lblStatusId)).text(this._enumHelper.getUDSTypologyStatusDescription(this._currentUDSTypology.Status));
        $('#'.concat(this.lblActiveFromId)).text(moment(this._currentUDSTypology.RegistrationDate).format("DD/MM/YYYY"));        
    }

    /**
     * Imposta gli archivi della tipologia selezionata
     */
    private loadUDSRepositories = () => {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._udsRepositoryService.getUDSRepositoryByUDSTypology(this._currentSelectedNode.get_value(),
                (response: any) => {
                    if (response == null) {
                        promise.resolve();
                        return;
                    }
                    this._currentUDSTypologyUDSRepositories = response;
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

     /**
     * Carica gli archivi della tipologia selezionata nella griglia
     */
    private fillDetailsTable = (udsRepositories: UDSRepositoryModel[]) => {                
        this._grdUDSRepositories = <Telerik.Web.UI.RadGrid>$find(this.grdUDSRepositoriesId);
        let grdUDSRepositoriesMasterTableView: Telerik.Web.UI.GridTableView = this._grdUDSRepositories.get_masterTableView();

        grdUDSRepositoriesMasterTableView.set_dataSource(udsRepositories);
        grdUDSRepositoriesMasterTableView.clearSelectedItems();
        grdUDSRepositoriesMasterTableView.dataBind();
    }

    /**
    * Carica il pannello dei detttagli
    */
    private loadDetailsTypology = () => {
        $.when(this.loadUDSTypology(), this.loadUDSRepositories()).done(() => {
            this.loadDetails();
            this._loadingPanel.hide(this.pnlInformationsId);
            this.fillDetailsTable(this._currentUDSTypologyUDSRepositories);
            this._loadingPanel.hide(this.pnlUDSRepositoriesId);
            $('#'.concat(this.pnlButtonsId)).show();
            this._loadingPanel.hide(this.paneSelectionId);
        }).fail((exception) => {
            $('#'.concat(this.pnlButtonsId)).show();
            this._loadingPanel.hide(this.paneSelectionId);
            this._loadingPanel.hide(this.pnlInformationsId);
            this._loadingPanel.hide(this.pnlUDSRepositoriesId);
            this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento della Tipologia.");
        });
    }
}
export = TbltUDSTypology;