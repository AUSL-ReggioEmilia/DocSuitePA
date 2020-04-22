/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "Workflows/WorkflowInstancesBase", "App/Helpers/EnumHelper", "App/DTOs/WorkflowInstanceSearchFilterDTO", "App/Models/Workflows/WorkflowStatus"], function (require, exports, WorkflowInstancesBase, EnumHelper, WorkflowInstanceSearchFilterDTO, WorkflowStatus) {
    var WorkflowInstances = /** @class */ (function (_super) {
        __extends(WorkflowInstances, _super);
        function WorkflowInstances(serviceConfigurations) {
            var _this = _super.call(this, serviceConfigurations) || this;
            _this.btnSearch_onClick = function (sender, args) {
                _this.loadResults();
            };
            _this.btnClean_onClick = function (sender, args) {
                _this.cleanSearchFilters();
            };
            _this.cleanSearchFilters = function () {
                _this._dtpWorkflowRepositoryActiveFrom.clear();
                _this._dtpWorkflowRepositoryActiveTo.clear();
            };
            _this.workflowInstancesGrid_rowDataBound = function (sender, args) {
                var dataItem = args.get_item().get_dataItem();
                var btnWorkflowActivities = args.get_item().findElement("btnWorkflowActivities");
                var btnWorkflowInstanceLogs = args.get_item().findElement("btnWorkflowInstanceLogs");
                if (btnWorkflowActivities) {
                    btnWorkflowActivities.innerText = dataItem.WorkflowActivitiesDoneCount + "/" + dataItem.WorkflowActivitiesCount;
                    _this._btnWorkflowActivities.push($find(btnWorkflowActivities.id));
                    _this._btnWorkflowActivities[_this._btnWorkflowActivities.length - 1].add_clicking(function () { return _this.btnWorkflowActivities_onClick(dataItem.WorkflowActivities); });
                }
                if (btnWorkflowInstanceLogs && dataItem.HasActivitiesInError === true) {
                    btnWorkflowInstanceLogs.innerText = "" + dataItem.HasActivitiesInErrorLabel;
                    _this._btnWorkflowInstanceLogs.push($find(btnWorkflowInstanceLogs.id));
                    _this._btnWorkflowInstanceLogs[_this._btnWorkflowInstanceLogs.length - 1].add_clicking(function () { return _this._btnWorkflowInstanceLogs_onClick(dataItem.UniqueId); });
                }
            };
            _this.btnWorkflowActivities_onClick = function (args) {
                _this._rwWorkflowActivities.show();
                _this._loadingPanel.hide(_this.workflowActivityGridId);
                _this.loadWorkflowActivities(args);
            };
            _this._btnWorkflowInstanceLogs_onClick = function (args) {
                _this._rwWorkflowInstanceLogs.show();
                _this._loadingPanel.hide(_this.workflowInstanceLogsGridId);
                _this.loadWorkflowInstanceLogs(args);
            };
            _this._serviceConfigurations = serviceConfigurations;
            _this._enumHelper = new EnumHelper();
            $(document).ready(function () {
            });
            return _this;
        }
        WorkflowInstances.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._workflowInstancesGrid = $find(this.workflowInstancesGridId);
            this._masterTableView = this._workflowInstancesGrid.get_masterTableView();
            this._workflowInstancesGrid.add_rowDataBound(this.workflowInstancesGrid_rowDataBound);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicking(this.btnSearch_onClick);
            this._btnClean = $find(this.btnCleanId);
            this._btnClean.add_clicking(this.btnClean_onClick);
            this._txtWorkflowRepositoryName = $find(this.txtWorkflowRepositoryNameId);
            this._dtpWorkflowRepositoryActiveFrom = $find(this.dtpWorkflowRepositoryActiveFromId);
            this._dtpWorkflowRepositoryActiveTo = $find(this.dtpWorkflowRepositoryActiveToId);
            this._cmbWorkflowRepositoryStatus = $find(this.cmbWorkflowRepositoryStatusId);
            this._rwWorkflowActivities = $find(this.rwWorkflowActivitiesId);
            this._rwWorkflowInstanceLogs = $find(this.rwWorkflowInstanceLogsId);
            this._workflowActivityGrid = $find(this.workflowActivityGridId);
            this._workflowInstanceLogsGrid = $find(this.workflowInstanceLogsGridId);
            this._workflowActivity_masterTableView = this._workflowActivityGrid.get_masterTableView();
            this._workflowInstanceLogs_masterTableView = this._workflowInstanceLogsGrid.get_masterTableView();
            this.loadWorkflowStatus();
            this.loadWorkflowInstancesGrid();
            this._btnWorkflowActivities = [];
            this._btnWorkflowInstanceLogs = [];
        };
        WorkflowInstances.prototype.loadWorkflowStatus = function () {
            this._loadingPanel.show(this.cmbWorkflowRepositoryStatusId);
            for (var n in WorkflowStatus) {
                if (typeof WorkflowStatus[n] === 'string' && WorkflowStatus[n] !== "None") {
                    var cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(this._enumHelper.getWorkflowStatusDescription(WorkflowStatus[n]));
                    cmbItem.set_value(WorkflowStatus[n]);
                    this._cmbWorkflowRepositoryStatus.get_items().add(cmbItem);
                }
            }
            this._loadingPanel.hide(this.cmbWorkflowRepositoryStatusId);
        };
        WorkflowInstances.prototype.loadWorkflowInstancesGrid = function () {
            if (!jQuery.isEmptyObject(this._workflowInstancesGrid)) {
                this.loadResults();
            }
        };
        WorkflowInstances.prototype.loadResults = function () {
            var _this = this;
            if (this.workflowRepositoryName) {
                this._txtWorkflowRepositoryName.set_textBoxValue(this.workflowRepositoryName);
                this._txtWorkflowRepositoryName.disable();
            }
            if (this.workflowRepositoryStatus) {
                var item = this._cmbWorkflowRepositoryStatus.findItemByText(this._enumHelper.getWorkflowStatusDescription(this.workflowRepositoryStatus));
                if (item) {
                    item.select();
                    this._cmbWorkflowRepositoryStatus.disable();
                }
            }
            this._loadingPanel.show(this.workflowInstancesGridId);
            var workflowRepositoryActiveFromFilter = "";
            if (this._dtpWorkflowRepositoryActiveFrom && this._dtpWorkflowRepositoryActiveFrom.get_selectedDate()) {
                workflowRepositoryActiveFromFilter = moment(this._dtpWorkflowRepositoryActiveFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var workflowRepositoryActiveToFilter = "";
            if (this._dtpWorkflowRepositoryActiveTo && this._dtpWorkflowRepositoryActiveTo.get_selectedDate()) {
                workflowRepositoryActiveToFilter = moment(this._dtpWorkflowRepositoryActiveTo.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var selectedWorkflowRepositoryStatusFilter = "";
            if (this._cmbWorkflowRepositoryStatus && this._cmbWorkflowRepositoryStatus.get_selectedItem() !== null) {
                selectedWorkflowRepositoryStatusFilter = this._cmbWorkflowRepositoryStatus.get_selectedItem().get_value();
            }
            var workflowRepositoryNameFilter = "";
            if (this._txtWorkflowRepositoryName && this._txtWorkflowRepositoryName.get_textBoxValue() !== "") {
                workflowRepositoryNameFilter = this._txtWorkflowRepositoryName.get_textBoxValue();
            }
            var searchDTO = new WorkflowInstanceSearchFilterDTO();
            searchDTO.activeFrom = workflowRepositoryActiveFromFilter;
            searchDTO.activeTo = workflowRepositoryActiveToFilter;
            searchDTO.name = workflowRepositoryNameFilter;
            searchDTO.status = selectedWorkflowRepositoryStatusFilter;
            this.workflowInstanceService.getWorkflowInstances(searchDTO, function (data) {
                if (!data)
                    return;
                _this.gridResult = data;
                for (var i = 0; i < _this.gridResult.length; i++) {
                    if (_this.gridResult[i].Status !== "None") {
                        _this.gridResult[i].Status = _this._enumHelper.getWorkflowStatusDescription(_this.gridResult[i].Status);
                    }
                }
                _this._masterTableView.set_dataSource(_this.gridResult);
                _this._masterTableView.dataBind();
                _this._loadingPanel.hide(_this.workflowInstancesGridId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.workflowInstancesGridId);
                $("#".concat(_this.workflowInstancesGridId)).hide();
            });
        };
        WorkflowInstances.prototype.loadWorkflowActivities = function (workflowActivities) {
            if (!workflowActivities)
                return;
            this.workflowActivityGridResult = workflowActivities;
            for (var i = 0; i < this.workflowActivityGridResult.length; i++) {
                this.workflowActivityGridResult[i].StatusDescription = this._enumHelper.getWorkflowStatusDescription(this.workflowActivityGridResult[i].Status.toString());
                this.workflowActivityGridResult[i].ActivityTypeDescription = this._enumHelper.getActivityTypeDescription(this.workflowActivityGridResult[i].ActivityType.toString());
                this.workflowActivityGridResult[i].RegistrationDateFormatted = moment(this.workflowActivityGridResult[i].RegistrationDate).format("DD/MM/YYYY");
            }
            this._workflowActivity_masterTableView.set_dataSource(this.workflowActivityGridResult);
            this._workflowActivity_masterTableView.dataBind();
            this._loadingPanel.hide(this.workflowActivityGridId);
        };
        WorkflowInstances.prototype.loadWorkflowInstanceLogs = function (uniqueId) {
            var _this = this;
            this.workflowInstanceLogService.getWorkflowInstanceLogs(uniqueId, function (data) {
                if (!data)
                    return;
                _this._workflowInstanceLogs_masterTableView.set_dataSource(data.value);
                _this._workflowInstanceLogs_masterTableView.dataBind();
                _this._loadingPanel.hide(_this.workflowInstanceLogsGridId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.workflowInstanceLogsGridId);
                $("#".concat(_this.workflowInstanceLogsGridId)).hide();
            });
        };
        WorkflowInstances.prototype.onGridDataBound = function () {
            var row = this._masterTableView.get_dataItems();
            for (var i = 0; i < row.length; i++) {
                if (i % 2) {
                    row[i].addCssClass("Chiaro");
                }
                else {
                    row[i].addCssClass("Scuro");
                }
            }
        };
        return WorkflowInstances;
    }(WorkflowInstancesBase));
    return WorkflowInstances;
});
//# sourceMappingURL=WorkflowInstances.js.map