import FascicolableBaseModel = require('App/Models/Fascicles/FascicolableBaseModel');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');

class FascicleDocumentUnitModel extends FascicolableBaseModel {
    DocumentUnit: DocumentUnitModel;
    constructor()
    constructor(idFascicle: string)
    constructor(idFascicle?: any) {
        super(idFascicle);
    }
}

export = FascicleDocumentUnitModel;