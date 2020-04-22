/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/PECMails/PECMailService", "App/Helpers/ServiceConfigurationHelper", "App/Services/Tenants/TenantService", "App/Services/Tenants/TenantWorkflowRepositoryService", "App/Services/PECMails/PECMailBoxService"], function (require, exports, PECMailService, ServiceConfigurationHelper, TenantService, TenantWorkflowRepositoryService, PECMailBoxService) {
    var PECInvoiceBase = /** @class */ (function () {
        function PECInvoiceBase(serviceConfiguration) {
            this._serviceConfigurations = serviceConfiguration;
        }
        PECInvoiceBase.prototype.initialize = function () {
            var pecMailConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, PECInvoiceBase.PECMailInvoice_TYPE_NAME);
            var pecMailBoxConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, PECInvoiceBase.PECMailBoxInvoice_TYPE_NAME);
            var tenantConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, PECInvoiceBase.Tenant_TYPE_NAME);
            var tenantWorkflowRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, PECInvoiceBase.TenantWorkflowRepository_TYPE_NAME);
            this._pecMailService = new PECMailService(pecMailConfiguration);
            this._pecMailBoxService = new PECMailBoxService(pecMailBoxConfiguration);
            this._tenantService = new TenantService(tenantConfiguration);
            this._tenantWorkflowRepositoryService = new TenantWorkflowRepositoryService(tenantWorkflowRepositoryConfiguration);
        };
        PECInvoiceBase.PECMailInvoice_TYPE_NAME = "PECMail";
        PECInvoiceBase.PECMailBoxInvoice_TYPE_NAME = "PECMailBox";
        PECInvoiceBase.Tenant_TYPE_NAME = "Tenant";
        PECInvoiceBase.TenantWorkflowRepository_TYPE_NAME = "TenantWorkflowRepository";
        return PECInvoiceBase;
    }());
    return PECInvoiceBase;
});
//# sourceMappingURL=PECInvoiceBase.js.map