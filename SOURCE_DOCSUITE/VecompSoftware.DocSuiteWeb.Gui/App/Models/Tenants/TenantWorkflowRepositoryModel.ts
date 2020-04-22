import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import TenantViewModel = require('App/ViewModels/Tenants/TenantViewModel');

interface TenantWorkflowRepositoryModel {
  UniqueId: string;
  JsonValue: string;
  IntegrationModuleName: string;
  Conditions: string;
  ConfigurationType: string;
  StartDate: string;
  EndDate: string;
  RegistrationUser: string;
  RegistrationDate?: Date;
  LastChangedUser: string;
  LastChangedDate?: Date;
  WorkflowRepository: WorkflowRepositoryModel;
  Tenant: TenantViewModel;
}

export = TenantWorkflowRepositoryModel;