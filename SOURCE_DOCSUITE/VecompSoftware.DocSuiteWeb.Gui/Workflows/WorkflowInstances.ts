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
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');

class WorkflowInstances extends WorkflowInstancesBase {

    public workflowInstancesGridId: string;
    public ajaxLoadingPanelId: string;
    public btnSearchId: string;
    public btnCleanId: string;
    public dtpWorkflowRepositoryActiveFromId: string;
    public dtpWorkflowRepositoryActiveToId: string;
    public cmbWorkflowRepositoryStatusId: string;
    public rwWorkflowActivitiesId: string;
    public rwWorkflowInstanceLogsId: string;
    public workflowActivityGridId: string;
    public workflowInstanceLogsGridId: string;
    public workflowRepositoryName: string;
    public rcbWorkflowRepositoriesId: string;
    public maxNumberElements: string;

    private _workflowInstancesGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _btnClean: Telerik.Web.UI.RadButton;
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
    private _rcbWorkflowRepositories: Telerik.Web.UI.RadComboBox;

    private _enumHelper: EnumHelper;
    private _gridResult: WorkflowInstanceModel[];
    private _workflowActivityGridResult: WorkflowActivityModel[];

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(serviceConfigurations);
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize() {
        super.initialize();

        this._registerPageElements();

        this._initializeWorkflowRepositoriesCombobox();
        this._loadWorkflowStatus();
        this._btnWorkflowActivities = [];
        this._btnWorkflowInstanceLogs = [];
    }

    public onPageChanged() {
        let currentPageIdx: number = this._masterTableView.get_currentPageIndex();
        let currentPageSize: number = this._masterTableView.get_pageSize();
        let skip = currentPageIdx * currentPageSize;

        this._loadWorkflowInstances(skip);
    }

    public onGridDataBound() {
        let row = this._masterTableView.get_dataItems();
        for (let i = 0; i < row.length; i++) {
            if (i % 2) {
                row[i].addCssClass("Chiaro");
            } else {
                row[i].addCssClass("Scuro");
            }
        }
    }

    private _registerPageElements(): void {
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._workflowInstancesGrid = <Telerik.Web.UI.RadGrid>$find(this.workflowInstancesGridId);
        this._masterTableView = this._workflowInstancesGrid.get_masterTableView();
        this._workflowInstancesGrid.add_rowDataBound(this._workflowInstancesGrid_rowDataBound);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicking(this._btnSearch_onClick);
        this._btnClean = <Telerik.Web.UI.RadButton>$find(this.btnCleanId);
        this._btnClean.add_clicking(this._btnClean_onClick);
        this._dtpWorkflowRepositoryActiveFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dtpWorkflowRepositoryActiveFromId);
        this._dtpWorkflowRepositoryActiveTo = <Telerik.Web.UI.RadDatePicker>$find(this.dtpWorkflowRepositoryActiveToId);
        this._cmbWorkflowRepositoryStatus = <Telerik.Web.UI.RadComboBox>$find(this.cmbWorkflowRepositoryStatusId);
        this._rwWorkflowActivities = <Telerik.Web.UI.RadWindow>$find(this.rwWorkflowActivitiesId);
        this._rwWorkflowInstanceLogs = <Telerik.Web.UI.RadWindow>$find(this.rwWorkflowInstanceLogsId);
        this._workflowActivityGrid = <Telerik.Web.UI.RadGrid>$find(this.workflowActivityGridId);
        this._workflowInstanceLogsGrid = <Telerik.Web.UI.RadGrid>$find(this.workflowInstanceLogsGridId);
        this._rcbWorkflowRepositories = <Telerik.Web.UI.RadComboBox>$find(this.rcbWorkflowRepositoriesId);
        this._rcbWorkflowRepositories.add_itemsRequested(this._searchWorkflowRepositories);

