import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ResolutionKindDocumentSeriesModel = require('App/Models/Resolutions/ResolutionKindDocumentSeriesModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class ResolutionKindDocumentSeriesService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
     * Costruttore
     * @param webApiUrl
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getByResolutionKind(idResolutionKind: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let qs: string = "$filter=ResolutionKind/UniqueId eq ".concat(idResolutionKind, "&$expand=DocumentSeries,DocumentSeriesConstraint");
        this.getRequest(this._configuration.ODATAUrl, qs,
            (response: any) => {
                if (callback && response) {
                    let resolutionKinds: ResolutionKindDocumentSeriesModel[] = response.value;
                    callback(resolutionKinds);
                }
            }, error);
    }

    insertResolutionKindDocumentSeriesModel(model: ResolutionKindDocumentSeriesModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    updateResolutionKindDocumentSeriesModel(model: ResolutionKindDocumentSeriesModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    deleteResolutionKindDocumentSeriesModel(model: ResolutionKindDocumentSeriesModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }
}
export = ResolutionKindDocumentSeriesService;