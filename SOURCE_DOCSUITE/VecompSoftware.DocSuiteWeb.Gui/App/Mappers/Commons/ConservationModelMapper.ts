import BaseMapper = require('App/Mappers/BaseMapper');
import ConservationModel = require('App/Models/Commons/ConservationModel')
import ConservationStatusType = require('App/Models/Commons/ConservationStatusType');
import ConservationType = require('App/Models/Commons/ConservationType');

class ConservationModelMapper extends BaseMapper<ConservationModel>{
    constructor() {
        super();
    }

    public Map(source: any): ConservationModel {
        let toMap = <ConservationModel>{};

        if (!source) {
            return null;
        }

        toMap.EntityType = source.EntityType;
        toMap.Message = source.Message;
        toMap.SendDate = source.SendDate;
        toMap.Status = ConservationStatusType[source.Status as string];
        toMap.Type = ConservationType[source.Type as string];
        toMap.UniqueId = source.UniqueId;
        toMap.Uri = source.Uri;
        return toMap;
    }
}

export = ConservationModelMapper;