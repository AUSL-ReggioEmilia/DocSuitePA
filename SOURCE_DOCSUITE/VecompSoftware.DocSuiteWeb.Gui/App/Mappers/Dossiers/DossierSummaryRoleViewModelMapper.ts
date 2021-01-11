import BaseMapper = require('App/Mappers/BaseMapper');
import BaseEntityRoleViewModel = require('App/ViewModels/BaseEntityRoleViewModel');

class DossierSummaryRoleViewModelMapper extends BaseMapper<BaseEntityRoleViewModel>{
    constructor() {
        super();
    }

    public Map(source: any): BaseEntityRoleViewModel {
        let toMap = new BaseEntityRoleViewModel();
        if (!source) {
            return null;
        }
        toMap.UniqueId = source.UniqueId;
        toMap.EntityShortId = source.Role.IdRole ? source.Role.IdRole:null;
        toMap.Name = source.Role.Name;
        toMap.Type = source.Type;
        toMap.IsMaster = source.IsMaster;
        toMap.IsActive = source.IsActive;

        return toMap;
    }
    
}

export = DossierSummaryRoleViewModelMapper;