import UDSRepositoryModel = require("../App/Models/UDS/UDSRepositoryModel");
import UserUDSBase = require("./UserUDSBase");
import ServiceConfiguration = require("../App/Services/ServiceConfiguration");
import ExceptionDTO = require("../App/DTOs/ExceptionDTO");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import UDSService = require('App/Services/UDS/UDSService');
import UDSArchiveSearchFilterDTO = require('App/DTOs/UDSArchiveSearchFilterDTO');
import UDSLogViewModel = require('App/ViewModels/UDS/UDSLogViewModel');

class UserUDS extends UserUDSBase {

    ajaxLoadingPanelId: string;
    tenantCompanyName: string;
    rcbRepositoryNameId: string;
    chkAuthorizedId: string;
    udsGridId: string;
    btnSearchId: string;
    currentUser: string;
    multiTenantEnabled: string;

    private _serviceConfiguration: ServiceConfiguration[];
    gridResult: any[];
    public static LOADED_EVENT: string = "onLoaded";
    public static PAGE_CHANGED_EVENT: string = "onPageChanged";

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rcbRepositoryName: Telerik.Web.UI.RadComboBox;
    private _chkAuthorized: HTMLInputElement;
    private _udsGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;
    private _btnSearch: Telerik.Web.UI.RadButton;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(serviceConfigurations);
        this._serviceConfiguration = serviceConfigurations;
        $(document).ready(() => { });
    }

    initialize() {
        super.initialize();

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rcbRepositoryName = <Telerik.Web.UI.RadComboBox>$find(this.rcbRepositoryNameId);
        this._chkAuthorized = <HTMLInputElement>document.getElementById(this.chkAuthorizedId);
        this._udsGrid = <Telerik.Web.UI.RadGrid>$find(this.udsGridId);
        this._masterTableView = this._udsGrid.get_masterTableView();
        if (this._masterTableView) {
            this._masterTableView.set_currentPageIndex(0);
            this._masterTableView.set_virtualItemCount(0);
        }
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        if (this._btnSearch) {
            this._btnSearch.add_clicked(this.btnSearch_onClick);
        }
        $("#".concat(this.udsGridId)).bind(UserUDS.LOADED_EVENT, () => {
            this.loadUDSGrid();
        });

        this.loadRepositories();
    }

    private loadUDSGrid() {
        if (!jQuery.isEmptyObject(this._udsGrid)) {
            this.loadResults(0);
        }
    }

    loadRepositories() {
        try {
            this._loadingPanel.show(this.rcbRepositoryNameId);
            this._rcbRepositoryName.clearSelection();
            if (this.multiTenantEnabled === "True") {
                this.udsRepositoryService.getTenantUDSRepositories(this.tenantCompanyName, "",
                    (data) => {
                        if (!data) return;
                        this._rcbRepositoryName.trackChanges();
                        this._rcbRepositoryName.get_items().clear();
                        let thisCmbRepositoryName = this._rcbRepositoryName;
                        var cmbItem: Telerik.Web.UI.RadComboBoxItem = null;
                        $.each(data, function (i, value: UDSRepositoryModel) {
                            cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                            cmbItem.set_text(value.Name);
                            cmbItem.set_value(value.UniqueId);
                            thisCmbRepositoryName.get_items().add(cmbItem);
                        });
                        this._rcbRepositoryName.commitChanges();
                        this._loadingPanel.hide(this.rcbRepositoryNameId);

                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.rcbRepositoryNameId);
                        $("#".concat(this.rcbRepositoryNameId)).hide();
                    });
            }
            else {
                this.udsRepositoryService.getUDSRepositories("",
                    (data) => {
                        if (!data) return;
                        this._rcbRepositoryName.trackChanges();
                        this._rcbRepositoryName.get_items().clear();
                        let thisCmbRepositoryName = this._rcbRepositoryName;
                        var cmbItem: Telerik.Web.UI.RadComboBoxItem = null;
                        $.each(data, function (i, value: UDSRepositoryModel) {
                            cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                            cmbItem.set_text(value.Name);
                            cmbItem.set_value(value.UniqueId);
                            thisCmbRepositoryName.get_items().add(cmbItem);
                        });
                        this._rcbRepositoryName.commitChanges();
                        this._loadingPanel.hide(this.rcbRepositoryNameId);

                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.rcbRepositoryNameId);
                        $("#".concat(this.rcbRepositoryNameId)).hide();
                    });
            }
        } catch (error) {
            console.log((<Error>error).stack);
        }
    }

    loadResults(skip: number) {
        let UDSArchive_TYPE_NAME = this._rcbRepositoryName.get_selectedItem().get_text();
        let udsService: UDSService;
        let UDSArchiveConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfiguration, UDSArchive_TYPE_NAME);
        udsService = new UDSService(UDSArchiveConfiguration);

        this._udsGrid = <Telerik.Web.UI.RadGrid>$find(this.udsGridId);
        this._masterTableView = this._udsGrid.get_masterTableView();

        this._loadingPanel.show(this.udsGridId);

        let rcbRepositoryName: string = "";
        if (this._rcbRepositoryName && this._rcbRepositoryName.get_selectedItem() !== null) {
            rcbRepositoryName = this._rcbRepositoryName.get_selectedItem().get_value();
        }

        let searchDTO: UDSArchiveSearchFilterDTO = new UDSArchiveSearchFilterDTO();

        searchDTO.startDateFromFilter = moment(new Date(new Date().setDate(new Date().getDate() - 30))).format("YYYY-MM-DDTHH:mm:ss[Z]");
        searchDTO.endDateFromFilter = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss[Z]");
        searchDTO.registrationUserFilter = this.currentUser;
        searchDTO.registrationUserFilterEnabled = this._chkAuthorized.checked;

        var sortExpressions = this._masterTableView.get_sortExpressions();
        var orderbyExpressions;
        orderbyExpressions = sortExpressions.toString();
        if (orderbyExpressions == "") {
            orderbyExpressions = "RegistrationDate asc";
        }
        let top = this._masterTableView.get_pageSize();
        try {
            udsService.getOnlyToReadUDSArchives(searchDTO, top, skip, "", (data) => {
                if (!data) return;
                this.gridResult = data;
                this._masterTableView.set_dataSource(data.value);
                this._masterTableView.set_virtualItemCount(data.count);
                this._masterTableView.dataBind();
                this._loadingPanel.hide(this.udsGridId);
            },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.udsGridId);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            this._loadingPanel.hide(this.udsGridId);
        }
    }

    onGridDataBound = (sender: Telerik.Web.UI.RadGrid, eventArgs: Telerik.Web.UI.GridDataBindingEventArgs) => {
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

    onGridCommand = (sender: Telerik.Web.UI.RadGrid, eventArgs: Telerik.Web.UI.GridCommandEventArgs) => {
        if (eventArgs.get_commandName() === "Page") {
            eventArgs.set_cancel(true);
            this.onPageChanged();
        }
        if (eventArgs.get_commandName() === "Sort") {
            eventArgs.set_cancel(true);
            this.loadResults(0);
        }
    }

    onPageChanged() {
        let skip: number = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
        this.loadResults(skip);
    }

    btnSearch_onClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        if (this._rcbRepositoryName.get_selectedItem()) {
            this.loadResults(0);
        }
        else {
            alert("Il filtro di archivio è obbligatorio");
        }
    }
}

export = UserUDS;