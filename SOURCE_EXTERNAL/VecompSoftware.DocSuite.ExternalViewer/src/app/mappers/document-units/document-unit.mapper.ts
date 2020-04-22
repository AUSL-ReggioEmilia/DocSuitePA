import { Injectable } from '@angular/core'; 

import { BaseMapper } from '../base.mapper'; 
import { DocumentUnitModel } from '../../models/document-units/document-unit.model'; 

@Injectable()
export class DocumentUnitMapper implements BaseMapper {

    mapFromJson(json: any): DocumentUnitModel{

        if (!json) {
            return null;
        }

        let model: DocumentUnitModel = new DocumentUnitModel();
        model.id = json.Id;
        model.name = json.Name;
        model.title = json.Title;
        model.subject = json.Subject;
        model.environment = json.Environment;
        model.documentUnitName = json.DocumentUnitName;
        model.year = json.Year;
        model.number = json.Number;

        return model;
    }

}