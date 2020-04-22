import BaseMapper = require('App/Mappers/BaseMapper');
import ContainerModel = require('App/Models/Commons/ContainerModel');

class ContainerViewModelMapper extends BaseMapper<ContainerModel>{
    constructor() {
        super();
    }

    public Map(source: any): ContainerModel {
        let toMap = <ContainerModel>{};

        if (!source) {
            return null;
        }

        toMap.EntityShortId = source.IdContainer;
        toMap.Name = source.Name;
        toMap.Note = source.Note;
        toMap.UniqueId = source.UniqueId;

        return toMap;
    }
}

export = ContainerViewModelMapper;