import CategoryFascicleRightViewModel = require('App/ViewModels/Commons/CategoryFascicleRightViewModel');
import BaseMapper = require('App/Mappers/BaseMapper');
import RoleModelMapper = require('./RoleModelMapper');

class CategoryFascicleRightViewModelMapper extends BaseMapper<CategoryFascicleRightViewModel>{
    constructor() {
        super();
    }

    public Map(source: any): CategoryFascicleRightViewModel {
        let toMap: CategoryFascicleRightViewModel = <CategoryFascicleRightViewModel>{};
        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.Role = source.Role? new RoleModelMapper().Map(source.Role):null;

        return toMap;
    }
}

export = CategoryFascicleRightViewModelMapper;