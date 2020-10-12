/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UDSInvoiceBase = require("./UDSInvoiceBase");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import UDSInvoiceSearchFilterDTO = require('App/DTOs/UDSInvoiceSearchFilterDTO');
import ExceptionDto = require("App/DTOs/ExceptionDTO");
import EnumHelper = require("App/Helpers/EnumHelper");
import UDSInvoiceDirection = require("App/Models/UDS/UDSInvoiceDirection");
import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import UDSService = require('App/Services/UDS/UDSService');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

abstract class UDSInvoiceSearch extends UDSInvoiceBase {

    udsInvoiceGridId: string;
    ajaxLoadingPanelId: string;
    radWindowManagerId: string;
    btnSearchId: string;
    btnDocumentsId: string;
    btnSelectAllId: string;
    btnDeselectAllId: string;
    btnUploadId: string;
    btnCleanId: string;

    btnInvoiceDeleteId: string;
    btnInvoiceMovedId: string;


    dpStartDateFromId: string;
    dpEndDateFromId: string;
    cmbStatoId: string;
    udsInvoiceTypology: string;
    cmdRepositoriNameId: string;
    txtNumeroFatturaId: string;
    txtImportoId: string;
    txtPIVACFId: string;
    txtDenominazioneManualId: string;
    uscDenominazioneId: string;
    txtYearId: string;
    dtpDataIvaFromId: string;
    dtpDataIvaToId: string;
    dtpDataReciveSDIFromId: string;
    dtpDataReciveSDIToId: string;
    dtpDataAcceptFromId: string;
    dtpDataAcceptToId: string;
    txtIDSDIId: string;
    txtProgressIDSDIId: string;
    txtIndirizzoPECId: string;

    invoiceType: string;
    udsInvoiceImageId: string;

    txtPECMailBox: string;


    gridResult: any[];
    udsInvoiceDirection: UDSInvoiceDirection;
    udRepositories: UDSRepositoryModel[];
    invoiceSelections: any[];

    direction: string;
    invoiceKind: string;
    invoiceStatus: string;
    tenantCompanyName: string;

    private static FILTER_CACHE_KEY: string = "udsInvoiceFilter";
    private static UDSINVOICE_FILTER_KEY: string = "UdsInvoiceSearch";
    private _dpStartDate: Telerik.Web.UI.RadDatePicker;
    private _dpEndDate: Telerik.Web.UI.RadDatePicker;
    private _cmdRepositoriName: Telerik.Web.UI.RadComboBox;

    private _cmbStato: Telerik.Web.UI.RadComboBox;
    private _txtNumeroFattura: Telerik.Web.UI.RadTextBox;

    private _txtImporto: Telerik.Web.UI.RadTextBox;

    private _txtDenominazioneManual: Telerik.Web.UI.RadTextBox;
    private _txtYear: Telerik.Web.UI.RadTextBox;

    private _dtpDataIvaFrom: Telerik.Web.UI.RadDatePicker;
    private _dtpDataIvaTo: Telerik.Web.UI.RadDatePicker;

    private _dtpDataReciveSDIFrom: Telerik.Web.UI.RadDatePicker;
    private _dtpDataReciveSDITo: Telerik.Web.UI.RadDatePicker;

    private _dtpDataAcceptFrom: Telerik.Web.UI.RadDatePicker;
    private _dtpDataAcceptTo: Telerik.Web.UI.RadDatePicker;

    private _btnSearch: Telerik.Web.UI.RadButton;
    private _btnUpload: Telerik.Web.UI.RadButton;
    private _btnClean: Telerik.Web.UI.RadButton;

    private _btnInvoiceDelete: Telerik.Web.UI.RadButton;
    private _btnInvoiceMove: Telerik.Web.UI.RadButton;

    public static LOADED_EVENT: string = "onLoaded";
    public static PAGE_CHANGED_EVENT: string = "onPageChanged";

