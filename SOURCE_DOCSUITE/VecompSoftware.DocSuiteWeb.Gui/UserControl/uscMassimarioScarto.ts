/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/helpers/stringhelper.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import MassimarioScartoModel = require('App/Models/MassimariScarto/MassimarioScartoModel');
import MassimarioScartoStatusType = require('App/Models/MassimariScarto/MassimarioScartoStatusType');
import MassimarioScartoService = require('App/Services/MassimariScarto/MassimarioScartoService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import StringHelper = require('App/Helpers/StringHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');

declare var hasFilter: boolean;
class uscMassimarioScarto {
    treeMassimarioId: string;
    toolBarSearchId: string;
    hideCanceledFilter: boolean;
    uscNotificationId: string;

    private _treeMassimario: Telerik.Web.UI.RadTreeView;
    private _toolBarSearch: Telerik.Web.UI.RadToolBar;
    private _service: MassimarioScartoService;
    static ON_SELECTED_NODE_EVENT = "onSelectedNode";
    static ON_START_LOAD_EVENT = "onStartLoad";
    static ON_END_LOAD_EVENT = "onEndLoad";
    static ON_ERROR_EVENT = "onErrorEvent";

    /**
     * Costruttore
     * @param serviceConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "MassimarioScarto");
        if (!serviceConfiguration) {
            this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione. Nessun servizio configurato per il Massimario di Scarto");
            return;
        }
        this._service = new MassimarioScartoService(serviceConfiguration);
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
    treeView_ClientNodeExpanding = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let node: Telerik.Web.UI.RadTreeNode = args.get_node();
        let strNodeType: string = <string>node.get_attributes().getAttribute("NodeType");
        if (node.get_nodes().get_count() == 0 && !hasFilter) {
            args.set_cancel(true);
            node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            node.set_selected(true);
            this._service.getMassimariByParent(this.getSearchIncludeCancel(), node.get_attributes().getAttribute("UniqueId"),
                (data: any) => {
                    this.loadNodes(data);
                    $("#".concat(this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_SELECTED_NODE_EVENT, node.toJsonString())
                }, this.errorEventCallback);
        } else {
            $("#".concat(this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_SELECTED_NODE_EVENT, node.toJsonString());
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
        if (node.get_value() != "0" && node.get_nodes().get_count() == 0 && !hasFilter) {
            node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            this._service.getMassimariByParent(this.getSearchIncludeCancel(), node.get_attributes().getAttribute("UniqueId"),
                (data: any) => {
                    this.loadNodes(data);
                    $("#".concat(this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_SELECTED_NODE_EVENT, node.toJsonString())
                }, this.errorEventCallback);
        } else {
            $("#".concat(this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_SELECTED_NODE_EVENT, node.toJsonString());
        }
    }

    /**
     * Evento scatenato al click di un RadButton nella toolbar di ricerca
     * @method
     * @param sender
     * @param eventArgs
     * @returns
     */
    toolBarSearch_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, eventArgs: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
        eventArgs.set_cancel(true);
        let rootNode: Telerik.Web.UI.RadTreeNode = this._treeMassimario.get_nodes().getNode(0);
        rootNode.get_nodes().clear();
        rootNode.select();

        let txtSearchDescription: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>sender.findItemByValue('searchDescription').findControl('txtSearchName');
        let txtSearchCode: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>sender.findItemByValue('searchCode').findControl('txtSearchCode');

        let description: string = txtSearchDescription.get_value();
        let code: string = null;
        if (!String.isNullOrEmpty(txtSearchCode.get_value()))
            code = txtSearchCode.get_value();

        $("#".concat(this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_START_LOAD_EVENT);
        if (String.isNullOrEmpty(description) && code == undefined) {
            hasFilter = false;
            this._service.getMassimariByParent(this.getSearchIncludeCancel(), null, this.loadNodes, this.errorEventCallback);
        } else {
            hasFilter = true;
            this._service.findMassimari(description, this.getSearchIncludeCancel(), code, this.loadNodes, this.errorEventCallback);
        }
    }

    /**
     *------------------------- Methods -----------------------------
     */
    /**
    * Metodo di inizializzazione
    */
    initialize(): void {

        this._treeMassimario = <Telerik.Web.UI.RadTreeView>$find(this.treeMassimarioId);
        this._toolBarSearch = <Telerik.Web.UI.RadToolBar>$find(this.toolBarSearchId);
        this._toolBarSearch.add_buttonClicking(this.toolBarSearch_ButtonClicked);
        if (this.hideCanceledFilter) {
            (<Telerik.Web.UI.RadButton>this._toolBarSearch.findItemByValue('includeCancel').findControl('btnIncludeCancel')).set_visible(false);
        }
        $("#".concat(this.treeMassimarioId)).data(this);

        hasFilter = false;
        //Caricamento dati iniziale
        this._service.getMassimariByParent(this.getSearchIncludeCancel(), null, this.loadNodes, this.errorEventCallback);
    }

    /**
    * Lancio un evento alla pagina di una eccezione avvenuta tramite una richiesta ajax.
    * Gestisco comunque l'errore nella console.
    * @param XMLHttpRequest
    * @param textStatus
    * @param errorThrown
    */
    private errorEventCallback = (exception:ExceptionDTO): void => {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotification(exception);
        }
        $("#".concat(this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_ERROR_EVENT, XMLHttpRequest);
    }




    /**
     * Imposta gli attributi di un nodo
     * @param node
     * @param massimario
     */
    private setNodeAttribute(node: Telerik.Web.UI.RadTreeNode, massimario: MassimarioScartoModel): Telerik.Web.UI.RadTreeNode {
        node.get_attributes().setAttribute("UniqueId", massimario.UniqueId);
        node.get_attributes().setAttribute("MassimarioScartoLevel", massimario.MassimarioScartoLevel);
        node.get_attributes().setAttribute("Code", massimario.Code);
        node.get_attributes().setAttribute("Name", massimario.Name);
        node.get_attributes().setAttribute("Note", massimario.Note);
        node.get_attributes().setAttribute("ConservationPeriod", massimario.ConservationPeriod);
        node.get_attributes().setAttribute("StartDate", massimario.StartDate);
        node.get_attributes().setAttribute("EndDate", massimario.EndDate);
        node.get_attributes().setAttribute("Status", massimario.Status);
        node.get_attributes().setAttribute("IsActive", massimario.isActive());
        return node;
    }

    /**
     * Carica gli elementi nella treeview
     * @method
     * @param data
     * @returns
     */
    loadNodes = (data: MassimarioScartoModel[], node?: Telerik.Web.UI.RadTreeNode): void => {
        let parent: Telerik.Web.UI.RadTreeNode = this._treeMassimario.get_selectedNode();
        if (node != undefined) {
            parent = node;
        }

        $.each(data, (index: number, massimario: MassimarioScartoModel) => {
            //Verifico se il nodo già esiste nella treeview
            if (this._treeMassimario.findNodeByValue(massimario.MassimarioScartoPath) != undefined) {
                return;
            }

            let newNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            let massimarioName: string = massimario.Code.toString().concat(".", massimario.Name);
            newNode.set_text(massimarioName);
            newNode.set_value(massimario.MassimarioScartoPath);
            if (!massimario.isActive()) {
                newNode.set_cssClass('node-disabled');
            }
            this.setNodeAttribute(newNode, massimario);

            //Gestisco il fatto che per il massimario di scarto saranno presenti al massimo 2 livelli
            if (massimario.MassimarioScartoLevel == 2) {
                newNode.set_imageUrl("../Comm/images/Classificatore.gif");
                let currentParent: Telerik.Web.UI.RadTreeNode = this._treeMassimario.findNodeByValue(massimario.MassimarioScartoParentPath);
                currentParent.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                currentParent.get_nodes().add(newNode);
                currentParent.set_expanded(true);
                return;
            } else {
                newNode.set_imageUrl("../Comm/images/FolderOpen16.gif");
                if (hasFilter) {
                    newNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                } else {
                    newNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ServerSideCallBack);
                }
            }
            parent.get_nodes().add(newNode);
        });

        parent.hideLoadingStatus();
        parent.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
        parent.set_expanded(true);
        this._treeMassimario.commitChanges();

        //Spedisco l'evento di fine loading massimari
        $("#".concat(this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_END_LOAD_EVENT);
    }

    /**
     * Recupera il modello dal nodo selezionato nella treeview
     */
    getSelectedMassimario(): MassimarioScartoModel {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._treeMassimario.get_selectedNode();
        let model: MassimarioScartoModel = new MassimarioScartoModel();
        model.UniqueId = selectedNode.get_attributes().getAttribute("UniqueId");
        model.Code = selectedNode.get_attributes().getAttribute("Code");
        model.Name = selectedNode.get_attributes().getAttribute("Name");
        model.Note = selectedNode.get_attributes().getAttribute("Note");
        model.ConservationPeriod = selectedNode.get_attributes().getAttribute("ConservationPeriod");
        model.StartDate = selectedNode.get_attributes().getAttribute("StartDate");
        model.EndDate = selectedNode.get_attributes().getAttribute("EndDate");
        if (selectedNode.get_attributes().getAttribute("Status") != undefined) {
            model.Status = MassimarioScartoStatusType[<string>selectedNode.get_attributes().getAttribute("Status")];
        }
        model.MassimarioScartoLevel = selectedNode.get_attributes().getAttribute("MassimarioScartoLevel");
        model.FakeInsertId = selectedNode.get_parent().get_attributes().getAttribute("UniqueId");
        return model;
    }

    /**
 * Aggiorna il nodo padre del nodo selezionato
 */
    updateParentNode(callback: Function): void {
        let parentNode: Telerik.Web.UI.RadTreeNode = this._treeMassimario.get_selectedNode().get_parent();
        parentNode.get_nodes().clear();
        parentNode.select();

        let idMassimarioScarto: string = null;
        if (parentNode.get_value() != 0) {
            idMassimarioScarto = parentNode.get_attributes().getAttribute("UniqueId");
        }

        this._service.getMassimariByParent(this.getSearchIncludeCancel(), idMassimarioScarto, (data: MassimarioScartoModel[]) => {
            this.loadNodes(data);
            if (callback)
                callback();
        }, this.errorEventCallback);
    }

    /**
     * Aggiorna i nodi figli del nodo selezionato
     */
    updateSelectedNodeChildren(): void {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._treeMassimario.get_selectedNode();
        selectedNode.get_nodes().clear();

        let idMassimarioScarto: string = null;
        if (selectedNode.get_value() != 0) {
            idMassimarioScarto = selectedNode.get_attributes().getAttribute("UniqueId");
        }

        this._service.getMassimariByParent(this.getSearchIncludeCancel(), idMassimarioScarto, this.loadNodes, this.errorEventCallback );
    }

    /**
     * Verifica se tutti i nodi figli del nodo selezionato sono in stato annullato
     */
    allSelectedChildrenIsCancel(): boolean {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._treeMassimario.get_selectedNode();
        let nodes: Array<Telerik.Web.UI.RadTreeNode> = selectedNode.get_nodes().toArray();
        return nodes.filter(node => MassimarioScartoStatusType[<string>node.get_attributes().getAttribute("Status")] == MassimarioScartoStatusType.Active).length == 0;
    }

    /**
     * Ritorna se devono essere inclusi gli elementi annullati nella ricerca. Data dal valore del controllo.
     */
    getSearchIncludeCancel(): boolean {
        if (this.hideCanceledFilter) return false;

        let includeCancel: boolean = (<Telerik.Web.UI.RadButton>this._toolBarSearch.findItemByValue('includeCancel').findControl('btnIncludeCancel')).get_checked();
        return includeCancel;
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }
}

export = uscMassimarioScarto;