/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowInstancesBase = require("Workflows/WorkflowInstancesBase");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import WorkflowInstanceModel = require("App/Models/Workflows/WorkflowInstanceModel");
import EnumHelper = require("App/Helpers/EnumHelper");
import WorkflowInstanceSearchFilterDTO = require("App/DTOs/WorkflowInstanceSearchFilterDTO");
import WorkflowStatus = require("App/Models/Workflows/WorkflowStatus");
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import WorkflowInstanceLogViewModel = require('App/ViewModels/Workflows/WorkflowInstanceLogViewModel');

abstract class WorkflowInstances extends WorkflowInstancesBase {

    workflowInstancesGridId: string;
    ajaxLoadingPanelId: string;
    btnSearchId: string;
    btnCleanId: string;
    txtWorkflowRepositoryNameId: string;
    dtpWorkflowRepositoryActiveFromId: string;
    dtpWorkflowRepositoryActiveToId: string;
    cmbWorkflowRepositoryStatusId: string;
    rwWorkflowActivitiesId: string;
    rwWorkflowInstanceLogsId: string;
    workflowActivityGridId: string;
    workflowInstanceLogsGridId: string;
    workflowRepositoryName: string;
    workflowRepositoryStatus: string;

    private _workflowInstancesGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _btnClean: Telerik.Web.UI.RadButton;
    private _txtWorkflowRepositoryName: Telerik.Web.UI.RadTextBox;
    private _dtpWorkflowRepositoryActiveFrom: Telerik.Web.UI.RadDatePicker;
    private _dtpWorkflowRepositoryActiveTo: Telerik.Web.UI.RadDatePicker;
    private _cmbWorkflowRepositoryStatus: Telerik.Web.UI.RadComboBox;
    private _btnWorkflowActivities: Telerik.Web.UI.RadButton[];
    private _btnWorkflowInstanceLogs: Telerik.Web.UI.RadButton[];
    private _rwWorkflowActivities: Telerik.Web.UI.RadWindow;
    private _rwWorkflowInstanceLogs: Telerik.Web.UI.RadWindow;
    private _workflowActivityGrid: Telerik.Web.UI.RadGrid;
    private _workflowInstanceLogsGrid: Telerik.Web.UI.RadGrid;
    private _workflowActivity_masterTableView: Telerik.Web.UI.GridTableView;
    private _workflowInstanceLogs_masterTableView: Telerik.Web.UI.GridTableView;

