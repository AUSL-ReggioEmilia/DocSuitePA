import AuthorizationRoleType = require("../Commons/AuthorizationRoleType");
import DocumentUnitModel = require("./DocumentUnitModel");

class DocumentUnitRoleModel {
    RoleLabel: string;
    UniqueIdRole: string;
    AssignUser: string;
    EntityShortId: number;
    AuthorizationRoleType: AuthorizationRoleType;
    DocumentUnit: DocumentUnitModel;
}

export = DocumentUnitRoleModel;