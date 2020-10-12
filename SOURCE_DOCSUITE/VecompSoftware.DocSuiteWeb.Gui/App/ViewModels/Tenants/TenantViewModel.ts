import LocationViewModel = require('App/ViewModels/Commons/LocationViewModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import PECMailBoxModel = require('App/Models/PECMails/PECMailBoxModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import TenantWorkflowRepositoryModel = require('App/Models/Tenants/TenantWorkflowRepositoryModel');
import TenantConfigurationModel = require('App/Models/Tenants/TenantConfigurationModel');
import ContactModel = require('../../Models/Commons/ContactModel');
import TenantAOOModel = require('../../Models/Tenants/TenantAOOModel');
import TenantTypologyTypeEnum = require('App/Models/Tenants/TenantTypologyTypeEnum');

class TenantViewModel {
    UniqueId: string;
    TenantName: string;
    CompanyName: string;
    StartDate: Date;
    EndDate: Date;
    Note: string;
    RegistrationUser: string;
    RegistrationDate?: Date;
    LastChangedUser: string;
    LastChangedDate?: Date;
    Location: LocationViewModel;
    Containers: ContainerModel[];
    PECMailBoxes: PECMailBoxModel[];
    Roles: RoleModel[];
    Contacts: ContactModel[];
    TenantWorkflowRepositories: TenantWorkflowRepositoryModel[];
    Configurations: TenantConfigurationModel[];
    TenantAOO: TenantAOOModel;
    TenantTypology: TenantTypologyTypeEnum;
}

export = TenantViewModel;