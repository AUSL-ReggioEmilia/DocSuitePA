import RoleUserModel = require("../../Models/RoleUsers/RoleUserModel");
import BaseMapper = require("../BaseMapper");

class RoleUserModelMapper extends BaseMapper<RoleUserModel>{
    constructor() {
        super();
    }
    public Map(source: any): RoleUserModel {
        let toMap: RoleUserModel = <RoleUserModel>{};

        if (!source) {
            return null;
        }
        toMap.EntityId = source.EntityId;
        toMap.UniqueId = source.UniqueId;
        toMap.Account = source.Account;
        toMap.Email = source.Email;
        toMap.Description = source.Description;
        toMap.Type = source.Type;
        return toMap;
    }
}

export = RoleUserModelMapper;