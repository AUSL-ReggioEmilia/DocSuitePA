import CategoryModel = require("../Commons/CategoryModel");
import FascicleModel = require("./FascicleModel");
import DocumentUnitModel = require("../DocumentUnits/DocumentUnitModel");

interface FascicleDocumentUnitCategoryModel {
    UniqueId: string;
    RegistrationDate: Date;
    RegistrationUser: string;
    LastChangedUser: string;
    CategoryTitle: string;
    LastChangedDate: Date;
    Category: CategoryModel;
    Fascicle: FascicleModel;
    DocumentUnit: DocumentUnitModel
}

export = FascicleDocumentUnitCategoryModel;