import BaseMapper = require('App/Mappers/BaseMapper');
import PrivacyLevelModel = require('App/Models/Commons/PrivacyLevelModel')

class PrivacyLevelModelMapper extends BaseMapper<PrivacyLevelModel>{
    constructor() {
        super();
    }

    public Map(source: any): PrivacyLevelModel {
        let toMap = <PrivacyLevelModel>{};

        if (!source) {
            return null;
        }

        toMap = source;
        return toMap;
    }
}

export = PrivacyLevelModelMapper;