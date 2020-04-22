import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ExceptionHandlerHelper } from '../../helpers/exception-handler.helper';
import { GlobalSetting } from '../../settings/global.setting';
import { BaseService } from '../base.service';
import { Observable } from 'rxjs';
import { ResponseModel } from '../../models/commons/response.model';
import { FascicleFolderModel } from '../../models/fascicles/fascicle-folder.model';
import { FascicleFolderMapper } from '../../mappers/fascicles/fascicle-folder.mapper';

@Injectable({
    providedIn: 'root'
})
export class FascicleFolderService extends BaseService<FascicleFolderModel> {

    constructor(http: HttpClient, exceptionHandler: ExceptionHandlerHelper, globalSetting: GlobalSetting, private fascicleFolderMapper: FascicleFolderMapper) {
        super(http, exceptionHandler, globalSetting, new FascicleFolderModel(), fascicleFolderMapper)
    }

    getNextFascicleFolders(id: string): Observable<ResponseModel<FascicleFolderModel>> {
        let odataFunction: string = `/FascicleService.GetNextFascicleFolders(id=${id})`;

        return super.getResults(odataFunction, true);
    }

    getFascicleDocumentUnitFromFolder(id: string): Observable<any> {
        let odataFunction: string = `/FascicleService.GetFascicleDocumentUnitFromFolder(id=${id})`;

        return super.getAnyResults(odataFunction, true);
    }

    getFascicleDocumentsFromFolder(id: string): Observable<any> {
        let odataFunction: string = `/FascicleService.GetFascicleDocumentFromFolder(id=${id})`;

        return super.getAnyResults(odataFunction, true);
    }
}