    private _udsInvoiveGridGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _serviceConfiguration: ServiceConfiguration[];
    private _txtPecMail: Telerik.Web.UI.RadTextBox;
    private _ltlchktxtPecMail: JQuery;
    private _ltlLabelPecMail: JQuery;
    private _emptyPecMailCheckbox: JQuery;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(serviceConfigurations);
        this._serviceConfiguration = serviceConfigurations;
        $(document).ready(() => { });
    }

    initialize() {
        super.initialize();

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        this._udsInvoiveGridGrid = <Telerik.Web.UI.RadGrid>$find(this.udsInvoiceGridId);
        this._masterTableView = this._udsInvoiveGridGrid.get_masterTableView();
        this._masterTableView.set_currentPageIndex(0);
        this._masterTableView.set_virtualItemCount(0);

        this.udsInvoiceDirection = this.direction === "1" ? UDSInvoiceDirection.Outgoing : UDSInvoiceDirection.Incoming;

        $("#".concat(this.udsInvoiceGridId)).bind(UDSInvoiceSearch.LOADED_EVENT, () => {
            this.loadUDSInvoiceGrid();
        });

        this._dpStartDate = <Telerik.Web.UI.RadDatePicker>$find(this.dpStartDateFromId);
        this._dpEndDate = <Telerik.Web.UI.RadDatePicker>$find(this.dpEndDateFromId);
        this._cmbStato = <Telerik.Web.UI.RadComboBox>$find(this.cmbStatoId);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicking(this.btnSearch_onClick);
        this._btnUpload = <Telerik.Web.UI.RadButton>$find(this.btnUploadId);
        this._btnUpload.add_clicking(this.btnUpload_onClick);

        this._btnInvoiceMove = <Telerik.Web.UI.RadButton>$find(this.btnInvoiceMovedId);
        if (this._btnInvoiceMove) {
            this._btnInvoiceMove.set_enabled(false);
            this._btnInvoiceMove.add_clicking(this.btnInvoiceMoved_onClick);
        }

        this._btnInvoiceDelete = <Telerik.Web.UI.RadButton>$find(this.btnInvoiceDeleteId);
        this._btnInvoiceDelete.set_enabled(false);
        this._btnInvoiceDelete.add_clicking(this.btnInvoiceDelete_onClick);

        this._btnClean = <Telerik.Web.UI.RadButton>$find(this.btnCleanId);
        this._btnClean.add_clicking(this.btnClean_onClick);
        this._cmdRepositoriName = <Telerik.Web.UI.RadComboBox>$find(this.cmdRepositoriNameId);
        this._cmdRepositoriName.add_selectedIndexChanged(this.cmdRepositoriName_OnSelectedIndexChange);

        this._txtNumeroFattura = <Telerik.Web.UI.RadTextBox>$find(this.txtNumeroFatturaId);
        this._txtImporto = <Telerik.Web.UI.RadTextBox>$find(this.txtImportoId);
        this._txtDenominazioneManual = <Telerik.Web.UI.RadTextBox>$find(this.txtDenominazioneManualId);
        this._txtYear = <Telerik.Web.UI.RadTextBox>$find(this.txtYearId);
        this._dtpDataIvaFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dtpDataIvaFromId);
        this._dtpDataIvaTo = <Telerik.Web.UI.RadDatePicker>$find(this.dtpDataIvaToId);
        this._dtpDataReciveSDIFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dtpDataReciveSDIFromId);
        this._dtpDataReciveSDITo = <Telerik.Web.UI.RadDatePicker>$find(this.dtpDataReciveSDIToId);
        this._dtpDataAcceptFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dtpDataAcceptFromId);
        this._dtpDataAcceptTo = <Telerik.Web.UI.RadDatePicker>$find(this.dtpDataAcceptToId);
        this._txtPecMail = <Telerik.Web.UI.RadTextBox>$find(this.txtIndirizzoPECId);

        this._ltlLabelPecMail = $("#ltlPecMail");
        this._ltlchktxtPecMail = $("#ltlchktxtPecMail");
        this._emptyPecMailCheckbox = $("#chktxtPecMail");

        this.cleanSearchFilters();
        this.invoiceSelections = [];
        this.loadData();
    }

    setdate() {
        var endDate = new Date();
        var startDate = new Date();
        startDate.setDate(endDate.getDate() - 30);
        this._dpStartDate.set_selectedDate(startDate);
        this._dpEndDate.set_selectedDate(endDate);
    }

    loadData(): void {
        this.loadUDSInvoiceTipology().done(() => {
            this.setComboValues();
        }).fail((exception) => {
            this.showNotificationException(this.cmdRepositoriNameId, exception, "Errore nel caricamento dei dati.");
        });
    }

    loadUDSInvoiceTipology(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._loadingPanel.show(this.cmdRepositoriNameId);
            this._cmdRepositoriName.clearSelection();
            this.udsRepositoryService.getUDSRepositoryByUDSTypologyName(this.udsInvoiceTypology, this.tenantCompanyName,
                (data) => {
                    if (!data) return;
                    let thisCmbRepositoriName = this._cmdRepositoriName;
                    var cmbItem: Telerik.Web.UI.RadComboBoxItem = null;
                    $.each(data, function (i, value: UDSRepositoryModel) {
                        cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                        cmbItem.set_text(value.Name);
                        cmbItem.set_value(value.UniqueId);
                        thisCmbRepositoriName.get_items().add(cmbItem);
                    });
                    this._loadingPanel.hide(this.cmdRepositoriNameId);
                    promise.resolve();
                },
                (exception: ExceptionDto) => {
                    this._loadingPanel.hide(this.cmdRepositoriNameId);
                    $("#".concat(this.cmdRepositoriNameId)).hide();
                });

        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    setComboValues() {
        let tipoarchivioId: string = '';
        let tipoarchivioName: string = '';
        let tipoarchiviofinder: string = '';
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
        let ddlRepositoriName: Telerik.Web.UI.RadComboBoxItem;
        let thisCmbRepositoriName = this._cmdRepositoriName;
        tipoarchiviofinder = tipoarchivioName;
        if (this.tenantCompanyName != "") {
            tipoarchiviofinder = this.tenantCompanyName + " - " + tipoarchivioName;
        }
        ddlRepositoriName = thisCmbRepositoriName.findItemByText(tipoarchiviofinder);
        var enablerepository: boolean = true;
        if (tipoarchivioName == "") {
            this.setLastRepositoriSearchFilter(UDSInvoiceSearch.UDSINVOICE_FILTER_KEY);
            if (this._cmdRepositoriName && this._cmdRepositoriName.get_selectedItem() !== null) {
                tipoarchivioId = this._cmdRepositoriName.get_selectedItem().get_value();
                tipoarchivioName = this._cmdRepositoriName.get_selectedItem().get_text();
            }
            ddlRepositoriName = thisCmbRepositoriName.findItemByValue(tipoarchivioId);
        } else {
            ddlRepositoriName = this._cmdRepositoriName.findItemByText(tipoarchiviofinder);
            if (ddlRepositoriName) {
                ddlRepositoriName.select();
            }
            enablerepository = false;
        }

        if (tipoarchivioName != "" && ddlRepositoriName != null) {
            this.setdate();
            this.loadUDSInvoiceTipologyByID(ddlRepositoriName.get_value()).done(() => {
                this.setLastSearchFilter(tipoarchiviofinder);
                if (enablerepository) {
                    this._cmdRepositoriName.enable();
                    this._cmbStato.enable();
                } else {
                    this._cmdRepositoriName.disable();
                    if (this.invoiceStatus != "") {
                        this.setComboState(this.invoiceStatus);
                    }
                    this._cmbStato.disable();
                }
                this.loadUDSInvoiceGrid();
            }).fail((exception) => {
                this.showNotificationException(this.cmdRepositoriNameId, exception, "Errore nel caricamento dei dati.");
            });
        }
    }

    setComboState(valore: string) {
        let selectedItem: Telerik.Web.UI.RadComboBoxItem = this._cmbStato.findItemByText(valore);
        if (selectedItem) {
            selectedItem.select();
        }
        this._cmbStato.trackChanges();
    }

    loadUDSInvoiceTipologyByID(udsid: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this.udsRepositoryService.getUDSRepositoryByID(udsid,
                (data: UDSRepositoryModel) => {
                    if (!data) return;
                    var modxml = data[0].ModuleXML;
                    let xmlDoc = $.parseXML(modxml);
                    let $xml = $(xmlDoc);
                    let status: string[] = [];
                    $xml.find("Status>Options>State").map((i, el) => {
                        status.push($(el).text());
                    });
                    this.loadStatus(status).done(() => {
                        promise.resolve();
                    });
                },
                (exception: ExceptionDto) => {
                    this._loadingPanel.hide(this.cmdRepositoriNameId);
                    $("#".concat(this.cmdRepositoriNameId)).hide();
                    this.showNotificationException(this.cmdRepositoriNameId, exception, "Errore nel caricamento dei dati.");
                    promise.reject(exception);
                });

        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    cmdRepositoriName_OnSelectedIndexChange = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let selectedItem: Telerik.Web.UI.RadComboBoxItem = sender.get_selectedItem();
        if (selectedItem != null && selectedItem.get_value() != "") {
            this.loadUDSInvoiceTipologyByID(selectedItem.get_value()).done(() => {
                let isReceivableInvoice = selectedItem.get_text().endsWith(InvoiceRepositories.B2BReceivable.toString());
                this._ltlLabelPecMail.hide();
                this._ltlchktxtPecMail.hide();
                if (isReceivableInvoice) {
                    this._ltlLabelPecMail.show();
                    this._ltlchktxtPecMail.show();
                }
                document.getElementById("ltlSupplier").hidden = !isReceivableInvoice;
                document.getElementById("ltlCustomer").hidden = isReceivableInvoice;
                if (this._btnInvoiceMove) {
                    this._btnInvoiceMove.set_enabled(isReceivableInvoice);
                }
                this._btnInvoiceDelete.set_visible(!isReceivableInvoice);
                let tipoarchivioName: string = selectedItem.get_text();
                this.setLastSearchFilter(tipoarchivioName);
                return;
            });
        }
    }

    btnSearch_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.loadResults(0);
    }
    /**
   * Apre una nuova nuova RadWindow
   * @param url
   * @param name
   * @param width
   * @param height
   */
    openWindow(url, name, width, height): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }

    btnUpload_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        let url: string = 'UDSInvoicesUpload.aspx?Type=UDS';
        this.openWindow(url, 'managerUploadDocument', 770, 450);
    }


    btnInvoiceMoved_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        let url: string = 'UDSInvoiceMove.aspx?Type=UDS&IdUDS='.concat(sender.get_value());
        this.openWindow(url, 'managerUploadDocument', 770, 450);
    }


    btnInvoiceDelete_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        let url: string = 'UDSInvoiceDelete.aspx?Type=UDS&IdUDS='.concat(sender.get_value());
        this.openWindow(url, 'managerUploadDocument', 770, 450);
    }


    btnClean_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.cleanSearchFilters();
    }

    private loadUDSInvoiceGrid() {
        if (!jQuery.isEmptyObject(this._udsInvoiveGridGrid)) {
            this.loadResults(0);
        }
    }

    loadStatus(status: string[]): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._loadingPanel.show(this.cmbStatoId);
            let cmbItem: Telerik.Web.UI.RadComboBoxItem = null;
            this._cmbStato.clearItems();
            let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
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
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    private _buildUdsService(): UDSService {
        let selectedInvoiceItem: Telerik.Web.UI.RadComboBoxItem = this._cmdRepositoriName.get_selectedItem();

        if (!selectedInvoiceItem) {
            return undefined;
        }

        let UDSInvoice_TYPE_NAME = selectedInvoiceItem.get_text();
        let UDSInvoiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfiguration, UDSInvoice_TYPE_NAME);

        return new UDSService(UDSInvoiceConfiguration);;
    }

    loadResults(skip: number) {
        let udsService: UDSService = this._buildUdsService();

        if (!udsService) {
            alert("Nessun nome di archivio selezionato");
            return;
        }

        let cmdRepositoryName: string = "";
        let cmdRepositoryId: string = "";
        if (this._cmdRepositoriName && this._cmdRepositoriName.get_selectedItem() !== null) {
            cmdRepositoryName = this._cmdRepositoriName.get_selectedItem().get_text();
            cmdRepositoryId = this._cmdRepositoriName.get_selectedItem().get_value();
        }

        let statofatturafilter: string = "";
        if (this._cmbStato && this._cmbStato.get_selectedItem() !== null) {
            statofatturafilter = this._cmbStato.get_selectedItem().get_value();
        }
        let startDateFromFilter: string = "";
        if (this._dpStartDate && this._dpStartDate.get_selectedDate()) {
            startDateFromFilter = moment(this._dpStartDate.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
        }
        let endDateFromFilter: string = "";
        if (this._dpEndDate && this._dpEndDate.get_selectedDate()) {
            endDateFromFilter = moment(this._dpEndDate.get_selectedDate()).add(1, 'days').add(-1, 'seconds').format("YYYY-MM-DDTHH:mm:ss[Z]");
        }

        let numerofatturafilter: string = this._txtNumeroFattura.get_value();
        let numerofatturafilterEq: boolean = !$("#chkNumeroFatturafilter").is(":checked");
        let importoFilter: string = this._txtImporto.get_value();
        let denomiazioneFilter: string = this._txtDenominazioneManual.get_value();
        let denomiazioneFilterEq: boolean = !$("#chkDenominazioneManualfilter").is(":checked");

        let annoivaFilter: string = this._txtYear.get_value();

        let dataIvaFromFilter: string = "";
        if (this._dtpDataIvaFrom && this._dtpDataIvaFrom.get_selectedDate()) {
            dataIvaFromFilter = moment(this._dtpDataIvaFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
        }

        let dataIvaToFilter: string = "";
        if (this._dtpDataIvaTo && this._dtpDataIvaTo.get_selectedDate()) {
            dataIvaToFilter = moment(this._dtpDataIvaTo.get_selectedDate()).add(1, 'days').add(-1, 'seconds').format("YYYY-MM-DDTHH:mm:ss[Z]");
        }

        let dataReceivedFromFilter: string = "";
        if (this._dtpDataReciveSDIFrom && this._dtpDataReciveSDIFrom.get_selectedDate()) {
            dataReceivedFromFilter = moment(this._dtpDataReciveSDIFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
        }

        let dataReceivedToFilter: string = "";
        if (this._dtpDataReciveSDITo && this._dtpDataReciveSDITo.get_selectedDate()) {
            dataReceivedToFilter = moment(this._dtpDataReciveSDITo.get_selectedDate()).add(1, 'days').add(-1, 'seconds').format("YYYY-MM-DDTHH:mm:ss[Z]");
        }

        let dataacceptFromFilter: string = "";
        if (this._dtpDataAcceptFrom && this._dtpDataAcceptFrom.get_selectedDate()) {
            dataacceptFromFilter = moment(this._dtpDataAcceptFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
        }

        let dataacceptToFilter: string = "";
        if (this._dtpDataAcceptTo && this._dtpDataAcceptTo.get_selectedDate()) {
            dataacceptToFilter = moment(this._dtpDataAcceptTo.get_selectedDate()).add(1, 'days').add(-1, 'seconds').format("YYYY-MM-DDTHH:mm:ss[Z]");
        }
        let pecMailBoxFilter: string = this._txtPecMail.get_textBoxValue();
        let pecMailBoxFilterEq: boolean = this._emptyPecMailCheckbox.is(":checked");

        let searchDTO: UDSInvoiceSearchFilterDTO = new UDSInvoiceSearchFilterDTO();

        searchDTO.cmdRepositoriName = cmdRepositoryId;
        searchDTO.startDateFromFilter = startDateFromFilter;
        searchDTO.endDateFromFilter = endDateFromFilter;
        searchDTO.numerofatturafilter = numerofatturafilter;
        searchDTO.numerofatturafilterEq = numerofatturafilterEq;
        searchDTO.importoFilter = importoFilter;
        searchDTO.denomiazioneFilter = denomiazioneFilter;
        searchDTO.denomiazioneFilterEq = denomiazioneFilterEq;
        searchDTO.annoivaFilter = annoivaFilter;


        searchDTO.dataReceivedFromFilter = dataReceivedFromFilter;
        searchDTO.dataReceivedToFilter = dataReceivedToFilter;

        searchDTO.dataIvaFromFilter = dataIvaFromFilter;
        searchDTO.dataIvaToFilter = dataIvaToFilter;

        searchDTO.dataacceptFromFilter = dataacceptFromFilter;
        searchDTO.dataacceptToFilter = dataacceptToFilter;

        searchDTO.statofatturaFilter = statofatturafilter;
        searchDTO.pecMailBoxFilter = pecMailBoxFilter;
        searchDTO.pecMailBoxFilterEq = pecMailBoxFilterEq;

        this._saveValueToSessionStorage(cmdRepositoryName, JSON.stringify(searchDTO));
        this._saveValueToSessionStorage(UDSInvoiceSearch.UDSINVOICE_FILTER_KEY, JSON.stringify(searchDTO));

        var sortExpressions = this._masterTableView.get_sortExpressions();
        var orderbyExpressions;
        orderbyExpressions = sortExpressions.toString();
        if (orderbyExpressions == "") {
            orderbyExpressions = "NumeroFattura asc";
        }
        let top = this._masterTableView.get_pageSize();

        let setVisiblePecMail: boolean = $("#ltlPecMail").is(":visible");
        if (setVisiblePecMail) {
            this._masterTableView.showColumn(13);//colonna indirizzo pec
            var gridSorter;
            gridSorter = sortExpressions.toString();
            if (gridSorter == "") {
                orderbyExpressions = "DataRicezioneSdi asc";
            }
        }
        else {
            this._masterTableView.hideColumn(13);//colonna indirizzo pec
        }

        try {
            this._loadingPanel.show(this.udsInvoiceGridId);
            udsService.getUDSInvoices(searchDTO, top, skip, orderbyExpressions, (data) => {
                if (!data) return;
                this.gridResult = data;
                this._masterTableView.set_dataSource(data.value);
                this._masterTableView.set_virtualItemCount(data.count);
                this._masterTableView.dataBind();
                this._loadingPanel.hide(this.udsInvoiceGridId);
            },
                (exception: ExceptionDto) => {
                    this._loadingPanel.hide(this.udsInvoiceGridId);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            this._loadingPanel.hide(this.udsInvoiceGridId);
        }
    }

    private _saveValueToSessionStorage(identifier: string, value: string): void {
        let cachedItemKey: string = `${UDSInvoiceSearch.FILTER_CACHE_KEY}_${identifier}`;

        sessionStorage.setItem(cachedItemKey, value);
    }

    private _getCachedValueFromSession(identifier: string): string {
        let cachedItemKey: string = `${UDSInvoiceSearch.FILTER_CACHE_KEY}_${identifier}`;

        return sessionStorage.getItem(cachedItemKey);
    }

    setLastRepositoriSearchFilter(tipoarchivioName: string) {
        let UDSinvoiceLastSearch: string = this._getCachedValueFromSession(tipoarchivioName);
        if (UDSinvoiceLastSearch == null) {
            UDSinvoiceLastSearch = this._getCachedValueFromSession(UDSInvoiceSearch.UDSINVOICE_FILTER_KEY);
        }
        if (UDSinvoiceLastSearch) {
            let lastsearchFilter: UDSInvoiceSearchFilterDTO = <UDSInvoiceSearchFilterDTO>JSON.parse(UDSinvoiceLastSearch);
            if (lastsearchFilter.cmdRepositoriName) {
                let selectedItem: Telerik.Web.UI.RadComboBoxItem = this._cmdRepositoriName.findItemByValue(lastsearchFilter.cmdRepositoriName.toString());
                selectedItem.select();
                this._cmdRepositoriName.trackChanges();
            }
        }
    }

    setLastSearchFilter(tipoarchivioName: string) {

        let UDSinvoiceLastSearch: string = this._getCachedValueFromSession(tipoarchivioName);
        if (UDSinvoiceLastSearch == null) {
            UDSinvoiceLastSearch = this._getCachedValueFromSession(UDSInvoiceSearch.UDSINVOICE_FILTER_KEY);
        }

        if (UDSinvoiceLastSearch) {
            let lastsearchFilter: UDSInvoiceSearchFilterDTO = <UDSInvoiceSearchFilterDTO>JSON.parse(UDSinvoiceLastSearch);
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
            let lastsearchFilterStatofattura: string;

            if (lastsearchFilter.statofatturaFilter) {
                lastsearchFilterStatofattura = lastsearchFilter.statofatturaFilter;
                this.setComboState(lastsearchFilterStatofattura);
            }

        }
    }
    onPageChanged() {
        let skip: number = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
        this.loadResults(skip);
    }

    private _clearSessionFilterCache(): void {
        let sessionStorageKeys: string[] = Object.keys(sessionStorage);

        sessionStorageKeys.forEach((sessionKey: string) => {
            if (sessionKey.startsWith(UDSInvoiceSearch.FILTER_CACHE_KEY)) {
                sessionStorage.removeItem(sessionKey);
            }
        });
    }

    cleanSearchFilters = () => {
        this._clearSessionFilterCache();
        this._dpStartDate.clear();
        this._dpEndDate.clear();
        if (this._cmdRepositoriName.get_enabled()) {
            this._cmdRepositoriName.clearSelection();
            this._cmbStato.clearSelection();
        }

        this._txtNumeroFattura.clear();
        this._txtImporto.clear();
        this._txtYear.clear();
        this._txtDenominazioneManual.clear();

        this._dtpDataAcceptFrom.clear();
        this._dtpDataAcceptTo.clear();

        this._dtpDataIvaFrom.clear();
        this._dtpDataIvaTo.clear();

        this._dtpDataReciveSDIFrom.clear();
        this._dtpDataReciveSDITo.clear();

        this._txtPecMail.clear();
        this._emptyPecMailCheckbox.prop("checked", false);
    }

    //region [ Grid Configuration Methods ]

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

    changeEnableBtn(setEnable: boolean, setvalue: string) {
        if (setEnable) {
            this._btnInvoiceDelete.set_value(setvalue);
            if (this._btnInvoiceMove) {
                this._btnInvoiceMove.set_value(setvalue);
            }
        } else {
            this._btnInvoiceDelete.set_value("");
            if (this._btnInvoiceMove) {
                this._btnInvoiceMove.set_value("");
            }
        }
        this._btnInvoiceDelete.set_enabled(setEnable);
        if (this._btnInvoiceMove) {
            let UDSInvoice_TYPE_NAME: string = this._cmdRepositoriName.get_selectedItem().get_text();
            let isReceivableInvoice: boolean = UDSInvoice_TYPE_NAME.endsWith(InvoiceRepositories.B2BReceivable.toString());
            this._btnInvoiceMove.set_enabled(setEnable && isReceivableInvoice);
        }
    }

    saveInvoiceSelectionsToSessionStorage(checkboxes) {

        for (let checkbox of checkboxes) {
            var index = this.invoiceSelections.indexOf(checkbox.value)
            if (checkbox.checked && index === -1) {
                this.invoiceSelections.push(checkbox.value);
            }
            else if (!checkbox.checked && index > -1) {
                this.invoiceSelections.splice(index, 1);
            }
        }
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_INVOICE_SELECTIONS, JSON.stringify(this.invoiceSelections));
    }
}
//endregion

export = UDSInvoiceSearch;

enum InvoiceRepositories {
    B2BSendable = "Fatture Attive",
    PASendable = "Fatture Attive PA",
    B2BReceivable = "Fatture Passive"
}