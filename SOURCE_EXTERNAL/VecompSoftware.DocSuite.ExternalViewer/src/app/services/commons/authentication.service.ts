import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { BaseService } from '../base.service';
import { AuthenticationModel } from '../../models/commons/authentication.model';
import { AuthenticationMapper } from '../../mappers/commons/authentication.mapper';
import { ExceptionHandlerHelper } from '../../helpers/exception-handler.helper';
import { ResponseModel } from '../../models/commons/response.model';
import { GlobalSetting } from '../../settings/global.setting';

@Injectable()
export class AuthenticationService extends BaseService<AuthenticationModel>{

    constructor(http: HttpClient, exceptionHandler: ExceptionHandlerHelper, globalSetting: GlobalSetting, private authenticationMapper: AuthenticationMapper) {
        super(http, exceptionHandler, globalSetting, new AuthenticationModel(), authenticationMapper)
    }

    postAuthToken(appId: string, kind: string, param: string, authRule: string): Observable<ResponseModel<AuthenticationModel>> {

        let username: string = this.globalSetting.getAuthUsername(appId);

        let body: HttpParams = new HttpParams({
            fromObject: {
                'grant_type': 'password',
                'username': username,
                'password': appId,
                'appId': appId,
                'kind': kind,
                'params': param,
                'authRule': authRule
            }
        });

        return super.postAuthResults(body);
    }

}