
import MassimarioScartoModel = require('App/Models/MassimariScarto/MassimarioScartoModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import MassimarioScartoModelMapper = require('App/Mappers/MassimariScarto/MassimarioScartoModelMapper');

class MassimarioScartoService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getMassimariByParent(includeCancel: boolean, parentId?: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/MassimariScartoService.GetAllChildren(parentId=", parentId, ",includeLogicalDelete=", String(includeCancel), ")");
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let instances: Array<MassimarioScartoModel> = new Array<MassimarioScartoModel>();
                let mapper = new MassimarioScartoModelMapper();
                instances = mapper.MapCollection(response.value);

                callback(instances);
            }
        }, error);
    }

    getMassimarioById(massimarioId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=UniqueId eq ".concat(massimarioId);
        this.getRequest(url, qs, (response: any) => {
            if (callback) {
                let mapper = new MassimarioScartoModelMapper();                
                callback(mapper.Map(response.value[0]));
            }
        }, error);
    }

    findMassimari(name: string, includeCancel: boolean, code?: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let codeQs: string = code != undefined ? ",fullCode='".concat(code, "'") : "";
        let url: string = this._configuration.ODATAUrl.concat("/MassimariScartoService.GetMassimari(name='", name, "'", codeQs, ",includeLogicalDelete=", String(includeCancel), ")");
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let instances: Array<MassimarioScartoModel> = new Array<MassimarioScartoModel>();
                let mapper = new MassimarioScartoModelMapper();
                instances = mapper.MapCollection(response.value);

                callback(instances);
            }
        }, error);
    }

    insertMassimario(model: MassimarioScartoModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    updateMassimario(model: MassimarioScartoModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }
}

export = MassimarioScartoService;