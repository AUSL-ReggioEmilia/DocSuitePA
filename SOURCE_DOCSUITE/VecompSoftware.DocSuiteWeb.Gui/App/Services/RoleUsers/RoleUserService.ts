import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import RoleUserModelMapper = require('../../Mappers/RoleUsers/RoleUserModelMapper');

class RoleUserService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
     * Costruttore
     * @param webApiUrl
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getRoleUserByRoleUniqueId(roleUniqueId: string, environment?: string, roleUserType?: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let urlPart: string = this._configuration.ODATAUrl;
        let filters: string = `?$filter=Role/UniqueId eq ${roleUniqueId}`;

        if (environment) {
            filters = `${filters} and DSWEnvironment eq '${environment}'`;
        }
        if (roleUserType) {
            filters = `${filters} and Type eq '${roleUserType}'`;
        }

        let url: string = urlPart.concat(filters);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let mapper: RoleUserModelMapper = new RoleUserModelMapper();
                callback(mapper.MapCollection(response.value));
            };
        }, error);
    }
}

export = RoleUserService;