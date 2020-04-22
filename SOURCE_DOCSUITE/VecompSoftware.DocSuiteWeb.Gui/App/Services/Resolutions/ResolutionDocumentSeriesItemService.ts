import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ResolutionDocumentSeriesItemModel = require("App/Models/Resolutions/ResolutionDocumentSeriesItemModel");

class ResolutionDocumentSeriesItemService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
     * Costruttore
     * @param webApiUrl
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }


    getResolutionDocumentSeriesItemLinksCount(idDocumentUnit: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `/$count?$filter=DocumentSeriesItem/Status ne 'Draft' and Resolution/UniqueId eq ${idDocumentUnit}`;
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    getResolutionDocumentSeriesItemLinks(idDocumentUnit: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let qs: string = `$expand=DocumentSeriesItem($expand=DocumentSeries)&$filter=DocumentSeriesItem/Status ne 'Draft' and Resolution/UniqueId eq ${idDocumentUnit}`;
        this.getRequest(this._configuration.ODATAUrl, qs,
            (response: any) => {
                if (callback && response) {
                    let resolutionDocumentSeriesItem: ResolutionDocumentSeriesItemModel[] = response.value;
                    callback(resolutionDocumentSeriesItem);
                }
            }, error);
    }

}
export = ResolutionDocumentSeriesItemService;