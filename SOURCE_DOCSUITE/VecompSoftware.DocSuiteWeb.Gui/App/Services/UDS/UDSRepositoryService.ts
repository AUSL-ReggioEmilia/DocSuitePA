import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import UDSRepositoryModelMapper = require('App/Mappers/UDS/UDSRepositoryModelMapper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');


class UDSRepositoryService extends BaseService {
    private _configuration: ServiceConfiguration;

    /**
     * Costruttore
     * @param webApiUrl
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    /**
     * Recupera una UDSRepository per Nome
     * @param name
     * @param callback
     * @param error
     */
    getUDSRepositoryByName(name: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=Name eq '".concat(name.toString(), "' and Status eq VecompSoftware.DocSuiteWeb.Entity.UDS.UDSRepositoryStatus'2'&$orderby=Version desc&$top=1");
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                callback(response.value[0]);
            }
        }, error);
    }

    /**
 * Recupera una UDSRepository per Nome
 * @param name
 * @param callback
 * @param error
 */
    getUDSRepositoryByDSWEnvironment(DSWEnvironment: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=DSWEnvironment eq ".concat(DSWEnvironment.toString(), "&$orderby=Version desc&$top=1");
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                callback(response.value[0]);
            }
        }, error);
    }

    /**
     * Recupera una UDSRepository per Environment
     * @param environment
     * @param callback
     * @param error
     */
    getUDSRepositoryByEnvironment(environment: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$filter=Environment eq '".concat(environment.toString(), "' and Status eq VecompSoftware.DocSuiteWeb.Entity.UDS.UDSRepositoryStatus'2'&$orderby=Version desc&$top=1");
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                callback(response.value[0]);
            }
        }, error);
    }

    getUDSRepositoryByUDSTypology(typologyId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("/?$filter=UDSTypologies/any(s:s/UniqueId eq ", typologyId, ")");
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let mapper = new UDSRepositoryModelMapper();
                callback(mapper.MapCollection(response.value));
            }
        }, error);
    }

    getUDSRepositoryByUDSTypologyName(typologyName: string, tentantName: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("/?$filter=UDSTypologies/any(s:s/Name eq '", typologyName, "')");
        if (tentantName != "") {
            url = url.concat(" and startsWith(Name", ",'", tentantName, " - ')");
        }
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let mapper = new UDSRepositoryModelMapper();
                callback(mapper.MapCollection(response.value));
            }
        }, error);
    }

    getUDSRepositories(tentantName: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("?$filter=Status eq VecompSoftware.DocSuiteWeb.Entity.UDS.UDSRepositoryStatus'2' and ExpiredDate eq null");
        if (tentantName != "") {
            url = url.concat(" and startsWith(Name", ",'", tentantName, " - ')");
        }
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let mapper = new UDSRepositoryModelMapper();
                callback(mapper.MapCollection(response.value));
            }
        }, error);
    }

    getTenantUDSRepositories(tenantName: string, udsName: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("/UDSRepositoryService.GetTenantUDSRepositories(tenantName='", tenantName, "',udsName='", udsName, "')");
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let mapper = new UDSRepositoryModelMapper();
                callback(mapper.MapCollection(response.value));
            }
        }, error);
    }

    getUDSRepositoryByID(udsID: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat("/?$filter=UniqueId eq ", udsID);
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let mapper = new UDSRepositoryModelMapper();
                callback(mapper.MapCollection(response.value));
            }
        }, error);
    }

    getAvailableCQRSPublishedUDSRepositories(typologyId: string, name: string, alias: string, idContainer: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let container: string = null;
        let nameUDSRepository: string = "";
        let aliasUDSRepository: string = "";
        if (idContainer) {
            container = idContainer.toString();
        }
        if (name) {
            nameUDSRepository = name.toString();
        }
        if (alias) {
            aliasUDSRepository = alias.toString();
        }
        url = url.concat("/UDSRepositoryService.GetAvailableCQRSPublishedUDSRepositories(idUDSTypology=", typologyId, ",name='", nameUDSRepository, "',alias='", aliasUDSRepository, "',idContainer=", container, ")");
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let mapper = new UDSRepositoryModelMapper();
                callback(mapper.MapCollection(response.value));
            }
        }, error);
    }

    getInsertableRepositoriesByTypology(username: string, domain: string, typologyId?: string, pecAnnexedEnabled?: boolean, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        if (!typologyId) {
            typologyId = null;
        }
        url = url.concat(`/UDSRepositoryService.GetInsertableRepositoriesByTypology(username='${username}',domain='${domain}',idUDSTypology=${typologyId},pecAnnexedEnabled=${pecAnnexedEnabled})`);
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let mapper = new UDSRepositoryModelMapper();
                callback(mapper.MapCollection(response.value));
            }
        }, error);
    }
}

export = UDSRepositoryService;