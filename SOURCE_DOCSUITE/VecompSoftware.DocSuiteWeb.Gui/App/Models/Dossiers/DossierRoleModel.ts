import RoleModel = require('App/Models/Commons/RoleModel');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');

class DossierRoleModel  {
    UniqueId: string;
    AuthorizationRoleType: AuthorizationRoleType;
    Role: RoleModel;
}

export = DossierRoleModel;