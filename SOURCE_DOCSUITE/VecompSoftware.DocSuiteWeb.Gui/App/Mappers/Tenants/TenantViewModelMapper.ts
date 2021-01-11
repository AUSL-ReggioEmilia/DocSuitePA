import TenantViewModel = require('App/ViewModels/Tenants/TenantViewModel');
import IMapper = require('App/Mappers/IMapper');
import BaseMapper = require("../BaseMapper");
import TenantAOOModelMapper = require('./TenantAOOModelMapper');

class TenantViewModelMapper extends BaseMapper<TenantViewModel> implements IMapper<TenantViewModel>{
    constructor() {
        super();
    }
    public Map(source: any): TenantViewModel {
        let toMap: TenantViewModel = <TenantViewModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.TenantName = source.TenantName;
        toMap.CompanyName = source.CompanyName;
        toMap.StartDate = source.StartDate;
        toMap.EndDate = source.EndDate;
        toMap.Note = source.Note;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.LastChangedDate = source.LastChangedDate;
        toMap.TenantAOO = source.TenantAOO ? new TenantAOOModelMapper().Map(source.TenantAOO) : null;
        toMap.TenantTypology = source.TenantTypology;
        return toMap;
    }
}

export = TenantViewModelMapper;