import DossierModel = require('App/Models/Dossiers/DossierModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');

interface DossierDocumentModel {
    UniqueId: string;
    IdArchiveChain: string;
    ChainType: ChainType;
    Dossier: DossierModel;
}

export = DossierDocumentModel;