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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/PECMails/PECMailViewModelMapper", "App/Models/PECMails/PECMailDirection", "App/Models/PECMails/InvoiceStatusEnum", "App/Models/PECMails/InvoiceTypeEnum"], function (require, exports, BaseService, PECMailViewModelMapper, PECMailDirection, InvoiceStatusEnum, InvoiceTypeEnum) {
    var PECMailService = /** @class */ (function (_super) {
        __extends(PECMailService, _super);
        function PECMailService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        PECMailService.prototype.getPECMails = function (searchFilter, callback, error) {
            var urlPart = this._configuration.ODATAUrl;
            //always expand PECMailBox for filtering the pec mails by InvoiceType 
            urlPart = urlPart + "?$expand=PECMailBox";
            var oDataFilters = "&$filter=Direction eq VecompSoftware.DocSuiteWeb.Entity.PECMails.PECMailDirection'" + PECMailDirection[searchFilter.direction] + "'";
            oDataFilters = oDataFilters.concat(" and (PECMailBox/InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'" + InvoiceTypeEnum[InvoiceTypeEnum.InvoicePA] + "' or \n                                            PECMailBox/InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'" + InvoiceTypeEnum[InvoiceTypeEnum.InvoicePR] + "')");
            oDataFilters = oDataFilters.concat(" and MailType eq 'posta-certificata' ");
            if (searchFilter.dateFrom) {
                oDataFilters = oDataFilters.concat(" and MailDate gt ", searchFilter.dateFrom);
            }
            if (searchFilter.dateTo) {
                oDataFilters = oDataFilters.concat(" and MailDate lt ", searchFilter.dateTo);
            }
            if (searchFilter.pecMailBox && searchFilter.pecMailBox !== 'Tutte') {
                oDataFilters = oDataFilters.concat(" and PECMailBox/EntityShortId eq ", searchFilter.pecMailBox);
            }
            if (searchFilter.invoiceStatus) {
                switch (InvoiceStatusEnum[searchFilter.invoiceStatus.toString()]) {
                    case InvoiceStatusEnum.None:
                        oDataFilters = oDataFilters.concat(" and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'" + InvoiceStatusEnum[InvoiceStatusEnum.None] + "'");
                        break;
                    case InvoiceStatusEnum.PAInvoiceSent:
                        oDataFilters = oDataFilters.concat(" and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'" + InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceSent] + "'");
                        break;
                    case InvoiceStatusEnum.PAInvoiceNotified:
                        oDataFilters = oDataFilters.concat(" and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'" + InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceNotified] + "'");
                        break;
                    case InvoiceStatusEnum.PAInvoiceAccepted:
                        oDataFilters = oDataFilters.concat(" and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'" + InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceAccepted] + "'");
                        break;
                    case InvoiceStatusEnum.PAInvoiceSdiRefused:
                        oDataFilters = oDataFilters.concat(" and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'" + InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceSdiRefused] + "'");
                        break;
                    case InvoiceStatusEnum.PAInvoiceRefused:
                        oDataFilters = oDataFilters.concat(" and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'" + InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceRefused] + "'");
                        break;
                    default:
                }
            }
            if (searchFilter.invoiceType) {
                switch (InvoiceTypeEnum[searchFilter.invoiceType.toString()]) {
                    case InvoiceTypeEnum.None:
                        oDataFilters = oDataFilters.concat(" and PECMailBox/InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'" + InvoiceTypeEnum[InvoiceTypeEnum.None] + "'");
                        break;
                    case InvoiceTypeEnum.InvoicePA:
                        oDataFilters = oDataFilters.concat(" and PECMailBox/InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'" + InvoiceTypeEnum[InvoiceTypeEnum.InvoicePA] + "'");
                        break;
                    case InvoiceTypeEnum.InvoicePR:
                        oDataFilters = oDataFilters.concat(" and PECMailBox/InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'" + InvoiceTypeEnum[InvoiceTypeEnum.InvoicePR] + "'");
                        break;
                    default:
                }
            }
            if (searchFilter.mailSenders) {
                oDataFilters = oDataFilters.concat(" and indexof(MailSenders, '" + searchFilter.mailSenders + "') gt -1");
            }
            if (searchFilter.mailRecipients) {
                oDataFilters = oDataFilters.concat(" and indexof(MailRecipients, '" + searchFilter.mailRecipients + "') gt -1");
            }
            var url = urlPart.concat(oDataFilters);
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var viewModelMapper_1 = new PECMailViewModelMapper();
                    var pecMails_1 = [];
                    $.each(response.value, function (i, value) {
                        pecMails_1.push(viewModelMapper_1.Map(value));
                    });
                    callback(pecMails_1);
                }
                ;
            }, error);
        };
        PECMailService.prototype.insertPECMailTenantCorrection = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl.concat("?actionType=PECMailInvoiceTenantCorrection");
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        PECMailService.prototype.getPECMailById = function (pecMailId, callback, error) {
            var url = this._configuration.ODATAUrl + "?$expand=PECMailBox($expand=Location),Location&$filter=EntityId eq " + pecMailId;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper = new PECMailViewModelMapper();
                    var pecMailInErrore = response.value[0];
                    modelMapper.Map(pecMailInErrore);
                    callback(pecMailInErrore);
                }
                ;
            }, error);
        };
        PECMailService.prototype.getOutgoingPECMail = function (idDocumentUnit, pecMailDirection, callback, error) {
            var url = this._configuration.ODATAUrl + "?$filter=DocumentUnit/UniqueId eq " + idDocumentUnit + " and Direction eq " + pecMailDirection + " and IsActive ne 'Processed'&$expand=PECMailReceipts";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper = new PECMailViewModelMapper();
                    var pecMailInErrore = response.value;
                    modelMapper.Map(pecMailInErrore);
                    callback(pecMailInErrore);
                }
                ;
            }, error);
        };
        PECMailService.prototype.getIncomingPECMail = function (idDocumentUnit, pecMailDirection, callback, error) {
            var url = this._configuration.ODATAUrl + "?$filter=DocumentUnit/UniqueId eq " + idDocumentUnit + " and Direction eq " + pecMailDirection + "&$expand=PECMailReceipts";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper = new PECMailViewModelMapper();
                    var pecMailInErrore = response.value;
                    modelMapper.Map(pecMailInErrore);
                    callback(pecMailInErrore);
                }
                ;
            }, error);
        };
        PECMailService.prototype.countIncomingPECMail = function (idDocumentUnit, pecMailDirection, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DocumentUnit/UniqueId eq " + idDocumentUnit + " and Direction eq " + pecMailDirection + "&$expand=PECMailReceipts";
            url = "" + url + data;
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        PECMailService.prototype.countOutgoingPECMail = function (idDocumentUnit, pecMailDirection, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DocumentUnit/UniqueId eq " + idDocumentUnit + " and Direction eq " + pecMailDirection + " and IsActive ne 'Processed'&$expand=PECMailReceipts";
            url = "" + url + data;
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        PECMailService.prototype.getOutgoingPECMailByEntityId = function (entityId, callback, error) {
            var url = this._configuration.ODATAUrl + "?$filter=EntityId eq " + entityId + "&$expand=PECMailReceipts";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper = new PECMailViewModelMapper();
                    var pecMailInErrore = response.value;
                    modelMapper.Map(pecMailInErrore);
                    callback(pecMailInErrore);
                }
                ;
            }, error);
        };
        PECMailService.prototype.getIncomingPECMailByEntityId = function (entityId, callback, error) {
            var url = this._configuration.ODATAUrl + "?$filter=EntityId eq " + entityId;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper = new PECMailViewModelMapper();
                    var pecMailInErrore = response.value;
                    modelMapper.Map(pecMailInErrore);
                    callback(pecMailInErrore);
                }
                ;
            }, error);
        };
        PECMailService.prototype.getProtocolledPecMail = function (idDocumentUnit, callback, error) {
            var url = this._configuration.ODATAUrl + "?$filter=DocumentUnit/UniqueId eq " + idDocumentUnit + "&$expand=pecmailreceipts";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper = new PECMailViewModelMapper();
                    var pecMailInErrore = response.value;
                    modelMapper.Map(pecMailInErrore);
                    callback(pecMailInErrore);
                }
                ;
            }, error);
        };
        PECMailService.prototype.countProtocolledPecMail = function (idDocumentUnit, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DocumentUnit/UniqueId eq " + idDocumentUnit;
            url = "" + url + data;
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        return PECMailService;
    }(BaseService));
    return PECMailService;
});
//# sourceMappingURL=PECMailService.js.map