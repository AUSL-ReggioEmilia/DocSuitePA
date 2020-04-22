import DossierModel = require('App/Models/Dossiers/DossierModel');
import DossierFolderModelMapper = require('./DossierFolderModelMapper');
import BaseMapper = require('App/Mappers/BaseMapper');

class DossierModelMapper extends BaseMapper<DossierModel>{
    construnctor() {
    }
    public Map(source: any): DossierModel {
        let toMap: DossierModel = <DossierModel>{};
        toMap.UniqueId = source.UniqueId;
        toMap.Year = source.Year;
        toMap.Number = source.Number;
        toMap.Subject = source.Subject;
        toMap.Note = source.Note;
        toMap.Container = source.Container;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.StartDate = source.StartDate;
        toMap.EndDate = source.EndDate;
        toMap.JsonMetadata = source.JsonMetadata;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.LastChangedDate = source.LastChangedDate;
        toMap.Contacts = source.Contacts;
        toMap.DossierRoles = source.DossierRoles;
        toMap.DossierDocuments = source.DossierDocuments;

        toMap.DossierFolders = source.DossierFolders ? new DossierFolderModelMapper().MapCollection(source.DossierFolders) : null;

        return toMap;
    }
}

export = DossierModelMapper;