import UDSModel = require('App/Models/UDS/UDSModel');
import CategoryModelMapper = require('App/Mappers/Commons/CategoryModelMapper');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');
import BaseMapper = require ('App/Mappers/BaseMapper');

class UDSModelMapper extends BaseMapper<UDSModel>{
    constructor() {      
        super();
    }

    public Map(source: any): UDSModel {
        let toMap: UDSModel = <UDSModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UDSId;
        toMap.Year = source._year;
        toMap.Number = source._number;
        toMap.Subject = source._subject;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category): null;
        toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;

        return toMap;
    }
}

export = UDSModelMapper;