
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicleDocumentModelMapper = require('App/Mappers/Fascicles/FascicleDocumentModelMapper');

class FascicleDocumentService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    insertFascicleDocument(model: FascicleDocumentModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    updateFascicleDocument(model: FascicleDocumentModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    /**
     * Recupera un Fascicolo per ID
     * @param id
     * @param callback
     * @param error
     */
    getByFolder(idFascicle: string, idFascicleFolder?: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=Fascicle/UniqueId eq ".concat(idFascicle);
        if (idFascicleFolder) {
            data = data.concat(" and FascicleFolder/UniqueId eq ", idFascicleFolder);
        }
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let mapper = new FascicleDocumentModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
    }

}

export = FascicleDocumentService;