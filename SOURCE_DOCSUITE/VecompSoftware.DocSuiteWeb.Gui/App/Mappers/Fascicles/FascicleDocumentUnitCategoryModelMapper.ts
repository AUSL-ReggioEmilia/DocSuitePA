import BaseMapper = require("../BaseMapper");
import FascicleDocumentUnitCategoryModel = require("../../Models/Fascicles/FascicleDocumentUnitCategoryModel");
import FascicleModelMapper = require("./FascicleModelMapper");
import RequireJSHelper = require("../../Helpers/RequireJSHelper");
import CategoryModelMapper = require("../Commons/CategoryModelMapper");
import DocumentUnitModelMapper = require("../DocumentUnits/DocumentUnitModelMapper");

class FascicleDocumentUnitCategoryModelMapper extends BaseMapper<FascicleDocumentUnitCategoryModel> {
    constructor() {
        super();
    }

    public Map(source: any): FascicleDocumentUnitCategoryModel {
        let toMap: FascicleDocumentUnitCategoryModel = <FascicleDocumentUnitCategoryModel>{};

        if (!source) {
            return null;
        }

        let _categoryModelMapper: CategoryModelMapper;
        _categoryModelMapper = RequireJSHelper.getModule<CategoryModelMapper>(CategoryModelMapper, 'App/Mappers/Commons/CategoryModelMapper');

        toMap.Category = source.Category ? _categoryModelMapper.Map(source.Category) : null;
        toMap.CategoryTitle = `${this.formatCategoryTitle(source.Category.FullCode)} ${source.Category.Name}`;
        toMap.UniqueId = source.UniqueId;

        return toMap;
    }

    private formatCategoryTitle(title: string): string {
        let formattedTitle = "";
        let titlesArray: string[] = [];
        let numbers: string[] = [];
        numbers = title.split("|");

        for (let char of numbers) {
            let number = +char;//parsing the string gets rid of the first 0 characters
            titlesArray.push(number.toString());
        }

        formattedTitle = titlesArray.join(".");

        return formattedTitle;
    }
}

export = FascicleDocumentUnitCategoryModelMapper;