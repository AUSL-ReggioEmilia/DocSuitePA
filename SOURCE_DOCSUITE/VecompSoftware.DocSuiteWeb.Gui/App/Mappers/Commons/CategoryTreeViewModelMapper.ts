import BaseMapper = require('App/Mappers/BaseMapper');
import CategoryTreeViewModel = require('App/ViewModels/Commons/CategoryTreeViewModel');

class CategoryTreeViewModelMapper extends BaseMapper<CategoryTreeViewModel>{
    constructor() {
        super();
    }

    public Map(source: any): CategoryTreeViewModel {
        let toMap = {} as CategoryTreeViewModel;

        if (!source) {
            return null;
        }

        toMap.FullCode = source.FullCode;
        toMap.Code = source.Code;
        toMap.FullIncrementalPath = source.FullIncrementalPath;
        toMap.IdCategory = source.IdCategory;
        toMap.IdParent = source.IdParent;
        toMap.Name = source.Name;
        toMap.UniqueId = source.UniqueId;
        toMap.HasChildren = source.HasChildren;
        toMap.HasFascicleDefinition = source.HasFascicleDefinition;
        toMap.CategoryType = source.CategoryType;

        return toMap;
    }
}

export = CategoryTreeViewModelMapper;