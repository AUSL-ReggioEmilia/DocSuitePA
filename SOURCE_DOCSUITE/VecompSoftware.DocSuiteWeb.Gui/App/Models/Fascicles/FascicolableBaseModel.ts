import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleReferenceType = require('App/Models/Fascicles/FascicleReferenceType');
import IFascicolableBaseModel = require('App/Models/Fascicles/IFascicolableBaseModel');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');

abstract class FascicolableBaseModel implements IFascicolableBaseModel {
    ReferenceType: FascicleReferenceType;
    UniqueId: string;
    FascicleFolder: FascicleFolderModel;
    Fascicle: FascicleModel;

    constructor()
    constructor(idFascicle: string)
    constructor(idFascicle?: any) {
        if (idFascicle != null) {
            this.Fascicle = new FascicleModel();
            this.Fascicle.UniqueId = idFascicle;
        }
    }
}

export = FascicolableBaseModel;