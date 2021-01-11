import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';

import { BaseService } from '../base.service';
import { ProtocolModel } from '../../models/protocols/protocol.model';
import { ProtocolMapper } from '../../mappers/protocols/protocol.mapper';
import { ExceptionHandlerHelper } from '../../helpers/exception-handler.helper';
import { ResponseModel } from '../../models/commons/response.model';
import { GlobalSetting } from '../../settings/global.setting';

@Injectable()
export class ProtocolService extends BaseService<ProtocolModel>{

    constructor(http: HttpClient, exceptionHandler: ExceptionHandlerHelper, globalSetting: GlobalSetting, private protocolMapper: ProtocolMapper) {
        super(http, exceptionHandler, globalSetting, new ProtocolModel(), protocolMapper)
    }

    getProtocolOutgoingPECCount(year: number, number: number): Observable<number> {
        let odataFunction: string = '/ProtocolService.GetProtocolOutgoingPECCount(year='.concat(year.toString(), ',number=', number.toString(), ')');
        return super.getAuthCountResults(odataFunction);
    }

    getProtocolIngoingPECCount(year: number, number: number): Observable<number> {
        let odataFunction: string = '/ProtocolService.GetProtocolIngoingPECCount(year='.concat(year.toString(), ',number=', number.toString(), ')');
        return super.getAuthCountResults(odataFunction);
    }

    getProtocolSummary(id: string): Observable<ResponseModel<ProtocolModel>>;
    getProtocolSummary(year: number, number: number): Observable<ResponseModel<ProtocolModel>>;
    getProtocolSummary(idOrYear: string | number, number?: number): Observable<ResponseModel<ProtocolModel>> {

        let odataFunction: string;
        if (typeof idOrYear == 'string') {
            odataFunction = '/ProtocolService.GetProtocolSummary(id='.concat(idOrYear, ')?$expand=Category,Container,Sectors($expand=Children($levels=5)),Contacts($expand=Children($levels=max))');
        }
        else {
            odataFunction = '/ProtocolService.GetProtocolSummary(year='.concat(idOrYear.toString(), ',number=', number.toString(), ')?$expand=Category,Container,Sectors($expand=Children($levels=max)),Contacts($expand=Children($levels=max))');
        }
        return super.getAuthResults(odataFunction);
    }
}