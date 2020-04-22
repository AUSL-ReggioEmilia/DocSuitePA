import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import DocumentSeriesConstraintModel = require('App/Models/DocumentArchives/DocumentSeriesConstraintModel');

class DocumentSeriesConstraintService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
     * Costruttore
     * @param webApiUrl
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getByIdSeries(idSeries: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$orderby=Name&$filter=DocumentSeries/EntityId eq ".concat(idSeries.toString());
        this.getRequest(url, data,
            (response: any) => {
                if (callback && response) {
                    let constraints: DocumentSeriesConstraintModel[] = response.value;
                    callback(constraints);
                }
            }, error);
    }

    insertDocumentSeriesConstraint(model: DocumentSeriesConstraintModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    updateDocumentSeriesConstraint(model: DocumentSeriesConstraintModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    deleteDocumentSeriesConstraint(model: DocumentSeriesConstraintModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }
}

export = DocumentSeriesConstraintService;