    private _enumHelper: EnumHelper;
    gridResult: WorkflowInstanceModel[];
    workflowActivityGridResult: WorkflowActivityModel[];

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(serviceConfigurations);
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
        $(document).ready(() => {

        });
    }

    initialize() {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._workflowInstancesGrid = <Telerik.Web.UI.RadGrid>$find(this.workflowInstancesGridId);
        this._masterTableView = this._workflowInstancesGrid.get_masterTableView();
        this._workflowInstancesGrid.add_rowDataBound(this.workflowInstancesGrid_rowDataBound);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicking(this.btnSearch_onClick);
        this._btnClean = <Telerik.Web.UI.RadButton>$find(this.btnCleanId);
        this._btnClean.add_clicking(this.btnClean_onClick);
        this._txtWorkflowRepositoryName = <Telerik.Web.UI.RadTextBox>$find(this.txtWorkflowRepositoryNameId);
        this._dtpWorkflowRepositoryActiveFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dtpWorkflowRepositoryActiveFromId);
        this._dtpWorkflowRepositoryActiveTo = <Telerik.Web.UI.RadDatePicker>$find(this.dtpWorkflowRepositoryActiveToId);
        this._cmbWorkflowRepositoryStatus = <Telerik.Web.UI.RadComboBox>$find(this.cmbWorkflowRepositoryStatusId);
        this._rwWorkflowActivities = <Telerik.Web.UI.RadWindow>$find(this.rwWorkflowActivitiesId);
        this._rwWorkflowInstanceLogs = <Telerik.Web.UI.RadWindow>$find(this.rwWorkflowInstanceLogsId);
        this._workflowActivityGrid = <Telerik.Web.UI.RadGrid>$find(this.workflowActivityGridId);
        this._workflowInstanceLogsGrid = <Telerik.Web.UI.RadGrid>$find(this.workflowInstanceLogsGridId);
        
        this._workflowActivity_masterTableView = this._workflowActivityGrid.get_masterTableView();
        this._workflowInstanceLogs_masterTableView = this._workflowInstanceLogsGrid.get_masterTableView();

        this.loadWorkflowStatus();
        this.loadWorkflowInstancesGrid();
        this._btnWorkflowActivities = [];
        this._btnWorkflowInstanceLogs = [];
    }

    loadWorkflowStatus() {
        this._loadingPanel.show(this.cmbWorkflowRepositoryStatusId);
        for (var n in WorkflowStatus) {
            if (typeof WorkflowStatus[n] === 'string' && WorkflowStatus[n] !== "None") {
                let cmbItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                cmbItem.set_text(
                    this._enumHelper.getWorkflowStatusDescription(WorkflowStatus[n]));
                cmbItem.set_value(<any>WorkflowStatus[n]);
                this._cmbWorkflowRepositoryStatus.get_items().add(cmbItem);
            }
        }
        this._loadingPanel.hide(this.cmbWorkflowRepositoryStatusId);
    }

    private loadWorkflowInstancesGrid() {
        if (!jQuery.isEmptyObject(this._workflowInstancesGrid)) {
            this.loadResults();
        }
    }

    loadResults() {
        if (this.workflowRepositoryName) {
            this._txtWorkflowRepositoryName.set_textBoxValue(this.workflowRepositoryName);
            this._txtWorkflowRepositoryName.disable();
        }
        if (this.workflowRepositoryStatus) {
            let item: Telerik.Web.UI.RadComboBoxItem = this._cmbWorkflowRepositoryStatus.findItemByText(this._enumHelper.getWorkflowStatusDescription(this.workflowRepositoryStatus));
            if (item) {
                item.select();
                this._cmbWorkflowRepositoryStatus.disable();
            }
        }

        this._loadingPanel.show(this.workflowInstancesGridId);
        let workflowRepositoryActiveFromFilter: string = "";
        if (this._dtpWorkflowRepositoryActiveFrom && this._dtpWorkflowRepositoryActiveFrom.get_selectedDate()) {
            workflowRepositoryActiveFromFilter = moment(this._dtpWorkflowRepositoryActiveFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
        }
        let workflowRepositoryActiveToFilter: string = "";
        if (this._dtpWorkflowRepositoryActiveTo && this._dtpWorkflowRepositoryActiveTo.get_selectedDate()) {
            workflowRepositoryActiveToFilter = moment(this._dtpWorkflowRepositoryActiveTo.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
        }
        let selectedWorkflowRepositoryStatusFilter: string = "";
        if (this._cmbWorkflowRepositoryStatus && this._cmbWorkflowRepositoryStatus.get_selectedItem() !== null) {
            selectedWorkflowRepositoryStatusFilter = this._cmbWorkflowRepositoryStatus.get_selectedItem().get_value();
        }
        let workflowRepositoryNameFilter: string = "";
        if (this._txtWorkflowRepositoryName && this._txtWorkflowRepositoryName.get_textBoxValue() !== "") {
            workflowRepositoryNameFilter = this._txtWorkflowRepositoryName.get_textBoxValue();
        }

        let searchDTO: WorkflowInstanceSearchFilterDTO = new WorkflowInstanceSearchFilterDTO();
        searchDTO.activeFrom = workflowRepositoryActiveFromFilter;
        searchDTO.activeTo = workflowRepositoryActiveToFilter;
        searchDTO.name = workflowRepositoryNameFilter;
        searchDTO.status = selectedWorkflowRepositoryStatusFilter;

        this.workflowInstanceService.getWorkflowInstances(searchDTO,
            (data) => {
                if (!data) return;
                this.gridResult = data;
                for (let i = 0; i < this.gridResult.length; i++) {
                    if (this.gridResult[i].Status !== "None") {
                        this.gridResult[i].Status = this._enumHelper.getWorkflowStatusDescription(this.gridResult[i].Status);
                    }
                }
                this._masterTableView.set_dataSource(this.gridResult);
                this._masterTableView.dataBind();
                this._loadingPanel.hide(this.workflowInstancesGridId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.workflowInstancesGridId);
                $("#".concat(this.workflowInstancesGridId)).hide();
            });
    }

    btnSearch_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this.loadResults();
    }

    btnClean_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this.cleanSearchFilters();
    }

    cleanSearchFilters = () => {
        this._dtpWorkflowRepositoryActiveFrom.clear();
        this._dtpWorkflowRepositoryActiveTo.clear();
    }

    workflowInstancesGrid_rowDataBound = (sender: any, args: Telerik.Web.UI.GridRowDataBoundEventArgs) => {
        let dataItem: WorkflowInstanceModel = args.get_item().get_dataItem();
        let btnWorkflowActivities = args.get_item().findElement("btnWorkflowActivities");
        let btnWorkflowInstanceLogs = args.get_item().findElement("btnWorkflowInstanceLogs");
        if (btnWorkflowActivities) {
            btnWorkflowActivities.innerText = `${dataItem.WorkflowActivitiesDoneCount}/${dataItem.WorkflowActivitiesCount}`;
            this._btnWorkflowActivities.push(<Telerik.Web.UI.RadButton>$find(btnWorkflowActivities.id));
            this._btnWorkflowActivities[this._btnWorkflowActivities.length - 1].add_clicking(
                () => this.btnWorkflowActivities_onClick(dataItem.WorkflowActivities)
            );
        }
        if (btnWorkflowInstanceLogs && dataItem.HasActivitiesInError === true) {
            btnWorkflowInstanceLogs.innerText = `${dataItem.HasActivitiesInErrorLabel}`;
            this._btnWorkflowInstanceLogs.push(<Telerik.Web.UI.RadButton>$find(btnWorkflowInstanceLogs.id));
            this._btnWorkflowInstanceLogs[this._btnWorkflowInstanceLogs.length - 1].add_clicking(
                () => this._btnWorkflowInstanceLogs_onClick(dataItem.UniqueId)
            );
        }
    }

    btnWorkflowActivities_onClick = (args: WorkflowActivityModel[]) => {
        this._rwWorkflowActivities.show();
        this._loadingPanel.hide(this.workflowActivityGridId);
        this.loadWorkflowActivities(args);
    }

    loadWorkflowActivities(workflowActivities: WorkflowActivityModel[]) {
        if (!workflowActivities) return;
        this.workflowActivityGridResult = workflowActivities;
        for (let i = 0; i < this.workflowActivityGridResult.length; i++) {
            this.workflowActivityGridResult[i].StatusDescription = this._enumHelper.getWorkflowStatusDescription(this.workflowActivityGridResult[i].Status.toString());
            this.workflowActivityGridResult[i].ActivityTypeDescription = this._enumHelper.getActivityTypeDescription(this.workflowActivityGridResult[i].ActivityType.toString());
            this.workflowActivityGridResult[i].RegistrationDateFormatted = moment(this.workflowActivityGridResult[i].RegistrationDate).format("DD/MM/YYYY");
        }
        this._workflowActivity_masterTableView.set_dataSource(this.workflowActivityGridResult);
        this._workflowActivity_masterTableView.dataBind();
        this._loadingPanel.hide(this.workflowActivityGridId);
    }

    _btnWorkflowInstanceLogs_onClick = (args: string) => {
        this._rwWorkflowInstanceLogs.show();
        this._loadingPanel.hide(this.workflowInstanceLogsGridId);
        this.loadWorkflowInstanceLogs(args);
    }

    loadWorkflowInstanceLogs(uniqueId: string) {
        this.workflowInstanceLogService.getWorkflowInstanceLogs(uniqueId,
            (data: ODATAResponseModel<WorkflowInstanceLogViewModel>) => {
                if (!data) return;
                
                this._workflowInstanceLogs_masterTableView.set_dataSource(data.value);
                this._workflowInstanceLogs_masterTableView.dataBind();
                this._loadingPanel.hide(this.workflowInstanceLogsGridId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.workflowInstanceLogsGridId);
                $("#".concat(this.workflowInstanceLogsGridId)).hide();
            });
    }

    onGridDataBound() {
        let row = this._masterTableView.get_dataItems();
        for (let i = 0; i < row.length; i++) {
            if (i % 2) {
                row[i].addCssClass("Chiaro");
            } else {
                row[i].addCssClass("Scuro");
            }
        }
    }

}

export = WorkflowInstances;
