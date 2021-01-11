import ResolutionKindModel = require('App/Models/Resolutions/ResolutionKindModel');
import DocumentSeriesModel = require('App/Models/DocumentArchives/DocumentSeriesModel');
import DocumentSeriesConstraintModel = require('App/Models/DocumentArchives/DocumentSeriesConstraintModel');

interface ResolutionKindDocumentSeriesModel {
    UniqueId: string;
    DocumentRequired: boolean;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser: string;
    LastChangedDate?: Date;
    DocumentSeriesConstraint: DocumentSeriesConstraintModel;
    ResolutionKind: ResolutionKindModel;
    DocumentSeries: DocumentSeriesModel;
}
export = ResolutionKindDocumentSeriesModel;