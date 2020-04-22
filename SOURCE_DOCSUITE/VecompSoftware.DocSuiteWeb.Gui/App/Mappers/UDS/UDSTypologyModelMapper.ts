import UDSTypologyModel = require('App/Models/UDS/UDSTypologyModel');
import BaseMapper = require ('App/Mappers/BaseMapper');

class UDSTypologyModelMapper extends BaseMapper<UDSTypologyModel>{
    constructor() {      
        super();
    }

    public Map(source: any): UDSTypologyModel {
        let toMap: UDSTypologyModel = new UDSTypologyModel();

        if (!source) {
            return null;
        }

        toMap.Alias = source.Alias;
        toMap.LastChangedDate = source.LastChangeDate;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.Name = source.Name;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.Status = source.Status;
        toMap.UniqueId = source.UniqueId;
                
        return toMap;
    }
}

export = UDSTypologyModelMapper;