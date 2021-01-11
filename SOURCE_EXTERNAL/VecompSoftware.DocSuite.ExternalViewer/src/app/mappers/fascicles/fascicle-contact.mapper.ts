import { Injectable } from '@angular/core'; 

import { BaseMapper } from '../base.mapper'; 
import { FascicleContactModel } from '../../models/fascicles/fascicle-contact.model'; 
import { ContactType } from '../../models/commons/contact.type';

@Injectable()
export class FascicleContactMapper implements BaseMapper {

    mapFromJson(json: any): FascicleContactModel {

        if (!json) {
            return null;

        }

        let model: FascicleContactModel = new FascicleContactModel();

        model.id = json.Id;
        model.name = json.Name;
        model.active = json.IsActive;
        model.type = ContactType[json.ContactType as string];
        model.uniqueIdFather = json.UniqueIdFather;
        model.isSelected = json.IsSelected;
        model.children = json.Children ? json.Children.map(item => this.mapFromJson(item)) : null;

        return model;
    }

}