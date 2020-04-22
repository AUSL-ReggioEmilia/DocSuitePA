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
define(["require", "exports", "./UDSInvoiceBase", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/UDSInvoiceSearchFilterDTO", "App/Helpers/EnumHelper", "App/Models/UDS/UDSInvoiceDirection", "App/Services/UDS/UDSService"], function (require, exports, UDSInvoiceBase, ServiceConfigurationHelper, UDSInvoiceSearchFilterDTO, EnumHelper, UDSInvoiceDirection, UDSService) {
    var UDSInvoiceSearch = /** @class */ (function (_super) {
        __extends(UDSInvoiceSearch, _super);
        function UDSInvoiceSearch(serviceConfigurations) {
            var _this = _super.call(this, serviceConfigurations) || this;
            _this.cmdRepositoriName_OnSelectedIndexChange = function (sender, args) {
                var selectedItem = sender.get_selectedItem();
                if (selectedItem != null && selectedItem.get_value() != "") {
                    _this.loadUDSInvoiceTipologyByID(selectedItem.get_value());
                    var isReceivableInvoice = selectedItem.get_text().endsWith(InvoiceRepositories.B2BReceivable.toString());
                    _this._ltlLabelPecMail.hide();
                    _this._ltlchktxtPecMail.hide();
                    if (isReceivableInvoice) {
                        _this._ltlLabelPecMail.show();
                        _this._ltlchktxtPecMail.show();
                    }
                    document.getElementById("ltlSupplier").hidden = !isReceivableInvoice;
                    document.getElementById("ltlCustomer").hidden = isReceivableInvoice;
                    if (_this._btnInvoiceMove) {
                        _this._btnInvoiceMove.set_enabled(isReceivableInvoice);
                    }
                    _this._btnInvoiceDelete.set_visible(!isReceivableInvoice);
                    return;
                }
            };
            _this.btnSearch_onClick = function (sender, args) {
                _this.loadResults(0);
            };
            _this.btnDocuments_onClick = function (sender, args) {
                //showGridLoadingPanel();
                //var selectedItems = getSelectedItems();
                //if (!selectedItems || selectedItems.length == 0) {
                //    hideGridLoadingPanel();
                //    alert("Nessun archivio selezionato.");
                //    return;
                //}
                //var selection = selectedItems.reduce(function (prev, curr) {
                //    prev[curr.getDataKeyValue("UDSId")] = curr.getDataKeyValue("IdUDSRepository");
                //    return prev;
                //}, {});
                //window.location.href = "../Viewers/UDSViewer.aspx?UDSIds=".concat(encodeURIComponent(JSON.stringify(selection)));
            };
            _this.btnSelectAll_onClick = function (sender, args) {
            };
            _this.btnDeselectAll_onClick = function (sender, args) {
            };
            _this.btnUpload_onClick = function (sender, args) {
                var url = 'UDSInvoicesUpload.aspx?Type=UDS';
                _this.openWindow(url, 'managerUploadDocument', 770, 450);
            };
            _this.btnInvoiceMoved_onClick = function (sender, args) {
                var url = 'UDSInvoiceMove.aspx?Type=UDS&IdUDS='.concat(sender.get_value());
                _this.openWindow(url, 'managerUploadDocument', 770, 450);
            };
            _this.btnInvoiceDelete_onClick = function (sender, args) {
                var url = 'UDSInvoiceDelete.aspx?Type=UDS&IdUDS='.concat(sender.get_value());
                _this.openWindow(url, 'managerUploadDocument', 770, 450);
            };
            _this.btnClean_onClick = function (sender, args) {
                _this.cleanSearchFilters();
            };
            _this.cleanSearchFilters = function () {
                _this._dpStartDate.clear();
                _this._dpEndDate.clear();
                _this._cmdRepositoriName.clearSelection();
                _this._cmdRepositoriName.enable();
                _this._cmbStato.clearSelection();
                _this._cmbStato.enable();
                _this._txtNumeroFattura.clear();
                _this._txtImporto.clear();
                _this._txtYear.clear();
                _this._txtDenominazioneManual.clear();
                _this._dtpDataAcceptFrom.clear();
                _this._dtpDataAcceptTo.clear();
                _this._dtpDataIvaFrom.clear();
                _this._dtpDataIvaTo.clear();
                _this._dtpDataReciveSDIFrom.clear();
                _this._dtpDataReciveSDITo.clear();
                sessionStorage.removeItem(_this._cmdRepositoriName.get_selectedItem().get_text());
            };
            _this._serviceConfiguration = serviceConfigurations;
            _this._enumHelper = new EnumHelper();
            $(document).ready(function () { });
            return _this;
        }
        UDSInvoiceSearch.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._udsInvoiveGridGrid = $find(this.udsInvoiceGridId);
            this._masterTableView = this._udsInvoiveGridGrid.get_masterTableView();
            this._masterTableView.set_currentPageIndex(0);
            this._masterTableView.set_virtualItemCount(0);
            this.udsInvoiceDirection = this.direction === "1" ? UDSInvoiceDirection.Outgoing : UDSInvoiceDirection.Incoming;
            $("#".concat(this.udsInvoiceGridId)).bind(UDSInvoiceSearch.LOADED_EVENT, function () {
                _this.loadUDSInvoiceGrid();
            });
            this._dpStartDate = $find(this.dpStartDateFromId);
            this._dpEndDate = $find(this.dpEndDateFromId);
            this._cmbStato = $find(this.cmbStatoId);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicking(this.btnSearch_onClick);
            //this._btnDocuments = <Telerik.Web.UI.RadButton>$find(this.btnDocumentsId);
            //this._btnDocuments.add_clicking(this.btnDocuments_onClick);
            //this._btnSelectAll = <Telerik.Web.UI.RadButton>$find(this.btnSelectAllId);
            //this._btnSelectAll.add_clicking(this.btnSelectAll_onClick);
            //this._btnDeselectAll = <Telerik.Web.UI.RadButton>$find(this.btnDeselectAllId);
            //this._btnDeselectAll.add_clicking(this.btnDeselectAll_onClick);
            this._btnUpload = $find(this.btnUploadId);
            this._btnUpload.add_clicking(this.btnUpload_onClick);
            this._btnInvoiceMove = $find(this.btnInvoiceMovedId);
            if (this._btnInvoiceMove) {
                this._btnInvoiceMove.set_enabled(false);
                this._btnInvoiceMove.add_clicking(this.btnInvoiceMoved_onClick);
            }
            this._btnInvoiceDelete = $find(this.btnInvoiceDeleteId);
            this._btnInvoiceDelete.set_enabled(false);
            this._btnInvoiceDelete.add_clicking(this.btnInvoiceDelete_onClick);
            this._btnClean = $find(this.btnCleanId);
            this._btnClean.add_clicking(this.btnClean_onClick);
            this._cmdRepositoriName = $find(this.cmdRepositoriNameId);
            this._cmdRepositoriName.add_selectedIndexChanged(this.cmdRepositoriName_OnSelectedIndexChange);
            this._txtNumeroFattura = $find(this.txtNumeroFatturaId);
            this._txtImporto = $find(this.txtImportoId);
            this._txtPIVACF = $find(this.txtPIVACFId);
            this._txtDenominazioneManual = $find(this.txtDenominazioneManualId);
            this._txtYear = $find(this.txtYearId);
            this._dtpDataIvaFrom = $find(this.dtpDataIvaFromId);
            this._dtpDataIvaTo = $find(this.dtpDataIvaToId);
            this._dtpDataReciveSDIFrom = $find(this.dtpDataReciveSDIFromId);
            this._dtpDataReciveSDITo = $find(this.dtpDataReciveSDIToId);
            this._dtpDataAcceptFrom = $find(this.dtpDataAcceptFromId);
            this._dtpDataAcceptTo = $find(this.dtpDataAcceptToId);
            this._txtIDSDI = $find(this.txtIDSDIId);
            this._txtProgressIDSDIId = $find(this.txtProgressIDSDIId);
            this._txtPecMail = $find(this.txtIndirizzoPECId);
            this._ltlLabelPecMail = $("#ltlPecMail");
            this._ltlchktxtPecMail = $("#ltlchktxtPecMail");
            this._gridTemplateColumn = $find("IndirizzoPec");
            this.invoiceSelections = [];
            this.loadData();
        };
        UDSInvoiceSearch.prototype.setdate = function () {
            var endDate = new Date();
            var startDate = new Date();
            startDate.setDate(endDate.getDate() - 30);
            this._dpStartDate.set_selectedDate(startDate);
            this._dpEndDate.set_selectedDate(endDate);
        };
        UDSInvoiceSearch.prototype.loadData = function () {
            var _this = this;
            this.loadUDSInvoiceTipology().done(function () {
                _this.setComboValues();
            }).fail(function (exception) {
                _this.showNotificationException(_this.cmdRepositoriNameId, exception, "Errore nel caricamento dei dati.");
            });
        };
        UDSInvoiceSearch.prototype.loadUDSInvoiceTipology = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this._loadingPanel.show(this.cmdRepositoriNameId);
                this._cmdRepositoriName.clearSelection();
                this.udsRepositoryService.getUDSRepositoryByUDSTypologyName(this.udsInvoiceTypology, this.tenantCompanyName, function (data) {
                    if (!data)
                        return;
                    var thisCmbRepositoriName = _this._cmdRepositoriName;
                    var cmbItem = null;
                    $.each(data, function (i, value) {
                        cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                        cmbItem.set_text(value.Name);
                        cmbItem.set_value(value.UniqueId);
                        thisCmbRepositoriName.get_items().add(cmbItem);
                    });
                    _this._loadingPanel.hide(_this.cmdRepositoriNameId);
                    promise.resolve();
                }, function (exception) {
                    _this._loadingPanel.hide(_this.cmdRepositoriNameId);
                    $("#".concat(_this.cmdRepositoriNameId)).hide();
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        UDSInvoiceSearch.prototype.setComboValues = function () {
            var _this = this;
            var tipoarchivioId = '';
            var tipoarchivioName = '';
            var tipoarchiviofinder = '';
            if (this.direction == "1") {
                switch (this.invoiceKind) {
                    case 'B2B':
                        tipoarchivioName = InvoiceRepositories.B2BSendable;
                        break;
                    case 'PA':
                        tipoarchivioName = InvoiceRepositories.PASendable;
                        break;
                }
                this._ltlLabelPecMail.hide();
                this._ltlchktxtPecMail.hide();
            }
            if (this.direction == "0") {
                if (this.invoiceKind == 'B2B') {
                    tipoarchivioName = InvoiceRepositories.B2BReceivable;
                }
                this._ltlLabelPecMail.show();
                this._ltlchktxtPecMail.show();
            }
            var ddlRepositoriName;
            var thisCmbRepositoriName = this._cmdRepositoriName;
            tipoarchiviofinder = tipoarchivioName;
            if (this.tenantCompanyName != "") {
                tipoarchiviofinder = this.tenantCompanyName + " - " + tipoarchivioName;
            }
            ddlRepositoriName = thisCmbRepositoriName.findItemByText(tipoarchiviofinder);
            var enablerepository = true;
            if (tipoarchivioName == "") {
                this.setLastRepositoriSearchFilter("UdsInvoiceSearch");
                if (this._cmdRepositoriName && this._cmdRepositoriName.get_selectedItem() !== null) {
                    tipoarchivioId = this._cmdRepositoriName.get_selectedItem().get_value();
                    tipoarchivioName = this._cmdRepositoriName.get_selectedItem().get_text();
                }
                ddlRepositoriName = thisCmbRepositoriName.findItemByValue(tipoarchivioId);
            }
            else {
                ddlRepositoriName = this._cmdRepositoriName.findItemByText(tipoarchiviofinder);
                ddlRepositoriName.select();
                enablerepository = false;
            }
            if (tipoarchivioName != "" && ddlRepositoriName != null) {
                this.setdate();
                this.loadUDSInvoiceTipologyByID(ddlRepositoriName.get_value()).done(function () {
                    _this.setLastSearchFilter(tipoarchiviofinder);
                    if (enablerepository) {
                        _this._cmdRepositoriName.enable();
                        _this._cmbStato.enable();
                    }
                    else {
                        _this._cmdRepositoriName.disable();
                        if (_this.invoiceStatus != "") {
                            _this.setComboState(_this.invoiceStatus);
                        }
                        _this._cmbStato.disable();
                    }
                    _this.loadUDSInvoiceGrid();
                }).fail(function (exception) {
                    _this.showNotificationException(_this.cmdRepositoriNameId, exception, "Errore nel caricamento dei dati.");
                });
            }
        };
        UDSInvoiceSearch.prototype.setComboState = function (valore) {
            var selectedItem = this._cmbStato.findItemByText(valore);
            selectedItem.select();
            this._cmbStato.trackChanges();
        };
        UDSInvoiceSearch.prototype.loadUDSInvoiceTipologyByID = function (udsid) {
            var _this = this;
            var promise = $.Deferred();
            try {
                this.udsRepositoryService.getUDSRepositoryByID(udsid, function (data) {
                    if (!data)
                        return;
                    var modxml = data[0].ModuleXML;
                    var xmlDoc = $.parseXML(modxml);
                    var $xml = $(xmlDoc);
                    var status = [];
                    $xml.find("Status>Options>State").map(function (i, el) {
                        status.push($(el).text());
                    });
                    _this.loadStatus(status).done(function () {
                        promise.resolve();
                    });
                }, function (exception) {
                    _this._loadingPanel.hide(_this.cmdRepositoriNameId);
                    $("#".concat(_this.cmdRepositoriNameId)).hide();
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        /**
       * Apre una nuova nuova RadWindow
       * @param url
       * @param name
       * @param width
       * @param height
       */
        UDSInvoiceSearch.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.radWindowManagerId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        UDSInvoiceSearch.prototype.loadUDSInvoiceGrid = function () {
            if (!jQuery.isEmptyObject(this._udsInvoiveGridGrid)) {
                this.loadResults(0);
            }
        };
        UDSInvoiceSearch.prototype.loadStatus = function (status) {
            var promise = $.Deferred();
            try {
                this._loadingPanel.show(this.cmbStatoId);
                var cmbItem = null;
                this._cmbStato.clearItems();
                var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._cmbStato.get_items().insert(0, emptyItem);
                for (var n in status) {
                    cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(status[n]);
                    cmbItem.set_value(status[n]);
                    this._cmbStato.get_items().add(cmbItem);
                }
                this._loadingPanel.hide(this.cmbStatoId);
                promise.resolve();
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        UDSInvoiceSearch.prototype.loadResults = function (skip) {
            var _this = this;
            var UDSInvoice_TYPE_NAME = this._cmdRepositoriName.get_selectedItem().get_text();
            var udsService;
            var UDSInvoiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfiguration, UDSInvoice_TYPE_NAME);
            udsService = new UDSService(UDSInvoiceConfiguration);
            this._loadingPanel.show(this.udsInvoiceGridId);
            var cmdRepositoryName = "";
            var cmdRepositoryId = "";
            if (this._cmdRepositoriName && this._cmdRepositoriName.get_selectedItem() !== null) {
                cmdRepositoryName = this._cmdRepositoriName.get_selectedItem().get_text();
                cmdRepositoryId = this._cmdRepositoriName.get_selectedItem().get_value();
            }
            var statofatturafilter = "";
            if (this._cmbStato && this._cmbStato.get_selectedItem() !== null) {
                statofatturafilter = this._cmbStato.get_selectedItem().get_value();
            }
            var startDateFromFilter = "";
            if (this._dpStartDate && this._dpStartDate.get_selectedDate()) {
                startDateFromFilter = moment(this._dpStartDate.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var endDateFromFilter = "";
            if (this._dpEndDate && this._dpEndDate.get_selectedDate()) {
                endDateFromFilter = moment(this._dpEndDate.get_selectedDate()).add(1, 'days').add(-1, 'seconds').format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var numerofatturafilter = this._txtNumeroFattura.get_value();
            var numerofatturafilterEq = !$("#chkNumeroFatturafilter").is(":checked");
            var importoFilter = this._txtImporto.get_value();
            //let importoFilterEq: boolean = !$("#rbltxtImportofilter").is(":checked");
            //let pivacfFilter: string = this._txtPIVACF.get_textBoxValue();
            //let pivacfFilterEq: boolean = !$("#rbltxtPIVACFfilter").is(":checked");
            var denomiazioneFilter = this._txtDenominazioneManual.get_value();
            var denomiazioneFilterEq = !$("#chkDenominazioneManualfilter").is(":checked");
            var annoivaFilter = this._txtYear.get_value();
            var dataIvaFromFilter = "";
            if (this._dtpDataIvaFrom && this._dtpDataIvaFrom.get_selectedDate()) {
                dataIvaFromFilter = moment(this._dtpDataIvaFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var dataIvaToFilter = "";
            if (this._dtpDataIvaTo && this._dtpDataIvaTo.get_selectedDate()) {
                dataIvaToFilter = moment(this._dtpDataIvaTo.get_selectedDate()).add(1, 'days').add(-1, 'seconds').format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var dataReceivedFromFilter = "";
            if (this._dtpDataReciveSDIFrom && this._dtpDataReciveSDIFrom.get_selectedDate()) {
                dataReceivedFromFilter = moment(this._dtpDataReciveSDIFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var dataReceivedToFilter = "";
            if (this._dtpDataReciveSDITo && this._dtpDataReciveSDITo.get_selectedDate()) {
                dataReceivedToFilter = moment(this._dtpDataReciveSDITo.get_selectedDate()).add(1, 'days').add(-1, 'seconds').format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var dataacceptFromFilter = "";
            if (this._dtpDataAcceptFrom && this._dtpDataAcceptFrom.get_selectedDate()) {
                dataacceptFromFilter = moment(this._dtpDataAcceptFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var dataacceptToFilter = "";
            if (this._dtpDataAcceptTo && this._dtpDataAcceptTo.get_selectedDate()) {
                dataacceptToFilter = moment(this._dtpDataAcceptTo.get_selectedDate()).add(1, 'days').add(-1, 'seconds').format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            //let identificativoSdiFilter: string = this._txtIDSDI.get_textBoxValue();
            //let identificativoSdiFilterEq: boolean = !$("#rbltxtIDSDIfilter").is(":checked");
            //let progressIDSDIFilter: string = this._txtProgressIDSDIId.get_textBoxValue();
            //let progressIDSDIFilterEq: boolean = !$("#rbltxtProgressIDSDIfilter").is(":checked");
            var pecMailBoxFilter = this._txtPecMail.get_textBoxValue();
            var pecMailBoxFilterEq = $("#chktxtPecMail").is(":checked");
            var searchDTO = new UDSInvoiceSearchFilterDTO();
            searchDTO.cmdRepositoriName = cmdRepositoryId;
            searchDTO.startDateFromFilter = startDateFromFilter;
            searchDTO.endDateFromFilter = endDateFromFilter;
            searchDTO.numerofatturafilter = numerofatturafilter;
            searchDTO.numerofatturafilterEq = numerofatturafilterEq;
            searchDTO.importoFilter = importoFilter;
            //searchDTO.importoFilterEq = importoFilterEq;
            //searchDTO.pivacfFilter = pivacfFilter;
            //searchDTO.pivacfFilterEq = pivacfFilterEq;
            searchDTO.denomiazioneFilter = denomiazioneFilter;
            searchDTO.denomiazioneFilterEq = denomiazioneFilterEq;
            searchDTO.annoivaFilter = annoivaFilter;
            searchDTO.dataReceivedFromFilter = dataReceivedFromFilter;
            searchDTO.dataReceivedToFilter = dataReceivedToFilter;
            searchDTO.dataIvaFromFilter = dataIvaFromFilter;
            searchDTO.dataIvaToFilter = dataIvaToFilter;
            searchDTO.dataacceptFromFilter = dataacceptFromFilter;
            searchDTO.dataacceptToFilter = dataacceptToFilter;
            //searchDTO.identificativoSdiFilter = identificativoSdiFilter;
            //searchDTO.identificativoSdiFilterEq = identificativoSdiFilterEq;
            //searchDTO.progressIDSDIFilter = progressIDSDIFilter;
            //searchDTO.progressIDSDIFilterEq = progressIDSDIFilterEq;
            searchDTO.statofatturaFilter = statofatturafilter;
            searchDTO.pecMailBoxFilter = pecMailBoxFilter;
            searchDTO.pecMailBoxFilterEq = pecMailBoxFilterEq;
            sessionStorage.setItem(cmdRepositoryName, JSON.stringify(searchDTO));
            sessionStorage.setItem("UdsInvoiceSearch", JSON.stringify(searchDTO));
            var sortExpressions = this._masterTableView.get_sortExpressions();
            var orderbyExpressions;
            orderbyExpressions = sortExpressions.toString();
            if (orderbyExpressions == "") {
                orderbyExpressions = "NumeroFattura asc";
            }
            var top = this._masterTableView.get_pageSize();
            var setVisiblePecMail = $("#ltlPecMail").is(":visible");
            if (setVisiblePecMail) {
                this._masterTableView.showColumn(13); //colonna indirizzo pec
                var gridSorter;
                gridSorter = sortExpressions.toString();
                if (gridSorter == "") {
                    orderbyExpressions = "DataRicezioneSdi asc";
                }
            }
            else {
                this._masterTableView.hideColumn(13); //colonna indirizzo pec
            }
            try {
                udsService.getUDSInvoices(searchDTO, top, skip, orderbyExpressions, function (data) {
                    if (!data)
                        return;
                    _this.gridResult = data;
                    _this._masterTableView.set_dataSource(data.value);
                    _this._masterTableView.set_virtualItemCount(data.count);
                    _this._masterTableView.dataBind();
                    _this._loadingPanel.hide(_this.udsInvoiceGridId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.udsInvoiceGridId);
                });
            }
            catch (error) {
                console.log(error.stack);
                this._loadingPanel.hide(this.udsInvoiceGridId);
            }
        };
        UDSInvoiceSearch.prototype.setLastRepositoriSearchFilter = function (tipoarchivioName) {
            var UDSinvoiceLastSearch = sessionStorage.getItem(tipoarchivioName);
            if (UDSinvoiceLastSearch == null) {
                UDSinvoiceLastSearch = sessionStorage.getItem("UdsInvoiceSearch");
            }
            if (UDSinvoiceLastSearch) {
                var lastsearchFilter = JSON.parse(UDSinvoiceLastSearch);
                if (lastsearchFilter.cmdRepositoriName) {
                    var selectedItem = this._cmdRepositoriName.findItemByValue(lastsearchFilter.cmdRepositoriName.toString());
                    selectedItem.select();
                    this._cmdRepositoriName.trackChanges();
                }
            }
        };
        UDSInvoiceSearch.prototype.setLastSearchFilter = function (tipoarchivioName) {
            var UDSinvoiceLastSearch = sessionStorage.getItem(tipoarchivioName);
            if (UDSinvoiceLastSearch == null) {
                UDSinvoiceLastSearch = sessionStorage.getItem("UdsInvoiceSearch");
            }
            if (UDSinvoiceLastSearch) {
                var lastsearchFilter = JSON.parse(UDSinvoiceLastSearch);
                this._dpStartDate.set_selectedDate(lastsearchFilter.startDateFromFilter ? new Date(lastsearchFilter.startDateFromFilter.toString()) : null);
                this._dpEndDate.set_selectedDate(lastsearchFilter.endDateFromFilter ? new Date(lastsearchFilter.endDateFromFilter.toString()) : null);
                this._txtNumeroFattura.set_value(lastsearchFilter.numerofatturafilter ? lastsearchFilter.numerofatturafilter.toString() : null);
                $("#chkNumeroFatturafilter").prop("checked", (lastsearchFilter.numerofatturafilterEq ? lastsearchFilter.numerofatturafilterEq.toString() : true));
                this._txtImporto.set_value(lastsearchFilter.importoFilter ? lastsearchFilter.importoFilter.toString().replace(".", ",") : null);
                this._txtDenominazioneManual.set_value(lastsearchFilter.denomiazioneFilter ? lastsearchFilter.denomiazioneFilter.toString() : null);
                !$("#chkDenominazioneManualfilter").prop("checked", (lastsearchFilter.denomiazioneFilterEq ? lastsearchFilter.denomiazioneFilterEq.toString() : true));
                this._txtYear.set_value(lastsearchFilter.annoivaFilter ? lastsearchFilter.annoivaFilter.toString() : null);
                this._dtpDataIvaFrom.set_selectedDate(lastsearchFilter.dataIvaFromFilter ? new Date(lastsearchFilter.dataIvaFromFilter.toString()) : null);
                this._dtpDataIvaTo.set_selectedDate(lastsearchFilter.dataIvaToFilter ? new Date(lastsearchFilter.dataIvaToFilter.toString()) : null);
                this._dtpDataReciveSDIFrom.set_selectedDate(lastsearchFilter.dataReceivedFromFilter ? new Date(lastsearchFilter.dataReceivedFromFilter.toString()) : null);
                this._dtpDataReciveSDITo.set_selectedDate(lastsearchFilter.dataReceivedToFilter ? new Date(lastsearchFilter.dataReceivedToFilter.toString()) : null);
                this._dtpDataAcceptFrom.set_selectedDate(lastsearchFilter.dataacceptFromFilter ? new Date(lastsearchFilter.dataacceptFromFilter.toString()) : null);
                this._dtpDataAcceptTo.set_selectedDate(lastsearchFilter.dataacceptToFilter ? new Date(lastsearchFilter.dataacceptToFilter.toString()) : null);
                var lastsearchFilterStatofattura = void 0;
                if (lastsearchFilter.statofatturaFilter) {
                    lastsearchFilterStatofattura = lastsearchFilter.statofatturaFilter;
                    this.setComboState(lastsearchFilterStatofattura);
                }
            }
        };
        UDSInvoiceSearch.prototype.onPageChanged = function () {
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
            this.loadResults(skip);
        };
        //region [ Grid Configuration Methods ]
        UDSInvoiceSearch.prototype.onGridDataBound = function () {
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
        UDSInvoiceSearch.prototype.changeEnableBtn = function (setEnable, setvalue) {
            if (setEnable) {
                this._btnInvoiceDelete.set_value(setvalue);
                if (this._btnInvoiceMove) {
                    this._btnInvoiceMove.set_value(setvalue);
                }
            }
            else {
                this._btnInvoiceDelete.set_value("");
                if (this._btnInvoiceMove) {
                    this._btnInvoiceMove.set_value("");
                }
            }
            this._btnInvoiceDelete.set_enabled(setEnable);
            if (this._btnInvoiceMove) {
                var UDSInvoice_TYPE_NAME = this._cmdRepositoriName.get_selectedItem().get_text();
                var isReceivableInvoice = UDSInvoice_TYPE_NAME.endsWith(InvoiceRepositories.B2BReceivable.toString());
                this._btnInvoiceMove.set_enabled(setEnable && isReceivableInvoice);
            }
        };
        UDSInvoiceSearch.prototype.saveInvoiceSelectionsToSessionStorage = function (checkboxes) {
            for (var _i = 0, checkboxes_1 = checkboxes; _i < checkboxes_1.length; _i++) {
                var checkbox = checkboxes_1[_i];
                var index = this.invoiceSelections.indexOf(checkbox.value);
                if (checkbox.checked && index === -1) {
                    this.invoiceSelections.push(checkbox.value);
                }
                else if (!checkbox.checked && index > -1) {
                    this.invoiceSelections.splice(index, 1);
                }
            }
            sessionStorage.setItem("InvoiceSelections", JSON.stringify(this.invoiceSelections));
        };
        UDSInvoiceSearch.LOADED_EVENT = "onLoaded";
        UDSInvoiceSearch.PAGE_CHANGED_EVENT = "onPageChanged";
        return UDSInvoiceSearch;
    }(UDSInvoiceBase));
    var InvoiceRepositories;
    (function (InvoiceRepositories) {
        InvoiceRepositories["B2BSendable"] = "Fatture Attive";
        InvoiceRepositories["PASendable"] = "Fatture Attive PA";
        InvoiceRepositories["B2BReceivable"] = "Fatture Passive";
    })(InvoiceRepositories || (InvoiceRepositories = {}));
    return UDSInvoiceSearch;
});
//# sourceMappingURL=UDSInvoiceSearch.js.map