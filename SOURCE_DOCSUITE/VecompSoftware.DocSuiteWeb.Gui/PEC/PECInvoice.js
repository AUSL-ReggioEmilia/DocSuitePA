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
define(["require", "exports", "./PECInvoiceBase", "App/DTOs/PECMailSearchFilterDTO", "App/Models/PECMails/InvoiceTypeEnum", "App/Helpers/EnumHelper", "App/Models/PECMails/InvoiceStatusEnum", "App/Models/PECMails/PECMailDirection", "App/Mappers/PECMails/PECMailViewModelMapper"], function (require, exports, PecInvoiceBase, PECMailSearchFilterDTO, InvoiceTypeEnum, EnumHelper, InvoiceStatusEnum, PECMailDirection, PECMailViewModelMapper) {
    var PECInvoice = /** @class */ (function (_super) {
        __extends(PECInvoice, _super);
        function PECInvoice(serviceConfigurations) {
            var _this = _super.call(this, serviceConfigurations) || this;
            _this.btnSearch_onClick = function (sender, args) {
                _this.loadResults();
            };
            _this.btnClean_onClick = function (sender, args) {
                _this.cleanSearchFilters();
            };
            _this.btnTenants_onClick = function (sender, args) {
                _this._rwTenants.show();
            };
            _this.cleanSearchFilters = function () {
                _this._dpStartDateFrom.clear();
                _this._dpEndDateFrom.clear();
                _this._txtMittente.clear();
                _this._txtDestinatario.clear();
                _this._cmbPecMailBox.clearSelection();
                if (_this._cmbStato.get_enabled()) {
                    _this._cmbStato.clearSelection();
                    _this._cmbTipologiaFattura.clearSelection();
                }
            };
            _this.btnContainerSelectorOkId_onClick = function (sender, args) {
                _this._pecMailService.getPECMailById(_this.selectedPECMailId, function (data) {
                    var pecMail = data;
                    _this._pecMailBoxService.getPECMailBoxById(Number(_this._cmbSelectPecMailBox.get_selectedItem().get_value()), function (data) {
                        pecMail.PECMailBox = data[0];
                        _this._pecMailService.insertPECMailTenantCorrection(pecMail, function (data) {
                            alert("Dati mandati correttamente");
                            _this._rwTenants.close();
                        }, function (exception) {
                        });
                    });
                }, function (exception) {
                });
            };
            _this._cmbTenantsCombo_OnClientItemsRequested = function (sender, args) {
                var tenantNumberOfItems = sender.get_items().get_count();
                _this._tenantService.getAllTenants(args.get_text(), _this.maxNumberElements, tenantNumberOfItems, function (data) {
                    try {
                        _this.refreshTenants(data.value);
                        var scrollToPosition = args.get_domEvent() == undefined;
                        if (scrollToPosition) {
                            if (sender.get_items().get_count() > 0) {
                                var scrollTenant = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                scrollTenant.scrollTop($(sender.get_items().getItem(tenantNumberOfItems + 1).get_element()).position().top);
                            }
                        }
                        sender.get_attributes().setAttribute('otherTenantCount', data.count.toString());
                        sender.get_attributes().setAttribute('updating', 'false');
                        if (sender.get_items().get_count() > 0) {
                            tenantNumberOfItems = sender.get_items().get_count() - 1;
                        }
                        _this._cmbTenantsCombo.get_moreResultsBoxMessageElement().innerText = "Visualizzati " + tenantNumberOfItems.toString() + " di " + data.count.toString();
                    }
                    catch (error) {
                    }
                }, function (exception) {
                });
            };
            _this.refreshTenants = function (data) {
                if (data.length > 0) {
                    _this._cmbTenantsCombo.beginUpdate();
                    if (_this._cmbTenantsCombo.get_items().get_count() === 0) {
                        var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
                        emptyItem.set_text("");
                        emptyItem.set_value("");
                        _this._cmbTenantsCombo.get_items().insert(0, emptyItem);
                    }
                    $.each(data, function (index, container) {
                        var item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_text(container.CompanyName);
                        item.set_value(container.UniqueId.toString());
                        _this._cmbTenantsCombo.get_items().add(item);
                    });
                    _this._cmbTenantsCombo.showDropDown();
                    _this._cmbTenantsCombo.endUpdate();
                }
                else {
                    if (_this._cmbTenantsCombo.get_items().get_count() === 0) {
                    }
                }
            };
            _this._cmbSelectPecMailBox_OnClientItemsRequested = function (sender, args) {
                var pecMailBoxNumberOfItems = sender.get_items().get_count();
                _this._tenantService.getAllPECMailBoxes(_this._cmbTenantsCombo.get_selectedItem().get_value(), args.get_text(), _this.maxNumberElements, pecMailBoxNumberOfItems, function (data) {
                    try {
                        _this.refreshPECMailBox(data);
                        var scrollToPosition = args.get_domEvent() == undefined;
                        if (scrollToPosition) {
                            if (sender.get_items().get_count() > 0) {
                                var scrollPECMailBox = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                scrollPECMailBox.scrollTop($(sender.get_items().getItem(pecMailBoxNumberOfItems + 1).get_element()).position().top);
                            }
                        }
                        sender.get_attributes().setAttribute('otherTenantCount', data.length.toString());
                        sender.get_attributes().setAttribute('updating', 'false');
                        if (sender.get_items().get_count() > 0) {
                            pecMailBoxNumberOfItems = sender.get_items().get_count() - 1;
                        }
                        _this._cmbSelectPecMailBox.get_moreResultsBoxMessageElement().innerText = "Visualizzati " + pecMailBoxNumberOfItems.toString() + " di " + data.length.toString();
                    }
                    catch (error) {
                    }
                }, function (exception) {
                });
            };
            _this.refreshPECMailBox = function (data) {
                if (data.length > 0) {
                    _this._cmbSelectPecMailBox.beginUpdate();
                    if (_this._cmbSelectPecMailBox.get_items().get_count() === 0) {
                        var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
                        emptyItem.set_text("");
                        emptyItem.set_value("");
                        _this._cmbSelectPecMailBox.get_items().insert(0, emptyItem);
                    }
                    $.each(data, function (index, container) {
                        var item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_text(container.MailBoxRecipient);
                        item.set_value(container.EntityShortId.toString());
                        _this._cmbSelectPecMailBox.get_items().add(item);
                    });
                    _this._cmbSelectPecMailBox.showDropDown();
                    _this._cmbSelectPecMailBox.endUpdate();
                }
                else {
                    if (_this._cmbSelectPecMailBox.get_items().get_count() === 0) {
                    }
                }
            };
            _this._cmbWorkflowRepositories_OnClientItemsRequested = function (sender, args) {
                var tenantWorkflowRepNumberOfItems = sender.get_items().get_count();
                _this._tenantWorkflowRepositoryService.getAllWorkflowRepositories(_this._cmbTenantsCombo.get_selectedItem().get_value(), args.get_text(), _this.maxNumberElements, tenantWorkflowRepNumberOfItems, function (data) {
                    try {
                        _this.refreshTenantWorkflowRep(data);
                        var scrollToPosition = args.get_domEvent() == undefined;
                        if (scrollToPosition) {
                            if (sender.get_items().get_count() > 0) {
                                var scrollWorkflowRepo = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                scrollWorkflowRepo.scrollTop($(sender.get_items().getItem(tenantWorkflowRepNumberOfItems + 1).get_element()).position().top);
                            }
                        }
                        sender.get_attributes().setAttribute('otherTenantCount', data.length.toString());
                        sender.get_attributes().setAttribute('updating', 'false');
                        if (sender.get_items().get_count() > 0) {
                            tenantWorkflowRepNumberOfItems = sender.get_items().get_count() - 1;
                        }
                        _this._cmbWorkflowRepositories.get_moreResultsBoxMessageElement().innerText = "Visualizzati " + tenantWorkflowRepNumberOfItems.toString() + " di " + data.length.toString();
                    }
                    catch (error) {
                    }
                }, function (exception) {
                });
            };
            _this.refreshTenantWorkflowRep = function (data) {
                if (data.length > 0) {
                    _this._cmbWorkflowRepositories.beginUpdate();
                    if (_this._cmbWorkflowRepositories.get_items().get_count() === 0) {
                        var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
                        emptyItem.set_text("");
                        emptyItem.set_value("");
                        _this._cmbWorkflowRepositories.get_items().insert(0, emptyItem);
                    }
                    $.each(data, function (index, container) {
                        var item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_text(container.Name);
                        item.set_value(container.UniqueId.toString());
                        _this._cmbWorkflowRepositories.get_items().add(item);
                    });
                    _this._cmbWorkflowRepositories.showDropDown();
                    _this._cmbWorkflowRepositories.endUpdate();
                }
                else {
                    if (_this._cmbWorkflowRepositories.get_items().get_count() === 0) {
                    }
                }
            };
            _this._serviceConfiguration = serviceConfigurations;
            _this._enumHelper = new EnumHelper();
            $(document).ready(function () {
            });
            return _this;
        }
        PECInvoice.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._pecInvoiveGridGrid = $find(this.pecInvoiveGridId);
            this._masterTableView = this._pecInvoiveGridGrid.get_masterTableView();
            this.pecInvoiceDirection = this.direction === "1" ? PECMailDirection.Outgoing : PECMailDirection.Incoming;
            $("#".concat(this.pecInvoiveGridId)).bind(PECInvoice.LOADED_EVENT, function () {
                _this.loadPECInvoiceGrid();
            });
            this._dpStartDateFrom = $find(this.dpStartDateFromId);
            this._dpEndDateFrom = $find(this.dpEndDateFromId);
            this._cmbPecMailBox = $find(this.cmbPecMailBoxId);
            this._cmbStato = $find(this.cmbStatoId);
            this._cmbTipologiaFattura = $find(this.cmbTipologiaFatturaId);
            this._txtMittente = $find(this.txtMittenteId);
            this._txtDestinatario = $find(this.txtDestinararioId);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicking(this.btnSearch_onClick);
            this._btnClean = $find(this.btnCleanId);
            this._btnClean.add_clicking(this.btnClean_onClick);
            this._btnTenants = $find(this.btnTenantsId);
            this._btnTenants.add_clicking(this.btnTenants_onClick);
            this._btnContainerSelectorOkId = $find(this.btnContainerSelectorOkId);
            this._btnContainerSelectorOkId.add_clicking(this.btnContainerSelectorOkId_onClick);
            this._rwTenants = $find(this.rwTenantSelectorId);
            this._cmbSelectPecMailBox = $find(this.cmbSelectPecMailBoxId);
            this._cmbSelectPecMailBox.add_itemsRequested(this._cmbSelectPecMailBox_OnClientItemsRequested);
            this._cmbWorkflowRepositories = $find(this.cmbWorkflowRepositoriesId);
            this._cmbWorkflowRepositories.add_itemsRequested(this._cmbWorkflowRepositories_OnClientItemsRequested);
            this._cmbTenantsCombo = $find(this.cmbTenantsComboId);
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
        };
        PECInvoice.prototype.constrainComboboxesOnPages = function () {
            var qs = this.parse_query_string(window.location.href);
            var param = qs["InvoiceType"];
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
        };
        PECInvoice.prototype.parse_query_string = function (query) {
            var vars = query.split("&");
            var query_string = {};
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                var key = decodeURIComponent(pair[0]);
                var value = decodeURIComponent(pair[1]);
                if (typeof query_string[key] === "undefined") {
                    query_string[key] = decodeURIComponent(value);
                }
                else if (typeof query_string[key] === "string") {
                    var arr = [query_string[key], decodeURIComponent(value)];
                    query_string[key] = arr;
                }
                else {
                    query_string[key].push(decodeURIComponent(value));
                }
            }
            return query_string;
        };
        PECInvoice.prototype.constrainComboboxes = function (status, fattura) {
            var stato = this._cmbStato.findItemByText(status);
            stato.select();
            var tipologia = this._cmbTipologiaFattura.findItemByText(fattura);
            tipologia.select();
            this._cmbStato.disable();
            this._cmbTipologiaFattura.disable();
        };
        PECInvoice.prototype.loadPECInvoiceGrid = function () {
            if (!jQuery.isEmptyObject(this._pecInvoiveGridGrid)) {
                this.loadResults();
            }
        };
        PECInvoice.prototype.addEmptyValuesToCombos = function () {
            var cmbItemStato = new Telerik.Web.UI.RadComboBoxItem();
            cmbItemStato.set_text("");
            cmbItemStato.set_value("");
            this._cmbStato.get_items().add(cmbItemStato);
            var cmbItemTipologiaFattura = new Telerik.Web.UI.RadComboBoxItem();
            cmbItemTipologiaFattura.set_text("");
            cmbItemTipologiaFattura.set_value("");
            this._cmbTipologiaFattura.get_items().add(cmbItemTipologiaFattura);
        };
        PECInvoice.prototype.loadStatus = function () {
            this._loadingPanel.show(this.cmbStatoId);
            var cmbItem = null;
            for (var n in InvoiceStatusEnum) {
                if (typeof InvoiceStatusEnum[n] === 'string') {
                    cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(this._enumHelper.getInvoiceStatusDescription(InvoiceStatusEnum[n]));
                    cmbItem.set_value(InvoiceStatusEnum[n]);
                    this._cmbStato.get_items().add(cmbItem);
                }
            }
            this._loadingPanel.hide(this.cmbStatoId);
        };
        PECInvoice.prototype.loadInvoiceType = function () {
            this._loadingPanel.show(this.cmbTipologiaFatturaId);
            for (var n in InvoiceTypeEnum) {
                if (typeof InvoiceTypeEnum[n] === 'string' && InvoiceTypeEnum[n] !== "None") {
                    var cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(this._enumHelper.getInvoiceTypeDescription(InvoiceTypeEnum[n], this.pecInvoiceDirection));
                    cmbItem.set_value(InvoiceTypeEnum[n]);
                    this._cmbTipologiaFattura.get_items().add(cmbItem);
                }
            }
            this._loadingPanel.hide(this.cmbTipologiaFatturaId);
        };
        PECInvoice.prototype.loadPECMailBoxes = function () {
            var _this = this;
            this._loadingPanel.show(this.cmbPecMailBoxId);
            this._pecMailBoxService.getFilteredPECMailBoxes(function (data) {
                if (!data)
                    return;
                _this.pecMailBoxes = data;
                var thisCmbPecMailBox = _this._cmbPecMailBox;
                var cmbItem = null;
                $.each(_this.pecMailBoxes, function (i, value) {
                    cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(value.MailBoxRecipient);
                    cmbItem.set_value(value.EntityShortId.toString());
                    thisCmbPecMailBox.get_items().add(cmbItem);
                });
                cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                cmbItem.set_text("Tutte");
                thisCmbPecMailBox.get_items().add(cmbItem);
                _this._loadingPanel.hide(_this.cmbPecMailBoxId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.cmbPecMailBoxId);
                $("#".concat(_this.cmbPecMailBoxId)).hide();
            });
        };
        PECInvoice.prototype.loadResults = function () {
            var _this = this;
            this._loadingPanel.show(this.pecInvoiveGridId);
            var startDateFromFilter = "";
            if (this._dpStartDateFrom && this._dpStartDateFrom.get_selectedDate()) {
                startDateFromFilter = moment(this._dpStartDateFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var endDateFromFilter = "";
            if (this._dpEndDateFrom && this._dpEndDateFrom.get_selectedDate()) {
                endDateFromFilter = moment(this._dpEndDateFrom.get_selectedDate()).format("YYYY-MM-DDTHH:mm:ss[Z]");
            }
            var cmbPecMailBoxId = "";
            if (this._cmbPecMailBox && this._cmbPecMailBox.get_selectedItem() !== null) {
                cmbPecMailBoxId = this._cmbPecMailBox.get_selectedItem().get_value();
            }
            var cmbStatoId = "";
            if (this._cmbStato && this._cmbStato.get_selectedItem() !== null) {
                cmbStatoId = this._cmbStato.get_selectedItem().get_value();
            }
            var cmbTipologiaFatturaId = "";
            if (this._cmbTipologiaFattura && this._cmbTipologiaFattura.get_selectedItem() !== null) {
                cmbTipologiaFatturaId = this._cmbTipologiaFattura.get_selectedItem().get_value();
            }
            var mittenteFilter = "";
            if (this._txtMittente && this._txtMittente.get_textBoxValue() !== "") {
                mittenteFilter = this._txtMittente.get_textBoxValue();
            }
            var destinatarioFilter = "";
            if (this._txtDestinatario && this._txtDestinatario.get_textBoxValue() !== "") {
                destinatarioFilter = this._txtDestinatario.get_textBoxValue();
            }
            var searchDTO = new PECMailSearchFilterDTO();
            searchDTO.direction = this.pecInvoiceDirection;
            searchDTO.dateFrom = startDateFromFilter;
            searchDTO.dateTo = endDateFromFilter;
            searchDTO.pecMailBox = cmbPecMailBoxId;
            searchDTO.invoiceStatus = cmbStatoId;
            searchDTO.invoiceType = cmbTipologiaFatturaId;
            searchDTO.mailSenders = mittenteFilter;
            searchDTO.mailRecipients = destinatarioFilter;
            this._pecMailService.getPECMails(searchDTO, function (data) {
                if (!data)
                    return;
                _this.gridResult = data;
                _this._masterTableView.set_dataSource(_this.gridResult);
                _this._masterTableView.dataBind();
                for (var rowIndex = 0; rowIndex < _this._masterTableView.get_dataItems().length; rowIndex++) {
                    //set links for subject
                    _this._masterTableView
                        .getCellByColumnUniqueName(_this._masterTableView.get_dataItems()[rowIndex], "MailSubject")
                        .innerHTML =
                        "<a runat=\"server\" href=\"PECSummary.aspx?Type=PEC&PECId=" +
                            data[rowIndex].EntityId +
                            "\">" +
                            data[rowIndex].MailSubject +
                            "</a>";
                    // set the correct image based on invoice status
                    var imgSrcBasedOnStatus = _this._masterTableView.getCellByColumnUniqueName(_this._masterTableView.get_dataItems()[rowIndex], "Icona");
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
                _this._loadingPanel.hide(_this.pecInvoiveGridId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.pecInvoiveGridId);
                $("#".concat(_this.pecInvoiveGridId)).hide();
            });
        };
        //region [ Grid Configuration Methods ]
        PECInvoice.prototype.onGridDataBound = function () {
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
        PECInvoice.prototype.onGridRowSelected = function () {
            var masterTable = this._pecInvoiveGridGrid.get_masterTableView();
            var selectedRow = masterTable.get_selectedItems();
            this._btnTenants.set_enabled(true);
            var viewModelMapper = new PECMailViewModelMapper();
            this.selectedPECMailId = viewModelMapper.Map(selectedRow[0]._dataItem).EntityId;
        };
        PECInvoice.LOADED_EVENT = "onLoaded";
        PECInvoice.PAGE_CHANGED_EVENT = "onPageChanged";
        return PECInvoice;
    }(PecInvoiceBase));
    return PECInvoice;
});
//# sourceMappingURL=PECInvoice.js.map