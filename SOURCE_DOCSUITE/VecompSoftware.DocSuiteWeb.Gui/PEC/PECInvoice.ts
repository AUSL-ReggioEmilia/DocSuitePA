/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import PecInvoiceBase = require("./PECInvoiceBase");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import PECMailSearchFilterDTO = require('App/DTOs/PECMailSearchFilterDTO');
import ExceptionDto = require("App/DTOs/ExceptionDTO");
import PECMailInvoiceGridViewModel = require("App/ViewModels/PECMails/PECMailInvoiceGridViewModel");
import PECMailBoxViewModel = require("App/ViewModels/PECMails/PECMailBoxViewModel");
import InvoiceTypeEnum = require("App/Models/PECMails/InvoiceTypeEnum");
import EnumHelper = require("App/Helpers/EnumHelper");
import PECMailViewModel = require('App/ViewModels/PECMails/PECMailViewModel');
import InvoiceStatusEnum = require("App/Models/PECMails/InvoiceStatusEnum");
import PECMailDirection = require("App/Models/PECMails/PECMailDirection");
import PECMailViewModelMapper = require('App/Mappers/PECMails/PECMailViewModelMapper');
import TenantViewModel = require('App/ViewModels/Tenants/TenantViewModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import PECMailBoxModel = require('App/Models/PECMails/PECMailBoxModel');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');


abstract class PECInvoice extends PecInvoiceBase {

    pecInvoiveGridId: string;
    ajaxLoadingPanelId: string;
    btnSearchId: string;
    btnCleanId: string;
    btnTenantsId: string;
    btnContainerSelectorOkId: string;
    dpStartDateFromId: string;
    dpEndDateFromId: string;
    cmbPecMailBoxId: string;
    txtMittenteId: string;
    txtDestinararioId: string;
    cmbStatoId: string;
    cmbTipologiaFatturaId: string;
    cmbSelectPecMailBoxId: string;
    cmbWorkflowRepositoriesId: string;
    direction: string;
    invoiceType: string;
    pecInvoiceImageId: string;
    rwTenantSelectorId: string;
    cmbTenantsComboId: string;
    private selectedPECMail: PECMailViewModel;
    selectedPECMailId: number;
    maxNumberElements: string;
    gridResult: PECMailInvoiceGridViewModel[];
    pecMailBoxes: PECMailBoxViewModel[];
    pecInvoiceDirection: PECMailDirection;

    private _dpStartDateFrom: Telerik.Web.UI.RadDatePicker;
    private _dpEndDateFrom: Telerik.Web.UI.RadDatePicker;
    private _cmbPecMailBox: Telerik.Web.UI.RadComboBox;
    private _txtMittente: Telerik.Web.UI.RadTextBox;
    private _txtDestinatario: Telerik.Web.UI.RadTextBox;
    private _cmbStato: Telerik.Web.UI.RadComboBox;
    private _cmbTipologiaFattura: Telerik.Web.UI.RadComboBox;
    private _cmbSelectPecMailBox: Telerik.Web.UI.RadComboBox;
    private _cmbWorkflowRepositories: Telerik.Web.UI.RadComboBox;
    private _cmbTenantsCombo: Telerik.Web.UI.RadComboBox;

    private _btnSearch: Telerik.Web.UI.RadButton;
    private _btnClean: Telerik.Web.UI.RadButton;
    private _btnTenants: Telerik.Web.UI.RadButton;
    private _rwTenants: Telerik.Web.UI.RadWindow;
    private _btnContainerSelectorOkId: Telerik.Web.UI.RadButton;

    public static LOADED_EVENT: string = "onLoaded";
    public static PAGE_CHANGED_EVENT: string = "onPageChanged";

