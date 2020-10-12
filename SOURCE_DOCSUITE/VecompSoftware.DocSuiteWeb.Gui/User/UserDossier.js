define(["require", "exports", "App/Models/Dossiers/DossierType", "App/Helpers/EnumHelper", "App/DTOs/DossierSearchFilterDTO", "App/Models/Dossiers/DossierStatus", "App/Services/Dossiers/DossierService", "UserControl/uscDossierGrid", "App/DTOs/ExceptionDTO", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, DossierType, EnumHelper, DossierSearchFilterDTO, DossierStatus, DossierService, UscDossierGrid, ExceptionDTO, ServiceConfigurationHelper) {
    var UserDossier = /** @class */ (function () {
        function UserDossier(serviceConfiguration) {
            var _this = this;
            this._currentDossierType = DossierType.Procedure;
            /**
             * When search button is clicked we fetch results
             **/
            this.btnSearch_onClick = function (sender, args) {
                var uscDossierGrid = $("#" + _this.uscDossierGridId).data();
                _this.loadDossierResults(uscDossierGrid, 0);
            };
            this._serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfiguration, UserDossier.DOSSIER_SERVICE_NAME);
        }
        UserDossier.prototype.initialize = function () {
            this._enumHelper = new EnumHelper();
            this.service = new DossierService(this._serviceConfiguration);
            this._rdpDateFrom = $find(this.rdpDateFromId);
            this._rdpDateTo = $find(this.rdpDateToId);
            this._btnSearch = $find(this.btnSearchId);
            this._ddlDossierType = $find(this.ddlDossierTypeId);
            this.initializeDropDownListItems();
            this.initializeSearchDateRanges();
            this.initializeMouseEvents();
            this.initializePageEvents();
            this.initializeDossierTypeChangeEvent();
            this.initializeResultsByActionType(this.actionType);
        };
        /**
         * When the page loads, and if the query parameter Action=DOP we load results for Procedure and for diffDays
         * @param action
         */
        UserDossier.prototype.initializeResultsByActionType = function (action) {
            if (action === UserDossier.ACTION_DOP) {
                var uscDossierGrid = $("#" + this.uscDossierGridId).data();
                this._currentDossierType = DossierType.Procedure;
                this.loadDossierResults(uscDossierGrid, 0);
            }
        };
        /**
         * When the dossier type changes we change the currentDossierType. On next search button click, the current dossier type is searched for
         **/
        UserDossier.prototype.initializeDossierTypeChangeEvent = function () {
            var _this = this;
            this._ddlDossierType.add_selectedIndexChanged(function (sender, args) {
                var uscDossierGrid = $("#" + _this.uscDossierGridId).data();
                var selectedDropDown = _this._ddlDossierType.getItem(args.get_index());
                _this._currentDossierType = parseInt(selectedDropDown.get_value(), 0);
            });
        };
        /**
         * The grid is paginated. When page changes we fetch next results
         **/
        UserDossier.prototype.initializePageEvents = function () {
            var _this = this;
            $("#" + this.uscDossierGridId).bind(UscDossierGrid.PAGE_CHANGED_EVENT, function (args) {
                var uscDossierGrid = $("#" + _this.uscDossierGridId).data();
                if (!jQuery.isEmptyObject(uscDossierGrid)) {
                    _this.pageChange(uscDossierGrid);
                }
            });
        };
        /**
         * The grid is paginated. When page changes we fetch next results
         **/
        UserDossier.prototype.pageChange = function (uscDossierGrid) {
            //this._loadingPanel.show(this.uscDossierGridId);
            var skip = uscDossierGrid.getGridCurrentPageIndex() * uscDossierGrid.getGridPageSize();
            this.loadDossierResults(uscDossierGrid, skip);
        };
        /**
         * When search button is clicked we fetch results
         **/
        UserDossier.prototype.initializeMouseEvents = function () {
            this._btnSearch.add_clicked(this.btnSearch_onClick);
        };
        /**
         * Initialize the combo box with the types of dossiers
         **/
        UserDossier.prototype.initializeDropDownListItems = function () {
            this.addDropDownListItem(DossierType.Person);
            this.addDropDownListItem(DossierType.PhysicalObject);
            this.addDropDownListItem(DossierType.Procedure);
            this.addDropDownListItem(DossierType.Process);
        };
        UserDossier.prototype.addDropDownListItem = function (type, selected) {
            if (selected === void 0) { selected = false; }
            var listItem = new Telerik.Web.UI.DropDownListItem();
            listItem.set_text(this._enumHelper.getDossierTypeDescription(DossierType[type]));
            listItem.set_value(type.toString()); //number as string
            this._ddlDossierType.get_items().add(listItem);
            if (type === this._currentDossierType) {
                listItem.set_selected(true);
            }
        };
        UserDossier.prototype.initializeSearchDateRanges = function () {
            var today = new Date();
            var beforeToday = new Date();
            beforeToday.setDate(beforeToday.getDate() - this.desktopDayDiff);
            this._rdpDateFrom.set_selectedDate(beforeToday);
            this._rdpDateTo.set_selectedDate(today);
        };
        /**
         * Loading results for the current search settings. Search settings take data from the form and class properties
         * @param uscDossierGrid the grid for setting the results
         * @param skip skipping a number of results
         */
        UserDossier.prototype.loadDossierResults = function (uscDossierGrid, skip) {
            var _this = this;
            var top = skip + uscDossierGrid.getGridPageSize();
            var searchDTO = new DossierSearchFilterDTO();
            var fromDateFilter = null;
            if (this._rdpDateFrom.get_selectedDate()) {
                fromDateFilter = moment(this._rdpDateFrom.get_selectedDate()).format("YYYY-MM-DD");
            }
            var toDateFilter = null;
            if (this._rdpDateTo.get_selectedDate()) {
                toDateFilter = moment(this._rdpDateTo.get_selectedDate()).format("YYYY-MM-DD");
            }
            searchDTO.StartDateFrom = fromDateFilter;
            searchDTO.StartDateTo = toDateFilter;
            searchDTO.Status = DossierStatus[DossierStatus.Open];
            searchDTO.DossierType = DossierType[this._currentDossierType];
            searchDTO.Skip = skip;
            searchDTO.Top = top;
            this.service.getAuthorizedDossiers(searchDTO, function (data) {
                if (!data)
                    return;
                uscDossierGrid.setDataSource(data);
                _this.service.countAuthorizedDossiers(searchDTO, function (data) {
                    if (data == undefined)
                        return;
                    uscDossierGrid.setItemCount(data);
                }, function (exception) {
                    $("#" + _this.uscDossierGridId).hide();
                    console.log(exception);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }, function (exception) {
                $("#" + _this.uscDossierGridId).hide();
                console.log(exception);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        UserDossier.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#" + uscNotificationId).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        UserDossier.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#" + uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        // STATIC
        UserDossier.DOSSIER_SERVICE_NAME = "Dossier";
        UserDossier.ACTION_DOP = "DOP";
        return UserDossier;
    }());
    return UserDossier;
});
//# sourceMappingURL=UserDossier.js.map