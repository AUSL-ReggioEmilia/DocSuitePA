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
            _this._searchWorkflowRepositories = function (sender, args) {
                var searchValue = args.get_text();
                var currentComboboxItemsCount = sender.get_items().get_count();
                var workflowRepositoryNumberOfItems = currentComboboxItemsCount === 0 ? currentComboboxItemsCount : currentComboboxItemsCount - 1;
                _this._loadWorkflowRepositories(searchValue, workflowRepositoryNumberOfItems, true, args.get_domEvent() === undefined);
            };
            _this._btnSearch_onClick = function (sender, args) {
                _this._loadWorkflowInstances();
            };
            _this._btnClean_onClick = function (sender, args) {
                _this._cleanSearchFilters();
            };
            _this._cleanSearchFilters = function () {
                _this._dtpWorkflowRepositoryActiveFrom.clear();
                _this._dtpWorkflowRepositoryActiveTo.clear();
                _this._cmbWorkflowRepositoryStatus.get_items().getItem(0).select();
            };
            _this._workflowInstancesGrid_rowDataBound = function (sender, args) {
                _this._btnWorkflowActivities = [];
                var dataItem = args.get_item().get_dataItem();
                var btnWorkflowActivities = args.get_item().findElement("btnWorkflowActivities");
                if (btnWorkflowActivities) {
                    btnWorkflowActivities.innerText = dataItem.WorkflowActivitiesDoneCount + "/" + dataItem.WorkflowActivitiesCount;
                    _this._btnWorkflowActivities.push($find(btnWorkflowActivities.id));
                    _this._btnWorkflowActivities[_this._btnWorkflowActivities.length - 1].add_clicking(function () { return _this._btnWorkflowActivities_onClick(dataItem.WorkflowActivities); });
                }
                var btnWorkflowInstanceLogs = args.get_item().findElement("btnWorkflowInstanceLogs");
                var gridBtn = $find(btnWorkflowInstanceLogs.id);
                if (dataItem.HasActivitiesInError) {
                    gridBtn.set_text("" + dataItem.HasActivitiesInErrorLabel);
                    _this._btnWorkflowInstanceLogs.push(gridBtn);
                    _this._btnWorkflowInstanceLogs[_this._btnWorkflowInstanceLogs.length - 1].add_clicking(function () { return _this._btnWorkflowInstanceLogs_onClick(dataItem.UniqueId); });
                }
            };
            _this._btnWorkflowActivities_onClick = function (args) {
                _this._rwWorkflowActivities.show();
                _this._loadingPanel.hide(_this.workflowActivityGridId);
                _this._loadWorkflowActivities(args);
            };
            _this._btnWorkflowInstanceLogs_onClick = function (args) {
                _this._rwWorkflowInstanceLogs.show();
                _this._loadingPanel.hide(_this.workflowInstanceLogsGridId);
                _this._loadWorkflowInstanceLogs(args);
            };
            _this._serviceConfigurations = serviceConfigurations;
            _this._enumHelper = new EnumHelper();
            return _this;
        }
        WorkflowInstances.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._registerPageElements();
            this._initializeWorkflowRepositoriesCombobox();
            this._loadWorkflowStatus();
            this._btnWorkflowActivities = [];
            this._btnWorkflowInstanceLogs = [];
        };
        WorkflowInstances.prototype.onPageChanged = function () {
            var currentPageIdx = this._masterTableView.get_currentPageIndex();
            var currentPageSize = this._masterTableView.get_pageSize();
            var skip = currentPageIdx * currentPageSize;
            this._loadWorkflowInstances(skip);
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
        WorkflowInstances.prototype._registerPageElements = function () {
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._workflowInstancesGrid = $find(this.workflowInstancesGridId);
            this._masterTableView = this._workflowInstancesGrid.get_masterTableView();
            this._workflowInstancesGrid.add_rowDataBound(this._workflowInstancesGrid_rowDataBound);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicking(this._btnSearch_onClick);
            this._btnClean = $find(this.btnCleanId);
            this._btnClean.add_clicking(this._btnClean_onClick);
            this._dtpWorkflowRepositoryActiveFrom = $find(this.dtpWorkflowRepositoryActiveFromId);
            this._dtpWorkflowRepositoryActiveTo = $find(this.dtpWorkflowRepositoryActiveToId);
            this._cmbWorkflowRepositoryStatus = $find(this.cmbWorkflowRepositoryStatusId);
            this._rwWorkflowActivities = $find(this.rwWorkflowActivitiesId);
            this._rwWorkflowInstanceLogs = $find(this.rwWorkflowInstanceLogsId);
            this._workflowActivityGrid = $find(this.workflowActivityGridId);
            this._workflowInstanceLogsGrid = $find(this.workflowInstanceLogsGridId);
            this._rcbWorkflowRepositories = $find(this.rcbWorkflowRepositoriesId);
            this._rcbWorkflowRepositories.add_itemsRequested(this._searchWorkflowRepositories);
            this._workflowActivity_masterTableView = this._workflowActivityGrid.get_masterTableView();
            this._workflowInstanceLogs_masterTableView = this._workflowInstanceLogsGrid.get_masterTableView();
        };
        WorkflowInstances.prototype._initializeWorkflowRepositoriesCombobox = function () {
            var _this = this;
            this._rcbWorkflowRepositories.clearItems();
            this._loadWorkflowRepositories(this.workflowRepositoryName, 0, false, false)
                .done(function () {
                if (_this.workflowRepositoryName) {
                    var item = _this._rcbWorkflowRepositories.findItemByText(_this.workflowRepositoryName);
                    if (item) {
                        item.select();
                        _this._rcbWorkflowRepositories.disable();
                    }
                }
                _this._initializeWorkflowInstancesGrid();
            });
        };
        WorkflowInstances.prototype._populateWorkflowRepositoriesCombobox = function (workflowRepositories) {
            var _this = this;
            if (!workflowRepositories.length) {
                return;
            }
            this._rcbWorkflowRepositories.beginUpdate();
            if (this._rcbWorkflowRepositories.get_items().get_count() === 0) {
                this._rcbWorkflowRepositories.get_items().add(this._getEmptyComboboxItem());
            }
            workflowRepositories.forEach(function (model) {
                var rdlItem = new Telerik.Web.UI.RadComboBoxItem();
                rdlItem.set_text(model.Name);
                rdlItem.set_value(model.UniqueId);
                _this._rcbWorkflowRepositories.get_items().add(rdlItem);
            });
            this._rcbWorkflowRepositories.endUpdate();
        };
        WorkflowInstances.prototype._updateWorkflowRepositoriesCbxState = function (hasDomEvent, currentItemsCount) {
            var currentElementsCount = this._rcbWorkflowRepositories.get_items().get_count();
            if (hasDomEvent && currentElementsCount > 0) {
                var scrollBarElement = $(this._rcbWorkflowRepositories.get_dropDownElement()).find('div.rcbScroll');
                var totalCountElementPosition = $(this._rcbWorkflowRepositories.get_items().getItem(currentItemsCount + 1).get_element()).position().top;
                scrollBarElement.scrollTop(totalCountElementPosition);
            }
            this._rcbWorkflowRepositories.get_attributes().setAttribute("otherContainerCount", currentItemsCount);
            this._rcbWorkflowRepositories.get_attributes().setAttribute('updating', 'false');
        };
        WorkflowInstances.prototype._getEmptyComboboxItem = function () {
            var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
            emptyItem.set_text("");
            emptyItem.set_value("");
            return emptyItem;
        };
        WorkflowInstances.prototype._loadWorkflowRepositories = function (searchValue, workflowRepositoryNumberOfItems, expandAfterPopulate, hasActiveDomEvent) {
            var _this = this;
            if (expandAfterPopulate === void 0) { expandAfterPopulate = false; }
            if (hasActiveDomEvent === void 0) { hasActiveDomEvent = false; }
            var deffered = $.Deferred();
            this.workflowRepositoryService.getAllWorkflowRepositories(searchValue, this.maxNumberElements, workflowRepositoryNumberOfItems, function (data) {
                var workflowRepositories = data.value;
                _this._populateWorkflowRepositoriesCombobox(workflowRepositories);
                _this._updateWorkflowRepositoriesCbxState(hasActiveDomEvent, workflowRepositoryNumberOfItems);
                if (_this._rcbWorkflowRepositories.get_items().get_count() > 0) {
                    workflowRepositoryNumberOfItems = _this._rcbWorkflowRepositories.get_items().get_count() - 1;
                }
                _this._rcbWorkflowRepositories.get_moreResultsBoxMessageElement().innerText = "Visualizzati " + workflowRepositoryNumberOfItems + " di " + data.count;
                if (expandAfterPopulate && data.value.length) {
                    _this._rcbWorkflowRepositories.showDropDown();
                }
                deffered.resolve();
            }, function (exception) {
                console.error(exception);
                deffered.reject();
            });
            return deffered.promise();
        };
        WorkflowInstances.prototype._loadWorkflowStatus = function () {
            this._loadingPanel.show(this.cmbWorkflowRepositoryStatusId);
            var defaultValue = new Telerik.Web.UI.RadComboBoxItem();
            defaultValue.set_text("");
            defaultValue.set_value(null);
            this._cmbWorkflowRepositoryStatus.get_items().add(defaultValue);
            for (var n in WorkflowStatus) {
                if (typeof WorkflowStatus[n] === 'string' && WorkflowStatus[n] !== "None" && n !== WorkflowStatus.LogicalDelete.toString()) {
                    var cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(this._enumHelper.getWorkflowStatusDescription(WorkflowStatus[n]));
                    cmbItem.set_value(WorkflowStatus[n]);
                    this._cmbWorkflowRepositoryStatus.get_items().add(cmbItem);
                }
            }
            this._loadingPanel.hide(this.cmbWorkflowRepositoryStatusId);
        };
        WorkflowInstances.prototype._loadWorkflowActivities = function (workflowActivities) {
            if (!workflowActivities)
                return;
            this._workflowActivityGridResult = workflowActivities;
            for (var i = 0; i < this._workflowActivityGridResult.length; i++) {
                this._workflowActivityGridResult[i].StatusDescription = this._enumHelper.getWorkflowStatusDescription(this._workflowActivityGridResult[i].Status.toString());
                this._workflowActivityGridResult[i].ActivityTypeDescription = this._enumHelper.getActivityTypeDescription(this._workflowActivityGridResult[i].ActivityType.toString());
                this._workflowActivityGridResult[i].RegistrationDateFormatted = moment(this._workflowActivityGridResult[i].RegistrationDate).format("DD/MM/YYYY");
            }
            this._workflowActivity_masterTableView.set_dataSource(this._workflowActivityGridResult);
            this._workflowActivity_masterTableView.dataBind();
            this._loadingPanel.hide(this.workflowActivityGridId);
        };
        WorkflowInstances.prototype._loadWorkflowInstanceLogs = function (uniqueId) {
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
        WorkflowInstances.prototype._initializeWorkflowInstancesGrid = function () {
            if (!jQuery.isEmptyObject(this._workflowInstancesGrid)) {
                this._loadWorkflowInstances();
            }
        };
        WorkflowInstances.prototype._loadWorkflowInstances = function (skip) {
            if (skip === void 0) { skip = 0; }
            var workflowRepositoryActiveFromFilter = "";
            if (this._dtpWorkflowRepositoryActiveFrom && this._dtpWorkflowRepositoryActiveFrom.get_selectedDate()) {
                workflowRepositoryActiveFromFilter = moment(this._dtpWorkflowRepositoryActiveFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var workflowRepositoryActiveToFilter = "";
            if (this._dtpWorkflowRepositoryActiveTo && this._dtpWorkflowRepositoryActiveTo.get_selectedDate()) {
                workflowRepositoryActiveToFilter = moment(this._dtpWorkflowRepositoryActiveTo.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            if (workflowRepositoryActiveFromFilter && workflowRepositoryActiveToFilter) {
                var startDate = new Date(workflowRepositoryActiveFromFilter);
                var endDate = new Date(workflowRepositoryActiveToFilter);
                if (startDate > endDate) {
                    return;
                }
            }
            var selectedWorkflowRepositoryStatusFilter = "";
            if (this._cmbWorkflowRepositoryStatus && this._cmbWorkflowRepositoryStatus.get_selectedItem() !== null) {
                selectedWorkflowRepositoryStatusFilter = this._cmbWorkflowRepositoryStatus.get_selectedItem().get_value();
            }
            var selectedWorkflowRepository = this._rcbWorkflowRepositories.get_selectedItem();
            var searchDTO = new WorkflowInstanceSearchFilterDTO();
            searchDTO.activeFrom = workflowRepositoryActiveFromFilter;
            searchDTO.activeTo = workflowRepositoryActiveToFilter;
            searchDTO.workflowRepositoryId = selectedWorkflowRepository ? selectedWorkflowRepository.get_value() : "";
            searchDTO.status = selectedWorkflowRepositoryStatusFilter;
            searchDTO.skip = skip;
            searchDTO.top = this._masterTableView.get_pageSize();
            this._searchWorkflowInstances(searchDTO);
        };
        WorkflowInstances.prototype._searchWorkflowInstances = function (searchDTO) {
            var _this = this;
            this._loadingPanel.show(this.workflowInstancesGridId);
            this.workflowInstanceService.getWorkflowInstances(searchDTO, function (data) {
                if (!data)
                    return;
                _this._gridResult = data;
                for (var i = 0; i < _this._gridResult.length; i++) {
                    if (_this._gridResult[i].Status !== "None") {
                        _this._gridResult[i].Status = _this._enumHelper.getWorkflowStatusDescription(_this._gridResult[i].Status);
                    }
                }
                _this._masterTableView.set_dataSource(_this._gridResult);
                if (!_this._gridResult.length) {
                    _this._setGridTotalElementsCount(0);
                    _this._loadingPanel.hide(_this.workflowInstancesGridId);
                    return;
                }
                _this.workflowInstanceService.countWorkflowInstances(searchDTO, function (totalWorkflowInstances) {
                    _this._setGridTotalElementsCount(totalWorkflowInstances);
                    _this._loadingPanel.hide(_this.workflowInstancesGridId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.workflowInstancesGridId);
                    $("#".concat(_this.workflowInstancesGridId)).hide();
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.workflowInstancesGridId);
                $("#".concat(_this.workflowInstancesGridId)).hide();
            });
        };
        WorkflowInstances.prototype._setGridTotalElementsCount = function (count) {
            this._masterTableView.set_virtualItemCount(count);
            this._masterTableView.dataBind();
        };
        return WorkflowInstances;
    }(WorkflowInstancesBase));
    return WorkflowInstances;
});
//# sourceMappingURL=WorkflowInstances.js.map