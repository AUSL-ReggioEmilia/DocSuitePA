import BaseMapper = require('App/Mappers/BaseMapper');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel')

class MetadataRepositoryModelMapper extends BaseMapper<MetadataRepositoryModel>{
    constructor() {
        super();
    }

    public Map(source: any): MetadataRepositoryModel {
        let toMap = <MetadataRepositoryModel>{};

        if (!source) {
            return null;
        }

        toMap = source;
        return toMap;
    }
}

export = MetadataRepositoryModelMapper