import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import RoleModelMapper = require('App/Mappers/Commons/RoleModelMapper');
import BaseMapper = require('App/Mappers/BaseMapper');
import FascicleModelMapper = require('App/Mappers/Fascicles/FascicleModelMapper');
import RequireJSHelper = require('App/Helpers/RequireJSHelper');

class FascicleRoleModelMapper extends BaseMapper<FascicleRoleModel>{
     constructor() {
        super();
    }

    public Map(source: any): FascicleRoleModel {
        let toMap: FascicleRoleModel = <FascicleRoleModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.AuthorizationRoleType = source.AuthorizationRoleType;

        let roleMapper: RoleModelMapper = new RoleModelMapper();
        toMap.Role = source.Role ? roleMapper.Map(source.Role) : null;

        let _fascicleModelMapper: FascicleModelMapper;
        _fascicleModelMapper = RequireJSHelper.getModule<FascicleModelMapper>(FascicleModelMapper, 'App/Mappers/Fascicles/FascicleModelMapper');

        toMap.Fascicle = source.Fascicle ? _fascicleModelMapper.Map(source.Fascicle): null;
        toMap.IsMaster = source.IsMaster;

        return toMap;
    }
}

export = FascicleRoleModelMapper;