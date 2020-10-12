
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class FascicleRoleService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }
    insertFascicleRole(model: FascicleRoleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }

    deleteFascicleRole(model: FascicleRoleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }

    updateFascicleRole(model: FascicleRoleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(model), callback, error);
    }
  }

export = FascicleRoleService;