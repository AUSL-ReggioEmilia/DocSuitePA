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
define(["require", "exports", "Monitors/TransparentAdministrationMonitorLogBase", "App/DTOs/TransparentAdministrationMonitorLogSearchFilterDTO", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscAmmTraspMonitorLogGrid", "App/Services/Commons/ContainerService", "App/Models/Commons/LocationTypeEnum"], function (require, exports, TransparentAdministrationMonitorLogBase, TransparentAdministrationMonitorLogSearchFilterDTO, ServiceConfigurationHelper, UscAmmTraspMonitorLogGrid, ContainerService, LocationTypeEnum) {
    var TransparentAdministrationMonitorLog = /** @class */ (function (_super) {
        __extends(TransparentAdministrationMonitorLog, _super);
        function TransparentAdministrationMonitorLog(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, TransparentAdministrationMonitorLogBase.TransparentAdministrationMonitorLog_TYPE_NAME)) || this;
            _this.btnSearch_onClick = function (sender, args) {
                var uscAmmTraspMonitorLogGrid = $("#".concat(_this.uscAmmTraspMonitorLogGridId)).data();
                _this.loadResults(uscAmmTraspMonitorLogGrid, 0);
            };
            _this.btnClean_onClick = function (sender, args) {
                _this.cleanSearchFilters();
            };
            _this.cleanSearchFilters = function () {
                _this._dpStartDateFrom.clear();
                _this._dpEndDateFrom.clear();
                _this._cmbDocumentType.clearSelection();
                _this._cmbContainer.clearSelection();
                _this._txtUsername.set_value("");
            };
            /**
            * Evento scatenato allo scrolling della RadComboBox di selezione fascicoli
            * @param args
            */
            _this.cmdContainer_onScroll = function (args) {
                var element = args.target;
                if ((element.scrollHeight - element.scrollTop === element.clientHeight) && element.clientHeight > 0) {
                    var filter = _this._cmbContainer.get_text();
                    _this.cmbContainer_OnClientItemsRequested(_this._cmbContainer, new Telerik.Web.UI.RadComboBoxRequestEventArgs(filter, args));
                }
            };
            _this._cmbContainer_OnSelectedIndexChanged = function (sender, args) {
                var selectedItem = sender.get_selectedItem();
                var domEvent = args.get_domEvent();
                if (domEvent.type == 'mousedown') {
                    return;
                }
                if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
                    var emptyItem = sender.findItemByText('');
                    sender.clearItems();
                    sender.get_items().add(emptyItem);
                    sender.get_items().add(selectedItem);
                    sender.get_attributes().setAttribute('currentFilter', selectedItem.get_text());
                    sender.get_attributes().setAttribute('otherContainerCount', '1');
                    _this.setMoreResultBoxText("Visualizzati 1 di 1");
                }
            };
            _this.cmbContainer_OnClientItemsRequested = function (sender, args) {
                var numberOfItems = sender.get_items().get_count();
                if (numberOfItems > 0) {
                    //Decremento di 1 perchè la combo visualizza anche un item vuoto
                    numberOfItems -= 1;
                }
                var currentOtherContainerItems = numberOfItems;
                var currentComboFilter = sender.get_attributes().getAttribute('currentFilter');
                var otherContainerCount = Number(sender.get_attributes().getAttribute('otherContainerCount'));
                var updating = sender.get_attributes().getAttribute('updating') == 'true';
                if (isNaN(otherContainerCount) || currentComboFilter != args.get_text()) {
                    //Se il valore del filtro è variato re-inizializzo la radcombobox per chiamare le WebAPI
                    otherContainerCount = undefined;
                }
                sender.get_attributes().setAttribute('currentFilter', args.get_text());
                _this.setMoreResultBoxText('Caricamento...');
                if ((otherContainerCount == undefined || currentOtherContainerItems < otherContainerCount) && !updating) {
                    sender.get_attributes().setAttribute('updating', 'true');
                    var location_1 = null;
                    if (_this._cmbDocumentType.get_selectedItem() != undefined && _this._cmbDocumentType.get_selectedItem().get_value() != "") {
                        var env = _this._cmbDocumentType.get_selectedItem().get_value();
                        if (env == "1") {
                            location_1 = LocationTypeEnum.ProtLocation;
                        }
                        if (env == "4") {
                            location_1 = LocationTypeEnum.DocumentSeriesLocation;
                        }
                        //i valori della variabile env sono dati dal popolamento della treeview della tipologia di Documento
                        if (env == "100") {
                            location_1 = LocationTypeEnum.UDSLocation;
                        }
                        if (location_1 == null) {
                            location_1 = LocationTypeEnum.ReslLocation;
                        }
                    }
                    _this._containerService.getContainersByEnvironment(args.get_text(), _this.maxNumberElements, currentOtherContainerItems, location_1, function (data) {
                        if (!data)
                            return;
                        try {
                            _this.addContainers(data.value);
                            var scrollToPosition = args.get_domEvent() == undefined;
                            if (scrollToPosition) {
                                if (sender.get_items().get_count() > 0) {
                                    var scrollContainer = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                    scrollContainer.scrollTop($(sender.get_items().getItem(currentOtherContainerItems + 1).get_element()).position().top);
                                }
                            }
                            sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
                            sender.get_attributes().setAttribute('updating', 'false');
                            if (sender.get_items().get_count() > 0) {
                                currentOtherContainerItems = sender.get_items().get_count() - 1;
                            }
                            _this.setMoreResultBoxText('Visualizzati '.concat(currentOtherContainerItems.toString(), ' di ', data.count.toString()));
                        }
                        catch (error) {
                            _this.showNotificationMessage(_this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                            console.log(JSON.stringify(error));
                        }
                    }, function (exception) {
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
            };
            _this._cmbDocumentType_OnSelectedIndexChanged = function (sender, args) {
                var domEvent = args.get_domEvent();
                if (domEvent.type == 'mousedown') {
                    return;
                }
                if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
                    _this._cmbContainer.clearItems();
                }
            };
            _this.getUscRole = function () {
                var roles = new Array();
                var uscRoles = $("#".concat(_this.uscOwnerRoleId)).data();
                if (!jQuery.isEmptyObject(uscRoles)) {
                    var source = JSON.parse(uscRoles.getRoles());
                    if (source != null) {
                        for (var _i = 0, source_1 = source; _i < source_1.length; _i++) {
                            var s = source_1[_i];
                            roles.push(s);
                        }
                    }
                }
                if (roles.length > 0) {
                    return roles[0];
                }
                return null;
            };
            _this._serviceConfigurations = serviceConfigurations;
            var containerConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Container");
            _this._containerService = new ContainerService(containerConfiguration);
            $(document).ready(function () {
            });
            return _this;
        }
        TransparentAdministrationMonitorLog.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._loadingPanel.show(this.uscAmmTraspMonitorLogGridId);
            $("#".concat(this.uscAmmTraspMonitorLogGridId)).bind(UscAmmTraspMonitorLogGrid.LOADED_EVENT, function () {
                _this.loadAmmTraspMonitorLogGrid();
            });
            this.loadAmmTraspMonitorLogGrid();
            $("#".concat(this.uscAmmTraspMonitorLogGridId)).bind(UscAmmTraspMonitorLogGrid.PAGE_CHANGED_EVENT, function (args) {
                var uscAmmTraspMonitorLogGrid = $("#".concat(_this.uscAmmTraspMonitorLogGridId)).data();
                if (!jQuery.isEmptyObject(uscAmmTraspMonitorLogGrid)) {
                    _this.pageChange(uscAmmTraspMonitorLogGrid);
                }
            });
            this._dpStartDateFrom = $find(this.dpStartDateFromId);
            this._dpEndDateFrom = $find(this.dpEndDateFromId);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicking(this.btnSearch_onClick);
            this._btnClean = $find(this.btnCleanId);
            this._btnClean.add_clicking(this.btnClean_onClick);
            this._txtTransparentAdministrationMonitorLogItems = document.getElementById(this.transparentAdministrationMonitorLogItemsId);
            this._txtUsername = $find(this.txtUsernameId);
            this._cmbDocumentType = $find(this.cmbDocumentTypeId);
            this._cmbContainer = $find(this.cmbContainerId);
            this._cmbDocumentType.add_selectedIndexChanged(this._cmbDocumentType_OnSelectedIndexChanged);
            this.populateDocumentTypeComboBox();
            this._cmbContainer.add_selectedIndexChanged(this._cmbContainer_OnSelectedIndexChanged);
            this._cmbContainer.add_itemsRequested(this.cmbContainer_OnClientItemsRequested);
            var scrollContainer = $(this._cmbContainer.get_dropDownElement()).find('div.rcbScroll');
            $(scrollContainer).scroll(this.cmdContainer_onScroll);
        };
        TransparentAdministrationMonitorLog.prototype.loadAmmTraspMonitorLogGrid = function () {
            var uscAmmTraspMonitorLogGrid = $("#".concat(this.uscAmmTraspMonitorLogGridId)).data();
            if (!jQuery.isEmptyObject(uscAmmTraspMonitorLogGrid)) {
                this.loadResults(uscAmmTraspMonitorLogGrid, 0);
            }
        };
        TransparentAdministrationMonitorLog.prototype.pageChange = function (uscAmmTraspMonitorLogGrid) {
            this._loadingPanel.show(this.uscAmmTraspMonitorLogGridId);
            var skip = uscAmmTraspMonitorLogGrid.getGridCurrentPageIndex() * uscAmmTraspMonitorLogGrid.getGridPageSize();
            this.loadResults(uscAmmTraspMonitorLogGrid, skip);
        };
        TransparentAdministrationMonitorLog.prototype.loadResults = function (uscAmmTraspMonitorLogGrid, skip) {
            var _this = this;
            var startDateFromFilter = "";
            if (this._dpStartDateFrom !== undefined && this._dpStartDateFrom.get_selectedDate()) {
                startDateFromFilter = this._dpStartDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
            }
            var endDateFromFilter = "";
            if (this._dpEndDateFrom !== undefined && this._dpEndDateFrom.get_selectedDate()) {
                endDateFromFilter = this._dpEndDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
            }
            var usernameFromFilter = "";
            if (this._txtUsername !== undefined && this._txtUsername.get_textBoxValue() !== "") {
                usernameFromFilter = this._txtUsername.get_textBoxValue();
            }
            var documentTypeFromFilter = "";
            if (this._cmbDocumentType != undefined && this._cmbDocumentType.get_selectedItem() !== null) {
                documentTypeFromFilter = this._cmbDocumentType.get_selectedItem().get_value();
            }
            var containerFromFilter = "";
            if (this._cmbContainer != undefined && this._cmbContainer.get_selectedItem() !== null) {
                containerFromFilter = this._cmbContainer.get_selectedItem().get_value();
            }
            var idRole;
            var role = this.getUscRole();
            if (role) {
                idRole = role.EntityShortId;
            }
            var searchDTO = new TransparentAdministrationMonitorLogSearchFilterDTO();
            searchDTO.dateFrom = startDateFromFilter;
            searchDTO.dateTo = endDateFromFilter;
            searchDTO.username = usernameFromFilter;
            searchDTO.documentType = documentTypeFromFilter;
            searchDTO.container = containerFromFilter;
            searchDTO.idRole = idRole;
            var top = skip + uscAmmTraspMonitorLogGrid.getGridPageSize();
            if (searchDTO.dateFrom !== "" || searchDTO.dateTo !== "")
                this.service.getTransparentAdministrationMonitorLogs(searchDTO, function (data) {
                    if (!data)
                        return;
                    _this.gridResult = data;
                    uscAmmTraspMonitorLogGrid.setDataSource(_this.gridResult);
                    _this._txtTransparentAdministrationMonitorLogItems.value = JSON.stringify(data);
                    _this._loadingPanel.hide(_this.uscAmmTraspMonitorLogGridId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.uscAmmTraspMonitorLogGridId);
                    $("#".concat(_this.uscAmmTraspMonitorLogGridId)).hide();
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            else
                this._loadingPanel.hide(this.uscAmmTraspMonitorLogGridId);
        };
        /**
    * Metodo che setta la label MoreResultBoxText della RadComboBox
    * @param message
    */
        TransparentAdministrationMonitorLog.prototype.setMoreResultBoxText = function (message) {
            this._cmbContainer.get_moreResultsBoxMessageElement().innerText = message;
        };
        TransparentAdministrationMonitorLog.prototype.populateDocumentTypeComboBox = function () {
            var documentTypeItems = [
                {
                    Name: "",
                    Value: ""
                },
                {
                    Name: "Protocollo",
                    Value: "1"
                },
                {
                    Name: "Atti",
                    Value: "2"
                },
                {
                    Name: "Serie Documentali",
                    Value: "4"
                },
                {
                    Name: "Archivio",
                    Value: "100"
                }
            ];
            var item;
            for (var _i = 0, documentTypeItems_1 = documentTypeItems; _i < documentTypeItems_1.length; _i++) {
                var documentTypeItem = documentTypeItems_1[_i];
                item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(documentTypeItem.Name);
                item.set_value(documentTypeItem.Value);
                this._cmbDocumentType.get_items().add(item);
            }
        };
        TransparentAdministrationMonitorLog.prototype.addContainers = function (containers) {
            var _this = this;
            if (containers.length > 0) {
                this._cmbContainer.beginUpdate();
                if (this._cmbContainer.get_items().get_count() == 0) {
                    var item = void 0;
                    item = new Telerik.Web.UI.RadComboBoxItem();
                    item.set_text("");
                    item.set_value("");
                    this._cmbContainer.get_items().add(item);
                }
                $.each(containers, function (index, container) {
                    var item = new Telerik.Web.UI.RadComboBoxItem();
                    item.set_text(container.Name);
                    item.set_value(container.EntityShortId.toString());
                    _this._cmbContainer.get_items().add(item);
                });
                this._cmbContainer.showDropDown();
                this._cmbContainer.endUpdate();
            }
            else {
                if (this._cmbContainer.get_items().get_count() == 0) {
                }
            }
        };
        return TransparentAdministrationMonitorLog;
    }(TransparentAdministrationMonitorLogBase));
    return TransparentAdministrationMonitorLog;
});
//# sourceMappingURL=TransparentAdministrationMonitorLog.js.map