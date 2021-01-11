import { Injectable } from '@angular/core'; 
import { HttpClient, HttpResponse } from '@angular/common/http'; 
import { Observable } from 'rxjs';

import { BaseService } from '../base.service';
import { PECMailModel } from '../../models/pec-mails/pec-mail.model';  
import { PECMailMapper } from '../../mappers/pec-mails/pec-mail.mapper';  
import { ExceptionHandlerHelper } from '../../helpers/exception-handler.helper';
import { PECDirectionType } from '../../models/pec-mails/pec-direction.type';
import { ResponseModel } from '../../models/commons/response.model';
import { GlobalSetting } from '../../settings/global.setting';

@Injectable()
export class PECMailService extends BaseService<PECMailModel>{

    constructor(http: HttpClient, exceptionHandler: ExceptionHandlerHelper, globalSetting: GlobalSetting, private pecMailMapper: PECMailMapper) {
        super(http, exceptionHandler, globalSetting, new PECMailModel(), pecMailMapper)
    }

    getPECMails(year: number, number: number, direction: PECDirectionType): Observable<ResponseModel<PECMailModel>> {
        let odataFunction: string = '/PECMailService.GetProtocolPECMails(year='.concat(year.toString(), ',number=', number.toString(), ',direction=', direction.toString(), ')');
        return super.getAuthResults(odataFunction);
    }

}