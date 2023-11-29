import TemplateCollaborationStatus = require('App/Models/Templates/TemplateCollaborationStatus');
import TemplateCollaborationRepresentationType = require('App/Models/Templates/TemplateCollaborationRepresentationType');
import EnumValue = require('App/Helpers/EnumValue');
import FolderPathValue = require('App/Helpers/FolderPathValue');
import TemplateCollaborationUserModel = require('App/Models/Templates/TemplateCollaborationUserModel');
import RoleModel = require('App/Models/Commons/RoleModel');

class TemplateCollaboration {
    UniqueId: string;
    Name: string;
    Status: TemplateCollaborationStatus;
    DocumentType: string;
    IdPriority: string;
    Object: string;
    Note: string;
    IsLocked: boolean;
    RegistrationUser: string;
    RegistrationDate: Date;
    JsonParameters: string;
    RepresentationType: TemplateCollaborationRepresentationType;
    TemplateCollaborationLevel: number;
    TemplateCollaborationPath: string;
    ParentInsertId: string;
    Roles: RoleModel[];
    TemplateCollaborationUsers: TemplateCollaborationUserModel[];

    public get StatusValue(): EnumValue {
        return new EnumValue(this.Status, TemplateCollaborationStatus);
    }

    public get RepresentationTypeValue(): EnumValue {
        return new EnumValue(this.RepresentationType, TemplateCollaborationRepresentationType);
    }

    public get TemplateCollaborationPathValue(): FolderPathValue {
        return new FolderPathValue(this.TemplateCollaborationPath);
    }
}

export = TemplateCollaboration;