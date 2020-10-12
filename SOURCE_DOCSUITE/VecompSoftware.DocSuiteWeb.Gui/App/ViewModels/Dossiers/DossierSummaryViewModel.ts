import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');
import BaseEntityRoleViewModel = require('App/ViewModels/BaseEntityRoleViewModel');
import CategoryTreeViewModel = require('App/ViewModels/Commons/CategoryTreeViewModel');

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
    MetadataDesigner: string;
    MetadataValues: string;
    DossierType: string;
    Status: string;

    Contacts: BaseEntityViewModel[];
    Roles: BaseEntityRoleViewModel[];
    Documents: BaseEntityViewModel[];
    Category: CategoryTreeViewModel;
}

export = DossierSummaryViewModel