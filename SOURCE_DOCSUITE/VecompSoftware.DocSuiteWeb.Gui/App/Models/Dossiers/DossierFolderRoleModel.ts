import RoleModel = require('App/Models/Commons/RoleModel');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import DossierRoleStatus = require('App/Models/Dossiers/DossierRoleStatus');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');

interface DossierFolderRoleModel {
    UniqueId: string;
    AuthorizationRoleType: AuthorizationRoleType;
    IsMaster: boolean;
    Status: DossierRoleStatus;
    Role: RoleModel;
}

export = DossierFolderRoleModel;