import BaseMapper = require('App/Mappers/BaseMapper');
import ContainerModel = require('App/Models/Commons/ContainerModel');

class ContainerModelMapper extends BaseMapper<ContainerModel>{
    constructor() {
        super();
    }

    public Map(source: any): ContainerModel {
        let toMap = <ContainerModel>{};      

        if (!source) {
            return null;
        }

        toMap.EntityShortId = source.EntityShortId;
        toMap.Name = source.Name;
        toMap.Note = source.Note;
        toMap.isActive = source.isActive;
        toMap.Massive = source.Massive;
        toMap.Conservation = source.Conservation;
        toMap.DocumentSeriesAnnexedLocation = source.DocumentSeriesAnnexedLocation;
        toMap.DocumentSeriesLocation = source.DocumentSeriesLocation;
        toMap.DocumentSeriesUnpublishedAnnexedLocation = source.DocumentSeriesUnpublishedAnnexedLocation;
        toMap.ProtocolRejection = source.ProtocolRejection;
        toMap.ActiveFrom = source.ActiveFrom;
        toMap.ActiveTo = source.ActiveTo;
        toMap.idArchive = source.idArchive;
        toMap.Privacy = source.Privacy;
        toMap.HeadingFrontalino = source.HeadingFrontalino;
        toMap.HeadingLetter = source.HeadingLetter;
        toMap.ProtAttachLocation = source.ProtAttachLocation;
        toMap.ProtLocation = source.ProtLocation;
        toMap.ReslLocation = source.ReslLocation;         
        
        return toMap;
    }
}

export = ContainerModelMapper;