import BaseMapper = require('App/Mappers/BaseMapper');
import RoleModel = require('App/Models/Commons/RoleModel')

class RoleModelMapper extends BaseMapper<RoleModel>{
    constructor() {
        super();
    }

    public Map(source: any): RoleModel {
        let toMap = <RoleModel>{};

        if (!source) {
            return null;
        }

        toMap.EntityShortId = source.EntityShortId;
        toMap.Name = source.Name;
        toMap.IdTenantAOO = source.IdTenantAOO;
        toMap.IdRole = source.IdRole ? source.IdRole : source.EntityShortId;
        toMap.FullIncrementalPath = source.FullIncrementalPath;
        toMap.IsActive = source.IsActive;
        toMap.IdRoleFather = source.Father ? source.Father.EntityShortId : source.IdRoleFather ? source.IdRoleFather : null;
        toMap.UniqueId = source.UniqueId;
        toMap.Children = source.Children;
        toMap.ServiceCode = source.ServiceCode;
        toMap.RoleTypology = source.RoleTypology;
        toMap.Children = source.Children && source.Children.length ? source.Children.map(childRole => this.Map(childRole)) : [];
        toMap.CategoryFascicleRights = source.CategoryFascicleRights;
        toMap.IsRealResult = source.IsRealResult;

        return toMap;
    }
}

export = RoleModelMapper