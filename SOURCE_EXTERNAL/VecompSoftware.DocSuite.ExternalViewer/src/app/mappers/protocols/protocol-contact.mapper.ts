import { Injectable } from '@angular/core'; 

import { BaseMapper } from '../base.mapper'; 
import { ProtocolContactModel } from '../../models/protocols/protocol-contact.model'; 
import { ComunicationType } from '../../models/protocols/comunication.type';
import { ContactType } from '../../models/commons/contact.type';

@Injectable()
export class ProtocolContactMapper implements BaseMapper {

    mapFromJson(json: any): ProtocolContactModel {

        if (!json) {
            return null;

        }

        let model: ProtocolContactModel = new ProtocolContactModel();

        model.id = json.Id;
        model.name = json.Name;
        model.active = json.IsActive;
        model.comunicationType = ComunicationType[json.ComunicationType as string];
        model.type = ContactType[json.ContactType as string];
        model.uniqueIdFather = json.UniqueIdFather;
        model.isSelected = json.IsSelected;
        model.isCC = json.IsCC;
        model.children = json.Children ? json.Children.map(item => this.mapFromJson(item)) : null;

        return model;
    }

}