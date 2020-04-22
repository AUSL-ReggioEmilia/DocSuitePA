import CategoryViewModelType = require("App/ViewModels/Commons/CategoryViewModelType");

interface CategoryTreeViewModel {
    IdCategory: number;
    Name: string;
    FullCode: string;
    Code?: number;
    UniqueId: string;
    FullIncrementalPath: string;
    HasChildren?: boolean;
    HasFascicleDefinition?: boolean;
    IdParent?: number;
    CategoryType?: CategoryViewModelType;
}

export = CategoryTreeViewModel;