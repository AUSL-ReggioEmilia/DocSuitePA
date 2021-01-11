import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ExceptionHandlerHelper } from '../../helpers/exception-handler.helper';
import { GlobalSetting } from '../../settings/global.setting';
import { BaseService } from '../base.service';
import { DossierModel } from '../../models/dossiers/dossier.model';
import { DossierMapper } from '../../mappers/dossiers/dossier.mapper';
import { Observable } from 'rxjs';
import { ResponseModel } from '../../models/commons/response.model';

@Injectable({
  providedIn: 'root'
})
export class DossierService extends BaseService<DossierModel> {

    constructor(http: HttpClient, exceptionHandler: ExceptionHandlerHelper, globalSetting: GlobalSetting, private dossierMapper: DossierMapper) {
        super(http, exceptionHandler, globalSetting, new DossierModel(), dossierMapper)
    }

    getDossierById(id: string): Observable<ResponseModel<DossierModel>> {
        let odataFunction: string = `/DossierService.GetDossierById(id=${id})`;

        return super.getResults(odataFunction,true);
    }

    getDossierByYearAndNumber(year: number, number: number): Observable<ResponseModel<DossierModel>> {
        let odataFunction: string = `/DossierService.GetDossierByYearAndNumber(year=${year},number=${number})`;

        return super.getResults(odataFunction,true);
    }

    getNextDossierFolders(id: string): Observable<ResponseModel<any>> {
        let odataFunction: string = `DossierFolders/DossierService.GetNextDossierFolders(id=${id})`;

        return super.getResults(odataFunction, true);
    }
}
