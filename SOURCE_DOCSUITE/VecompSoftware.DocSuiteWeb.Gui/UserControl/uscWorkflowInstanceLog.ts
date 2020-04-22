import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import WorkflowInstanceLogViewModel = require('App/ViewModels/Workflows/WorkflowInstanceLogViewModel');

class uscWorkflowInstanceLog {

    workflowInstanceLogGridId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    pageContentId: string;

    public static ON_PAGE_CHANGE: string = "onPageChange";

    private _serviceConfigurations: ServiceConfiguration[];
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _workflowInstanceLogGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     * evento scatenato al cambio di pagina
     */
    onPageChanged() {
        let skip: number = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
        $("#".concat(this.pageContentId)).triggerHandler(uscWorkflowInstanceLog.ON_PAGE_CHANGE, skip);
    }

    /**
     * evento scatenato al caricamento di dati nella griglia
     */
    onGridDataBound() {
        let row = this._masterTableView.get_dataItems();
        for (let i = 0; i < row.length; i++) {
            if (i % 2) {
                row[i].addCssClass("Chiaro");
            }
            else {
                row[i].addCssClass("Scuro");
            }
        }
    }

    /**
     * Inizializzazione
     */
    initialize() {

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._workflowInstanceLogGrid = <Telerik.Web.UI.RadGrid>$find(this.workflowInstanceLogGridId);
        this._masterTableView = this._workflowInstanceLogGrid.get_masterTableView();
        this._masterTableView.set_currentPageIndex(0);
        this._masterTableView.set_virtualItemCount(0);

        this.bindLoaded();
    }


    /**
     * Carico i dati nella griglia
     * @param responseModel
     */
    setGrid(responseModel: ODATAResponseModel<WorkflowInstanceLogViewModel>) {
        this._masterTableView.set_dataSource(responseModel.value);
        this._masterTableView.set_virtualItemCount(responseModel.count);
        this._masterTableView.dataBind();

        this.bindLoaded();
    }

    /**
    * Scateno l'evento di "Load Completed" del controllo
    */
    private bindLoaded(): void {
        $("#".concat(this.pageContentId)).data(this);
    }
}


export = uscWorkflowInstanceLog; 