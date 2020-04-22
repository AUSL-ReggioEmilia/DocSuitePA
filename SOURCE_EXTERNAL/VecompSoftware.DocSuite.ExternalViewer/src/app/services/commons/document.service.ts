import { Injectable } from '@angular/core'; 
import { HttpClient, HttpResponse } from '@angular/common/http'; 
import { Observable } from 'rxjs';

import { BaseService } from '../base.service';
import { DocumentModel } from '../../models/commons/document.model';  
import { DocumentMapper } from '../../mappers/commons/document.mapper';  
import { ExceptionHandlerHelper } from '../../helpers/exception-handler.helper';
import { ResponseModel } from '../../models/commons/response.model';
import { GlobalSetting } from '../../settings/global.setting';

@Injectable()
export class DocumentService extends BaseService<DocumentModel>{

    constructor(http: HttpClient, exceptionHandler: ExceptionHandlerHelper, globalSetting: GlobalSetting, private documentMapper: DocumentMapper) {
        super(http, exceptionHandler, globalSetting, new DocumentModel(), documentMapper)
    }

    getDocuments(id: string): Observable<ResponseModel<DocumentModel>> {
        let odataFunction: string = `/DocumentUnitService.GetDocuments(uniqueId=${id},workflowArchiveChainId=null)`;
        return super.getAuthResults(odataFunction);
    }

    getFascicleMiscellaneous(id: string): Observable<ResponseModel<DocumentModel>> {
        let odataFunction: string = `/DocumentUnitService.GetDocuments(uniqueId=${id},workflowArchiveChainId=null)`;
        return super.getAuthResults(odataFunction);
    }

    getDocumentsByArchiveChain(id: string): Observable<ResponseModel<DocumentModel>> {
        let odataFunction: string = '/DocumentUnitService.GetDocumentsByArchiveChain(idArchiveChain='.concat(id, ')');
        return super.getAuthResults(odataFunction);
    }

}