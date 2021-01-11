import { Injectable } from '@angular/core'; 

import { BaseMapper } from '../base.mapper'; 
import { CategoryModel } from '../../models/commons/category.model'; 

@Injectable()
export class CategoryMapper implements BaseMapper {

    mapFromJson(json: any): CategoryModel {

        if (!json) {
            return null;
        }

        let model: CategoryModel = new CategoryModel();

        model.name = json.Name;
        model.active = json.isActive;
        model.code = json.Code;
        model.hierarchyCode = json.HierarchyCode;
        model.hierarchyDescription = json.HierarchyDescription;

        return model;
    }

}