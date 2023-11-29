import UDSFieldListModel = require("App/Models/UDS/UDSFieldListModel");
import BaseMapper = require("App/Mappers/BaseMapper");
import UDSRepositoryModelMapper = require("./UDSRepositoryModelMapper");

class UDSFieldListModelMapper extends BaseMapper<UDSFieldListModel>{
    constructor() {
        super();
    }

    public Map(source: any): UDSFieldListModel {
        let toMap: UDSFieldListModel = <UDSFieldListModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.FieldName = source.FieldName;
        toMap.Name = source.Name;
        toMap.Status = source.Status;
        toMap.Environment = source.Environment;
        toMap.UDSFieldListPath = source.UDSFieldListPath;
        toMap.UDSFieldListLevel = source.UDSFieldListLevel;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.LastChangedDate = source.LastChangedDate;

        toMap.Repository = source.Repository
            ? new UDSRepositoryModelMapper().Map(source.Repository)
            : null;

        return toMap;
    }
}

export = UDSFieldListModelMapper;