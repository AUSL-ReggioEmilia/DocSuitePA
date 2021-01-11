import DossierFolderRoleModel = require("App/Models/Dossiers/DossierFolderRoleModel");
import RoleModelMapper = require("App/Mappers/Commons/RoleModelMapper");
import BaseMapper = require("App/Mappers/BaseMapper");

class DossierFolderRoleModelMapper extends BaseMapper<DossierFolderRoleModel>{
    constructor() {
        super();
    }
    public Map(source: any): DossierFolderRoleModel {
        let toMap: DossierFolderRoleModel = <DossierFolderRoleModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.AuthorizationRoleType = source.AuthorizationRoleType;
        toMap.IsMaster = source.IsMaster;
        toMap.Status = source.Status;
        toMap.Role = source.Role ? new RoleModelMapper().Map(source.Role) : null;
        return toMap;
    }
}

export = DossierFolderRoleModelMapper;