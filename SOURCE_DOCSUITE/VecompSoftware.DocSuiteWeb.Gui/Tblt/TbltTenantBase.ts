/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import TenantService = require('App/Services/Tenants/TenantService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ContainerService = require("App/Services/Commons/ContainerService");
import PECMailBoxService = require("App/Services/PECMails/PECMailBoxService");
import RoleService = require("App/Services/Commons/RoleService");
import WorkflowRepositoryService = require("App/Services/Workflows/WorkflowRepositoryService");
import TenantConfigurationService = require("App/Services/Tenants/TenantConfigurationService");
import TenantWorkflowRepositoryService = require("App/Services/Tenants/TenantWorkflowRepositoryService");
import TenantAOOService = require('App/Services/Tenants/TenantAOOService');


class TbltTenantBase {

  protected static Tenant_TYPE_NAME = "Tenant";
  protected static Container_TYPE_NAME = "Container";
  protected static PECMailBox_TYPE_NAME = "PECMailBox";
  protected static Role_TYPE_NAME = "Role";
  protected static WorkflowRepository_TYPE_NAME = "WorkflowRepository";
  protected static TenantConfiguration_TYPE_NAME = "TenantConfiguration";
  protected static TenantWorkflowRepository_TYPE_NAME = "TenantWorkflowRepository";
  protected static TenantAOO_TYPE_NAME = "TenantAOO";


  private _serviceConfigurations: ServiceConfiguration[];
  protected _tenantService: TenantService;
  protected _containerService: ContainerService;
  protected _pecMailBoxService: PECMailBoxService;
  protected _roleService: RoleService;
  protected _workflowRepositoryService: WorkflowRepositoryService;
  protected _tenantConfigurationService: TenantConfigurationService;
  protected _tenantWorkflowRepositoryService: TenantWorkflowRepositoryService;
  protected _tenantAOOService: TenantAOOService;


  constructor(serviceConfigurations: ServiceConfiguration[]) {
    this._serviceConfigurations = serviceConfigurations;
  }

  initialize(): void {
    let tenantConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.Tenant_TYPE_NAME);
    let containerConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.Container_TYPE_NAME);
    let pecMailBoxConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.PECMailBox_TYPE_NAME);
    let roleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.Role_TYPE_NAME);
    let workflowRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.WorkflowRepository_TYPE_NAME);
    let tenantConfigurationConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.TenantConfiguration_TYPE_NAME);
    let tenantWorkflowRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.TenantWorkflowRepository_TYPE_NAME);
    let tenantAOOConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltTenantBase.TenantAOO_TYPE_NAME);

    this._tenantService = new TenantService(tenantConfiguration);
    this._containerService = new ContainerService(containerConfiguration);
    this._pecMailBoxService = new PECMailBoxService(pecMailBoxConfiguration);
    this._roleService = new RoleService(roleConfiguration);
    this._workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);
    this._tenantConfigurationService = new TenantConfigurationService(tenantConfigurationConfiguration);
    this._tenantWorkflowRepositoryService = new TenantWorkflowRepositoryService(tenantWorkflowRepositoryConfiguration);
    this._tenantAOOService = new TenantAOOService(tenantAOOConfiguration);
  }

  protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
    if (exception && exception instanceof ExceptionDTO) {
      let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
      if (!jQuery.isEmptyObject(uscNotification)) {
        uscNotification.showNotification(exception);
      }
    }
    else {
      this.showNotificationMessage(uscNotificationId, customMessage)
    }
  }

  protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
    let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
    if (!jQuery.isEmptyObject(uscNotification)) {
      uscNotification.showNotificationMessage(customMessage);
    }
  }

}

export = TbltTenantBase;