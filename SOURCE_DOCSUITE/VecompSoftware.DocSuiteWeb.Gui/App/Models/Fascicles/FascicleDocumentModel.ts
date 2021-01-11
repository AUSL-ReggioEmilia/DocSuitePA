import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');

interface FascicleDocumentModel {
    UniqueId: string;
    IdArchiveChain: string;
    Fascicle: FascicleModel;
    ChainType: ChainType;
    FascicleFolder: FascicleFolderModel;
    FileName: string;
    ImageUrl: string;
}

export = FascicleDocumentModel;