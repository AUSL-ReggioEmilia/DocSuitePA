interface TenantModel {
  UniqueId: string;
  TenantName: string;
  CompanyName: string;
  StartDate: Date;
  EndDate: Date;
  Note: string;
}

export = TenantModel;