import EvaluateCollaborationDocumentModel = require('App/Models/Commons/EvaluateCollaborationDocumentModel');

interface EvaluateCollaborationModel {
    DocumentModels: EvaluateCollaborationDocumentModel[];
    FromWorkflow: boolean;
}

export = EvaluateCollaborationModel;