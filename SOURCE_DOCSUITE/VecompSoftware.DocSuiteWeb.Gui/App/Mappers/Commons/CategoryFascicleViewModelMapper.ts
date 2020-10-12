import CategoryFascicleViewModel = require('App/ViewModels/Commons/CategoryFascicleViewModel');
import BaseMapper = require('App/Mappers/BaseMapper');
import CategoryFascicleRightViewModelMapper = require('App/Mappers/Commons/CategoryFascicleRightViewModelMapper');

class CategoryFascicleViewModelMapper extends BaseMapper<CategoryFascicleViewModel>{
    constructor() {
        super();
    }

    public Map(source: any): CategoryFascicleViewModel {
        let toMap: CategoryFascicleViewModel = <CategoryFascicleViewModel>{};
        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.FascicleType = source.FascicleType;
        toMap.Environment = source.DSWEnvironment;
        toMap.PeriodDays = source.FasciclePeriod ? source.FasciclePeriod.PeriodDays : null;
        toMap.PeriodName = source.FasciclePeriod ? source.FasciclePeriod.PeriodName : null;
        toMap.PeriodUniqueId = source.FasciclePeriod ? source.FasciclePeriod.PeriodUniqueId : null;
        toMap.CategoryId = source.Category ? source.Category.EntityShortId : null;
        toMap.ManagerId = source.Manager ? source.Manager.UniqueId : null;
        toMap.ManagerName = source.Manager ? source.Manager.Description : null;
        toMap.CustomActions = source.CustomActions;
        toMap.CategoryFascicleRights = source.CategoryFascicleRights ? new CategoryFascicleRightViewModelMapper().MapCollection(source.CategoryFascicleRights) : null;

        return toMap;
    }
}

export = CategoryFascicleViewModelMapper;