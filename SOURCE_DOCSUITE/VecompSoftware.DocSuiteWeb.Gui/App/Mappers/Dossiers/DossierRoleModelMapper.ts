import DossierRoleModel = require('App/Models/Dossiers/DossierRoleModel');
import IMapper = require('App/Mappers/IMapper');
import RoleModelMapper = require('App/Mappers/Commons/RoleModelMapper');

class DossierRoleModelMapper implements IMapper<DossierRoleModel>{
    construnctor() {
    }
    public Map(source: any): DossierRoleModel {
        let toMap: DossierRoleModel = <DossierRoleModel>{};
        toMap.UniqueId = source.UniqueId;
        toMap.AuthorizationRoleType = source.AuthorizationRoleType;
        toMap.IsMaster = source.IsMaster;
        toMap.EntityShortId = source.EntityShortId;
        toMap.Dossier = source.Dossier;
        toMap.Status = source.Status;

        return toMap;
    }
}

export = DossierRoleModelMapper;