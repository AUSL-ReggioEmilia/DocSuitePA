import TenantModel = require('App/Models/Tenants/TenantModel');
import IMapper = require('App/Mappers/IMapper');

class TenantMapper implements IMapper<TenantModel>{
  constructor() {
  }
  public Map(source: any): TenantModel {
    let toMap: TenantModel = <TenantModel>{};

    if (!source) {
      return null;
    }

    toMap.UniqueId = source.UniqueId;
    toMap.TenantName = source.TenantName;
    toMap.CompanyName = source.CompanyName;
    toMap.StartDate = source.StartDate;
    toMap.EndDate = source.EndDate;
    toMap.Note = source.Note;
    return toMap;
  }
}

export = TenantMapper;