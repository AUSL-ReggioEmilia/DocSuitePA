import { Injectable } from '@angular/core'; 

import { BaseMapper } from '../base.mapper'; 
import { ContainerModel } from '../../models/commons/container.model'; 

@Injectable()
export class ContainerMapper implements BaseMapper {

    mapFromJson(json: any): ContainerModel {

        if (!json) {
            return null;
        }

        let model: ContainerModel = new ContainerModel();


        model.name = json.Name;
        model.active = json.isActive;

        return model;
    }

}