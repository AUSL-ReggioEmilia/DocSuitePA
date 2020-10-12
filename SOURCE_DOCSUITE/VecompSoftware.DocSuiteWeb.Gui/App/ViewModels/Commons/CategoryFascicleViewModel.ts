import CategoryFascicleRightViewModel = require('App/ViewModels/Commons/CategoryFascicleRightViewModel');

interface CategoryFascicleViewModel {
    UniqueId: string;
    Environment: number;
    FascicleType: string;
    CategoryId: string;
    PeriodName: string;
    PeriodDays: number;
    PeriodUniqueId: number;
    ManagerId: number;
    ManagerName: string;
    CustomActions: string;
    CategoryFascicleRights: CategoryFascicleRightViewModel[];
}
export = CategoryFascicleViewModel