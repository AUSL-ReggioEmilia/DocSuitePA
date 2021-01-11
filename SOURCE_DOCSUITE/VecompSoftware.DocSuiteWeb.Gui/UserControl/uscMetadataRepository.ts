/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import MetadataRepositoryBase = require('Tblt/MetadataRepositoryBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import MetadataRepositoryViewModel = require('App/ViewModels/Commons/MetadataRepositoryViewModel');
import MetadataRepositoryStatus = require('App/Models/Commons/MetadataRepositoryStatus');

class uscMetadatarepository extends MetadataRepositoryBase {

    rtvMetadataRepositoryId: string;
    toolBarSearchId: string;
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    pageId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfigurationHelper: ServiceConfigurationHelper;
    private _rtvMetadataRepository: Telerik.Web.UI.RadTreeView;
    private _toolBarSearch: Telerik.Web.UI.RadToolBar;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    public static ON_ROOT_NODE_CLICKED = "onRootNodeClick";
    public static ON_NODE_CLICKED = "onNodeClicked";
    public static ON_TREEVIEW_LOADED = "onTreeViewLoaded";

    /**
     * Costruttore
     * @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME));
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     * --------------------------- Events -------------------------------
     */


    /**
     * Evento scatenato al click di un RadButton nella toolbar di ricerca
     * @param sender
     * @param eventArgs
     */
    toolBar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, eventArgs: Telerik.Web.UI.RadToolBarEventArgs) => {
        let txtSearchDescription: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>this._toolBarSearch.findItemByValue('searchDescription').findControl('txtRepositoryName');
        this.loadRepositories();
    }

    /**
     * Evento scatenato al click di un nodo
     * @param sender
     * @param eventArgs
     */
    treeView_ClientNodeClicked = (sender: Telerik.Web.UI.RadTreeView, eventArgs: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let selectedNode = eventArgs.get_node();
        if (selectedNode == this._rtvMetadataRepository.get_nodes().getNode(0)) {
            $("#".concat(this.pageId)).triggerHandler(uscMetadatarepository.ON_ROOT_NODE_CLICKED);
        }
        else {
            $("#".concat(this.pageId)).triggerHandler(uscMetadatarepository.ON_NODE_CLICKED, selectedNode.get_value());
        }
    }

    /**
     * --------------------------- Methods -------------------------------
     */

    /**
     * Funzione chiamata in inizializzazione
     */
    initialize() {
        super.initialize();
        this._rtvMetadataRepository = <Telerik.Web.UI.RadTreeView>$find(this.rtvMetadataRepositoryId);
        this._rtvMetadataRepository.add_nodeClicked(this.treeView_ClientNodeClicked);
        this._toolBarSearch = <Telerik.Web.UI.RadToolBar>$find(this.toolBarSearchId);
        this._toolBarSearch.add_buttonClicked(this.toolBar_ButtonClicked);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        this.loadRepositories();
        $("#".concat(this.pageId)).data(this);
    }

    /**
     * Carico i nodi della RadTreeView
     */
    loadRepositories() {
        //se ripetuto mettere nell'initialize
        let txtSearchDescription: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>this._toolBarSearch.findItemByValue('searchDescription').findControl('txtRepositoryName');
        this._loadingPanel.show(this.pageId);
        this._service.findMetadataRepositories(txtSearchDescription.get_value(),
            (data: any) => {
                this.loadNodes(data);
            },
            (exception: ExceptionDTO) => {
                let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                    this._loadingPanel.hide(this.pageId);
                }
            }
        );
    }

    /**
     * Popolo i nodi della treeview con i vari metadati
     * @param repositories
     */
    loadNodes(repositories: MetadataRepositoryViewModel[]) {
        if (repositories == null) return;

        let rootNode = this._rtvMetadataRepository.get_nodes().getNode(0);
        rootNode.get_nodes().clear();

        let newNode: Telerik.Web.UI.RadTreeNode;
        $.each(repositories, (index: number, repository: MetadataRepositoryViewModel) => {

            newNode = new Telerik.Web.UI.RadTreeNode();
            newNode.set_text(repository.Name);
            newNode.set_value(repository.UniqueId);
            newNode.get_attributes().setAttribute("Status", repository.Status);
            switch (Number(MetadataRepositoryStatus[repository.Status])) {
                case MetadataRepositoryStatus.Confirmed:
                    newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/EnumDesigner_16x_green.png")
                    break;
                case MetadataRepositoryStatus.Draft:
                    newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/EnumDesigner_16x.png")
                    break;
            }
            rootNode.get_nodes().add(newNode);
        });

        rootNode.set_expanded(true);
        this._rtvMetadataRepository.commitChanges();
        this._loadingPanel.hide(this.pageId);
        $("#".concat(this.pageId)).triggerHandler(uscMetadatarepository.ON_TREEVIEW_LOADED);
    }

    getSelectedNode(): Telerik.Web.UI.RadTreeNode {
        if (!this._rtvMetadataRepository.get_selectedNode()) {
            return null;
        }
        return this._rtvMetadataRepository.get_selectedNode();
    }


    findNodeByValue(id: string): Telerik.Web.UI.RadTreeNode {
        if (id) {
            return this._rtvMetadataRepository.findNodeByValue(id);
        }
    }
}
export = uscMetadatarepository;