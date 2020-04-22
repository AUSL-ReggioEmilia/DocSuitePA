import DossierBase = require('Dossiers/DossierBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import DossierLogService = require('App/Services/Dossiers/DossierLogService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DossierLogViewModel = require('App/ViewModels/Dossiers/DossierLogViewModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class DossierLog extends DossierBase {

    dossierId: string;
    dossierLogGridId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    dossierTitle: string;
    titleContainerId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _dossierLogService: DossierLogService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _dossierLogGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;
    private _titleContainer: JQuery;

    /**
    * Costruttore
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    onPageChanged() {
        this._loadingPanel.show(this.dossierLogGridId);
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
        let dossierLogConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERLOG_TYPE_NAME);
        this._dossierLogService = new DossierLogService(dossierLogConfiguration);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.dossierLogGridId);
        this._dossierLogGrid = <Telerik.Web.UI.RadGrid>$find(this.dossierLogGridId);
        this._masterTableView = this._dossierLogGrid.get_masterTableView();
        this._masterTableView.set_currentPageIndex(0);
        this._masterTableView.set_virtualItemCount(0);

        this._titleContainer = $("#".concat(this.titleContainerId));
        if (this._titleContainer) {
            let titleLabel: JQuery = this._titleContainer.children("span");
            titleLabel.html("Dossier - Log ".concat(this.dossierTitle));
        }
        this.getLogs(0);
    }

    getLogs(skip: number) {
        let top = this._masterTableView.get_pageSize();
        this._dossierLogService.getDossierLogs(this.dossierId, skip, top,
            (response: ODATAResponseModel<DossierLogViewModel>) => {
                if (!response) return;
                this._masterTableView.set_dataSource(response.value);
                this._masterTableView.set_virtualItemCount(response.count);
                this._masterTableView.dataBind();
                this._loadingPanel.hide(this.dossierLogGridId);
            },
            (exception) => {
                this.showNotificationException(this.uscNotificationId, exception);
                this._loadingPanel.hide(this.dossierLogGridId);
            })
    }

}

export = DossierLog;