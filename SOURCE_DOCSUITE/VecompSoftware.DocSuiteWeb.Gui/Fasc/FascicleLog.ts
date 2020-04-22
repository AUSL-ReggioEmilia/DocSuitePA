import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import FascicleLogService = require('App/Services/Fascicles/FascicleLogService');
import FascicleLogViewModel = require('App/ViewModels/Fascicles/FascicleLogViewModel');

import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import FascBase = require('Fasc/FascBase');

class FascicleLog extends FascBase {

    fascicleId: string;
    fascicleLogGridId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    fascicleTitle: string;
    titleContainerId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _fascicleLogService: FascicleLogService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _fascicleLogGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;
    private _titleContainer: JQuery;

    /**
   * Costruttore
   */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    onPageChanged() {
        this._loadingPanel.show(this.fascicleLogGridId);
        let skip: number = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
        this.getLogs(skip);
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

    initialize() {
        super.initialize();
        let fascicleLogConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLE_LOG_TYPE_NAME);
        this._fascicleLogService = new FascicleLogService(fascicleLogConfiguration);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.fascicleLogGridId);
        this._fascicleLogGrid = <Telerik.Web.UI.RadGrid>$find(this.fascicleLogGridId);
        this._masterTableView = this._fascicleLogGrid.get_masterTableView();
        this._masterTableView.set_currentPageIndex(0);
        this._masterTableView.set_virtualItemCount(0);

        this._titleContainer = $("#".concat(this.titleContainerId));
        if (this._titleContainer) {
            let titleLabel: JQuery = this._titleContainer.children("span");
            titleLabel.html("Fascicle - Log ".concat(this.fascicleTitle));
        }
        this.getLogs(0);
    }

    getLogs(skip: number) {
        let top = this._masterTableView.get_pageSize();
        this._fascicleLogService.getFascicleLogs(this.fascicleId, skip, top,
            (response: ODATAResponseModel<FascicleLogViewModel>) => {
                if (!response) return;
                this._masterTableView.set_dataSource(response.value);
                this._masterTableView.set_virtualItemCount(response.count);
                this._masterTableView.dataBind();
                this._loadingPanel.hide(this.fascicleLogGridId);
            },
            (exception) => {
                this.showNotificationException(this.uscNotificationId, exception);
                this._loadingPanel.hide(this.fascicleLogGridId);
            })
    }
}

export = FascicleLog;