import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');


class DocumentUnitViewModel {
    DocumentUnit: DocumentUnitModel;
    DocumentUnitUrl: string;
    ViewerUrl: string;
    IconUrl: string;
    RegistrationDate: string;
    constructor() {
    }
}

export = DocumentUnitViewModel;