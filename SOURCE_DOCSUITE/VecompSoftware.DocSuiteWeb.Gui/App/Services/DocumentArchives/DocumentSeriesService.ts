import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import DocumentSeriesModel = require('App/Models/DocumentArchives/DocumentSeriesModel');

class DocumentSeriesService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
     * Costruttore
     * @param webApiUrl
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getAll(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        this.getRequest(this._configuration.ODATAUrl, "$orderby=Name",
            (response: any) => {
                if (callback && response) {
                    let documentSeries: DocumentSeriesModel[] = response.value;
                    callback(documentSeries);
                }
            }, error);
    }

    getById(idDocumentSeries: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let qs: string = "$filter=EntityId eq ".concat(idDocumentSeries.toString());
        this.getRequest(this._configuration.ODATAUrl, qs,
            (response: any) => {
                if (callback && response) {
                    let documentSeries: DocumentSeriesModel[] = response.value;
                    callback(documentSeries[0]);
                }
            }, error);
    }
}

export = DocumentSeriesService;