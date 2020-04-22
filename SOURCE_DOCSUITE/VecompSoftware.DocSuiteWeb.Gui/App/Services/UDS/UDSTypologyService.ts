import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UDSTypologyModel = require('App/Models/UDS/UDSTypologyModel');
import UDSTypologyModelMapper = require('App/Mappers/UDS/UDSTypologyModelMapper');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class UDSTypologyService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
     * Costruttore
     * @param webApiUrl
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }   

    getUDSTypologyById(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=UniqueId eq ".concat(uniqueId);
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                let mapper: UDSTypologyModelMapper = new UDSTypologyModelMapper();
                callback(mapper.Map(response.value[0]));
            }
        }, error);
    }

    /**
     * Recupera una UDSRepository per Nome
     * @param name
     * @param callback
     * @param error
     */
    getUDSTypologyByName(name: string, status: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;       
        let data: string = "$filter=contains(Name,'".concat(name.toString(), "')");
        if (status) {
            data = data.concat("and Status eq VecompSoftware.DocSuiteWeb.Entity.UDS.UDSTypologyStatus'", status.toString(), "'");
        }
        data = data.concat("&$orderby=Name asc");
        this.getRequest(url, data, (response: any) => {
            if (callback) {   
                let mapper: UDSTypologyModelMapper = new UDSTypologyModelMapper();
                callback(mapper.MapCollection(response.value));
            }
        }, error);
    }

    insertUDSTypology(typology: UDSTypologyModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(typology), callback, error);
    }

    /**
    * Aggiorno una UDSTypology
    */
    updateUDSTypology(typology: UDSTypologyModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(typology), callback, error);
    }
}

export = UDSTypologyService;