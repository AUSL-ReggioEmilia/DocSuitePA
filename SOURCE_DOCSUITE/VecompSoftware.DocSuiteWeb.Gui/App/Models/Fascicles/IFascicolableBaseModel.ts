import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleReferenceType = require('FascicleReferenceType');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');

interface IFascicolableBaseModel {
    ReferenceType: FascicleReferenceType;
    UniqueId: string;
    FascicleFolder: FascicleFolderModel;
    Fascicle: FascicleModel;
}

export = IFascicolableBaseModel