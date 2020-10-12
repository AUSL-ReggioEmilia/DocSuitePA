import BaseMapper = require('App/Mappers/BaseMapper');
import DocumentSeriesItemLinksModel = require('App/Models/Series/DocumentSeriesItemLinksModel');

class DocumentSeriesItemLinksModelMapper extends BaseMapper<DocumentSeriesItemLinksModel>{
    constructor() {
        super();
    }

    public Map(source: any): DocumentSeriesItemLinksModel {
        let toMap: DocumentSeriesItemLinksModel = <DocumentSeriesItemLinksModel>{};

        if (!source) {
            return null;
        }

        toMap.EntityId = source.EntityId;
        toMap.LinkType = source.LinkType;
        toMap.EntityShortId = source.EntityShortId;
        toMap.UniqueId = source.UniqueId;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.Resolution = source.Resolution;

        return toMap;
    }
}

export =DocumentSeriesItemLinksModelMapper;