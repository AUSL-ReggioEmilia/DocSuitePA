/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Tenants/TenantService", "App/DTOs/ExceptionDTO", "App/Services/Commons/ContainerService", "App/Services/PECMails/PECMailBoxService", "App/Services/Commons/RoleService", "App/Services/Workflows/WorkflowRepositoryService", "App/Services/Tenants/TenantConfigurationService", "App/Services/Tenants/TenantWorkflowRepositoryService", "App/Services/Tenants/TenantAOOService"], function (require, exports, ServiceConfigurationHelper, TenantService, ExceptionDTO, ContainerService, PECMailBoxService, RoleService, WorkflowRepositoryService, TenantConfigurationService, TenantWorkflowRepositoryService, TenantAOOService) {
    var TbltTenantBase = /** @class */ (function () {
        function TbltTenantBase(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
        }
        TbltTenantBase.prototype.initialize = function () {
            var tenantConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.Tenant_TYPE_NAME);
            var containerConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.Container_TYPE_NAME);
            var pecMailBoxConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.PECMailBox_TYPE_NAME);
            var roleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.Role_TYPE_NAME);
            var workflowRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.WorkflowRepository_TYPE_NAME);
            var tenantConfigurationConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.TenantConfiguration_TYPE_NAME);
            var tenantWorkflowRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.TenantWorkflowRepository_TYPE_NAME);
            var tenantAOOConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.TenantAOO_TYPE_NAME);
            this._tenantService = new TenantService(tenantConfiguration);
            this._containerService = new ContainerService(containerConfiguration);
            this._pecMailBoxService = new PECMailBoxService(pecMailBoxConfiguration);
            this._roleService = new RoleService(roleConfiguration);
            this._workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);
            this._tenantConfigurationService = new TenantConfigurationService(tenantConfigurationConfiguration);
            this._tenantWorkflowRepositoryService = new TenantWorkflowRepositoryService(tenantWorkflowRepositoryConfiguration);
            this._tenantAOOService = new TenantAOOService(tenantAOOConfiguration);
        };
        TbltTenantBase.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#".concat(uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        TbltTenantBase.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TbltTenantBase.Tenant_TYPE_NAME = "Tenant";
        TbltTenantBase.Container_TYPE_NAME = "Container";
        TbltTenantBase.PECMailBox_TYPE_NAME = "PECMailBox";
        TbltTenantBase.Role_TYPE_NAME = "Role";
        TbltTenantBase.WorkflowRepository_TYPE_NAME = "WorkflowRepository";
        TbltTenantBase.TenantConfiguration_TYPE_NAME = "TenantConfiguration";
        TbltTenantBase.TenantWorkflowRepository_TYPE_NAME = "TenantWorkflowRepository";
        TbltTenantBase.TenantAOO_TYPE_NAME = "TenantAOO";
        return TbltTenantBase;
    }());
    return TbltTenantBase;
});
//# sourceMappingURL=TbltTenantBase.js.map