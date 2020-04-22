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
define(["require", "exports", "./UserUDSBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/UDS/UDSService", "App/DTOs/UDSArchiveSearchFilterDTO"], function (require, exports, UserUDSBase, ServiceConfigurationHelper, UDSService, UDSArchiveSearchFilterDTO) {
    var UserUDS = /** @class */ (function (_super) {
        __extends(UserUDS, _super);
        function UserUDS(serviceConfigurations) {
            var _this = _super.call(this, serviceConfigurations) || this;
            _this.onGridDataBound = function (sender, eventArgs) {
                var row = _this._masterTableView.get_dataItems();
                for (var i = 0; i < row.length; i++) {
                    if (i % 2) {
                        row[i].addCssClass("Chiaro");
                    }
                    else {
                        row[i].addCssClass("Scuro");
                    }
                }
            };
            _this.onGridCommand = function (sender, eventArgs) {
                if (eventArgs.get_commandName() === "Page") {
                    eventArgs.set_cancel(true);
                    _this.onPageChanged();
                }
                if (eventArgs.get_commandName() === "Sort") {
                    eventArgs.set_cancel(true);
                    _this.loadResults(0);
                }
            };
            _this.btnSearch_onClick = function (sender, eventArgs) {
                if (_this._rcbRepositoryName.get_selectedItem()) {
                    _this.loadResults(0);
                }
                else {
                    alert("Il filtro di archivio è obbligatorio");
                }
            };
            _this._serviceConfiguration = serviceConfigurations;
            $(document).ready(function () { });
            return _this;
        }
        UserUDS.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._rcbRepositoryName = $find(this.rcbRepositoryNameId);
            this._chkAuthorized = document.getElementById(this.chkAuthorizedId);
            this._udsGrid = $find(this.udsGridId);
            this._masterTableView = this._udsGrid.get_masterTableView();
            if (this._masterTableView) {
                this._masterTableView.set_currentPageIndex(0);
                this._masterTableView.set_virtualItemCount(0);
            }
            this._btnSearch = $find(this.btnSearchId);
            if (this._btnSearch) {
                this._btnSearch.add_clicked(this.btnSearch_onClick);
            }
            $("#".concat(this.udsGridId)).bind(UserUDS.LOADED_EVENT, function () {
                _this.loadUDSGrid();
            });
            this.loadRepositories();
        };
        UserUDS.prototype.loadUDSGrid = function () {
            if (!jQuery.isEmptyObject(this._udsGrid)) {
                this.loadResults(0);
            }
        };
        UserUDS.prototype.loadRepositories = function () {
            var _this = this;
            try {
                this._loadingPanel.show(this.rcbRepositoryNameId);
                this._rcbRepositoryName.clearSelection();
                if (this.multiTenantEnabled === "True") {
                    this.udsRepositoryService.getTenantUDSRepositories(this.tenantCompanyName, "", function (data) {
                        if (!data)
                            return;
                        _this._rcbRepositoryName.trackChanges();
                        _this._rcbRepositoryName.get_items().clear();
                        var thisCmbRepositoryName = _this._rcbRepositoryName;
                        var cmbItem = null;
                        $.each(data, function (i, value) {
                            cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                            cmbItem.set_text(value.Name);
                            cmbItem.set_value(value.UniqueId);
                            thisCmbRepositoryName.get_items().add(cmbItem);
                        });
                        _this._rcbRepositoryName.commitChanges();
                        _this._loadingPanel.hide(_this.rcbRepositoryNameId);
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.rcbRepositoryNameId);
                        $("#".concat(_this.rcbRepositoryNameId)).hide();
                    });
                }
                else {
                    this.udsRepositoryService.getUDSRepositories("", function (data) {
                        if (!data)
                            return;
                        _this._rcbRepositoryName.trackChanges();
                        _this._rcbRepositoryName.get_items().clear();
                        var thisCmbRepositoryName = _this._rcbRepositoryName;
                        var cmbItem = null;
                        $.each(data, function (i, value) {
                            cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                            cmbItem.set_text(value.Name);
                            cmbItem.set_value(value.UniqueId);
                            thisCmbRepositoryName.get_items().add(cmbItem);
                        });
                        _this._rcbRepositoryName.commitChanges();
                        _this._loadingPanel.hide(_this.rcbRepositoryNameId);
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.rcbRepositoryNameId);
                        $("#".concat(_this.rcbRepositoryNameId)).hide();
                    });
                }
            }
            catch (error) {
                console.log(error.stack);
            }
        };
        UserUDS.prototype.loadResults = function (skip) {
            var _this = this;
            var UDSArchive_TYPE_NAME = this._rcbRepositoryName.get_selectedItem().get_text();
            var udsService;
            var UDSArchiveConfiguration = ServiceConfigurationHelper.getService(this._serviceConfiguration, UDSArchive_TYPE_NAME);
            udsService = new UDSService(UDSArchiveConfiguration);
            this._udsGrid = $find(this.udsGridId);
            this._masterTableView = this._udsGrid.get_masterTableView();
            this._loadingPanel.show(this.udsGridId);
            var rcbRepositoryName = "";
            if (this._rcbRepositoryName && this._rcbRepositoryName.get_selectedItem() !== null) {
                rcbRepositoryName = this._rcbRepositoryName.get_selectedItem().get_value();
            }
            var searchDTO = new UDSArchiveSearchFilterDTO();
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
            var top = this._masterTableView.get_pageSize();
            try {
                udsService.getOnlyToReadUDSArchives(searchDTO, top, skip, "", function (data) {
                    if (!data)
                        return;
                    _this.gridResult = data;
                    _this._masterTableView.set_dataSource(data.value);
                    _this._masterTableView.set_virtualItemCount(data.count);
                    _this._masterTableView.dataBind();
                    _this._loadingPanel.hide(_this.udsGridId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.udsGridId);
                });
            }
            catch (error) {
                console.log(error.stack);
                this._loadingPanel.hide(this.udsGridId);
            }
        };
        UserUDS.prototype.onPageChanged = function () {
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
            this.loadResults(skip);
        };
        UserUDS.LOADED_EVENT = "onLoaded";
        UserUDS.PAGE_CHANGED_EVENT = "onPageChanged";
        return UserUDS;
    }(UserUDSBase));
    return UserUDS;
});
//# sourceMappingURL=UserUDS.js.map