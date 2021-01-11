import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from '../base.service';
import { FascicleModel } from '../../models/fascicles/fascicle.model';
import { FascicleMapper } from '../../mappers/fascicles/fascicle.mapper';
import { ExceptionHandlerHelper } from '../../helpers/exception-handler.helper';
import { ResponseModel } from '../../models/commons/response.model';
import { GlobalSetting } from '../../settings/global.setting';

@Injectable()
export class FascicleService extends BaseService<FascicleModel>{

    constructor(http: HttpClient, exceptionHandler: ExceptionHandlerHelper, globalSetting: GlobalSetting, private fascicleMapper: FascicleMapper) {
        super(http, exceptionHandler, globalSetting, new FascicleModel(), fascicleMapper)
    }

    getFascicleSummary(id: string): Observable<ResponseModel<FascicleModel>> {
        let odataFunction: string = '/FascicleService.GetFascicleSummary(uniqueId='.concat(id, ')?$expand=Category,Contacts($expand=Children($levels=max))');

        return super.getResults(odataFunction,true);
    }

    getFascicleSummaryByTitle(title: string): Observable<ResponseModel<FascicleModel>> {
        let odataFunction: string = `/FascicleService.GetFascicleSummaryByTitle(title='${title}')?$expand=Category,Contacts($expand=Children($levels=max))`;

        return super.getResults(odataFunction, true);
    }

    getFascicleDocumentUnits(id: string, filter: string): Observable<ResponseModel<FascicleModel>> {
        let odataFunction: string = '/FascicleService.GetFascicleDocumentUnits(uniqueId='.concat(id, ',filter=\'', filter, '\' )?$expand=Category,DocumentUnits,Contacts($expand=Children($levels=max))');

        return super.getResults(odataFunction,true);
    }

    getFascicleDocuments(id: string): Observable<ResponseModel<FascicleModel>> {
        let odataFunction: string = '/FascicleService.GetFascicleDocuments(uniqueId='.concat(id, ')?$expand=Documents,Category,Contacts($expand=Children($levels=max))');

        return super.getResults(odataFunction,true);
    }

    getDossierFascicleDocumentUnits(id: string, filter: string = ""): Observable<ResponseModel<FascicleModel>> {
        let odataFunction: string = `/FascicleService.GetFascicleDocumentUnits(uniqueId=${id}, filter='${filter}')?$expand=FascicleFolders($expand=FascicleDocumentUnits)`;

        return super.getResults(odataFunction,true);
    }
}