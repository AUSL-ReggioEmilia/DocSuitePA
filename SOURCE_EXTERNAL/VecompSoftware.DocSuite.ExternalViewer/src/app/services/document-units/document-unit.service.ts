import { Injectable } from '@angular/core'; 
import { HttpClient, HttpResponse } from '@angular/common/http'; 
import { Observable } from 'rxjs';

import { BaseService } from '../base.service';
import { DocumentUnitModel } from '../../models/document-units/document-unit.model'; 
import { DocumentUnitMapper } from '../../mappers/document-units/document-unit.mapper'; 
import { ExceptionHandlerHelper } from '../../helpers/exception-handler.helper';
import { ResponseModel } from '../../models/commons/response.model';
import { GlobalSetting } from '../../settings/global.setting';

@Injectable()
export class DocumentUnitService extends BaseService<DocumentUnitModel>{

    constructor(http: HttpClient, exceptionHandler: ExceptionHandlerHelper, globalSetting: GlobalSetting, private documentUnitMapper: DocumentUnitMapper) {
        super(http, exceptionHandler, globalSetting, new DocumentUnitModel(), documentUnitMapper)
    }

    getFascicleDocumentUnits(id: string, filter: string): Observable<ResponseModel<DocumentUnitModel>>{
        let odataFunction: string;
        odataFunction = '/DocumentUnitService.GetFascicleDocumentUnits(fascicleId='.concat(id, ',filter=\'', filter, '\')');
        return super.getResults(odataFunction);
    }
}