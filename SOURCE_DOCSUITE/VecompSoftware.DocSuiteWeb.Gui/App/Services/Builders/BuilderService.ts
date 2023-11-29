import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import BaseService = require("App/Services/BaseService");
import BuildActionModel = require("App/Models/Commons/BuildActionModel");

class BuilderService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    sendBuild(model: BuildActionModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        this.postRequest(this._configuration.WebAPIUrl, JSON.stringify(model), (response: any) => {
            if (callback) {
                callback(response);
            }
        }, error);
    }
}

export = BuilderService;