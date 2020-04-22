import { Injectable } from '@angular/core'; 

import { BaseMapper } from '../base.mapper'; 
import { AuthenticationModel } from '../../models/commons/authentication.model'; 

@Injectable()
export class AuthenticationMapper implements BaseMapper {

    mapFromJson(json: any): AuthenticationModel {

        if (!json) {
            return null;
        }

        let model: AuthenticationModel = new AuthenticationModel();

        model.accessToken = json.access_token;
        model.tokenType = json.token_type;
        model.expiresIn = json.expires_in;
        model.issued = json['.issued'];
        model.expires = json['.expires'];

        return model;
    }

}