import DomainBaseModel = require("App/Models/Securities/DomainBaseModel");
import DomainGroupModel = require("App/Models/Securities/DomainGroupModel");

interface DomainUserModel extends DomainBaseModel {
    EmailAddress?: string;
    EmployeeId?: string;
    GivenName?: string;
    MiddleName?: string;
    Surname?: string;
    VoiceTelephoneNumber?: string;
    DomainGroups?: DomainGroupModel[];
    Account?: string;
    Domain?: string;
}

export = DomainUserModel;
