import BaseMapper = require('App/Mappers/BaseMapper');
import MetadataRepositoryViewModel = require('App/ViewModels/Commons/MetadataRepositoryViewModel');


class MetadataRepositoryViewModelMapper extends BaseMapper<MetadataRepositoryViewModel>{
    constructor() {
        super();
    }

    public Map(source): MetadataRepositoryViewModel {
        if (!source){
            return null;
        }
        let toMap: MetadataRepositoryViewModel = <MetadataRepositoryViewModel>{};
        toMap = source;
        return toMap;
    }
}
export = MetadataRepositoryViewModelMapper;