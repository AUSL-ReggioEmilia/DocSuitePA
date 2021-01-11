import IMapper = require('App/Mappers/IMapper');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');

class DossierSummaryContactViewModelMapper implements IMapper<BaseEntityViewModel>{
    constructor() {
    }

    public Map(source: any): BaseEntityViewModel {
        let toMap = new BaseEntityViewModel();
        if (!source) {
            return null;
        }
        toMap.UniqueId = source.UniqueId;
        toMap.EntityShortId = source.IdContact;
        toMap.Name = source.Description;
        toMap.Type = source.ContactType;
        toMap.IncrementalFather = source.IncrementalFather;

        return toMap;
    }

}

export = DossierSummaryContactViewModelMapper;