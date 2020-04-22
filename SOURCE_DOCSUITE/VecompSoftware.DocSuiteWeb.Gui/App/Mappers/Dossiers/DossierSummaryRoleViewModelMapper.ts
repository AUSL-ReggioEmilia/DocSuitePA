import BaseMapper = require('App/Mappers/BaseMapper');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');

class DossierSummaryRoleViewModelMapper extends BaseMapper<BaseEntityViewModel>{
    constructor() {
        super();
    }

    public Map(source: any): BaseEntityViewModel {
        let toMap = new BaseEntityViewModel();
        if (!source) {
            return null;
        }
        toMap.UniqueId = source.Role ? source.Role.UniqueId : null;
        toMap.EntityShortId = source.Role.IdRole ? source.Role.IdRole:null;
        toMap.Name = source.Role.Name;
        toMap.Type = source.Type;

        return toMap;
    }
    
}

export = DossierSummaryRoleViewModelMapper;