import ResolutionModel = require("../Resolutions/ResolutionModel");

interface DocumentSeriesItemLinksModel {
    EntityId: number;
    LinkType: string;
    EntityShortId: number;
    UniqueId: string;
    RegistrationUser: string;
    RegistrationDate: string;
    Resolution: ResolutionModel;
}

export = DocumentSeriesItemLinksModel;