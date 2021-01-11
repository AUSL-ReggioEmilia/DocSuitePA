import DocumentSeriesModel = require("./DocumentSeriesModel");

interface DocumentSeriesItemModel {
    UniqueId: string;
    Year: number;
    Number: number;
    Status: string;
    Subject: string;
    RegistrationDate: Date;
    RegistrationUser: string;
    RetireDate: Date;
    PublishingDate: Date;
    EntityId: string;
    DocumentSeries: DocumentSeriesModel;
}
export = DocumentSeriesItemModel;