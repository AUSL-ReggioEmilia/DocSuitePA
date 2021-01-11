import BaseMapper = require('App/Mappers/BaseMapper');
import DocumentUnitRoleModel = require('App/Models/DocumentUnits/DocumentUnitRoleModel');

class DocumentUnitRoleMapper extends BaseMapper<DocumentUnitRoleModel> {
    constructor() {
        super();
    }

    public Map(source: any): DocumentUnitRoleModel {
        let toMap: DocumentUnitRoleModel = <DocumentUnitRoleModel>{};

        if (!source) {
            return null;
        }

        toMap.AssignUser = source.AssignUser;
        toMap.AuthorizationRoleType = source.AuthorizationRoleType;
        toMap.RoleLabel = source.RoleLabel;
        toMap.UniqueIdRole = source.UniqueIdRole;
        toMap.EntityShortId = source.EntityShortId;

        return toMap;
    }
}

export = DocumentUnitRoleMapper