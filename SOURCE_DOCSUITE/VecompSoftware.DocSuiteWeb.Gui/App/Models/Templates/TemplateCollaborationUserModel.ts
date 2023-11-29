import RoleModel = require("App/Models/Commons/RoleModel");

class TemplateCollaborationUserModel {
    public UniqueId: string;
    public Account: string;
    public Incremental: number;
    public IsRequired: boolean;
    public IsValid: boolean;
    public UserType: string;
    public Role: RoleModel;
}

export = TemplateCollaborationUserModel;