import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import BaseMapper = require('App/Mappers/BaseMapper');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');

class UDSRepositoryModelMapper extends BaseMapper<UDSRepositoryModel>{
    constructor() {      
        super();
    }

    public Map(source: any): UDSRepositoryModel {
        let toMap: UDSRepositoryModel = <UDSRepositoryModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;   
        toMap.Name = source.Name;
        toMap.SequenceCurrentYear = source.SequenceCurrentYear;
        toMap.SequenceCurrentNumber = source.SequenceCurrentNumber;
        toMap.ModuleXML = source.ModuleXML;
        toMap.Version = source.Version;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.ActiveDate = source.ActiveDate;
        toMap.ExpiredDate = source.ExpiredDate;
        toMap.DSWEnvironment = source.DSWEnvironment;
        toMap.Alias = source.Alias;
        toMap.Status = source.Status;
        toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container):null;

        return toMap;
    }
}

export = UDSRepositoryModelMapper;