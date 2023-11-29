import RoleUserType = require('App/Models/RoleUsers/RoleUserType');

interface RoleUserModel {
    EntityId: number;
    UniqueId: string;
    Account: string;
    Email: string;
    Description: string;
    Type: RoleUserType;
}

export = RoleUserModel;