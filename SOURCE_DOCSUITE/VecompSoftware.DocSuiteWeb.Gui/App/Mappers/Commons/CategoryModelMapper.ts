import BaseMapper = require('App/Mappers/BaseMapper');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import MassimarioScartoModelMapper = require('App/Mappers/MassimariScarto/MassimarioScartoModelMapper');

class CategoryModelMapper extends BaseMapper<CategoryModel>{
    constructor() {
        super();
    }

    public Map(source: any): CategoryModel {
        let toMap = new CategoryModel();      

        if (!source) {
            return null;
        }

        toMap.Code = source.Code;
        toMap.EntityShortId = source.EntityShortId;
        toMap.FullCode = source.FullCode;
        toMap.FullIncrementalPath = source.FullIncrementalPath;
        toMap.FullSearchComputed = source.FullSearchComputed;
        toMap.IdCategory = source.IdCategory;
        toMap.IsActive = source.IsActive;
        toMap.MassimarioScarto = source.MassimarioScarto ? new MassimarioScartoModelMapper().Map(source.MassimarioScarto) : null;
        toMap.Parent = source.Parent ? new CategoryModelMapper().Map(source.Parent) : null;
        toMap.Name = source.Name;          
        toMap.UniqueId = source.UniqueId;
        toMap.StartDate = source.StartDate;
        toMap.EndDate = source.EndDate;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.LastChangedDate = source.LastChangedDate;
        toMap.LastChangedUser = source.LastChangedUser;

        return toMap;
    }
}

export = CategoryModelMapper;