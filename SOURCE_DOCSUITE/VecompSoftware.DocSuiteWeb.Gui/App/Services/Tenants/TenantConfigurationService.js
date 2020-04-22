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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Tenants/TenantConfigurationModelMapper"], function (require, exports, BaseService, TenantConfigurationModelMapper) {
    var TenantConfigurationService = /** @class */ (function (_super) {
        __extends(TenantConfigurationService, _super);
        function TenantConfigurationService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        TenantConfigurationService.prototype.getTenantConfigurations = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var modelMapper_1 = new TenantConfigurationModelMapper();
                    var tenantConfigurations_1 = [];
                    $.each(response.value, function (i, value) {
                        tenantConfigurations_1.push(modelMapper_1.Map(value));
                    });
                    callback(tenantConfigurations_1);
                }
                ;
            }, error);
        };
        TenantConfigurationService.prototype.updateTenantConfiguration = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        return TenantConfigurationService;
    }(BaseService));
    return TenantConfigurationService;
});
//# sourceMappingURL=TenantConfigurationService.js.map