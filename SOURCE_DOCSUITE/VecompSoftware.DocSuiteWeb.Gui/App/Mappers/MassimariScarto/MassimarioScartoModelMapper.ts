import BaseMapper = require('App/Mappers/BaseMapper');
import MassimarioScartoModel = require('App/Models/MassimariScarto/MassimarioScartoModel');
import MassimarioScartoStatusType = require('App/Models/MassimariScarto/MassimarioScartoStatusType');

class MassimarioScartoModelMapper extends BaseMapper<MassimarioScartoModel> {

    constructor() {
        super();
    }

    public Map(source: any): MassimarioScartoModel {
        let toMap: MassimarioScartoModel = new MassimarioScartoModel();

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.Status = source.Status;
        toMap.Name = source.Name;
        toMap.Code = source.Code;
        toMap.FullCode = source.FullCode;
        toMap.Note = source.Note;
        toMap.ConservationPeriod = source.ConservationPeriod;
        toMap.StartDate = source.StartDate;
        toMap.EndDate = source.EndDate;
        toMap.MassimarioScartoPath = source.MassimarioScartoPath;
        toMap.MassimarioScartoParentPath = source.MassimarioScartoParentPath;
        toMap.MassimarioScartoLevel = source.MassimarioScartoLevel;
        toMap.FakeInsertId = source.FakeInsertId;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.LastChangedDate = source.LastChangedDate;        

        return toMap;
    }

}

export = MassimarioScartoModelMapper;