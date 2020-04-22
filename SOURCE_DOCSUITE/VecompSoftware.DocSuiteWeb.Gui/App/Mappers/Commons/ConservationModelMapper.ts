import BaseMapper = require('App/Mappers/BaseMapper');
import ConservationModel = require('App/Models/Commons/ConservationModel')

class ConservationModelMapper extends BaseMapper<ConservationModel>{
    constructor() {
        super();
    }

    public Map(source: any): ConservationModel {
        let toMap = <ConservationModel>{};

        if (!source) {
            return null;
        }

        toMap = source;
        return toMap;
    }
}

export = ConservationModelMapper;