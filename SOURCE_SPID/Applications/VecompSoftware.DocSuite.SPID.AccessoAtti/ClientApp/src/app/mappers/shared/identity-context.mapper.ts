import { Injectable } from '@angular/core';

import { BaseMapper } from '@app-mappers/base.mapper';
import {
    AuthorizationType,
    IdentityContextModel,
    IdentityModel
} from '@app-models/_index';
import { NewRequestViewModel } from '@app-viewmodels/requests/new-request.viewmodel';

@Injectable()
export class IdentityContextMapper extends BaseMapper<NewRequestViewModel, IdentityContextModel> {

    constructor() {
        super();
    }

    map(source: NewRequestViewModel): IdentityContextModel {
        let identity: IdentityContextModel = <IdentityContextModel>{};
        identity.Identity = <IdentityModel>{ Account: '', Name: '', AuthorizationType: AuthorizationType.NTLM };
        return identity;
    }
}
