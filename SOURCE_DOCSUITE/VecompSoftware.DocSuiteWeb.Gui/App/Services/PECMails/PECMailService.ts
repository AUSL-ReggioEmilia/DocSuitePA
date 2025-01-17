﻿import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import PECMailViewModel = require('App/ViewModels/PECMails/PECMailViewModel');
import PECMailViewModelMapper = require('App/Mappers/PECMails/PECMailViewModelMapper');
import PECMailSearchFilterDTO = require('App/DTOs/PECMailSearchFilterDTO');
import PECMailDirection = require("App/Models/PECMails/PECMailDirection");
import InvoiceStatusEnum = require("App/Models/PECMails/InvoiceStatusEnum");
import InvoiceTypeEnum = require("App/Models/PECMails/InvoiceTypeEnum");
import PECMailActiveTypeEnum = require('App/Models/PECMails/PECMailActiveTypeEnum');
import PaginationModel = require("App/Models/Commons/PaginationModel");

class PECMailService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getPECMails(searchFilter: PECMailSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {

        let urlPart: string = this._configuration.ODATAUrl;

        //always expand PECMailBox for filtering the pec mails by InvoiceType 
        urlPart = urlPart + "?$expand=PECMailBox";

        let oDataFilters: string = `&$filter=Direction eq VecompSoftware.DocSuiteWeb.Entity.PECMails.PECMailDirection'${PECMailDirection[searchFilter.direction]}'`;

        oDataFilters = oDataFilters.concat(` and (PECMailBox/InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'${InvoiceTypeEnum[InvoiceTypeEnum.InvoicePA]}' or 
                                            PECMailBox/InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'${InvoiceTypeEnum[InvoiceTypeEnum.InvoicePR]}')`);
        oDataFilters = oDataFilters.concat(` and MailType eq 'posta-certificata' `);

        if (searchFilter.dateFrom) {
            oDataFilters = oDataFilters.concat(` and MailDate gt `, searchFilter.dateFrom);
        }
        if (searchFilter.dateTo) {
            oDataFilters = oDataFilters.concat(` and MailDate lt `, searchFilter.dateTo);
        }
        if (searchFilter.pecMailBox && searchFilter.pecMailBox !== 'Tutte') {
            oDataFilters = oDataFilters.concat(` and PECMailBox/EntityShortId eq `, searchFilter.pecMailBox);
        }
        if (searchFilter.invoiceStatus) {
            switch (<InvoiceStatusEnum>InvoiceStatusEnum[searchFilter.invoiceStatus.toString()]) {
                case InvoiceStatusEnum.None:
                    oDataFilters = oDataFilters.concat(` and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'${InvoiceStatusEnum[InvoiceStatusEnum.None]}'`);
                    break;
                case InvoiceStatusEnum.PAInvoiceSent:
                    oDataFilters = oDataFilters.concat(` and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'${InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceSent]}'`);
                    break;
                case InvoiceStatusEnum.PAInvoiceNotified:
                    oDataFilters = oDataFilters.concat(` and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'${InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceNotified]}'`);
                    break;
                case InvoiceStatusEnum.PAInvoiceAccepted:
                    oDataFilters = oDataFilters.concat(` and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'${InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceAccepted]}'`);
                    break;
                case InvoiceStatusEnum.PAInvoiceSdiRefused:
                    oDataFilters = oDataFilters.concat(` and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'${InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceSdiRefused]}'`);
                    break;
                case InvoiceStatusEnum.PAInvoiceRefused:
                    oDataFilters = oDataFilters.concat(` and InvoiceStatus eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceStatus'${InvoiceStatusEnum[InvoiceStatusEnum.PAInvoiceRefused]}'`);
                    break;
                default:
            }
        }
        if (searchFilter.invoiceType) {
            switch (<InvoiceTypeEnum>InvoiceTypeEnum[searchFilter.invoiceType.toString()]) {
                case InvoiceTypeEnum.None:
                    oDataFilters = oDataFilters.concat(` and PECMailBox/InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'${InvoiceTypeEnum[InvoiceTypeEnum.None]}'`);
                    break;
                case InvoiceTypeEnum.InvoicePA:
                    oDataFilters = oDataFilters.concat(` and PECMailBox/InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'${InvoiceTypeEnum[InvoiceTypeEnum.InvoicePA]}'`);
                    break;
                case InvoiceTypeEnum.InvoicePR:
                    oDataFilters = oDataFilters.concat(` and PECMailBox/InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'${InvoiceTypeEnum[InvoiceTypeEnum.InvoicePR]}'`);
                    break;
                default:
            }
        }
        if (searchFilter.mailSenders) {
            oDataFilters = oDataFilters.concat(` and indexof(MailSenders, '${searchFilter.mailSenders}') gt -1`);
        }
        if (searchFilter.mailRecipients) {
            oDataFilters = oDataFilters.concat(` and indexof(MailRecipients, '${searchFilter.mailRecipients}') gt -1`);
        }

        let url: string = urlPart.concat(oDataFilters);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let viewModelMapper = new PECMailViewModelMapper();
                let pecMails: PECMailViewModel[] = [];
                $.each(response.value, function (i, value) {
                    pecMails.push(viewModelMapper.Map(value));
                });
                callback(pecMails);
            };
        }, error);
    }

    insertPECMailTenantCorrection(model: PECMailViewModel,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl.concat("?actionType=PECMailInvoiceTenantCorrection");
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    getPECMailById(pecMailId: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        const url = `${this._configuration.ODATAUrl}?$expand=PECMailBox($expand=Location),Location&$filter=EntityId eq ${pecMailId}`;

        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    const modelMapper = new PECMailViewModelMapper();
                    callback(modelMapper.Map(response.value[0]));
                }
            }, error);
    }

    getPECMailsByDocumentUnit(idDocumentUnit: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, excludeDisabledPECMails: boolean = true, paginationModel: PaginationModel = null): void {
        let url = `${this._configuration.ODATAUrl}?$filter=DocumentUnit/UniqueId eq ${idDocumentUnit} and IsActive ne 'Processed'`;
        if (excludeDisabledPECMails) {
            url = `${url} and IsActive ne '${PECMailActiveTypeEnum[PECMailActiveTypeEnum.Disabled]}'`;
        }
        url = `${url} &$expand=PECMailReceipts`;
        if (paginationModel) {
            url = `${url}&$skip=${paginationModel.Skip}&$top=${paginationModel.Take}`;
        }
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                const viewModelMapper = new PECMailViewModelMapper();
                const pecMails: PECMailViewModel[] = [];
                $.each(response.value, function (i, value) {
                    pecMails.push(viewModelMapper.Map(value));
                });
                callback(pecMails);
            };
        }, error);
    }

    getOutgoingPECMail(idDocumentUnit: string, pecMailDirection: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, excludeDisabledPECMails: boolean = true, paginationModel?: PaginationModel): void {
        let url: string = `${this._configuration.ODATAUrl}?$filter=DocumentUnit/UniqueId eq ${idDocumentUnit} and Direction eq ${pecMailDirection} and IsActive ne 'Processed'`;
        if (excludeDisabledPECMails) {
            url = `${url} and IsActive ne '${PECMailActiveTypeEnum[PECMailActiveTypeEnum.Disabled]}'`;
        }
        url = `${url} &$expand=PECMailReceipts`;
        if (paginationModel) {
            url = `${url}&$skip=${paginationModel.Skip}&$top=${paginationModel.Take}`;
        }
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                const viewModelMapper = new PECMailViewModelMapper();
                const pecMails: PECMailViewModel[] = [];
                $.each(response.value, function (i, value) {
                    pecMails.push(viewModelMapper.Map(value));
                });
                callback(pecMails);
            };
        }, error);
    }

    getIncomingPECMail(idDocumentUnit: string, pecMailDirection: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, excludeDisabledPECMails: boolean = true, paginationModel?: PaginationModel): void {
        let url: string =
            `${this._configuration.ODATAUrl}?$expand=PECMailReceipts&$filter=DocumentUnit/UniqueId eq ${idDocumentUnit} and Direction eq ${pecMailDirection}`;
        if (excludeDisabledPECMails) {
            url = `${url} and IsActive ne '${PECMailActiveTypeEnum[PECMailActiveTypeEnum.Disabled]}'`;
        }
        if (paginationModel) {
            url = `${url}&$skip=${paginationModel.Skip}&$top=${paginationModel.Take}`;
        }
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                const viewModelMapper = new PECMailViewModelMapper();
                const pecMails: PECMailViewModel[] = [];
                $.each(response.value, function (i, value) {
                    pecMails.push(viewModelMapper.Map(value));
                });
                callback(pecMails);
            };
        }, error);
    }

    countIncomingPECMail(idDocumentUnit: string, pecMailDirection: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, excludeDisabledPECMails: boolean = true): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=DocumentUnit/UniqueId eq ${idDocumentUnit} and Direction eq ${pecMailDirection}`;
        if (excludeDisabledPECMails) {
            data = `${data} and IsActive ne '${PECMailActiveTypeEnum[PECMailActiveTypeEnum.Disabled]}'`;
        }
        url = `${url}${data}`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
        
    }

    countOutgoingPECMail(idDocumentUnit: string, pecMailDirection: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, excludeDisabledPECMails: boolean = true): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=DocumentUnit/UniqueId eq ${idDocumentUnit} and Direction eq ${pecMailDirection} and IsActive ne 'Processed'`;
        if (excludeDisabledPECMails) {
            data = `${data} and IsActive ne '${PECMailActiveTypeEnum[PECMailActiveTypeEnum.Disabled]}'`;
        }
        data = `${data} &$expand=PECMailReceipts`;
        url = `${url}${data}`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    countPECMail(idDocumentUnit: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, excludeDisabledPECMails: boolean = true): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=DocumentUnit/UniqueId eq ${idDocumentUnit} and IsActive ne 'Processed'`;
        if (excludeDisabledPECMails) {
            data = `${data} and IsActive ne '${PECMailActiveTypeEnum[PECMailActiveTypeEnum.Disabled]}'`;
        }
        data = `${data} &$expand=PECMailReceipts`;
        url = `${url}${data}`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    getOutgoingPECMailByEntityId(entityId: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string =
            `${this._configuration.ODATAUrl}?$filter=EntityId eq ${entityId}&$expand=PECMailReceipts`;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new PECMailViewModelMapper();
                let pecMailInErrore: PECMailViewModel = response.value;
                modelMapper.Map(pecMailInErrore);
                callback(pecMailInErrore);
            };
        }, error);
    }

    getIncomingPECMailByEntityId(entityId: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string =
            `${this._configuration.ODATAUrl}?$filter=EntityId eq ${entityId}`;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new PECMailViewModelMapper();
                let pecMailInErrore: PECMailViewModel = response.value;
                modelMapper.Map(pecMailInErrore);
                callback(pecMailInErrore);
            };
        }, error);
    }

    getProtocolledPecMail(idDocumentUnit: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string =
            `${this._configuration.ODATAUrl}?$filter=DocumentUnit/UniqueId eq ${idDocumentUnit}&$expand=pecmailreceipts`;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let modelMapper = new PECMailViewModelMapper();
                let pecMailInErrore: PECMailViewModelMapper = response.value;
                modelMapper.Map(pecMailInErrore);
                callback(pecMailInErrore);
            };
        }, error);
    }

    countProtocolledPecMail(idDocumentUnit: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=DocumentUnit/UniqueId eq ${idDocumentUnit}`;
        url = `${url}${data}`;
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }
}

export = PECMailService;