    private _pecInvoiveGridGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;
    private _enumHelper: EnumHelper;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _serviceConfiguration: ServiceConfiguration[];


    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(serviceConfigurations);
        this._serviceConfiguration = serviceConfigurations;
        this._enumHelper = new EnumHelper();
        $(document).ready(() => {

        });
    }

    initialize() {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        this._pecInvoiveGridGrid = <Telerik.Web.UI.RadGrid>$find(this.pecInvoiveGridId);
        this._masterTableView = this._pecInvoiveGridGrid.get_masterTableView();
        this.pecInvoiceDirection = this.direction === "1" ? PECMailDirection.Outgoing : PECMailDirection.Incoming;

        $("#".concat(this.pecInvoiveGridId)).bind(PECInvoice.LOADED_EVENT,
            () => {
                this.loadPECInvoiceGrid();
            });

        this._dpStartDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpStartDateFromId);
        this._dpEndDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpEndDateFromId);
        this._cmbPecMailBox = <Telerik.Web.UI.RadComboBox>$find(this.cmbPecMailBoxId);
        this._cmbStato = <Telerik.Web.UI.RadComboBox>$find(this.cmbStatoId);
        this._cmbTipologiaFattura = <Telerik.Web.UI.RadComboBox>$find(this.cmbTipologiaFatturaId);
        this._txtMittente = <Telerik.Web.UI.RadTextBox>$find(this.txtMittenteId);
        this._txtDestinatario = <Telerik.Web.UI.RadTextBox>$find(this.txtDestinararioId);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicking(this.btnSearch_onClick);
        this._btnClean = <Telerik.Web.UI.RadButton>$find(this.btnCleanId);
        this._btnClean.add_clicking(this.btnClean_onClick);
        this._btnTenants = <Telerik.Web.UI.RadButton>$find(this.btnTenantsId);
        this._btnTenants.add_clicking(this.btnTenants_onClick);
        this._btnContainerSelectorOkId = <Telerik.Web.UI.RadButton>$find(this.btnContainerSelectorOkId);
        this._btnContainerSelectorOkId.add_clicking(this.btnContainerSelectorOkId_onClick);
        this._rwTenants = <Telerik.Web.UI.RadWindow>$find(this.rwTenantSelectorId);
        this._cmbSelectPecMailBox = <Telerik.Web.UI.RadComboBox>$find(this.cmbSelectPecMailBoxId);
        this._cmbSelectPecMailBox.add_itemsRequested(this._cmbSelectPecMailBox_OnClientItemsRequested);
        this._cmbWorkflowRepositories = <Telerik.Web.UI.RadComboBox>$find(this.cmbWorkflowRepositoriesId);
        this._cmbWorkflowRepositories.add_itemsRequested(this._cmbWorkflowRepositories_OnClientItemsRequested);
        this._cmbTenantsCombo = <Telerik.Web.UI.RadComboBox>$find(this.cmbTenantsComboId);
        this._cmbTenantsCombo.add_itemsRequested(this._cmbTenantsCombo_OnClientItemsRequested);



        // add empty values for status and invoice type combos
        this.addEmptyValuesToCombos();
        // load combo filters
        this.loadStatus();
        this.loadInvoiceType();
        this.loadPECMailBoxes();
        this._btnTenants.set_visible(false);
        this._btnContainerSelectorOkId.set_enabled(false);
        this._btnTenants.set_enabled(false);
        //constrain comboxes for each page
        this.constrainComboboxesOnPages();

        // load grid data
        this.loadPECInvoiceGrid();

    }


    constrainComboboxesOnPages() {
        let qs = this.parse_query_string(window.location.href);
        let param = qs["InvoiceType"];
        switch (param) {
            case "B2BSendableSDI":
                this.constrainComboboxes("Consegnata", "Fattura tra privati attiva");
                break;
            case "B2BSendableToSDI":
                this.constrainComboboxes("Flusso di lavoro non avviato", "Fattura tra privati attiva");
                break;
            case "B2BInErroreAttive":
                {
                    this.constrainComboboxes("Flusso di lavoro in errore", "Fattura tra privati attiva");
                    this._btnTenants.set_visible(true);
                    break;
                }
            case "B2BInLavorazioneAttive":
                this.constrainComboboxes("Flusso di lavoro avviato", "Fattura tra privati attiva");
                break;
            case "B2BPayableFromSDI":
                this.constrainComboboxes("Consegnata", "Fattura tra privati passiva");
                break;
            case "B2BPayableToERP":
                this.constrainComboboxes("In attesa registrazione contabile", "Fattura tra privati passiva");
                break;
            case "B2BPayableFromERP":
                this.constrainComboboxes("Contabilizzata", "Fattura tra privati passiva");
                break;
            case "B2BInErrorePassive":
                {
                    this.constrainComboboxes("Flusso di lavoro in errore", "Fattura tra privati passiva");
                    this._btnTenants.set_visible(true);
                    this._pecInvoiveGridGrid.get_allowMultiRowSelection();
                    break;
                }
            case "B2BInLavorazionePassive":
                this.constrainComboboxes("Flusso di lavoro avviato", "Fattura tra privati passiva");
                break;
        }
    }

    private parse_query_string(query): any {
        var vars = query.split("&");
        var query_string = {};
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            var key = decodeURIComponent(pair[0]);
            var value = decodeURIComponent(pair[1]);
            if (typeof query_string[key] === "undefined") {
                query_string[key] = decodeURIComponent(value);
            } else if (typeof query_string[key] === "string") {
                var arr = [query_string[key], decodeURIComponent(value)];
                query_string[key] = arr;
            } else {
                query_string[key].push(decodeURIComponent(value));
            }
        }
        return query_string;
    }

    private constrainComboboxes(status: string, fattura: string) {

        let stato: Telerik.Web.UI.RadComboBoxItem = this._cmbStato.findItemByText(status);
        stato.select();

        let tipologia: Telerik.Web.UI.RadComboBoxItem = this._cmbTipologiaFattura.findItemByText(fattura);
        tipologia.select();

        this._cmbStato.disable();
        this._cmbTipologiaFattura.disable();

    }

    btnSearch_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.loadResults();
    }

    btnClean_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.cleanSearchFilters();
    }

    btnTenants_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._rwTenants.show();
    }

    private loadPECInvoiceGrid() {
        if (!jQuery.isEmptyObject(this._pecInvoiveGridGrid)) {
            this.loadResults();
        }
    }

    addEmptyValuesToCombos() {
        let cmbItemStato: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
        cmbItemStato.set_text("");
        cmbItemStato.set_value("");
        this._cmbStato.get_items().add(cmbItemStato);
        let cmbItemTipologiaFattura: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
        cmbItemTipologiaFattura.set_text("");
        cmbItemTipologiaFattura.set_value("");
        this._cmbTipologiaFattura.get_items().add(cmbItemTipologiaFattura);
    }

    loadStatus() {
        this._loadingPanel.show(this.cmbStatoId);
        let cmbItem: Telerik.Web.UI.RadComboBoxItem = null;
        for (var n in InvoiceStatusEnum) {
            if (typeof InvoiceStatusEnum[n] === 'string') {
                cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                cmbItem.set_text(this._enumHelper.getInvoiceStatusDescription(InvoiceStatusEnum[n]));
                cmbItem.set_value(<any>InvoiceStatusEnum[n]);
                this._cmbStato.get_items().add(cmbItem);
            }
        }


        this._loadingPanel.hide(this.cmbStatoId);
    }

    loadInvoiceType() {
        this._loadingPanel.show(this.cmbTipologiaFatturaId);
        for (var n in InvoiceTypeEnum) {
            if (typeof InvoiceTypeEnum[n] === 'string' && InvoiceTypeEnum[n] !== "None") {
                let cmbItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                cmbItem.set_text(
                    this._enumHelper.getInvoiceTypeDescription(InvoiceTypeEnum[n], this.pecInvoiceDirection));
                cmbItem.set_value(<any>InvoiceTypeEnum[n]);
                this._cmbTipologiaFattura.get_items().add(cmbItem);
            }
        }
        this._loadingPanel.hide(this.cmbTipologiaFatturaId);
    }

    loadPECMailBoxes() {
        this._loadingPanel.show(this.cmbPecMailBoxId);

        this._pecMailBoxService.getFilteredPECMailBoxes(
            (data) => {
                if (!data) return;
                this.pecMailBoxes = data;

                let thisCmbPecMailBox = this._cmbPecMailBox;
                var cmbItem: Telerik.Web.UI.RadComboBoxItem = null;
                $.each(this.pecMailBoxes, function (i, value: PECMailBoxViewModel) {
                    cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(value.MailBoxRecipient);
                    cmbItem.set_value(value.EntityShortId.toString());
                    thisCmbPecMailBox.get_items().add(cmbItem);
                });

                cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                cmbItem.set_text("Tutte");
                thisCmbPecMailBox.get_items().add(cmbItem);

                this._loadingPanel.hide(this.cmbPecMailBoxId);
            },
            (exception: ExceptionDto) => {
                this._loadingPanel.hide(this.cmbPecMailBoxId);
                $("#".concat(this.cmbPecMailBoxId)).hide();
            });
    }

    loadResults() {

        this._loadingPanel.show(this.pecInvoiveGridId);

        let startDateFromFilter: string = "";
        if (this._dpStartDateFrom && this._dpStartDateFrom.get_selectedDate()) {
            startDateFromFilter = moment(this._dpStartDateFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
        }
        let endDateFromFilter: string = "";
        if (this._dpEndDateFrom && this._dpEndDateFrom.get_selectedDate()) {
            endDateFromFilter = moment(this._dpEndDateFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
        }
        let cmbPecMailBoxId: string = "";
        if (this._cmbPecMailBox && this._cmbPecMailBox.get_selectedItem() !== null) {
            cmbPecMailBoxId = this._cmbPecMailBox.get_selectedItem().get_value();
        }
        let cmbStatoId: string = "";
        if (this._cmbStato && this._cmbStato.get_selectedItem() !== null) {
            cmbStatoId = this._cmbStato.get_selectedItem().get_value();
        }
        let cmbTipologiaFatturaId: string = "";
        if (this._cmbTipologiaFattura && this._cmbTipologiaFattura.get_selectedItem() !== null) {
            cmbTipologiaFatturaId = this._cmbTipologiaFattura.get_selectedItem().get_value();
        }
        let mittenteFilter: string = "";
        if (this._txtMittente && this._txtMittente.get_textBoxValue() !== "") {
            mittenteFilter = this._txtMittente.get_textBoxValue();
        }
        let destinatarioFilter: string = "";
        if (this._txtDestinatario && this._txtDestinatario.get_textBoxValue() !== "") {
            destinatarioFilter = this._txtDestinatario.get_textBoxValue();
        }

        let searchDTO: PECMailSearchFilterDTO = new PECMailSearchFilterDTO();
        searchDTO.direction = this.pecInvoiceDirection;
        searchDTO.dateFrom = startDateFromFilter;
        searchDTO.dateTo = endDateFromFilter;
        searchDTO.pecMailBox = cmbPecMailBoxId;
        searchDTO.invoiceStatus = cmbStatoId;
        searchDTO.invoiceType = cmbTipologiaFatturaId;
        searchDTO.mailSenders = mittenteFilter;
        searchDTO.mailRecipients = destinatarioFilter;

        this._pecMailService.getPECMails(searchDTO,
            (data) => {
                if (!data) return;
                this.gridResult = data;
                this._masterTableView.set_dataSource(this.gridResult);
                this._masterTableView.dataBind();

                for (let rowIndex = 0; rowIndex < this._masterTableView.get_dataItems().length; rowIndex++) {
                    //set links for subject
                    this._masterTableView
                        .getCellByColumnUniqueName(this._masterTableView.get_dataItems()[rowIndex], "MailSubject")
                        .innerHTML =
                        "<a runat=\"server\" href=\"PECSummary.aspx?Type=PEC&PECId=" +
                        data[rowIndex].EntityId +
                        "\">" +
                        data[rowIndex].MailSubject +
                        "</a>";

                    // set the correct image based on invoice status
                    let imgSrcBasedOnStatus =
                        this._masterTableView.getCellByColumnUniqueName(this._masterTableView.get_dataItems()[rowIndex],
                            "Icona");

                    // TODO Look at this images and see if are correct. If there aren't correct where to take them ? I try to put correct image based on the comments find in US 15036
                    //icona colorata che rappresenta lo stato della PEC (non inviato (grigio), inviato (giallo), da consegnare (verde chiaro), consegnato (blu), in attesa contabile (blu scuro), in errore (rosso)

                    switch (data[rowIndex].InvoiceStatus) {
                        case InvoiceStatusEnum[InvoiceStatusEnum.None]: // non inviato
                            imgSrcBasedOnStatus.innerHTML =
                                "<img runat=\"server\" src=\"../App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png\"/>";
                        case InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceSent]: // inviato
                            imgSrcBasedOnStatus.innerHTML =
                                "<img runat=\"server\" src=\"../App_Themes/DocSuite2008/imgset16/StatusSecurityWarning_16x.png\"/>";
                        case InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceNotified]: // in attesa registrazione contabile
                            imgSrcBasedOnStatus.innerHTML =
                                "<img runat=\"server\" src=\"../App_Themes/DocSuite2008/imgset16/information.png\"/>";
                        case InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceAccepted]: // consegnata
                            imgSrcBasedOnStatus.innerHTML =
                                "<img runat=\"server\" src=\"../App_Themes/DocSuite2008/imgset16/network-share.png\"/>";
                        case InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceSdiRefused]: // scartata dallo SDI
                            imgSrcBasedOnStatus.innerHTML =
                                "<img runat=\"server\" src=\"../App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png\"/>";
                        case InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceRefused]: // rifiutata
                            imgSrcBasedOnStatus.innerHTML =
                                "<img runat=\"server\" src=\"../App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png\"/>";
                    }
                }

                this._loadingPanel.hide(this.pecInvoiveGridId);
            },
            (exception: ExceptionDto) => {
                this._loadingPanel.hide(this.pecInvoiveGridId);
                $("#".concat(this.pecInvoiveGridId)).hide();

            });

    }

    cleanSearchFilters = () => {
        this._dpStartDateFrom.clear();
        this._dpEndDateFrom.clear();
        this._txtMittente.clear();
        this._txtDestinatario.clear();
        this._cmbPecMailBox.clearSelection();
        if (this._cmbStato.get_enabled()) {
            this._cmbStato.clearSelection();
            this._cmbTipologiaFattura.clearSelection();
        }
    }

    //region [ Grid Configuration Methods ]

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

    onGridRowSelected() {
        var masterTable = this._pecInvoiveGridGrid.get_masterTableView();
        var selectedRow = masterTable.get_selectedItems();
        this._btnTenants.set_enabled(true);
        let viewModelMapper = new PECMailViewModelMapper();
        this.selectedPECMailId = viewModelMapper.Map(selectedRow[0]._dataItem).EntityId;
    }

    btnContainerSelectorOkId_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._pecMailService.getPECMailById(this.selectedPECMailId,
            (data: any) => {
                let pecMail: PECMailViewModel = data;
                this._pecMailBoxService.getPECMailBoxById(
                    Number(this._cmbSelectPecMailBox.get_selectedItem().get_value()),
                    (data: any) => {
                        pecMail.PECMailBox = data[0];
                        this._pecMailService.insertPECMailTenantCorrection(pecMail,
                            (data: any) => {
                                alert("Dati mandati correttamente");
                                this._rwTenants.close();
                            },
                            (exception: ExceptionDto) => {

                            });
                    });
            },
            (exception: ExceptionDto) => {
            });
    }

    _cmbTenantsCombo_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox,
        args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
        let tenantNumberOfItems: number = sender.get_items().get_count();
        this._tenantService.getAllTenants(args.get_text(), this.maxNumberElements, tenantNumberOfItems,
            (data: ODATAResponseModel<TenantViewModel>) => {
                try {
                    this.refreshTenants(data.value);
                    let scrollToPosition: boolean = args.get_domEvent() == undefined;
                    if (scrollToPosition) {
                        if (sender.get_items().get_count() > 0) {
                            let scrollTenant: JQuery = $(sender.get_dropDownElement()).find('div.rcbScroll');
                            scrollTenant.scrollTop($(sender.get_items().getItem(tenantNumberOfItems + 1).get_element()).position().top);
                        }
                    }
                    sender.get_attributes().setAttribute('otherTenantCount', data.count.toString());
                    sender.get_attributes().setAttribute('updating', 'false');
                    if (sender.get_items().get_count() > 0) {
                        tenantNumberOfItems = sender.get_items().get_count() - 1;
                    }
                    this._cmbTenantsCombo.get_moreResultsBoxMessageElement().innerText = `Visualizzati ${tenantNumberOfItems.toString()} di ${data.count.toString()}`;
                }
                catch (error) {
                }
            },
            (exception: ExceptionDto) => {

            });
    }

    refreshTenants = (data: TenantViewModel[]) => {
        if (data.length > 0) {
            this._cmbTenantsCombo.beginUpdate();
            if (this._cmbTenantsCombo.get_items().get_count() === 0) {
                let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._cmbTenantsCombo.get_items().insert(0, emptyItem);
            }

            $.each(data, (index, container) => {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(container.CompanyName);
                item.set_value(container.UniqueId.toString());
                this._cmbTenantsCombo.get_items().add(item);
            });
            this._cmbTenantsCombo.showDropDown();
            this._cmbTenantsCombo.endUpdate();
        }
        else {
            if (this._cmbTenantsCombo.get_items().get_count() === 0) {
            }

        }
    }

    _cmbSelectPecMailBox_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox,
        args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
        let pecMailBoxNumberOfItems: number = sender.get_items().get_count();
        this._tenantService.getAllPECMailBoxes(this._cmbTenantsCombo.get_selectedItem().get_value(), args.get_text(), this.maxNumberElements, pecMailBoxNumberOfItems,
            (data: PECMailBoxModel[]) => {
                try {
                    this.refreshPECMailBox(data);
                    let scrollToPosition: boolean = args.get_domEvent() == undefined;
                    if (scrollToPosition) {
                        if (sender.get_items().get_count() > 0) {
                            let scrollPECMailBox: JQuery = $(sender.get_dropDownElement()).find('div.rcbScroll');
                            scrollPECMailBox.scrollTop($(sender.get_items().getItem(pecMailBoxNumberOfItems + 1).get_element()).position().top);
                        }
                    }
                    sender.get_attributes().setAttribute('otherTenantCount', data.length.toString());
                    sender.get_attributes().setAttribute('updating', 'false');
                    if (sender.get_items().get_count() > 0) {
                        pecMailBoxNumberOfItems = sender.get_items().get_count() - 1;
                    }
                    this._cmbSelectPecMailBox.get_moreResultsBoxMessageElement().innerText = `Visualizzati ${pecMailBoxNumberOfItems.toString()} di ${data.length.toString()}`;
                }
                catch (error) {
                }
            },
            (exception: ExceptionDto) => {

            });
    }

    refreshPECMailBox = (data: PECMailBoxModel[]) => {
        if (data.length > 0) {
            this._cmbSelectPecMailBox.beginUpdate();
            if (this._cmbSelectPecMailBox.get_items().get_count() === 0) {
                let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._cmbSelectPecMailBox.get_items().insert(0, emptyItem);
            }

            $.each(data, (index, container) => {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(container.MailBoxRecipient);
                item.set_value(container.EntityShortId.toString());
                this._cmbSelectPecMailBox.get_items().add(item);
            });
            this._cmbSelectPecMailBox.showDropDown();
            this._cmbSelectPecMailBox.endUpdate();
        }
        else {
            if (this._cmbSelectPecMailBox.get_items().get_count() === 0) {
            }

        }
    }

    _cmbWorkflowRepositories_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox,
        args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
        let tenantWorkflowRepNumberOfItems: number = sender.get_items().get_count();
        this._tenantWorkflowRepositoryService.getAllWorkflowRepositories(this._cmbTenantsCombo.get_selectedItem().get_value(), args.get_text(), this.maxNumberElements, tenantWorkflowRepNumberOfItems,
            (data: WorkflowRepositoryModel[]) => {
                try {
                    this.refreshTenantWorkflowRep(data);
                    let scrollToPosition: boolean = args.get_domEvent() == undefined;
                    if (scrollToPosition) {
                        if (sender.get_items().get_count() > 0) {
                            let scrollWorkflowRepo: JQuery = $(sender.get_dropDownElement()).find('div.rcbScroll');
                            scrollWorkflowRepo.scrollTop($(sender.get_items().getItem(tenantWorkflowRepNumberOfItems + 1).get_element()).position().top);
                        }
                    }
                    sender.get_attributes().setAttribute('otherTenantCount', data.length.toString());
                    sender.get_attributes().setAttribute('updating', 'false');
                    if (sender.get_items().get_count() > 0) {
                        tenantWorkflowRepNumberOfItems = sender.get_items().get_count() - 1;
                    }
                    this._cmbWorkflowRepositories.get_moreResultsBoxMessageElement().innerText = `Visualizzati ${tenantWorkflowRepNumberOfItems.toString()} di ${data.length.toString()}`;
                }
                catch (error) {
                }
            },
            (exception: ExceptionDto) => {

            });
    }

    refreshTenantWorkflowRep = (data: WorkflowRepositoryModel[]) => {
        if (data.length > 0) {
            this._cmbWorkflowRepositories.beginUpdate();
            if (this._cmbWorkflowRepositories.get_items().get_count() === 0) {
                let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._cmbWorkflowRepositories.get_items().insert(0, emptyItem);
            }

            $.each(data, (index, container) => {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(container.Name);
                item.set_value(container.UniqueId.toString());
                this._cmbWorkflowRepositories.get_items().add(item);
            });
            this._cmbWorkflowRepositories.showDropDown();
            this._cmbWorkflowRepositories.endUpdate();
        }
        else {
            if (this._cmbWorkflowRepositories.get_items().get_count() === 0) {
            }

        }
    }

}
//endregion

export = PECInvoice;