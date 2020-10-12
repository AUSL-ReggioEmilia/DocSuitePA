/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import DSWEnvironment = require('App/Models/Environment');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UDSRepositoryService = require('App/Services/UDS/UDSRepositoryService');
import CategoryFascicleService = require('App/Services/Commons/CategoryFascicleService');
import CategoryFascicleViewModel = require('App/ViewModels/Commons/CategoryFascicleViewModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import AjaxModel = require('App/Models/AjaxModel');
import CategoryFascicleModel = require('App/Models/Commons/CategoryFascicleModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import CategoryFascicleViewModelMapper = require('App/Mappers/Commons/CategoryFascicleViewModelMapper');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class uscFasciclePlan {

    btnAddPeriodicPlan: string;
    btnRemovePeriodicPlan: string;

    rtvEnvironmentsId: string;
    pnlFasciclePlanId: string;
    ajaxLoadingPanelId: string;
    currentCategoryId: string;
    currentResolutionName: string;
    currentDocumentSeriesName: string;
    managerCreatePeriodId: string;
    managerId: string;
    managerWindowId: string;
    pnlTreeViewId: string;
    uscNotificationId: string;
    ajaxManagerId: string;

    private _btnAddPeriodicPlan: Telerik.Web.UI.RadButton;
    private _btnRemovePeriodicPlan: Telerik.Web.UI.RadButton;

    private _rtvEnvironments: Telerik.Web.UI.RadTreeView;
    private _serviceConfigurations: ServiceConfiguration[];
    private _udsRepositoryService: UDSRepositoryService;
    private _categoryFascicleService: CategoryFascicleService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _udsRepositories: Array<UDSRepositoryModel>
    private _managerCreatePeriod: Telerik.Web.UI.RadWindowManager;
    private _managerFlush: Telerik.Web.UI.RadWindowManager;
    private _manager: Telerik.Web.UI.RadWindowManager;


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
    * ---------------------- Events -------------------------
    */
    /**
    * Evento alla chiusura della radWindow
    * @param sender
    * @param args
    */
    managerCreatePeriod_OnClose = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {

            let item = <CategoryFascicleModel>{};
            item = JSON.parse(args.get_argument());
            //let node: Telerik.Web.UI.RadTreeNode = this._rtvEnvironments.get_selectedNode();
            //node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/history.png");
            //node.get_attributes().setAttribute("Type", "Periodic");
            //node.set_toolTip("Periodico");
            //node.get_attributes().setAttribute("Period", item.FasciclePeriod.PeriodDays);
            //node.set_value(item.UniqueId);
            this._btnRemovePeriodicPlan.set_enabled(false);
            let params: string = "ReloadNodes|".concat(item.Category.EntityShortId.toString());
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(params);
        }
    }

    /**
     * Evento chiamato al click del bottone Procedimento Amministrativo
     * @param sender
     * @param args
     */
    btnProcedure_onClientClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        this._manager.radconfirm("Sei sicuro di avviare un piano di fascicolazione per tutte le tipologie documentarie?", (arg) => {
            if (arg && this._rtvEnvironments.get_selectedNode() == this._rtvEnvironments.get_nodes().getNode(0)) {
                let rootNode: Telerik.Web.UI.RadTreeNode = this._rtvEnvironments.get_nodes().getNode(0);
                this._loadingPanel.show(this.pnlFasciclePlanId);
                let item = <CategoryFascicleModel>{};
                let ajaxModel: AjaxModel = <AjaxModel>{};
                ajaxModel.Value = new Array<string>();

                item.FascicleType = FascicleType['Procedure'];
                item.DSWEnvironment = 0;
                let category: CategoryModel = new CategoryModel;
                category.EntityShortId = rootNode.get_attributes().getAttribute("IdCategory");
                category.Id = rootNode.get_attributes().getAttribute("IdCategory");
                item.Category = category;

                ajaxModel.Value.push(JSON.stringify(item));
                ajaxModel.ActionName = "ProcedureExternalDataCallback";
                (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
                this._rtvEnvironments.commitChanges();
            }
        });
    }

    /**
     * Evento chiamato al click del bottone Procedimento Periodico
     * @param sender
     * @param args
     */
    btnAddPeriodicPlan_onClientClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvEnvironments.get_selectedNode();
        let rootNode: Telerik.Web.UI.RadTreeNode;
        rootNode = this._rtvEnvironments.get_nodes().getNode(0);

        let url: string = '../Tblt/TbltCreaFascicolo.aspx?Type=Comm&Environment='.concat(selectedNode.get_attributes().getAttribute("Environment"), '&IdCategory=', rootNode.get_attributes().getAttribute("IdCategory"));
        this.openWindow(url, "managerCreatePeriod", 750, 600);
    }

    /**
    * Evento scatenato al click di svuota
    * @param sender
    * @param args 
    */
    btnRemovePeriodicPlan_onClientClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        this._manager.radconfirm("Sicuro di procedere?", (arg) => {
            if (arg) {
                this._loadingPanel.show(this.pnlFasciclePlanId);
                let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvEnvironments.get_selectedNode();
                let rootNode: Telerik.Web.UI.RadTreeNode = this._rtvEnvironments.get_nodes().getNode(0);
                this._categoryFascicleService.getByIdCategory(rootNode.get_attributes().getAttribute("IdCategory"),
                    (data: CategoryFascicleModel[]) => {
                        if (data) {
                            let categoryFascicles: CategoryFascicleModel[] = data;
                            let categoryFascicleToDelete: CategoryFascicleModel[] = data.filter(item => item.UniqueId == selectedNode.get_value());
                            if (categoryFascicleToDelete && categoryFascicleToDelete.length > 0) {
                                let item: CategoryFascicleModel = categoryFascicleToDelete[0];
                                this._categoryFascicleService.deleteCategoryFascicle(item,
                                    (data: any) => {

                                        if (categoryFascicles.some(function (item) {
                                            return (item.FascicleType == FascicleType[FascicleType.Procedure.toString()] || item.FascicleType == FascicleType[FascicleType.SubFascicle.toString()]) && item.DSWEnvironment == 0
                                        })) {
                                            selectedNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_procedure.png");
                                            selectedNode.get_attributes().setAttribute("Type", "Procedure");
                                            selectedNode.set_toolTip("Procedimento");
                                            selectedNode.set_visible(false);
                                        }
                                        else {
                                            selectedNode.set_visible(false);
                                        }

                                        this._btnAddPeriodicPlan.set_enabled(false);
                                        this._btnRemovePeriodicPlan.set_enabled(true);

                                        this._rtvEnvironments.commitChanges();
                                        let params: string = "ReloadNodes|".concat(item.Category.EntityShortId.toString());
                                        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(params);
                                        this._loadingPanel.hide(this.pnlFasciclePlanId);

                                    },
                                    (exception: ExceptionDTO) => {
                                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                                        if (!jQuery.isEmptyObject(uscNotification)) {
                                            uscNotification.showNotification(exception);
                                        }
                                        this._loadingPanel.hide(this.pnlFasciclePlanId);
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
    }

    /**
    * Evento al click della Treeview
    * @param sender
    * @param eventArgs
    */
    environmentTreeView_ClientNodeClicked = (sender: Telerik.Web.UI.RadTreeView, eventArgs: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = eventArgs.get_node();

        switch (selectedNode.get_attributes().getAttribute("Type")) {
            case "Periodic": {
                this._btnAddPeriodicPlan.set_enabled(false);
                this._btnRemovePeriodicPlan.set_enabled(true);
                break;
            }
            case "Procedure": {
                this._btnAddPeriodicPlan.set_enabled(false);
                this._btnRemovePeriodicPlan.set_enabled(true);
                break;
            }
            case "ToManage": {
                this._btnAddPeriodicPlan.set_enabled(false);
                this._btnRemovePeriodicPlan.set_enabled(true);
                break;
            }
            case "SubFascicle": {
                this._btnAddPeriodicPlan.set_enabled(false);
                this._btnRemovePeriodicPlan.set_enabled(true);
                break;
            }
            default: {
                this._btnAddPeriodicPlan.set_enabled(true);
                this._btnRemovePeriodicPlan.set_enabled(false);
                break;
            }
        }
    }

    /**
     * Inizializzazione
     */
    initialize() {

        this._btnRemovePeriodicPlan = <Telerik.Web.UI.RadButton>$find(this.btnRemovePeriodicPlan);
        this._btnRemovePeriodicPlan.add_clicking(this.btnRemovePeriodicPlan_onClientClick);
        this._btnRemovePeriodicPlan.set_enabled(false);
        this._btnAddPeriodicPlan = <Telerik.Web.UI.RadButton>$find(this.btnAddPeriodicPlan);
        this._btnAddPeriodicPlan.add_clicking(this.btnAddPeriodicPlan_onClientClick);
        this._rtvEnvironments = <Telerik.Web.UI.RadTreeView>$find(this.rtvEnvironmentsId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._managerCreatePeriod = <Telerik.Web.UI.RadWindowManager>$find(this.managerCreatePeriodId)
        this._managerCreatePeriod.add_close(this.managerCreatePeriod_OnClose);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);

        let udsRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "UDSRepository");
        this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);

        let categoryFascicleService: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "CategoryFascicle");
        this._categoryFascicleService = new CategoryFascicleService(categoryFascicleService);

        if (this._rtvEnvironments.get_nodes().get_count() == 1) {
            this.loadNodes("0");
        }
        this.bindLoaded();
    }

    /**
    * ---------------------- Methods -------------------------
    */


    /**
    * Imposto gli attributi dei nodi
    */
    setNodesAttributes(idCategory: string) {

        this.currentCategoryId = idCategory;
        this._loadingPanel.show(this.pnlFasciclePlanId);
        this._categoryFascicleService.getByIdCategory(idCategory,
            (data: any) => {
                if (data) {
                    let rootNode: Telerik.Web.UI.RadTreeNode;
                    rootNode = this._rtvEnvironments.get_nodes().getNode(0);
                    rootNode.get_attributes().setAttribute("IdCategory", idCategory);
                    let mapper = new CategoryFascicleViewModelMapper();
                    let categoryFascicles: CategoryFascicleViewModel[] = mapper.MapCollection(data);
                    $.each(rootNode.get_allNodes(), (index: number, node: Telerik.Web.UI.RadTreeNode) => {

                        let env: number = node.get_attributes().getAttribute("Environment");
                        if (categoryFascicles.some(function (item) {
                            return item.Environment == env || item.Environment == 0
                        })) {
                            $.each(categoryFascicles, (index: number, categoryFascicle: CategoryFascicleViewModel) => {

                                switch (categoryFascicle.FascicleType) {
                                    case "Period": {
                                        if (categoryFascicle.Environment == env) {
                                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/history.png");
                                            node.get_attributes().setAttribute("Type", "Periodic");
                                            node.set_toolTip("Periodico");
                                            node.get_attributes().setAttribute("Period", categoryFascicle.PeriodName);
                                            node.set_value(categoryFascicle.UniqueId);
                                        }
                                        break;
                                    }
                                    case "Procedure": {
                                        if (categoryFascicle.Environment == 0 &&
                                            (!node.get_attributes().getAttribute("Type") || node.get_attributes().getAttribute("Type") != "Periodic")) {
                                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_procedure.png");
                                            node.get_attributes().setAttribute("Type", "Procedure");
                                            node.set_toolTip("Procedimento");
                                            node.set_value(categoryFascicle.UniqueId);
                                        }
                                        break;
                                    }
                                    case "SubFascicle": {
                                        if (categoryFascicle.Environment == 0 &&
                                            (!node.get_attributes().getAttribute("Type") || node.get_attributes().getAttribute("Type") != "Periodic")) {
                                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_procedure.png");
                                            node.get_attributes().setAttribute("Type", "SubFascicle");
                                            node.set_toolTip("SottoFascicolo");
                                            node.set_value(categoryFascicle.UniqueId);
                                        }
                                    }
                                }
                            });
                        }
                        else {
                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_hidden.png");
                            node.get_attributes().setAttribute("Type", "ToManage");
                            node.set_toolTip("Da gestire");
                        }
                    });
                    this._rtvEnvironments.commitChanges();
                    this._loadingPanel.hide(this.pnlFasciclePlanId);
                }
            },
            (exception: ExceptionDTO) => {
                let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            });
    }

    /**
     * Funzione che carica i nodi
     */
    loadNodes(idCategory: string) {



        //recupera fasci category
        this.currentCategoryId = idCategory;
        this.pnlFasciclePlanId
        this._loadingPanel.show(this.pnlFasciclePlanId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvEnvironments.get_selectedNode();
        let rootNode: Telerik.Web.UI.RadTreeNode = this._rtvEnvironments.get_nodes().getNode(0);
        let newNode: Telerik.Web.UI.RadTreeNode;

        rootNode = this._rtvEnvironments.get_nodes().getNode(0);
        rootNode.get_attributes().setAttribute("IdCategory", idCategory);

        rootNode = this._rtvEnvironments.get_nodes().getNode(0);
        rootNode.set_expanded(true);
        rootNode.get_nodes().clear();

        this._categoryFascicleService.getByIdCategory(idCategory,
            (data: CategoryFascicleModel[]) => {
                let mapper = new CategoryFascicleViewModelMapper();
                let categoryFascicles: CategoryFascicleViewModel[] = mapper.MapCollection(data);
                $.each(categoryFascicles, (index: number, categoryFascicle: CategoryFascicleViewModel) => {
                    switch (categoryFascicle.Environment) {
                        case DSWEnvironment.Protocol: {
                            newNode = new Telerik.Web.UI.RadTreeNode();
                            newNode.set_text("Protocollo".concat(" (", categoryFascicle.PeriodName, ")"));
                            newNode.get_attributes().setAttribute("Environment", DSWEnvironment.Protocol);
                            newNode.set_value(categoryFascicle.UniqueId);
                            rootNode.get_nodes().add(this.setNodePeriodic(newNode));
                            break;
                        }
                        case DSWEnvironment.Resolution: {
                            newNode = new Telerik.Web.UI.RadTreeNode();
                            newNode.set_text(this.currentResolutionName.concat(" (", categoryFascicle.PeriodName, ")"));
                            newNode.get_attributes().setAttribute("Environment", DSWEnvironment.Resolution);
                            newNode.set_value(categoryFascicle.UniqueId);

                            rootNode.get_nodes().add(this.setNodePeriodic(newNode));
                            break;
                        }
                        case DSWEnvironment.DocumentSeries: {
                            newNode = new Telerik.Web.UI.RadTreeNode();
                            newNode.set_text(this.currentDocumentSeriesName.concat(" (", categoryFascicle.PeriodName, ")"));
                            newNode.get_attributes().setAttribute("Environment", DSWEnvironment.DocumentSeries);
                            newNode.set_value(categoryFascicle.UniqueId);
                            rootNode.get_nodes().add(this.setNodePeriodic(newNode));
                            break;
                        }
                        default: {
                            if (categoryFascicle.Environment >= 100) {
                                this._udsRepositoryService.getUDSRepositoryByDSWEnvironment(categoryFascicle.Environment.toString(),
                                    (dataRep: any) => {
                                        if (dataRep) {
                                            newNode = new Telerik.Web.UI.RadTreeNode();
                                            newNode.set_text(dataRep.Name.concat(" (", categoryFascicle.PeriodName, ")"));
                                            newNode.get_attributes().setAttribute("Environment", dataRep.DSWEnvironment);
                                            newNode.set_value(categoryFascicle.UniqueId);
                                            rootNode.get_nodes().add(this.setNodePeriodic(newNode))
                                        }
                                    },
                                    (exception: ExceptionDTO) => {
                                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                                        if (!jQuery.isEmptyObject(uscNotification)) {
                                            uscNotification.showNotification(exception);
                                        }
                                    });
                            }  
                            break;
                        }
                    }

                });

            });

        this._rtvEnvironments.commitChanges();
        this._loadingPanel.hide(this.pnlFasciclePlanId);
        this.bindLoaded();
    }

    /**
    * Scateno l'evento di "Load Completed" del controllo
    */
    bindLoaded(): void {
        $("#".concat(this.pnlFasciclePlanId)).data(this);
    }

    /**
     * Imposto come nodo selezionato quello root e resetto la visibilità dei bottoni
     */
    setRootNode() {
        let rootNode: Telerik.Web.UI.RadTreeNode;
        rootNode = this._rtvEnvironments.get_nodes().getNode(0);
        rootNode.set_selected(true);

        this._btnAddPeriodicPlan.set_enabled(true);
        this._btnRemovePeriodicPlan.set_enabled(false);
    }

    /**
     * Apre una nuova nuova RadWindow
     * @param url
     * @param name
     * @param width
     * @param height
     */
    openWindow(url, name, width, height): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.managerWindowId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }

    /**
     * metodo che setta il nodo root
     * @param node
     */
    setNodeDefault(node: Telerik.Web.UI.RadTreeNode): Telerik.Web.UI.RadTreeNode {
        node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_hidden.png");
        node.get_attributes().setAttribute("Type", "ToManage");
        node.set_toolTip("Da gestire");
        return node;
    }

    setNodePeriodic(node: Telerik.Web.UI.RadTreeNode): Telerik.Web.UI.RadTreeNode {
        node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/history.png");
        node.get_attributes().setAttribute("Type", "Periodic");
        node.set_toolTip("Periodico");
        return node;
    }

    /**
     * Nascondo il loading
     */
    hideLoadingPanel() {
        this._loadingPanel.hide(this.pnlFasciclePlanId);
    }
}
export = uscFasciclePlan;