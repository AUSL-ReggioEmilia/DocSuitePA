import { Injectable } from '@angular/core'; 

import { BaseMapper } from '../base.mapper'; 
import { ProtocolSectorModel } from '../../models/protocols/protocol-sector.model'; 

@Injectable()
export class ProtocolSectorMapper implements BaseMapper {

    mapFromJson(json: any): ProtocolSectorModel {

        if (!json) {
            return null;
        }

        let model: ProtocolSectorModel = new ProtocolSectorModel();

        model.id = json.Id;
        model.name = json.Name;
        model.active = json.IsActive;
        model.authorized = json.IsAuthorized;
        model.uniqueIdFather = json.UniqueIdFather;
        model.children = json.Children ? json.Children.map(item => this.mapFromJson(item)) : null;

        return model;
    }

}