import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import MessagesModelMapper = require('App/Mappers/Messages/MessageModelMapper');
import MessageModel = require('App/Models/Messages/MessageModel');

class MessageService extends BaseService {
    private _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    countProtocolMessagesById(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
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

    getProtocolMessagesByShortId(messageId: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$expand=MessageContacts,MessageEmails&$filter=EntityId eq ${messageId}`;
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                let mapper = new MessagesModelMapper();
                let instance = <MessageModel>{};
                instance = mapper.Map(response.value[0]);
                callback(instance); }
        }, error);
    }

    countDocumentSeriesItemById(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
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

    countResolutionMessagesById(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=Resolutions/any(d: d/UniqueId eq ${documentUnitId})`;
        url = url.concat(data);
        this.getRequest(url, null, 
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }
}

export = MessageService;