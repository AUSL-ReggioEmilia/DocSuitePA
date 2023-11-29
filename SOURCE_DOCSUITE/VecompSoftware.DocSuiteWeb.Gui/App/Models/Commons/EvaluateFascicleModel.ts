import BuildArchiveDocumentModel = require('App/Models/Commons/BuildArchiveDocumentModel');

interface EvaluateFascicleModel {
    IdFascicle: string;
    ArchiveDocument: BuildArchiveDocumentModel;
}

export = EvaluateFascicleModel;