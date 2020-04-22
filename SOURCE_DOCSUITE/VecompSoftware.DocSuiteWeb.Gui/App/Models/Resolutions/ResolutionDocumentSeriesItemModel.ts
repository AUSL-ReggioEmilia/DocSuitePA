import ResolutionModel = require("./ResolutionModel");
import DocumentSeriesItemModel = require("../DocumentArchives/DocumentSeriesItemModel");

interface ResolutionDocumentSeriesItemModel {
    EntityId: number;
    EntityShortId: number;
    UniqueId: string;
    RegistrationUser: string;
    RegistrationDate: string;
    Resolution: ResolutionModel;
    DocumentSeriesItem: DocumentSeriesItemModel;
}
export = ResolutionDocumentSeriesItemModel;