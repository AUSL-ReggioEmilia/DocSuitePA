import LocationModel = require('App/Models/Commons/LocationModel');

interface ContainerModel {
    EntityShortId: number;
    Name?: string;
    Note?: string;
    isActive?: boolean;
    Massive?: number;
    Conservation?: number;
    DocumentSeriesAnnexedLocation?: LocationModel;
    DocumentSeriesLocation?: LocationModel;
    DocumentSeriesUnpublishedAnnexedLocation?: LocationModel;
    ProtocolRejection?: number;
    idArchive?: number;
    Privacy?: number;
    HeadingFrontalino?: string;
    HeadingLetter?: string;
    ProtAttachLocation?: LocationModel;
    ProtLocation?: LocationModel;
    ReslLocation?: LocationModel;
    UniqueId?: string;
}

export = ContainerModel;