import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import DocumentUnitModelMapper = require('App/Mappers/DocumentUnits/DocumentUnitModelMapper');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import Environment = require('App/Models/Environment');
import DocumentUnitSearchFilterDTO = require('App/DTOs/DocumentUnitSearchFilterDTO');
import PaginationModel = require('App/Models/Commons/PaginationModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');

class DocumentUnitService extends BaseService {
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
     * Recupera le Document Units per anno/numero e utente specifico
     * @param year
     * @param number
     * @param username
     * @param domain
     * @param isSecurityEnabled
     * @param callback
     * @param error
     */
    findDocumentUnits(finderModel: DocumentUnitSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`/DocumentUnitService.AuthorizedDocumentUnits(finder=@p0)?@p0=${JSON.stringify(finderModel)}`);
        this.getRequest(url, null, (response: any) => {
            let instances: Array<DocumentUnitModel> = new Array<DocumentUnitModel>();
            let mapper = new DocumentUnitModelMapper();
            instances = mapper.MapCollection(response.value);
            if (callback) callback(instances);
        }, error);
    }

    /**
   * Recupera il count totale delle document units
   */
    countDocumentUnits(finderModel: DocumentUnitSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`/DocumentUnitService.CountAuthorizedDocumentUnits(finder=@p0)?@p0=${JSON.stringify(finderModel)}`);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                callback(response.value);
            };
        }, error);
    }

    /**
     * Recupera le Document Units associate al Fascicolo
     * TODO: da implementare in SignalR
     * @param model
     * @param qs
     * @param callback
     * @param error
     */
    getFascicleDocumentUnits(model: FascicleModel, qs: string, idTenantAOO: string, idFascicleFolder?: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        if (!idFascicleFolder) {
            idFascicleFolder = null;
        }
        let odataUrl: string = this._configuration.ODATAUrl;
        let odataQuery = `${odataUrl}/DocumentUnitService.FascicleDocumentUnits(idFascicle=@p1,idFascicleFolder=@p2,idTenantAOO=@p3)?@p1=${model.UniqueId}&@p2=${idFascicleFolder}&@p3=${idTenantAOO}`;

        this.getRequest(odataQuery, qs,
            (response: any) => {

                if (!callback) {
                    return;
                }

                let docUnitMapper: DocumentUnitModelMapper = new DocumentUnitModelMapper();
                let fascicleFolderDocUnits = [];
                if (response && response.value) {
                    fascicleFolderDocUnits = docUnitMapper.MapCollection(response.value);
                }

                callback(fascicleFolderDocUnits);
            }, error);
    }

    getDocumentUnitEnvironment(documentUnit: DocumentUnitModel): Environment {
        let env: Environment = (documentUnit.Environment < 100 ? <Environment>documentUnit.Environment : Environment.UDS);
        return env;
    }

    getDocumentUnitById(idDocumentUnit: string, callback?: (data: DocumentUnitModel) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=UniqueId eq ${idDocumentUnit}&$expand=Category,Container,UDSRepository,DocumentUnitRoles,DocumentUnitChains`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let mapper = new DocumentUnitModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
    }

    getDocumentUnitsFullText(fullTextSearch: string, idTenant: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        fullTextSearch = fullTextSearch.replace("'", "''");
        let url: string = this._configuration.ODATAUrl.concat(`/DocumentUnitService.FullTextSearchDocumentUnits(filter='${fullTextSearch}',idTenant=${idTenant})`);
        this.getRequest(url, null, (response: any) => {
            if (callback) callback(response.value);
        }, error);
    }

    countTenantFascicleDocumentUnits(tenantAOOId: string, fascicleId: string, fascicleFolderId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        const baseOdataURL: string = `${this._configuration.ODATAUrl}/$count`;
        let odataQuery: string = `$filter=TenantAOO/UniqueId eq ${tenantAOOId} and FascicleDocumentUnits/any(fdu: fdu/Fascicle/UniqueId eq ${fascicleId} and fdu/FascicleFolder/UniqueId eq ${fascicleFolderId})`;

        this.getRequest(baseOdataURL, odataQuery,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    getTenantFascicleDocumentUnits(tenantAOOId: string, fascicleId: string, fascicleFolderId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, paginationModel: PaginationModel = null): void {
        let url: string = this._configuration.ODATAUrl;
        let odataQuery: string = `$filter=TenantAOO/UniqueId eq ${tenantAOOId} and FascicleDocumentUnits/any(fdu: fdu/Fascicle/UniqueId eq ${fascicleId} and fdu/FascicleFolder/UniqueId eq ${fascicleFolderId})`;

        if (paginationModel) {
            odataQuery = `${odataQuery}&$skip=${paginationModel.Skip}&$top=${paginationModel.Take}&$count=true`;
        }

        this.getRequest(url, odataQuery,
            (response: any) => {
                if (!callback) {
                    return;
                }

                let docUnitMapper: DocumentUnitModelMapper = new DocumentUnitModelMapper();
                let fascicleFolderDocUnits = [];
                if (response && response.value) {
                    fascicleFolderDocUnits = docUnitMapper.MapCollection(response.value);
                }

                if (!paginationModel) {
                    callback(response.value);
                    return;
                }

                const odataResult: ODATAResponseModel<DocumentUnitModel> = new ODATAResponseModel<DocumentUnitModel>(response);
                odataResult.value = fascicleFolderDocUnits;
                callback(odataResult);
            }, error);
    }
}

export = DocumentUnitService;