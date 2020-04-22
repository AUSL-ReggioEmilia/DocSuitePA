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
        toMap.IdRoleTenant = source.IdRoleTenant;
        toMap.TenantId = source.TenantId;
        toMap.IdRole = source.IdRole ? source.IdRole : source.EntityShortId;
        toMap.FullIncrementalPath = source.FullIncrementalPath;
        toMap.IsActive = source.IsActive;
        toMap.IdRoleFather = source.Father ? source.Father.EntityShortId : source.IdRoleFather ? source.IdRoleFather : null;
        toMap.UniqueId = source.UniqueId;
        toMap.Children = source.Children;
        toMap.ServiceCode = source.ServiceCode;
        toMap.ActiveFrom = source.ActiveFrom ? moment(source.ActiveFrom).format("DD/MM/YYYY") : "";
        toMap.Children = source.Children && source.Children.length ? source.Children.map(childRole => this.Map(childRole)) : [];

        return toMap;
    }
}

export = RoleModelMapper