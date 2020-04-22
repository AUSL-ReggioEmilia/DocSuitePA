import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import DocumentSeriesItemModelMapper = require('../../Mappers/Series/DocumentSeriesItemModelMapper');
import DocumentSeriesItemModel = require('../../Models/Series/DocumentSeriesItemModel');

class DocumentSeriesItemService extends BaseService {
    private _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    countDocumentSeriesItemById(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=Protocols/any(d: d/UniqueId eq ${documentUnitId})`;
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    getDocumentSeriesItemById(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$expand=Messages($expand=MessageContacts,MessageEmails)&$filter=UniqueId eq ${documentUnitId}`;
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                let mapper = new DocumentSeriesItemModelMapper();
                let instance = <DocumentSeriesItemModel>{};
                instance = mapper.Map(response.value[0]);
                callback(instance);
            }
        }, error);
    }

    getDocumentSeriesItemProtocolById(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$expand=Protocols&$filter=UniqueId eq ${documentUnitId}`;
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                let mapper = new DocumentSeriesItemModelMapper();
                let instance = <DocumentSeriesItemModel>{};
                instance = mapper.Map(response.value[0]);
                callback(instance);
            }
        }, error);
    }
}

export = DocumentSeriesItemService;