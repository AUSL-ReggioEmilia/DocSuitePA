import ProcessModel = require('App/Models/Processes/ProcessModel');
import CategoryModelMapper = require('App/Mappers/Commons/CategoryModelMapper');
import DossierModelMapper = require('App/Mappers/Dossiers/DossierModelMapper');
import BaseMapper = require('App/Mappers/BaseMapper');
import RoleModelMapper = require('App/Mappers/Commons/RoleModelMapper');

class ProcessModelMapper extends BaseMapper<ProcessModel>{
    constructor() {
        super();
    }
    public Map(source: any): ProcessModel {
        let toMap: ProcessModel = <ProcessModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category) : null;
        toMap.Dossier = source.Dossier ? new DossierModelMapper().Map(source.Dossier) : null;
        toMap.Name = source.Name;
        toMap.StartDate = source.StartDate;
        toMap.EndDate = source.EndDate;
        toMap.Note = source.Note;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.LastChangedDate = source.LastChangedDate;
        toMap.Roles = source.Roles ? new RoleModelMapper().MapCollection(source.Roles) : null;
        toMap.ProcessType = source.ProcessType;
        
        return toMap;
    }
}

export = ProcessModelMapper;