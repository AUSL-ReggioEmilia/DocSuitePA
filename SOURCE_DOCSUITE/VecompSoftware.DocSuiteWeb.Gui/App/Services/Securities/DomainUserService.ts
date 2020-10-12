import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class DomainUserService extends BaseService {
    private _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getUser(account: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let username: string = account.substring(account.lastIndexOf("\\") + 1);
        let domain: string = account.substring(0, account.indexOf("\\"));
        let url: string = this._configuration.ODATAUrl.concat("/DomainUserService.GetUser(username='", username, "',domain='", domain, "')");
        let data: string = "";
        this.getRequest(url, data, (response: any) => {
            if (callback) callback(response);
        }, error);
    }

    userFinder(account: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let username: string = account.substring(account.lastIndexOf("\\") + 1);
        let url: string = this._configuration.ODATAUrl.concat("/DomainUserService.UsersFinder(text='", username, "')");
        let data: string = "";
        this.getRequest(url, data, (response: any) => {
            if (callback) callback(response.value);
        }, error);
    }
}

export = DomainUserService;