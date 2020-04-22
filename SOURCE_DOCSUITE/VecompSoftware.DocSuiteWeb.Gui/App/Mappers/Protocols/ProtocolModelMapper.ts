import BaseMapper = require('App/Mappers/BaseMapper');
import CategoryModelMapper = require('App/Mappers/Commons/CategoryModelMapper');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');
import ProtocolModel = require('App/Models/Protocols/ProtocolModel');

class ProtocolModelMapper extends BaseMapper<ProtocolModel> {
    constructor() {
        super();
    }

    public Map(source: any): ProtocolModel {
        let toMap: ProtocolModel = <ProtocolModel>{};

        if (!source) {
            return null;
        }

        toMap.Year = source.Year;
        toMap.Number = source.Number;
        toMap.Object = source.Object;
        toMap.ObjectChangeReason = source.ObjectChangeReason;
        toMap.DocumentDate = source.DocumentDate;
        toMap.DocumentProtocol = source.DocumentProtocol;
        toMap.IdDocument = source.IdDocument;
        toMap.IdAttachments = source.IdAttachments;
        toMap.DocumentCode = source.DocumentCode;
        toMap.IdStatus = source.IdStatus;
        toMap.LastChangedReason = source.LastChangedReason;
        toMap.AlternativeRecipient = source.AlternativeRecipient;
        toMap.CheckPublication = source.CheckPublication;
        toMap.JournalDate = source.JournalDate;
        toMap.ConservationStatus = source.ConservationStatus;
        toMap.LastConservationDate = source.LastConservationDate;
        toMap.HasConservatedDocs = source.HasConservatedDocs;
        toMap.IdAnnexed = source.IdAnnexed;
        toMap.HandlerDate = source.HandlerDate;
        toMap.Modified = source.Modified;
        toMap.IdHummingBird = source.IdHummingBird;
        toMap.ProtocolCheckDate = source.ProtocolCheckDate;
        toMap.TdIdDocument = source.TdIdDocument;
        toMap.TDError = source.TDError;
        toMap.DocAreaStatus = source.DocAreaStatus;
        toMap.DocAreaStatusDesc = source.DocAreaStatusDesc;
        toMap.IdAttachLocation = source.IdAttachLocation;
        toMap.IdProtocolKind = source.IdProtocolKind;
        toMap.IdProtocolJournalLog = source.IdProtocolJournalLog;
        toMap.UniqueId = source.UniqueId;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category): null;
        toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;
        toMap.DocumentSeriesItems = source.DocumentSeriesItems;
        toMap.Messages = source.Messages;

        return toMap;
    }
}
export = ProtocolModelMapper;