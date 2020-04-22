import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');

interface DossierSummaryViewModel{
    UniqueId: string;
    Year: number;
    Number: string;
    Subject: string;
    Note: string;
    ContainerName: string;
    ContainerId: string;
    FormattedStartDate: string;
    FormattedEndDate: string;
    StartDate: Date;
    RegistrationDate: string;
    RegistrationUser: string;
    LastChangedUser: string;
    LastChangedDate: string;
    ContactId: string;
    JsonMetadata: string;

    Contacts: BaseEntityViewModel[];
    Roles: BaseEntityViewModel[];
    Documents: BaseEntityViewModel[];
    
}

export = DossierSummaryViewModel