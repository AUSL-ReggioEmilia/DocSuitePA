import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import RoleModel = require('App/Models/Commons/RoleModel');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');

class FascicleRoleModel {
    UniqueId: string;
    AuthorizationRoleType: AuthorizationRoleType;
    Role: RoleModel;
    Fascicle: FascicleModel;
    IsMaster: boolean;
}

export = FascicleRoleModel;
