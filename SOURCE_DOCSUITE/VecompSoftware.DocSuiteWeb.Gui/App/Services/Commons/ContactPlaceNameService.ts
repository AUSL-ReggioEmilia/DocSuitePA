import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");

class ContactPlaceNameService extends BaseService {
    private readonly _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getAll(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {
                callback(response.value);
            }
        }, error);
    }
}

export = ContactPlaceNameService;