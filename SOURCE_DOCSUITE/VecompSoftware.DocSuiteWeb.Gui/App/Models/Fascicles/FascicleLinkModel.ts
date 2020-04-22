import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleLinkType = require('FascicleLinkType');

class FascicleLinkModel {
    UniqueId: string;
    Fascicle: FascicleModel;
    FascicleLinked: FascicleModel;
    FascicleLinkType: FascicleLinkType;


    constructor(idFascicle: string) {
        this.FascicleLinked = new FascicleModel();
        this.FascicleLinked.UniqueId = idFascicle;
    }
}

export = FascicleLinkModel;