        this._workflowActivity_masterTableView = this._workflowActivityGrid.get_masterTableView();
        this._workflowInstanceLogs_masterTableView = this._workflowInstanceLogsGrid.get_masterTableView();

    }

    private _initializeWorkflowRepositoriesCombobox(): void {
        this._rcbWorkflowRepositories.clearItems();
        this._loadWorkflowRepositories(this.workflowRepositoryName, 0, false, false)
            .done(() => {
                if (this.workflowRepositoryName) {
                    let item: Telerik.Web.UI.RadComboBoxItem = this._rcbWorkflowRepositories.findItemByText(this.workflowRepositoryName);
                    if (item) {
                        item.select();
                        this._rcbWorkflowRepositories.disable();
                    }
                }
                this._initializeWorkflowInstancesGrid();
            });
    }

    private _populateWorkflowRepositoriesCombobox(workflowRepositories: WorkflowRepositoryModel[]): void {
        if (!workflowRepositories.length) {
            return;
        }

        this._rcbWorkflowRepositories.beginUpdate();

        if (this._rcbWorkflowRepositories.get_items().get_count() === 0) {
            this._rcbWorkflowRepositories.get_items().add(this._getEmptyComboboxItem());
        }

        workflowRepositories.forEach(model => {
            let rdlItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
            rdlItem.set_text(model.Name);
            rdlItem.set_value(model.UniqueId);

            this._rcbWorkflowRepositories.get_items().add(rdlItem);
        });
        this._rcbWorkflowRepositories.endUpdate();
    }

    private _updateWorkflowRepositoriesCbxState(hasDomEvent: boolean, currentItemsCount: number): void {
        let currentElementsCount: number = this._rcbWorkflowRepositories.get_items().get_count();

        if (hasDomEvent && currentElementsCount > 0) {
            let scrollBarElement: JQuery = $(this._rcbWorkflowRepositories.get_dropDownElement()).find('div.rcbScroll');
            let totalCountElementPosition: number = $(this._rcbWorkflowRepositories.get_items().getItem(currentItemsCount + 1).get_element()).position().top;
            scrollBarElement.scrollTop(totalCountElementPosition);
        }

        this._rcbWorkflowRepositories.get_attributes().setAttribute("otherContainerCount", currentItemsCount);
        this._rcbWorkflowRepositories.get_attributes().setAttribute('updating', 'false');
    }

    private _getEmptyComboboxItem(): Telerik.Web.UI.RadComboBoxItem {
        let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
        emptyItem.set_text("");
        emptyItem.set_value("");

        return emptyItem;
    }

    private _searchWorkflowRepositories = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxRequestEventArgs): void => {
        let searchValue: string = args.get_text();
        let currentComboboxItemsCount: number = sender.get_items().get_count();
        let workflowRepositoryNumberOfItems: number = currentComboboxItemsCount === 0 ? currentComboboxItemsCount : currentComboboxItemsCount - 1;

        this._loadWorkflowRepositories(searchValue, workflowRepositoryNumberOfItems, true, args.get_domEvent() === undefined);
    }

    private _loadWorkflowRepositories(searchValue: string, workflowRepositoryNumberOfItems: number, expandAfterPopulate: boolean = false, hasActiveDomEvent: boolean = false): JQueryPromise<void> {
        let deffered: JQueryDeferred<void> = $.Deferred<void>();
        this.workflowRepositoryService.getAllWorkflowRepositories(searchValue, this.maxNumberElements, workflowRepositoryNumberOfItems,
            (data: ODATAResponseModel<WorkflowRepositoryModel>) => {
                let workflowRepositories: WorkflowRepositoryModel[] = data.value;
                this._populateWorkflowRepositoriesCombobox(workflowRepositories);
                this._updateWorkflowRepositoriesCbxState(hasActiveDomEvent, workflowRepositoryNumberOfItems);

                if (this._rcbWorkflowRepositories.get_items().get_count() > 0) {
                    workflowRepositoryNumberOfItems = this._rcbWorkflowRepositories.get_items().get_count() - 1;
                }

                this._rcbWorkflowRepositories.get_moreResultsBoxMessageElement().innerText = `Visualizzati ${workflowRepositoryNumberOfItems} di ${data.count}`;

                if (expandAfterPopulate && data.value.length) {
                    this._rcbWorkflowRepositories.showDropDown();
                }

                deffered.resolve();
            },
            (exception: ExceptionDTO) => {
                console.error(exception);
                deffered.reject();
            });

        return deffered.promise();
    }

    private _loadWorkflowStatus() {
        this._loadingPanel.show(this.cmbWorkflowRepositoryStatusId);

        let defaultValue: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
        defaultValue.set_text("");
        defaultValue.set_value(null);
        this._cmbWorkflowRepositoryStatus.get_items().add(defaultValue);

        for (var n in WorkflowStatus) {
            if (typeof WorkflowStatus[n] === 'string' && WorkflowStatus[n] !== "None" && n !== WorkflowStatus.LogicalDelete.toString()) {
                let cmbItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                cmbItem.set_text(this._enumHelper.getWorkflowStatusDescription(WorkflowStatus[n]));
                cmbItem.set_value(<any>WorkflowStatus[n]);
                this._cmbWorkflowRepositoryStatus.get_items().add(cmbItem);
            }
        }
        this._loadingPanel.hide(this.cmbWorkflowRepositoryStatusId);
    }

    private _btnSearch_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._loadWorkflowInstances();
    }

    private _btnClean_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._cleanSearchFilters();
    }

    private _cleanSearchFilters = () => {
        this._dtpWorkflowRepositoryActiveFrom.clear();
        this._dtpWorkflowRepositoryActiveTo.clear();
        this._cmbWorkflowRepositoryStatus.get_items().getItem(0).select();
    }

    private _workflowInstancesGrid_rowDataBound = (sender: any, args: Telerik.Web.UI.GridRowDataBoundEventArgs) => {
        this._btnWorkflowActivities = [];
        let dataItem: WorkflowInstanceModel = args.get_item().get_dataItem();
        let btnWorkflowActivities = args.get_item().findElement("btnWorkflowActivities");
        if (btnWorkflowActivities) {
            btnWorkflowActivities.innerText = `${dataItem.WorkflowActivitiesDoneCount}/${dataItem.WorkflowActivitiesCount}`;
            this._btnWorkflowActivities.push(<Telerik.Web.UI.RadButton>$find(btnWorkflowActivities.id));
            this._btnWorkflowActivities[this._btnWorkflowActivities.length - 1].add_clicking(
                () => this._btnWorkflowActivities_onClick(dataItem.WorkflowActivities)
            );
        }
        let btnWorkflowInstanceLogs = args.get_item().findElement("btnWorkflowInstanceLogs");
        let gridBtn: Telerik.Web.UI.RadButton = <Telerik.Web.UI.RadButton>$find(btnWorkflowInstanceLogs.id);

        if (dataItem.HasActivitiesInError) {
            gridBtn.set_text(`${dataItem.HasActivitiesInErrorLabel}`);
            this._btnWorkflowInstanceLogs.push(gridBtn);
            this._btnWorkflowInstanceLogs[this._btnWorkflowInstanceLogs.length - 1].add_clicking(
                () => this._btnWorkflowInstanceLogs_onClick(dataItem.UniqueId)
            );
        }
    }

    private _btnWorkflowActivities_onClick = (args: WorkflowActivityModel[]) => {
        this._rwWorkflowActivities.show();
        this._loadingPanel.hide(this.workflowActivityGridId);
        this._loadWorkflowActivities(args);
    }

    private _loadWorkflowActivities(workflowActivities: WorkflowActivityModel[]) {
        if (!workflowActivities) return;
        this._workflowActivityGridResult = workflowActivities;
        for (let i = 0; i < this._workflowActivityGridResult.length; i++) {
            this._workflowActivityGridResult[i].StatusDescription = this._enumHelper.getWorkflowStatusDescription(this._workflowActivityGridResult[i].Status.toString());
            this._workflowActivityGridResult[i].ActivityTypeDescription = this._enumHelper.getActivityTypeDescription(this._workflowActivityGridResult[i].ActivityType.toString());
            this._workflowActivityGridResult[i].RegistrationDateFormatted = moment(this._workflowActivityGridResult[i].RegistrationDate).format("DD/MM/YYYY");
        }
        this._workflowActivity_masterTableView.set_dataSource(this._workflowActivityGridResult);
        this._workflowActivity_masterTableView.dataBind();
        this._loadingPanel.hide(this.workflowActivityGridId);
    }

    private _btnWorkflowInstanceLogs_onClick = (args: string) => {
        this._rwWorkflowInstanceLogs.show();
        this._loadingPanel.hide(this.workflowInstanceLogsGridId);
        this._loadWorkflowInstanceLogs(args);
    }

    private _loadWorkflowInstanceLogs(uniqueId: string) {
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

    private _initializeWorkflowInstancesGrid() {
        if (!jQuery.isEmptyObject(this._workflowInstancesGrid)) {
            this._loadWorkflowInstances();
        }
    }

    private _loadWorkflowInstances(skip: number = 0) {
        let workflowRepositoryActiveFromFilter: string = "";
        if (this._dtpWorkflowRepositoryActiveFrom && this._dtpWorkflowRepositoryActiveFrom.get_selectedDate()) {
            workflowRepositoryActiveFromFilter = moment(this._dtpWorkflowRepositoryActiveFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
        }
        let workflowRepositoryActiveToFilter: string = "";
        if (this._dtpWorkflowRepositoryActiveTo && this._dtpWorkflowRepositoryActiveTo.get_selectedDate()) {
            workflowRepositoryActiveToFilter = moment(this._dtpWorkflowRepositoryActiveTo.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
        }

        if (workflowRepositoryActiveFromFilter && workflowRepositoryActiveToFilter) {
            let startDate: Date = new Date(workflowRepositoryActiveFromFilter);
            let endDate: Date = new Date(workflowRepositoryActiveToFilter);

            if (startDate > endDate) {
                return;
            }
        }

        let selectedWorkflowRepositoryStatusFilter: string = "";
        if (this._cmbWorkflowRepositoryStatus && this._cmbWorkflowRepositoryStatus.get_selectedItem() !== null) {
            selectedWorkflowRepositoryStatusFilter = this._cmbWorkflowRepositoryStatus.get_selectedItem().get_value();
        }

        let selectedWorkflowRepository: Telerik.Web.UI.RadComboBoxItem = this._rcbWorkflowRepositories.get_selectedItem();
        let searchDTO: WorkflowInstanceSearchFilterDTO = new WorkflowInstanceSearchFilterDTO();
        searchDTO.activeFrom = workflowRepositoryActiveFromFilter;
        searchDTO.activeTo = workflowRepositoryActiveToFilter;
        searchDTO.workflowRepositoryId = selectedWorkflowRepository ? selectedWorkflowRepository.get_value() : "";
        searchDTO.status = selectedWorkflowRepositoryStatusFilter;
        searchDTO.skip = skip;
        searchDTO.top = this._masterTableView.get_pageSize();

        this._searchWorkflowInstances(searchDTO);
    }

    private _searchWorkflowInstances(searchDTO: WorkflowInstanceSearchFilterDTO): void {
        this._loadingPanel.show(this.workflowInstancesGridId);
        this.workflowInstanceService.getWorkflowInstances(searchDTO,
            (data) => {
                if (!data) return;
                this._gridResult = data;
                for (let i = 0; i < this._gridResult.length; i++) {
                    if (this._gridResult[i].Status !== "None") {
                        this._gridResult[i].Status = this._enumHelper.getWorkflowStatusDescription(this._gridResult[i].Status);
                    }
                }

                this._masterTableView.set_dataSource(this._gridResult);
                if (!this._gridResult.length) {
                    this._setGridTotalElementsCount(0);
                    this._loadingPanel.hide(this.workflowInstancesGridId);
                    return;
                }

                this.workflowInstanceService.countWorkflowInstances(searchDTO, (totalWorkflowInstances) => {
                    this._setGridTotalElementsCount(totalWorkflowInstances);
                    this._loadingPanel.hide(this.workflowInstancesGridId);
                }, (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.workflowInstancesGridId);
                    $("#".concat(this.workflowInstancesGridId)).hide();
                });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.workflowInstancesGridId);
                $("#".concat(this.workflowInstancesGridId)).hide();
            });
    }

    private _setGridTotalElementsCount(count: number): void {
        this._masterTableView.set_virtualItemCount(count);
        this._masterTableView.dataBind();
    }
}

export = WorkflowInstances;
