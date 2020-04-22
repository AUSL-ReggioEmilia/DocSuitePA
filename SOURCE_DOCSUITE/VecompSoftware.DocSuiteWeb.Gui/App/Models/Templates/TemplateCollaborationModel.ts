import TemplateCollaborationStatus = require('App/Models/Templates/TemplateCollaborationStatus');

interface TemplateCollaboration {
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
}

export = TemplateCollaboration;