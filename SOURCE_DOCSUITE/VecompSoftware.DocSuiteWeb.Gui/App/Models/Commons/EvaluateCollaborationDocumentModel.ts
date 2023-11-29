import BuildArchiveDocumentModel = require('App/Models/Commons/BuildArchiveDocumentModel');

interface EvaluateCollaborationDocumentModel {
    IdCollaboration: string;
    IdCollaborationVersioning: string;
    ArchiveDocument: BuildArchiveDocumentModel;
}

export = EvaluateCollaborationDocumentModel;