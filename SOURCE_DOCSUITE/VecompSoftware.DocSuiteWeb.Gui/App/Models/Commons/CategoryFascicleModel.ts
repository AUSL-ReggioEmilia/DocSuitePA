import FascicleType = require('App/Models/Fascicles/FascicleType');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import FasciclePeriod = require('App/Models/Commons/FasciclePeriod');

interface CategoryFascicleModel {
    UniqueId: string;
    FascicleType: FascicleType;
    DSWEnvironment: number;
    Category: CategoryModel;
    Manager: ContactModel;
    FasciclePeriod: FasciclePeriod;
}
export = CategoryFascicleModel;