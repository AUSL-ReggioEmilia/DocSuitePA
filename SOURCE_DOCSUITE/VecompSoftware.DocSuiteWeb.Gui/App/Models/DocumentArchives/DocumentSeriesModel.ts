import ContainerModel = require('App/Models/Commons/ContainerModel');
import DocumentSeriesConstraintModel = require('App/Models/DocumentArchives/DocumentSeriesConstraintModel');

interface DocumentSeriesModel {
    EntityId: number;
    Name: string;
    PublicationEnabled?: boolean;
    SubsectionEnabled?: boolean;
    RoleEnabled?: boolean;
    AllowNoDocument?: boolean
    AllowAddDocument?: boolean;
    AttributeSorting?: boolean;
    AttributeCache?: boolean;
    SortOrder?: number;
    IdDocumentSeriesFamily?: number;
    RegistrationDate: Date;
    RegistrationUser: string;
    LastChangedUser: string;
    LastChangedDate?: Date;
    Container: ContainerModel;
    DocumentSeriesConstraints: DocumentSeriesConstraintModel[];
}
export = DocumentSeriesModel;