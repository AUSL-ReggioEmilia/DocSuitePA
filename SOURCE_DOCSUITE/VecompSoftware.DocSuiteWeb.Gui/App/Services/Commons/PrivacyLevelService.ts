import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import PrivacyLevelModelMapper = require('App/Mappers/Commons/PrivacyLevelModelMapper');
import PrivacyLevelModel = require('App/Models/Commons/PrivacyLevelModel');

class PrivacyLevelService extends BaseService {
    _configuration: ServiceConfiguration;
    _mapper: PrivacyLevelModelMapper;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
        this._mapper = new PrivacyLevelModelMapper();
    }

    /**
     * Recupero tutti i privacyLevel
     * @param callback
     * @param error
     */
    findPrivacyLevels(filter: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "";
        if (filter && filter.length > 0) {
            qs = "$filter=contains(Description,'".concat(filter,"')&");
        }
        qs = qs.concat("$orderby=Level asc");
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let privacyLevels: PrivacyLevelModel[] = [];
                    if (response) {
                        privacyLevels = this._mapper.MapCollection(response.value);
                    }
                    callback(privacyLevels);
                }
            }, error);
    }

    getById(id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=UniqueId eq ".concat(id);
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let result: PrivacyLevelModel = this._mapper.Map(response.value[0]);
                    callback(result);
                }
            }, error);
    }

    /**
     * Inserisco un nuovo privacyLevel
     * @param model
     * @param callback
     * @param error
     */
    insertPrivacyLevel(model: PrivacyLevelModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }
    

    /**
     * Aggiorno un privacyLevel
     * @param model
     * @param callback
     * @param error
     */
    updatePrivacyLevel(model: PrivacyLevelModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }
}
export = PrivacyLevelService;