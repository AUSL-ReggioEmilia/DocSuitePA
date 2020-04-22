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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Workflows/WorkflowRepositoryModelMapper"], function (require, exports, BaseService, WorkflowRepositoryModelMapper) {
    var TenantWorkflowRepositoryService = /** @class */ (function (_super) {
        __extends(TenantWorkflowRepositoryService, _super);
        function TenantWorkflowRepositoryService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        TenantWorkflowRepositoryService.prototype.getTenantWorkflowRepositories = function (callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=Tenant,WorkflowRepository");
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
                ;
            }, error);
        };
        TenantWorkflowRepositoryService.prototype.getTenantWorkflowRepositoryById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl + "?$expand=Tenant,WorkflowRepository&filter=UniqueId eq " + uniqueId;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
                ;
            }, error);
        };
        TenantWorkflowRepositoryService.prototype.insertTenantWorkflowRepository = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        TenantWorkflowRepositoryService.prototype.updateTenantWorkflowRepository = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        TenantWorkflowRepositoryService.prototype.deleteTenantWorkflowRepository = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        TenantWorkflowRepositoryService.prototype.getAllWorkflowRepositories = function (uniqueId, name, topElement, skipElement, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$expand=Tenant,WorkflowRepository&$filter=contains(WorkflowRepository/Name,'" + name + "') and Tenant/UniqueId eq " + uniqueId);
            var qs = " and &$count=true&$top=" + topElement + "&$skip=" + skipElement.toString();
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var modelMapper_1 = new WorkflowRepositoryModelMapper();
                    var tenantWorkflowRepo_1 = [];
                    $.each(response.value, function (i, value) {
                        tenantWorkflowRepo_1.push(modelMapper_1.Map(value.WorkflowRepository));
                    });
                    callback(tenantWorkflowRepo_1);
                }
            }, error);
        };
        return TenantWorkflowRepositoryService;
    }(BaseService));
    return TenantWorkflowRepositoryService;
});
//# sourceMappingURL=TenantWorkflowRepositoryService.js.map