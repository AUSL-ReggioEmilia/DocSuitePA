import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import DocumentSeriesItemLinksModel = require('../../Models/Series/DocumentSeriesItemLinksModel');
import DocumentSeriesItemLinksModelMapper = require('../../Mappers/Series/DocumentSeriesItemLinksModelMapper');

class DocumentSeriesItemLinksService extends BaseService {
    private _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    countDocumentSeriesItemLinksById(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=DocumentSeriesItem/UniqueId eq ${documentUnitId}`;
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    getDocumentSeriesItemLinksById(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$expand=Resolution&$filter=DocumentSeriesItem/UniqueId eq ${documentUnitId}`;
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                let mapper = new DocumentSeriesItemLinksModelMapper();
                let instance = <DocumentSeriesItemLinksModel>{};
                instance = mapper.Map(response.value[0]);
                callback(instance);
            }
        }, error);
    }

}

export = DocumentSeriesItemLinksService;