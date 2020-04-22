import BaseMapper = require('App/Mappers/BaseMapper');
import ContainerPropertyModel = require('App/Models/Commons/ContainerPropertyModel');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');

class ContainerPropertyModelMapper extends BaseMapper<ContainerPropertyModel>{
    constructor() {
        super();
    }

    public Map(source: any): ContainerPropertyModel {
        let toMap = <ContainerPropertyModel>{};

        if (!source) {
            return null;
        }

        toMap.Name = source.Name;
        toMap.ContainerType = source.ContainerType;
        toMap.ValueInt = source.ValueInt;
        toMap.ValueDate = source.ValueDate;
        toMap.ValueDouble = source.ValueDouble;
        toMap.ValueString = source.ValueString;
        toMap.ValueGuid = source.ValueGuid;
        toMap.ValueBoolean = source.ValueBoolean;  
        toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;

        return toMap;
    }
}

export = ContainerPropertyModelMapper;