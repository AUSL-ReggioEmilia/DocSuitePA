import IMapper = require('App/Mappers/IMapper');
import TemplateCollaborationUserModel = require('App/Models/Templates/TemplateCollaborationUserModel');
import RoleModelMapper = require('App/Mappers/Commons/RoleModelMapper');
import RequireJSHelper = require('App/Helpers/RequireJSHelper');
import BaseMapper = require('App/Mappers/BaseMapper');

class TemplateCollaborationUserModelMapper extends BaseMapper<TemplateCollaborationUserModel> {

    constructor() {
        super();
    }

    public Map(source: any): TemplateCollaborationUserModel {
        let toMap: TemplateCollaborationUserModel = <TemplateCollaborationUserModel>{};

        const _roleModelMapper: RoleModelMapper = RequireJSHelper.getModule<RoleModelMapper>(RoleModelMapper, 'App/Mappers/Commons/RoleModelMapper');

        toMap.UniqueId = source.UniqueId;
        toMap.Account = source.Account;
        toMap.Incremental = source.Incremental;
        toMap.IsRequired = source.IsRequired;
        toMap.IsValid = source.IsValid;
        toMap.UserType = source.UserType;
        toMap.Role = source.Role ? _roleModelMapper.Map(source.Role) : null;
        return toMap;
    }
}

export = TemplateCollaborationUserModelMapper;