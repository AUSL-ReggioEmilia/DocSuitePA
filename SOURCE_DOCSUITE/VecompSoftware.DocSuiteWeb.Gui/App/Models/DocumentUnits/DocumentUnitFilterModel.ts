import FascicleReferenceType = require('App/Models/Fascicles/FascicleReferenceType');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');

interface DocumentUnitFilterModel {
    Fascicle: FascicleModel;
    DocumentUnitName: string;
    ReferenceType?: FascicleReferenceType
    Title: string;
}

export = DocumentUnitFilterModel;