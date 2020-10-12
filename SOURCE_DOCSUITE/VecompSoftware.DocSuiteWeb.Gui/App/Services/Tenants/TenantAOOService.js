var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Services/BaseService", "../../Mappers/Tenants/TenantAOOModelMapper", "App/Models/Tenants/TenantTypologyTypeEnum"], function (require, exports, BaseService, TenantAOOModelMapper, TenantTypologyTypeEnum) {
    var TenantAOOService = /** @class */ (function (_super) {
        __extends(TenantAOOService, _super);
        function TenantAOOService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        TenantAOOService.prototype.getTenantsWithoutCurrentTenant = function (uniqueId, callback, error) {
            var urlPart = this._configuration.ODATAUrl;
            var oDataFilters = "?$expand=Tenants($filter=UniqueId ne " + uniqueId + ")";
            var url = urlPart.concat(oDataFilters);
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var mapper = new TenantAOOModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
                ;
            }, error);
        };
        TenantAOOService.prototype.getTenantsAOO = function (callback, error) {
            var urlPart = this._configuration.ODATAUrl;
            var oDataFilters = "?$orderby=Name&$filter=TenantTypology eq '" + TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant] + "'";
            var url = urlPart.concat(oDataFilters);
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var mapper = new TenantAOOModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
                ;
            }, error);
        };
        TenantAOOService.prototype.getFilteredTenants = function (searchFilter, callback, error) {
            var url = this._configuration.ODATAUrl + "?$orderby=Name&$filter=TenantTypology eq '" + TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant] + "'&$expand=Tenants($orderby=TenantName";
            var urlPart = "";
            if (searchFilter.tenantName && searchFilter.companyName) {
                urlPart += ";$filter=contains(TenantName, '" + searchFilter.tenantName + "') and contains(CompanyName, '" + searchFilter.companyName + "')";
            }
            else if (searchFilter.tenantName) {
                urlPart += ";$filter=contains(TenantName, '" + searchFilter.tenantName + "')";
            }
            else if (searchFilter.companyName) {
                urlPart += ";$filter=contains(CompanyName, '" + searchFilter.companyName + "')";
            }
            if (searchFilter.isActive !== null) {
                var isActiveFilter = searchFilter.isActive ? "(EndDate ge now() or EndDate eq null)" : "EndDate le now()";
                urlPart += urlPart !== "" ? " and " + isActiveFilter : ";$filter=" + isActiveFilter;
            }
            url += urlPart !== "" ? urlPart + ")" : ")";
            //if (urlPart !== "") {
            //    url = url.replace("TenantName", `TenantName${urlPart}`);
            //}
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var mapper = new TenantAOOModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
                ;
            }, error);
        };
        TenantAOOService.prototype.getTenantsByTenantAOOId = function (tenantAOOId, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url + "?$expand=Tenants($filter=TenantTypology eq '" + TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant] + "')&$filter=UniqueId eq " + tenantAOOId + " and TenantTypology eq '" + TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant] + "'";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var mapper = new TenantAOOModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
                ;
            }, error);
        };
        TenantAOOService.prototype.getTenantAOOById = function (tenantAOOId, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url + "?$filter=UniqueId eq " + tenantAOOId + " and TenantTypology eq '" + TenantTypologyTypeEnum[TenantTypologyTypeEnum.InternalTenant] + "'";
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var mapper = new TenantAOOModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
                ;
            }, error);
        };
        TenantAOOService.prototype.updateTenantAOO = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        TenantAOOService.prototype.insertTenantAOO = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        return TenantAOOService;
    }(BaseService));
    return TenantAOOService;
});
//# sourceMappingURL=TenantAOOService.js.map