import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ResolutionKindModel = require('App/Models/Resolutions/ResolutionKindModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class ResolutionKindService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
     * Costruttore
     * @param webApiUrl
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    findActiveTypologies(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let qs: string = "$filter=IsActive eq true";
        this.findTypologies(qs, callback, error);
    }

    findDisabledTypologies(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let qs: string = "$filter=IsActive eq false";
        this.findTypologies(qs, callback, error);
    }

    findAllTypologies(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let qs: string = "$filter=IsActive eq true or IsActive eq false";
        this.findTypologies(qs, callback, error);
    }

    private findTypologies(qs: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        qs = qs.concat("&$orderby=Name");
        this.getRequest(this._configuration.ODATAUrl, qs,
            (response: any) => {
                if (callback && response) {
                    let resolutionKinds: ResolutionKindModel[] = response.value;
                    callback(resolutionKinds);
                }
            }, error);
    }

    getById(idResolutionKind: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let qs: string = "$filter=UniqueId eq ".concat(idResolutionKind);
        this.getRequest(this._configuration.ODATAUrl, qs,
            (response: any) => {
                if (callback && response) {
                    let resolutionKind: ResolutionKindModel[] = response.value;
                    callback(resolutionKind[0]);
                }
            }, error);
    }

    insertResolutionKindModel(model: ResolutionKindModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    updateResolutionKindModel(model: ResolutionKindModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    deleteResolutionKindModel(model: ResolutionKindModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }
}
export = ResolutionKindService;