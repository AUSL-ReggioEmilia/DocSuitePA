import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UDSLogService = require('App/Services/UDS/UDSLogService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UDSLogViewModel = require('App/ViewModels/Dossiers/DossierLogViewModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');


class UDSLog {
    UDSId: string;
    UDSIdRepository: string;
    UDSLogGridId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    titleContainerId: string;
    HasAdminRight: boolean;
    UDSLogShowOnlyCurrentIfNotAdmin: boolean;

    private _serviceConfigurations: ServiceConfiguration[];
    private _UDSLogService: UDSLogService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _udsLogGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;
    private _titleContainer: JQuery;

    /**
* Costruttore
* @param serviceConfiguration
*/
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }


    onPageChanged() {
        this._loadingPanel.show(this.UDSLogGridId);
        let skip: number = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();

        if (!this.HasAdminRight && this.UDSLogShowOnlyCurrentIfNotAdmin) {
            this.getOnlyMyLogs(0);
        }
        else {
            this.getLogs(skip);
        }

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
        let UDSLogConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "UDSLog");
        this._UDSLogService = new UDSLogService(UDSLogConfiguration);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.UDSLogGridId);
        this._udsLogGrid = <Telerik.Web.UI.RadGrid>$find(this.UDSLogGridId);
        this._masterTableView = this._udsLogGrid.get_masterTableView();
        this._masterTableView.set_currentPageIndex(0);
        this._masterTableView.set_virtualItemCount(0);

        this._titleContainer = $("#".concat(this.titleContainerId));
        if (this._titleContainer) {
            let titleLabel: JQuery = this._titleContainer.children("span");
            titleLabel.html("Archivi - Log ");
        }


        if (!this.HasAdminRight && this.UDSLogShowOnlyCurrentIfNotAdmin) {
            this.getOnlyMyLogs(0);
        }
        else {
            this.getLogs(0);
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


    getLogs(skip: number) {
        let top = this._masterTableView.get_pageSize();
        this._UDSLogService.getUDSLogs(this.UDSId, skip, top,
            (response: ODATAResponseModel<UDSLogViewModel>) => {
                if (!response) return;
                this._masterTableView.set_dataSource(response.value);
                this._masterTableView.set_virtualItemCount(response.count);
                this._masterTableView.dataBind();
                this._loadingPanel.hide(this.UDSLogGridId);
            },
            (exception) => {
                this.showNotificationException(this.uscNotificationId, exception);
                this._loadingPanel.hide(this.UDSLogGridId);
            })
    }

    getOnlyMyLogs(skip: number) {
        let top = this._masterTableView.get_pageSize();
        this._UDSLogService.getMyUDSLog(this.UDSId, skip, top,
            (response: ODATAResponseModel<UDSLogViewModel>) => {
                if (!response) return;
                this._masterTableView.set_dataSource(response.value);
                this._masterTableView.set_virtualItemCount(response.count);
                this._masterTableView.dataBind();
                this._loadingPanel.hide(this.UDSLogGridId);
            },
            (exception) => {
                this.showNotificationException(this.uscNotificationId, exception);
                this._loadingPanel.hide(this.UDSLogGridId);
            })
    }

}

export = UDSLog;