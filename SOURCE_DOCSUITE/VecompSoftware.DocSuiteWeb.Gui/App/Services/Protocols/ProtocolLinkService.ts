import BaseService = require("../BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ProtocolLinkModelMapper = require("App/Mappers/Protocols/ProtocolLinkModelMapper");

class ProtocolLinkService extends BaseService {
    _configuration: ServiceConfiguration;
    _mapper: ProtocolLinkModelMapper;

    /**
     * Costruttore
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
        this._mapper = new ProtocolLinkModelMapper();
    }

    getProtocolById(Id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$expand=ProtocolLinked&$filter=Protocol/UniqueId eq ".concat(Id);
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    countProtocolsById(Id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "/$count?$expand=ProtocolLinked&$filter=Protocol/UniqueId eq ".concat(Id);
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

}

export = ProtocolLinkService;