import { Injectable } from '@angular/core';
import { Http } from '@angular/http';  
import { Observable } from 'rxjs';

import { BaseService } from './base.service';
import { ResolutionModel } from '../models/resolution.model'; 
import { ResolutionType } from '../models/resolution.type'; 
import { ResolutionMapper } from '../mappers/resolution.mapper'; 
import { ODataResultModel } from '../models/odata-result.model'; 

@Injectable()
export class ResolutionService extends BaseService<ResolutionModel> {

    constructor(http: Http, private resolutionMapper: ResolutionMapper) {
        super(http, resolutionMapper)
    }

    getExecutiveResolutions(type: ResolutionType, skipValue: number, topValue: number, year: string, number: string, subject: string, adoptionDate: string, proposer: string): Observable<ODataResultModel> {
        let odataFilter: string = 'Resolution/ResolutionService.GetExecutiveResolutions(skip='.concat(skipValue.toString(), ',top=', topValue.toString(),
            ',type=VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions.ResolutionType\'', type.toString(), '\',year=', !!year ? year.toString() : null, ',number=', !!number ? number.toString() : null,
            ',subject=\'', !!subject ? subject : '', '\',adoptionDate=\'', !!adoptionDate ? adoptionDate : '', '\',proposer=\'', !!proposer ? proposer : '','\')');

        odataFilter = odataFilter.concat('?$orderby=EffectivenessDate desc,InclusiveNumber desc');

        return super.getResults(odataFilter);
    }

    getPublishedResolutions(type: ResolutionType, skipValue: number, topValue: number, year: string, number: string, subject: string, adoptionDate: string, proposer: string): Observable<ODataResultModel> {
        let odataFilter: string = 'Resolution/ResolutionService.GetPublishedResolutions(skip='.concat(skipValue.toString(), ',top=', topValue.toString(),
            ',type=VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions.ResolutionType\'', type.toString(), '\',year=', !!year ? year.toString() : null, ',number=', !!number ? number.toString() : null,
            ',subject=\'', !!subject ? subject : '', '\',adoptionDate=\'', !!adoptionDate ? adoptionDate : '', '\',proposer=\'', !!proposer ? proposer : '', '\')');

        odataFilter = odataFilter.concat('?$orderby=PublishingDate desc,InclusiveNumber desc');

        return super.getResults(odataFilter);
    }

    getOnlinePublishedResolutions(type: ResolutionType, sorting: string): Observable<ODataResultModel> {
        let odataFilter: string = 'Resolution/ResolutionService.GetOnlinePublishedResolutions(type=VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions.ResolutionType\''.concat(type.toString(), '\')');

        odataFilter = odataFilter.concat('?$orderby=InclusiveNumber ', sorting,',PublishingDate ', sorting);

        return super.getResults(odataFilter);
    }

    countExecutiveResolutions(type: ResolutionType, skipValue: number, topValue: number, year: string, number: string, subject: string, adoptionDate: string, proposer: string): Observable<number> {
        let odataFilter: string = 'Resolution/ResolutionService.GetExecutiveResolutionsCount(type=VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions.ResolutionType\''.concat(type.toString(),
            '\',year=', !!year ? year.toString() : null, ',number=', !!number ? number.toString() : null, ',subject=\'', !!subject ? subject : '', '\',adoptionDate=\'', !!adoptionDate ? adoptionDate : '',
            '\',proposer=\'', !!proposer ? proposer : '', '\')');

        return super.getCountResults(odataFilter);
    }

    countPublishedResolutions(type: ResolutionType, skipValue: number, topValue: number, year: string, number: string, subject: string, adoptionDate: string, proposer: string): Observable<number> {
        let odataFilter: string = 'Resolution/ResolutionService.GetPublishedResolutionsCount(type=VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions.ResolutionType\''.concat(type.toString(),
            '\',year=', !!year ? year.toString() : null, ',number=', !!number ? number.toString() : null, ',subject=\'', !!subject ? subject : '', '\',adoptionDate=\'', !!adoptionDate ? adoptionDate : '',
            '\',proposer=\'', !!proposer ? proposer : '', '\')');

        return super.getCountResults(odataFilter);
    }
}