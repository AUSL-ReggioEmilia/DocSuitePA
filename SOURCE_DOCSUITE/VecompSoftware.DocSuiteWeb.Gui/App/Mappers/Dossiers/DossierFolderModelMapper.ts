import IMapper = require("App/Mappers/IMapper");
import DossierFolderModel = require("App/Models/Dossiers/DossierFolderModel");
import CategoryModelMapper = require("App/Mappers/Commons/CategoryModelMapper");
import DossierModelMapper = require("App/Mappers/Dossiers/DossierModelMapper");
import FascicleModelMapper = require("App/Mappers/Fascicles/FascicleModelMapper");
import DossierFolderRoleModelMapper = require("App/Mappers/Dossiers/DossierFolderRoleModelMapper");
import BaseMapper = require("App/Mappers/BaseMapper");

class DossierFolderModelMapper extends BaseMapper<DossierFolderModel>{
    constructor() {
        super();
    }
    public Map(source: any): DossierFolderModel {
        let toMap: DossierFolderModel = <DossierFolderModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.Name = source.Name;
        toMap.Status = source.Status;
        toMap.JsonMetadata = source.JsonMetadata;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.LastChangedDate = source.LastChangedDate;
        toMap.Category = source.CAtegory ? new CategoryModelMapper().Map(source.Category) : null;
        toMap.Dossier = source.Dossier ? new DossierModelMapper().Map(source.Dossier) : null;
        toMap.Fascicle = source.Fascicle ? new FascicleModelMapper().Map(source.Fascicle) : null;
        toMap.DossierFolderRoles = source.DossierFolderRoles ? new DossierFolderRoleModelMapper().MapCollection(source.DossierFolderRoles) : null;
        toMap.DossierFolders = source.DossierFolders ? new DossierFolderModelMapper().MapCollection(source.DossierFolders) : null;
        toMap.ParentInsertId = source.ParentInsertId;
        return toMap;
    }
}

export = DossierFolderModelMapper;