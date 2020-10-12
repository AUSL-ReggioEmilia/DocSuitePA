import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import TenantSearchFilterDTO = require('App/DTOs/TenantSearchFilterDTO');
import TenantAOOModelMapper = require('../../Mappers/Tenants/TenantAOOModelMapper');
import TenantAOOModel = require('../../Models/Tenants/TenantAOOModel');
import TenantTypologyTypeEnum = require('App/Models/Tenants/TenantTypologyTypeEnum');

class TenantAOOService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getTenantsWithoutCurrentTenant(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let urlPart: string = this._configuration.ODATAUrl;

        let oDataFilters: string = `?$expand=Tenants($filter=UniqueId ne ${uniqueId})`;
        let url: string = urlPart.concat(oDataFilters);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let mapper: TenantAOOModelMapper = new TenantAOOModelMapper();
                callback(mapper.MapCollection(response.value));
            };
        }, error);
    }

    getTenantsAOO(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let urlPart: string = this._configuration.ODATAUrl;

        let oDataFilters: string = `?$orderby=Name&$filter=TenantTypology eq '${TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant]}'`;

        let url: string = urlPart.concat(oDataFilters);
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let mapper: TenantAOOModelMapper = new TenantAOOModelMapper();
                callback(mapper.MapCollection(response.value));
            };
        }, error);
    }

    getFilteredTenants(searchFilter: TenantSearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}?$orderby=Name&$filter=TenantTypology eq '${TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant]}'&$expand=Tenants($orderby=TenantName`;
        let urlPart: string = "";
        if (searchFilter.tenantName && searchFilter.companyName) {
            urlPart += `;$filter=contains(TenantName, '${searchFilter.tenantName}') and contains(CompanyName, '${searchFilter.companyName}')`;
        }
        else if (searchFilter.tenantName) {
            urlPart += `;$filter=contains(TenantName, '${searchFilter.tenantName}')`;
        }
        else if (searchFilter.companyName) {
            urlPart += `;$filter=contains(CompanyName, '${searchFilter.companyName}')`;
        }

        if (searchFilter.isActive !== null) {
            let isActiveFilter: string = searchFilter.isActive ? "(EndDate ge now() or EndDate eq null)" : "EndDate le now()";
            urlPart += urlPart !== "" ? ` and ${isActiveFilter}` : `;$filter=${isActiveFilter}`;
        }
        url += urlPart !== "" ? `${urlPart})` : ")";
        //if (urlPart !== "") {
        //    url = url.replace("TenantName", `TenantName${urlPart}`);
        //}
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let mapper: TenantAOOModelMapper = new TenantAOOModelMapper();
                callback(mapper.MapCollection(response.value));
            };
        }, error);
    }

    getTenantsByTenantAOOId(tenantAOOId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = `${url}?$expand=Tenants($filter=TenantTypology eq '${TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant]}')&$filter=UniqueId eq ${tenantAOOId} and TenantTypology eq '${TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant]}'`;
        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let mapper: TenantAOOModelMapper = new TenantAOOModelMapper();
                callback(mapper.MapCollection(response.value));
            };
        }, error);
    }

    getTenantAOOById(tenantAOOId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = `${url}?$filter=UniqueId eq ${tenantAOOId} and TenantTypology eq '${TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant]}'`;

        this.getRequest(url, null, (response: any) => {
            if (callback && response) {
                let mapper: TenantAOOModelMapper = new TenantAOOModelMapper();
                callback(mapper.Map(response.value[0]));
            };
        }, error);
    }

    updateTenantAOO(model: TenantAOOModel,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;

        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    insertTenantAOO(model: TenantAOOModel,
        callback?: (data: any) => any,
        error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(model), callback, error);
    }
}

export = TenantAOOService;