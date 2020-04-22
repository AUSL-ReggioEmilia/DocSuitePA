import DocumentSeriesModel = require('App/Models/DocumentArchives/DocumentSeriesModel');

interface DocumentSeriesConstraintModel {
    UniqueId: string;
    Name: string;
    DocumentSeries: DocumentSeriesModel;
    RegistrationDate: Date;
    RegistrationUser: string;
    LastChangedUser: string;
    LastChangedDate?: Date;
}
export = DocumentSeriesConstraintModel;