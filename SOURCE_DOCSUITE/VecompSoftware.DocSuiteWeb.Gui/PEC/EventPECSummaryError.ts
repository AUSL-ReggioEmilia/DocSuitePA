import EventPECSummaryErrorBase = require('PEC/EventPECSummaryErrorBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceBusTopicService = require('App/Services/ServiceBus/ServiceBusTopicService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ServiceBusTopicModel = require('App/Models/ServiceBus/ServiceBusTopicModel');
import ServiceBusTopicContentModel = require('App/Models/ServiceBus/ServiceBusTopicContentModel');
import PECMailSummaryErrorModel = require('App/Models/PECMails/PECMailSummaryErrorModel');

class EventPECSummaryError extends EventPECSummaryErrorBase {

    public static LOADED_EVENT: string = "onLoaded";
    public static PAGE_CHANGED_EVENT: string = "onPageChanged";

    pageId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    eventPECSummaryErrorGridId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _eventPECSummaryErrorGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;


    /**
    * Costruttore
    * @param serviceConfiguration
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, EventPECSummaryErrorBase.SERVICEBUSTOPIC_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

     /**
     *------------------------- Methods -----------------------------
     */

    /**
    * Metodo di inizializzazione
    */
    initialize() {
        super.initialize();

        //region [ Grid Configuration Initialization ]
        this._eventPECSummaryErrorGrid = <Telerik.Web.UI.RadGrid>$find(this.eventPECSummaryErrorGridId);
        this._masterTableView = this._eventPECSummaryErrorGrid.get_masterTableView();
        this._masterTableView.set_currentPageIndex(0);
        this.bindLoaded();
        //endregion

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        $("#".concat(this.eventPECSummaryErrorGridId)).bind(EventPECSummaryError.LOADED_EVENT, () => {
            this.loadPECSummaryErrorGrid();
        });

        $("#".concat(this.eventPECSummaryErrorGridId)).bind(EventPECSummaryError.PAGE_CHANGED_EVENT, (args) => {
            if (!jQuery.isEmptyObject(this._eventPECSummaryErrorGrid)) {
                this.pageChange();
            }
        });

        this.loadResults(ServiceBusTopicService.TOPIC_NAME_ENTITY_EVENT, ServiceBusTopicService.SUBSCRIPTION_NAME_ENTITY_EVENT_PECMAILERRORSUMMARY, 0);
    }
    
    //region [ Grid Configuration Methods ]
    onPageChanged() {
        let skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
        $("#".concat(this.pageId)).triggerHandler(EventPECSummaryError.PAGE_CHANGED_EVENT);
        this.loadResults(ServiceBusTopicService.TOPIC_NAME_ENTITY_EVENT, ServiceBusTopicService.SUBSCRIPTION_NAME_ENTITY_EVENT_PECMAILERRORSUMMARY, skip);
    }

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

    private bindLoaded(): void {
        $("#".concat(this.pageId)).data(this);
        $("#".concat(this.pageId)).triggerHandler(EventPECSummaryError.LOADED_EVENT);
    }

    setDataSource(results: PECMailSummaryErrorModel[]) {
        this._masterTableView.set_dataSource(results);
        this._masterTableView.dataBind();
    }

    setItemCount(count: number) {
        this._masterTableView.set_virtualItemCount(count);
        this._masterTableView.dataBind();
    }
    //endregion

    private loadPECSummaryErrorGrid() {
        if (!jQuery.isEmptyObject(this._eventPECSummaryErrorGrid)) {
            this.loadResults(ServiceBusTopicService.TOPIC_NAME_ENTITY_EVENT, ServiceBusTopicService.SUBSCRIPTION_NAME_ENTITY_EVENT_PECMAILERRORSUMMARY, 0);
        }
    }

    private pageChange() {
        this._loadingPanel.show(this.eventPECSummaryErrorGridId);
        let skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_currentPageIndex();
        this.loadResults(ServiceBusTopicService.TOPIC_NAME_ENTITY_EVENT, ServiceBusTopicService.SUBSCRIPTION_NAME_ENTITY_EVENT_PECMAILERRORSUMMARY, skip);
    }

    private loadResults(topicName: string, subscriptionName: string, skip: number) {
        this._loadingPanel.show(this.eventPECSummaryErrorGridId);
        this.service.getTopicMessages(topicName, subscriptionName,
            (data: any) => {
                if (!data) return;
                var result: ServiceBusTopicModel[] = data;
                var gridSummary: PECMailSummaryErrorModel[] = [];
                let pageSize = this._masterTableView.get_pageSize();
                this._masterTableView.set_virtualItemCount(result.length);
                let pageCount: number = skip + pageSize < result.length ? skip + pageSize : result.length;
                for (let i = skip; i < pageCount; i++) {
                    var content: ServiceBusTopicContentModel = JSON.parse(result[i].Content);
                    var summary: PECMailSummaryErrorModel = JSON.parse(result[i].Content).Contents.$values[0].ContentValue;
                    summary.ReceivedDate = new Date(summary.ReceivedDate).format("dd/MM/yyyy hh:mm");
                    gridSummary.push(summary);
                }
                this._masterTableView.set_dataSource(gridSummary);
                this._masterTableView.dataBind();
                for (let rowIndex = 0; rowIndex < this._masterTableView.get_dataItems().length; rowIndex++) {
                    this._masterTableView.getCellByColumnUniqueName(this._masterTableView.get_dataItems()[rowIndex], "Subject").innerHTML =
                        "<a runat=\"server\" href=\"../PEC/EventPECStreamError.aspx?CorrelatedId=".concat(gridSummary[rowIndex].CorrelatedId).concat("\">").concat(gridSummary[rowIndex].Subject).concat("</a>");
                }
                this._loadingPanel.hide(this.eventPECSummaryErrorGridId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.eventPECSummaryErrorGridId);
                $("#".concat(this.eventPECSummaryErrorGridId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }
}

export = EventPECSummaryError;