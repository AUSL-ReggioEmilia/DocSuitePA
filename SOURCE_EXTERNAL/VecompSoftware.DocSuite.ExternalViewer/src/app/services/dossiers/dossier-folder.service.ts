import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ExceptionHandlerHelper } from '../../helpers/exception-handler.helper';
import { GlobalSetting } from '../../settings/global.setting';
import { BaseService } from '../base.service';
import { Observable } from 'rxjs';
import { ResponseModel } from '../../models/commons/response.model';
import { DossierFolderModel } from '../../models/dossiers/dossier-folder.model';
import { DossierFolderMapper } from '../../mappers/dossiers/dossier-folder.mapper';

@Injectable({
    providedIn: 'root'
})
export class DossierFolderService extends BaseService<DossierFolderModel> {

    constructor(http: HttpClient, exceptionHandler: ExceptionHandlerHelper, globalSetting: GlobalSetting, private dossierFolderMapper: DossierFolderMapper) {
        super(http, exceptionHandler, globalSetting, new DossierFolderModel(), dossierFolderMapper)
    }

    getNextDossierFolders(id: string): Observable<ResponseModel<DossierFolderModel>> {
        let odataFunction: string = `/DossierService.GetNextDossierFolders(id=${id})`;

        return super.getResults(odataFunction, true);
    }

    hasChildren(id: string): Observable<boolean> {
        let odataFunction: string = `/DossierService.HasChildren(id=${id})`;

        return super.getBoolResult(odataFunction, true);
    }
}
