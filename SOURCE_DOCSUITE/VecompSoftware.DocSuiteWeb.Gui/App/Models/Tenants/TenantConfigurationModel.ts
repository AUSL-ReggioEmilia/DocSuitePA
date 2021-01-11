import TenantViewModel = require("App/ViewModels/Tenants/TenantViewModel");

class TenantConfigurationModel {
  UniqueId: string;
  Tenant: TenantViewModel;
  ConfigurationType: string;
  StartDate: string;
  EndDate: string;
  Note: string;
  JsonValue: string;
}

export = TenantConfigurationModel;