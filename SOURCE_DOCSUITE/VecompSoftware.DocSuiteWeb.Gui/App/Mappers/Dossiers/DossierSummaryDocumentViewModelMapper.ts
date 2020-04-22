import IMapper = require('App/Mappers/IMapper');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');

class DossierSummaryDocumentViewModelMapper implements IMapper<BaseEntityViewModel>{
    constructor() {
    }

    public Map(source: any): BaseEntityViewModel {
        let toMap = new BaseEntityViewModel();
        if (!source) {
            return null;
        }
        toMap.UniqueId = source.UniqueId;
        toMap.IdArchiveChain = source.IdArchiveChain;

        return toMap;
    }

}

export = DossierSummaryDocumentViewModelMapper;