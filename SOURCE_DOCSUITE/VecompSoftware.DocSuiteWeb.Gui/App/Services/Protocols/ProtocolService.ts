import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class ProtocolService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
     * Costruttore
     * @param webApiUrl
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    /**
     * Recupera un protocollo per UniqueId
     * @param uniqueId
     * @param callback
     * @param error
     */
    getProtocolByUniqueId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=UniqueId eq ".concat(uniqueId.toString(), "&$expand=Category,Container");
        this.getRequest(url, data, (response: any) => {
            if (callback) callback(response.value[0]);
        }, error);
    }

    getDocumentSerieslByUniqueId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=UniqueId eq ".concat(uniqueId.toString(), "&$expand=documentseriesitems");
        this.getRequest(url, data, (response: any) => {
            if (callback) callback(response.value[0]);
        }, error);
    }

    getProtocolMessageById(Id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$expand=Messages($expand=MessageContacts, MessageEmails)&$filter=UniqueId eq ".concat(Id, "&$select=Messages");
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    countDocumentSeriesItemProtocolById(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=DocumentSeriesItems/any(d: d/UniqueId eq ${documentUnitId})`;
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }
}

export = ProtocolService;