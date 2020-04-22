import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ResolutionModel = require('App/Models/Resolutions/ResolutionModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ResolutionModelMapper = require('App/Mappers/Resolutions/ResolutionModelMapper');

class ResolutionService extends BaseService {
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
     * Recupera una resolution per ID
     * @param idResolution
     * @param callback
     * @param error
     */
    getResolutionById(idResolution: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=EntityId eq ".concat(idResolution.toString(), "&$expand=Category");
        this.getRequest(url, data, (response: any) => {
            let mapper = new ResolutionModelMapper();            
            callback(mapper.Map(response.value[0]));
        }, error);
    }

    /**
     * Ritorna una resolution per Year/Number e IdType e verifica le autorizzazioni utente
     * @param year
     * @param number
     * @param idType
     * @param isSecurityEnabled
     * @param callback
     * @param error
     */
    getAuthorizedResolution(year: number, number: string, idType: number, isSecurityEnabled: boolean, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=Year eq ".concat(year.toString(), " and IdType eq ", idType.toString(), " and ServiceNumber eq '", number, "' ");
        if (!isNaN(Number(number))) {
            data = data.concat("or Number eq ", number);
        }
        data = data.concat("&$expand=Category");
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                let mapper = new ResolutionModelMapper();
                callback(mapper.Map(response.value[0]));
            }
        }, error);
    }

    /**
     * Recupera una resolution per UniqueId
     * @param uniqueId
     * @param callback
     * @param error
     */
    getResolutionByUniqueId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=UniqueId eq ".concat(uniqueId.toString(), "&$expand=Category,Container");
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                let mapper = new ResolutionModelMapper();
                callback(mapper.Map(response.value[0]));
            }
        }, error);
    }

    getResolutionMessageByUniqueId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$expand=Messages($expand=MessageContacts,MessageEmails)&$filter=UniqueId eq ${uniqueId}`;
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                let mapper = new ResolutionModelMapper();
                callback(mapper.Map(response.value[0]));
            }
        }, error);
    }
}

export = ResolutionService;