import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import PECMailBoxViewModel = require('App/ViewModels/PECMails/PECMailBoxViewModel');
import PECMailBoxViewModelMapper = require('App/Mappers/PECMails/PECMailBoxViewModelMapper');
import PECMailBoxModel = require('App/Models/PECMails/PECMailBoxModel');
import InvoiceTypeEnum = require("App/Models/PECMails/InvoiceTypeEnum");
import ODATAResponseModel = require('App/Models/ODATAResponseModel');

class PECMailBoxService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getPECMailBoxes(searchFilter: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let urlPart: string = this._configuration.ODATAUrl;
        let url: string =
            searchFilter === ""
                ? urlPart.concat(`?$filter=IncomingServer ne null and OutgoingServer ne null`)
                : urlPart.concat(`?$filter=indexof(MailBoxRecipient,'${searchFilter}') gt -1 and IncomingServer ne null and OutgoingServer ne null`);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let viewModelMapper = new PECMailBoxViewModelMapper();
                let pecMailBoxes: PECMailBoxViewModel[] = [];
                $.each(response.value, function (i, value) {
                    pecMailBoxes.push(viewModelMapper.Map(value));
                });
                callback(pecMailBoxes);
            };
        }, error);
    }

    getFilteredPECMailBoxes(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let urlPart: string = this._configuration.ODATAUrl;

        // get pec mail boxes only with invoice type InvoicePA or InvoicePR
        let url: string = urlPart.concat(`?$filter= InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'${InvoiceTypeEnum[InvoiceTypeEnum.InvoicePA]}' 
                                          or InvoiceType eq VecompSoftware.DocSuiteWeb.Entity.PECMails.InvoiceType'${InvoiceTypeEnum[InvoiceTypeEnum.InvoicePR]}'`);

        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let viewModelMapper = new PECMailBoxViewModelMapper();
                let pecMailBoxes: PECMailBoxViewModel[] = [];
                $.each(response.value, function (i, value) {
                    pecMailBoxes.push(viewModelMapper.Map(value));
                });
                callback(pecMailBoxes);
            };
        }, error);
    }

    updatePECMailBox(model: PECMailBoxModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    insertPECMailBox(model: PECMailBoxModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    getPECMailBoxById(pecMailBoxId: number,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`?$expand=Location&$filter=EntityShortId eq ${pecMailBoxId}`);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let viewModelMapper = new PECMailBoxViewModelMapper();
                let pecMailBoxes: PECMailBoxViewModel[] = [];
                $.each(response.value, function (i, value) {
                    pecMailBoxes.push(viewModelMapper.Map(value));
                });
                callback(pecMailBoxes);
            };
        }, error);
    }

    getAllPECMailBoxes(name: string, topElement: string,
        skipElement: number,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=contains(MailBoxRecipient,'${name}') and IncomingServer ne null and OutgoingServer ne null&$count=true&$top=${topElement}&$skip=${skipElement.toString()}`;

        this.getRequest(url,
            qs,
            (response: any) => {
                if (callback) {
                    let responseModel: ODATAResponseModel<PECMailBoxModel> = new ODATAResponseModel<PECMailBoxModel>(response);

                    let mapper = new PECMailBoxViewModelMapper();
                    responseModel.value = mapper.MapCollection(response.value);;

                    callback(responseModel);
                }
            },
            error);
    }

}

export = PECMailBoxService;