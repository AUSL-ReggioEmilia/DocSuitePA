import RoleModel = require('App/Models/Commons/RoleModel');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import DossierModel = require('./DossierModel');

class DossierRoleModel  {
    UniqueId: string;
    AuthorizationRoleType: AuthorizationRoleType;
    Role: RoleModel;
    IsMaster: boolean;
    EntityShortId: number;
    Dossier: DossierModel;
    Status: string;
}

export = DossierRoleModel;