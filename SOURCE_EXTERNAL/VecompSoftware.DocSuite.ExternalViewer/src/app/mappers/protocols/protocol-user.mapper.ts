import { Injectable } from '@angular/core'; 

import { BaseMapper } from '../base.mapper'; 
import { ProtocolUserModel } from '../../models/protocols/protocol-user.model'; 

@Injectable()
export class ProtocolUserMapper implements BaseMapper {

    mapFromJson(json: any): ProtocolUserModel {

        if (!json) {
            return null;
        }

        let model: ProtocolUserModel = new ProtocolUserModel();

        model.id = json.Id;
        model.account = json.Account;
        model.type = json.Type;

        return model;
    }

}