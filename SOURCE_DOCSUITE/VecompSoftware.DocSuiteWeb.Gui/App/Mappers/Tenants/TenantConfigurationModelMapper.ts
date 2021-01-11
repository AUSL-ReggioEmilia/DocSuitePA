import TenantConfigurationModel = require('App/Models/Tenants/TenantConfigurationModel');
import IMapper = require('App/Mappers/IMapper');

class TenantConfigurationModelMapper implements IMapper<TenantConfigurationModel>{
  constructor() {
  }
  public Map(source: any): TenantConfigurationModel {
    let toMap: TenantConfigurationModel = <TenantConfigurationModel>{};

    if (!source) {
      return null;
    }

    toMap.UniqueId = source.UniqueId;
    toMap.Tenant = source.Tenant;
    toMap.ConfigurationType = source.ConfigurationType;
    toMap.StartDate = source.StartDate;
    toMap.EndDate = source.EndDate;
    toMap.Note = source.Note;
    toMap.JsonValue = source.JsonValue;
    return toMap;
  }
}

export = TenantConfigurationModelMapper;