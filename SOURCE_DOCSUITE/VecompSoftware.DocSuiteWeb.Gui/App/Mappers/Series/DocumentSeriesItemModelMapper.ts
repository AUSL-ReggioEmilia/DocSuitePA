import DocumentSeriesItemModel = require('App/Models/Series/DocumentSeriesItemModel');
import BaseMapper = require('App/Mappers/BaseMapper');

class DocumentSeriesItemModelMapper extends BaseMapper<DocumentSeriesItemModel>{
    constructor() {
        super();
    }

    public Map(source: any): DocumentSeriesItemModel {
        let toMap: DocumentSeriesItemModel = <DocumentSeriesItemModel>{}; 

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.Year = source.Year;
        toMap.Number = source.Number;
        toMap.Status = source.Status;
        toMap.Subject = source.Subject;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RetireDate = source.RetireDate;
        toMap.PublishingDate = source.PublishingDate;
        toMap.EntityId = source.EntityId;
        toMap.Messages = source.Messages;
        toMap.DocumentSeriesItemLinks = source.DocumentSeriesItemLinks;
        toMap.Protocols = source.Protocols;
        return toMap;
    }
}

export = DocumentSeriesItemModelMapper;